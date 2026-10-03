using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for department-related operations.
/// </summary>
public class DepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all departments");
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return departments.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DeptName = d.DeptName,
                Description = d.Description,
                DoctorCount = d.Doctors?.Count ?? 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all departments");
            throw;
        }
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var dept = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            if (dept == null) return null;
            return new DepartmentDto
            {
                DepartmentId = dept.DepartmentId,
                DeptName = dept.DeptName,
                Description = dept.Description,
                DoctorCount = dept.Doctors?.Count ?? 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with ID {DepartmentId}", id);
            throw;
        }
    }

    public async Task<DepartmentDto> CreateAsync(string deptName, string description, CancellationToken cancellationToken = default)
    {
        try
        {
            var dept = new Department { DeptName = deptName, Description = description, IsActive = true };
            var created = await _departmentRepository.AddAsync(dept, cancellationToken);
            return new DepartmentDto { DepartmentId = created.DepartmentId, DeptName = created.DeptName, Description = created.Description };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department {DeptName}", deptName);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await _departmentRepository.ExistsAsync(id, cancellationToken))
                throw new EntityNotFoundException("Department", id);
            await _departmentRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex) when (ex is not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error deleting department with ID {DepartmentId}", id);
            throw;
        }
    }
}
