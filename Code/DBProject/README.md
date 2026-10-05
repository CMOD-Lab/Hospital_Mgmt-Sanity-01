# Clinic Management System - .NET 8 Migration

## Overview
This is the migrated version of the Clinic Management System, converted from ASP.NET Web Forms 4.5.2 to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
ClinicManagement.sln
├── src/
│   ├── ClinicManagement.Domain          # Entities, Interfaces
│   ├── ClinicManagement.Application     # Services, DTOs
│   ├── ClinicManagement.Infrastructure  # EF Core, Repositories
│   └── ClinicManagement.Web             # Razor Pages, Program.cs
└── tests/
    └── ClinicManagement.UnitTests       # xUnit tests
```

## Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code

## Setup

### 1. Database Configuration
Update the connection string in `src/ClinicManagement.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=DBProject;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

### 2. Build
```bash
cd src/ClinicManagement.Web
dotnet restore
dotnet build
```

### 3. Run
```bash
dotnet run --project src/ClinicManagement.Web
```

## User Roles
- **Patient (Type=1)**: Register, book appointments, view history
- **Doctor (Type=2)**: Manage appointments, update prescriptions, billing
- **Admin (Type=3)**: Manage doctors, staff, view statistics

## Migration Notes
- Web Forms (.aspx) → Razor Pages (.cshtml)
- ADO.NET → Entity Framework Core 8.0
- Web.config → appsettings.json
- Global.asax → Program.cs
- System.Web → ASP.NET Core
- Session state preserved using distributed memory cache
