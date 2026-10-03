#!/bin/bash
set -e
set -o pipefail

# =============================================================
# deploy-image.sh - Deploy HospitalMgmtApp to AWS EKS
# ASP.NET Web Forms (.NET Framework 4.5.2)
# Namespace: hospitalmgmtapp
# =============================================================

APP_NAME="hospitalmgmtapp"
K8S_DIR="$(dirname "$0")/../kubernetes"

echo "=============================================="
echo " HospitalMgmtApp - AWS EKS Deployment Script"
echo "=============================================="
echo ""

# ---- Collect deployment inputs ----
read -p "Enter AWS Region (e.g. us-east-1): " AWS_REGION
if [ -z "$AWS_REGION" ]; then
  echo "ERROR: AWS Region is required."
  exit 1
fi

read -p "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS Cluster Name is required."
  exit 1
fi

read -p "Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/hospitalmgmtapp:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker image URI is required."
  exit 1
fi

echo ""
echo "---- Application Environment Variables ----"
read -p "Enter DB_CONNECTION_STRING (SQL Server connection string, or press Enter to skip): " DB_CONNECTION_STRING
read -p "Enter REDIS_CONNECTION_STRING (ElastiCache Redis endpoint, or press Enter to skip): " REDIS_CONNECTION_STRING

# ---- Configure kubectl for EKS ----
echo ""
echo "Configuring kubectl for EKS cluster: $CLUSTER_NAME in $AWS_REGION..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo ""
echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to EKS cluster. Check your credentials and cluster name."; exit 1; }

# ---- Update Kubernetes manifests with actual values ----
echo ""
echo "Updating Kubernetes manifests with deployment values..."

# Use pipe delimiter in sed to safely handle URIs containing forward slashes
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g" "$K8S_DIR/deployment.yaml"

if [ -n "$DB_CONNECTION_STRING" ]; then
  sed -i "s|{{DB_CONNECTION_STRING}}|${DB_CONNECTION_STRING}|g" "$K8S_DIR/deployment.yaml"
else
  sed -i "s|{{DB_CONNECTION_STRING}}||g" "$K8S_DIR/deployment.yaml"
fi

if [ -n "$REDIS_CONNECTION_STRING" ]; then
  sed -i "s|{{REDIS_CONNECTION_STRING}}|${REDIS_CONNECTION_STRING}|g" "$K8S_DIR/deployment.yaml"
else
  sed -i "s|{{REDIS_CONNECTION_STRING}}||g" "$K8S_DIR/deployment.yaml"
fi

# ---- Apply Kubernetes manifests in order ----
echo ""
echo "Applying Kubernetes manifests..."

echo "  [1/4] Applying namespace..."
kubectl apply -f "$K8S_DIR/namespace.yaml"

echo "  [2/4] Applying deployment..."
kubectl apply -f "$K8S_DIR/deployment.yaml"

echo "  [3/4] Applying service..."
kubectl apply -f "$K8S_DIR/service.yaml"

echo "  [4/4] Applying ingress..."
kubectl apply -f "$K8S_DIR/ingress.yaml"

# ---- Wait for rollout ----
echo ""
echo "Waiting for deployment rollout to complete..."
kubectl rollout status deployment/"$APP_NAME" -n "$APP_NAME" --timeout=300s

# ---- Verify resources ----
echo ""
echo "Verifying deployed resources..."
kubectl get pods,svc,ingress -n "$APP_NAME"

# ---- Display application URL ----
echo ""
echo "Fetching application ingress URL..."
INGRESS_HOST=$(kubectl get ingress "${APP_NAME}-ingress" -n "$APP_NAME" -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
  echo "  Application URL: http://$INGRESS_HOST"
else
  echo "  Ingress hostname is still provisioning. Run the following to check:"
  echo "  kubectl get ingress ${APP_NAME}-ingress -n ${APP_NAME}"
fi

echo ""
echo "=============================================="
echo " Deployment completed successfully!"
echo " Namespace : $APP_NAME"
echo " Image     : $IMAGE_URI"
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/$APP_NAME -n $APP_NAME"
