using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

/// <summary>
/// Unit tests for FeedbackModel page.
/// </summary>
public class FeedbackModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;

    public FeedbackModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Patient.FeedbackModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.FeedbackModel(_mockPatientService.Object);
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
        var model = new ClinicManagement.Web.Pages.Patient.FeedbackModel(_mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_PendingFeedbackIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.PendingFeedback);
    }

    [Fact]
    public void Constructor_InitialState_MessageIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.Message);
    }

    [Fact]
    public void Constructor_InitialState_IsSuccessIsFalse()
    {
        var sut = CreateSut(1);
        Assert.False(sut.IsSuccess);
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
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetPendingFeedbackAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PendingFeedbackData?)null);

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithPendingFeedback_SetsPendingFeedback()
    {
        var sut = CreateSut(1);
        var feedback = new PendingFeedbackData(5, "Dr. Smith", "10:00 AM");
        _mockPatientService
            .Setup(s => s.GetPendingFeedbackAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.NotNull(sut.PendingFeedback);
        Assert.Equal(5, sut.PendingFeedback!.AppointId);
    }

    // ─── OnPostAsync Tests ────────────────────────────────────────────────────

    [Fact]
    public async Task OnPostAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostAsync(1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_SuccessfulSubmit_SetsSuccessMessage()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.SubmitFeedbackAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await sut.OnPostAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Feedback submitted successfully!", sut.Message);
    }

    [Fact]
    public async Task OnPostAsync_FailedSubmit_SetsFailureMessage()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.SubmitFeedbackAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockPatientService
            .Setup(s => s.GetPendingFeedbackAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PendingFeedbackData?)null);

        var result = await sut.OnPostAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to submit feedback.", sut.Message);
    }

    [Fact]
    public async Task OnPostAsync_FailedSubmit_ReloadsPendingFeedback()
    {
        var sut = CreateSut(1);
        var feedback = new PendingFeedbackData(5, "Dr. Smith", "10:00 AM");
        _mockPatientService
            .Setup(s => s.SubmitFeedbackAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockPatientService
            .Setup(s => s.GetPendingFeedbackAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);

        await sut.OnPostAsync(5, CancellationToken.None);

        Assert.NotNull(sut.PendingFeedback);
        _mockPatientService.Verify(s => s.GetPendingFeedbackAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_SuccessfulSubmit_DoesNotReloadPendingFeedback()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.SubmitFeedbackAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await sut.OnPostAsync(5, CancellationToken.None);

        _mockPatientService.Verify(s => s.GetPendingFeedbackAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
