using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

/// <summary>
/// Comprehensive unit tests for AddStaffModel page.
/// </summary>
public class AddStaffModelTests
{
    private readonly Mock<IAdminService> _mockAdminService;

    public AddStaffModelTests()
    {
        _mockAdminService = new Mock<IAdminService>();
    }

    private ClinicManagement.Web.Pages.Admin.AddStaffModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Admin.AddStaffModel(_mockAdminService.Object);
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
        var model = new ClinicManagement.Web.Pages.Admin.AddStaffModel(_mockAdminService.Object);
        Assert.NotNull(model);
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

    [Fact]
    public void OnGet_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = sut.OnGet();
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public void OnGet_WithValidSession_ReturnsPage()
    {
        var sut = CreateSut(3);
        var result = sut.OnGet();
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostAsync(
            "John", "1990-01-01", "1234567890", "M", "123 Main St",
            50000, "BSc", "Nurse", CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_SuccessfulAdd_SetsSuccessMessage()
    {
        var sut = CreateSut(3);
        _mockAdminService
            .Setup(s => s.AddStaffAsync(It.IsAny<AddStaffRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await sut.OnPostAsync(
            "John", "1990-01-01", "1234567890", "M", "123 Main St",
            50000, "BSc", "Nurse", CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Staff member added successfully!", sut.Message);
    }

    [Fact]
    public async Task OnPostAsync_FailedAdd_SetsFailureMessage()
    {
        var sut = CreateSut(3);
        _mockAdminService
            .Setup(s => s.AddStaffAsync(It.IsAny<AddStaffRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await sut.OnPostAsync(
            "John", "1990-01-01", "1234567890", "M", "123 Main St",
            50000, "BSc", "Nurse", CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to add staff member.", sut.Message);
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyGender_UsesDefaultMale()
    {
        var sut = CreateSut(3);
        AddStaffRequest? capturedRequest = null;
        _mockAdminService
            .Setup(s => s.AddStaffAsync(It.IsAny<AddStaffRequest>(), It.IsAny<CancellationToken>()))
            .Callback<AddStaffRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(true);

        await sut.OnPostAsync(
            "John", "1990-01-01", "1234567890", "", "123 Main St",
            50000, "BSc", "Nurse", CancellationToken.None);

        Assert.NotNull(capturedRequest);
        Assert.Equal('M', capturedRequest!.Gender);
    }

    [Fact]
    public async Task OnPostAsync_WithFemaleGender_UsesFemale()
    {
        var sut = CreateSut(3);
        AddStaffRequest? capturedRequest = null;
        _mockAdminService
            .Setup(s => s.AddStaffAsync(It.IsAny<AddStaffRequest>(), It.IsAny<CancellationToken>()))
            .Callback<AddStaffRequest, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(true);

        await sut.OnPostAsync(
            "Jane", "1990-01-01", "1234567890", "F", "123 Main St",
            50000, "BSc", "Nurse", CancellationToken.None);

        Assert.NotNull(capturedRequest);
        Assert.Equal('F', capturedRequest!.Gender);
    }

    [Fact]
    public async Task OnPostAsync_CallsAddStaffAsyncWithCorrectData()
    {
        var sut = CreateSut(3);
        _mockAdminService
            .Setup(s => s.AddStaffAsync(It.IsAny<AddStaffRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await sut.OnPostAsync(
            "John", "1990-01-01", "1234567890", "M", "123 Main St",
            50000, "BSc", "Nurse", CancellationToken.None);

        _mockAdminService.Verify(s => s.AddStaffAsync(
            It.Is<AddStaffRequest>(r =>
                r.Name == "John" &&
                r.Phone == "1234567890" &&
                r.Salary == 50000 &&
                r.Designation == "Nurse"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
