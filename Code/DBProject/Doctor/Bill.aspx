@*
    Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
    Rule cr-dotnet-0026: Web Forms Usage – replaced <%@ Page %> directive with
    Razor Pages @page / @model directives for cloud-native, horizontally-scalable
    deployment on AWS (Linux containers / Elastic Beanstalk / ECS).
*@
@page "/Doctor/Bill"
@model DBProject.Pages.Doctor.BillModel
@{
    ViewData["Title"] = "Generate Bill";
    Layout = "~/Pages/Doctor/_DoctorLayout.cshtml";
}

<h1>
    Your Bill For this Appointment is :
    <strong>@Model.BillAmount</strong>
</h1>

<br /><br /><br /><br /><br />
<br /><br /><br /><br /><br />
<br /><br /><br /><br /><br />

@if (!string.IsNullOrEmpty(Model.ErrorMessage))
{
    <div class="alert alert-danger">@Model.ErrorMessage</div>
}

<form method="post">
    &nbsp;&nbsp;&nbsp;&nbsp;
    <button type="submit" asp-page-handler="BillPaid" class="btn btn-primary" style="font-weight:bold">
        Bill Paid
    </button>

    &nbsp;&nbsp;&nbsp;&nbsp;
    <button type="submit" asp-page-handler="BillUnpaid" class="btn btn-secondary" style="font-weight:bold">
        Bill Unpaid
    </button>
</form>
