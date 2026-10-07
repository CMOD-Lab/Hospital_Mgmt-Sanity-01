# Clinic Management System (DBProject) — Deployment Guide

## Overview

This guide covers containerizing and deploying the **Clinic Management System** (ASP.NET Web Forms, .NET Framework 4.8) to **AWS EKS** using Windows node groups.

---

## Prerequisites

### Local Development
- Docker Desktop with **Windows Containers** enabled
- .NET Framework 4.8 SDK (for local builds)
- Git

### AWS EKS Deployment
- AWS CLI v2 (`aws --version`)
- `kubectl` v1.24+ (`kubectl version --client`)
- `eksctl` (optional, for cluster creation)
- AWS IAM permissions: ECR push, EKS describe/update, EC2 (for node groups)
- An EKS cluster with **Windows node groups** (Windows Server 2019 or 2022)

---

## Project Structure

```
Code/DBProject/
├── Dockerfile                  # Multi-stage Windows container build
├── .dockerignore               # Excludes build artifacts from context
├── docker-compose.yml          # Local development compose file
├── kubernetes/
│   ├── namespace.yaml          # Kubernetes namespace
│   ├── deployment.yaml         # Deployment with 2 replicas
│   ├── service.yaml            # ClusterIP service
│   └── ingress.yaml            # ALB ingress (AWS Load Balancer Controller)
├── scripts/
│   ├── build-push.sh           # Linux/macOS build & push script
│   ├── build-push.bat          # Windows build & push script
│   ├── deploy-image.sh         # Linux/macOS EKS deploy script
│   └── deploy-image.bat        # Windows EKS deploy script
└── docs/
    └── DEPLOYMENT.md           # This file
```

---

## Technology Stack

| Component | Details |
|-----------|---------|
| Framework | .NET Framework 4.8 |
| Application Type | ASP.NET Web Forms |
| Web Server | IIS (Windows Container) |
| Base Build Image | `mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019` |
| Base Runtime Image | `mcr.microsoft.com/dotnet/framework/runtime:4.8` |
| Database | SQL Server (via `DB_CONNECTION_STRING`) |
| Session Store | Redis / AWS ElastiCache (via `REDIS_CONNECTION_STRING`) |
| Telemetry | Application Insights (via `APPINSIGHTS_INSTRUMENTATIONKEY`) |
| Health Endpoint | `GET /Health.aspx` → HTTP 200 JSON |
| Application Port | 80 (HTTP) |

---

## Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `DB_CONNECTION_STRING` | Yes | SQL Server connection string (e.g., `Server=myserver;Database=DBProject;User Id=sa;Password=...`) |
| `REDIS_CONNECTION_STRING` | Yes | Redis/ElastiCache connection string for distributed session state |
| `APPINSIGHTS_INSTRUMENTATIONKEY` | No | Azure Application Insights instrumentation key |
| `ASPNET_ENVIRONMENT` | No | Environment name (default: `Production`) |

---

## Local Development with Docker Compose

### 1. Switch Docker to Windows Containers

Right-click the Docker Desktop tray icon → **Switch to Windows containers**.

### 2. Configure Environment Variables

Create a `.env` file in `Code/DBProject/`:

```env
DB_CONNECTION_STRING=Server=host.docker.internal;Database=DBProject;User Id=sa;Password=YourPassword;
REDIS_CONNECTION_STRING=host.docker.internal:6379
APPINSIGHTS_INSTRUMENTATIONKEY=your-key-here
```

### 3. Build and Start

```bash
# From repository root
docker-compose -f Code/DBProject/docker-compose.yml up --build
```

### 4. Access the Application

Open: [http://localhost:80](http://localhost:80)

Health check: [http://localhost:80/Health.aspx](http://localhost:80/Health.aspx)

---

## Build and Push Docker Image

### Linux/macOS

```bash
chmod +x Code/DBProject/scripts/build-push.sh
./Code/DBProject/scripts/build-push.sh
```

### Windows

```cmd
Code\DBProject\scripts\build-push.bat
```

The script will prompt for:
1. Image tag (default: `latest`)
2. Registry type: AWS ECR or Docker Hub
3. Registry credentials

**Note**: Windows container images must be built on a Windows host with Docker in Windows container mode.

---

## AWS EKS Prerequisites

### 1. EKS Cluster with Windows Node Group

Your EKS cluster must have a Windows node group. If not already configured:

```bash
eksctl create nodegroup \
  --cluster my-cluster \
  --name windows-ng \
  --node-type m5.xlarge \
  --nodes 2 \
  --nodes-min 1 \
  --nodes-max 4 \
  --node-ami-family WindowsServer2019FullContainer \
  --region us-east-1
```

### 2. AWS Load Balancer Controller

Install the AWS Load Balancer Controller for ALB ingress support:

```bash
# Add the EKS chart repo
helm repo add eks https://aws.github.io/eks-charts
helm repo update

# Install the controller
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=my-cluster \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller
```

### 3. Configure kubectl

```bash
aws eks update-kubeconfig --region us-east-1 --name my-cluster
kubectl cluster-info
```

---

## Deploy to AWS EKS

### Linux/macOS

```bash
chmod +x Code/DBProject/scripts/deploy-image.sh
./Code/DBProject/scripts/deploy-image.sh
```

### Windows

```cmd
Code\DBProject\scripts\deploy-image.bat
```

The script will prompt for:
1. AWS region
2. EKS cluster name
3. Full Docker image URI (e.g., `123456789.dkr.ecr.us-east-1.amazonaws.com/dbproject:latest`)
4. Environment variable values (DB_CONNECTION_STRING, REDIS_CONNECTION_STRING, APPINSIGHTS_INSTRUMENTATIONKEY)

### Manual Deployment

```bash
# 1. Update image in deployment.yaml
sed -i 's|{{IMAGE_URI}}|YOUR_IMAGE_URI|g' Code/DBProject/kubernetes/deployment.yaml
sed -i 's|{{DB_CONNECTION_STRING}}|YOUR_DB_CONN|g' Code/DBProject/kubernetes/deployment.yaml
sed -i 's|{{REDIS_CONNECTION_STRING}}|YOUR_REDIS_CONN|g' Code/DBProject/kubernetes/deployment.yaml
sed -i 's|{{APPINSIGHTS_INSTRUMENTATIONKEY}}|YOUR_AI_KEY|g' Code/DBProject/kubernetes/deployment.yaml

# 2. Apply manifests
kubectl apply -f Code/DBProject/kubernetes/namespace.yaml
kubectl apply -f Code/DBProject/kubernetes/deployment.yaml
kubectl apply -f Code/DBProject/kubernetes/service.yaml
kubectl apply -f Code/DBProject/kubernetes/ingress.yaml

# 3. Wait for rollout
kubectl rollout status deployment/dbproject -n dbproject

# 4. Check resources
kubectl get pods,svc,ingress -n dbproject
```

---

## Kubernetes Manifest Descriptions

| File | Kind | Description |
|------|------|-------------|
| `namespace.yaml` | Namespace | Isolates all resources under `dbproject` namespace |
| `deployment.yaml` | Deployment | 2 replicas, Windows node selector, liveness/readiness probes on `/Health.aspx` |
| `service.yaml` | Service | ClusterIP service exposing port 80 |
| `ingress.yaml` | Ingress | AWS ALB ingress with health check on `/Health.aspx` |

---

## Health Checks

The application exposes a health endpoint at `/Health.aspx` that returns:

```json
{"status":"Healthy","application":"ClinicManagementSystem","timestamp":"2024-01-01T00:00:00.0000000Z"}
```

Kubernetes probes are configured as:
- **Liveness probe**: `GET /Health.aspx` — initial delay 90s, period 30s
- **Readiness probe**: `GET /Health.aspx` — initial delay 60s, period 15s

---

## Scaling and Management

### Scale Replicas

```bash
kubectl scale deployment dbproject --replicas=3 -n dbproject
```

### Rolling Update (new image)

```bash
kubectl set image deployment/dbproject dbproject=NEW_IMAGE_URI -n dbproject
kubectl rollout status deployment/dbproject -n dbproject
```

### Rollback

```bash
kubectl rollout undo deployment/dbproject -n dbproject
```

### View Logs

```bash
kubectl logs -l app=dbproject -n dbproject --tail=100
```

### Describe Pod (troubleshooting)

```bash
kubectl describe pod -l app=dbproject -n dbproject
```

---

## Troubleshooting

### Pod Not Starting

```bash
kubectl describe pod -l app=dbproject -n dbproject
kubectl logs -l app=dbproject -n dbproject
```

Common causes:
- **ImagePullBackOff**: Check ECR permissions and image URI
- **CrashLoopBackOff**: Check `DB_CONNECTION_STRING` is valid and SQL Server is reachable
- **Pending**: No Windows nodes available — check node group status

### IIS / Application Errors

```bash
# Exec into the container (Windows)
kubectl exec -it <pod-name> -n dbproject -- powershell
# Check IIS logs
Get-Content C:\inetpub\logs\LogFiles\W3SVC1\*.log -Tail 50
```

### Database Connection Issues

Verify the `DB_CONNECTION_STRING` environment variable is set correctly:

```bash
kubectl get deployment dbproject -n dbproject -o jsonpath='{.spec.template.spec.containers[0].env}'
```

### Redis Session Issues

Verify the `REDIS_CONNECTION_STRING` is pointing to your ElastiCache endpoint and the security group allows inbound on port 6379 from the EKS node group.

### Ingress Not Getting Hostname

```bash
kubectl describe ingress dbproject-ingress -n dbproject
```

Ensure the AWS Load Balancer Controller is installed and the IAM role has the required permissions.

---

## Security Considerations

1. **Secrets Management**: Store `DB_CONNECTION_STRING` and `REDIS_CONNECTION_STRING` in Kubernetes Secrets or AWS Secrets Manager, not as plain environment variables in deployment.yaml.

   ```bash
   kubectl create secret generic dbproject-secrets \
     --from-literal=DB_CONNECTION_STRING="Server=...;..." \
     --from-literal=REDIS_CONNECTION_STRING="redis-host:6379" \
     -n dbproject
   ```

2. **Network Policies**: Restrict pod-to-pod communication using Kubernetes NetworkPolicies.

3. **ECR Image Scanning**: Enable ECR image scanning on push to detect vulnerabilities.

4. **IAM Roles for Service Accounts (IRSA)**: Use IRSA to grant the application pod access to AWS services without static credentials.

5. **TLS/HTTPS**: Configure HTTPS on the ALB ingress using ACM certificates:
   ```yaml
   alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:us-east-1:123456789:certificate/...
   alb.ingress.kubernetes.io/listen-ports: '[{"HTTPS": 443}]'
   ```

6. **Windows Container Security**: Windows containers run as `ContainerAdministrator` by default. Consider using `runAsNonRoot` where supported.

---

## .NET Framework 4.8 Specific Notes

- **Windows Containers Only**: .NET Framework 4.8 requires Windows containers. Ensure your EKS cluster has Windows node groups.
- **IIS Hosting**: The application is hosted by IIS inside the container. The `ServiceMonitor.exe` entrypoint keeps the container alive while monitoring the W3SVC service.
- **Session State**: The application uses Redis for distributed session state (required for multi-replica deployments). Configure `REDIS_CONNECTION_STRING` with your AWS ElastiCache endpoint.
- **Application Insights**: Telemetry is configured via `APPINSIGHTS_INSTRUMENTATIONKEY`. Set this to your Application Insights resource key for production monitoring.
- **Build Time**: Windows container builds are significantly slower than Linux builds. Allow 10-20 minutes for the first build.
- **Image Size**: Windows container images are large (5-10 GB). Use ECR for storage to avoid Docker Hub rate limits.
