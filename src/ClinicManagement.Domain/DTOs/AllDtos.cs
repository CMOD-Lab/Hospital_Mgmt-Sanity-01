namespace ClinicManagement.Domain.DTOs;

/// <summary>Result DTO for login validation.</summary>
public class LoginResultDto
{
    /// <summary>0=Success, 1=Email not found, 2=Wrong password, -1=Error</summary>
    public int Status { get; set; }
    public int UserId { get; set; }
    /// <summary>1=Patient, 2=Doctor, 3=Admin</summary>
    public int UserType { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>Result DTO for patient sign-up.</summary>
public class SignUpResultDto
{
    /// <summary>0=Email exists, 1=Success, -1=Error</summary>
    public int Status { get; set; }
    public int PatientId { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>DTO for patient sign-up form data.</summary>
public class PatientSignUpDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

/// <summary>DTO for admin home dashboard data.</summary>
public class AdminHomeDto
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentViewDto> Departments { get; set; } = new List<DepartmentViewDto>();
    public IEnumerable<AppointmentViewDto> Appointments { get; set; } = new List<AppointmentViewDto>();
}

/// <summary>DTO for department view in admin dashboard.</summary>
public class DepartmentViewDto
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
}

/// <summary>DTO for appointment view in admin dashboard.</summary>
public class AppointmentViewDto
{
    public int AppointID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string? Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>DTO for adding a new doctor.</summary>
public class AddDoctorDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
}

/// <summary>DTO for adding a new staff member.</summary>
public class AddStaffDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>DTO for doctor list item.</summary>
public class DoctorListItemDto
{
    public int DoctorID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

/// <summary>DTO for patient list item.</summary>
public class PatientListItemDto
{
    public int PatientID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

/// <summary>DTO for staff list item.</summary>
public class StaffListItemDto
{
    public int StaffID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>DTO for department information.</summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>DTO for patient information display.</summary>
public class PatientInfoDto
{
    public int PatientID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string BirthDate { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}

/// <summary>DTO for bill history entry.</summary>
public class BillHistoryDto
{
    public int AppointID { get; set; }
    public string? DoctorName { get; set; }
    public string? Date { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>DTO for current appointment.</summary>
public class CurrentAppointmentDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>DTO for treatment history entry.</summary>
public class TreatmentHistoryDto
{
    public int AppointID { get; set; }
    public string? DoctorName { get; set; }
    public string? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>DTO for doctor profile display.</summary>
public class DoctorProfileDto
{
    public int DoctorID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public double ChargesPerVisit { get; set; }
    public double ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public int WorkExperience { get; set; }
    public int Age { get; set; }
}

/// <summary>DTO for appointment slot.</summary>
public class AppointmentSlotDto
{
    public int SlotId { get; set; }
    public string Timings { get; set; } = string.Empty;
}

/// <summary>DTO for notification.</summary>
public class NotificationDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>DTO for pending feedback.</summary>
public class PendingFeedbackDto
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>DTO for doctor info display.</summary>
public class DoctorInfoDto
{
    public int DoctorID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string BirthDate { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public double ChargesPerVisit { get; set; }
    public double? MonthlySalary { get; set; }
    public double? ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public int? WorkExperience { get; set; }
}

/// <summary>DTO for pending appointment (doctor view).</summary>
public class PendingAppointmentDto
{
    public int AppointID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>DTO for today's appointment (doctor view).</summary>
public class TodayAppointmentDto
{
    public int AppointID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>DTO for billable appointment (doctor view).</summary>
public class BillableAppointmentDto
{
    public int AppointID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
    public string? Date { get; set; }
}

/// <summary>DTO for patient history (doctor view).</summary>
public class PatientHistoryDto
{
    public int AppointID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public string? BillStatus { get; set; }
}
