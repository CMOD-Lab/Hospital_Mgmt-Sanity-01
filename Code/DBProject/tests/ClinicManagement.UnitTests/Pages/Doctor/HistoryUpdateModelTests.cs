using ClinicManagement.Application.DTOs;
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
/// Unit tests for HistoryUpdateModel page model.
/// </summary>
public class HistoryUpdateModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public HistoryUpdateModelTests()
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

    private HistoryUpdateModel CreatePageModel(int? sessionUserId = 2)
    {
        var model = new HistoryUpdateModel(_doctorService);
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
    public void OnGet_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = model.OnGet(1);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public void OnGet_WhenUserLoggedIn_SetsAppointmentIdAndReturnsPage()
    {
        // Arrange
        var model = CreatePageModel(2);

        // Act
        var result = model.OnGet(42);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.AppointmentId.Should().Be(42);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnPostAsync(1, "Flu", "Improving", "Paracetamol", CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostAsync_WhenUpdateSucceeds_SetsSuccessMessage()
    {
        // Arrange
        var appointment = new Appointment { AppointId = 1, AppointmentStatus = 2 };
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);
        _appointmentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnPostAsync(1, "Flu", "Improving", "Paracetamol", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("successfully");
        model.Disease.Should().Be("Flu");
        model.Progress.Should().Be("Improving");
        model.Prescription.Should().Be("Paracetamol");
    }

    [Fact]
    public async Task OnPostAsync_WhenUpdateFails_SetsFailureMessage()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnPostAsync(99, "Flu", "Improving", "Paracetamol", CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeFalse();
        model.Message.Should().Contain("Failed");
    }

    [Fact]
    public void HistoryUpdateModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(2);

        // Assert
        model.Disease.Should().BeEmpty();
        model.Progress.Should().BeEmpty();
        model.Prescription.Should().BeEmpty();
        model.Message.Should().BeEmpty();
        model.Success.Should().BeFalse();
        model.AppointmentId.Should().Be(0);
    }
}
