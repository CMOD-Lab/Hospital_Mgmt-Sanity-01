using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for doctor-related operations.
/// </summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IDoctorRepository doctorRepository, IMapper mapper, ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all doctors");
            var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all doctors");
            throw;
        }
    }

    public async Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor with ID {DoctorId}", id);
            var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken);
            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with ID {DoctorId}", id);
            throw;
        }
    }

    public async Task<DoctorDto> CreateAsync(DoctorCreateDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new doctor with email {Email}", createDto.Email);
            if (await _doctorRepository.EmailExistsAsync(createDto.Email, cancellationToken))
                throw new DuplicateEntityException($"A doctor with email '{createDto.Email}' already exists.");

            var doctor = _mapper.Map<Doctor>(createDto);
            var created = await _doctorRepository.AddAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor created with ID {DoctorId}", created.DoctorId);
            return _mapper.Map<DoctorDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor with email {Email}", createDto.Email);
            throw;
        }
    }

    public async Task UpdateAsync(int id, DoctorUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating doctor with ID {DoctorId}", id);
            var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Doctor), id);

            doctor.Name = updateDto.Name;
            doctor.Phone = updateDto.Phone;
            doctor.Address = updateDto.Address;
            doctor.Specialization = updateDto.Specialization;
            doctor.Qualification = updateDto.Qualification;
            doctor.Experience = updateDto.Experience;
            doctor.Salary = updateDto.Salary;
            doctor.ChargesPerVisit = updateDto.ChargesPerVisit;

            await _doctorRepository.UpdateAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor {DoctorId} updated successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor with ID {DoctorId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting doctor with ID {DoctorId}", id);
            if (!await _doctorRepository.ExistsAsync(id, cancellationToken))
                throw new NotFoundException(nameof(Doctor), id);

            await _doctorRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Doctor {DoctorId} deleted successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor with ID {DoctorId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<DoctorDto>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching doctors with query '{SearchQuery}'", searchQuery);
            var doctors = await _doctorRepository.SearchAsync(searchQuery, cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors");
            throw;
        }
    }

    public async Task<IEnumerable<DoctorDto>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctors for department '{DepartmentName}'", departmentName);
            var doctors = await _doctorRepository.GetByDepartmentAsync(departmentName, cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department '{DepartmentName}'", departmentName);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _doctorRepository.EmailExistsAsync(email, cancellationToken);
    }
}
