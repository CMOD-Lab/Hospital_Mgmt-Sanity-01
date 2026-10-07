using ClinicManagement.Domain.DTOs;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>Service interface for authentication operations.</summary>
public interface IAuthService
{
    Task<LoginResultDto> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<SignUpResultDto> SignUpPatientAsync(PatientSignUpDto dto, CancellationToken cancellationToken = default);
}
