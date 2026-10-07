# Clinic Management System - .NET 8 Migration

## Overview
This is the migrated version of the Clinic Management System, originally built with ASP.NET Web Forms 4.5.2, now running on .NET 8 with clean architecture.

## Architecture
The solution follows Clean Architecture with four layers:

```
src/
├── ClinicManagement.Domain/          # Domain entities, interfaces, DTOs
├── ClinicManagement.Application/     # Business logic services
├── ClinicManagement.Infrastructure/  # EF Core, repositories
└── ClinicManagement.Web/             # Razor Pages UI

tests/
└── ClinicManagement.UnitTests/       # Unit tests
```

## Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)

## Setup
1. Update the connection string in `src/ClinicManagement.Web/appsettings.json`
2. Run database migrations or use the existing SQL scripts in `Database Files/`
3. Run `dotnet restore`
4. Run `dotnet build`
5. Run `dotnet run --project src/ClinicManagement.Web`

## User Types
- **Patient** (Type=1): Can view doctors, book appointments, view history
- **Doctor** (Type=2): Can manage appointments, update prescriptions, billing
- **Admin** (Type=3): Can manage doctors, staff, view dashboard

## Build Verification
✅ Build succeeded with 0 errors, 0 warnings on .NET 8
