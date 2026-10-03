using System;
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

public class UpdateHistoryModelTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<ILogger<UpdateHistoryModel>> _mockLogger;
    private readonly UpdateHistoryModel _updateHistoryModel;
    private readonly TestSession _testSession;

    public UpdateHistoryModelTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockLogger = new Mock<ILogger<UpdateHistoryModel>>();
        _updateHistoryModel = new UpdateHistoryModel(_mockAppointmentService.Object, _mockLogger.Object);

        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _updateHistoryModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    private void SetupDoctorSession(int doctorId = 3) { _testSession.SetInt32("UserId", doctorId); _testSession.SetInt32("UserType", 1); }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new UpdateHistoryModel(_mockAppointmentService.Object, _mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitializesInputProperty()
    {
        Assert.NotNull(_updateHistoryModel.Input);
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _updateHistoryModel.OnGetAsync(1);
        Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Account/Login", ((RedirectToPageResult)result).PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedInAsDoctor_LoadsAppointmentData()
    {
        SetupDoctorSession(3);
        var appointment = new AppointmentDto
        {
            AppointmentId = 5, Disease = "Flu", Progress = "Improving", Prescription = "Paracetamol"
        };
        _mockAppointmentService.Setup(s => s.GetByIdAsync(5, default)).ReturnsAsync(appointment);

        var result = await _updateHistoryModel.OnGetAsync(5);

        Assert.IsType<PageResult>(result);
        Assert.Equal(5, _updateHistoryModel.AppointmentId);
        Assert.Equal("Flu", _updateHistoryModel.Input.Disease);
        Assert.Equal("Improving", _updateHistoryModel.Input.Progress);
        Assert.Equal("Paracetamol", _updateHistoryModel.Input.Prescription);
    }

    [Fact]
    public async Task OnGetAsync_WhenAppointmentNotFound_ReturnsPage()
    {
        SetupDoctorSession(3);
        _mockAppointmentService.Setup(s => s.GetByIdAsync(999, default)).ReturnsAsync((AppointmentDto?)null);

        var result = await _updateHistoryModel.OnGetAsync(999);

        Assert.IsType<PageResult>(result);
        Assert.Null(_updateHistoryModel.Input.Disease);
    }

    [Fact]
    public async Task OnPostAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var result = await _updateHistoryModel.OnPostAsync();
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
    {
        SetupDoctorSession(3);
        _updateHistoryModel.ModelState.AddModelError("Disease", "Too long");

        var result = await _updateHistoryModel.OnPostAsync();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenSuccessful_SetsSuccessMessage()
    {
        SetupDoctorSession(3);
        _updateHistoryModel.AppointmentId = 5;
        _updateHistoryModel.Input = new UpdateHistoryModel.UpdateHistoryInputModel
        {
            Disease = "Cold", Progress = "Stable", Prescription = "Rest"
        };

        _mockAppointmentService.Setup(s => s.UpdatePrescriptionAsync(5, It.IsAny<AppointmentUpdateDto>(), default))
            .Returns(Task.CompletedTask);

        var result = await _updateHistoryModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("Patient history updated successfully.", _updateHistoryModel.SuccessMessage);
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_SetsErrorMessage()
    {
        SetupDoctorSession(3);
        _updateHistoryModel.AppointmentId = 5;
        _updateHistoryModel.Input = new UpdateHistoryModel.UpdateHistoryInputModel
        {
            Disease = "Cold", Progress = "Stable", Prescription = "Rest"
        };

        _mockAppointmentService.Setup(s => s.UpdatePrescriptionAsync(5, It.IsAny<AppointmentUpdateDto>(), default))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _updateHistoryModel.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("An error occurred. Please try again.", _updateHistoryModel.ErrorMessage);
    }

    [Fact]
    public void UpdateHistoryInputModel_DefaultValues()
    {
        var input = new UpdateHistoryModel.UpdateHistoryInputModel();
        Assert.Null(input.Disease);
        Assert.Null(input.Progress);
        Assert.Null(input.Prescription);
    }
}
