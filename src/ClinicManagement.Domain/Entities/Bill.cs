using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents a bill for a patient appointment.
/// </summary>
public class Bill
{
    public int BillId { get; set; }
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public decimal Amount { get; set; }
    public BillStatus Status { get; set; } = BillStatus.Pending;
    public DateTime BillDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Appointment? Appointment { get; set; }
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}
