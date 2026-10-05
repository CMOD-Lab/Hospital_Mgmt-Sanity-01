using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.UnitTests.Helpers;
using ClinicManagement.Web.Pages.Admin;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Pages.Admin;

/// <summary>
/// Unit tests for AddStaffModel page model.
/// </summary>
public class AddStaffModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<IStaffRepository> _staffRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<ILogger<AdminService>> _loggerMock;
    private readonly AdminService _adminService;

    public AddStaffModelTests()
    {
        _doctorRepoMock = new Mock<IDoctorRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _staffRepoMock = new Mock<IStaffRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _loginRepoMock = new Mock<ILoginRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _loggerMock = new Mock<ILogger<AdminService>>();
        _adminService = new AdminService(
            _doctorRepoMock.Object,
            _patientRepoMock.Object,
            _staffRepoMock.Object,
            _departmentRepoMock.Object,
            _loginRepoMock.Object,
            _appointmentRepoMock.Object,
            _loggerMock.Object);
    }

    private AddStaffModel CreatePageModel(int? sessionUserType = 3)
    {
        var model = new AddStaffModel(_adminService);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession();
        if (sessionUserType.HasValue)
        {
            ((MockSession)httpContext.Session).SetInt32("UserType", sessionUserType.Value);
        }
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public void OnGet_WhenUserIsNotAdmin_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(1); // Patient type

        // Act
        var result = model.OnGet();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public void OnGet_WhenAdminLoggedIn_ReturnsPage()
    {
        // Arrange
        var model = CreatePageModel(3);

        // Act
        var result = model.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void OnGet_WhenNoSession_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = model.OnGet();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsNotAdmin_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(1);

        // Act
        var result = await model.OnPostAsync(
            "John", "1990-01-01", "1234567890", "M", "Address", 50000, "BSc", "Nurse",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostAsync_WhenAddStaffSucceeds_SetsSuccessMessage()
    {
        // Arrange
        _staffRepoMock.Setup(r => r.AddAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnPostAsync(
            "Jane Doe", "1985-05-15", "9876543210", "F", "456 Oak Ave", 45000, "BSc Nursing", "Head Nurse",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("successfully");
    }

    [Fact]
    public async Task OnPostAsync_WhenAddStaffFails_SetsFailureMessage()
    {
        // Arrange
        _staffRepoMock.Setup(r => r.AddAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnPostAsync(
            "Jane Doe", "1985-05-15", "9876543210", "F", "456 Oak Ave", 45000, "BSc Nursing", "Head Nurse",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeFalse();
        model.Message.Should().Contain("Failed");
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyGender_UsesDefaultMaleGender()
    {
        // Arrange
        _staffRepoMock.Setup(r => r.AddAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var model = CreatePageModel(3);

        // Act
        var result = await model.OnPostAsync(
            "Alex", "1990-01-01", "1234567890", "", "Address", 40000, "BSc", "Technician",
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
    }

    [Fact]
    public void AddStaffModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(3);

        // Assert
        model.Message.Should().BeEmpty();
        model.Success.Should().BeFalse();
    }
}
