using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Bill entity.
/// </summary>
public interface IBillRepository
{
    Task<IEnumerable<Bill>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Bill?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default);
    Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalIncomeAsync(CancellationToken cancellationToken = default);
}
