using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Interfaces.Services;

// ─── Auth ────────────────────────────────────────────────────────────────────

public record LoginResult(bool Success, int UserId, UserType UserType, string Message);
public record SignupResult(bool Success, int UserId, string Message);

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<SignupResult> SignupPatientAsync(string name, string birthDate, string email, string password, string phone, string gender, string address, CancellationToken ct = default);
}

// ─── Admin ───────────────────────────────────────────────────────────────────

public record AdminDashboardData(int TotalDoctors, int TotalPatients, double TotalIncome, IEnumerable<DepartmentSummary> Departments, IEnumerable<AppointmentSummary> Appointments);
public record DepartmentSummary(int DeptNo, string DeptName, string? Description, int DoctorCount);
public record AppointmentSummary(int AppointId, string? PatientName, string? DoctorName, DateTime? Date, string Status);

public interface IAdminService
{
    Task<AdminDashboardData> GetDashboardDataAsync(CancellationToken ct = default);
    Task<bool> RegisterDoctorAsync(RegisterDoctorRequest request, CancellationToken ct = default);
    Task<bool> AddStaffAsync(AddStaffRequest request, CancellationToken ct = default);
    Task<bool> DeleteDoctorAsync(int id, CancellationToken ct = default);
    Task<bool> DeleteStaffAsync(int id, CancellationToken ct = default);
    Task<bool> DoctorEmailExistsAsync(string email, CancellationToken ct = default);
}

public record RegisterDoctorRequest(string Name, string Email, string Password, string BirthDate, int DeptNo, string Phone, char Gender, string Address, int Experience, int Salary, int ChargesPerVisit, string Specialization, string Qualification);
public record AddStaffRequest(string Name, string BirthDate, string Phone, char Gender, string Address, int Salary, string Qualification, string Designation);

// ─── Patient ─────────────────────────────────────────────────────────────────

public record PatientProfileData(int PatientId, string Name, string? Phone, string? Address, string BirthDate, int Age, string Gender);
public record DoctorProfileData(int DoctorId, string Name, string? Phone, string Gender, double ChargesPerVisit, double ReputeIndex, int PatientsTreated, string Qualification, string? Specialization, int WorkExperience, int Age, string? DeptName);
public record BillHistoryItem(int AppointId, DateTime? Date, string? DoctorName, double? BillAmount, string? BillStatus, string? Disease);
public record TreatmentHistoryItem(int AppointId, DateTime? Date, string? DoctorName, string? Disease, string? Progress, string? Prescription);
public record CurrentAppointmentData(string DoctorName, string Timings);
public record NotificationData(string DoctorName, string Timings);
public record PendingFeedbackData(int AppointId, string DoctorName, string Timings);
public record DeptInfo(int DeptNo, string DeptName, string? Description);
public record DoctorListItem(int DoctorId, string Name, string? DeptName);

public interface IPatientService
{
    Task<PatientProfileData?> GetPatientProfileAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<BillHistoryItem>> GetBillHistoryAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<TreatmentHistoryItem>> GetTreatmentHistoryAsync(int patientId, CancellationToken ct = default);
    Task<CurrentAppointmentData?> GetCurrentAppointmentAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<NotificationData>> GetNotificationsAsync(int patientId, CancellationToken ct = default);
    Task<PendingFeedbackData?> GetPendingFeedbackAsync(int patientId, CancellationToken ct = default);
    Task<bool> SubmitFeedbackAsync(int appointId, CancellationToken ct = default);
    Task<IEnumerable<DeptInfo>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<IEnumerable<DoctorListItem>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken ct = default);
    Task<DoctorProfileData?> GetDoctorProfileAsync(int doctorId, CancellationToken ct = default);
    Task<IEnumerable<AppointmentSlot>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken ct = default);
    Task<bool> BookAppointmentAsync(int doctorId, int patientId, int slotId, CancellationToken ct = default);
}

public record AppointmentSlot(int SlotId, string Timings, DateTime Date);

// ─── Doctor ──────────────────────────────────────────────────────────────────

public record DoctorDashboardData(int DoctorId, string Name, string? Phone, string? Address, DateTime BirthDate, char Gender, string? DeptName, double ChargesPerVisit, double? MonthlySalary, double? ReputeIndex, int PatientsTreated, string Qualification, string? Specialization, int? WorkExperience);
public record PendingAppointmentItem(int AppointId, string? PatientName, DateTime? Date, string Status);
public record TodayAppointmentItem(int AppointId, string? PatientName, DateTime? Date, string? Disease, string? Progress, string? Prescription);
public record BillItem(int AppointId, string? PatientName, DateTime? Date, double? BillAmount, string? BillStatus);

public interface IDoctorService
{
    Task<DoctorDashboardData?> GetDoctorDashboardAsync(int doctorId, CancellationToken ct = default);
    Task<IEnumerable<PendingAppointmentItem>> GetPendingAppointmentsAsync(int doctorId, CancellationToken ct = default);
    Task<bool> ApproveAppointmentAsync(int appointId, CancellationToken ct = default);
    Task<bool> RejectAppointmentAsync(int appointId, CancellationToken ct = default);
    Task<IEnumerable<TodayAppointmentItem>> GetTodayAppointmentsAsync(int doctorId, CancellationToken ct = default);
    Task<bool> UpdatePrescriptionAsync(int doctorId, int appointId, string disease, string progress, string prescription, CancellationToken ct = default);
    Task<IEnumerable<BillItem>> GetBillsAsync(int doctorId, CancellationToken ct = default);
    Task<bool> MarkBillPaidAsync(int doctorId, int appointId, CancellationToken ct = default);
    Task<bool> MarkBillUnpaidAsync(int doctorId, int appointId, CancellationToken ct = default);
    Task<IEnumerable<TreatmentHistoryItem>> GetPatientHistoryAsync(int doctorId, CancellationToken ct = default);
}
