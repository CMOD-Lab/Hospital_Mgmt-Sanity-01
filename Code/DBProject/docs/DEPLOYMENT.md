# Deployment Guide – Clinic Management System

## Overview

This guide covers containerising and deploying the **Clinic Management System** (ASP.NET Web Forms, .NET Framework 4.5.2) to **AWS EKS** using Windows node groups.

| Item | Value |
|------|-------|
| Application | Clinic Management System |
| Framework | ASP.NET Web Forms (.NET Framework 4.5.2) |
| Runtime base image | `mcr.microsoft.com/dotnet/framework/aspnet:4.5.2` |
| Build base image | `mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019` |
| Container port | 80 (HTTP / IIS) |
| Health endpoint | `GET /Health.aspx` → HTTP 200 |
| Target platform | AWS EKS (Windows node groups) |

---

## Prerequisites

### Local Development
- Docker Desktop with **Windows containers** enabled
- AWS CLI v2 (`aws --version`)
- `kubectl` v1.24+ (`kubectl version --client`)
- `eksctl` (optional, for cluster creation)
- Git

### AWS Permissions Required
- `ecr:*` – push/pull images
- `eks:DescribeCluster`, `eks:UpdateKubeconfig`
- `iam:PassRole` (for node groups)
- `ec2:*` (for load balancer provisioning)

---

## 1. Local Development with Docker Compose

### 1.1 Set environment variables

Create a `.env` file in `Code/DBProject/`:

```env
DB_CONNECTION_STRING=Data Source=<sql-host>;Initial Catalog=DBProject;User ID=<user>;Password=<pass>
REDIS_HOST=<redis-host>
REDIS_PORT=6379
REDIS_ACCESS_KEY=
```

### 1.2 Build and run locally

```bash
# From repository root
docker-compose -f Code/DBProject/docker-compose.yml up --build
```

The application will be available at `http://localhost:80`.

### 1.3 Verify health endpoint

```bash
curl http://localhost/Health.aspx
# Expected: {"status":"healthy","application":"HospitalMgmt","timestamp":"..."}
```

---

## 2. Build and Push Docker Image

### 2.1 Linux / macOS

```bash
chmod +x Code/DBProject/scripts/build-push.sh
./Code/DBProject/scripts/build-push.sh
```

### 2.2 Windows

```cmd
Code\DBProject\scripts\build-push.bat
```

Both scripts will prompt you to:
1. Choose registry (AWS ECR or Docker Hub)
2. Enter registry credentials / details
3. Enter an image tag (defaults to `latest`)

The scripts build from the **repository root** using:
```
docker build -f Code/DBProject/Dockerfile -t <registry>/<repo>:<tag> .
```

---

## 3. AWS EKS Prerequisites

### 3.1 Install tools

```bash
# AWS CLI
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip && sudo ./aws/install

# kubectl
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
chmod +x kubectl && sudo mv kubectl /usr/local/bin/

# eksctl (optional)
curl --silent --location "https://github.com/weaveworks/eksctl/releases/latest/download/eksctl_$(uname -s)_amd64.tar.gz" | tar xz -C /tmp
sudo mv /tmp/eksctl /usr/local/bin
```

### 3.2 Configure AWS credentials

```bash
aws configure
# Enter: AWS Access Key ID, Secret Access Key, Region, Output format
```

### 3.3 Create EKS cluster with Windows node group (if needed)

```bash
eksctl create cluster \
  --name hospital-mgmt-cluster \
  --region us-east-1 \
  --nodegroup-name windows-nodes \
  --node-type m5.xlarge \
  --nodes 2 \
  --nodes-min 1 \
  --nodes-max 4 \
  --managed \
  --node-ami-family WindowsServer2019FullContainer
```

> **Note**: Windows containers on EKS require Windows node groups. The `nodeSelector: kubernetes.io/os: windows` in `deployment.yaml` ensures pods are scheduled on Windows nodes.

### 3.4 Install AWS Load Balancer Controller

```bash
# Add IAM policy
curl -O https://raw.githubusercontent.com/kubernetes-sigs/aws-load-balancer-controller/v2.7.1/docs/install/iam_policy.json
aws iam create-policy \
  --policy-name AWSLoadBalancerControllerIAMPolicy \
  --policy-document file://iam_policy.json

# Install via Helm
helm repo add eks https://aws.github.io/eks-charts
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=hospital-mgmt-cluster \
  --set serviceAccount.create=true
```

---

## 4. Deploy to AWS EKS

### 4.1 Configure kubectl

```bash
aws eks update-kubeconfig --region us-east-1 --name hospital-mgmt-cluster
kubectl cluster-info
```

### 4.2 Run deployment script

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
- AWS region and EKS cluster name
- Full Docker image URI (e.g. `123456789.dkr.ecr.us-east-1.amazonaws.com/clinic-management-system:latest`)
- `DB_CONNECTION_STRING` – SQL Server connection string
- `REDIS_HOST` – Amazon ElastiCache Redis endpoint
- `REDIS_PORT` – Redis port (default: 6379)
- `REDIS_ACCESS_KEY` – Redis auth key (if AUTH enabled)

### 4.3 Manual deployment (alternative)

```bash
# Update deployment.yaml with your image URI
sed -i 's|{{IMAGE_URI}}|<your-image-uri>|g' Code/DBProject/kubernetes/deployment.yaml

# Apply manifests in order
kubectl apply -f Code/DBProject/kubernetes/namespace.yaml
kubectl apply -f Code/DBProject/kubernetes/deployment.yaml
kubectl apply -f Code/DBProject/kubernetes/service.yaml
kubectl apply -f Code/DBProject/kubernetes/ingress.yaml

# Wait for rollout
kubectl rollout status deployment/clinic-management-system -n clinic-management-system
```

---

## 5. Kubernetes Manifest Descriptions

| File | Purpose |
|------|---------|
| `namespace.yaml` | Creates the `clinic-management-system` namespace |
| `deployment.yaml` | Deploys 2 replicas with liveness/readiness probes on `/Health.aspx` |
| `service.yaml` | ClusterIP service exposing port 80 |
| `ingress.yaml` | AWS ALB Ingress (internet-facing) routing to the service |

### Environment Variables in deployment.yaml

| Variable | Description |
|----------|-------------|
| `DB_CONNECTION_STRING` | SQL Server connection string (inject via Kubernetes Secret) |
| `REDIS_HOST` | Amazon ElastiCache Redis endpoint |
| `REDIS_PORT` | Redis port (default: 6379) |
| `REDIS_ACCESS_KEY` | Redis AUTH key (if enabled) |

---

## 6. Verify Deployment

```bash
# Check pod status
kubectl get pods -n clinic-management-system

# Check pod logs
kubectl logs -l app=clinic-management-system -n clinic-management-system

# Check ingress and get ALB hostname
kubectl get ingress -n clinic-management-system

# Test health endpoint
curl http://<alb-hostname>/Health.aspx
```

---

## 7. Scaling and Management

### Horizontal scaling

```bash
kubectl scale deployment clinic-management-system \
  --replicas=4 -n clinic-management-system
```

### Rolling update (new image)

```bash
kubectl set image deployment/clinic-management-system \
  clinic-management-system=<new-image-uri> \
  -n clinic-management-system

kubectl rollout status deployment/clinic-management-system \
  -n clinic-management-system
```

### Rollback

```bash
kubectl rollout undo deployment/clinic-management-system \
  -n clinic-management-system
```

### Horizontal Pod Autoscaler (HPA)

```bash
kubectl autoscale deployment clinic-management-system \
  --cpu-percent=70 --min=2 --max=10 \
  -n clinic-management-system
```

---

## 8. Troubleshooting

### Pod not starting

```bash
kubectl describe pod -l app=clinic-management-system -n clinic-management-system
kubectl logs -l app=clinic-management-system -n clinic-management-system --previous
```

**Common causes:**
- `ImagePullBackOff` – ECR credentials not configured; check node IAM role has `ecr:GetAuthorizationToken`
- `CrashLoopBackOff` – Application startup failure; check `DB_CONNECTION_STRING` is correct
- `Pending` – No Windows nodes available; verify Windows node group is running

### Health probe failures

```bash
# Exec into pod (Windows)
kubectl exec -it <pod-name> -n clinic-management-system -- powershell
# Test health endpoint from inside pod
Invoke-WebRequest -Uri http://localhost/Health.aspx -UseBasicParsing
```

### Database connectivity

- Ensure SQL Server is reachable from the EKS VPC
- Verify security groups allow traffic on port 1433
- Check `DB_CONNECTION_STRING` format: `Data Source=<host>;Initial Catalog=DBProject;User ID=<user>;Password=<pass>`

### Redis session state

- Verify ElastiCache Redis is in the same VPC as EKS
- Check security group allows port 6379 from EKS node security group
- If AUTH is disabled, leave `REDIS_ACCESS_KEY` empty

### Ingress not getting an address

```bash
kubectl describe ingress clinic-management-system-ingress -n clinic-management-system
# Check AWS Load Balancer Controller logs
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller
```

---

## 9. Security Considerations

1. **Secrets management**: Store `DB_CONNECTION_STRING` and `REDIS_ACCESS_KEY` in Kubernetes Secrets or AWS Secrets Manager, not as plain environment variables in deployment.yaml.

   ```bash
   kubectl create secret generic clinic-mgmt-secrets \
     --from-literal=DB_CONNECTION_STRING="<connection-string>" \
     --from-literal=REDIS_ACCESS_KEY="<key>" \
     -n clinic-management-system
   ```

2. **Network policies**: Restrict pod-to-pod communication using Kubernetes NetworkPolicies.

3. **ECR image scanning**: Enable ECR image scanning on push to detect vulnerabilities.

4. **IAM roles for service accounts (IRSA)**: Use IRSA instead of node-level IAM roles for fine-grained permissions.

5. **TLS termination**: Configure HTTPS on the ALB by adding the `alb.ingress.kubernetes.io/certificate-arn` annotation with your ACM certificate ARN.

6. **Windows container security**: Windows containers run as `ContainerAdministrator` by default. Consider using `runAsNonRoot` where supported.

---

## 10. .NET Framework Specific Notes

- **Windows containers required**: .NET Framework 4.5.2 applications must run on Windows containers. Ensure your EKS cluster has Windows node groups.
- **IIS hosting**: The application is hosted by IIS inside the container. The `ServiceMonitor.exe` entry point keeps the container alive while monitoring the `w3svc` Windows service.
- **Session state**: Redis-backed distributed session state is configured via `Microsoft.Web.RedisSessionStateProvider`. Ensure `REDIS_HOST` is set correctly.
- **Connection strings**: The DAL reads `DB_CONNECTION_STRING` from the environment variable first, falling back to `Web.config` for local development.
- **Startup time**: Windows containers take longer to start than Linux containers. The liveness probe has a 90-second initial delay and the readiness probe has a 60-second initial delay to accommodate IIS startup.
- **Image size**: Windows container images are significantly larger than Linux images (several GB). Plan ECR storage and pull times accordingly.
