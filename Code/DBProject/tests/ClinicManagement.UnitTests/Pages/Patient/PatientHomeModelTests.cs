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
using PatientEntity = ClinicManagement.Domain.Entities.Patient;

namespace ClinicManagement.UnitTests.Pages.Patient;

/// <summary>
/// Unit tests for PatientHomeModel page model.
/// </summary>
public class PatientHomeModelTests
{
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<ILogger<PatientService>> _patientServiceLoggerMock;
    private readonly Mock<ILogger<PatientHomeModel>> _loggerMock;
    private readonly PatientService _patientService;

    public PatientHomeModelTests()
    {
        _patientRepoMock = new Mock<IPatientRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _patientServiceLoggerMock = new Mock<ILogger<PatientService>>();
        _loggerMock = new Mock<ILogger<PatientHomeModel>>();
        _patientService = new PatientService(_patientRepoMock.Object, _appointmentRepoMock.Object, _patientServiceLoggerMock.Object);
    }

    private PatientHomeModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new PatientHomeModel(_patientService, _loggerMock.Object);
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
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPatientInfo()
    {
        // Arrange
        var patient = new PatientEntity
        {
            PatientId = 1,
            Name = "John Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = 'M'
        };
        _patientRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Patient.Should().NotBeNull();
        model.Patient!.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task OnGetAsync_WhenPatientNotFound_ReturnsPageWithNullPatient()
    {
        // Arrange
        _patientRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PatientEntity?)null);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Patient.Should().BeNull();
    }

    [Fact]
    public void PatientName_WhenPatientIsNull_ReturnsDefaultName()
    {
        // Arrange
        var model = CreatePageModel(1);

        // Act
        var name = model.PatientName;

        // Assert
        name.Should().Be("Patient");
    }

    [Fact]
    public async Task PatientName_WhenPatientLoaded_ReturnsPatientName()
    {
        // Arrange
        var patient = new PatientEntity
        {
            PatientId = 1,
            Name = "Jane Smith",
            BirthDate = new DateTime(1985, 5, 15),
            Gender = 'F'
        };
        _patientRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        var model = CreatePageModel(1);
        await model.OnGetAsync(CancellationToken.None);

        // Act
        var name = model.PatientName;

        // Assert
        name.Should().Be("Jane Smith");
    }

    [Fact]
    public void PatientHomeModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Patient.Should().BeNull();
        model.PatientName.Should().Be("Patient");
    }
}
