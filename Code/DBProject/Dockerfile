# ============================================================
# Dockerfile for Clinic Management System (ASP.NET Web Forms)
# Technology  : ASP.NET Web Forms on .NET Framework 4.5.2
# Base Image  : mcr.microsoft.com/dotnet/framework/aspnet:4.5.2 (explicit)
# Build Image : mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019
# Target      : Windows Container on AWS EKS Windows node groups
# IIS site    : Default Web Site (port 80)
#
# Multi-stage build optimisations:
#   - Stage 1 (build): SDK image restores NuGet packages and publishes artefacts
#   - Stage 2 (runtime): Lean aspnet image receives only published output
#   - Post-publish cleanup removes PDB, source, obj/ from publish folder
#   - Release build with /p:DebugType=None suppresses PDB generation
# ============================================================

# ── Stage 1: Build ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR C:\\build

# Copy solution / project files first so NuGet restore layer is cached
COPY ["Code/DBProject/packages.config", "Code/DBProject/packages.config"]
COPY ["Code/DBProject/Clinic Management System.csproj", "Code/DBProject/Clinic Management System.csproj"]
COPY ["Code/DBProject/DBProject.sln", "Code/DBProject/DBProject.sln"]

# Restore NuGet packages (cached layer)
RUN nuget restore "Code/DBProject/packages.config" \
        -PackagesDirectory packages \
        -NonInteractive

# Copy the rest of the source code
COPY . .

# Build and publish the web application to C:\publish
# /p:DebugType=None  – suppresses PDB output entirely
# /p:DebugSymbols=false – no symbol files in publish output
RUN msbuild "Code/DBProject/DBProject.sln" \
        /p:Configuration=Release \
        /p:DeployOnBuild=true \
        /p:WebPublishMethod=FileSystem \
        /p:PublishUrl=C:\\publish \
        /p:DeleteExistingFiles=True \
        /p:SkipInvalidConfigurations=true \
        /p:DebugType=None \
        /p:DebugSymbols=false \
        /nologo \
        /verbosity:minimal

# Post-publish cleanup: remove PDB, source, obj/ and temp artefacts
RUN powershell -Command \
        "Get-ChildItem -Path C:\\publish -Recurse -Include *.pdb,*.cs,*.vb,*.csproj,*.sln,*.user,*.cache,*.tmp,*.log | Remove-Item -Force -ErrorAction SilentlyContinue; \
         if (Test-Path C:\\publish\\obj)  { Remove-Item -Recurse -Force C:\\publish\\obj  }; \
         if (Test-Path C:\\publish\\bin\\roslyn) { Remove-Item -Recurse -Force C:\\publish\\bin\\roslyn }; \
         Write-Host 'Post-publish cleanup complete.'"

# ── Stage 2: Runtime ────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.5.2 AS runtime

LABEL maintainer="Clinic Management System" \
      application="clinic-management-system" \
      version="1.0" \
      description="ASP.NET Web Forms Clinic Management System containerized for AWS EKS Windows node groups"

# Configure IIS and Application Pool for .NET Framework 4.5.2
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\\AppPools\\DefaultAppPool' managedRuntimeVersion 'v4.0'; \
    Set-ItemProperty 'IIS:\\AppPools\\DefaultAppPool' enable32BitAppOnWin64 $false; \
    Set-ItemProperty 'IIS:\\AppPools\\DefaultAppPool' processModel.idleTimeout ([TimeSpan]::FromMinutes(0)); \
    Remove-Item -Recurse -Force C:\\inetpub\\wwwroot\\*

# Copy ONLY the lean published artefacts from the build stage
COPY --from=build C:\\publish C:\\inetpub\\wwwroot

# ── Environment variables (override at runtime via K8s ConfigMap / Secret) ──
ENV DB_CONNECTION_STRING="" \
    REDIS_CONNECTION_STRING="" \
    ASPNET_ENVIRONMENT="Production"

# ── Expose HTTP port ─────────────────────────────────────────
EXPOSE 80

# ── Entry point: IIS Service Monitor ─────────────────────────
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
