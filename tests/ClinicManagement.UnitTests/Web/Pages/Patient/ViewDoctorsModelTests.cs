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
/// Unit tests for ViewDoctorsModel page.
/// </summary>
public class ViewDoctorsModelTests
{
    private readonly Mock<IPatientService> _mockPatientService;

    public ViewDoctorsModelTests()
    {
        _mockPatientService = new Mock<IPatientService>();
    }

    private ClinicManagement.Web.Pages.Patient.ViewDoctorsModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Patient.ViewDoctorsModel(_mockPatientService.Object);
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
        var model = new ClinicManagement.Web.Pages.Patient.ViewDoctorsModel(_mockPatientService.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_DepartmentsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Departments);
    }

    [Fact]
    public void Constructor_InitialState_DoctorsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Doctors);
    }

    [Fact]
    public void Constructor_InitialState_SelectedDeptIsNull()
    {
        var sut = CreateSut(1);
        Assert.Null(sut.SelectedDept);
    }

    [Fact]
    public async Task OnGetAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnGetAsync(null, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WithValidSession_NoDeptName_ReturnsDepartmentsOnly()
    {
        var sut = CreateSut(1);
        var depts = new List<DeptInfo>
        {
            new DeptInfo(1, "Cardiology", "Heart dept"),
            new DeptInfo(2, "Neurology", "Brain dept")
        };
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(depts);

        var result = await sut.OnGetAsync(null, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, sut.Departments.Count());
        Assert.Empty(sut.Doctors);
        Assert.Null(sut.SelectedDept);
    }

    [Fact]
    public async Task OnGetAsync_WithDeptName_LoadsDoctors()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo> { new DeptInfo(1, "Cardiology", null) });
        _mockPatientService
            .Setup(s => s.GetDoctorsByDepartmentAsync("Cardiology", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorListItem>
            {
                new DoctorListItem(1, "Dr. Smith", "Cardiology"),
                new DoctorListItem(2, "Dr. Jones", "Cardiology")
            });

        var result = await sut.OnGetAsync("Cardiology", CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Cardiology", sut.SelectedDept);
        Assert.Equal(2, sut.Doctors.Count());
    }

    [Fact]
    public async Task OnGetAsync_WithEmptyDeptName_DoesNotLoadDoctors()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());

        await sut.OnGetAsync("", CancellationToken.None);

        _mockPatientService.Verify(s => s.GetDoctorsByDepartmentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task OnGetAsync_AlwaysLoadsDepartments()
    {
        var sut = CreateSut(1);
        _mockPatientService
            .Setup(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeptInfo>());

        await sut.OnGetAsync(null, CancellationToken.None);

        _mockPatientService.Verify(s => s.GetDepartmentsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
