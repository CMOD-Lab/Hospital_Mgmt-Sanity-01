<%--
    ============================================================================
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (SignUp.aspx) was the Web Forms page for the Login & Registration UI.
    It has been migrated to ASP.NET Core MVC Razor:

      • SignUp.aspx    → Views/SignUp/Index.cshtml  (Razor View)
      • SignUp.aspx.cs → Controllers/SignUpController.cs (MVC Controller)

    Web Forms patterns removed / replaced (cr-dotnet-0026):
      Line 1 – <%@ Page Language="C#" AutoEventWireup="true"
                        CodeBehind="SignUp.aspx.cs" Inherits="DBProject.SignUp" %>
               → @{ Layout = null; } in Views/SignUp/Index.cshtml
      <head runat="server">                → <head> (plain HTML)
      <form id="SignUpPage" runat="server"> → <form asp-controller="SignUp" asp-action="Login">
                                              and <form asp-controller="SignUp" asp-action="Register">
      asp:TextBox ID="loginEmail"          → <input asp-for="LoginEmail" ...>
      asp:TextBox ID="loginPassword"       → <input asp-for="LoginPassword" ...>
      asp:button ID="loginUserName" onclick="loginV" → <button type="submit"> in Login form
      asp:TextBox ID="sName"               → <input asp-for="Name" ...>
      asp:TextBox ID="sBirthDate"          → <input asp-for="BirthDate" ...>
      asp:TextBox ID="sEmail"              → <input asp-for="Email" ...>
      asp:TextBox ID="sPassword"           → <input asp-for="Password" ...>
      asp:TextBox ID="scPassword"          → <input asp-for="ConfirmPassword" ...>
      asp:TextBox ID="Phone"               → <input asp-for="PhoneNo" ...>
      asp:TextBox ID="Address"             → <textarea asp-for="Address" ...>
      asp:button Text="SignUp" onclick="signupV" → <button type="submit"> in Register form
      <%=sName.ClientID %>                 → id="Name" (standard HTML id)
      Request.Form["Gender"] in JS         → standard radio button name="Gender"

    The active Razor view is at Views/SignUp/Index.cshtml.
    The active MVC controller is at Controllers/SignUpController.cs.
    ============================================================================
--%>
