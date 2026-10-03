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

public class ManageDoctorsModelTests
{
    private readonly Mock<IDoctorService> _mockDoctorService;
    private readonly Mock<ILogger<ManageDoctorsModel>> _mockLogger;
    private readonly ManageDoctorsModel _manageDoctorsModel;
    private readonly TestSession _testSession;

    public ManageDoctorsModelTests()
    {
        _mockDoctorService = new Mock<IDoctorService>();
        _mockLogger = new Mock<ILogger<ManageDoctorsModel>>();
        _manageDoctorsModel = new ManageDoctorsModel(_mockDoctorService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _manageDoctorsModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupAdminSession() { _testSession.SetInt32("UserId", 1); _testSession.SetInt32("UserType", 0); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new ManageDoctorsModel(_mockDoctorService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Doctors_InitiallyEmpty()
    {
        Assert.Empty(_manageDoctorsModel.Doctors);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _manageDoctorsModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsAdmin_WithNoSearch_LoadsAllDoctors()
    {
        SetupAdminSession();
        var doctors = new List<DoctorDto>
        {
            new DoctorDto { DoctorId = 1, Name = "Dr. Smith" },
            new DoctorDto { DoctorId = 2, Name = "Dr. Jones" }
        };
        _mockDoctorService.Setup(s => s.GetAllAsync(default)).ReturnsAsync(doctors);

        var result = await _manageDoctorsModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, ((List<DoctorDto>)_manageDoctorsModel.Doctors).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsAdmin_WithSearch_LoadsFilteredDoctors()
    {
        SetupAdminSession();
        var doctors = new List<DoctorDto> { new DoctorDto { DoctorId = 1, Name = "Dr. Smith" } };
        _mockDoctorService.Setup(s => s.SearchAsync("Smith", default)).ReturnsAsync(doctors);

        var result = await _manageDoctorsModel.OnGetAsync("Smith");

        Assert.IsType<PageResult>(result);
        Assert.Equal("Smith", _manageDoctorsModel.SearchQuery);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithEmptyDoctors()
    {
        SetupAdminSession();
        _mockDoctorService.Setup(s => s.GetAllAsync(default)).ThrowsAsync(new Exception("Service error"));

        var result = await _manageDoctorsModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _manageDoctorsModel.OnPostDeleteAsync(1);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenLoggedInAsAdmin_DeletesDoctorAndReturnsPage()
    {
        SetupAdminSession();
        _mockDoctorService.Setup(s => s.DeleteAsync(1, default)).Returns(Task.CompletedTask);
        _mockDoctorService.Setup(s => s.GetAllAsync(default)).ReturnsAsync(new List<DoctorDto>());

        var result = await _manageDoctorsModel.OnPostDeleteAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Doctor removed successfully.", _manageDoctorsModel.SuccessMessage);
        _mockDoctorService.Verify(s => s.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenExceptionThrown_ReturnsPageWithoutSuccessMessage()
    {
        SetupAdminSession();
        _mockDoctorService.Setup(s => s.DeleteAsync(99, default)).ThrowsAsync(new Exception("Not found"));
        _mockDoctorService.Setup(s => s.GetAllAsync(default)).ReturnsAsync(new List<DoctorDto>());

        var result = await _manageDoctorsModel.OnPostDeleteAsync(99);

        Assert.IsType<PageResult>(result);
        Assert.Null(_manageDoctorsModel.SuccessMessage);
    }
}
