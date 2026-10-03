using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for patient-related operations.
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository patientRepository, IMapper mapper, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
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
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
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
            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID {PatientId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new patient (signup).
    /// </summary>
    public async Task<PatientDto> CreateAsync(PatientCreateDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new patient with email {Email}", createDto.Email);
            var existing = await _patientRepository.GetByEmailAsync(createDto.Email, cancellationToken);
            if (existing != null)
                throw new DuplicateEntityException($"A patient with email '{createDto.Email}' already exists.");

            var patient = _mapper.Map<Patient>(createDto);
            var created = await _patientRepository.AddAsync(patient, cancellationToken);
            _logger.LogInformation("Patient created with ID {PatientId}", created.PatientId);
            return _mapper.Map<PatientDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient with email {Email}", createDto.Email);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing patient.
    /// </summary>
    public async Task UpdateAsync(int id, PatientUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating patient with ID {PatientId}", id);
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Patient), id);

            patient.Name = updateDto.Name;
            patient.Phone = updateDto.Phone;
            patient.Address = updateDto.Address;
            patient.Gender = updateDto.Gender;

            await _patientRepository.UpdateAsync(patient, cancellationToken);
            _logger.LogInformation("Patient {PatientId} updated successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient with ID {PatientId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes (deactivates) a patient.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting patient with ID {PatientId}", id);
            if (!await _patientRepository.ExistsAsync(id, cancellationToken))
                throw new NotFoundException(nameof(Patient), id);

            await _patientRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Patient {PatientId} deleted successfully", id);
        }
        catch (Exception ex)
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
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients with query '{SearchQuery}'", searchQuery);
            throw;
        }
    }

    /// <summary>
    /// Validates login credentials.
    /// </summary>
    public async Task<LoginResultDto> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email {Email}", email);
            var (id, type) = await _patientRepository.ValidateLoginAsync(email, password, cancellationToken);
            if (id <= 0)
                return new LoginResultDto { Success = false, Message = "Invalid email or password." };

            return new LoginResultDto { Success = true, UserId = id, UserType = type };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email {Email}", email);
            throw;
        }
    }
}
