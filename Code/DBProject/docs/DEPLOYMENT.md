# Deployment Guide – Clinic Management System on AWS EKS

## Overview

This guide covers the complete deployment of the **Clinic Management System** (ASP.NET Web Forms, .NET Framework 4.5.2) as a Windows container on **AWS Elastic Kubernetes Service (EKS)**.

| Property | Value |
|---|---|
| Application | Clinic Management System |
| Technology | ASP.NET Web Forms (.NET Framework 4.5.2) |
| Container Type | Windows Container (IIS) |
| Runtime Base Image | `mcr.microsoft.com/dotnet/framework/aspnet:4.5.2` |
| Build Base Image | `mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019` |
| Application Port | 80 (HTTP / IIS) |
| Health Endpoint | `/Health.aspx` |
| Target Platform | AWS EKS (Windows node groups) |

---

## Prerequisites

### Local Development Tools
- **Docker Desktop** (Windows containers enabled) – https://docs.docker.com/desktop/windows/
- **AWS CLI v2** – https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html
- **kubectl** – https://kubernetes.io/docs/tasks/tools/
- **eksctl** (optional, for cluster creation) – https://eksctl.io/

### AWS Requirements
- AWS account with appropriate IAM permissions
- IAM permissions required:
  - `eks:*` (EKS cluster management)
  - `ecr:*` (ECR repository management)
  - `ec2:*` (node group management)
  - `iam:PassRole` (for node group IAM roles)
- AWS CLI configured: `aws configure`

### Windows Container Requirements
- Docker Desktop must be switched to **Windows containers** mode
- EKS cluster must have **Windows node groups** configured
- Windows Server 2019 or 2022 nodes recommended

---

## Project Structure

```
Code/DBProject/
├── Dockerfile                    # Multi-stage Windows container build
├── .dockerignore                 # Build context exclusions
├── docker-compose.yml            # Local development compose file
├── Clinic Management System.csproj
├── Web.config                    # ASP.NET configuration
├── Health.aspx / Health.aspx.cs  # Health check endpoint
├── DAL/myDAL.cs                  # Data access layer
├── Admin/                        # Admin pages
├── Doctor/                       # Doctor pages
├── Patient/                      # Patient pages
├── kubernetes/
│   ├── namespace.yaml
│   ├── deployment.yaml
│   ├── service.yaml
│   └── ingress.yaml
├── scripts/
│   ├── build-push.sh             # Linux/macOS build & push
│   ├── build-push.bat            # Windows build & push
│   ├── deploy-image.sh           # Linux/macOS EKS deploy
│   └── deploy-image.bat          # Windows EKS deploy
└── docs/
    └── DEPLOYMENT.md             # This file
```

---

## Local Development Setup

### 1. Switch Docker to Windows Containers

Right-click the Docker Desktop tray icon → **Switch to Windows containers**.

### 2. Configure Environment Variables

Create a `.env` file in the `Code/DBProject/` directory:

```env
DB_CONNECTION_STRING=Data Source=<your-sql-server>;Initial Catalog=DBProject;User ID=<user>;Password=<password>
REDIS_CONNECTION_STRING=<your-redis-host>:6379,password=<password>,ssl=False
```

### 3. Build and Run Locally

```bash
# From the repository root
docker-compose -f Code/DBProject/docker-compose.yml up --build
```

The application will be available at: **http://localhost:8080**

Health check: **http://localhost:8080/Health.aspx**

### 4. Stop Local Environment

```bash
docker-compose -f Code/DBProject/docker-compose.yml down
```

---

## Build and Push Docker Image

### Linux / macOS

```bash
chmod +x Code/DBProject/scripts/build-push.sh
./Code/DBProject/scripts/build-push.sh
```

### Windows

```cmd
Code\DBProject\scripts\build-push.bat
```

Both scripts will interactively prompt for:
1. **Image tag** (default: `latest`)
2. **Registry type**: AWS ECR or Docker Hub
3. **Registry credentials** and repository details

The scripts automatically:
- Sanitise the image name (lowercase, hyphenated)
- Create the ECR repository if it does not exist (ECR only)
- Build the Docker image from the repository root
- Push the image to the selected registry

---

## AWS EKS Cluster Setup

### Option A: Create a New EKS Cluster with Windows Node Group

```bash
eksctl create cluster \
  --name clinic-management-cluster \
  --region us-east-1 \
  --version 1.28 \
  --nodegroup-name linux-nodes \
  --node-type t3.medium \
  --nodes 2

# Enable Windows support
eksctl utils install-vpc-controllers --cluster clinic-management-cluster --region us-east-1 --approve

# Add Windows node group
eksctl create nodegroup \
  --cluster clinic-management-cluster \
  --region us-east-1 \
  --name windows-nodes \
  --node-type t3.xlarge \
  --nodes 2 \
  --node-ami-family WindowsServer2019FullContainer
```

### Option B: Add Windows Node Group to Existing Cluster

```bash
eksctl create nodegroup \
  --cluster <your-cluster-name> \
  --region <your-region> \
  --name windows-nodes \
  --node-type t3.xlarge \
  --nodes 2 \
  --node-ami-family WindowsServer2019FullContainer
```

### Install AWS Load Balancer Controller

```bash
# Install cert-manager
kubectl apply --validate=false -f https://github.com/jetstack/cert-manager/releases/download/v1.13.0/cert-manager.yaml

# Install AWS Load Balancer Controller
helm repo add eks https://aws.github.io/eks-charts
helm repo update
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=<your-cluster-name> \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller
```

---

## Kubernetes Deployment

### Option A: Using the Deploy Script (Recommended)

**Linux / macOS:**
```bash
chmod +x Code/DBProject/scripts/deploy-image.sh
./Code/DBProject/scripts/deploy-image.sh
```

**Windows:**
```cmd
Code\DBProject\scripts\deploy-image.bat
```

The script will prompt for:
- AWS Region
- EKS Cluster Name
- Docker image URI (full path with tag)
- `DB_CONNECTION_STRING` (SQL Server connection string)
- `REDIS_CONNECTION_STRING` (Redis/ElastiCache connection string)

### Option B: Manual kubectl Deployment

#### Step 1: Configure kubectl

```bash
aws eks update-kubeconfig --region us-east-1 --name <your-cluster-name>
kubectl cluster-info
```

#### Step 2: Create Kubernetes Secrets

```bash
# Database connection string
kubectl create secret generic clinic-db-secret \
  --namespace=clinic-management-system \
  --from-literal=connectionString="Data Source=<host>;Initial Catalog=DBProject;User ID=<user>;Password=<password>"

# Redis connection string
kubectl create secret generic clinic-redis-secret \
  --namespace=clinic-management-system \
  --from-literal=connectionString="<redis-host>:6379,password=<password>"
```

#### Step 3: Update Image URI in Deployment Manifest

```bash
# Linux/macOS
sed -i 's|{{IMAGE_URI}}|<your-registry>/<repo>:<tag>|g' Code/DBProject/kubernetes/deployment.yaml

# Windows PowerShell
(Get-Content Code\DBProject\kubernetes\deployment.yaml) `
  -replace '\{\{IMAGE_URI\}\}', '<your-registry>/<repo>:<tag>' | `
  Set-Content Code\DBProject\kubernetes\deployment.yaml
```

#### Step 4: Apply Manifests

```bash
kubectl apply -f Code/DBProject/kubernetes/namespace.yaml
kubectl apply -f Code/DBProject/kubernetes/deployment.yaml
kubectl apply -f Code/DBProject/kubernetes/service.yaml
kubectl apply -f Code/DBProject/kubernetes/ingress.yaml
```

#### Step 5: Verify Deployment

```bash
# Watch pod status
kubectl get pods -n clinic-management-system -w

# Check deployment rollout
kubectl rollout status deployment/clinic-management-system -n clinic-management-system

# View all resources
kubectl get pods,svc,ingress -n clinic-management-system

# Get application URL
kubectl get ingress clinic-management-system-ingress -n clinic-management-system
```

---

## Configuration Management

### Environment Variables

| Variable | Description | Source |
|---|---|---|
| `DB_CONNECTION_STRING` | SQL Server connection string | Kubernetes Secret `clinic-db-secret` |
| `REDIS_CONNECTION_STRING` | Redis/ElastiCache connection string | Kubernetes Secret `clinic-redis-secret` |
| `ASPNET_ENVIRONMENT` | Application environment | Deployment env (default: `Production`) |

### Updating Secrets

```bash
# Update database connection string
kubectl create secret generic clinic-db-secret \
  --namespace=clinic-management-system \
  --from-literal=connectionString="<new-connection-string>" \
  --dry-run=client -o yaml | kubectl apply -f -

# Restart pods to pick up new secret values
kubectl rollout restart deployment/clinic-management-system -n clinic-management-system
```

---

## Scaling and Management

### Horizontal Scaling

```bash
# Scale to 3 replicas
kubectl scale deployment clinic-management-system \
  --replicas=3 \
  -n clinic-management-system
```

### Horizontal Pod Autoscaler (HPA)

```bash
kubectl autoscale deployment clinic-management-system \
  --namespace=clinic-management-system \
  --cpu-percent=70 \
  --min=2 \
  --max=10
```

### Rolling Update

```bash
# Update image
kubectl set image deployment/clinic-management-system \
  clinic-management-system=<new-image-uri> \
  -n clinic-management-system

# Monitor rollout
kubectl rollout status deployment/clinic-management-system -n clinic-management-system
```

### Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/clinic-management-system -n clinic-management-system

# Rollback to specific revision
kubectl rollout history deployment/clinic-management-system -n clinic-management-system
kubectl rollout undo deployment/clinic-management-system --to-revision=<revision-number> -n clinic-management-system
```

---

## Troubleshooting

### Pod Not Starting

```bash
# Check pod status and events
kubectl describe pod -l app=clinic-management-system -n clinic-management-system

# Check pod logs
kubectl logs -l app=clinic-management-system -n clinic-management-system --tail=100

# Check previous pod logs (if crashed)
kubectl logs -l app=clinic-management-system -n clinic-management-system --previous
```

### Health Check Failures

The application exposes a health endpoint at `/Health.aspx` which returns:
```json
{"status":"healthy","application":"DBProject","timestamp":"<ISO-8601>"}
```

```bash
# Test health endpoint from within the cluster
kubectl exec -it <pod-name> -n clinic-management-system -- \
  powershell -Command "Invoke-WebRequest -Uri 'http://localhost/Health.aspx' -UseBasicParsing"
```

### Database Connection Issues

```bash
# Verify secret exists
kubectl get secret clinic-db-secret -n clinic-management-system

# Check secret value (base64 encoded)
kubectl get secret clinic-db-secret -n clinic-management-system -o jsonpath='{.data.connectionString}' | base64 -d
```

### Ingress / Load Balancer Issues

```bash
# Check ingress status
kubectl describe ingress clinic-management-system-ingress -n clinic-management-system

# Check AWS Load Balancer Controller logs
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller
```

### Windows Node Issues

```bash
# Verify Windows nodes are ready
kubectl get nodes -l kubernetes.io/os=windows

# Check node taints
kubectl describe node <windows-node-name> | grep Taints
```

---

## Security Considerations

1. **Secrets Management**: Always use Kubernetes Secrets (or AWS Secrets Manager with External Secrets Operator) for sensitive values. Never hardcode credentials.

2. **Network Policies**: Consider adding Kubernetes NetworkPolicies to restrict pod-to-pod communication.

3. **HTTPS**: Configure TLS termination at the ALB level using AWS Certificate Manager (ACM):
   ```yaml
   annotations:
     alb.ingress.kubernetes.io/listen-ports: '[{"HTTPS": 443}]'
     alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:<region>:<account>:certificate/<cert-id>
   ```

4. **Image Scanning**: Enable ECR image scanning to detect vulnerabilities:
   ```bash
   aws ecr put-image-scanning-configuration \
     --repository-name clinic-management-system \
     --image-scanning-configuration scanOnPush=true \
     --region us-east-1
   ```

5. **IAM Roles for Service Accounts (IRSA)**: Use IRSA instead of node-level IAM roles for fine-grained access control.

6. **Resource Limits**: Resource requests and limits are configured in `deployment.yaml` to prevent resource starvation.

---

## .NET Framework Specific Notes

- This application uses **ASP.NET Web Forms** on **.NET Framework 4.5.2** and requires **Windows containers**.
- The runtime base image `mcr.microsoft.com/dotnet/framework/aspnet:4.5.2` includes IIS and the full .NET Framework 4.5.2 runtime.
- The application uses **Redis-backed session state** (`Microsoft.Web.RedisSessionStateProvider`) for distributed session management across multiple pods. Ensure `REDIS_CONNECTION_STRING` is set correctly.
- The **Data Access Layer** (`DAL/myDAL.cs`) reads `DB_CONNECTION_STRING` from environment variables, falling back to `Web.config` if not set.
- **Application Insights** is configured via `ApplicationInsights.config`. Set the `APPINSIGHTS_INSTRUMENTATIONKEY` environment variable to enable telemetry.
- Windows containers require **Windows node groups** in EKS. The deployment manifest includes the required `nodeSelector` (`kubernetes.io/os: windows`) and tolerations.
- IIS startup time for Windows containers is typically 60–90 seconds. The liveness and readiness probes are configured with appropriate `initialDelaySeconds` values.
