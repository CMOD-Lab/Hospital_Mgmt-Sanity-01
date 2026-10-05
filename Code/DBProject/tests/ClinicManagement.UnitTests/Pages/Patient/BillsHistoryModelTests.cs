using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
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
/// Unit tests for BillsHistoryModel page model.
/// </summary>
public class BillsHistoryModelTests
{
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _patientService;

    public BillsHistoryModelTests()
    {
        _patientRepoMock = new Mock<IPatientRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_patientRepoMock.Object, _appointmentRepoMock.Object, _loggerMock.Object);
    }

    private BillsHistoryModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new BillsHistoryModel(_patientService);
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
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsBillHistory()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new() { AppointId = 1, BillAmount = 100.0f, BillStatus = "Paid" },
            new() { AppointId = 2, BillAmount = 200.0f, BillStatus = "Unpaid" }
        };
        _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Bills.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoBills_ReturnsEmptyList()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Bills.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_FiltersOnlyAppointmentsWithBillAmount()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new() { AppointId = 1, BillAmount = 100.0f, BillStatus = "Paid" },
            new() { AppointId = 2, BillAmount = null, BillStatus = null }  // No bill - should be excluded
        };
        _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Bills.Should().HaveCount(1);
        model.Bills.First().AppointId.Should().Be(1);
    }

    [Fact]
    public void BillsHistoryModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.Bills.Should().NotBeNull();
        model.Bills.Should().BeEmpty();
    }
}
