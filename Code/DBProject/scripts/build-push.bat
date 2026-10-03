@echo off
setlocal enabledelayedexpansion

:: =============================================================
:: build-push.bat - Build and push Docker image for HospitalMgmtApp
:: ASP.NET Web Forms (.NET Framework 4.5.2) on Amazon EKS
:: =============================================================

set "PROJECT_NAME=hospitalmgmtapp"
set "DOCKERFILE_PATH=Code\DBProject\Dockerfile"
set "BUILD_CONTEXT=Code\DBProject"

echo ==============================================
echo  HospitalMgmtApp - Docker Build ^& Push Script
echo ==============================================
echo.

:: Sanitize image name using PowerShell
for /f "delims=" %%i in ('powershell -Command "$n = 'hospitalmgmtapp' -replace '[^a-z0-9]','-'; $n = $n.ToLower().Trim('-'); while($n -match '--'){{$n=$n -replace '--','-'}}; $n"') do set "IMAGE_NAME=%%i"

:: Prompt for image tag
set /p "IMAGE_TAG_INPUT=Enter image tag [default: latest]: "
if "!IMAGE_TAG_INPUT!"=="" (
    set "IMAGE_TAG=latest"
) else (
    for /f "delims=" %%t in ('powershell -Command "$t = '!IMAGE_TAG_INPUT!' -replace '[^a-z0-9._-]','-'; $t = $t.ToLower().Trim('-'); $t"') do set "IMAGE_TAG=%%t"
    if "!IMAGE_TAG!"=="" set "IMAGE_TAG=latest"
)

echo.
echo Select container registry:
echo   1. AWS ECR (Elastic Container Registry)
echo   2. Docker Hub
set /p "REGISTRY_CHOICE=Enter choice [1 or 2]: "

echo.

if "!REGISTRY_CHOICE!"=="1" goto ecr_flow
if "!REGISTRY_CHOICE!"=="2" goto dockerhub_flow
echo ERROR: Invalid choice. Please enter 1 or 2.
exit /b 1

:ecr_flow
set /p "AWS_REGION=Enter AWS Region (e.g. us-east-1): "
set /p "AWS_ACCOUNT_ID=Enter AWS Account ID (12-digit): "

set "ECR_REPO=!IMAGE_NAME!"
set "REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com"
set "FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!"

echo.
echo Logging in to AWS ECR...
aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo ERROR: ECR login failed.
    exit /b 1
)

echo.
echo Checking if ECR repository exists...
aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Creating ECR repository: !ECR_REPO!
    aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECR repository.
        exit /b 1
    )
)
goto build_image

:dockerhub_flow
set /p "DOCKER_USERNAME=Enter Docker Hub username: "
set /p "DOCKER_PASSWORD=Enter Docker Hub password/token: "

set "REGISTRY_URL=docker.io"
set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!"

echo.
echo Logging in to Docker Hub...
echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker Hub login failed.
    exit /b 1
)
goto build_image

:build_image
echo.
echo Building Docker image...
echo   Image  : !FULL_IMAGE_NAME!
echo   File   : !DOCKERFILE_PATH!
echo   Context: !BUILD_CONTEXT!
echo.

docker build -f "!DOCKERFILE_PATH!" -t "!FULL_IMAGE_NAME!" "!BUILD_CONTEXT!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)

echo.
echo Pushing image to registry...
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ==============================================
echo  Build and push completed successfully!
echo  Image: !FULL_IMAGE_NAME!
echo ==============================================

endlocal
