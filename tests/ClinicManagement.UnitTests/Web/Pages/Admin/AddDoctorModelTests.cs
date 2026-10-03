using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

public class AddDoctorModelTests
{
    private readonly Mock<IDoctorService> _mockDoctorService;
    private readonly Mock<IDepartmentService> _mockDepartmentService;
    private readonly Mock<ILogger<AddDoctorModel>> _mockLogger;
    private readonly AddDoctorModel _addDoctorModel;
    private readonly TestSession _testSession;

    public AddDoctorModelTests()
    {
        _mockDoctorService = new Mock<IDoctorService>();
        _mockDepartmentService = new Mock<IDepartmentService>();
        _mockLogger = new Mock<ILogger<AddDoctorModel>>();
        _addDoctorModel = new AddDoctorModel(_mockDoctorService.Object, _mockDepartmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _addDoctorModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };

        _mockDepartmentService.Setup(s => s.GetAllAsync(default))
            .ReturnsAsync(new List<DepartmentDto>
            {
                new DepartmentDto { DepartmentId = 1, DeptName = "Cardiology" },
                new DepartmentDto { DepartmentId = 2, DeptName = "Neurology" }
            });
    }

    private void SetupAdminSession() { _testSession.SetInt32("UserId", 1); _testSession.SetInt32("UserType", 0); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new AddDoctorModel(_mockDoctorService.Object, _mockDepartmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitializesInputAndDepartmentOptions()
    {
        Assert.NotNull(_addDoctorModel.Input);
        Assert.NotNull(_addDoctorModel.DepartmentOptions);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _addDoctorModel.OnGetAsync();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsAdmin_LoadsDepartmentsAndReturnsPage()
    {
        SetupAdminSession();
        var result = await _addDoctorModel.OnGetAsync();
        Assert.IsType<PageResult>(result);
        Assert.NotEmpty(_addDoctorModel.DepartmentOptions);
    }

    [Fact]
    public async Task OnPostAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _addDoctorModel.OnPostAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
    {
        SetupAdminSession();
        _addDoctorModel.ModelState.AddModelError("Name", "Name is required");
        var result = await _addDoctorModel.OnPostAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenSuccessful_RedirectsToManageDoctors()
    {
        SetupAdminSession();
        _addDoctorModel.Input = new AddDoctorModel.AddDoctorInputModel
        {
            Name = "Dr. Smith", Email = "smith@hospital.com", Password = "password",
            Phone = "1234567890", Address = "Hospital St", BirthDate = new DateTime(1975, 3, 10),
            Gender = "M", DepartmentId = 1, Specialization = "Cardiology",
            Qualification = "MBBS, MD", Experience = 10, Salary = 80000m, ChargesPerVisit = 500m
        };

        _mockDoctorService.Setup(s => s.CreateAsync(It.IsAny<DoctorCreateDto>(), default))
            .ReturnsAsync(new DoctorDto { DoctorId = 1, Name = "Dr. Smith" });

        var result = await _addDoctorModel.OnPostAsync();

        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/ManageDoctors", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenDuplicateEmail_ReturnsPageWithErrorMessage()
    {
        SetupAdminSession();
        _addDoctorModel.Input = new AddDoctorModel.AddDoctorInputModel
        {
            Name = "Dr. Smith", Email = "existing@hospital.com", Password = "password",
            Phone = "1234567890", Address = "Hospital St", BirthDate = new DateTime(1975, 3, 10),
            Gender = "M", DepartmentId = 1, Specialization = "Cardiology",
            Qualification = "MBBS", Experience = 5, Salary = 60000m, ChargesPerVisit = 300m
        };

        _mockDoctorService.Setup(s => s.CreateAsync(It.IsAny<DoctorCreateDto>(), default))
            .ThrowsAsync(new DuplicateEntityException("A doctor with this email already exists."));

        var result = await _addDoctorModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("A doctor with this email already exists.", _addDoctorModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_ReturnsPageWithErrorMessage()
    {
        SetupAdminSession();
        _addDoctorModel.Input = new AddDoctorModel.AddDoctorInputModel
        {
            Name = "Dr. Jones", Email = "jones@hospital.com", Password = "password",
            Phone = "9876543210", Address = "Medical Ave", BirthDate = new DateTime(1980, 7, 20),
            Gender = "F", DepartmentId = 2, Specialization = "Neurology",
            Qualification = "MBBS, DM", Experience = 8, Salary = 90000m, ChargesPerVisit = 600m
        };

        _mockDoctorService.Setup(s => s.CreateAsync(It.IsAny<DoctorCreateDto>(), default))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _addDoctorModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("An error occurred. Please try again.", _addDoctorModel.ErrorMessage);
    }

    [Fact]
    public void AddDoctorInputModel_DefaultValues()
    {
        var input = new AddDoctorModel.AddDoctorInputModel();
        Assert.Equal(string.Empty, input.Name);
        Assert.Equal(string.Empty, input.Email);
        Assert.Equal(0, input.DepartmentId);
        Assert.Equal(0, input.Experience);
        Assert.Equal(0m, input.Salary);
        Assert.Equal(0m, input.ChargesPerVisit);
    }
}
