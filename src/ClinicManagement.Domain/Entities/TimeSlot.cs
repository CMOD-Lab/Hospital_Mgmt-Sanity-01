namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents a time slot for doctor appointments.
/// </summary>
public class TimeSlot
{
    public int TimeSlotId { get; set; }
    public int DoctorId { get; set; }
    public string Timings { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
