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

public class PatientHistoryModelTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<ILogger<PatientHistoryModel>> _mockLogger;
    private readonly PatientHistoryModel _patientHistoryModel;
    private readonly TestSession _testSession;

    public PatientHistoryModelTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockLogger = new Mock<ILogger<PatientHistoryModel>>();
        _patientHistoryModel = new PatientHistoryModel(_mockAppointmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _patientHistoryModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int doctorId = 3) { _testSession.SetInt32("UserId", doctorId); _testSession.SetInt32("UserType", 1); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new PatientHistoryModel(_mockAppointmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Appointments_InitiallyEmpty()
    {
        Assert.Empty(_patientHistoryModel.Appointments);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _patientHistoryModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsDoctor_LoadsTodaysAppointments()
    {
        SetupDoctorSession(3);
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { AppointmentId = 1, PatientName = "Alice", AppointmentDate = DateTime.Today },
            new AppointmentDto { AppointmentId = 2, PatientName = "Bob", AppointmentDate = DateTime.Today }
        };
        _mockAppointmentService.Setup(s => s.GetTodaysByDoctorIdAsync(3, default)).ReturnsAsync(appointments);

        var result = await _patientHistoryModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, ((List<AppointmentDto>)_patientHistoryModel.Appointments).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonDoctor_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 2); // Patient

        var result = await _patientHistoryModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockAppointmentService.Setup(s => s.GetTodaysByDoctorIdAsync(3, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _patientHistoryModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
    }
}
