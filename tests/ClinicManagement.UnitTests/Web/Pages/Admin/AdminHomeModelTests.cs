using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

/// <summary>
/// Unit tests for Admin HomeModel page.
/// </summary>
public class AdminHomeModelTests
{
    private readonly Mock<IAdminService> _mockAdminService;

    public AdminHomeModelTests()
    {
        _mockAdminService = new Mock<IAdminService>();
    }

    private ClinicManagement.Web.Pages.Admin.HomeModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Admin.HomeModel(_mockAdminService.Object);
        SetupSession(sut, sessionUserId);
        return sut;
    }

    private static void SetupSession(PageModel model, int? userId)
    {
        var mockSession = new Mock<ISession>();
        if (userId.HasValue)
        {
            var bytes = BitConverter.GetBytes(userId.Value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            byte[] outBytes = bytes;
            mockSession.Setup(s => s.TryGetValue("UserId", out outBytes!)).Returns(true);
        }
        else
        {
            byte[]? nullBytes = null;
            mockSession.Setup(s => s.TryGetValue("UserId", out nullBytes!)).Returns(false);
        }
        mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
        var httpContext = new DefaultHttpContext();
        httpContext.Session = mockSession.Object;
        model.PageContext = new PageContext { HttpContext = httpContext };
    }

    [Fact]
    public void Constructor_WithValidService_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.Admin.HomeModel(_mockAdminService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_DashboardIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.Dashboard);
    }

    [Fact]
    public async Task OnGetAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnGetAsync(CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WithValidSession_ReturnsPage()
    {
        var sut = CreateSut(3);
        var dashboard = new AdminDashboardData(
            10, 50, 25000.0,
            new List<DepartmentSummary>(),
            new List<AppointmentSummary>());
        _mockAdminService
            .Setup(s => s.GetDashboardDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dashboard);

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithDashboardData_SetsDashboard()
    {
        var sut = CreateSut(3);
        var dashboard = new AdminDashboardData(
            10, 50, 25000.0,
            new List<DepartmentSummary> { new DepartmentSummary(1, "Cardiology", "Heart", 3) },
            new List<AppointmentSummary>());
        _mockAdminService
            .Setup(s => s.GetDashboardDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dashboard);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.NotNull(sut.Dashboard);
        Assert.Equal(10, sut.Dashboard!.TotalDoctors);
        Assert.Equal(50, sut.Dashboard.TotalPatients);
        Assert.Equal(25000.0, sut.Dashboard.TotalIncome);
    }

    [Fact]
    public async Task OnGetAsync_CallsGetDashboardDataAsync()
    {
        var sut = CreateSut(3);
        _mockAdminService
            .Setup(s => s.GetDashboardDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdminDashboardData(0, 0, 0, new List<DepartmentSummary>(), new List<AppointmentSummary>()));

        await sut.OnGetAsync(CancellationToken.None);

        _mockAdminService.Verify(s => s.GetDashboardDataAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
