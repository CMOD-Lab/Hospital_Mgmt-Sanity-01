using System;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

public class AddStaffModelTests
{
    private readonly Mock<IStaffService> _mockStaffService;
    private readonly Mock<ILogger<AddStaffModel>> _mockLogger;
    private readonly AddStaffModel _addStaffModel;
    private readonly TestSession _testSession;

    public AddStaffModelTests()
    {
        _mockStaffService = new Mock<IStaffService>();
        _mockLogger = new Mock<ILogger<AddStaffModel>>();
        _addStaffModel = new AddStaffModel(_mockStaffService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _addStaffModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupAdminSession() { _testSession.SetInt32("UserId", 1); _testSession.SetInt32("UserType", 0); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new AddStaffModel(_mockStaffService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitializesInputProperty()
    {
        Assert.NotNull(_addStaffModel.Input);
    }

    [Fact]
    public void OnGet_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = _addStaffModel.OnGet();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public void OnGet_WhenLoggedInAsAdmin_ReturnsPage()
    {
        SetupAdminSession();
        var result = _addStaffModel.OnGet();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public void OnGet_WhenLoggedInAsNonAdmin_RedirectsToLogin()
    {
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 2); // Patient
        var result = _addStaffModel.OnGet();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _addStaffModel.OnPostAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
    {
        SetupAdminSession();
        _addStaffModel.ModelState.AddModelError("Name", "Name is required");
        var result = await _addStaffModel.OnPostAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenSuccessful_RedirectsToManageStaff()
    {
        SetupAdminSession();
        _addStaffModel.Input = new AddStaffModel.AddStaffInputModel
        {
            Name = "John Staff", Phone = "1234567890", Address = "Staff Quarters",
            BirthDate = new DateTime(1985, 5, 15), Gender = "M",
            Designation = "Nurse", Qualification = "BSc Nursing", Salary = 30000m
        };

        _mockStaffService.Setup(s => s.CreateAsync(It.IsAny<StaffCreateDto>(), default))
            .ReturnsAsync(new StaffDto { StaffId = 1, Name = "John Staff" });

        var result = await _addStaffModel.OnPostAsync();

        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/ManageStaff", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_ReturnsPageWithErrorMessage()
    {
        SetupAdminSession();
        _addStaffModel.Input = new AddStaffModel.AddStaffInputModel
        {
            Name = "Jane Staff", Phone = "9876543210", Address = "Staff Block",
            BirthDate = new DateTime(1990, 8, 20), Gender = "F",
            Designation = "Technician", Qualification = "Diploma", Salary = 25000m
        };

        _mockStaffService.Setup(s => s.CreateAsync(It.IsAny<StaffCreateDto>(), default))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _addStaffModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("An error occurred. Please try again.", _addStaffModel.ErrorMessage);
    }

    [Fact]
    public void AddStaffInputModel_DefaultValues()
    {
        var input = new AddStaffModel.AddStaffInputModel();
        Assert.Equal(string.Empty, input.Name);
        Assert.Equal(string.Empty, input.Phone);
        Assert.Equal(string.Empty, input.Address);
        Assert.Equal(string.Empty, input.Gender);
        Assert.Equal(string.Empty, input.Designation);
        Assert.Equal(string.Empty, input.Qualification);
        Assert.Equal(0m, input.Salary);
    }
}
