namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents the login credentials table.
/// Type: 1=Patient, 2=Doctor, 3=Admin
/// </summary>
public class LoginTable
{
    public int LoginId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    /// <summary>1=Patient, 2=Doctor, 3=Admin</summary>
    public int Type { get; set; }
}
