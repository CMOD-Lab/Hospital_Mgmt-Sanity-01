using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service implementation for doctor operations.</summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>Gets doctor information by ID.</summary>
    public async Task<DoctorInfoDto?> GetDoctorInfoAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor info for ID: {DoctorId}", doctorId);
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor == null) return null;

            return new DoctorInfoDto
            {
                DoctorID = doctor.DoctorID,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Address = doctor.Address,
                BirthDate = doctor.BirthDate.ToString("yyyy-MM-dd"),
                Gender = doctor.Gender.ToString(),
                Department = doctor.Department?.DeptName ?? string.Empty,
                ChargesPerVisit = doctor.ChargesPerVisit,
                MonthlySalary = doctor.MonthlySalary,
                ReputeIndex = doctor.ReputeIndex,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor info for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>Gets pending appointments for a doctor.</summary>
    public async Task<IEnumerable<PendingAppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pending appointments for doctor ID: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);

            return appointments.Select(a => new PendingAppointmentDto
            {
                AppointID = a.AppointID,
                PatientName = a.Patient?.Name ?? "Unknown",
                Date = a.Date?.ToString("yyyy-MM-dd HH:mm"),
                Status = a.AppointmentStatus == 2 ? "Pending" : "Unknown"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<PendingAppointmentDto>();
        }
    }

    /// <summary>Approves an appointment.</summary>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment ID: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.AppointmentStatus = 1; // Approved
            appointment.PatientNotification = 2; // Unseen
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>Deletes an appointment.</summary>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting appointment ID: {AppointmentId}", appointmentId);
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>Gets today's appointments for a doctor.</summary>
    public async Task<IEnumerable<TodayAppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving today's appointments for doctor ID: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);

            return appointments.Select(a => new TodayAppointmentDto
            {
                AppointID = a.AppointID,
                PatientName = a.Patient?.Name ?? "Unknown",
                Date = a.Date?.ToString("yyyy-MM-dd HH:mm"),
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<TodayAppointmentDto>();
        }
    }

    /// <summary>Updates prescription for an appointment.</summary>
    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating prescription for appointment ID: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorID != doctorId) return false;

            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>Gets billable appointments for a doctor.</summary>
    public async Task<IEnumerable<BillableAppointmentDto>> GetBillableAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving billable appointments for doctor ID: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetBillableByDoctorIdAsync(doctorId, cancellationToken);

            return appointments.Select(a => new BillableAppointmentDto
            {
                AppointID = a.AppointID,
                PatientName = a.Patient?.Name ?? "Unknown",
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus,
                Date = a.Date?.ToString("yyyy-MM-dd HH:mm")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving billable appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<BillableAppointmentDto>();
        }
    }

    /// <summary>Marks a bill as paid and completes the appointment.</summary>
    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as paid for appointment ID: {AppointmentId}", appointmentId);
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
            _logger.LogError(ex, "Error marking bill as paid for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>Marks a bill as unpaid and completes the appointment.</summary>
    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as unpaid for appointment ID: {AppointmentId}", appointmentId);
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
            _logger.LogError(ex, "Error marking bill as unpaid for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>Gets patient history for a doctor.</summary>
    public async Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient history for doctor ID: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetHistoryByDoctorIdAsync(doctorId, cancellationToken);

            return appointments.Select(a => new PatientHistoryDto
            {
                AppointID = a.AppointID,
                PatientName = a.Patient?.Name ?? "Unknown",
                Date = a.Date?.ToString("yyyy-MM-dd HH:mm"),
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription,
                BillStatus = a.BillStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient history for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<PatientHistoryDto>();
        }
    }
}
