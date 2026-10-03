using ClinicManagement.Application.DTOs;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service interface for appointment operations.
/// </summary>
public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppointmentDto> CreateAsync(AppointmentCreateDto createDto, CancellationToken cancellationToken = default);
    Task UpdatePrescriptionAsync(int id, AppointmentUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task ApproveAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<AppointmentDto?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<AppointmentDto?> GetPendingFeedbackByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
}
