using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for doctor-related operations.
/// </summary>
public class DoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository,
        IDepartmentRepository departmentRepository,
        ILoginRepository loginRepository,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _departmentRepository = departmentRepository;
        _loginRepository = loginRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets doctor profile by ID.
    /// </summary>
    public async Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor == null) return null;

            var age = DateTime.Today.Year - doctor.BirthDate.Year;
            if (doctor.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return new DoctorProfileDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender.ToString(),
                ChargesPerVisit = doctor.ChargesPerVisit,
                ReputeIndex = doctor.ReputeIndex ?? 0,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience ?? 0,
                Age = age,
                DepartmentName = doctor.Department?.DeptName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor profile for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>
    /// Gets all active doctors, optionally filtered by search query.
    /// </summary>
    public async Task<IEnumerable<DoctorListDto>> GetDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
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

            return doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                DepartmentName = d.Department?.DeptName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors list");
            return Enumerable.Empty<DoctorListDto>();
        }
    }

    /// <summary>
    /// Gets doctors by department name.
    /// </summary>
    public async Task<IEnumerable<DoctorListDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.GetByDepartmentAsync(deptName, cancellationToken);
            return doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                DepartmentName = d.Department?.DeptName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors by department: {DeptName}", deptName);
            return Enumerable.Empty<DoctorListDto>();
        }
    }

    /// <summary>
    /// Adds a new doctor.
    /// </summary>
    public async Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailExists = await _doctorRepository.EmailExistsAsync(dto.Email, cancellationToken);
            if (emailExists) return false;

            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 2 // Doctor
            };
            var loginId = await _loginRepository.AddAsync(login, cancellationToken);

            if (!DateTime.TryParse(dto.BirthDate, out var birthDate))
                birthDate = DateTime.Today;

            var doctor = new Doctor
            {
                DoctorId = loginId,
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
                WorkExperience = dto.WorkExperience,
                Status = 1
            };

            await _doctorRepository.AddAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor added successfully with ID: {DoctorId}", loginId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor: {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>
    /// Soft-deletes a doctor (sets status to 0).
    /// </summary>
    public async Task<bool> DeleteDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _doctorRepository.SoftDeleteAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor ID: {DoctorId}", doctorId);
            return false;
        }
    }

    /// <summary>
    /// Gets pending appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                PatientId = a.PatientId,
                PatientName = a.Patient?.Name,
                Date = a.Date,
                Timings = a.Date?.ToString("hh:mm tt"),
                AppointmentStatus = a.AppointmentStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Gets today's appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                PatientId = a.PatientId,
                PatientName = a.Patient?.Name,
                Date = a.Date,
                Timings = a.Date?.ToString("hh:mm tt"),
                AppointmentStatus = a.AppointmentStatus,
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Approves an appointment.
    /// </summary>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _appointmentRepository.ApproveAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Deletes (rejects) an appointment.
    /// </summary>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Updates prescription for an appointment.
    /// </summary>
    public async Task<bool> UpdatePrescriptionAsync(UpdatePrescriptionDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.Disease = dto.Disease;
            appointment.Progress = dto.Progress;
            appointment.Prescription = dto.Prescription;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment ID: {AppointmentId}", dto.AppointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets patient history for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return appointments
                .Where(a => a.AppointmentStatus == 3)
                .Select(a => new AppointmentDto
                {
                    AppointId = a.AppointId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient?.Name,
                    Date = a.Date,
                    Disease = a.Disease,
                    Progress = a.Progress,
                    Prescription = a.Prescription,
                    BillAmount = a.BillAmount,
                    BillStatus = a.BillStatus
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient history for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Marks appointment as paid and completed.
    /// </summary>
    public async Task<bool> MarkAppointmentPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.BillStatus = "Paid";
            appointment.AppointmentStatus = 3; // Completed
            appointment.FeedbackStatus = 2; // Pending
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment paid: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Marks appointment as unpaid and completed.
    /// </summary>
    public async Task<bool> MarkAppointmentUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.BillStatus = "Unpaid";
            appointment.AppointmentStatus = 3; // Completed
            appointment.FeedbackStatus = 2; // Pending
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment unpaid: {AppointmentId}", appointmentId);
            return false;
        }
    }
}
