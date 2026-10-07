@echo off
setlocal enabledelayedexpansion

rem ============================================================
rem deploy-image.bat - Deploy to AWS EKS
rem Application: Clinic Management System (DBProject)
rem Platform: Windows
rem Target: AWS EKS (Windows node groups)
rem ============================================================

set NAMESPACE=dbproject
set APP_NAME=dbproject
set K8S_DIR=%~dp0..\kubernetes
set TEMP_DIR=%TEMP%\dbproject-k8s-deploy

echo ==============================================
echo  Clinic Management System - Deploy to EKS
echo ==============================================
echo.

rem Prompt for AWS and cluster details
set /p AWS_REGION="Enter AWS region (e.g. us-east-1): "
if "!AWS_REGION!"=="" (
    echo AWS region is required. Exiting.
    exit /b 1
)

set /p CLUSTER_NAME="Enter EKS cluster name: "
if "!CLUSTER_NAME!"=="" (
    echo EKS cluster name is required. Exiting.
    exit /b 1
)

set /p IMAGE_URI="Enter full Docker image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/dbproject:latest): "
if "!IMAGE_URI!"=="" (
    echo Image URI is required. Exiting.
    exit /b 1
)

echo.
echo --- Optional: Application Environment Variables ---
echo Press Enter to skip any variable.
echo.

set /p DB_CONNECTION_STRING="Enter DB_CONNECTION_STRING (SQL Server connection string): "
set /p REDIS_CONNECTION_STRING="Enter REDIS_CONNECTION_STRING (Redis/ElastiCache connection string): "
set /p APPINSIGHTS_INSTRUMENTATIONKEY="Enter APPINSIGHTS_INSTRUMENTATIONKEY (Application Insights key): "

echo.
echo Configuring kubectl for EKS cluster: !CLUSTER_NAME! in !AWS_REGION!...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo Failed to configure kubectl. Exiting.
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo Cannot connect to cluster. Exiting.
    exit /b 1
)

echo.
echo Updating Kubernetes manifests with deployment values...

rem Copy manifests to temp directory
if exist "!TEMP_DIR!" rmdir /s /q "!TEMP_DIR!"
xcopy /e /i /q "!K8S_DIR!" "!TEMP_DIR!" >nul

rem Replace placeholders using PowerShell
powershell -Command "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{IMAGE_URI}}','!IMAGE_URI!' | Set-Content '!TEMP_DIR!\deployment.yaml'"
powershell -Command "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{DB_CONNECTION_STRING}}','!DB_CONNECTION_STRING!' | Set-Content '!TEMP_DIR!\deployment.yaml'"
powershell -Command "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{REDIS_CONNECTION_STRING}}','!REDIS_CONNECTION_STRING!' | Set-Content '!TEMP_DIR!\deployment.yaml'"
powershell -Command "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{APPINSIGHTS_INSTRUMENTATIONKEY}}','!APPINSIGHTS_INSTRUMENTATIONKEY!' | Set-Content '!TEMP_DIR!\deployment.yaml'"

echo.
echo Applying Kubernetes manifests...

echo   [1/4] Applying namespace...
kubectl apply -f "!TEMP_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo Failed to apply namespace. & exit /b 1 )

echo   [2/4] Applying deployment...
kubectl apply -f "!TEMP_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo Failed to apply deployment. & exit /b 1 )

echo   [3/4] Applying service...
kubectl apply -f "!TEMP_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo Failed to apply service. & exit /b 1 )

echo   [4/4] Applying ingress...
kubectl apply -f "!TEMP_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo Failed to apply ingress. & exit /b 1 )

echo.
echo Waiting for deployment rollout to complete...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo Deployment rollout did not complete successfully.
    echo Run: kubectl describe deployment !APP_NAME! -n !NAMESPACE!
    exit /b 1
)

echo.
echo Verifying deployed resources...
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ==============================================
echo  Deployment complete!
echo  Namespace: !NAMESPACE!
echo  Image: !IMAGE_URI!
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!

rem Cleanup temp files
if exist "!TEMP_DIR!" rmdir /s /q "!TEMP_DIR!"

endlocal
