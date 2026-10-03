# Migration Notes

## What Was Migrated

### From ASP.NET Web Forms 4.5.2 to .NET 8

| Old Component | New Component |
|---------------|---------------|
| `.aspx` pages | Razor Pages (`.cshtml`) |
| Code-behind (`.aspx.cs`) | Page Models (`.cshtml.cs`) |
| Master pages (`.master`) | Layout pages (`_Layout.cshtml`) |
| `Web.config` | `appsettings.json` |
| `packages.config` | `<PackageReference>` in `.csproj` |
| ADO.NET + Stored Procedures | Entity Framework Core 8.0 |
| `System.Web` | ASP.NET Core equivalents |
| Session via `System.Web` | `ISession` via `IHttpContextAccessor` |
| `ConfigurationManager` | `IConfiguration` |

## Key Differences from Web Forms

1. **No ViewState**: State is managed via session or TempData
2. **No Code-Behind**: Logic is in Page Models with proper separation
3. **Dependency Injection**: All services are injected via constructor
4. **Async/Await**: All I/O operations are async
5. **Clean Architecture**: Domain, Application, Infrastructure, Web layers

## Breaking Changes

1. **Authentication**: Simplified session-based auth (no Forms Authentication)
2. **Database Access**: EF Core instead of ADO.NET stored procedures
3. **Configuration**: `appsettings.json` instead of `Web.config`
4. **Routing**: Razor Pages routing instead of WebForms URL mapping

## Configuration Changes

- Connection string moved from `Web.config` to `appsettings.json`
- ApplicationInsights removed (not needed for .NET 8 basic setup)
- Logging configured via Serilog

## Known Issues

1. Database schema must match the entity configurations
2. Stored procedures from original app are replaced by EF Core LINQ queries
3. Admin login requires manual database entry (no admin registration page)

## Future Improvements

1. Add ASP.NET Core Identity for proper authentication
2. Add JWT tokens for API support
3. Add pagination for large data sets
4. Add email notifications
5. Add proper admin registration flow
