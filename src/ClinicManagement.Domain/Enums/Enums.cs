namespace ClinicManagement.Domain.Enums;

/// <summary>User type enumeration.</summary>
public enum UserType
{
    Patient = 1,
    Doctor = 2,
    Admin = 3
}

/// <summary>Appointment status enumeration.</summary>
public enum AppointmentStatus
{
    Approved = 1,
    Pending = 2,
    Completed = 3,
    Rejected = 4
}

/// <summary>Notification status enumeration.</summary>
public enum NotificationStatus
{
    Seen = 1,
    Unseen = 2
}

/// <summary>Feedback status enumeration.</summary>
public enum FeedbackStatus
{
    Given = 1,
    Pending = 2
}

/// <summary>Doctor status enumeration.</summary>
public enum DoctorStatus
{
    Left = 0,
    Present = 1
}
