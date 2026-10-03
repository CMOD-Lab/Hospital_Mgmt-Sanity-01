using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Enums;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Patient;

public class BillsHistoryModelTests
{
    private readonly Mock<IBillService> _mockBillService;
    private readonly Mock<ILogger<BillsHistoryModel>> _mockLogger;
    private readonly BillsHistoryModel _billsHistoryModel;
    private readonly TestSession _testSession;

    public BillsHistoryModelTests()
    {
        _mockBillService = new Mock<IBillService>();
        _mockLogger = new Mock<ILogger<BillsHistoryModel>>();
        _billsHistoryModel = new BillsHistoryModel(_mockBillService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _billsHistoryModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupPatientSession(int patientId = 7) { _testSession.SetInt32("UserId", patientId); _testSession.SetInt32("UserType", 2); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new BillsHistoryModel(_mockBillService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Bills_InitiallyEmpty()
    {
        Assert.Empty(_billsHistoryModel.Bills);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _billsHistoryModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsPatient_LoadsBills()
    {
        SetupPatientSession(7);
        var bills = new List<BillDto>
        {
            new BillDto { BillId = 1, Amount = 500m, Status = BillStatus.Paid },
            new BillDto { BillId = 2, Amount = 300m, Status = BillStatus.Unpaid }
        };
        _mockBillService.Setup(s => s.GetByPatientIdAsync(7, default)).ReturnsAsync(bills);

        var result = await _billsHistoryModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, ((List<BillDto>)_billsHistoryModel.Bills).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsNonPatient_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 1); // Doctor

        var result = await _billsHistoryModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupPatientSession(7);
        _mockBillService.Setup(s => s.GetByPatientIdAsync(7, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _billsHistoryModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenNoBills_ReturnsPageWithEmptyBills()
    {
        SetupPatientSession(7);
        _mockBillService.Setup(s => s.GetByPatientIdAsync(7, default)).ReturnsAsync(new List<BillDto>());

        var result = await _billsHistoryModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Empty(_billsHistoryModel.Bills);
    }
}
