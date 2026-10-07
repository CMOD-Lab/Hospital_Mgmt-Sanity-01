@page "/Doctor/HistoryUpdate"
@model doctor.HistoryUpdateModel
@{
    ViewData["Title"] = "Update History";
    Layout = "~/Doctor/_DoctorLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@

@section head {
    <title>Update History</title>
}

<h1>Update history</h1>

@if (!string.IsNullOrEmpty(Model.ErrorMessage))
{
    <script>alert('@Model.ErrorMessage');</script>
}
@if (!string.IsNullOrEmpty(Model.SuccessMessage))
{
    <script>alert('@Model.SuccessMessage');</script>
}

<form method="post">
    <h4>Disease:</h4>
    <input type="text" name="Disease" value="@Model.Disease" class="form-control" />

    <h4>Progress:</h4>
    <input type="text" name="Progress" value="@Model.Progress" class="form-control" />

    <h4>Prescription</h4>
    <input type="text" name="Prescription" value="@Model.Prescription" class="form-control" />

    <br /><br /><br />

    <button type="submit" asp-page-handler="SaveInDatabase" style="font-weight:bold" class="btn btn-primary">Accept &amp; Save</button>
    &nbsp;&nbsp;
    <button type="submit" asp-page-handler="GenerateBill" style="font-weight:bold" class="btn btn-default">Generate Bill</button>
</form>
