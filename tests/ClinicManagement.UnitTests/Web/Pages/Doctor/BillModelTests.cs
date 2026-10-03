using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Doctor;

public class BillModelTests
{
    private readonly Mock<IBillService> _mockBillService;
    private readonly Mock<ILogger<BillModel>> _mockLogger;
    private readonly BillModel _billModel;
    private readonly TestSession _testSession;

    public BillModelTests()
    {
        _mockBillService = new Mock<IBillService>();
        _mockLogger = new Mock<ILogger<BillModel>>();
        _billModel = new BillModel(_mockBillService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _billModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int doctorId = 3) { _testSession.SetInt32("UserId", doctorId); _testSession.SetInt32("UserType", 1); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new BillModel(_mockBillService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Bills_InitiallyEmpty()
    {
        Assert.Empty(_billModel.Bills);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _billModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsDoctor_LoadsBills()
    {
        SetupDoctorSession(3);
        var bills = new List<BillDto>
        {
            new BillDto { BillId = 1, PatientName = "Alice", Amount = 500m },
            new BillDto { BillId = 2, PatientName = "Bob", Amount = 300m }
        };
        _mockBillService.Setup(s => s.GetByDoctorIdAsync(3, default)).ReturnsAsync(bills);

        var result = await _billModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, ((List<BillDto>)_billModel.Bills).Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockBillService.Setup(s => s.GetByDoctorIdAsync(3, default)).ThrowsAsync(new Exception("Service error"));

        var result = await _billModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostMarkPaidAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _billModel.OnPostMarkPaidAsync(1);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostMarkPaidAsync_WhenLoggedInAsDoctor_MarksBillAsPaid()
    {
        SetupDoctorSession(3);
        _mockBillService.Setup(s => s.MarkAsPaidAsync(3, 1, default)).Returns(Task.CompletedTask);
        _mockBillService.Setup(s => s.GetByDoctorIdAsync(3, default)).ReturnsAsync(new List<BillDto>());

        var result = await _billModel.OnPostMarkPaidAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Bill marked as paid.", _billModel.SuccessMessage);
        _mockBillService.Verify(s => s.MarkAsPaidAsync(3, 1, default), Times.Once);
    }

    [Fact]
    public async Task OnPostMarkPaidAsync_WhenExceptionThrown_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockBillService.Setup(s => s.MarkAsPaidAsync(3, 99, default)).ThrowsAsync(new Exception("Not found"));
        _mockBillService.Setup(s => s.GetByDoctorIdAsync(3, default)).ReturnsAsync(new List<BillDto>());

        var result = await _billModel.OnPostMarkPaidAsync(99);

        Assert.IsType<PageResult>(result);
        Assert.Null(_billModel.SuccessMessage);
    }

    [Fact]
    public async Task OnPostMarkUnpaidAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _billModel.OnPostMarkUnpaidAsync(1);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostMarkUnpaidAsync_WhenLoggedInAsDoctor_MarksBillAsUnpaid()
    {
        SetupDoctorSession(3);
        _mockBillService.Setup(s => s.MarkAsUnpaidAsync(3, 1, default)).Returns(Task.CompletedTask);
        _mockBillService.Setup(s => s.GetByDoctorIdAsync(3, default)).ReturnsAsync(new List<BillDto>());

        var result = await _billModel.OnPostMarkUnpaidAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Bill marked as unpaid.", _billModel.SuccessMessage);
        _mockBillService.Verify(s => s.MarkAsUnpaidAsync(3, 1, default), Times.Once);
    }

    [Fact]
    public async Task OnPostMarkUnpaidAsync_WhenExceptionThrown_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockBillService.Setup(s => s.MarkAsUnpaidAsync(3, 99, default)).ThrowsAsync(new Exception("Not found"));
        _mockBillService.Setup(s => s.GetByDoctorIdAsync(3, default)).ReturnsAsync(new List<BillDto>());

        var result = await _billModel.OnPostMarkUnpaidAsync(99);

        Assert.IsType<PageResult>(result);
        Assert.Null(_billModel.SuccessMessage);
    }
}
