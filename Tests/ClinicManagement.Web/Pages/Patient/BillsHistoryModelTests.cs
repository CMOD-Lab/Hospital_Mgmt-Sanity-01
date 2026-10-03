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
/// Comprehensive unit tests for BillsHistoryModel page.
/// </summary>
public class BillsHistoryModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;

    public BillsHistoryModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Patient.BillsHistoryModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.BillsHistoryModel(_mockPatientService.Object);
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
        var model = new ClinicManagement.Web.Pages.Patient.BillsHistoryModel(_mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_BillsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Bills);
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
            .Setup(s => s.GetBillHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BillHistoryItem>());

        var result = await sut.OnGetAsync(CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WithBills_SetsBills()
    {
        var sut = CreateSut(1);
        var bills = new List<BillHistoryItem>
        {
            new BillHistoryItem(1, DateTime.Now, "Dr. Smith", 500.0, "Paid", "Flu"),
            new BillHistoryItem(2, DateTime.Now.AddDays(-10), "Dr. Jones", 300.0, "Unpaid", "Cold")
        };
        _mockPatientService
            .Setup(s => s.GetBillHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bills);

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Equal(2, sut.Bills.Count());
    }

    [Fact]
    public async Task OnGetAsync_EmptyBills_BillsIsEmpty()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetBillHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<BillHistoryItem>());

        await sut.OnGetAsync(CancellationToken.None);

        Assert.Empty(sut.Bills);
    }

    [Fact]
    public async Task OnGetAsync_CallsServiceWithCorrectUserId()
    {
        var sut = CreateSut(20);
        _mockPatientService
            .Setup(s => s.GetBillHistoryAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BillHistoryItem>());

        await sut.OnGetAsync(CancellationToken.None);

        _mockPatientService.Verify(s => s.GetBillHistoryAsync(20, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_BillItemHasCorrectData()
    {
        var sut = CreateSut(1);
        var date = new DateTime(2024, 3, 10);
        var bills = new List<BillHistoryItem>
        {
            new BillHistoryItem(5, date, "Dr. Wilson", 750.0, "Paid", "Diabetes")
        };
        _mockPatientService
            .Setup(s => s.GetBillHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bills);

        await sut.OnGetAsync(CancellationToken.None);

        var bill = sut.Bills.First();
        Assert.Equal(5, bill.AppointId);
        Assert.Equal("Dr. Wilson", bill.DoctorName);
        Assert.Equal(750.0, bill.BillAmount);
        Assert.Equal("Paid", bill.BillStatus);
    }
}
