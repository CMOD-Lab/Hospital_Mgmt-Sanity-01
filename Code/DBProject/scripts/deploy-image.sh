#!/bin/bash
set -e
set -o pipefail

# =============================================================
# deploy-image.sh – Deploy Clinic Management System to AWS EKS
# =============================================================

NAMESPACE="clinic-management-system"
APP_NAME="clinic-management-system"
K8S_DIR="$(cd "$(dirname "$0")/.." && pwd)/kubernetes"

echo "=============================================="
echo " Clinic Management System – Deploy to EKS"
echo "=============================================="
echo ""

# ── Collect deployment inputs ─────────────────────────────────
read -rp "Enter AWS region (e.g. us-east-1): " AWS_REGION
if [ -z "$AWS_REGION" ]; then
  echo "ERROR: AWS region is required."
  exit 1
fi

read -rp "Enter EKS cluster name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS cluster name is required."
  exit 1
fi

read -rp "Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/clinic-management-system:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker image URI is required."
  exit 1
fi

echo ""
echo "── Application environment variables ──────────────────"
echo "   (Press Enter to skip any optional variable)"
echo ""

read -rp "Enter DB_CONNECTION_STRING (SQL Server connection string): " DB_CONNECTION_STRING
read -rp "Enter REDIS_HOST (ElastiCache Redis host): " REDIS_HOST
read -rp "Enter REDIS_PORT (default: 6379): " REDIS_PORT_INPUT
REDIS_PORT="${REDIS_PORT_INPUT:-6379}"
read -rsp "Enter REDIS_ACCESS_KEY (Redis auth key, if enabled): " REDIS_ACCESS_KEY
echo ""

echo ""
echo "── Configuring kubectl for EKS cluster ─────────────────"
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo "── Verifying cluster connectivity ──────────────────────"
kubectl cluster-info || { echo "ERROR: Cannot connect to EKS cluster."; exit 1; }

echo ""
echo "── Updating Kubernetes manifests ───────────────────────"

# Work on copies to avoid modifying originals
TMP_DIR=$(mktemp -d)
cp "$K8S_DIR"/*.yaml "$TMP_DIR/"

# Replace placeholders using pipe delimiter
sed -i 's|{{IMAGE_URI}}|'"$IMAGE_URI"'|g'                         "$TMP_DIR/deployment.yaml"
sed -i 's|{{DB_CONNECTION_STRING}}|'"$DB_CONNECTION_STRING"'|g'   "$TMP_DIR/deployment.yaml"
sed -i 's|{{REDIS_HOST}}|'"$REDIS_HOST"'|g'                       "$TMP_DIR/deployment.yaml"
sed -i 's|{{REDIS_PORT}}|'"$REDIS_PORT"'|g'                       "$TMP_DIR/deployment.yaml"
sed -i 's|{{REDIS_ACCESS_KEY}}|'"$REDIS_ACCESS_KEY"'|g'           "$TMP_DIR/deployment.yaml"

echo ""
echo "── Applying Kubernetes manifests ───────────────────────"
echo "  1/4 Applying namespace..."
kubectl apply -f "$TMP_DIR/namespace.yaml"

echo "  2/4 Applying deployment..."
kubectl apply -f "$TMP_DIR/deployment.yaml"

echo "  3/4 Applying service..."
kubectl apply -f "$TMP_DIR/service.yaml"

echo "  4/4 Applying ingress..."
kubectl apply -f "$TMP_DIR/ingress.yaml"

echo ""
echo "── Waiting for rollout to complete ─────────────────────"
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s

echo ""
echo "── Verifying deployed resources ────────────────────────"
kubectl get pods,svc,ingress -n "$NAMESPACE"

echo ""
echo "── Application access URL ──────────────────────────────"
INGRESS_HOST=$(kubectl get ingress "$APP_NAME"-ingress -n "$NAMESPACE" \
  -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "<pending>")
echo "  http://$INGRESS_HOST"

echo ""
echo "=============================================="
echo " Deployment complete!"
echo " Namespace : $NAMESPACE"
echo " Image     : $IMAGE_URI"
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"

# Cleanup temp files
rm -rf "$TMP_DIR"
