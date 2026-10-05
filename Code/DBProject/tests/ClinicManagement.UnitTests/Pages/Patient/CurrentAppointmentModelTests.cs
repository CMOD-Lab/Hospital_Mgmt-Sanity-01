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
/// Unit tests for CurrentAppointmentModel page model.
/// </summary>
public class CurrentAppointmentModelTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _patientService;

    public CurrentAppointmentModelTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_patientRepoMock.Object, _appointmentRepoMock.Object, _loggerMock.Object);
    }

    private CurrentAppointmentModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new CurrentAppointmentModel(_patientService);
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
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsCurrentAppointment()
    {
        // Arrange
        var appointment = new ClinicManagement.Domain.Entities.Appointment
        {
            AppointId = 7,
            AppointmentStatus = 2,
            Date = DateTime.Now
        };
        _appointmentRepoMock.Setup(r => r.GetCurrentByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Appointment.Should().NotBeNull();
        model.Appointment!.AppointId.Should().Be(7);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoCurrentAppointment_ReturnsPageWithNullAppointment()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetCurrentByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClinicManagement.Domain.Entities.Appointment?)null);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Appointment.Should().BeNull();
    }

    [Fact]
    public void CurrentAppointmentModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Appointment.Should().BeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenRepositoryThrows_ReturnsPageWithNullAppointment()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetCurrentByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Appointment.Should().BeNull();
    }
}
