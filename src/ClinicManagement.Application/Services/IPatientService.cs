using ClinicManagement.Application.DTOs;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service interface for patient operations.
/// </summary>
public interface IPatientService
{
    Task<IEnumerable<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PatientDto> CreateAsync(PatientCreateDto createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, PatientUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientDto>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<LoginResultDto> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
