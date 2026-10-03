# HospitalMgmtApp – Deployment Guide

## Overview

This guide covers containerization and deployment of the **HospitalMgmtApp** (Clinic Management System), an ASP.NET Web Forms application targeting **.NET Framework 4.5.2**, to **Amazon EKS** using Windows node groups.

- **Application**: Clinic Management System (Hospital Management)
- **Framework**: ASP.NET Web Forms / .NET Framework 4.5.2
- **Container Base Image (Runtime)**: `mcr.microsoft.com/dotnet/framework/aspnet:4.5.2`
- **Container Base Image (Build)**: `mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019`
- **Web Server**: IIS (Internet Information Services) inside Windows container
- **Health Endpoint**: `GET /Health.aspx` → HTTP 200 `{"status":"healthy",...}`
- **Application Port**: 80 (HTTP)
- **Target Platform**: AWS EKS (Windows node groups)

---

## Prerequisites

### Local Development
- Docker Desktop with **Windows containers** enabled
- .NET Framework 4.5.2 SDK (Visual Studio 2017+)
- AWS CLI v2 (`aws --version`)
- kubectl (`kubectl version --client`)

### AWS EKS Deployment
- AWS CLI configured with appropriate IAM permissions:
  - `ecr:*` (push/pull images)
  - `eks:DescribeCluster`, `eks:UpdateKubeconfig`
  - `ec2:*` (for Windows node groups)
- EKS cluster with **Windows node groups** (required for .NET Framework Windows containers)
- AWS Load Balancer Controller installed on the EKS cluster
- kubectl configured for the target cluster

---

## Project Structure

```
Code/DBProject/
├── Dockerfile                  # Multi-stage build (SDK → aspnet:4.5.2 runtime)
├── .dockerignore               # Excludes bin/, obj/, .vs/, packages/, etc.
├── docker-compose.yml          # Local development (single app service)
├── kubernetes/
│   ├── namespace.yaml          # Namespace: hospitalmgmtapp
│   ├── deployment.yaml         # Deployment with 2 replicas, health probes
│   ├── service.yaml            # ClusterIP service on port 80
│   └── ingress.yaml            # AWS ALB Ingress (internet-facing)
├── scripts/
│   ├── build-push.sh           # Linux/macOS: build & push to ECR or Docker Hub
│   ├── build-push.bat          # Windows: build & push to ECR or Docker Hub
│   ├── deploy-image.sh         # Linux/macOS: deploy to AWS EKS
│   └── deploy-image.bat        # Windows: deploy to AWS EKS
└── docs/
    └── DEPLOYMENT.md           # This file
```

---

## Environment Variables

| Variable | Description | Required |
|---|---|---|
| `DB_CONNECTION_STRING` | SQL Server connection string (e.g., `Data Source=<host>;Initial Catalog=DBProject;User ID=sa;Password=<pwd>`) | Yes |
| `REDIS_CONNECTION_STRING` | Amazon ElastiCache Redis endpoint (e.g., `<host>:6379,abortConnect=False`) | Yes (for session state) |
| `ASPNET_ENV` | Application environment (`Production`, `Development`) | No (default: `Production`) |
| `PORT` | IIS listening port inside container | No (default: `80`) |

---

## Local Development with Docker Compose

### Step 1: Set environment variables

Create a `.env` file in `Code/DBProject/`:

```env
DB_CONNECTION_STRING=Data Source=host.docker.internal;Initial Catalog=DBProject;User ID=sa;Password=YourPassword
REDIS_CONNECTION_STRING=host.docker.internal:6379,abortConnect=False
```

### Step 2: Start the application

```bash
cd Code/DBProject
docker-compose up --build
```

### Step 3: Access the application

Open your browser at: `http://localhost:8080`

Health check: `http://localhost:8080/Health.aspx`

### Step 4: Stop the application

```bash
docker-compose down
```

> **Note**: Docker Desktop must be switched to **Windows containers** mode for this application.

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

The script will prompt you to:
1. Enter an image tag (default: `latest`)
2. Select registry type: **AWS ECR** or **Docker Hub**
3. Provide registry credentials

**ECR Example:**
```
Enter image tag [default: latest]: v1.0.0
Select container registry:
  1. AWS ECR (Elastic Container Registry)
  2. Docker Hub
Enter choice [1 or 2]: 1
Enter AWS Region (e.g. us-east-1): us-east-1
Enter AWS Account ID (12-digit): 123456789012
```

The script automatically creates the ECR repository if it does not exist.

---

## AWS EKS Prerequisites

### 1. EKS Cluster with Windows Node Group

Your EKS cluster must have a **Windows node group** to run .NET Framework containers:

```bash
eksctl create nodegroup \
  --cluster <your-cluster-name> \
  --name windows-nodes \
  --node-type m5.xlarge \
  --nodes 2 \
  --nodes-min 1 \
  --nodes-max 4 \
  --node-ami-family WindowsServer2019FullContainer \
  --region us-east-1
```

### 2. AWS Load Balancer Controller

Install the AWS Load Balancer Controller for ALB Ingress support:

```bash
helm repo add eks https://aws.github.io/eks-charts
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=<your-cluster-name> \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller
```

### 3. Configure kubectl

```bash
aws eks update-kubeconfig --region us-east-1 --name <your-cluster-name>
kubectl cluster-info
```

---

## Kubernetes Deployment

### Option A: Using the deploy script (recommended)

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
- Full Docker image URI
- `DB_CONNECTION_STRING`
- `REDIS_CONNECTION_STRING`

### Option B: Manual kubectl apply

```bash
# 1. Update deployment.yaml with your image URI
sed -i 's|{{IMAGE_URI}}|123456789.dkr.ecr.us-east-1.amazonaws.com/hospitalmgmtapp:latest|g' kubernetes/deployment.yaml
sed -i 's|{{DB_CONNECTION_STRING}}|Data Source=mydb.example.com;Initial Catalog=DBProject;User ID=sa;Password=pwd|g' kubernetes/deployment.yaml
sed -i 's|{{REDIS_CONNECTION_STRING}}|myredis.cache.amazonaws.com:6379,abortConnect=False|g' kubernetes/deployment.yaml

# 2. Apply manifests in order
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# 3. Wait for rollout
kubectl rollout status deployment/hospitalmgmtapp -n hospitalmgmtapp

# 4. Verify
kubectl get pods,svc,ingress -n hospitalmgmtapp
```

---

## Kubernetes Manifest Descriptions

| File | Kind | Description |
|---|---|---|
| `namespace.yaml` | Namespace | Creates `hospitalmgmtapp` namespace |
| `deployment.yaml` | Deployment | 2 replicas, Windows node selector, liveness/readiness probes on `/Health.aspx` |
| `service.yaml` | Service | ClusterIP on port 80, routes to pod port 80 |
| `ingress.yaml` | Ingress | AWS ALB (internet-facing), health check on `/Health.aspx` |

---

## Health Checks

The application exposes a health endpoint at `/Health.aspx`:

```json
{"status":"healthy","application":"HospitalMgmtApp","timestamp":"2024-01-01T00:00:00.0000000Z"}
```

Kubernetes probes are configured in `deployment.yaml`:

| Probe | Path | Initial Delay | Period |
|---|---|---|---|
| Liveness | `/Health.aspx` | 90s | 30s |
| Readiness | `/Health.aspx` | 60s | 15s |

> **Note**: The longer initial delay accounts for IIS startup time in Windows containers.

---

## Scaling

### Manual scaling

```bash
kubectl scale deployment hospitalmgmtapp --replicas=4 -n hospitalmgmtapp
```

### Horizontal Pod Autoscaler (HPA)

```bash
kubectl autoscale deployment hospitalmgmtapp \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n hospitalmgmtapp
```

---

## Rolling Updates

```bash
# Update image
kubectl set image deployment/hospitalmgmtapp \
  hospitalmgmtapp=123456789.dkr.ecr.us-east-1.amazonaws.com/hospitalmgmtapp:v2.0.0 \
  -n hospitalmgmtapp

# Monitor rollout
kubectl rollout status deployment/hospitalmgmtapp -n hospitalmgmtapp
```

---

## Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/hospitalmgmtapp -n hospitalmgmtapp

# Rollback to specific revision
kubectl rollout history deployment/hospitalmgmtapp -n hospitalmgmtapp
kubectl rollout undo deployment/hospitalmgmtapp --to-revision=2 -n hospitalmgmtapp
```

---

## Troubleshooting

### Pods not starting

```bash
kubectl describe pod -l app=hospitalmgmtapp -n hospitalmgmtapp
kubectl logs -l app=hospitalmgmtapp -n hospitalmgmtapp --previous
```

### IIS startup issues

Windows containers take longer to start (60–120 seconds). Ensure `initialDelaySeconds` in probes is sufficient.

### Database connection failures

Verify `DB_CONNECTION_STRING` is correct and the SQL Server is reachable from the EKS VPC:

```bash
kubectl exec -it <pod-name> -n hospitalmgmtapp -- powershell -Command "Test-NetConnection -ComputerName <db-host> -Port 1433"
```

### Redis session issues

Verify `REDIS_CONNECTION_STRING` and ElastiCache security group allows inbound on port 6379 from the EKS node security group.

### Ingress not getting an address

```bash
kubectl describe ingress hospitalmgmtapp-ingress -n hospitalmgmtapp
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller
```

### Windows node not scheduling pods

Ensure the node group has the `kubernetes.io/os=windows` label and the deployment's `nodeSelector` matches:

```bash
kubectl get nodes --show-labels | grep windows
```

---

## Security Considerations

1. **Secrets Management**: Store `DB_CONNECTION_STRING` and `REDIS_CONNECTION_STRING` in Kubernetes Secrets or AWS Secrets Manager rather than plain environment variables in production.
2. **Network Policies**: Restrict pod-to-pod communication using Kubernetes NetworkPolicies.
3. **ECR Image Scanning**: Enable ECR image scanning to detect vulnerabilities.
4. **IAM Roles for Service Accounts (IRSA)**: Use IRSA for fine-grained AWS permissions instead of node-level IAM roles.
5. **TLS/HTTPS**: Configure HTTPS on the ALB Ingress using ACM certificates:
   ```yaml
   alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:us-east-1:123456789:certificate/xxx
   alb.ingress.kubernetes.io/listen-ports: '[{"HTTPS":443}]'
   ```
6. **Session Security**: The Redis-backed session state (`RedisSessionHelper`) ensures sessions persist across pod restarts and are not lost during rolling updates.

---

## .NET Framework Specific Notes

- This application uses **.NET Framework 4.5.2** and **ASP.NET Web Forms** — it requires **Windows containers**.
- The runtime base image `mcr.microsoft.com/dotnet/framework/aspnet:4.5.2` includes IIS and the full .NET Framework runtime.
- The build stage uses `mcr.microsoft.com/dotnet/framework/sdk:4.8` which is backward-compatible with 4.5.2 projects.
- **Session State**: Distributed Redis session state is configured via `Microsoft.Web.RedisSessionStateProvider` in `Web.config`. The `REDIS_CONNECTION_STRING` environment variable must be set for sessions to work in a multi-pod deployment.
- **Application Insights**: The application includes Microsoft Application Insights (`ApplicationInsights.config`). Configure the instrumentation key via environment variable or config for production monitoring.
- **IIS Application Pool**: Configured for .NET 4.0 Integrated pipeline mode with `AlwaysRunning` start mode to minimize cold-start latency.
