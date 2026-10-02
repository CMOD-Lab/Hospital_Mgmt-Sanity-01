@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true" CodeBehind="AppointmentRequestSent.aspx.cs"
              Inherits="DBProject.AppointmentNotificationSent" %>
              replaced with Razor Page directive and layout reference (occurrence 4)

   The Web Forms <%@ Page %> directive, <asp:Content>, <asp:Button>, and <asp:Label>
   server controls have been replaced with standard Razor Page syntax and an HTML form,
   enabling stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.AppointmentRequestSentModel
@{
    ViewData["Title"] = "Send Appointment Request";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Send Appointment Request</title>
}

<br /><br /><br /><br />
<h3><strong>Click on this button to send an appointment request to the Doctor</strong></h3>

@* Replaces <asp:Button runat="server" OnClick="sendARequest" Text="Send Request" Font-Bold="true" /> *@
<form method="post">
    @Html.AntiForgeryToken()
    <button type="submit" asp-page-handler="SendRequest" style="font-weight:bold;">Send Request</button>
</form>

<br /><br />

@* Replaces <asp:Label ID="Message" runat="server"> *@
@if (!string.IsNullOrEmpty(Model.Message))
{
    <span>@Model.Message</span>
}

<br /><br />
