# ============================================================
# Dockerfile for ASP.NET Web Forms – Clinic Management System
# Base Image  : mcr.microsoft.com/dotnet/framework/aspnet:4.5.2 (explicit)
# Build Image : mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019
# Target      : AWS EKS cluster with Windows node groups
# IIS site    : Default Web Site (port 80)
# Health      : GET /Health.aspx  →  HTTP 200 {"status":"healthy",...}
# ============================================================

# ── Stage 1: Build ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

WORKDIR C:\\build

# Copy solution / project files first so NuGet restore is cached
COPY ["Code/DBProject/Clinic Management System.csproj", "Code/DBProject/"]
COPY ["Code/DBProject/packages.config", "Code/DBProject/"]

# Restore NuGet packages
RUN nuget restore "Code/DBProject/packages.config" \
        -PackagesDirectory Code/packages \
        -NonInteractive

# Copy the rest of the source
COPY . .

# Build & publish the web application
RUN msbuild "Code/DBProject/Clinic Management System.csproj" \
        /p:Configuration=Release \
        /p:DeployOnBuild=true \
        /p:WebPublishMethod=FileSystem \
        /p:PublishUrl=C:\\publish \
        /p:DeleteExistingFiles=True \
        /p:DebugType=None \
        /p:DebugSymbols=false \
        /nologo \
        /verbosity:minimal

# Cleanup: remove PDB files and temp artefacts from publish output
RUN powershell -NoProfile -Command \
    "Get-ChildItem -Path 'C:\\publish' -Recurse -Include '*.pdb' | Remove-Item -Force; \
     Get-ChildItem -Path 'C:\\publish' -Recurse -Include '*.xml' -Exclude 'ApplicationInsights.config' | Remove-Item -Force; \
     Get-ChildItem -Path 'C:\\publish' -Recurse -Include '*.tmp','*.temp','*.cache' | Remove-Item -Force; \
     if (Test-Path 'C:\\publish\\bin\\roslyn') { Remove-Item -Path 'C:\\publish\\bin\\roslyn' -Recurse -Force }"

# ── Stage 2: Runtime ────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.5.2 AS runtime

# Configure IIS site for the application
RUN powershell -NoProfile -Command \
    "Import-Module WebAdministration; \
     Remove-WebSite -Name 'Default Web Site' -ErrorAction SilentlyContinue; \
     New-WebSite -Name 'HospitalMgmt' \
                 -Port 80 \
                 -PhysicalPath 'C:\inetpub\wwwroot\HospitalMgmt' \
                 -Force; \
     Set-ItemProperty 'IIS:\AppPools\HospitalMgmt' -Name enable32BitAppOnWin64 -Value $false; \
     Set-ItemProperty 'IIS:\AppPools\HospitalMgmt' -Name managedPipelineMode -Value 0; \
     Set-ItemProperty 'IIS:\AppPools\HospitalMgmt' -Name managedRuntimeVersion -Value 'v4.0'"

# Copy ONLY the published artefacts (no source, no PDB files)
WORKDIR C:\\inetpub\\wwwroot\\HospitalMgmt
COPY --from=build C:\\publish .

# Environment variables (injected at runtime via EKS ConfigMap / Secret)
ENV DB_CONNECTION_STRING="" \
    REDIS_HOST="" \
    REDIS_PORT="6379" \
    REDIS_ACCESS_KEY=""

# Expose HTTP port
EXPOSE 80

# IIS is managed by the base image's ServiceMonitor.exe entry-point
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
