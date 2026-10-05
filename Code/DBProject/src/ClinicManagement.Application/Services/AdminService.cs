using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for admin-related operations.
/// </summary>
public class AdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IStaffRepository staffRepository,
        IDepartmentRepository departmentRepository,
        ILoginRepository loginRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _staffRepository = staffRepository;
        _departmentRepository = departmentRepository;
        _loginRepository = loginRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets admin home statistics.
    /// </summary>
    public async Task<AdminHomeDto> GetAdminHomeInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);

            var deptStats = departments.Select(d => new DepartmentStatsDto
            {
                DeptName = d.DeptName,
                DoctorCount = d.Doctors.Count(doc => doc.Status == 1)
            });

            return new AdminHomeDto
            {
                TotalPatients = patients.Count(),
                TotalDoctors = doctors.Count(),
                DepartmentStats = deptStats
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin home info");
            return new AdminHomeDto();
        }
    }

    /// <summary>
    /// Adds a new staff member.
    /// </summary>
    public async Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!DateTime.TryParse(dto.BirthDate, out var birthDate))
                birthDate = DateTime.Today;

            var staff = new OtherStaff
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                Gender = dto.Gender,
                BirthDate = birthDate,
                Salary = dto.Salary,
                Designation = dto.Designation,
                HighestQualification = dto.Qualification
            };

            await _staffRepository.AddAsync(staff, cancellationToken);
            _logger.LogInformation("Staff added: {Name}", dto.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff: {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>
    /// Deletes a staff member.
    /// </summary>
    public async Task<bool> DeleteStaffAsync(int staffId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _staffRepository.DeleteAsync(staffId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff ID: {StaffId}", staffId);
            return false;
        }
    }

    /// <summary>
    /// Gets all staff, optionally filtered by search query.
    /// </summary>
    public async Task<IEnumerable<StaffDto>> GetStaffAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<OtherStaff> staff;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                staff = await _staffRepository.GetAllAsync(cancellationToken);
            }
            else
            {
                staff = await _staffRepository.SearchAsync(searchQuery, cancellationToken);
            }

            return staff.Select(s => new StaffDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Phone = s.Phone,
                Address = s.Address,
                Gender = s.Gender.ToString(),
                Designation = s.Designation,
                Salary = s.Salary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff list");
            return Enumerable.Empty<StaffDto>();
        }
    }

    /// <summary>
    /// Gets staff member by ID.
    /// </summary>
    public async Task<StaffDto?> GetStaffByIdAsync(int staffId, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.GetByIdAsync(staffId, cancellationToken);
            if (staff == null) return null;

            return new StaffDto
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                Phone = staff.Phone,
                Address = staff.Address,
                Gender = staff.Gender.ToString(),
                Designation = staff.Designation,
                Salary = staff.Salary
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff by ID: {StaffId}", staffId);
            return null;
        }
    }
}
