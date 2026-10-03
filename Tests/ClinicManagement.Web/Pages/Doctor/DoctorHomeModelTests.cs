using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

/// <summary>
/// Comprehensive unit tests for Doctor HomeModel page.
/// </summary>
public class DoctorHomeModelTests
{
    private readonly Mock<IDoctorService> _mockDoctorService;

    public DoctorHomeModelTests()
    {
        _mockDoctorService = new Mock<IDoctorService>();
    }

    private ClinicManagement.Web.Pages.Doctor.HomeModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Doctor.HomeModel(_mockDoctorService.Object);
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
        var model = new ClinicManagement.Web.Pages.Doctor.HomeModel(_mockDoctorService.Object);
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
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.GetDoctorDashboardAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorDashboardData?)null);

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithDashboardData_SetsDashboard()
    {
        var sut = CreateSut(2);
        var dashboard = new DoctorDashboardData(
            2, "Dr. Smith", "1234567890", "123 Main St",
            new DateTime(1980, 1, 1), 'M', "Cardiology",
            500.0, 5000.0, 4.5, 100, "MBBS", "Cardiology", 10);
        _mockDoctorService
            .Setup(s => s.GetDoctorDashboardAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dashboard);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.NotNull(sut.Dashboard);
        Assert.Equal("Dr. Smith", sut.Dashboard!.Name);
        Assert.Equal(100, sut.Dashboard.PatientsTreated);
    }

    [Fact]
    public async Task OnGetAsync_CallsServiceWithCorrectDoctorId()
    {
        var sut = CreateSut(8);
        _mockDoctorService
            .Setup(s => s.GetDoctorDashboardAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorDashboardData?)null);

        await sut.OnGetAsync(CancellationToken.None);

        _mockDoctorService.Verify(s => s.GetDoctorDashboardAsync(8, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_DashboardIsNull_DashboardPropertyIsNull()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.GetDoctorDashboardAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DoctorDashboardData?)null);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Null(sut.Dashboard);
    }

    [Fact]
    public async Task OnGetAsync_DashboardHasCorrectDepartment()
    {
        var sut = CreateSut(2);
        var dashboard = new DoctorDashboardData(
            2, "Dr. Jones", "9876543210", "456 Oak Ave",
            new DateTime(1975, 6, 15), 'F', "Neurology",
            700.0, 6000.0, 4.9, 250, "MD", "Neurology", 20);
        _mockDoctorService
            .Setup(s => s.GetDoctorDashboardAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dashboard);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Equal("Neurology", sut.Dashboard!.DeptName);
        Assert.Equal(700.0, sut.Dashboard.ChargesPerVisit);
    }
}
