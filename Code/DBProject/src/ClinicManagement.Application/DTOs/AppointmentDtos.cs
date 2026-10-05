namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for appointment display.
/// </summary>
public class AppointmentDto
{
    public int AppointId { get; set; }
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public int? PatientId { get; set; }
    public string? PatientName { get; set; }
    public DateTime? Date { get; set; }
    public string? Timings { get; set; }
    public int AppointmentStatus { get; set; }
    public float? BillAmount { get; set; }
    public string? BillStatus { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public int? FeedbackStatus { get; set; }
}

/// <summary>
/// DTO for bill history display.
/// </summary>
public class BillHistoryDto
{
    public int AppointId { get; set; }
    public string? DoctorName { get; set; }
    public DateTime? Date { get; set; }
    public float? BillAmount { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>
/// DTO for treatment history display.
/// </summary>
public class TreatmentHistoryDto
{
    public int AppointId { get; set; }
    public string? DoctorName { get; set; }
    public DateTime? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>
/// DTO for inserting a new appointment.
/// </summary>
public class InsertAppointmentDto
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int FreeSlotAppointId { get; set; }
}

/// <summary>
/// DTO for updating prescription.
/// </summary>
public class UpdatePrescriptionDto
{
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}
