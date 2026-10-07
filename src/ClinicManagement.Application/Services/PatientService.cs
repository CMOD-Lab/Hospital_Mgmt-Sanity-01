using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for patient operations.</summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        IDepartmentRepository departmentRepository,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    /// <summary>Gets patient information by ID.</summary>
    public async Task<PatientInfoDto?> GetPatientInfoAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient info for ID: {PatientId}", patientId);
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
            if (patient == null) return null;

            var age = DateTime.Today.Year - patient.BirthDate.Year;
            if (patient.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return new PatientInfoDto
            {
                PatientID = patient.PatientID,
                Name = patient.Name,
                Phone = patient.Phone,
                Address = patient.Address,
                BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
                Age = age,
                Gender = patient.Gender.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient info for ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Gets bill history for a patient.</summary>
    public async Task<IEnumerable<BillHistoryDto>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bill history for patient ID: {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetBillHistoryByPatientIdAsync(patientId, cancellationToken);

            return appointments.Select(a => new BillHistoryDto
            {
                AppointID = a.AppointID,
                DoctorName = a.Doctor?.Name,
                Date = a.Date?.ToString("yyyy-MM-dd HH:mm"),
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<BillHistoryDto>();
        }
    }

    /// <summary>Gets current appointment for a patient.</summary>
    public async Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving current appointment for patient ID: {PatientId}", patientId);
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new CurrentAppointmentDto
            {
                DoctorName = appointment.Doctor?.Name ?? "Unknown",
                Timings = appointment.Date?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Gets treatment history for a patient.</summary>
    public async Task<IEnumerable<TreatmentHistoryDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving treatment history for patient ID: {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetTreatmentHistoryByPatientIdAsync(patientId, cancellationToken);

            return appointments.Select(a => new TreatmentHistoryDto
            {
                AppointID = a.AppointID,
                DoctorName = a.Doctor?.Name,
                Date = a.Date?.ToString("yyyy-MM-dd HH:mm"),
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving treatment history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<TreatmentHistoryDto>();
        }
    }

    /// <summary>Gets department information.</summary>
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentInfoAsync(CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "Error retrieving department info");
            return Enumerable.Empty<DepartmentDto>();
        }
    }

    /// <summary>Gets doctors by department name.</summary>
    public async Task<IEnumerable<DoctorListItemDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.GetByDepartmentAsync(deptName, cancellationToken);
            return doctors.Select(d => new DoctorListItemDto
            {
                DoctorID = d.DoctorID,
                Name = d.Name,
                Department = d.Department?.DeptName ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department: {DeptName}", deptName);
            return Enumerable.Empty<DoctorListItemDto>();
        }
    }

    /// <summary>Gets doctor profile by ID.</summary>
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
                DoctorID = doctor.DoctorID,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender.ToString(),
                ChargesPerVisit = doctor.ChargesPerVisit,
                ReputeIndex = doctor.ReputeIndex ?? 0,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience ?? 0,
                Age = age
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor profile for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>Gets free appointment slots for a doctor.</summary>
    public async Task<IEnumerable<AppointmentSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var slots = await _appointmentRepository.GetFreeSlotsByDoctorAndPatientAsync(doctorId, patientId, cancellationToken);
            return slots.Select((a, index) => new AppointmentSlotDto
            {
                SlotId = a.AppointID,
                Timings = a.Date?.ToString("yyyy-MM-dd HH:mm") ?? $"Slot {index + 1}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentSlotDto>();
        }
    }

    /// <summary>Books an appointment for a patient.</summary>
    public async Task<bool> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Booking appointment for patient: {PatientId} with doctor: {DoctorId}", patientId, doctorId);

            var appointment = new Domain.Entities.Appointment
            {
                DoctorID = doctorId,
                PatientID = patientId,
                Date = DateTime.Now,
                AppointmentStatus = 2, // Pending
                DoctorNotification = 2, // Unseen
                PatientNotification = 2, // Unseen
                FeedbackStatus = 2 // Pending
            };

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient: {PatientId}", patientId);
            return false;
        }
    }

    /// <summary>Gets notifications for a patient.</summary>
    public async Task<NotificationDto?> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new NotificationDto
            {
                DoctorName = appointment.Doctor?.Name ?? "Unknown",
                Timings = appointment.Date?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty,
                Count = 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Gets pending feedback for a patient.</summary>
    public async Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new PendingFeedbackDto
            {
                AppointmentId = appointment.AppointID,
                DoctorName = appointment.Doctor?.Name ?? "Unknown",
                Timings = appointment.Date?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Submits feedback for an appointment.</summary>
    public async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Submitting feedback for appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.FeedbackStatus = 1; // Given
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }
}
