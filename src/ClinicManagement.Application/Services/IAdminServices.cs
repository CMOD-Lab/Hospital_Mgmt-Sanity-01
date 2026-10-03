using ClinicManagement.Application.DTOs;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service interface for staff operations.
/// </summary>
public interface IStaffService
{
    Task<IEnumerable<StaffDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StaffDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StaffDto> CreateAsync(StaffCreateDto createDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StaffDto>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for department operations.
/// </summary>
public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for bill operations.
/// </summary>
public interface IBillService
{
    Task<IEnumerable<BillDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BillDto>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task MarkAsPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task MarkAsUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for admin dashboard.
/// </summary>
public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default);
}
