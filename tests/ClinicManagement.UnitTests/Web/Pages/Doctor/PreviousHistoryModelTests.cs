using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

public class PreviousHistoryModelTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<ILogger<PreviousHistoryModel>> _mockLogger;
    private readonly PreviousHistoryModel _previousHistoryModel;
    private readonly TestSession _testSession;

    public PreviousHistoryModelTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockLogger = new Mock<ILogger<PreviousHistoryModel>>();
        _previousHistoryModel = new PreviousHistoryModel(_mockAppointmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _previousHistoryModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int doctorId = 3) { _testSession.SetInt32("UserId", doctorId); _testSession.SetInt32("UserType", 1); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new PreviousHistoryModel(_mockAppointmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Appointments_InitiallyEmpty()
    {
        Assert.Empty(_previousHistoryModel.Appointments);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _previousHistoryModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsDoctor_LoadsPreviousAppointments()
    {
        SetupDoctorSession(3);
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { AppointmentId = 1, PatientName = "Alice" },
            new AppointmentDto { AppointmentId = 2, PatientName = "Bob" },
            new AppointmentDto { AppointmentId = 3, PatientName = "Charlie" }
        };
        _mockAppointmentService.Setup(s => s.GetByDoctorIdAsync(3, default)).ReturnsAsync(appointments);

        var result = await _previousHistoryModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(3, ((List<AppointmentDto>)_previousHistoryModel.Appointments).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonDoctor_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 0); // Admin

        var result = await _previousHistoryModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockAppointmentService.Setup(s => s.GetByDoctorIdAsync(3, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _previousHistoryModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
    }
}
