using AutoMapper;
using ClinicManagement.Application.Mappings;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application.Extensions;

/// <summary>Extension methods for registering Application layer services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers all Application layer services with the DI container.</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper manually (without AutoMapper.Extensions.Microsoft.DependencyInjection)
        services.AddSingleton<IMapper>(sp =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        });

        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IDoctorService, DoctorService>();

        return services;
    }
}
