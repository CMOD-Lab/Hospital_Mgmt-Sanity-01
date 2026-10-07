#!/bin/bash
set -e
set -o pipefail

# ============================================================
# deploy-image.sh - Deploy to AWS EKS
# Application: Clinic Management System (DBProject)
# Platform: Linux/macOS
# Target: AWS EKS (Windows node groups)
# ============================================================

NAMESPACE="dbproject"
APP_NAME="dbproject"
K8S_DIR="$(dirname "$0")/../kubernetes"

echo "=============================================="
echo " Clinic Management System - Deploy to EKS"
echo "=============================================="
echo ""

# Prompt for AWS and cluster details
read -p "Enter AWS region (e.g. us-east-1): " AWS_REGION
if [ -z "$AWS_REGION" ]; then
  echo "AWS region is required. Exiting."
  exit 1
fi

read -p "Enter EKS cluster name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "EKS cluster name is required. Exiting."
  exit 1
fi

read -p "Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/dbproject:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "Image URI is required. Exiting."
  exit 1
fi

echo ""
echo "--- Optional: Application Environment Variables ---"
echo "Press Enter to skip any variable."
echo ""

read -p "Enter DB_CONNECTION_STRING (SQL Server connection string): " DB_CONNECTION_STRING
read -p "Enter REDIS_CONNECTION_STRING (Redis/ElastiCache connection string): " REDIS_CONNECTION_STRING
read -p "Enter APPINSIGHTS_INSTRUMENTATIONKEY (Application Insights key): " APPINSIGHTS_INSTRUMENTATIONKEY

echo ""
echo "Configuring kubectl for EKS cluster: ${CLUSTER_NAME} in ${AWS_REGION}..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "Cannot connect to cluster. Exiting."; exit 1; }

echo ""
echo "Updating Kubernetes manifests with deployment values..."

# Work on copies to avoid modifying originals
cp -r "$K8S_DIR" /tmp/dbproject-k8s-deploy

# Replace image placeholder
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g" /tmp/dbproject-k8s-deploy/deployment.yaml

# Replace environment variable placeholders
sed -i "s|{{DB_CONNECTION_STRING}}|${DB_CONNECTION_STRING}|g" /tmp/dbproject-k8s-deploy/deployment.yaml
sed -i "s|{{REDIS_CONNECTION_STRING}}|${REDIS_CONNECTION_STRING}|g" /tmp/dbproject-k8s-deploy/deployment.yaml
sed -i "s|{{APPINSIGHTS_INSTRUMENTATIONKEY}}|${APPINSIGHTS_INSTRUMENTATIONKEY}|g" /tmp/dbproject-k8s-deploy/deployment.yaml

echo ""
echo "Applying Kubernetes manifests..."

echo "  [1/4] Applying namespace..."
kubectl apply -f /tmp/dbproject-k8s-deploy/namespace.yaml

echo "  [2/4] Applying deployment..."
kubectl apply -f /tmp/dbproject-k8s-deploy/deployment.yaml

echo "  [3/4] Applying service..."
kubectl apply -f /tmp/dbproject-k8s-deploy/service.yaml

echo "  [4/4] Applying ingress..."
kubectl apply -f /tmp/dbproject-k8s-deploy/ingress.yaml

echo ""
echo "Waiting for deployment rollout to complete..."
kubectl rollout status deployment/${APP_NAME} -n ${NAMESPACE} --timeout=300s

echo ""
echo "Verifying deployed resources..."
kubectl get pods,svc,ingress -n ${NAMESPACE}

echo ""
echo "Fetching application URL from ingress..."
INGRESS_HOST=$(kubectl get ingress ${APP_NAME}-ingress -n ${NAMESPACE} -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
  echo "Application URL: http://${INGRESS_HOST}"
else
  echo "Ingress hostname is still provisioning. Run the following to check:"
  echo "  kubectl get ingress ${APP_NAME}-ingress -n ${NAMESPACE}"
fi

echo ""
echo "=============================================="
echo " Deployment complete!"
echo " Namespace: ${NAMESPACE}"
echo " Image: ${IMAGE_URI}"
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/${APP_NAME} -n ${NAMESPACE}"

# Cleanup temp files
rm -rf /tmp/dbproject-k8s-deploy
