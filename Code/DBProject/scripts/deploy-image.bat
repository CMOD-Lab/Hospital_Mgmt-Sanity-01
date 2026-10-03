@echo off
setlocal enabledelayedexpansion

:: =============================================================
:: deploy-image.bat - Deploy HospitalMgmtApp to AWS EKS
:: ASP.NET Web Forms (.NET Framework 4.5.2)
:: Namespace: hospitalmgmtapp
:: =============================================================

set "APP_NAME=hospitalmgmtapp"
set "K8S_DIR=%~dp0..\kubernetes"

echo ==============================================
echo  HospitalMgmtApp - AWS EKS Deployment Script
echo ==============================================
echo.

:: ---- Collect deployment inputs ----
set /p "AWS_REGION=Enter AWS Region (e.g. us-east-1): "
if "!AWS_REGION!"=="" (
    echo ERROR: AWS Region is required.
    exit /b 1
)

set /p "CLUSTER_NAME=Enter EKS Cluster Name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS Cluster Name is required.
    exit /b 1
)

set /p "IMAGE_URI=Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/hospitalmgmtapp:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker image URI is required.
    exit /b 1
)

echo.
echo ---- Application Environment Variables ----
set /p "DB_CONNECTION_STRING=Enter DB_CONNECTION_STRING (SQL Server connection string, or press Enter to skip): "
set /p "REDIS_CONNECTION_STRING=Enter REDIS_CONNECTION_STRING (ElastiCache Redis endpoint, or press Enter to skip): "

:: ---- Configure kubectl for EKS ----
echo.
echo Configuring kubectl for EKS cluster: !CLUSTER_NAME! in !AWS_REGION!...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl. Check your AWS credentials and cluster name.
    exit /b 1
)

echo.
echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to EKS cluster.
    exit /b 1
)

:: ---- Update Kubernetes manifests with actual values ----
echo.
echo Updating Kubernetes manifests with deployment values...

powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content '!K8S_DIR!\deployment.yaml'"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to update IMAGE_URI in deployment.yaml
    exit /b 1
)

if not "!DB_CONNECTION_STRING!"=="" (
    powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{DB_CONNECTION_STRING}}', '!DB_CONNECTION_STRING!' | Set-Content '!K8S_DIR!\deployment.yaml'"
) else (
    powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{DB_CONNECTION_STRING}}', '' | Set-Content '!K8S_DIR!\deployment.yaml'"
)

if not "!REDIS_CONNECTION_STRING!"=="" (
    powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{REDIS_CONNECTION_STRING}}', '!REDIS_CONNECTION_STRING!' | Set-Content '!K8S_DIR!\deployment.yaml'"
) else (
    powershell -Command "(Get-Content '!K8S_DIR!\deployment.yaml') -replace '{{REDIS_CONNECTION_STRING}}', '' | Set-Content '!K8S_DIR!\deployment.yaml'"
)

:: ---- Apply Kubernetes manifests in order ----
echo.
echo Applying Kubernetes manifests...

echo   [1/4] Applying namespace...
kubectl apply -f "!K8S_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace.yaml & exit /b 1 )

echo   [2/4] Applying deployment...
kubectl apply -f "!K8S_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment.yaml & exit /b 1 )

echo   [3/4] Applying service...
kubectl apply -f "!K8S_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service.yaml & exit /b 1 )

echo   [4/4] Applying ingress...
kubectl apply -f "!K8S_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress.yaml & exit /b 1 )

:: ---- Wait for rollout ----
echo.
echo Waiting for deployment rollout to complete...
kubectl rollout status deployment/!APP_NAME! -n !APP_NAME! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo ERROR: Deployment rollout did not complete within timeout.
    echo Run: kubectl describe deployment !APP_NAME! -n !APP_NAME!
    exit /b 1
)

:: ---- Verify resources ----
echo.
echo Verifying deployed resources...
kubectl get pods,svc,ingress -n !APP_NAME!

echo.
echo ==============================================
echo  Deployment completed successfully!
echo  Namespace : !APP_NAME!
echo  Image     : !IMAGE_URI!
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !APP_NAME!

endlocal
