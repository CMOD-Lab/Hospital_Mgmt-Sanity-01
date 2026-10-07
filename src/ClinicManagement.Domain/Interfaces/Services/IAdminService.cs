using ClinicManagement.Domain.DTOs;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>Service interface for admin operations.</summary>
public interface IAdminService
{
    Task<AdminHomeDto> GetAdminHomeInformationAsync(CancellationToken cancellationToken = default);
    Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default);
    Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorListItemDto>> GetDoctorsAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientListItemDto>> GetPatientsAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<IEnumerable<StaffListItemDto>> GetStaffAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<bool> DoctorEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
}
