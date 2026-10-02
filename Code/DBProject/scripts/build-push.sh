#!/bin/bash
# ============================================================
# build-push.sh – Build and push Clinic Management System Docker image
# Application : ASP.NET Web Forms (.NET Framework 4.5.2)
# Target      : AWS ECR or Docker Hub
# ============================================================
set -e
set -o pipefail

PROJECT_NAME="clinic-management-system"
DOCKERFILE_PATH="Code/DBProject/Dockerfile"

echo "=============================================="
echo " Clinic Management System – Build & Push"
echo "=============================================="
echo ""

# ── Tag sanitisation ─────────────────────────────────────────
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

# ── Prompt for image tag ─────────────────────────────────────
read -rp "Enter image tag [latest]: " INPUT_TAG
INPUT_TAG=$(echo "${INPUT_TAG:-latest}" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
IMAGE_TAG="${INPUT_TAG:-latest}"
echo "Using image tag: $IMAGE_TAG"
echo ""

# ── Registry selection ───────────────────────────────────────
echo "Select container registry:"
echo "  1) AWS ECR"
echo "  2) Docker Hub"
read -rp "Enter choice [1 or 2]: " REGISTRY_CHOICE
echo ""

if [ "$REGISTRY_CHOICE" = "1" ]; then
    # ── AWS ECR ──────────────────────────────────────────────
    read -rp "Enter AWS Region (e.g. us-east-1): " AWS_REGION
    read -rp "Enter AWS Account ID: " AWS_ACCOUNT_ID
    read -rp "Enter ECR repository name [$IMAGE_NAME]: " ECR_REPO_INPUT
    ECR_REPO="${ECR_REPO_INPUT:-$IMAGE_NAME}"

    REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
    FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"

    echo "Logging in to AWS ECR..."
    aws ecr get-login-password --region "$AWS_REGION" | \
        docker login --username AWS --password-stdin "$REGISTRY_URL"
    echo "ECR login successful."

    # Auto-create ECR repository if it does not exist
    echo "Checking ECR repository '$ECR_REPO'..."
    aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || \
        aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
    echo "ECR repository ready."

elif [ "$REGISTRY_CHOICE" = "2" ]; then
    # ── Docker Hub ───────────────────────────────────────────
    read -rp "Enter Docker Hub username: " DOCKER_USERNAME
    read -rsp "Enter Docker Hub password/token: " DOCKER_PASSWORD
    echo ""
    read -rp "Enter Docker Hub repository name [$IMAGE_NAME]: " DOCKER_REPO_INPUT
    DOCKER_REPO="${DOCKER_REPO_INPUT:-$IMAGE_NAME}"

    FULL_IMAGE_NAME="${DOCKER_USERNAME}/${DOCKER_REPO}:${IMAGE_TAG}"

    echo "Logging in to Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
    echo "Docker Hub login successful."

else
    echo "ERROR: Invalid registry choice. Please enter 1 or 2."
    exit 1
fi

echo ""
echo "Building Docker image: $FULL_IMAGE_NAME"
echo "Dockerfile: $DOCKERFILE_PATH"
echo "Build context: . (repository root)"
echo ""

# ── Build ────────────────────────────────────────────────────
docker build \
    -f "$DOCKERFILE_PATH" \
    -t "$FULL_IMAGE_NAME" \
    .

echo ""
echo "Build successful. Pushing image: $FULL_IMAGE_NAME"

# ── Push ─────────────────────────────────────────────────────
docker push "$FULL_IMAGE_NAME"

echo ""
echo "=============================================="
echo " Image pushed successfully!"
echo " Image: $FULL_IMAGE_NAME"
echo "=============================================="
