using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for patient-related operations.
/// </summary>
public class PatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all active patients.
    /// </summary>
    public async Task<IEnumerable<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all patients");
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            return patients.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all patients");
            throw;
        }
    }

    /// <summary>
    /// Gets a patient by ID.
    /// </summary>
    public async Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient with ID {PatientId}", id);
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
            return patient == null ? null : MapToDto(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID {PatientId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new patient.
    /// </summary>
    public async Task<PatientDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new patient with email {Email}", dto.Email);

            if (await _patientRepository.EmailExistsAsync(dto.Email, cancellationToken))
            {
                throw new DuplicateEntityException($"A patient with email '{dto.Email}' already exists.");
            }

            var patient = new Patient
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _patientRepository.AddAsync(patient, cancellationToken);
            _logger.LogInformation("Patient created with ID {PatientId}", created.PatientId);
            return MapToDto(created);
        }
        catch (Exception ex) when (ex is not DuplicateEntityException)
        {
            _logger.LogError(ex, "Error creating patient with email {Email}", dto.Email);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing patient.
    /// </summary>
    public async Task UpdateAsync(int id, PatientUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating patient with ID {PatientId}", id);
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new EntityNotFoundException("Patient", id);

            patient.Name = dto.Name;
            patient.Phone = dto.Phone;
            patient.Address = dto.Address;
            patient.BirthDate = dto.BirthDate;
            patient.Gender = dto.Gender;

            await _patientRepository.UpdateAsync(patient, cancellationToken);
            _logger.LogInformation("Patient with ID {PatientId} updated successfully", id);
        }
        catch (Exception ex) when (ex is not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error updating patient with ID {PatientId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a patient by ID.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting patient with ID {PatientId}", id);
            if (!await _patientRepository.ExistsAsync(id, cancellationToken))
            {
                throw new EntityNotFoundException("Patient", id);
            }
            await _patientRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Patient with ID {PatientId} deleted successfully", id);
        }
        catch (Exception ex) when (ex is not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error deleting patient with ID {PatientId}", id);
            throw;
        }
    }

    /// <summary>
    /// Searches patients by name.
    /// </summary>
    public async Task<IEnumerable<PatientDto>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching patients with query '{SearchQuery}'", searchQuery);
            var patients = await _patientRepository.SearchAsync(searchQuery, cancellationToken);
            return patients.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients with query '{SearchQuery}'", searchQuery);
            throw;
        }
    }

    /// <summary>
    /// Validates patient login credentials.
    /// </summary>
    public async Task<LoginResultDto> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for patient email {Email}", email);
            var patient = await _patientRepository.ValidateLoginAsync(email, password, cancellationToken);
            if (patient == null)
            {
                return new LoginResultDto { Success = false, ErrorMessage = "Invalid email or password." };
            }
            return new LoginResultDto
            {
                Success = true,
                UserId = patient.PatientId,
                UserType = "Patient",
                Name = patient.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for patient email {Email}", email);
            throw;
        }
    }

    private static PatientDto MapToDto(Patient patient) => new()
    {
        PatientId = patient.PatientId,
        Name = patient.Name,
        Email = patient.Email,
        Phone = patient.Phone,
        Address = patient.Address,
        BirthDate = patient.BirthDate,
        Gender = patient.Gender,
        IsActive = patient.IsActive
    };
}
