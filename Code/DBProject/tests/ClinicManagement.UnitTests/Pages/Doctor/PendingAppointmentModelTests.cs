using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.UnitTests.Helpers;
using ClinicManagement.Web.Pages.Doctor;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Pages.Doctor;

/// <summary>
/// Unit tests for PendingAppointmentModel page model.
/// </summary>
public class PendingAppointmentModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public PendingAppointmentModelTests()
    {
        _doctorRepoMock = new Mock<IDoctorRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _loginRepoMock = new Mock<ILoginRepository>();
        _loggerMock = new Mock<ILogger<DoctorService>>();
        _doctorService = new DoctorService(
            _doctorRepoMock.Object,
            _appointmentRepoMock.Object,
            _departmentRepoMock.Object,
            _loginRepoMock.Object,
            _loggerMock.Object);
    }

    private PendingAppointmentModel CreatePageModel(int? sessionUserId = 2)
    {
        var model = new PendingAppointmentModel(_doctorService);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession();
        if (sessionUserId.HasValue)
        {
            ((MockSession)httpContext.Session).SetInt32("UserId", sessionUserId.Value);
        }
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPendingAppointments()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new() { AppointId = 1, AppointmentStatus = 2 },
            new() { AppointId = 2, AppointmentStatus = 2 }
        };
        _appointmentRepoMock.Setup(r => r.GetPendingByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Appointments.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnPostApproveAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenApproveSucceeds_SetsSuccessMessage()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.ApproveAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _appointmentRepoMock.Setup(r => r.GetPendingByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnPostApproveAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("approved");
    }

    [Fact]
    public async Task OnPostRejectAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnPostRejectAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostRejectAsync_WhenRejectSucceeds_SetsSuccessMessage()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _appointmentRepoMock.Setup(r => r.GetPendingByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnPostRejectAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("rejected");
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenApproveThrows_SetsFailureMessage()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.ApproveAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));
        _appointmentRepoMock.Setup(r => r.GetPendingByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnPostApproveAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeFalse();
        model.Message.Should().Contain("Failed");
    }

    [Fact]
    public void PendingAppointmentModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(2);

        // Assert
        model.Appointments.Should().NotBeNull();
        model.Appointments.Should().BeEmpty();
        model.Message.Should().BeEmpty();
        model.Success.Should().BeFalse();
    }
}
