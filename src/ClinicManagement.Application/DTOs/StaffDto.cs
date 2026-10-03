namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for staff data.
/// </summary>
public class StaffDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new staff member.
/// </summary>
public class StaffCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}

/// <summary>
/// DTO for updating a staff member.
/// </summary>
public class StaffUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}
