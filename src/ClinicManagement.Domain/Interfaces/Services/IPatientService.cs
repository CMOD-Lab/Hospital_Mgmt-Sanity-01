using ClinicManagement.Domain.DTOs;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>Service interface for patient operations.</summary>
public interface IPatientService
{
    Task<PatientInfoDto?> GetPatientInfoAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BillHistoryDto>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TreatmentHistoryDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DepartmentDto>> GetDepartmentInfoAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorListItemDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default);
    Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task<bool> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default);
    Task<NotificationDto?> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default);
    Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default);
    Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
}
