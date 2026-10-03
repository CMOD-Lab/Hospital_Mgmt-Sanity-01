#!/bin/bash
set -e

# =============================================================
# build-push.sh - Build and push Docker image for HospitalMgmtApp
# ASP.NET Web Forms (.NET Framework 4.5.2) on Amazon EKS
# =============================================================

PROJECT_NAME="hospitalmgmtapp"
DOCKERFILE_PATH="Code/DBProject/Dockerfile"
BUILD_CONTEXT="Code/DBProject"

echo "=============================================="
echo " HospitalMgmtApp - Docker Build & Push Script"
echo "=============================================="
echo ""

# Sanitize project name: lowercase, replace non-alphanumeric with hyphens, trim hyphens
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

# Prompt for image tag
read -p "Enter image tag [default: latest]: " IMAGE_TAG_INPUT
IMAGE_TAG=$(echo "$IMAGE_TAG_INPUT" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
if [ -z "$IMAGE_TAG" ]; then
  IMAGE_TAG="latest"
fi

echo ""
echo "Select container registry:"
echo "  1. AWS ECR (Elastic Container Registry)"
echo "  2. Docker Hub"
read -p "Enter choice [1 or 2]: " REGISTRY_CHOICE

echo ""

if [ "$REGISTRY_CHOICE" = "1" ]; then
  # ---- AWS ECR ----
  read -p "Enter AWS Region (e.g. us-east-1): " AWS_REGION
  read -p "Enter AWS Account ID (12-digit): " AWS_ACCOUNT_ID

  ECR_REPO="$IMAGE_NAME"
  REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
  FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to AWS ECR..."
  aws ecr get-login-password --region "$AWS_REGION" | \
    docker login --username AWS --password-stdin "$REGISTRY_URL"

  echo ""
  echo "Checking if ECR repository exists..."
  aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || \
    (echo "Creating ECR repository: $ECR_REPO" && \
     aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION")

elif [ "$REGISTRY_CHOICE" = "2" ]; then
  # ---- Docker Hub ----
  read -p "Enter Docker Hub username: " DOCKER_USERNAME
  read -s -p "Enter Docker Hub password/token: " DOCKER_PASSWORD
  echo ""

  REGISTRY_URL="docker.io"
  FULL_IMAGE_NAME="${DOCKER_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to Docker Hub..."
  echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin

else
  echo "ERROR: Invalid choice. Please enter 1 or 2."
  exit 1
fi

echo ""
echo "Building Docker image..."
echo "  Image : $FULL_IMAGE_NAME"
echo "  File  : $DOCKERFILE_PATH"
echo "  Context: $BUILD_CONTEXT"
echo ""

docker build -f "$DOCKERFILE_PATH" -t "$FULL_IMAGE_NAME" "$BUILD_CONTEXT"

echo ""
echo "Pushing image to registry..."
docker push "$FULL_IMAGE_NAME"

echo ""
echo "=============================================="
echo " Build and push completed successfully!"
echo " Image: $FULL_IMAGE_NAME"
echo "=============================================="
