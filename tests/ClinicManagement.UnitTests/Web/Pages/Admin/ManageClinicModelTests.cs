using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
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
using DoctorEntity = ClinicManagement.Domain.Entities.Doctor;
using PatientEntity = ClinicManagement.Domain.Entities.Patient;
using OtherStaffEntity = ClinicManagement.Domain.Entities.OtherStaff;

namespace ClinicManagement.UnitTests.Web.Pages.Admin;

/// <summary>
/// Unit tests for ManageClinicModel page.
/// </summary>
public class ManageClinicModelTests
{
    private readonly Mock<IAdminService> _mockAdminService;
    private readonly Mock<IDoctorRepository> _mockDoctorRepo;
    private readonly Mock<IStaffRepository> _mockStaffRepo;
    private readonly Mock<IPatientRepository> _mockPatientRepo;

    public ManageClinicModelTests()
    {
        _mockAdminService = new Mock<IAdminService>();
        _mockDoctorRepo = new Mock<IDoctorRepository>();
        _mockStaffRepo = new Mock<IStaffRepository>();
        _mockPatientRepo = new Mock<IPatientRepository>();
    }

    private ClinicManagement.Web.Pages.Admin.ManageClinicModel CreateSut(int? sessionUserId)
    {
        var sut = new ClinicManagement.Web.Pages.Admin.ManageClinicModel(
            _mockAdminService.Object,
            _mockDoctorRepo.Object,
            _mockStaffRepo.Object,
            _mockPatientRepo.Object);
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

    private void SetupDefaultRepoMocks()
    {
        _mockDoctorRepo
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorEntity>());
        _mockStaffRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OtherStaffEntity>());
        _mockPatientRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientEntity>());
    }

    // ─── Constructor Tests ────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.Admin.ManageClinicModel(
            _mockAdminService.Object,
            _mockDoctorRepo.Object,
            _mockStaffRepo.Object,
            _mockPatientRepo.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_DoctorsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Doctors);
    }

    [Fact]
    public void Constructor_InitialState_StaffIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Staff);
    }

    [Fact]
    public void Constructor_InitialState_PatientsIsEmpty()
    {
        var sut = CreateSut(1);
        Assert.Empty(sut.Patients);
    }

    // ─── OnGetAsync Tests ─────────────────────────────────────────────────────

    [Fact]
    public async Task OnGetAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnGetAsync(null, null, null, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WithValidSession_ReturnsPage()
    {
        var sut = CreateSut(3);
        SetupDefaultRepoMocks();

        var result = await sut.OnGetAsync(null, null, null, CancellationToken.None);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_NoSearchTerms_LoadsAllData()
    {
        var sut = CreateSut(3);
        var doctors = new List<DoctorEntity>
        {
            new DoctorEntity
            {
                DoctorId = 1, Name = "Dr. Smith", Status = DoctorStatus.Present,
                Qualification = "MBBS", BirthDate = new DateTime(1980, 1, 1),
                Gender = 'M', DeptNo = 1, ChargesPerVisit = 500
            }
        };
        _mockDoctorRepo
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(doctors);
        _mockStaffRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OtherStaffEntity>());
        _mockPatientRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientEntity>());

        await sut.OnGetAsync(null, null, null, CancellationToken.None);

        Assert.Single(sut.Doctors);
        _mockDoctorRepo.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithDoctorSearch_SearchesDoctors()
    {
        var sut = CreateSut(3);
        _mockDoctorRepo
            .Setup(r => r.SearchAsync("Smith", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorEntity>
            {
                new DoctorEntity
                {
                    DoctorId = 1, Name = "Dr. Smith", Status = DoctorStatus.Present,
                    Qualification = "MBBS", BirthDate = new DateTime(1980, 1, 1),
                    Gender = 'M', DeptNo = 1, ChargesPerVisit = 500
                }
            });
        _mockStaffRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OtherStaffEntity>());
        _mockPatientRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientEntity>());

        await sut.OnGetAsync("Smith", null, null, CancellationToken.None);

        _mockDoctorRepo.Verify(r => r.SearchAsync("Smith", It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Smith", sut.DoctorSearch);
    }

    [Fact]
    public async Task OnGetAsync_WithStaffSearch_SearchesStaff()
    {
        var sut = CreateSut(3);
        _mockDoctorRepo
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorEntity>());
        _mockStaffRepo
            .Setup(r => r.SearchAsync("John", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OtherStaffEntity>());
        _mockPatientRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientEntity>());

        await sut.OnGetAsync(null, "John", null, CancellationToken.None);

        _mockStaffRepo.Verify(r => r.SearchAsync("John", It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("John", sut.StaffSearch);
    }

    [Fact]
    public async Task OnGetAsync_WithPatientSearch_SearchesPatients()
    {
        var sut = CreateSut(3);
        _mockDoctorRepo
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorEntity>());
        _mockStaffRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OtherStaffEntity>());
        _mockPatientRepo
            .Setup(r => r.SearchAsync("Jane", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PatientEntity>());

        await sut.OnGetAsync(null, null, "Jane", CancellationToken.None);

        _mockPatientRepo.Verify(r => r.SearchAsync("Jane", It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Jane", sut.PatientSearch);
    }

    // ─── OnPostDeleteDoctorAsync Tests ────────────────────────────────────────

    [Fact]
    public async Task OnPostDeleteDoctorAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostDeleteDoctorAsync(1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_SuccessfulDelete_SetsSuccessMessage()
    {
        var sut = CreateSut(3);
        SetupDefaultRepoMocks();
        _mockAdminService
            .Setup(s => s.DeleteDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await sut.OnPostDeleteDoctorAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Doctor removed successfully.", sut.Message);
    }

    [Fact]
    public async Task OnPostDeleteDoctorAsync_FailedDelete_SetsFailureMessage()
    {
        var sut = CreateSut(3);
        SetupDefaultRepoMocks();
        _mockAdminService
            .Setup(s => s.DeleteDoctorAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await sut.OnPostDeleteDoctorAsync(5, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to remove doctor.", sut.Message);
    }

    // ─── OnPostDeleteStaffAsync Tests ─────────────────────────────────────────

    [Fact]
    public async Task OnPostDeleteStaffAsync_NoSession_RedirectsToIndex()
    {
        var sut = CreateSut(null);
        var result = await sut.OnPostDeleteStaffAsync(1, CancellationToken.None);
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostDeleteStaffAsync_SuccessfulDelete_SetsSuccessMessage()
    {
        var sut = CreateSut(3);
        SetupDefaultRepoMocks();
        _mockAdminService
            .Setup(s => s.DeleteStaffAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await sut.OnPostDeleteStaffAsync(7, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(sut.IsSuccess);
        Assert.Equal("Staff member deleted successfully.", sut.Message);
    }

    [Fact]
    public async Task OnPostDeleteStaffAsync_FailedDelete_SetsFailureMessage()
    {
        var sut = CreateSut(3);
        SetupDefaultRepoMocks();
        _mockAdminService
            .Setup(s => s.DeleteStaffAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await sut.OnPostDeleteStaffAsync(7, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.False(sut.IsSuccess);
        Assert.Equal("Failed to delete staff member.", sut.Message);
    }
}
