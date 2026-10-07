using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application.Extensions;

/// <summary>Extension methods for registering Application layer services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers all Application layer services with the DI container.</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, Services.AuthService>();
        services.AddScoped<IAdminService, Services.AdminService>();
        services.AddScoped<IPatientService, Services.PatientService>();
        services.AddScoped<IDoctorService, Services.DoctorService>();

        return services;
    }
}
