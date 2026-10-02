namespace ClinicManagement.Domain.Enums;

/// <summary>User type in the system.</summary>
public enum UserType
{
    Patient = 1,
    Doctor = 2,
    Admin = 3
}

/// <summary>Doctor employment status.</summary>
public enum DoctorStatus
{
    Left = 0,
    Present = 1
}

/// <summary>Appointment lifecycle status.</summary>
public enum AppointmentStatus
{
    Approved = 1,
    Pending = 2,
    Completed = 3,
    Rejected = 4
}

/// <summary>Bill payment status.</summary>
public enum BillStatus
{
    Unpaid,
    Paid
}

/// <summary>Notification read status.</summary>
public enum NotificationStatus
{
    Seen = 1,
    Unseen = 2
}

/// <summary>Feedback submission status.</summary>
public enum FeedbackStatus
{
    Given = 1,
    Pending = 2
}
