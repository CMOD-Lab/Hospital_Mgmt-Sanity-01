using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for appointment data.
/// </summary>
public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int TimeSlotId { get; set; }
    public string Timings { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public bool FeedbackGiven { get; set; }
}

/// <summary>
/// DTO for creating a new appointment.
/// </summary>
public class AppointmentCreateDto
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int TimeSlotId { get; set; }
}

/// <summary>
/// DTO for updating appointment prescription.
/// </summary>
public class AppointmentUpdateDto
{
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>
/// DTO for time slot data.
/// </summary>
public class TimeSlotDto
{
    public int TimeSlotId { get; set; }
    public int DoctorId { get; set; }
    public string Timings { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}
