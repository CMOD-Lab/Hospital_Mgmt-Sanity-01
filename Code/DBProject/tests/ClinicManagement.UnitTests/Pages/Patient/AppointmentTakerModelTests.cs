using ClinicManagement.Application.DTOs;
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
/// Unit tests for AppointmentTakerModel page model.
/// </summary>
public class AppointmentTakerModelTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IDoctorRepository> _doctorRepoMock;
    private readonly Mock<IDepartmentRepository> _departmentRepoMock;
    private readonly Mock<ILogger<AppointmentService>> _loggerMock;
    private readonly AppointmentService _appointmentService;

    public AppointmentTakerModelTests()
    {
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _doctorRepoMock = new Mock<IDoctorRepository>();
        _departmentRepoMock = new Mock<IDepartmentRepository>();
        _loggerMock = new Mock<ILogger<AppointmentService>>();
        _appointmentService = new AppointmentService(
            _appointmentRepoMock.Object,
            _doctorRepoMock.Object,
            _departmentRepoMock.Object,
            _loggerMock.Object);
    }

    private AppointmentTakerModel CreatePageModel(int? sessionUserId = 1)
    {
        var model = new AppointmentTakerModel(_appointmentService);
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
        var result = await model.OnGetAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnGetAsync_WhenUserLoggedIn_ReturnsFreeSlots()
    {
        // Arrange
        var slots = new List<Appointment>
        {
            new() { AppointId = 1, AppointmentStatus = 1, Date = DateTime.Now.AddDays(1) },
            new() { AppointId = 2, AppointmentStatus = 1, Date = DateTime.Now.AddDays(2) }
        };
        _appointmentRepoMock.Setup(r => r.GetFreeSlotsByDoctorAsync(5, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(slots);

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnGetAsync(5, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.DoctorId.Should().Be(5);
        model.FreeSlots.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserNotLoggedIn_RedirectsToSignUp()
    {
        // Arrange
        var model = CreatePageModel(null);

        // Act
        var result = await model.OnPostAsync(1, 5, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/SignUp");
    }

    [Fact]
    public async Task OnPostAsync_WhenBookingSucceeds_SetsSuccessMessage()
    {
        // Arrange
        var slot = new Appointment { AppointId = 1, AppointmentStatus = 1 };
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(slot);
        _appointmentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _appointmentRepoMock.Setup(r => r.GetFreeSlotsByDoctorAsync(5, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnPostAsync(1, 5, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeTrue();
        model.Message.Should().Contain("successfully");
    }

    [Fact]
    public async Task OnPostAsync_WhenBookingFails_SetsFailureMessage()
    {
        // Arrange
        _appointmentRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);
        _appointmentRepoMock.Setup(r => r.GetFreeSlotsByDoctorAsync(5, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());

        var model = CreatePageModel(1);

        // Act
        var result = await model.OnPostAsync(99, 5, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Success.Should().BeFalse();
        model.Message.Should().Contain("Failed");
    }

    [Fact]
    public void AppointmentTakerModel_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var model = CreatePageModel(1);

        // Assert
        model.FreeSlots.Should().NotBeNull();
        model.FreeSlots.Should().BeEmpty();
        model.Message.Should().BeEmpty();
        model.Success.Should().BeFalse();
        model.DoctorId.Should().Be(0);
    }
}
