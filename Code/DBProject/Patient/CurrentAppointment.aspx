@page "/Patient/CurrentAppointment"
@model DBProject.Patient.CurrentAppointmentModel
@{
    ViewData["Title"] = "Current Appointment";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@

@section head {
    <title>Current Appointment</title>
}

<h1><strong style="margin:30%">Current Appointments</strong></h1>
<br /><br />

<div style="margin-left: 70px">

    @if (!string.IsNullOrEmpty(Model.AppointmentStatus))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.AppointmentStatus</p>
        <br /><br />
    }

    @if (!string.IsNullOrEmpty(Model.DoctorInfo))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.DoctorInfo</p>
        <br /><br />
    }

    @if (!string.IsNullOrEmpty(Model.TimingInfo))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.TimingInfo</p>
        <br /><br />
    }

</div>
