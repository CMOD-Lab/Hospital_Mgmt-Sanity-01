using AutoMapper;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Doctor service for doctor-facing operations.</summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepo;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepo,
        IAppointmentRepository appointmentRepo,
        IMapper mapper,
        ILogger<DoctorService> logger)
    {
        _doctorRepo = doctorRepo;
        _appointmentRepo = appointmentRepo;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<DoctorDashboardData?> GetDoctorDashboardAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var doctor = await _doctorRepo.GetByIdAsync(doctorId, ct);
            if (doctor == null) return null;
            return _mapper.Map<DoctorDashboardData>(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor dashboard for {DoctorId}", doctorId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PendingAppointmentItem>> GetPendingAppointmentsAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetPendingByDoctorAsync(doctorId, ct);
            return _mapper.Map<IEnumerable<PendingAppointmentItem>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<PendingAppointmentItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ApproveAppointmentAsync(int appointId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointId, ct);
            if (appointment == null) return false;

            appointment.AppointmentStatus = AppointmentStatus.Approved;
            appointment.PatientNotification = NotificationStatus.Unseen;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            _logger.LogInformation("Appointment {AppointId} approved", appointId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment {AppointId}", appointId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RejectAppointmentAsync(int appointId, CancellationToken ct = default)
    {
        try
        {
            await _appointmentRepo.DeleteAsync(appointId, ct);
            _logger.LogInformation("Appointment {AppointId} rejected/deleted", appointId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment {AppointId}", appointId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodayAppointmentItem>> GetTodayAppointmentsAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetTodaysByDoctorAsync(doctorId, ct);
            return _mapper.Map<IEnumerable<TodayAppointmentItem>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<TodayAppointmentItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointId, string disease, string progress, string prescription, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointId, ct);
            if (appointment == null || appointment.DoctorId != doctorId) return false;

            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            _logger.LogInformation("Prescription updated for appointment {AppointId}", appointId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment {AppointId}", appointId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BillItem>> GetBillsAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetBillsByDoctorAsync(doctorId, ct);
            return _mapper.Map<IEnumerable<BillItem>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<BillItem>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointId, ct);
            if (appointment == null) return false;

            appointment.BillStatus = BillStatus.Paid;
            appointment.AppointmentStatus = AppointmentStatus.Completed;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            _logger.LogInformation("Bill marked paid for appointment {AppointId}", appointId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill paid for appointment {AppointId}", appointId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointId, ct);
            if (appointment == null) return false;

            appointment.BillStatus = BillStatus.Unpaid;
            appointment.AppointmentStatus = AppointmentStatus.Completed;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            _logger.LogInformation("Bill marked unpaid for appointment {AppointId}", appointId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill unpaid for appointment {AppointId}", appointId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TreatmentHistoryItem>> GetPatientHistoryAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetHistoryByPatientAsync(doctorId, ct);
            return _mapper.Map<IEnumerable<TreatmentHistoryItem>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient history for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<TreatmentHistoryItem>();
        }
    }
}
