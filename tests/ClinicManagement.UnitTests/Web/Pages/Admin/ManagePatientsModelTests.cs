using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

public class ManagePatientsModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly Mock<ILogger<ManagePatientsModel>> _mockLogger;
    private readonly ManagePatientsModel _managePatientsModel;
    private readonly TestSession _testSession;

    public ManagePatientsModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
        _mockLogger = new Mock<ILogger<ManagePatientsModel>>();
        _managePatientsModel = new ManagePatientsModel(_mockPatientService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _managePatientsModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupAdminSession() { _testSession.SetInt32("UserId", 1); _testSession.SetInt32("UserType", 0); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new ManagePatientsModel(_mockPatientService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Patients_InitiallyEmpty()
    {
        Assert.Empty(_managePatientsModel.Patients);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _managePatientsModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsAdmin_WithNoSearch_LoadsAllPatients()
    {
        SetupAdminSession();
        var patients = new List<PatientDto>
        {
            new PatientDto { PatientId = 1, Name = "Alice Patient" },
            new PatientDto { PatientId = 2, Name = "Bob Patient" }
        };
        _mockPatientService.Setup(s => s.GetAllAsync(default)).ReturnsAsync(patients);

        var result = await _managePatientsModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, ((List<PatientDto>)_managePatientsModel.Patients).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsAdmin_WithSearch_LoadsFilteredPatients()
    {
        SetupAdminSession();
        var patients = new List<PatientDto> { new PatientDto { PatientId = 1, Name = "Alice Patient" } };
        _mockPatientService.Setup(s => s.SearchAsync("Alice", default)).ReturnsAsync(patients);

        var result = await _managePatientsModel.OnGetAsync("Alice");

        Assert.IsType<PageResult>(result);
        Assert.Equal("Alice", _managePatientsModel.SearchQuery);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonAdmin_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 1); // Doctor

        var result = await _managePatientsModel.OnGetAsync();

        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupAdminSession();
        _mockPatientService.Setup(s => s.GetAllAsync(default)).ThrowsAsync(new Exception("Service error"));

        var result = await _managePatientsModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
    }
}
