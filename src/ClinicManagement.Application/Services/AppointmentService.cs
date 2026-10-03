using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for appointment-related operations.
/// </summary>
public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IAppointmentRepository appointmentRepository, ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all appointments.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all appointments");
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all appointments");
            throw;
        }
    }

    /// <summary>
    /// Gets an appointment by ID.
    /// </summary>
    public async Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving appointment with ID {AppointmentId}", id);
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            return appointment == null ? null : MapToDto(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment with ID {AppointmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new appointment.
    /// </summary>
    public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating appointment for patient {PatientId} with doctor {DoctorId}", dto.PatientId, dto.DoctorId);

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                TimeSlotId = dto.TimeSlotId,
                AppointmentDate = DateTime.UtcNow,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            var created = await _appointmentRepository.AddAsync(appointment, cancellationToken);
            _logger.LogInformation("Appointment created with ID {AppointmentId}", created.AppointmentId);
            return MapToDto(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment for patient {PatientId}", dto.PatientId);
            throw;
        }
    }

    /// <summary>
    /// Deletes an appointment.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting appointment with ID {AppointmentId}", id);
            if (!await _appointmentRepository.ExistsAsync(id, cancellationToken))
            {
                throw new EntityNotFoundException("Appointment", id);
            }
            await _appointmentRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Appointment with ID {AppointmentId} deleted successfully", id);
        }
        catch (Exception ex) when (ex is not EntityNotFoundException)
        {
            _logger.LogError(ex, "Error deleting appointment with ID {AppointmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Gets appointments for a specific patient.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving appointments for patient {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments for patient {PatientId}", patientId);
            throw;
        }
    }

    /// <summary>
    /// Gets appointments for a specific doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving appointments for doctor {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    /// <summary>
    /// Gets pending appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pending appointments for doctor {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    /// <summary>
    /// Gets today's appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving today's appointments for doctor {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    /// <summary>
    /// Gets the current appointment for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving current appointment for patient {PatientId}", patientId);
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            return appointment == null ? null : MapToDto(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient {PatientId}", patientId);
            throw;
        }
    }

    /// <summary>
    /// Gets pending feedback appointment for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetPendingFeedbackByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pending feedback appointment for patient {PatientId}", patientId);
            var appointment = await _appointmentRepository.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
            return appointment == null ? null : MapToDto(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback appointment for patient {PatientId}", patientId);
            throw;
        }
    }

    /// <summary>
    /// Gets free time slots for a doctor.
    /// </summary>
    public async Task<IEnumerable<TimeSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving free slots for doctor {DoctorId}", doctorId);
            var slots = await _appointmentRepository.GetFreeSlotsAsync(doctorId, patientId, cancellationToken);
            return slots.Select(s => new TimeSlotDto
            {
                TimeSlotId = s.TimeSlotId,
                DoctorId = s.DoctorId,
                Timings = s.Timings,
                IsAvailable = s.IsAvailable
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    /// <summary>
    /// Approves an appointment.
    /// </summary>
    public async Task ApproveAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment {AppointmentId}", appointmentId);
            await _appointmentRepository.ApproveAsync(appointmentId, cancellationToken);
            _logger.LogInformation("Appointment {AppointmentId} approved", appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    /// <summary>
    /// Updates prescription for an appointment.
    /// </summary>
    public async Task UpdatePrescriptionAsync(PrescriptionUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating prescription for appointment {AppointmentId}", dto.AppointmentId);
            await _appointmentRepository.UpdatePrescriptionAsync(
                dto.DoctorId, dto.AppointmentId, dto.Disease, dto.Progress, dto.Prescription, cancellationToken);
            _logger.LogInformation("Prescription updated for appointment {AppointmentId}", dto.AppointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment {AppointmentId}", dto.AppointmentId);
            throw;
        }
    }

    /// <summary>
    /// Marks an appointment as paid.
    /// </summary>
    public async Task MarkPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking appointment {AppointmentId} as paid", appointmentId);
            await _appointmentRepository.MarkPaidAsync(doctorId, appointmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment {AppointmentId} as paid", appointmentId);
            throw;
        }
    }

    /// <summary>
    /// Marks an appointment as unpaid.
    /// </summary>
    public async Task MarkUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking appointment {AppointmentId} as unpaid", appointmentId);
            await _appointmentRepository.MarkUnpaidAsync(doctorId, appointmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment {AppointmentId} as unpaid", appointmentId);
            throw;
        }
    }

    /// <summary>
    /// Stores feedback for an appointment.
    /// </summary>
    public async Task StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Storing feedback for appointment {AppointmentId}", appointmentId);
            await _appointmentRepository.StoreFeedbackAsync(appointmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing feedback for appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    private static AppointmentDto MapToDto(Appointment a) => new()
    {
        AppointmentId = a.AppointmentId,
        PatientId = a.PatientId,
        PatientName = a.Patient?.Name ?? string.Empty,
        DoctorId = a.DoctorId,
        DoctorName = a.Doctor?.Name ?? string.Empty,
        TimeSlotId = a.TimeSlotId,
        Timings = a.TimeSlot?.Timings ?? string.Empty,
        AppointmentDate = a.AppointmentDate,
        Status = a.Status,
        Disease = a.Disease,
        Progress = a.Progress,
        Prescription = a.Prescription,
        FeedbackGiven = a.FeedbackGiven,
        IsPaid = a.IsPaid
    };
}
