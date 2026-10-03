using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for doctor-related operations.
/// </summary>
public class DoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IDoctorRepository doctorRepository, ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all active doctors.
    /// </summary>
    public async Task<IEnumerable<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all doctors");
            var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
            return doctors.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all doctors");
            throw;
        }
    }

    /// <summary>
    /// Gets a doctor by ID.
    /// </summary>
    public async Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor with ID {DoctorId}", id);
            var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken);
            return doctor == null ? null : MapToDto(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with ID {DoctorId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new doctor.
    /// </summary>
    public async Task<DoctorDto> CreateAsync(DoctorCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new doctor with email {Email}", dto.Email);

            if (await _doctorRepository.EmailExistsAsync(dto.Email, cancellationToken))
            {
                throw new DuplicateEntityException($"A doctor with email '{dto.Email}' already exists.");
            }

            var doctor = new Doctor
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                DepartmentId = dto.DepartmentId,
                Specialization = dto.Specialization,
                Qualification = dto.Qualification,
                Experience = dto.Experience,
                Salary = dto.Salary,
                ChargesPerVisit = dto.ChargesPerVisit,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _doctorRepository.AddAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor created with ID {DoctorId}", created.DoctorId);
            return MapToDto(created);
        }
        catch (Exception ex) when (ex is not DuplicateEntityException)
        {
            _logger.LogError(ex, "Error creating doctor with email {Email}", dto.Email);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing doctor.
    /// </summary>
    public async Task UpdateAsync(int id, DoctorUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating doctor with ID {DoctorId}", id);
            var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new EntityNotFoundException("Doctor", id);

            doctor.Name = dto.Name;
            doctor.Phone = dto.Phone;
            doctor.Address = dto.Address;
            doctor.Specialization = dto.Specialization;
            doctor.Qualification = dto.Qualification;
            doctor.Experience = dto.Experience;
            doctor.Salary = dto.Salary;
            doctor.ChargesPerVisit = dto.ChargesPerVisit;

            await _doctorRepository.UpdateAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor with ID {DoctorId} updated successfully", id);
        }
        catch (Exception ex) when (ex is not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error updating doctor with ID {DoctorId}", id);
            throw;
        }
    }

    /// <summary>
    /// Soft-deletes a doctor by ID.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting doctor with ID {DoctorId}", id);
            if (!await _doctorRepository.ExistsAsync(id, cancellationToken))
            {
                throw new EntityNotFoundException("Doctor", id);
            }
            await _doctorRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Doctor with ID {DoctorId} deleted successfully", id);
        }
        catch (Exception ex) when (ex is not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error deleting doctor with ID {DoctorId}", id);
            throw;
        }
    }

    /// <summary>
    /// Searches doctors by name.
    /// </summary>
    public async Task<IEnumerable<DoctorDto>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching doctors with query '{SearchQuery}'", searchQuery);
            var doctors = await _doctorRepository.SearchAsync(searchQuery, cancellationToken);
            return doctors.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors with query '{SearchQuery}'", searchQuery);
            throw;
        }
    }

    /// <summary>
    /// Gets doctors by department name.
    /// </summary>
    public async Task<IEnumerable<DoctorDto>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctors for department '{DepartmentName}'", departmentName);
            var doctors = await _doctorRepository.GetByDepartmentAsync(departmentName, cancellationToken);
            return doctors.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department '{DepartmentName}'", departmentName);
            throw;
        }
    }

    /// <summary>
    /// Validates doctor login credentials.
    /// </summary>
    public async Task<LoginResultDto> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for doctor email {Email}", email);
            var doctor = await _doctorRepository.ValidateLoginAsync(email, password, cancellationToken);
            if (doctor == null)
            {
                return new LoginResultDto { Success = false, ErrorMessage = "Invalid email or password." };
            }
            return new LoginResultDto
            {
                Success = true,
                UserId = doctor.DoctorId,
                UserType = "Doctor",
                Name = doctor.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for doctor email {Email}", email);
            throw;
        }
    }

    private static DoctorDto MapToDto(Doctor doctor) => new()
    {
        DoctorId = doctor.DoctorId,
        Name = doctor.Name,
        Email = doctor.Email,
        Phone = doctor.Phone,
        Address = doctor.Address,
        BirthDate = doctor.BirthDate,
        Gender = doctor.Gender,
        DepartmentId = doctor.DepartmentId,
        DepartmentName = doctor.Department?.DeptName ?? string.Empty,
        Specialization = doctor.Specialization,
        Qualification = doctor.Qualification,
        Experience = doctor.Experience,
        Salary = doctor.Salary,
        ChargesPerVisit = doctor.ChargesPerVisit,
        ReputeIndex = doctor.ReputeIndex,
        PatientsTreated = doctor.PatientsTreated,
        IsActive = doctor.IsActive
    };
}
