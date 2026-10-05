namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents an appointment in the clinic management system.
/// Status: 1=Approved, 2=Pending, 3=Completed, 4=Rejected
/// </summary>
public class Appointment
{
    public int AppointId { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public DateTime? Date { get; set; }

    /// <summary>1=Approved, 2=Pending, 3=Completed, 4=Rejected</summary>
    public int AppointmentStatus { get; set; }

    public float? BillAmount { get; set; }
    /// <summary>Paid or Unpaid</summary>
    public string? BillStatus { get; set; }

    /// <summary>1=Seen, 2=Unseen</summary>
    public int? DoctorNotification { get; set; }
    /// <summary>1=Seen, 2=Unseen</summary>
    public int? PatientNotification { get; set; }
    /// <summary>1=Given, 2=Pending</summary>
    public int? FeedbackStatus { get; set; }

    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }

    // Navigation
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
}
