using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Enums;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

public class PatientHomeModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly Mock<ILogger<HomeModel>> _mockLogger;
    private readonly HomeModel _homeModel;
    private readonly TestSession _testSession;
    private readonly Mock<HttpContext> _mockHttpContext;

    public PatientHomeModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
        _mockLogger = new Mock<ILogger<HomeModel>>();
        _homeModel = new HomeModel(_mockPatientService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _homeModel.PageContext = new PageContext { HttpContext = _mockHttpContext.Object };
    }

    private void SetupPatientSession(int patientId = 7) { _testSession.SetInt32("UserId", patientId); _testSession.SetInt32("UserType", 2); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new HomeModel(_mockPatientService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void PatientInfo_InitiallyNull()
    {
        Assert.Null(_homeModel.PatientInfo);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _homeModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonPatient_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 1); // Doctor

        var result = await _homeModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsPatient_LoadsPatientInfo()
    {
        SetupPatientSession(7);
        var patientInfo = new PatientDto { PatientId = 7, Name = "John Patient", Email = "john@patient.com" };
        _mockPatientService.Setup(s => s.GetByIdAsync(7, default)).ReturnsAsync(patientInfo);

        var result = await _homeModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.NotNull(_homeModel.PatientInfo);
        Assert.Equal("John Patient", _homeModel.PatientInfo!.Name);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithNullPatientInfo()
    {
        SetupPatientSession(7);
        _mockPatientService.Setup(s => s.GetByIdAsync(7, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _homeModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Null(_homeModel.PatientInfo);
    }
}

public class CurrentAppointmentModelTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<ILogger<CurrentAppointmentModel>> _mockLogger;
    private readonly CurrentAppointmentModel _currentAppointmentModel;
    private readonly TestSession _testSession;
    private readonly Mock<HttpContext> _mockHttpContext;

    public CurrentAppointmentModelTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockLogger = new Mock<ILogger<CurrentAppointmentModel>>();
        _currentAppointmentModel = new CurrentAppointmentModel(_mockAppointmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _currentAppointmentModel.PageContext = new PageContext { HttpContext = _mockHttpContext.Object };
    }

    private void SetupPatientSession(int patientId = 7) { _testSession.SetInt32("UserId", patientId); _testSession.SetInt32("UserType", 2); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new CurrentAppointmentModel(_mockAppointmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void CurrentAppointment_InitiallyNull()
    {
        Assert.Null(_currentAppointmentModel.CurrentAppointment);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _currentAppointmentModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsPatient_LoadsCurrentAppointment()
    {
        SetupPatientSession(7);
        var appointment = new AppointmentDto { AppointmentId = 5, PatientId = 7, DoctorName = "Dr. Smith", Status = AppointmentStatus.Approved };
        _mockAppointmentService.Setup(s => s.GetCurrentByPatientIdAsync(7, default)).ReturnsAsync(appointment);

        var result = await _currentAppointmentModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.NotNull(_currentAppointmentModel.CurrentAppointment);
        Assert.Equal(5, _currentAppointmentModel.CurrentAppointment!.AppointmentId);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoCurrentAppointment_ReturnsPageWithNullAppointment()
    {
        SetupPatientSession(7);
        _mockAppointmentService.Setup(s => s.GetCurrentByPatientIdAsync(7, default)).ReturnsAsync((AppointmentDto?)null);

        var result = await _currentAppointmentModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Null(_currentAppointmentModel.CurrentAppointment);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonPatient_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 1); // Doctor

        var result = await _currentAppointmentModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithNullAppointment()
    {
        SetupPatientSession(7);
        _mockAppointmentService.Setup(s => s.GetCurrentByPatientIdAsync(7, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _currentAppointmentModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Null(_currentAppointmentModel.CurrentAppointment);
    }
}

public class TreatmentHistoryModelTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<ILogger<TreatmentHistoryModel>> _mockLogger;
    private readonly TreatmentHistoryModel _treatmentHistoryModel;
    private readonly TestSession _testSession;
    private readonly Mock<HttpContext> _mockHttpContext;

    public TreatmentHistoryModelTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockLogger = new Mock<ILogger<TreatmentHistoryModel>>();
        _treatmentHistoryModel = new TreatmentHistoryModel(_mockAppointmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _treatmentHistoryModel.PageContext = new PageContext { HttpContext = _mockHttpContext.Object };
    }

    private void SetupPatientSession(int patientId = 7) { _testSession.SetInt32("UserId", patientId); _testSession.SetInt32("UserType", 2); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new TreatmentHistoryModel(_mockAppointmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Appointments_InitiallyEmpty()
    {
        Assert.Empty(_treatmentHistoryModel.Appointments);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _treatmentHistoryModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsPatient_LoadsTreatmentHistory()
    {
        SetupPatientSession(7);
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { AppointmentId = 1, Disease = "Flu", Prescription = "Rest" },
            new AppointmentDto { AppointmentId = 2, Disease = "Cold", Prescription = "Vitamins" }
        };
        _mockAppointmentService.Setup(s => s.GetByPatientIdAsync(7, default)).ReturnsAsync(appointments);

        var result = await _treatmentHistoryModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, ((List<AppointmentDto>)_treatmentHistoryModel.Appointments).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonPatient_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 0); // Admin

        var result = await _treatmentHistoryModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(7);
        _mockAppointmentService.Setup(s => s.GetByPatientIdAsync(7, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _treatmentHistoryModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
    }
}
