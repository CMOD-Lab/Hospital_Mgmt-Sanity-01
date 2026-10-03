using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for appointment-related operations.
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper, ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all appointments");
            throw;
        }
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            return appointment == null ? null : _mapper.Map<AppointmentDto>(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment with ID {AppointmentId}", id);
            throw;
        }
    }

    public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating appointment for patient {PatientId} with doctor {DoctorId}", createDto.PatientId, createDto.DoctorId);
            var appointment = _mapper.Map<Appointment>(createDto);
            var created = await _appointmentRepository.AddAsync(appointment, cancellationToken);
            return _mapper.Map<AppointmentDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            throw;
        }
    }

    public async Task UpdatePrescriptionAsync(int id, AppointmentUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating prescription for appointment {AppointmentId}", id);
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), id);

            appointment.Disease = updateDto.Disease;
            appointment.Progress = updateDto.Progress;
            appointment.Prescription = updateDto.Prescription;

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment {AppointmentId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting appointment {AppointmentId}", id);
            if (!await _appointmentRepository.ExistsAsync(id, cancellationToken))
                throw new NotFoundException(nameof(Appointment), id);

            await _appointmentRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment {AppointmentId}", id);
            throw;
        }
    }

    public async Task ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment {AppointmentId}", id);
            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), id);

            appointment.Status = AppointmentStatus.Approved;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment {AppointmentId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<IEnumerable<AppointmentDto>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    public async Task<IEnumerable<AppointmentDto>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    public async Task<IEnumerable<AppointmentDto>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    public async Task<AppointmentDto?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            return appointment == null ? null : _mapper.Map<AppointmentDto>(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<AppointmentDto?> GetPendingFeedbackByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
            return appointment == null ? null : _mapper.Map<AppointmentDto>(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback appointment for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<IEnumerable<TimeSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var slots = await _appointmentRepository.GetFreeSlotsAsync(doctorId, patientId, cancellationToken);
            return _mapper.Map<IEnumerable<TimeSlotDto>>(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots for doctor {DoctorId}", doctorId);
            throw;
        }
    }

    public async Task SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Submitting feedback for appointment {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), appointmentId);

            appointment.FeedbackGiven = true;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment {AppointmentId}", appointmentId);
            throw;
        }
    }
}
