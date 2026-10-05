namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for department information.
/// </summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
