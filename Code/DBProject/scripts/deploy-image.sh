#!/bin/bash
# ============================================================
# deploy-image.sh – Deploy Clinic Management System to AWS EKS
# Application : ASP.NET Web Forms (.NET Framework 4.5.2)
# Platform    : AWS EKS (Windows node groups)
# ============================================================
set -e
set -o pipefail

APP_NAME="clinic-management-system"
NAMESPACE="clinic-management-system"
K8S_DIR="$(dirname "$0")/../kubernetes"

echo "=============================================="
echo " Clinic Management System – Deploy to EKS"
echo "=============================================="
echo ""

# ── Prompt for AWS / EKS configuration ───────────────────────
read -rp "Enter AWS Region (e.g. us-east-1): " AWS_REGION
if [ -z "$AWS_REGION" ]; then
    echo "ERROR: AWS Region is required."
    exit 1
fi

read -rp "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
    echo "ERROR: EKS Cluster Name is required."
    exit 1
fi

read -rp "Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/clinic-management-system:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
    echo "ERROR: Docker image URI is required."
    exit 1
fi

echo ""
echo "── Application Environment Variables ──────────────────"
echo "The following environment variables are required by the application."
echo "Press Enter to skip (you can configure them manually in Kubernetes Secrets)."
echo ""

read -rsp "Enter DB_CONNECTION_STRING value (SQL Server connection string): " DB_CONNECTION_STRING_VAL
echo ""
read -rsp "Enter REDIS_CONNECTION_STRING value (Redis connection string): " REDIS_CONNECTION_STRING_VAL
echo ""
echo ""

# ── Configure kubectl for EKS ────────────────────────────────
echo "Configuring kubectl for EKS cluster '$CLUSTER_NAME' in region '$AWS_REGION'..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"
echo "kubectl configured successfully."
echo ""

# ── Verify cluster connectivity ──────────────────────────────
echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to EKS cluster. Check your credentials and cluster name."; exit 1; }
echo ""

# ── Create Kubernetes Secrets if values were provided ────────
if [ -n "$DB_CONNECTION_STRING_VAL" ]; then
    echo "Creating/updating clinic-db-secret..."
    kubectl create secret generic clinic-db-secret \
        --namespace="$NAMESPACE" \
        --from-literal=connectionString="$DB_CONNECTION_STRING_VAL" \
        --dry-run=client -o yaml | kubectl apply -f -
    echo "clinic-db-secret applied."
fi

if [ -n "$REDIS_CONNECTION_STRING_VAL" ]; then
    echo "Creating/updating clinic-redis-secret..."
    kubectl create secret generic clinic-redis-secret \
        --namespace="$NAMESPACE" \
        --from-literal=connectionString="$REDIS_CONNECTION_STRING_VAL" \
        --dry-run=client -o yaml | kubectl apply -f -
    echo "clinic-redis-secret applied."
fi
echo ""

# ── Replace {{IMAGE_URI}} placeholder in deployment manifest ─
echo "Updating deployment manifest with image URI..."
# Work on a temporary copy to avoid modifying the source file
cp "$K8S_DIR/deployment.yaml" /tmp/deployment-deploy.yaml
sed -i 's|{{IMAGE_URI}}|'"$IMAGE_URI"'|g' /tmp/deployment-deploy.yaml
echo "Manifest updated."
echo ""

# ── Apply Kubernetes manifests in order ──────────────────────
echo "Applying namespace..."
kubectl apply -f "$K8S_DIR/namespace.yaml"

echo "Applying deployment..."
kubectl apply -f /tmp/deployment-deploy.yaml

echo "Applying service..."
kubectl apply -f "$K8S_DIR/service.yaml"

echo "Applying ingress..."
kubectl apply -f "$K8S_DIR/ingress.yaml"

echo ""
echo "Waiting for deployment rollout to complete..."
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s

echo ""
echo "── Deployed Resources ──────────────────────────────────"
kubectl get pods,svc,ingress -n "$NAMESPACE"

echo ""
echo "── Application Access URL ──────────────────────────────"
INGRESS_HOST=$(kubectl get ingress "$APP_NAME-ingress" -n "$NAMESPACE" -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
    echo "Application URL: http://$INGRESS_HOST"
else
    echo "Ingress hostname is still provisioning. Run the following to check:"
    echo "  kubectl get ingress -n $NAMESPACE"
fi

echo ""
echo "=============================================="
echo " Deployment complete!"
echo " Namespace : $NAMESPACE"
echo " Image     : $IMAGE_URI"
echo "=============================================="
echo ""
echo "── Rollback Instructions ───────────────────────────────"
echo "To rollback to the previous version:"
echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"
echo ""
echo "To check rollout history:"
echo "  kubectl rollout history deployment/$APP_NAME -n $NAMESPACE"

# ── Cleanup temp file ────────────────────────────────────────
rm -f /tmp/deployment-deploy.yaml
