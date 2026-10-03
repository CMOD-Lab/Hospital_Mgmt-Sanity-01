# Clinic Management System - .NET 8 Migration

## Overview
This is the migrated version of the Clinic Management System, originally built with ASP.NET Web Forms 4.5.2, now fully migrated to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
ClinicManagement/
├── src/
│   ├── ClinicManagement.Domain/          # Domain entities, interfaces, enums
│   ├── ClinicManagement.Application/     # Business logic, services, DTOs
│   ├── ClinicManagement.Infrastructure/  # EF Core, repositories, data access
│   └── ClinicManagement.Web/             # Razor Pages UI layer
├── tests/
│   ├── ClinicManagement.UnitTests/       # Unit tests for services
│   └── ClinicManagement.IntegrationTests/ # Integration tests for repositories
└── docs/                                 # Documentation
```

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)

### Configuration
1. Update the connection string in `src/ClinicManagement.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DBProject;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

2. Run database migrations:
```bash
cd src/ClinicManagement.Web
dotnet ef database update
```

### Running the Application
```bash
cd src/ClinicManagement.Web
dotnet run
```

### Running Tests
```bash
dotnet test
```

## User Roles
- **Admin** (UserType=0): Manage doctors, staff, and view patients
- **Doctor** (UserType=1): View appointments, update patient history, manage bills
- **Patient** (UserType=2): Book appointments, view history, check bills

## Key Features
- Patient registration and login
- Doctor registration by admin
- Appointment booking and management
- Treatment history tracking
- Bill management
- Department-based doctor search
