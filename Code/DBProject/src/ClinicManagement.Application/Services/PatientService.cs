using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for patient-related operations.
/// </summary>
public class PatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets patient information by ID.
    /// </summary>
    public async Task<PatientDto?> GetPatientInfoAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
            if (patient == null) return null;

            var age = DateTime.Today.Year - patient.BirthDate.Year;
            if (patient.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return new PatientDto
            {
                PatientId = patient.PatientId,
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
            _logger.LogError(ex, "Error getting patient info for ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets all patients, optionally filtered by search query.
    /// </summary>
    public async Task<IEnumerable<PatientListDto>> GetPatientsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Domain.Entities.Patient> patients;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                patients = await _patientRepository.GetAllAsync(cancellationToken);
            }
            else
            {
                patients = await _patientRepository.SearchAsync(searchQuery, cancellationToken);
            }

            return patients.Select(p => new PatientListDto
            {
                PatientId = p.PatientId,
                Name = p.Name,
                Phone = p.Phone
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patients list");
            return Enumerable.Empty<PatientListDto>();
        }
    }

    /// <summary>
    /// Gets bill history for a patient.
    /// </summary>
    public async Task<IEnumerable<BillHistoryDto>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken);
            return appointments
                .Where(a => a.BillAmount.HasValue)
                .Select(a => new BillHistoryDto
                {
                    AppointId = a.AppointId,
                    DoctorName = a.Doctor?.Name,
                    Date = a.Date,
                    BillAmount = a.BillAmount,
                    BillStatus = a.BillStatus
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<BillHistoryDto>();
        }
    }

    /// <summary>
    /// Gets treatment history for a patient.
    /// </summary>
    public async Task<IEnumerable<TreatmentHistoryDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken);
            return appointments
                .Where(a => a.AppointmentStatus == 3) // Completed
                .Select(a => new TreatmentHistoryDto
                {
                    AppointId = a.AppointId,
                    DoctorName = a.Doctor?.Name,
                    Date = a.Date,
                    Disease = a.Disease,
                    Progress = a.Progress,
                    Prescription = a.Prescription
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting treatment history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<TreatmentHistoryDto>();
        }
    }

    /// <summary>
    /// Gets current appointment for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new AppointmentDto
            {
                AppointId = appointment.AppointId,
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor?.Name,
                Date = appointment.Date,
                Timings = appointment.Date?.ToString("hh:mm tt"),
                AppointmentStatus = appointment.AppointmentStatus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current appointment for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets notification for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetNotificationAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetNotificationByPatientAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new AppointmentDto
            {
                AppointId = appointment.AppointId,
                DoctorName = appointment.Doctor?.Name,
                Timings = appointment.Date?.ToString("hh:mm tt"),
                AppointmentStatus = appointment.AppointmentStatus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets pending feedback for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetPendingFeedbackByPatientAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new AppointmentDto
            {
                AppointId = appointment.AppointId,
                DoctorName = appointment.Doctor?.Name,
                Timings = appointment.Date?.ToString("hh:mm tt"),
                FeedbackStatus = appointment.FeedbackStatus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending feedback for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Submits feedback for an appointment.
    /// </summary>
    public async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.FeedbackStatus = 1; // Given
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }
}
