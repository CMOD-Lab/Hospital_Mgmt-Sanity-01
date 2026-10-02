using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Infrastructure.Extensions;

/// <summary>Extension methods for registering Infrastructure layer services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers all Infrastructure layer services with the DI container.</summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        // NOTE: To use SQL Server, install Microsoft.EntityFrameworkCore.SqlServer package
        // and replace the options configuration with: options.UseSqlServer(connectionString)
        services.AddDbContext<ClinicDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=clinic.db";

            // Configure using the provider specified in configuration
            var provider = configuration["DatabaseProvider"] ?? "SqlServer";

            // Default configuration - actual provider must be configured at startup
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging(false);
        });

        // Register repositories
        services.AddScoped<ILoginRepository, LoginRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        return services;
    }
}
