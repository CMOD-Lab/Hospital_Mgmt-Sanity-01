@echo off
setlocal enabledelayedexpansion

:: =============================================================
:: deploy-image.bat – Deploy Clinic Management System to AWS EKS
:: =============================================================

set NAMESPACE=clinic-management-system
set APP_NAME=clinic-management-system
set SCRIPT_DIR=%~dp0
set K8S_DIR=%SCRIPT_DIR%..\kubernetes

echo ==============================================
echo  Clinic Management System - Deploy to EKS
echo ==============================================
echo.

:: ── Collect deployment inputs ─────────────────────────────────
set /p AWS_REGION="Enter AWS region (e.g. us-east-1): "
if "!AWS_REGION!"=="" (
    echo ERROR: AWS region is required.
    exit /b 1
)

set /p CLUSTER_NAME="Enter EKS cluster name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS cluster name is required.
    exit /b 1
)

set /p IMAGE_URI="Enter full Docker image URI: "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker image URI is required.
    exit /b 1
)

echo.
echo -- Application environment variables --
echo    (Press Enter to skip any optional variable)
echo.

set /p DB_CONNECTION_STRING="Enter DB_CONNECTION_STRING (SQL Server connection string): "
set /p REDIS_HOST="Enter REDIS_HOST (ElastiCache Redis host): "
set /p REDIS_PORT_INPUT="Enter REDIS_PORT (default: 6379): "
if "!REDIS_PORT_INPUT!"=="" (
    set REDIS_PORT=6379
) else (
    set REDIS_PORT=!REDIS_PORT_INPUT!
)
set /p REDIS_ACCESS_KEY="Enter REDIS_ACCESS_KEY (Redis auth key, if enabled): "

:: ── Create temp directory for modified manifests ─────────────
set TMP_DIR=%TEMP%\cms-deploy-%RANDOM%
mkdir "!TMP_DIR!"

copy "!K8S_DIR!\namespace.yaml"  "!TMP_DIR!\namespace.yaml"  >nul
copy "!K8S_DIR!\deployment.yaml" "!TMP_DIR!\deployment.yaml" >nul
copy "!K8S_DIR!\service.yaml"    "!TMP_DIR!\service.yaml"    >nul
copy "!K8S_DIR!\ingress.yaml"    "!TMP_DIR!\ingress.yaml"    >nul

:: ── Replace placeholders using PowerShell ────────────────────
echo.
echo -- Updating Kubernetes manifests --
powershell -NoProfile -Command ^
  "(Get-Content '!TMP_DIR!\deployment.yaml') ^
   -replace '{{IMAGE_URI}}','!IMAGE_URI!' ^
   -replace '{{DB_CONNECTION_STRING}}','!DB_CONNECTION_STRING!' ^
   -replace '{{REDIS_HOST}}','!REDIS_HOST!' ^
   -replace '{{REDIS_PORT}}','!REDIS_PORT!' ^
   -replace '{{REDIS_ACCESS_KEY}}','!REDIS_ACCESS_KEY!' ^
   | Set-Content '!TMP_DIR!\deployment.yaml'"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to update deployment manifest.
    exit /b 1
)

:: ── Configure kubectl ─────────────────────────────────────────
echo.
echo -- Configuring kubectl for EKS cluster --
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl.
    exit /b 1
)

echo -- Verifying cluster connectivity --
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to EKS cluster.
    exit /b 1
)

:: ── Apply Kubernetes manifests ────────────────────────────────
echo.
echo -- Applying Kubernetes manifests --
echo   1/4 Applying namespace...
kubectl apply -f "!TMP_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace. & exit /b 1 )

echo   2/4 Applying deployment...
kubectl apply -f "!TMP_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment. & exit /b 1 )

echo   3/4 Applying service...
kubectl apply -f "!TMP_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service. & exit /b 1 )

echo   4/4 Applying ingress...
kubectl apply -f "!TMP_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress. & exit /b 1 )

:: ── Wait for rollout ──────────────────────────────────────────
echo.
echo -- Waiting for rollout to complete --
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo ERROR: Rollout did not complete successfully.
    echo Rollback command: kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
    exit /b 1
)

:: ── Verify resources ──────────────────────────────────────────
echo.
echo -- Verifying deployed resources --
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ==============================================
echo  Deployment complete!
echo  Namespace : !NAMESPACE!
echo  Image     : !IMAGE_URI!
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!

:: Cleanup
rmdir /s /q "!TMP_DIR!" 2>nul

endlocal
