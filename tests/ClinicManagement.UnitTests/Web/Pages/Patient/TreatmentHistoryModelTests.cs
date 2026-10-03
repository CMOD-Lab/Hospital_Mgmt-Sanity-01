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
/// Unit tests for TreatmentHistoryModel page.
/// </summary>
public class TreatmentHistoryModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;

    public TreatmentHistoryModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Patient.TreatmentHistoryModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.TreatmentHistoryModel(_mockPatientService.Object);
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
        var model = new ClinicManagement.Web.Pages.Patient.TreatmentHistoryModel(_mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_HistoryIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.History);
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
            .Setup(s => s.GetTreatmentHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TreatmentHistoryItem>());

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithHistory_SetsHistory()
    {
        var sut = CreateSut(1);
        var history = new List<TreatmentHistoryItem>
        {
            new TreatmentHistoryItem(1, DateTime.Now, "Dr. Smith", "Flu", "Recovered", "Paracetamol"),
            new TreatmentHistoryItem(2, DateTime.Now.AddDays(-30), "Dr. Jones", "Cold", "Improving", "Vitamin C")
        };
        _mockPatientService
            .Setup(s => s.GetTreatmentHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(history);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Equal(2, sut.History.Count());
    }

    [Fact]
    public async Task OnGetAsync_EmptyHistory_HistoryIsEmpty()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetTreatmentHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<TreatmentHistoryItem>());

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Empty(sut.History);
    }

    [Fact]
    public async Task OnGetAsync_CallsServiceWithCorrectUserId()
    {
        var sut = CreateSut(15);
        _mockPatientService
            .Setup(s => s.GetTreatmentHistoryAsync(15, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TreatmentHistoryItem>());

        await sut.OnGetAsync(CancellationToken.None);

        _mockPatientService.Verify(s => s.GetTreatmentHistoryAsync(15, It.IsAny<CancellationToken>()), Times.Once);
    }
}
