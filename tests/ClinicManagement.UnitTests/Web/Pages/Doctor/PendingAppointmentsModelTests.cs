using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

/// <summary>
/// Unit tests for PendingAppointmentsModel page.
/// </summary>
public class PendingAppointmentsModelTests
{
    private readonly Mock<IDoctorService> _mockDoctorService;

    public PendingAppointmentsModelTests()
    {
        _mockDoctorService = new Mock<IDoctorService>();
    }

    private ClinicManagement.Web.Pages.Doctor.PendingAppointmentsModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Doctor.PendingAppointmentsModel(_mockDoctorService.Object);
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

    // ─── Constructor Tests ────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidService_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.Doctor.PendingAppointmentsModel(_mockDoctorService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_AppointmentsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Appointments);
    }

    [Fact]
    public void Constructor_InitialState_MessageIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.Message);
    }

    // ─── OnGetAsync Tests ─────────────────────────────────────────────────────

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
            .Setup(s => s.GetPendingAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PendingAppointmentItem>());

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithAppointments_SetsAppointments()
    {
        var sut = CreateSut(2);
        var appointments = new List<PendingAppointmentItem>
        {
            new PendingAppointmentItem(1, "John Doe", DateTime.Today, "Pending"),
            new PendingAppointmentItem(2, "Jane Smith", DateTime.Today, "Pending")
        };
        _mockDoctorService
            .Setup(s => s.GetPendingAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Equal(2, sut.Appointments.Count());
    }

    // ─── OnPostApproveAsync Tests ─────────────────────────────────────────────

    [Fact]
    public async Task OnPostApproveAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostApproveAsync(1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostApproveAsync_SuccessfulApproval_SetsSuccessMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.ApproveAppointmentAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetPendingAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PendingAppointmentItem>());

        var result = await sut.OnPostApproveAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Appointment approved successfully.", sut.Message);
    }

    [Fact]
    public async Task OnPostApproveAsync_FailedApproval_SetsFailureMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.ApproveAppointmentAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDoctorService
            .Setup(s => s.GetPendingAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PendingAppointmentItem>());

        var result = await sut.OnPostApproveAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to approve appointment.", sut.Message);
    }

    // ─── OnPostRejectAsync Tests ──────────────────────────────────────────────

    [Fact]
    public async Task OnPostRejectAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostRejectAsync(1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostRejectAsync_SuccessfulRejection_SetsSuccessMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.RejectAppointmentAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetPendingAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PendingAppointmentItem>());

        var result = await sut.OnPostRejectAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Appointment rejected.", sut.Message);
    }

    [Fact]
    public async Task OnPostRejectAsync_FailedRejection_SetsFailureMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.RejectAppointmentAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDoctorService
            .Setup(s => s.GetPendingAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PendingAppointmentItem>());

        var result = await sut.OnPostRejectAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to reject appointment.", sut.Message);
    }

    [Fact]
    public async Task OnPostApproveAsync_ReloadsAppointmentsAfterAction()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.ApproveAppointmentAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetPendingAppointmentsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PendingAppointmentItem>());

        await sut.OnPostApproveAsync(5, CancellationToken.None);

        _mockDoctorService.Verify(s => s.GetPendingAppointmentsAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }
}
