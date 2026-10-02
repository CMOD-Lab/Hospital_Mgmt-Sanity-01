using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for domain entities and enums.
/// Note: To run tests, install xunit and Microsoft.NET.Test.Sdk packages.
/// </summary>
public class DomainEntityTests
{
    public void Patient_ShouldHaveCorrectDefaults()
    {
        var patient = new Patient
        {
            PatientId = 1,
            Name = "John Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = 'M'
        };

        if (patient.PatientId != 1) throw new Exception("PatientId should be 1");
        if (patient.Name != "John Doe") throw new Exception("Name should be John Doe");
        if (patient.Gender != 'M') throw new Exception("Gender should be M");
        if (patient.Appointments == null) throw new Exception("Appointments should not be null");
    }

    public void Doctor_ShouldHaveCorrectStatus()
    {
        var doctor = new Doctor
        {
            DoctorId = 1,
            Name = "Dr. Smith",
            Status = DoctorStatus.Present,
            Qualification = "MBBS",
            BirthDate = new DateTime(1980, 1, 1),
            Gender = 'M',
            DeptNo = 1,
            ChargesPerVisit = 500.0
        };

        if (doctor.Status != DoctorStatus.Present) throw new Exception("Status should be Present");
        if (doctor.Name != "Dr. Smith") throw new Exception("Name should be Dr. Smith");
        if (doctor.ChargesPerVisit != 500.0) throw new Exception("ChargesPerVisit should be 500.0");
    }

    public void Appointment_ShouldHaveCorrectStatus()
    {
        var appointment = new Appointment
        {
            AppointId = 1,
            AppointmentStatus = AppointmentStatus.Pending,
            FeedbackStatus = FeedbackStatus.Pending,
            DoctorNotification = NotificationStatus.Unseen,
            PatientNotification = NotificationStatus.Unseen
        };

        if (appointment.AppointmentStatus != AppointmentStatus.Pending)
            throw new Exception("AppointmentStatus should be Pending");
        if (appointment.FeedbackStatus != FeedbackStatus.Pending)
            throw new Exception("FeedbackStatus should be Pending");
    }

    public void LoginTable_ShouldStoreUserType()
    {
        var login = new LoginTable
        {
            LoginId = 1,
            Email = "test@test.com",
            Password = "pass123",
            Type = UserType.Patient
        };

        if (login.Type != UserType.Patient) throw new Exception("Type should be Patient");
        if (login.Email != "test@test.com") throw new Exception("Email should be test@test.com");
    }
}
