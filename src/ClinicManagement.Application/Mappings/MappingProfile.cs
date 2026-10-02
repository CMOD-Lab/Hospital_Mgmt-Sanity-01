using AutoMapper;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;

namespace ClinicManagement.Application.Mappings;

/// <summary>AutoMapper profile for entity-to-DTO mappings.</summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient mappings
        CreateMap<Patient, PatientProfileData>()
            .ForMember(d => d.PatientId, o => o.MapFrom(s => s.PatientId))
            .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate.ToString("yyyy-MM-dd")))
            .ForMember(d => d.Age, o => o.MapFrom(s => CalculateAge(s.BirthDate)))
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender.ToString()));

        // Doctor mappings
        CreateMap<Doctor, DoctorProfileData>()
            .ForMember(d => d.DoctorId, o => o.MapFrom(s => s.DoctorId))
            .ForMember(d => d.ReputeIndex, o => o.MapFrom(s => s.ReputeIndex ?? 0.0))
            .ForMember(d => d.WorkExperience, o => o.MapFrom(s => s.WorkExperience ?? 0))
            .ForMember(d => d.Age, o => o.MapFrom(s => CalculateAge(s.BirthDate)))
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender.ToString()))
            .ForMember(d => d.DeptName, o => o.MapFrom(s => s.Department != null ? s.Department.DeptName : null));

        // Doctor dashboard
        CreateMap<Doctor, DoctorDashboardData>()
            .ForMember(d => d.DoctorId, o => o.MapFrom(s => s.DoctorId))
            .ForMember(d => d.DeptName, o => o.MapFrom(s => s.Department != null ? s.Department.DeptName : null));

        // Department
        CreateMap<Department, DeptInfo>()
            .ForMember(d => d.DeptNo, o => o.MapFrom(s => s.DeptNo));

        // Appointment
        CreateMap<Appointment, AppointmentSummary>()
            .ForMember(d => d.AppointId, o => o.MapFrom(s => s.AppointId))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : null))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor != null ? s.Doctor.Name : null))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.AppointmentStatus.ToString()));

        CreateMap<Appointment, PendingAppointmentItem>()
            .ForMember(d => d.AppointId, o => o.MapFrom(s => s.AppointId))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : null))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.AppointmentStatus.ToString()));

        CreateMap<Appointment, TodayAppointmentItem>()
            .ForMember(d => d.AppointId, o => o.MapFrom(s => s.AppointId))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : null));

        CreateMap<Appointment, BillItem>()
            .ForMember(d => d.AppointId, o => o.MapFrom(s => s.AppointId))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : null))
            .ForMember(d => d.BillStatus, o => o.MapFrom(s => s.BillStatus.HasValue ? s.BillStatus.ToString() : null));

        CreateMap<Appointment, BillHistoryItem>()
            .ForMember(d => d.AppointId, o => o.MapFrom(s => s.AppointId))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor != null ? s.Doctor.Name : null))
            .ForMember(d => d.BillStatus, o => o.MapFrom(s => s.BillStatus.HasValue ? s.BillStatus.ToString() : null));

        CreateMap<Appointment, TreatmentHistoryItem>()
            .ForMember(d => d.AppointId, o => o.MapFrom(s => s.AppointId))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor != null ? s.Doctor.Name : null));

        // Doctor list item
        CreateMap<Doctor, DoctorListItem>()
            .ForMember(d => d.DoctorId, o => o.MapFrom(s => s.DoctorId))
            .ForMember(d => d.DeptName, o => o.MapFrom(s => s.Department != null ? s.Department.DeptName : null));

        // Department summary
        CreateMap<Department, DepartmentSummary>()
            .ForMember(d => d.DoctorCount, o => o.MapFrom(s => s.Doctors.Count(d => d.Status == Domain.Enums.DoctorStatus.Present)));
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}
