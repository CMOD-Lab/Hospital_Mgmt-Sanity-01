using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.UnitTests.Helpers;
using ClinicManagement.Web.Pages.Patient;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Pages.Patient;

/// <summary>
/// Unit tests for PatientNotificationsModel page model.
/// </summary>
public class PatientNotificationsModelTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _patientService;

    public PatientNotificationsModelTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_patientRepoMock.Object, _appointmentRepoMock.Object, _loggerMock.Object);
    }

    private PatientNotificationsModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new PatientNotificationsModel(_patientService);
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
    public async Task OnGetAsync_WhenNotificationExists_ReturnsNotification()
    {
        // Arrange
        var appointment = new ClinicManagement.Domain.Entities.Appointment
        {
            AppointId = 3,
            AppointmentStatus = 2,
            PatientNotification = 2
        };
        _appointmentRepoMock.Setup(r => r.GetNotificationByPatientAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Notification.Should().NotBeNull();
        model.Notification!.AppointId.Should().Be(3);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoNotification_ReturnsPageWithNullNotification()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetNotificationByPatientAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClinicManagement.Domain.Entities.Appointment?)null);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Notification.Should().BeNull();
    }

    [Fact]
    public void PatientNotificationsModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Notification.Should().BeNull();
    }
}
