@echo off
setlocal enabledelayedexpansion

:: ============================================================
:: build-push.bat – Build and push Clinic Management System Docker image
:: Application : ASP.NET Web Forms (.NET Framework 4.5.2)
:: Target      : AWS ECR or Docker Hub
:: ============================================================

set PROJECT_NAME=clinic-management-system
set DOCKERFILE_PATH=Code\DBProject\Dockerfile

echo ==============================================
echo  Clinic Management System - Build ^& Push
echo ==============================================
echo.

:: ── Tag sanitisation via PowerShell ─────────────────────────
for /f "delims=" %%i in ('powershell -Command "$n = 'clinic-management-system'; $n = $n.ToLower() -replace '[^a-z0-9]+','-'; $n = $n.Trim('-'); Write-Output $n"') do set IMAGE_NAME=%%i

:: ── Prompt for image tag ─────────────────────────────────────
set /p INPUT_TAG="Enter image tag [latest]: "
if "!INPUT_TAG!"=="" set INPUT_TAG=latest
for /f "delims=" %%t in ('powershell -Command "$t = '!INPUT_TAG!'; $t = $t.ToLower() -replace '[^a-z0-9._-]+','-'; $t = $t.Trim('-'); if ($t -eq '') { $t = 'latest' }; Write-Output $t"') do set IMAGE_TAG=%%t
echo Using image tag: !IMAGE_TAG!
echo.

:: ── Registry selection ───────────────────────────────────────
echo Select container registry:
echo   1) AWS ECR
echo   2) Docker Hub
set /p REGISTRY_CHOICE="Enter choice [1 or 2]: "
echo.

if "!REGISTRY_CHOICE!"=="1" goto :ecr_flow
if "!REGISTRY_CHOICE!"=="2" goto :dockerhub_flow
echo ERROR: Invalid registry choice. Please enter 1 or 2.
exit /b 1

:ecr_flow
set /p AWS_REGION="Enter AWS Region (e.g. us-east-1): "
set /p AWS_ACCOUNT_ID="Enter AWS Account ID: "
set /p ECR_REPO_INPUT="Enter ECR repository name [!IMAGE_NAME!]: "
if "!ECR_REPO_INPUT!"=="" (set ECR_REPO=!IMAGE_NAME!) else (set ECR_REPO=!ECR_REPO_INPUT!)

set REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com
set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!

echo Logging in to AWS ECR...
aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo ERROR: ECR login failed.
    exit /b 1
)
echo ECR login successful.

echo Checking ECR repository '!ECR_REPO!'...
aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Creating ECR repository '!ECR_REPO!'...
    aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECR repository.
        exit /b 1
    )
)
echo ECR repository ready.
goto :build_image

:dockerhub_flow
set /p DOCKER_USERNAME="Enter Docker Hub username: "
set /p DOCKER_PASSWORD="Enter Docker Hub password/token: "
set /p DOCKER_REPO_INPUT="Enter Docker Hub repository name [!IMAGE_NAME!]: "
if "!DOCKER_REPO_INPUT!"=="" (set DOCKER_REPO=!IMAGE_NAME!) else (set DOCKER_REPO=!DOCKER_REPO_INPUT!)

set FULL_IMAGE_NAME=!DOCKER_USERNAME!/!DOCKER_REPO!:!IMAGE_TAG!

echo Logging in to Docker Hub...
echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker Hub login failed.
    exit /b 1
)
echo Docker Hub login successful.
goto :build_image

:build_image
echo.
echo Building Docker image: !FULL_IMAGE_NAME!
echo Dockerfile: !DOCKERFILE_PATH!
echo Build context: . (repository root)
echo.

docker build -f !DOCKERFILE_PATH! -t !FULL_IMAGE_NAME! .
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)

echo.
echo Build successful. Pushing image: !FULL_IMAGE_NAME!

docker push !FULL_IMAGE_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ==============================================
echo  Image pushed successfully!
echo  Image: !FULL_IMAGE_NAME!
echo ==============================================

endlocal
