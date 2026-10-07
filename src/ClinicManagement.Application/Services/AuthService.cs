using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for authentication operations.</summary>
public class AuthService : IAuthService
{
    private readonly ILoginRepository _loginRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ILoginRepository loginRepository,
        IPatientRepository patientRepository,
        ILogger<AuthService> logger)
    {
        _loginRepository = loginRepository;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>Validates user login credentials.</summary>
    public async Task<LoginResultDto> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email: {Email}", email);

            var login = await _loginRepository.GetByEmailAsync(email, cancellationToken);

            if (login == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", email);
                return new LoginResultDto { Status = 1, ErrorMessage = "Email not found. Try Again!" };
            }

            if (login.Password != password)
            {
                _logger.LogWarning("Incorrect password for email: {Email}", email);
                return new LoginResultDto { Status = 2, ErrorMessage = "Incorrect Password. Try Again!" };
            }

            _logger.LogInformation("Successful login for user ID: {UserId}, Type: {Type}", login.LoginID, login.Type);
            return new LoginResultDto
            {
                Status = 0,
                UserId = login.LoginID,
                UserType = login.Type
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email: {Email}", email);
            return new LoginResultDto { Status = -1, ErrorMessage = "There was some error. Try Again!" };
        }
    }

    /// <summary>Registers a new patient.</summary>
    public async Task<SignUpResultDto> SignUpPatientAsync(PatientSignUpDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Signing up new patient with email: {Email}", dto.Email);

            // Check if email already exists
            if (await _loginRepository.EmailExistsAsync(dto.Email, cancellationToken))
            {
                _logger.LogWarning("Sign-up attempt with existing email: {Email}", dto.Email);
                return new SignUpResultDto { Status = 0, ErrorMessage = "Email already exists. Please choose a different one." };
            }

            // Create login entry
            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 1 // Patient
            };

            var createdLogin = await _loginRepository.AddAsync(login, cancellationToken);

            // Parse birth date
            if (!DateTime.TryParse(dto.BirthDate, out var birthDate))
            {
                return new SignUpResultDto { Status = -1, ErrorMessage = "Invalid birth date format." };
            }

            // Create patient entry
            var patient = new Patient
            {
                PatientID = createdLogin.LoginID,
                Name = dto.Name,
                Phone = dto.PhoneNo,
                Address = dto.Address,
                BirthDate = birthDate,
                Gender = string.IsNullOrEmpty(dto.Gender) ? 'M' : dto.Gender[0]
            };

            await _patientRepository.AddAsync(patient, cancellationToken);

            _logger.LogInformation("Patient registered successfully with ID: {PatientId}", createdLogin.LoginID);
            return new SignUpResultDto { Status = 1, PatientId = createdLogin.LoginID };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing up patient with email: {Email}", dto.Email);
            return new SignUpResultDto { Status = -1, ErrorMessage = "There was some error. Try again!" };
        }
    }
}
