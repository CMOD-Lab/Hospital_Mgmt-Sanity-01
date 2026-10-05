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
/// Unit tests for TreatmentHistoryModel page model.
/// </summary>
public class TreatmentHistoryModelTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _patientService;

    public TreatmentHistoryModelTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _patientRepoMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_patientRepoMock.Object, _appointmentRepoMock.Object, _loggerMock.Object);
    }

    private TreatmentHistoryModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new TreatmentHistoryModel(_patientService);
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
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsTreatmentHistory()
    {
        // Arrange
        var appointments = new List<ClinicManagement.Domain.Entities.Appointment>
        {
            new() { AppointId = 1, AppointmentStatus = 3, Disease = "Flu" },
            new() { AppointId = 2, AppointmentStatus = 3, Disease = "Cold" }
        };
        _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Treatments.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoTreatments_ReturnsEmptyList()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ClinicManagement.Domain.Entities.Appointment>());

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Treatments.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_FiltersOnlyCompletedAppointments()
    {
        // Arrange
        var appointments = new List<ClinicManagement.Domain.Entities.Appointment>
        {
            new() { AppointId = 1, AppointmentStatus = 3, Disease = "Flu" },   // Completed
            new() { AppointId = 2, AppointmentStatus = 2, Disease = "Cold" },  // Pending - should be excluded
            new() { AppointId = 3, AppointmentStatus = 1, Disease = "Fever" }  // Free slot - should be excluded
        };
        _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Treatments.Should().HaveCount(1);
        model.Treatments.First().AppointId.Should().Be(1);
    }

    [Fact]
    public void TreatmentHistoryModel_Constructor_InitializesEmptyTreatments()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Treatments.Should().NotBeNull();
        model.Treatments.Should().BeEmpty();
    }
}
