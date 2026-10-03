using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Enums;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

public class DoctorHomeModelTests
{
    private readonly Mock<IDoctorService> _mockDoctorService;
    private readonly Mock<ILogger<HomeModel>> _mockLogger;
    private readonly HomeModel _homeModel;
    private readonly TestSession _testSession;

    public DoctorHomeModelTests()
    {
        _mockDoctorService = new Mock<IDoctorService>();
        _mockLogger = new Mock<ILogger<HomeModel>>();
        _homeModel = new HomeModel(_mockDoctorService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _homeModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int doctorId = 5) { _testSession.SetInt32("UserId", doctorId); _testSession.SetInt32("UserType", 1); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new HomeModel(_mockDoctorService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void DoctorInfo_InitiallyNull()
    {
        Assert.Null(_homeModel.DoctorInfo);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _homeModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonDoctor_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 2); // Patient

        var result = await _homeModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsDoctor_LoadsDoctorInfo()
    {
        SetupDoctorSession(5);
        var doctorInfo = new DoctorDto { DoctorId = 5, Name = "Dr. Smith", Specialization = "Cardiology" };
        _mockDoctorService.Setup(s => s.GetByIdAsync(5, default)).ReturnsAsync(doctorInfo);

        var result = await _homeModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.NotNull(_homeModel.DoctorInfo);
        Assert.Equal("Dr. Smith", _homeModel.DoctorInfo!.Name);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithNullDoctorInfo()
    {
        SetupDoctorSession(5);
        _mockDoctorService.Setup(s => s.GetByIdAsync(5, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _homeModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Null(_homeModel.DoctorInfo);
    }
}

public class PendingAppointmentsModelTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<ILogger<PendingAppointmentsModel>> _mockLogger;
    private readonly PendingAppointmentsModel _pendingAppointmentsModel;
    private readonly TestSession _testSession;

    public PendingAppointmentsModelTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockLogger = new Mock<ILogger<PendingAppointmentsModel>>();
        _pendingAppointmentsModel = new PendingAppointmentsModel(_mockAppointmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _pendingAppointmentsModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int doctorId = 3) { _testSession.SetInt32("UserId", doctorId); _testSession.SetInt32("UserType", 1); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new PendingAppointmentsModel(_mockAppointmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Appointments_InitiallyEmpty()
    {
        Assert.Empty(_pendingAppointmentsModel.Appointments);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _pendingAppointmentsModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsDoctor_LoadsPendingAppointments()
    {
        SetupDoctorSession(3);
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { AppointmentId = 1, PatientName = "Alice", Status = AppointmentStatus.Pending },
            new AppointmentDto { AppointmentId = 2, PatientName = "Bob", Status = AppointmentStatus.Pending }
        };
        _mockAppointmentService.Setup(s => s.GetPendingByDoctorIdAsync(3, default)).ReturnsAsync(appointments);

        var result = await _pendingAppointmentsModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, ((List<AppointmentDto>)_pendingAppointmentsModel.Appointments).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockAppointmentService.Setup(s => s.GetPendingByDoctorIdAsync(3, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _pendingAppointmentsModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _pendingAppointmentsModel.OnPostApproveAsync(1);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenLoggedInAsDoctor_ApprovesAppointment()
    {
        SetupDoctorSession(3);
        _mockAppointmentService.Setup(s => s.ApproveAsync(1, default)).Returns(Task.CompletedTask);
        _mockAppointmentService.Setup(s => s.GetPendingByDoctorIdAsync(3, default)).ReturnsAsync(new List<AppointmentDto>());

        var result = await _pendingAppointmentsModel.OnPostApproveAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Appointment approved successfully.", _pendingAppointmentsModel.SuccessMessage);
        _mockAppointmentService.Verify(s => s.ApproveAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task OnPostApproveAsync_WhenExceptionThrown_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockAppointmentService.Setup(s => s.ApproveAsync(99, default)).ThrowsAsync(new Exception("Not found"));
        _mockAppointmentService.Setup(s => s.GetPendingByDoctorIdAsync(3, default)).ReturnsAsync(new List<AppointmentDto>());

        var result = await _pendingAppointmentsModel.OnPostApproveAsync(99);

        Assert.IsType<PageResult>(result);
        Assert.Null(_pendingAppointmentsModel.SuccessMessage);
    }

    [Fact]
    public async Task OnPostRejectAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _pendingAppointmentsModel.OnPostRejectAsync(1);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostRejectAsync_WhenLoggedInAsDoctor_RejectsAppointment()
    {
        SetupDoctorSession(3);
        _mockAppointmentService.Setup(s => s.DeleteAsync(1, default)).Returns(Task.CompletedTask);
        _mockAppointmentService.Setup(s => s.GetPendingByDoctorIdAsync(3, default)).ReturnsAsync(new List<AppointmentDto>());

        var result = await _pendingAppointmentsModel.OnPostRejectAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Appointment rejected.", _pendingAppointmentsModel.SuccessMessage);
        _mockAppointmentService.Verify(s => s.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task OnPostRejectAsync_WhenExceptionThrown_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockAppointmentService.Setup(s => s.DeleteAsync(99, default)).ThrowsAsync(new Exception("Not found"));
        _mockAppointmentService.Setup(s => s.GetPendingByDoctorIdAsync(3, default)).ReturnsAsync(new List<AppointmentDto>());

        var result = await _pendingAppointmentsModel.OnPostRejectAsync(99);

        Assert.IsType<PageResult>(result);
        Assert.Null(_pendingAppointmentsModel.SuccessMessage);
    }
}
