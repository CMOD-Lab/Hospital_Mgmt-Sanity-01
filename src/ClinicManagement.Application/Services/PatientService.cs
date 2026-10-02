using AutoMapper;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Patient service for patient-facing operations.</summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepo;
    private readonly IDoctorRepository _doctorRepo;
    private readonly IDepartmentRepository _deptRepo;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepo,
        IDoctorRepository doctorRepo,
        IDepartmentRepository deptRepo,
        IAppointmentRepository appointmentRepo,
        IMapper mapper,
        ILogger<PatientService> logger)
    {
        _patientRepo = patientRepo;
        _doctorRepo = doctorRepo;
        _deptRepo = deptRepo;
        _appointmentRepo = appointmentRepo;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PatientProfileData?> GetPatientProfileAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var patient = await _patientRepo.GetByIdAsync(patientId, ct);
            if (patient == null) return null;
            return _mapper.Map<PatientProfileData>(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient profile for {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BillHistoryItem>> GetBillHistoryAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetBillHistoryByPatientAsync(patientId, ct);
            return _mapper.Map<IEnumerable<BillHistoryItem>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill history for patient {PatientId}", patientId);
            return Enumerable.Empty<BillHistoryItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TreatmentHistoryItem>> GetTreatmentHistoryAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetHistoryByPatientAsync(patientId, ct);
            return _mapper.Map<IEnumerable<TreatmentHistoryItem>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving treatment history for patient {PatientId}", patientId);
            return Enumerable.Empty<TreatmentHistoryItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<CurrentAppointmentData?> GetCurrentAppointmentAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetCurrentByPatientAsync(patientId, ct);
            if (appointment == null) return null;

            var doctorName = appointment.Doctor?.Name ?? "Unknown";
            var timings = appointment.Date.HasValue ? appointment.Date.Value.ToString("hh:mm tt") : "N/A";
            return new CurrentAppointmentData(doctorName, timings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<NotificationData>> GetNotificationsAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var notifications = await _appointmentRepo.GetNotificationsByPatientAsync(patientId, ct);
            return notifications.Select(a => new NotificationData(
                a.Doctor?.Name ?? "Unknown",
                a.Date.HasValue ? a.Date.Value.ToString("hh:mm tt") : "N/A"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for patient {PatientId}", patientId);
            return Enumerable.Empty<NotificationData>();
        }
    }

    /// <inheritdoc/>
    public async Task<PendingFeedbackData?> GetPendingFeedbackAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetPendingFeedbackByPatientAsync(patientId, ct);
            if (appointment == null) return null;

            return new PendingFeedbackData(
                appointment.AppointId,
                appointment.Doctor?.Name ?? "Unknown",
                appointment.Date.HasValue ? appointment.Date.Value.ToString("hh:mm tt") : "N/A"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback for patient {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SubmitFeedbackAsync(int appointId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointId, ct);
            if (appointment == null) return false;

            appointment.FeedbackStatus = FeedbackStatus.Given;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            _logger.LogInformation("Feedback submitted for appointment {AppointId}", appointId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment {AppointId}", appointId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DeptInfo>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        try
        {
            var depts = await _deptRepo.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<DeptInfo>>(depts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            return Enumerable.Empty<DeptInfo>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DoctorListItem>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken ct = default)
    {
        try
        {
            var doctors = await _doctorRepo.GetByDepartmentAsync(deptName, ct);
            return _mapper.Map<IEnumerable<DoctorListItem>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department {DeptName}", deptName);
            return Enumerable.Empty<DoctorListItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<DoctorProfileData?> GetDoctorProfileAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var doctor = await _doctorRepo.GetByIdAsync(doctorId, ct);
            if (doctor == null) return null;
            return _mapper.Map<DoctorProfileData>(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor profile for {DoctorId}", doctorId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AppointmentSlot>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken ct = default)
    {
        try
        {
            var slots = await _appointmentRepo.GetFreeSlotsByDoctorAsync(doctorId, patientId, ct);
            return slots.Select((a, i) => new AppointmentSlot(
                a.AppointId,
                a.Date.HasValue ? a.Date.Value.ToString("hh:mm tt") : "N/A",
                a.Date ?? DateTime.Now
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentSlot>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> BookAppointmentAsync(int doctorId, int patientId, int slotId, CancellationToken ct = default)
    {
        try
        {
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                AppointmentStatus = AppointmentStatus.Pending,
                DoctorNotification = NotificationStatus.Unseen,
                PatientNotification = NotificationStatus.Unseen,
                FeedbackStatus = FeedbackStatus.Pending,
                Date = DateTime.Now
            };

            await _appointmentRepo.AddAsync(appointment, ct);
            _logger.LogInformation("Appointment booked for patient {PatientId} with doctor {DoctorId}", patientId, doctorId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient {PatientId}", patientId);
            return false;
        }
    }
}
