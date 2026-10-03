using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

public class ViewDoctorsModelTests
{
    private readonly Mock<IDoctorService> _mockDoctorService;
    private readonly Mock<IDepartmentService> _mockDepartmentService;
    private readonly Mock<ILogger<ViewDoctorsModel>> _mockLogger;
    private readonly ViewDoctorsModel _viewDoctorsModel;
    private readonly TestSession _testSession;

    public ViewDoctorsModelTests()
    {
        _mockDoctorService = new Mock<IDoctorService>();
        _mockDepartmentService = new Mock<IDepartmentService>();
        _mockLogger = new Mock<ILogger<ViewDoctorsModel>>();
        _viewDoctorsModel = new ViewDoctorsModel(_mockDoctorService.Object, _mockDepartmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _viewDoctorsModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };

        _mockDepartmentService.Setup(s => s.GetAllAsync(default))
            .ReturnsAsync(new List<DepartmentDto>
            {
                new DepartmentDto { DepartmentId = 1, DeptName = "Cardiology" },
                new DepartmentDto { DepartmentId = 2, DeptName = "Neurology" }
            });
    }

    private void SetupPatientSession(int patientId = 7) { _testSession.SetInt32("UserId", patientId); _testSession.SetInt32("UserType", 2); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new ViewDoctorsModel(_mockDoctorService.Object, _mockDepartmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Doctors_InitiallyEmpty()
    {
        Assert.Empty(_viewDoctorsModel.Doctors);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _viewDoctorsModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsPatient_WithNoDept_LoadsDepartmentsOnly()
    {
        SetupPatientSession(7);
        var result = await _viewDoctorsModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.NotEmpty(_viewDoctorsModel.DepartmentOptions);
        Assert.Empty(_viewDoctorsModel.Doctors);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsPatient_WithDept_LoadsDoctorsByDepartment()
    {
        SetupPatientSession(7);
        var doctors = new List<DoctorDto>
        {
            new DoctorDto { DoctorId = 1, Name = "Dr. Smith", DepartmentName = "Cardiology" }
        };
        _mockDoctorService.Setup(s => s.GetByDepartmentAsync("Cardiology", default)).ReturnsAsync(doctors);

        var result = await _viewDoctorsModel.OnGetAsync("Cardiology");

        Assert.IsType<PageResult>(result);
        Assert.Equal("Cardiology", _viewDoctorsModel.SelectedDept);
        Assert.Single(_viewDoctorsModel.Doctors);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonPatient_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 1); // Doctor

        var result = await _viewDoctorsModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(7);
        _mockDepartmentService.Setup(s => s.GetAllAsync(default)).ThrowsAsync(new Exception("Service error"));

        var result = await _viewDoctorsModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_DepartmentOptions_ContainCorrectValues()
    {
        SetupPatientSession(7);
        await _viewDoctorsModel.OnGetAsync();

        Assert.Equal(2, _viewDoctorsModel.DepartmentOptions.Count);
        Assert.Contains(_viewDoctorsModel.DepartmentOptions, o => o.Value == "Cardiology");
        Assert.Contains(_viewDoctorsModel.DepartmentOptions, o => o.Value == "Neurology");
    }
}
