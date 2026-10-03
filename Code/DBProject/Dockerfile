# ============================================================
# Dockerfile for ASP.NET Web Forms Application (Clinic Management System / HospitalMgmtApp)
# Base Image (Runtime) : mcr.microsoft.com/dotnet/framework/aspnet:4.5.2
# Build Image          : mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019
# Target               : Amazon EKS with Windows node groups
# Application Port     : 80 (IIS default)
# Health Endpoint      : /Health.aspx
# ============================================================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR /app/src

# Copy solution and project files first for NuGet layer caching
COPY ["Clinic Management System.sln", "Clinic Management System.sln"]
COPY ["DBProject.csproj", "DBProject.csproj"]
COPY ["packages.config", "packages.config"]

# Restore NuGet packages (cached layer - only re-runs when project/packages files change)
RUN nuget restore "Clinic Management System.sln" -NonInteractive

# Copy the rest of the source code
COPY . .

# Build and publish the web application to C:\publish
RUN msbuild "DBProject.csproj" \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\publish \
    /p:DeleteExistingFiles=True \
    /p:platform="AnyCPU" \
    /p:VisualStudioVersion=16.0

# Remove PDB files, XML documentation, and temp artifacts from publish output
# to reduce the final image size
RUN powershell -Command \
    Write-Host 'Removing PDB files from publish output...'; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.pdb | Remove-Item -Force; \
    Write-Host 'Removing XML documentation files from publish output...'; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.xml | Remove-Item -Force; \
    Write-Host 'Removing temporary and cache files from publish output...'; \
    Get-ChildItem -Path C:\publish -Recurse -Include *.tmp,*.cache,*.log | Remove-Item -Force; \
    Write-Host 'Cleanup complete.'

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.5.2 AS runtime

# Set working directory to IIS default web root
WORKDIR /inetpub/wwwroot

# Remove default IIS welcome page
RUN powershell -Command \
    Remove-Item -Recurse -Force C:\inetpub\wwwroot\* -ErrorAction SilentlyContinue; \
    Import-Module WebAdministration; \
    Set-WebConfigurationProperty -Filter '/system.webServer/defaultDocument' -Name 'enabled' -Value 'True' -PSPath 'IIS:\Sites\Default Web Site'

# Copy ONLY the published application artifacts from the build stage.
# Source code, PDB files, obj/, bin/roslyn/, and temp files are NOT copied
# because they were excluded in the build stage cleanup and are not part
# of the publish output (multi-stage isolation).
COPY --from=build C:\publish .

# Configure IIS application pool to use .NET 4.0 Integrated pipeline
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' managedRuntimeVersion v4.0; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' managedPipelineMode Integrated; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' startMode AlwaysRunning; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' applicationPool DefaultAppPool

# Environment variables - override at runtime via EKS pod spec / ConfigMap / Secrets
# SQL Server connection string for the hospital management database
ENV DB_CONNECTION_STRING=""
# Redis / ElastiCache connection string for distributed session state
ENV REDIS_CONNECTION_STRING=""
# Application environment indicator
ENV ASPNET_ENV="Production"
# Application port (IIS listens on 80 inside the container)
ENV PORT="80"

# Expose HTTP port
EXPOSE 80

# IIS is managed by the base image's ServiceMonitor.exe entry point
# The base image already sets ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
# Kubernetes liveness and readiness probes use /Health.aspx (httpGet)
