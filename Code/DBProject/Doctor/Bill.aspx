@page "/Doctor/Bill"
@model doctor.BillModel
@{
    ViewData["Title"] = "Generate Bill";
    Layout = "~/Doctor/_DoctorLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@

@section head {
    <title>Generate Bill</title>
}

<div>
    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
    {
        <script>alert('@Model.ErrorMessage');</script>
    }

    <h1>Your Bill For this Appointment is :
        <span style="font-weight:bold; font-size:medium">@Model.BillAmount</span>
    </h1>

    <br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />

    <form method="post">
        &nbsp;&nbsp;&nbsp;&nbsp;
        <button type="submit" asp-page-handler="BillPaid" style="font-weight:bold" class="btn">Bill Paid</button>

        &nbsp;&nbsp;&nbsp;&nbsp;
        <button type="submit" asp-page-handler="BillUnpaid" style="font-weight:bold" class="btn">Bill Unpaid</button>
    </form>
</div>
