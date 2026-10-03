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
/// Unit tests for BillModel page.
/// </summary>
public class BillModelTests
{
    private readonly Mock<IDoctorService> _mockDoctorService;

    public BillModelTests()
    {
        _mockDoctorService = new Mock<IDoctorService>();
    }

    private ClinicManagement.Web.Pages.Doctor.BillModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Doctor.BillModel(_mockDoctorService.Object);
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
        var model = new ClinicManagement.Web.Pages.Doctor.BillModel(_mockDoctorService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_BillsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Bills);
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
            .Setup(s => s.GetBillsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BillItem>());

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithBills_SetsBills()
    {
        var sut = CreateSut(2);
        var bills = new List<BillItem>
        {
            new BillItem(1, "John Doe", DateTime.Today, 500.0, "Unpaid"),
            new BillItem(2, "Jane Smith", DateTime.Today, 300.0, "Paid")
        };
        _mockDoctorService
            .Setup(s => s.GetBillsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bills);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Equal(2, sut.Bills.Count());
    }

    // ─── OnPostMarkPaidAsync Tests ────────────────────────────────────────────

    [Fact]
    public async Task OnPostMarkPaidAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostMarkPaidAsync(1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostMarkPaidAsync_SuccessfulMark_SetsSuccessMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.MarkBillPaidAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetBillsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BillItem>());

        var result = await sut.OnPostMarkPaidAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Bill marked as paid.", sut.Message);
    }

    [Fact]
    public async Task OnPostMarkPaidAsync_FailedMark_SetsFailureMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.MarkBillPaidAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDoctorService
            .Setup(s => s.GetBillsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BillItem>());

        var result = await sut.OnPostMarkPaidAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to update bill.", sut.Message);
    }

    // ─── OnPostMarkUnpaidAsync Tests ──────────────────────────────────────────

    [Fact]
    public async Task OnPostMarkUnpaidAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostMarkUnpaidAsync(1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostMarkUnpaidAsync_SuccessfulMark_SetsSuccessMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.MarkBillUnpaidAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetBillsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BillItem>());

        var result = await sut.OnPostMarkUnpaidAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Bill marked as unpaid.", sut.Message);
    }

    [Fact]
    public async Task OnPostMarkUnpaidAsync_FailedMark_SetsFailureMessage()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.MarkBillUnpaidAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDoctorService
            .Setup(s => s.GetBillsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BillItem>());

        var result = await sut.OnPostMarkUnpaidAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to update bill.", sut.Message);
    }

    [Fact]
    public async Task OnPostMarkPaidAsync_ReloadsBillsAfterAction()
    {
        var sut = CreateSut(2);
        _mockDoctorService
            .Setup(s => s.MarkBillPaidAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockDoctorService
            .Setup(s => s.GetBillsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BillItem>());

        await sut.OnPostMarkPaidAsync(5, CancellationToken.None);

        _mockDoctorService.Verify(s => s.GetBillsAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }
}
