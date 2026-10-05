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
/// Unit tests for PreviousHistoryModel page model.
/// </summary>
public class PreviousHistoryModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public PreviousHistoryModelTests()
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

    private PreviousHistoryModel CreatePageModel(int? sessionUserId = 2)
    {
        var model = new PreviousHistoryModel(_doctorService);
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
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsPreviousHistory()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new() { AppointId = 1, AppointmentStatus = 3, Disease = "Flu" },
            new() { AppointId = 2, AppointmentStatus = 3, Disease = "Fever" }
        };
        _appointmentRepoMock.Setup(r => r.GetByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.History.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoHistory_ReturnsEmptyList()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.History.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_FiltersOnlyCompletedAppointments()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new() { AppointId = 1, AppointmentStatus = 3, Disease = "Flu" },   // Completed
            new() { AppointId = 2, AppointmentStatus = 2, Disease = "Cold" }   // Pending - excluded
        };
        _appointmentRepoMock.Setup(r => r.GetByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.History.Should().HaveCount(1);
        model.History.First().AppointId.Should().Be(1);
    }

    [Fact]
    public void PreviousHistoryModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(2);

        // Assert
        model.History.Should().NotBeNull();
        model.History.Should().BeEmpty();
    }
}
