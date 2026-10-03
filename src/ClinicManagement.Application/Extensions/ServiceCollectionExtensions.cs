using ClinicManagement.Application.Mappings;
using ClinicManagement.Application.Services;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper manually
        services.AddSingleton<IMapper>(sp =>
            new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper());

        // Register services
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IBillService, BillService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}
