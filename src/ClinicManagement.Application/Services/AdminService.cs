using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for admin dashboard operations.
/// </summary>
public class AdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IBillRepository _billRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IBillRepository billRepository,
        IDepartmentRepository departmentRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _billRepository = billRepository;
        _departmentRepository = departmentRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets the admin dashboard summary data.
    /// </summary>
    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving admin dashboard data");

            var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var totalIncome = await _billRepository.GetTotalIncomeAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);

            return new AdminDashboardDto
            {
                TotalDoctors = doctors.Count(),
                TotalPatients = patients.Count(),
                TotalIncome = totalIncome,
                Departments = departments.Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    DeptName = d.DeptName,
                    Description = d.Description,
                    DoctorCount = d.Doctors?.Count ?? 0
                }),
                RecentAppointments = appointments.Take(10).Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient?.Name ?? string.Empty,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor?.Name ?? string.Empty,
                    TimeSlotId = a.TimeSlotId,
                    Timings = a.TimeSlot?.Timings ?? string.Empty,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin dashboard data");
            throw;
        }
    }

    /// <summary>
    /// Validates admin login.
    /// </summary>
    public LoginResultDto ValidateAdminLogin(string email, string password)
    {
        // In a real system, admin credentials would be stored securely
        // For this migration, we use a simple check that can be configured
        if (email == "admin@clinic.com" && password == "Admin@123")
        {
            return new LoginResultDto
            {
                Success = true,
                UserId = 1,
                UserType = "Admin",
                Name = "Administrator"
            };
        }
        return new LoginResultDto { Success = false, ErrorMessage = "Invalid admin credentials." };
    }
}
