namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for bill data.
/// </summary>
public class BillDto
{
    public int BillId { get; set; }
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BillDate { get; set; }
    public string? Disease { get; set; }
    public string? Timings { get; set; }
}

/// <summary>
/// DTO for department data.
/// </summary>
public class DepartmentDto
{
    public int DepartmentId { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
}

/// <summary>
/// DTO for admin dashboard summary.
/// </summary>
public class AdminDashboardDto
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public decimal TotalIncome { get; set; }
    public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    public IEnumerable<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
}

/// <summary>
/// DTO for login result.
/// </summary>
public class LoginResultDto
{
    public bool Success { get; set; }
    public int UserId { get; set; }
    public string UserType { get; set; } = string.Empty; // Admin, Doctor, Patient
    public string Name { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
