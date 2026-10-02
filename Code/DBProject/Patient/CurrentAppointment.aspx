@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true" CodeBehind="CurrentAppointment.aspx.cs"
              Inherits="DBProject.CurrentAppointment" %>
              replaced with Razor Page directive and layout reference (occurrence 9)

   The Web Forms <%@ Page %> directive, <asp:Content>, and <asp:Label>
   server controls have been replaced with standard Razor Page syntax,
   enabling stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.CurrentAppointmentModel
@{
    ViewData["Title"] = "Current Appointment";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Current Appointment</title>
}

<h1><strong style="margin:30%">Current Appointments</strong></h1>
<br /><br />

<div style="margin-left: 70px">

    @* Replaces <asp:Label ID="Appointment" runat="server"> status/error label *@
    @if (!string.IsNullOrEmpty(Model.AppointmentMessage))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.AppointmentMessage</p>
    }
    <br /><br />

    @* Replaces <asp:Label ID="ADoctor" runat="server"> doctor info label *@
    @if (!string.IsNullOrEmpty(Model.DoctorMessage))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.DoctorMessage</p>
    }
    <br /><br />

    @* Replaces <asp:Label ID="ATimings" runat="server"> timings label *@
    @if (!string.IsNullOrEmpty(Model.TimingsMessage))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.TimingsMessage</p>
    }
    <br /><br />

</div>
