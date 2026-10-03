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

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

/// <summary>
/// Unit tests for NotificationsModel page.
/// </summary>
public class NotificationsModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;

    public NotificationsModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Patient.NotificationsModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.NotificationsModel(_mockPatientService.Object);
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
        var model = new ClinicManagement.Web.Pages.Patient.NotificationsModel(_mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_NotificationsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Notifications);
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
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetNotificationsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationData>());

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithValidSession_SetsNotifications()
    {
        var sut = CreateSut(1);
        var notifications = new List<NotificationData>
        {
            new NotificationData("Dr. Smith", "10:00 AM"),
            new NotificationData("Dr. Jones", "2:00 PM")
        };
        _mockPatientService
            .Setup(s => s.GetNotificationsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Equal(2, sut.Notifications.Count());
    }

    [Fact]
    public async Task OnGetAsync_ServiceReturnsEmpty_NotificationsIsEmpty()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetNotificationsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<NotificationData>());

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Empty(sut.Notifications);
    }

    [Fact]
    public async Task OnGetAsync_CallsServiceWithCorrectUserId()
    {
        var sut = CreateSut(42);
        _mockPatientService
            .Setup(s => s.GetNotificationsAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationData>());

        await sut.OnGetAsync(CancellationToken.None);

        _mockPatientService.Verify(s => s.GetNotificationsAsync(42, It.IsAny<CancellationToken>()), Times.Once);
    }
}
