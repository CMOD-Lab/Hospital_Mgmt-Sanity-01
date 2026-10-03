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

public class DashboardModelTests
{
    private readonly Mock<IAdminService> _mockAdminService;
    private readonly Mock<ILogger<DashboardModel>> _mockLogger;
    private readonly DashboardModel _dashboardModel;
    private readonly TestSession _testSession;

    public DashboardModelTests()
    {
        _mockAdminService = new Mock<IAdminService>();
        _mockLogger = new Mock<ILogger<DashboardModel>>();
        _dashboardModel = new DashboardModel(_mockAdminService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _dashboardModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new DashboardModel(_mockAdminService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        // No session values set
        var result = await _dashboardModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonAdmin_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 1); // Doctor

        var result = await _dashboardModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsAdmin_LoadsDashboardData()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 0); // Admin

        var dashboardData = new AdminDashboardDto
        {
            TotalDoctors = 10, TotalPatients = 50, TotalIncome = 5000m,
            Departments = new List<DepartmentDto>(),
            RecentAppointments = new List<AppointmentDto>()
        };

        _mockAdminService.Setup(s => s.GetDashboardDataAsync(default)).ReturnsAsync(dashboardData);

        var result = await _dashboardModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.NotNull(_dashboardModel.Dashboard);
        Assert.Equal(10, _dashboardModel.Dashboard!.TotalDoctors);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPageWithNullDashboard()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 0);

        _mockAdminService.Setup(s => s.GetDashboardDataAsync(default))
            .ThrowsAsync(new Exception("Service error"));

        var result = await _dashboardModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Null(_dashboardModel.Dashboard);
    }

    [Fact]
    public void Dashboard_InitiallyNull()
    {
        Assert.Null(_dashboardModel.Dashboard);
    }
}
