using System;
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

public class NotificationsModelTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<ILogger<NotificationsModel>> _mockLogger;
    private readonly NotificationsModel _notificationsModel;
    private readonly TestSession _testSession;

    public NotificationsModelTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockLogger = new Mock<ILogger<NotificationsModel>>();
        _notificationsModel = new NotificationsModel(_mockAppointmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _notificationsModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession(int patientId = 7) { _testSession.SetInt32("UserId", patientId); _testSession.SetInt32("UserType", 2); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new NotificationsModel(_mockAppointmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void PendingFeedback_InitiallyNull()
    {
        Assert.Null(_notificationsModel.PendingFeedback);
    }

    [Fact]
    public void CurrentAppointment_InitiallyNull()
    {
        Assert.Null(_notificationsModel.CurrentAppointment);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _notificationsModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsPatient_LoadsNotifications()
    {
        SetupPatientSession(7);
        var pendingFeedback = new AppointmentDto { AppointmentId = 3, FeedbackGiven = false };
        var currentAppointment = new AppointmentDto { AppointmentId = 5 };

        _mockAppointmentService.Setup(s => s.GetPendingFeedbackByPatientIdAsync(7, default)).ReturnsAsync(pendingFeedback);
        _mockAppointmentService.Setup(s => s.GetCurrentByPatientIdAsync(7, default)).ReturnsAsync(currentAppointment);

        var result = await _notificationsModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.NotNull(_notificationsModel.PendingFeedback);
        Assert.NotNull(_notificationsModel.CurrentAppointment);
        Assert.Equal(3, _notificationsModel.PendingFeedback!.AppointmentId);
        Assert.Equal(5, _notificationsModel.CurrentAppointment!.AppointmentId);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonPatient_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 1); // Doctor

        var result = await _notificationsModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(7);
        _mockAppointmentService.Setup(s => s.GetPendingFeedbackByPatientIdAsync(7, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _notificationsModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _notificationsModel.OnPostAsync(1);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenLoggedInAsPatient_SubmitsFeedbackAndRedirects()
    {
        SetupPatientSession(7);
        _mockAppointmentService.Setup(s => s.SubmitFeedbackAsync(3, default)).Returns(Task.CompletedTask);

        var result = await _notificationsModel.OnPostAsync(3);

        Assert.IsType<RedirectToPageResult>(result);
        _mockAppointmentService.Verify(s => s.SubmitFeedbackAsync(3, default), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_RedirectsToPage()
    {
        SetupPatientSession(7);
        _mockAppointmentService.Setup(s => s.SubmitFeedbackAsync(99, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _notificationsModel.OnPostAsync(99);
        Assert.IsType<RedirectToPageResult>(result);
    }
}
