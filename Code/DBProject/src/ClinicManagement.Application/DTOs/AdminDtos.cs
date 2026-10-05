namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for staff display.
/// </summary>
public class StaffDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public float? Salary { get; set; }
}

/// <summary>
/// DTO for adding new staff.
/// </summary>
public class AddStaffDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>
/// DTO for admin home statistics.
/// </summary>
public class AdminHomeDto
{
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public float TotalIncome { get; set; }
    public IEnumerable<DepartmentStatsDto> DepartmentStats { get; set; } = new List<DepartmentStatsDto>();
    public IEnumerable<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
}

/// <summary>
/// DTO for department statistics.
/// </summary>
public class DepartmentStatsDto
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
}
