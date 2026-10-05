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
/// Unit tests for BillModel page model.
/// </summary>
public class BillModelTests
{
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILoginRepository> _loginRepoMock;
    private readonly Mock<ILogger<DoctorService>> _loggerMock;
    private readonly DoctorService _doctorService;

    public BillModelTests()
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

    private BillModel CreatePageModel(int? sessionUserId = 2)
    {
        var model = new BillModel(_doctorService);
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
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsTodaysAppointments()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new() { AppointId = 1, AppointmentStatus = 2, BillAmount = 100 },
            new() { AppointId = 2, AppointmentStatus = 2, BillAmount = 200 }
        };
        _appointmentRepoMock.Setup(r => r.GetTodaysByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnGetAsync(CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Appointments.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnPostPaidAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnPostPaidAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostPaidAsync_WhenMarkPaidSucceeds_SetsSuccessMessage()
    {
        // Arrange
        var appointment = new Appointment { AppointId = 1, AppointmentStatus = 2 };
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);
        _appointmentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _appointmentRepoMock.Setup(r => r.GetTodaysByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnPostPaidAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("paid");
    }

    [Fact]
    public async Task OnPostPaidAsync_WhenMarkPaidFails_SetsFailureMessage()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);
        _appointmentRepoMock.Setup(r => r.GetTodaysByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnPostPaidAsync(99, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeFalse();
        model.Message.Should().Contain("Failed");
    }

    [Fact]
    public async Task OnPostUnpaidAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnPostUnpaidAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostUnpaidAsync_WhenMarkUnpaidSucceeds_SetsSuccessMessage()
    {
        // Arrange
        var appointment = new Appointment { AppointId = 1, AppointmentStatus = 2 };
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);
        _appointmentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _appointmentRepoMock.Setup(r => r.GetTodaysByDoctorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(2);

        // Act
        var result = await model.OnPostUnpaidAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("unpaid");
    }

    [Fact]
    public void BillModel_Constructor_InitializesProperties()
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
