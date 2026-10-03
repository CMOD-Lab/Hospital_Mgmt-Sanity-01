using System;
using ClinicManagement.Application.DTOs;
using Xunit;

namespace ClinicManagement.UnitTests.Web.DTOs;

public class PatientDtoTests
{
    [Fact]
    public void PatientDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new PatientDto();

        // Assert
        Assert.Equal(0, dto.PatientId);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.Phone);
        Assert.Equal(string.Empty, dto.Address);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void PatientDto_Age_CalculatedCorrectly()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var dto = new PatientDto { BirthDate = birthDate };

        // Act
        var age = dto.Age;

        // Assert
        var expectedAge = DateTime.Today.Year - 1990 - (DateTime.Today.DayOfYear < birthDate.DayOfYear ? 1 : 0);
        Assert.Equal(expectedAge, age);
    }

    [Fact]
    public void PatientDto_CanSetAllProperties()
    {
        // Arrange
        var birthDate = new DateTime(1985, 6, 15);
        var dto = new PatientDto
        {
            PatientId = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = "123 Main St",
            BirthDate = birthDate,
            Gender = "M",
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.PatientId);
        Assert.Equal("John Doe", dto.Name);
        Assert.Equal("john@example.com", dto.Email);
        Assert.Equal("1234567890", dto.Phone);
        Assert.Equal("123 Main St", dto.Address);
        Assert.Equal(birthDate, dto.BirthDate);
        Assert.Equal("M", dto.Gender);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void PatientCreateDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new PatientCreateDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.Password);
        Assert.Equal(string.Empty, dto.Phone);
        Assert.Equal(string.Empty, dto.Address);
        Assert.Equal(string.Empty, dto.Gender);
    }

    [Fact]
    public void PatientCreateDto_CanSetAllProperties()
    {
        // Arrange
        var birthDate = new DateTime(1990, 3, 20);
        var dto = new PatientCreateDto
        {
            Name = "Jane Doe",
            Email = "jane@example.com",
            Password = "securepass",
            Phone = "9876543210",
            Address = "456 Oak Ave",
            BirthDate = birthDate,
            Gender = "F"
        };

        // Assert
        Assert.Equal("Jane Doe", dto.Name);
        Assert.Equal("jane@example.com", dto.Email);
        Assert.Equal("securepass", dto.Password);
        Assert.Equal("9876543210", dto.Phone);
        Assert.Equal("456 Oak Ave", dto.Address);
        Assert.Equal(birthDate, dto.BirthDate);
        Assert.Equal("F", dto.Gender);
    }

    [Fact]
    public void PatientUpdateDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new PatientUpdateDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Phone);
        Assert.Equal(string.Empty, dto.Address);
        Assert.Equal(string.Empty, dto.Gender);
    }
}

public class DoctorDtoTests
{
    [Fact]
    public void DoctorDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new DoctorDto();

        // Assert
        Assert.Equal(0, dto.DoctorId);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(0, dto.Experience);
        Assert.Equal(0m, dto.Salary);
        Assert.Equal(0m, dto.ChargesPerVisit);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void DoctorDto_Age_CalculatedCorrectly()
    {
        // Arrange
        var birthDate = new DateTime(1975, 5, 10);
        var dto = new DoctorDto { BirthDate = birthDate };

        // Act
        var age = dto.Age;

        // Assert
        var expectedAge = DateTime.Today.Year - 1975 - (DateTime.Today.DayOfYear < birthDate.DayOfYear ? 1 : 0);
        Assert.Equal(expectedAge, age);
    }

    [Fact]
    public void DoctorDto_CanSetAllProperties()
    {
        // Arrange
        var dto = new DoctorDto
        {
            DoctorId = 1,
            Name = "Dr. Smith",
            Email = "smith@hospital.com",
            Phone = "1234567890",
            Address = "Hospital St",
            Gender = "M",
            DepartmentId = 2,
            DepartmentName = "Cardiology",
            Specialization = "Heart Surgery",
            Qualification = "MBBS, MD",
            Experience = 15,
            Salary = 100000m,
            ChargesPerVisit = 800m,
            ReputeIndex = 4.5f,
            PatientsTreated = 500,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.DoctorId);
        Assert.Equal("Dr. Smith", dto.Name);
        Assert.Equal(2, dto.DepartmentId);
        Assert.Equal("Cardiology", dto.DepartmentName);
        Assert.Equal(15, dto.Experience);
        Assert.Equal(100000m, dto.Salary);
        Assert.Equal(800m, dto.ChargesPerVisit);
        Assert.Equal(4.5f, dto.ReputeIndex);
        Assert.Equal(500, dto.PatientsTreated);
    }

    [Fact]
    public void DoctorCreateDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new DoctorCreateDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.Password);
        Assert.Equal(0, dto.DepartmentId);
        Assert.Equal(0, dto.Experience);
        Assert.Equal(0m, dto.Salary);
        Assert.Equal(0m, dto.ChargesPerVisit);
    }
}

public class AppointmentDtoTests
{
    [Fact]
    public void AppointmentDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new AppointmentDto();

        // Assert
        Assert.Equal(0, dto.AppointmentId);
        Assert.Equal(string.Empty, dto.PatientName);
        Assert.Equal(string.Empty, dto.DoctorName);
        Assert.Equal(string.Empty, dto.Timings);
        Assert.False(dto.FeedbackGiven);
        Assert.Null(dto.Disease);
        Assert.Null(dto.Progress);
        Assert.Null(dto.Prescription);
    }

    [Fact]
    public void AppointmentCreateDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new AppointmentCreateDto();

        // Assert
        Assert.Equal(0, dto.DoctorId);
        Assert.Equal(0, dto.PatientId);
        Assert.Equal(0, dto.TimeSlotId);
    }

    [Fact]
    public void AppointmentUpdateDto_DefaultValues_AreNull()
    {
        // Arrange & Act
        var dto = new AppointmentUpdateDto();

        // Assert
        Assert.Null(dto.Disease);
        Assert.Null(dto.Progress);
        Assert.Null(dto.Prescription);
    }

    [Fact]
    public void TimeSlotDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new TimeSlotDto();

        // Assert
        Assert.Equal(0, dto.TimeSlotId);
        Assert.Equal(0, dto.DoctorId);
        Assert.Equal(string.Empty, dto.Timings);
        Assert.False(dto.IsAvailable);
    }
}

public class MiscDtoTests
{
    [Fact]
    public void BillDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new BillDto();

        // Assert
        Assert.Equal(0, dto.BillId);
        Assert.Equal(0, dto.AppointmentId);
        Assert.Equal(0, dto.PatientId);
        Assert.Equal(string.Empty, dto.PatientName);
        Assert.Equal(0, dto.DoctorId);
        Assert.Equal(string.Empty, dto.DoctorName);
        Assert.Equal(0m, dto.Amount);
    }

    [Fact]
    public void StaffDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new StaffDto();

        // Assert
        Assert.Equal(0, dto.StaffId);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Phone);
        Assert.Equal(string.Empty, dto.Address);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Designation);
        Assert.Equal(string.Empty, dto.Qualification);
        Assert.Equal(0m, dto.Salary);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void DepartmentDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new DepartmentDto();

        // Assert
        Assert.Equal(0, dto.DepartmentId);
        Assert.Equal(string.Empty, dto.DeptName);
        Assert.Equal(string.Empty, dto.Description);
    }

    [Fact]
    public void AdminDashboardDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new AdminDashboardDto();

        // Assert
        Assert.Equal(0, dto.TotalDoctors);
        Assert.Equal(0, dto.TotalPatients);
        Assert.Equal(0m, dto.TotalIncome);
        Assert.NotNull(dto.Departments);
        Assert.NotNull(dto.RecentAppointments);
    }

    [Fact]
    public void LoginResultDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new LoginResultDto();

        // Assert
        Assert.False(dto.Success);
        Assert.Equal(0, dto.UserId);
        Assert.Equal(0, dto.UserType);
        Assert.Equal(string.Empty, dto.Message);
    }

    [Fact]
    public void LoginResultDto_CanSetAllProperties()
    {
        // Arrange
        var dto = new LoginResultDto
        {
            Success = true,
            UserId = 5,
            UserType = 2,
            Message = "Login successful"
        };

        // Assert
        Assert.True(dto.Success);
        Assert.Equal(5, dto.UserId);
        Assert.Equal(2, dto.UserType);
        Assert.Equal("Login successful", dto.Message);
    }

    [Fact]
    public void StaffCreateDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new StaffCreateDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Phone);
        Assert.Equal(string.Empty, dto.Address);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Designation);
        Assert.Equal(string.Empty, dto.Qualification);
        Assert.Equal(0m, dto.Salary);
    }
}
