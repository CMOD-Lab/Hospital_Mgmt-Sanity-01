using System;
using ClinicManagement.UnitTests.Web.Helpers;
using ClinicManagement.Web.Pages.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Web.Pages.Account;

public class LogoutModelTests
{
    private readonly LogoutModel _logoutModel;
    private readonly TestSession _testSession;

    public LogoutModelTests()
    {
        _logoutModel = new LogoutModel();
        _testSession = new TestSession();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Session).Returns(_testSession);
        _logoutModel.PageContext = new PageContext { HttpContext = mockHttpContext.Object };
    }

    [Fact]
    public void Constructor_CreatesInstance()
    {
        var model = new LogoutModel();
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_ClearsSession()
    {
        // Arrange - set some session values
        _testSession.SetInt32("UserId", 1);
        _testSession.SetInt32("UserType", 2);

        // Act
        _logoutModel.OnGet();

        // Assert - session should be cleared
        Assert.False(_testSession.TryGetValue("UserId", out _));
    }

    [Fact]
    public void OnGet_RedirectsToLoginPage()
    {
        var result = _logoutModel.OnGet();
        Assert.IsType<RedirectToPageResult>(result);
        var redirect = (RedirectToPageResult)result;
        Assert.Equal("/Account/Login", redirect.PageName);
    }

    [Fact]
    public void OnGet_ClearsSessionBeforeRedirect()
    {
        _testSession.SetInt32("UserId", 5);
        var result = _logoutModel.OnGet();
        Assert.IsType<RedirectToPageResult>(result);
        Assert.False(_testSession.TryGetValue("UserId", out _));
    }
}
