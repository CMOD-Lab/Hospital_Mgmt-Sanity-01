namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents a department in the clinic.
/// </summary>
public class Department
{
    public int DepartmentId { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
