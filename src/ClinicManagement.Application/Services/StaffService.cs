using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for staff-related operations.
/// </summary>
public class StaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly ILogger<StaffService> _logger;

    public StaffService(IStaffRepository staffRepository, ILogger<StaffService> logger)
    {
        _staffRepository = staffRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<StaffDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all staff");
            var staff = await _staffRepository.GetAllAsync(cancellationToken);
            return staff.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all staff");
            throw;
        }
    }

    public async Task<StaffDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving staff with ID {StaffId}", id);
            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            return staff == null ? null : MapToDto(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with ID {StaffId}", id);
            throw;
        }
    }

    public async Task<StaffDto> CreateAsync(StaffCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new staff member {Name}", dto.Name);
            var staff = new Staff
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                Designation = dto.Designation,
                Qualification = dto.Qualification,
                Salary = dto.Salary,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            var created = await _staffRepository.AddAsync(staff, cancellationToken);
            _logger.LogInformation("Staff member created with ID {StaffId}", created.StaffId);
            return MapToDto(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff member {Name}", dto.Name);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff with ID {StaffId}", id);
            if (!await _staffRepository.ExistsAsync(id, cancellationToken))
            {
                throw new EntityNotFoundException("Staff", id);
            }
            await _staffRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Staff with ID {StaffId} deleted successfully", id);
        }
        catch (Exception ex) when (ex is not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error deleting staff with ID {StaffId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<StaffDto>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching staff with query '{SearchQuery}'", searchQuery);
            var staff = await _staffRepository.SearchAsync(searchQuery, cancellationToken);
            return staff.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff with query '{SearchQuery}'", searchQuery);
            throw;
        }
    }

    private static StaffDto MapToDto(Staff s) => new()
    {
        StaffId = s.StaffId,
        Name = s.Name,
        Phone = s.Phone,
        Address = s.Address,
        BirthDate = s.BirthDate,
        Gender = s.Gender,
        Designation = s.Designation,
        Qualification = s.Qualification,
        Salary = s.Salary,
        IsActive = s.IsActive
    };
}
