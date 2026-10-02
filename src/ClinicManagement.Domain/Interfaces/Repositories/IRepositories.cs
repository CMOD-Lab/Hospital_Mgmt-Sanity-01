using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>Repository interface for LoginTable operations.</summary>
public interface ILoginRepository
{
    Task<LoginTable?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<LoginTable?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LoginTable> AddAsync(LoginTable login, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}

/// <summary>Repository interface for Patient operations.</summary>
public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Patient>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Patient>> SearchAsync(string query, CancellationToken ct = default);
    Task<Patient> AddAsync(Patient patient, CancellationToken ct = default);
    Task UpdateAsync(Patient patient, CancellationToken ct = default);
}

/// <summary>Repository interface for Doctor operations.</summary>
public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Doctor>> GetAllActiveAsync(CancellationToken ct = default);
    Task<IEnumerable<Doctor>> SearchAsync(string query, CancellationToken ct = default);
    Task<IEnumerable<Doctor>> GetByDepartmentAsync(string deptName, CancellationToken ct = default);
    Task<Doctor> AddAsync(Doctor doctor, CancellationToken ct = default);
    Task UpdateAsync(Doctor doctor, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task SoftDeleteAsync(int id, CancellationToken ct = default);
}

/// <summary>Repository interface for Department operations.</summary>
public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync(CancellationToken ct = default);
    Task<Department?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Department?> GetByNameAsync(string name, CancellationToken ct = default);
}

/// <summary>Repository interface for OtherStaff operations.</summary>
public interface IStaffRepository
{
    Task<OtherStaff?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<OtherStaff>> SearchAsync(string query, CancellationToken ct = default);
    Task<OtherStaff> AddAsync(OtherStaff staff, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

/// <summary>Repository interface for Appointment operations.</summary>
public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetPendingByDoctorAsync(int doctorId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetTodaysByDoctorAsync(int doctorId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetBillsByDoctorAsync(int doctorId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetHistoryByPatientAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetBillHistoryByPatientAsync(int patientId, CancellationToken ct = default);
    Task<Appointment?> GetCurrentByPatientAsync(int patientId, CancellationToken ct = default);
    Task<Appointment?> GetPendingFeedbackByPatientAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetNotificationsByPatientAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken ct = default);
    Task<Appointment> AddAsync(Appointment appointment, CancellationToken ct = default);
    Task UpdateAsync(Appointment appointment, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> GetTotalPatientsCountAsync(CancellationToken ct = default);
    Task<int> GetTotalDoctorsCountAsync(CancellationToken ct = default);
    Task<double> GetTotalIncomeAsync(CancellationToken ct = default);
}
