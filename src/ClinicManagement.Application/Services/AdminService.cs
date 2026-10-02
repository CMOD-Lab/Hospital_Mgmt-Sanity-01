using AutoMapper;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Admin service for managing doctors, staff, and dashboard data.</summary>
public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly ILoginRepository _loginRepo;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IDepartmentRepository _deptRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepo,
        IStaffRepository staffRepo,
        ILoginRepository loginRepo,
        IAppointmentRepository appointmentRepo,
        IDepartmentRepository deptRepo,
        IMapper mapper,
        ILogger<AdminService> logger)
    {
        _doctorRepo = doctorRepo;
        _staffRepo = staffRepo;
        _loginRepo = loginRepo;
        _appointmentRepo = appointmentRepo;
        _deptRepo = deptRepo;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<AdminDashboardData> GetDashboardDataAsync(CancellationToken ct = default)
    {
        try
        {
            var totalDoctors = await _appointmentRepo.GetTotalDoctorsCountAsync(ct);
            var totalPatients = await _appointmentRepo.GetTotalPatientsCountAsync(ct);
            var totalIncome = await _appointmentRepo.GetTotalIncomeAsync(ct);
            var departments = await _deptRepo.GetAllAsync(ct);
            var appointments = await _appointmentRepo.GetPendingByDoctorAsync(0, ct);

            var deptSummaries = _mapper.Map<IEnumerable<DepartmentSummary>>(departments);
            var apptSummaries = _mapper.Map<IEnumerable<AppointmentSummary>>(appointments);

            return new AdminDashboardData(totalDoctors, totalPatients, totalIncome, deptSummaries, apptSummaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin dashboard data");
            return new AdminDashboardData(0, 0, 0, Enumerable.Empty<DepartmentSummary>(), Enumerable.Empty<AppointmentSummary>());
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RegisterDoctorAsync(RegisterDoctorRequest request, CancellationToken ct = default)
    {
        try
        {
            if (await _loginRepo.EmailExistsAsync(request.Email, ct))
                return false;

            var login = new LoginTable
            {
                Email = request.Email,
                Password = request.Password,
                Type = UserType.Doctor
            };
            var createdLogin = await _loginRepo.AddAsync(login, ct);

            var doctor = new Doctor
            {
                DoctorId = createdLogin.LoginId,
                Name = request.Name,
                BirthDate = DateTime.Parse(request.BirthDate),
                DeptNo = request.DeptNo,
                Phone = request.Phone,
                Gender = request.Gender,
                Address = request.Address,
                WorkExperience = request.Experience,
                MonthlySalary = request.Salary,
                ChargesPerVisit = request.ChargesPerVisit,
                Specialization = request.Specialization,
                Qualification = request.Qualification,
                Status = DoctorStatus.Present,
                PatientsTreated = 0,
                ReputeIndex = 0
            };

            await _doctorRepo.AddAsync(doctor, ct);
            _logger.LogInformation("Doctor {Name} registered with ID {Id}", request.Name, createdLogin.LoginId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor {Name}", request.Name);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AddStaffAsync(AddStaffRequest request, CancellationToken ct = default)
    {
        try
        {
            var staff = new OtherStaff
            {
                Name = request.Name,
                BirthDate = DateTime.TryParse(request.BirthDate, out var bd) ? bd : null,
                Phone = request.Phone,
                Gender = request.Gender,
                Address = request.Address,
                Salary = request.Salary,
                HighestQualification = request.Qualification,
                Designation = request.Designation
            };

            await _staffRepo.AddAsync(staff, ct);
            _logger.LogInformation("Staff {Name} added", request.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff {Name}", request.Name);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await _doctorRepo.SoftDeleteAsync(id, ct);
            _logger.LogInformation("Doctor {Id} soft-deleted", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor {Id}", id);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteStaffAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await _staffRepo.DeleteAsync(id, ct);
            _logger.LogInformation("Staff {Id} deleted", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff {Id}", id);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DoctorEmailExistsAsync(string email, CancellationToken ct = default)
    {
        try
        {
            return await _loginRepo.EmailExistsAsync(email, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking doctor email {Email}", email);
            return false;
        }
    }
}
