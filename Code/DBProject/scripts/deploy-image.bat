@echo off
setlocal enabledelayedexpansion

:: ============================================================
:: deploy-image.bat – Deploy Clinic Management System to AWS EKS
:: Application : ASP.NET Web Forms (.NET Framework 4.5.2)
:: Platform    : AWS EKS (Windows node groups)
:: ============================================================

set APP_NAME=clinic-management-system
set NAMESPACE=clinic-management-system
set K8S_DIR=%~dp0..\kubernetes

echo ==============================================
echo  Clinic Management System - Deploy to EKS
echo ==============================================
echo.

:: ── Prompt for AWS / EKS configuration ───────────────────────
set /p AWS_REGION="Enter AWS Region (e.g. us-east-1): "
if "!AWS_REGION!"=="" (
    echo ERROR: AWS Region is required.
    exit /b 1
)

set /p CLUSTER_NAME="Enter EKS Cluster Name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS Cluster Name is required.
    exit /b 1
)

set /p IMAGE_URI="Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/clinic-management-system:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker image URI is required.
    exit /b 1
)

echo.
echo -- Application Environment Variables --
echo The following environment variables are required by the application.
echo Press Enter to skip (configure manually in Kubernetes Secrets).
echo.

set /p DB_CONNECTION_STRING_VAL="Enter DB_CONNECTION_STRING value (SQL Server connection string): "
set /p REDIS_CONNECTION_STRING_VAL="Enter REDIS_CONNECTION_STRING value (Redis connection string): "
echo.

:: ── Configure kubectl for EKS ────────────────────────────────
echo Configuring kubectl for EKS cluster '!CLUSTER_NAME!' in region '!AWS_REGION!'...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl for EKS.
    exit /b 1
)
echo kubectl configured successfully.
echo.

:: ── Verify cluster connectivity ──────────────────────────────
echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to EKS cluster. Check credentials and cluster name.
    exit /b 1
)
echo.

:: ── Create Kubernetes Secrets if values were provided ────────
if not "!DB_CONNECTION_STRING_VAL!"=="" (
    echo Creating/updating clinic-db-secret...
    kubectl create secret generic clinic-db-secret --namespace=!NAMESPACE! --from-literal=connectionString="!DB_CONNECTION_STRING_VAL!" --dry-run=client -o yaml | kubectl apply -f -
    if !ERRORLEVEL! neq 0 (
        echo WARNING: Failed to create clinic-db-secret. You may need to create it manually.
    ) else (
        echo clinic-db-secret applied.
    )
)

if not "!REDIS_CONNECTION_STRING_VAL!"=="" (
    echo Creating/updating clinic-redis-secret...
    kubectl create secret generic clinic-redis-secret --namespace=!NAMESPACE! --from-literal=connectionString="!REDIS_CONNECTION_STRING_VAL!" --dry-run=client -o yaml | kubectl apply -f -
    if !ERRORLEVEL! neq 0 (
        echo WARNING: Failed to create clinic-redis-secret. You may need to create it manually.
    ) else (
        echo clinic-redis-secret applied.
    )
)
echo.

:: ── Replace {{IMAGE_URI}} placeholder in deployment manifest ─
echo Updating deployment manifest with image URI...
set TEMP_DEPLOY=%TEMP%\deployment-deploy.yaml
copy /Y "!K8S_DIR!\deployment.yaml" "!TEMP_DEPLOY!" >nul
powershell -Command "(Get-Content '!TEMP_DEPLOY!') -replace '\{\{IMAGE_URI\}\}', '!IMAGE_URI!' | Set-Content '!TEMP_DEPLOY!'"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to update deployment manifest.
    exit /b 1
)
echo Manifest updated.
echo.

:: ── Apply Kubernetes manifests in order ──────────────────────
echo Applying namespace...
kubectl apply -f "!K8S_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace. & exit /b 1 )

echo Applying deployment...
kubectl apply -f "!TEMP_DEPLOY!"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment. & exit /b 1 )

echo Applying service...
kubectl apply -f "!K8S_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service. & exit /b 1 )

echo Applying ingress...
kubectl apply -f "!K8S_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress. & exit /b 1 )

echo.
echo Waiting for deployment rollout to complete...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo WARNING: Rollout did not complete within timeout. Check pod status manually.
)

echo.
echo -- Deployed Resources --
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ==============================================
echo  Deployment complete!
echo  Namespace : !NAMESPACE!
echo  Image     : !IMAGE_URI!
echo ==============================================
echo.
echo -- Rollback Instructions --
echo To rollback to the previous version:
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
echo.
echo To check rollout history:
echo   kubectl rollout history deployment/!APP_NAME! -n !NAMESPACE!

:: ── Cleanup temp file ────────────────────────────────────────
if exist "!TEMP_DEPLOY!" del /f /q "!TEMP_DEPLOY!"

endlocal
