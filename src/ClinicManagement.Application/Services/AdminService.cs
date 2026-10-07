using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for admin operations.</summary>
public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IStaffRepository staffRepository,
        IPatientRepository patientRepository,
        IDepartmentRepository departmentRepository,
        ILoginRepository loginRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _staffRepository = staffRepository;
        _patientRepository = patientRepository;
        _departmentRepository = departmentRepository;
        _loginRepository = loginRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>Gets admin home dashboard information.</summary>
    public async Task<AdminHomeDto> GetAdminHomeInformationAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving admin home information");

            var totalDoctors = await _doctorRepository.GetTotalCountAsync(cancellationToken);
            var totalPatients = await _loginRepository.GetTotalPatientCountAsync(cancellationToken);
            var totalIncome = await _loginRepository.GetTotalIncomeAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetTotalCountAsync(cancellationToken);

            var deptViews = departments.Select(d => new DepartmentViewDto
            {
                DeptName = d.DeptName,
                DoctorCount = d.Doctors.Count(doc => doc.Status == 1)
            });

            var recentAppointments = (await _appointmentRepository.GetByDoctorIdAsync(0, cancellationToken))
                .Take(10)
                .Select(a => new AppointmentViewDto
                {
                    AppointID = a.AppointID,
                    PatientName = a.Patient?.Name ?? "Unknown",
                    DoctorName = a.Doctor?.Name ?? "Unknown",
                    Date = a.Date?.ToString("yyyy-MM-dd HH:mm"),
                    Status = a.AppointmentStatus.ToString()
                });

            return new AdminHomeDto
            {
                TotalDoctors = totalDoctors,
                TotalPatients = totalPatients,
                TotalIncome = totalIncome,
                Departments = deptViews,
                Appointments = recentAppointments
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin home information");
            return new AdminHomeDto();
        }
    }

    /// <summary>Adds a new doctor to the system.</summary>
    public async Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new doctor with email: {Email}", dto.Email);

            // Create login entry
            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 2 // Doctor
            };

            var createdLogin = await _loginRepository.AddAsync(login, cancellationToken);

            if (!DateTime.TryParse(dto.BirthDate, out var birthDate))
            {
                _logger.LogWarning("Invalid birth date format for doctor: {Email}", dto.Email);
                return false;
            }

            var doctor = new Doctor
            {
                DoctorID = createdLogin.LoginID,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = birthDate,
                Gender = dto.Gender,
                DeptNo = dto.DeptNo,
                ChargesPerVisit = dto.ChargesPerVisit,
                MonthlySalary = dto.Salary,
                Qualification = dto.Qualification,
                Specialization = dto.Specialization,
                WorkExperience = dto.Experience,
                Status = 1 // Present
            };

            await _doctorRepository.AddAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor added successfully with ID: {DoctorId}", createdLogin.LoginID);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor with email: {Email}", dto.Email);
            return false;
        }
    }

    /// <summary>Adds a new staff member to the system.</summary>
    public async Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new staff member: {Name}", dto.Name);

            DateTime? birthDate = null;
            if (DateTime.TryParse(dto.BirthDate, out var parsedDate))
            {
                birthDate = parsedDate;
            }

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
            _logger.LogInformation("Staff member added successfully: {Name}", dto.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>Soft-deletes a doctor (sets status to 0).</summary>
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Soft-deleting doctor with ID: {DoctorId}", id);
            await _doctorRepository.SoftDeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor with ID: {DoctorId}", id);
            return false;
        }
    }

    /// <summary>Deletes a staff member.</summary>
    public async Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff member with ID: {StaffId}", id);
            await _staffRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member with ID: {StaffId}", id);
            return false;
        }
    }

    /// <summary>Gets list of doctors with optional search.</summary>
    public async Task<IEnumerable<DoctorListItemDto>> GetDoctorsAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Doctor> doctors;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            }
            else
            {
                doctors = await _doctorRepository.SearchAsync(searchQuery, cancellationToken);
            }

            return doctors.Select(d => new DoctorListItemDto
            {
                DoctorID = d.DoctorID,
                Name = d.Name,
                Department = d.Department?.DeptName ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors with search: {SearchQuery}", searchQuery);
            return Enumerable.Empty<DoctorListItemDto>();
        }
    }

    /// <summary>Gets list of patients with optional search.</summary>
    public async Task<IEnumerable<PatientListItemDto>> GetPatientsAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Patient> patients;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                patients = await _patientRepository.GetAllAsync(cancellationToken);
            }
            else
            {
                patients = await _patientRepository.SearchAsync(searchQuery, cancellationToken);
            }

            return patients.Select(p => new PatientListItemDto
            {
                PatientID = p.PatientID,
                Name = p.Name,
                Phone = p.Phone
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients with search: {SearchQuery}", searchQuery);
            return Enumerable.Empty<PatientListItemDto>();
        }
    }

    /// <summary>Gets list of staff with optional search.</summary>
    public async Task<IEnumerable<StaffListItemDto>> GetStaffAsync(string searchQuery, CancellationToken cancellationToken = default)
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

            return staff.Select(s => new StaffListItemDto
            {
                StaffID = s.StaffID,
                Name = s.Name,
                Designation = s.Designation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with search: {SearchQuery}", searchQuery);
            return Enumerable.Empty<StaffListItemDto>();
        }
    }

    /// <summary>Checks if a doctor email already exists.</summary>
    public async Task<bool> DoctorEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _loginRepository.EmailExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking doctor email: {Email}", email);
            return false;
        }
    }

    /// <summary>Gets all departments.</summary>
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return departments.Select(d => new DepartmentDto
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName,
                Description = d.Description
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }
}
