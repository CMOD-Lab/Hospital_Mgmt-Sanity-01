@page "/Patient/AppointmentRequestSent"
@model DBProject.Patient.AppointmentRequestSentModel
@{
    ViewData["Title"] = "Send Appointment Request";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@

@section head {
    <title>Send Appointment Request</title>
}

<br /><br /><br /><br />
<h3><strong>Click on this button to send an appointment request to the Doctor</strong></h3>

<form method="post">
    @Html.AntiForgeryToken()
    <button type="submit" asp-page-handler="SendRequest" style="font-weight:bold;">Send Request</button>
</form>

<br /><br />
@if (!string.IsNullOrEmpty(Model.Message))
{
    <p>@Model.Message</p>
}
<br /><br />
