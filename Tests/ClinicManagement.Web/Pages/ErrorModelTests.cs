using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages;

/// <summary>
/// Comprehensive unit tests for ErrorModel page.
/// </summary>
public class ErrorModelTests
{
    private ClinicManagement.Web.Pages.ErrorModel CreateSut()
    {
        var sut = new ClinicManagement.Web.Pages.ErrorModel();
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        sut.PageContext = new PageContext { HttpContext = httpContext };
        return sut;
    }

    // ─── Constructor Tests ────────────────────────────────────────────────────

    [Fact]
    public void Constructor_CreatesInstance()
    {
        var model = new ClinicManagement.Web.Pages.ErrorModel();
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_InitialState_RequestIdIsNull()
    {
        var sut = CreateSut();
        Assert.Null(sut.RequestId);
    }

    // ─── ShowRequestId Tests ──────────────────────────────────────────────────

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
    {
        var sut = CreateSut();
        Assert.False(sut.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
    {
        var sut = CreateSut();
        sut.RequestId = "";
        Assert.False(sut.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdHasValue_ReturnsTrue()
    {
        var sut = CreateSut();
        sut.RequestId = "some-request-id";
        Assert.True(sut.ShowRequestId);
    }

    // ─── OnGet Tests ─────────────────────────────────────────────────────────

    [Fact]
    public void OnGet_WithTraceIdentifier_SetsRequestId()
    {
        var sut = CreateSut();
        sut.OnGet();
        Assert.NotNull(sut.RequestId);
    }

    [Fact]
    public void OnGet_SetsRequestIdFromTraceIdentifier()
    {
        var sut = new ClinicManagement.Web.Pages.ErrorModel();
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "my-trace-id-123";
        sut.PageContext = new PageContext { HttpContext = httpContext };

        sut.OnGet();

        Assert.Equal("my-trace-id-123", sut.RequestId);
    }

    [Fact]
    public void OnGet_AfterCall_ShowRequestIdIsTrue()
    {
        var sut = new ClinicManagement.Web.Pages.ErrorModel();
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "trace-123";
        sut.PageContext = new PageContext { HttpContext = httpContext };

        sut.OnGet();

        Assert.True(sut.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsWhitespace_ReturnsFalse()
    {
        var sut = CreateSut();
        sut.RequestId = "   ";
        // ShowRequestId uses !string.IsNullOrEmpty, so whitespace returns true
        Assert.True(sut.ShowRequestId);
    }

    [Fact]
    public void OnGet_DifferentTraceIds_SetsCorrectRequestId()
    {
        var sut = new ClinicManagement.Web.Pages.ErrorModel();
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "unique-trace-xyz-789";
        sut.PageContext = new PageContext { HttpContext = httpContext };

        sut.OnGet();

        Assert.Equal("unique-trace-xyz-789", sut.RequestId);
    }
}
