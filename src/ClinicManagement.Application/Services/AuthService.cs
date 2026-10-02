using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Authentication service handling login and patient signup.</summary>
public class AuthService : IAuthService
{
    private readonly ILoginRepository _loginRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ILoginRepository loginRepo, IPatientRepository patientRepo, ILogger<AuthService> logger)
    {
        _loginRepo = loginRepo;
        _patientRepo = patientRepo;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<LoginResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        try
        {
            var login = await _loginRepo.GetByEmailAsync(email, ct);
            if (login == null)
                return new LoginResult(false, 0, UserType.Patient, "Email not found. Try Again!");

            if (login.Password != password)
                return new LoginResult(false, 0, UserType.Patient, "Incorrect Password. Try Again!");

            _logger.LogInformation("User {Email} logged in successfully as {Type}", email, login.Type);
            return new LoginResult(true, login.LoginId, login.Type, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", email);
            return new LoginResult(false, 0, UserType.Patient, "There was some error. Try Again!");
        }
    }

    /// <inheritdoc/>
    public async Task<SignupResult> SignupPatientAsync(string name, string birthDate, string email, string password, string phone, string gender, string address, CancellationToken ct = default)
    {
        try
        {
            if (await _loginRepo.EmailExistsAsync(email, ct))
                return new SignupResult(false, 0, "Email already exists. Please choose a different one.");

            var login = new LoginTable
            {
                Email = email,
                Password = password,
                Type = UserType.Patient
            };

            var createdLogin = await _loginRepo.AddAsync(login, ct);

            var patient = new Patient
            {
                PatientId = createdLogin.LoginId,
                Name = name,
                Phone = phone,
                Address = address,
                BirthDate = DateTime.Parse(birthDate),
                Gender = string.IsNullOrEmpty(gender) ? 'M' : gender[0]
            };

            await _patientRepo.AddAsync(patient, ct);

            _logger.LogInformation("Patient {Name} registered with ID {Id}", name, createdLogin.LoginId);
            return new SignupResult(true, createdLogin.LoginId, "Registration Successful!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient signup for {Email}", email);
            return new SignupResult(false, 0, "There was some error. Try again!");
        }
    }
}
