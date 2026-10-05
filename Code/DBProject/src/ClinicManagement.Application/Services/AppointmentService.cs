using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for appointment-related operations.
/// </summary>
public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        IDepartmentRepository departmentRepository,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets available free slots for a doctor for a patient.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var slots = await _appointmentRepository.GetFreeSlotsByDoctorAsync(doctorId, patientId, cancellationToken);
            return slots.Select(s => new AppointmentDto
            {
                AppointId = s.AppointId,
                Date = s.Date,
                Timings = s.Date?.ToString("hh:mm tt"),
                AppointmentStatus = s.AppointmentStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Books an appointment for a patient with a doctor.
    /// </summary>
    public async Task<bool> BookAppointmentAsync(InsertAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var slot = await _appointmentRepository.GetByIdAsync(dto.FreeSlotAppointId, cancellationToken);
            if (slot == null) return false;

            slot.PatientId = dto.PatientId;
            slot.AppointmentStatus = 2; // Pending
            slot.DoctorNotification = 2; // Unseen
            slot.PatientNotification = 2; // Unseen
            slot.FeedbackStatus = 2; // Pending

            await _appointmentRepository.UpdateAsync(slot, cancellationToken);
            _logger.LogInformation("Appointment booked: Doctor {DoctorId}, Patient {PatientId}", dto.DoctorId, dto.PatientId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            return false;
        }
    }

    /// <summary>
    /// Gets all departments for appointment selection.
    /// </summary>
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
            _logger.LogError(ex, "Error getting departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }
}
