# Clinic Management System - .NET 8 Migration

## Overview
This is a complete migration of the Clinic Management System from ASP.NET Web Forms (.NET 4.5.2) to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
ClinicManagement/
├── src/
│   ├── ClinicManagement.Domain/          # Domain entities, interfaces, enums
│   ├── ClinicManagement.Application/     # Business logic, services, DTOs
│   ├── ClinicManagement.Infrastructure/  # EF Core, repositories, data access
│   └── ClinicManagement.Web/             # ASP.NET Core Razor Pages UI
├── tests/
│   └── ClinicManagement.UnitTests/       # Unit tests
└── docs/                                 # Documentation
```

## Features
- **Patient Portal**: Registration, appointment booking, bill history, treatment history, notifications, feedback
- **Doctor Portal**: Dashboard, pending appointments, patient history, prescription updates, billing
- **Admin Portal**: Dashboard with statistics, doctor registration, staff management, clinic management

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)

### Database Setup
1. Run the SQL scripts in `Database Files/` folder
2. Update the connection string in `src/ClinicManagement.Web/appsettings.json`

### Running the Application
```bash
cd src/ClinicManagement.Web
dotnet run
```

### Building
```bash
dotnet restore ClinicManagement.sln
dotnet build ClinicManagement.sln
```

## Configuration
Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=DBProject;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

**Note**: Add the `Microsoft.EntityFrameworkCore.SqlServer` package and configure `UseSqlServer()` in `Infrastructure/Extensions/ServiceCollectionExtensions.cs` for production use.

## Migration Notes
- Web Forms pages migrated to Razor Pages
- ADO.NET replaced with Entity Framework Core 8
- Web.config replaced with appsettings.json
- Global.asax replaced with Program.cs
- Session state preserved using ASP.NET Core session middleware
- System.Web dependencies removed
- Authentication uses session-based approach (can be upgraded to ASP.NET Core Identity)
