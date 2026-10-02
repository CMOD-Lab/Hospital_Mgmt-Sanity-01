using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Entities;

/// <summary>Represents a login account in the system.</summary>
public class LoginTable
{
    public int LoginId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserType Type { get; set; }

    // Navigation properties
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}

/// <summary>Represents a patient in the clinic.</summary>
public class Patient
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public char Gender { get; set; }

    // Navigation properties
    public LoginTable? Login { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

/// <summary>Represents a department in the clinic.</summary>
public class Department
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}

/// <summary>Represents a doctor in the clinic.</summary>
public class Doctor
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public char Gender { get; set; }

    public int DeptNo { get; set; }
    public double ChargesPerVisit { get; set; }
    public double? MonthlySalary { get; set; }
    public double? ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }

    public string Qualification { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public int? WorkExperience { get; set; }

    public DoctorStatus Status { get; set; }

    // Navigation properties
    public Department? Department { get; set; }
    public LoginTable? Login { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

/// <summary>Represents other (non-doctor) staff members.</summary>
public class OtherStaff
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Designation { get; set; } = string.Empty;
    public char Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? HighestQualification { get; set; }
    public double? Salary { get; set; }
}

/// <summary>Represents an appointment between a patient and a doctor.</summary>
public class Appointment
{
    public int AppointId { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public DateTime? Date { get; set; }
    public AppointmentStatus AppointmentStatus { get; set; }

    public double? BillAmount { get; set; }
    public BillStatus? BillStatus { get; set; }

    public NotificationStatus DoctorNotification { get; set; }
    public NotificationStatus PatientNotification { get; set; }
    public FeedbackStatus FeedbackStatus { get; set; }

    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
}
