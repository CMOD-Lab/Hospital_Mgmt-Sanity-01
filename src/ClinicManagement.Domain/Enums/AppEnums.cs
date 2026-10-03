namespace ClinicManagement.Domain.Enums;

/// <summary>
/// Represents the status of an appointment.
/// </summary>
public enum AppointmentStatus
{
    Pending = 0,
    Approved = 1,
    Completed = 2,
    Cancelled = 3
}

/// <summary>
/// Represents the payment status of a bill.
/// </summary>
public enum BillStatus
{
    Pending = 0,
    Paid = 1,
    Unpaid = 2
}

/// <summary>
/// Represents the type of user in the system.
/// </summary>
public enum UserType
{
    Admin = 0,
    Doctor = 1,
    Patient = 2
}
