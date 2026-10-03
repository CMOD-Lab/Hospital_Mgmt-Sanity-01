# Clinic Management System - .NET 8 Migration

## Overview
This is the migrated version of the Clinic Management System, originally built with ASP.NET Web Forms 4.5.2, now fully migrated to .NET 8 with clean architecture.

## Architecture
The solution follows **Clean Architecture** with four main layers:

```
ClinicManagement.sln
├── src/
│   ├── ClinicManagement.Domain          # Domain entities, interfaces, exceptions
│   ├── ClinicManagement.Application     # Business logic services, DTOs
│   ├── ClinicManagement.Infrastructure  # EF Core, repositories, data access
│   └── ClinicManagement.Web             # ASP.NET Core Razor Pages UI
└── tests/
    ├── ClinicManagement.UnitTests        # xUnit unit tests
    └── ClinicManagement.IntegrationTests # Integration tests
```

## Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)

## Setup Instructions

### 1. Configure Database Connection
Update `src/ClinicManagement.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DBProject;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

### 2. Run Database Migrations
```bash
cd src/ClinicManagement.Web
dotnet ef database update
```

### 3. Run the Application
```bash
cd src/ClinicManagement.Web
dotnet run
```

## User Roles
- **Admin**: Manage doctors, patients, and staff. Default: `admin@clinic.com` / `Admin@123`
- **Doctor**: View appointments, update patient history, manage billing
- **Patient**: Register, book appointments, view treatment history and bills

## Key Features
- Patient registration and login
- Doctor management (Admin)
- Appointment booking and management
- Treatment history tracking
- Billing management
- Patient feedback system

## Migration Notes
- Replaced ADO.NET with Entity Framework Core 8.0
- Replaced Web.config with appsettings.json
- Replaced System.Web with ASP.NET Core equivalents
- Replaced WebForms pages with Razor Pages
- Replaced master pages with Razor layout pages
- Added proper dependency injection throughout
- Added async/await patterns for all I/O operations
- Added structured logging with Serilog
