@page "/Patient/PatientNotifications"
@model DBProject.Patient.PatientNotificationsModel
@{
    ViewData["Title"] = "Notifications";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@

@section head {
    <title>Notifications</title>
}

<h1><strong style="margin:37%">Notifications</strong></h1>
<br /><br />

<div style="margin-left: 70px">

    @if (!string.IsNullOrEmpty(Model.Notify))
    {
        <span style="font-weight:bold; font-size:medium;">@Model.Notify</span>
        <br /><br />
    }

    @if (!string.IsNullOrEmpty(Model.NDoctor))
    {
        <span style="font-weight:bold; font-size:medium;">@Model.NDoctor</span>
        <br /><br />
    }

    @if (!string.IsNullOrEmpty(Model.NTimings))
    {
        <span style="font-weight:bold; font-size:medium;">@Model.NTimings</span>
        <br /><br />
    }

</div>
