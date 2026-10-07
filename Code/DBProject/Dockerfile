# ============================================================
# Dockerfile for Clinic Management System (ASP.NET Web Forms)
# Technology: .NET Framework 4.8 / ASP.NET Web Forms
# Build Tool: MSBuild
# Runtime Base Image: mcr.microsoft.com/dotnet/framework/runtime:4.8
# Target Platform: AWS EKS (Windows node groups)
# ============================================================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR /src

# Copy solution and project files first for NuGet package layer caching
COPY ["Code/Clinic Management System.sln", "Code/"]
COPY ["Code/DBProject/DBProject.csproj", "Code/DBProject/"]
COPY ["Code/DBProject/packages.config", "Code/DBProject/"]

# Restore NuGet packages (cached layer - only invalidated when project/packages files change)
RUN nuget restore "Code/Clinic Management System.sln"

# Copy the rest of the source code
COPY . .

# Build and publish the Web Application in Release configuration
RUN msbuild "Code/DBProject/DBProject.csproj" \
    /p:Configuration=Release \
    /p:Platform="AnyCPU" \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\publish \
    /p:DeleteExistingFiles=True \
    /p:PrecompileBeforePublish=true \
    /p:EnableUpdateable=false

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8 AS runtime

# Set working directory to IIS default web root
WORKDIR /inetpub/wwwroot

# Remove default IIS welcome page
RUN powershell -Command \
    Remove-Item -Recurse -Force C:\inetpub\wwwroot\* -ErrorAction SilentlyContinue

# Copy published application from build stage
COPY --from=build C:\publish .

# ---- IIS Site Configuration ----
# Configure IIS application pool and site settings via PowerShell
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool managedRuntimeVersion v4.0; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool enable32BitAppOnWin64 False; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool processModel.idleTimeout ([TimeSpan]::FromMinutes(0)); \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool recycling.periodicRestart.time ([TimeSpan]::FromMinutes(0))

# ---- Environment Variables ----
# Database connection string - override at runtime via Kubernetes Secret/ConfigMap
ENV DB_CONNECTION_STRING=""

# Redis session state connection string for distributed sessions (EKS/ElastiCache)
ENV REDIS_CONNECTION_STRING=""

# Application Insights instrumentation key (optional telemetry)
ENV APPINSIGHTS_INSTRUMENTATIONKEY=""

# ASP.NET environment designation
ENV ASPNET_ENVIRONMENT="Production"

# Expose HTTP port (IIS default)
EXPOSE 80

# ---- Startup ----
# Start IIS service (W3SVC) as the container entry point
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
