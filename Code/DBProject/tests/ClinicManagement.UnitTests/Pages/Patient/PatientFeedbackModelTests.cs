using ClinicManagement.Application.DTOs;
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
/// Unit tests for PatientFeedbackModel page model.
/// </summary>
public class PatientFeedbackModelTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _patientService;

    public PatientFeedbackModelTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_patientRepoMock.Object, _appointmentRepoMock.Object, _loggerMock.Object);
    }

    private PatientFeedbackModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new PatientFeedbackModel(_patientService);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession();
        if (sessionUserId.HasValue)
        {
            ((MockSession)httpContext.Session).SetInt32("UserId", sessionUserId.Value);
        }
        model.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
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
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPendingFeedback()
    {
        // Arrange
        var appointment = new ClinicManagement.Domain.Entities.Appointment
        {
            AppointId = 10,
            FeedbackStatus = 2
        };
        _appointmentRepoMock.Setup(r => r.GetPendingFeedbackByPatientAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.PendingFeedback.Should().NotBeNull();
        model.PendingFeedback!.AppointId.Should().Be(10);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoPendingFeedback_ReturnsPageWithNullFeedback()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetPendingFeedbackByPatientAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClinicManagement.Domain.Entities.Appointment?)null);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.PendingFeedback.Should().BeNull();
    }

    [Fact]
    public async Task OnPostAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnPostAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostAsync_WhenFeedbackSubmitSucceeds_SetsSuccessMessage()
    {
        // Arrange
        var appointment = new ClinicManagement.Domain.Entities.Appointment
        {
            AppointId = 5,
            FeedbackStatus = 2
        };
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);
        _appointmentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ClinicManagement.Domain.Entities.Appointment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnPostAsync(5, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("Thank you");
    }

    [Fact]
    public async Task OnPostAsync_WhenFeedbackSubmitFails_SetsFailureMessage()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClinicManagement.Domain.Entities.Appointment?)null);
        _appointmentRepoMock.Setup(r => r.GetPendingFeedbackByPatientAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClinicManagement.Domain.Entities.Appointment?)null);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnPostAsync(99, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeFalse();
        model.Message.Should().Contain("Failed");
    }

    [Fact]
    public void PatientFeedbackModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Message.Should().BeEmpty();
        model.Success.Should().BeFalse();
        model.PendingFeedback.Should().BeNull();
    }
}
