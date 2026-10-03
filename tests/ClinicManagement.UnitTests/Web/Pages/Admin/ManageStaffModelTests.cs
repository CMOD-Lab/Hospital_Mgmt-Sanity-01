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

public class ManageStaffModelTests
{
    private readonly Mock<IStaffService> _mockStaffService;
    private readonly Mock<ILogger<ManageStaffModel>> _mockLogger;
    private readonly ManageStaffModel _manageStaffModel;
    private readonly TestSession _testSession;

    public ManageStaffModelTests()
    {
        _mockStaffService = new Mock<IStaffService>();
        _mockLogger = new Mock<ILogger<ManageStaffModel>>();
        _manageStaffModel = new ManageStaffModel(_mockStaffService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _manageStaffModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupAdminSession() { _testSession.SetInt32("UserId", 1); _testSession.SetInt32("UserType", 0); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new ManageStaffModel(_mockStaffService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void StaffList_InitiallyEmpty()
    {
        Assert.Empty(_manageStaffModel.StaffList);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _manageStaffModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsAdmin_WithNoSearch_LoadsAllStaff()
    {
        SetupAdminSession();
        var staffList = new List<StaffDto>
        {
            new StaffDto { StaffId = 1, Name = "Alice" },
            new StaffDto { StaffId = 2, Name = "Bob" }
        };
        _mockStaffService.Setup(s => s.GetAllAsync(default)).ReturnsAsync(staffList);

        var result = await _manageStaffModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, ((List<StaffDto>)_manageStaffModel.StaffList).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsAdmin_WithSearch_LoadsFilteredStaff()
    {
        SetupAdminSession();
        var staffList = new List<StaffDto> { new StaffDto { StaffId = 1, Name = "Alice" } };
        _mockStaffService.Setup(s => s.SearchAsync("Alice", default)).ReturnsAsync(staffList);

        var result = await _manageStaffModel.OnGetAsync("Alice");

        Assert.IsType<PageResult>(result);
        Assert.Equal("Alice", _manageStaffModel.SearchQuery);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupAdminSession();
        _mockStaffService.Setup(s => s.GetAllAsync(default)).ThrowsAsync(new Exception("Service error"));

        var result = await _manageStaffModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _manageStaffModel.OnPostDeleteAsync(1);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenLoggedInAsAdmin_DeletesStaffAndReturnsPage()
    {
        SetupAdminSession();
        _mockStaffService.Setup(s => s.DeleteAsync(1, default)).Returns(Task.CompletedTask);
        _mockStaffService.Setup(s => s.GetAllAsync(default)).ReturnsAsync(new List<StaffDto>());

        var result = await _manageStaffModel.OnPostDeleteAsync(1);

        Assert.IsType<PageResult>(result);
        _mockStaffService.Verify(s => s.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenExceptionThrown_ReturnsPage()
    {
        SetupAdminSession();
        _mockStaffService.Setup(s => s.DeleteAsync(99, default)).ThrowsAsync(new Exception("Not found"));
        _mockStaffService.Setup(s => s.GetAllAsync(default)).ReturnsAsync(new List<StaffDto>());

        var result = await _manageStaffModel.OnPostDeleteAsync(99);

        Assert.IsType<PageResult>(result);
    }
}
