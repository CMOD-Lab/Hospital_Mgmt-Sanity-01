using ClinicManagement.Application.DTOs;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service interface for doctor operations.
/// </summary>
public interface IDoctorService
{
    Task<IEnumerable<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DoctorDto> CreateAsync(DoctorCreateDto createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, DoctorUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
