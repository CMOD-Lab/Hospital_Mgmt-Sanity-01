using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Appointment entity.
/// </summary>
public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetPendingFeedbackByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSlot>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task ApproveAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default);
    Task MarkPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task MarkUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
}
