@page "/Patient/ViewDoctors"
@model DBProject.Patient.ViewDoctorsModel
@{
    ViewData["Title"] = "Doctors";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls - replaced with async Razor Pages model binding *@
@* OnGetAsync() / OnPostSelectDoctorAsync() use async DAL (getDeptDoctorInfoAsync) connected to Amazon RDS *@
@* preventing thread pool exhaustion under load and enabling efficient auto-scaling. *@

@section head {
    <title>Doctors</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<h1><strong style="margin:23%">Select a Doctor to view his Profile</strong></h1>
<br /><br />

@if (!string.IsNullOrEmpty(Model.TDoctor))
{
    <span>@Model.TDoctor</span>
    <br /><br />
}

@if (Model.Doctors != null && Model.Doctors.Rows.Count > 0)
{
    <table class="GridView-d" cellpadding="4" style="color:black; width:1000px; border:1px solid #DEDFDE;">
        <thead>
            <tr style="background-color:#6B696B; color:white; font-weight:bold;">
                <th style="width:50px;">No.</th>
                @foreach (System.Data.DataColumn col in Model.Doctors.Columns)
                {
                    <th>@col.ColumnName</th>
                }
                <th>Select</th>
            </tr>
        </thead>
        <tbody>
            @{ int rowIndex = 0; }
            @foreach (System.Data.DataRow row in Model.Doctors.Rows)
            {
                <tr style="background-color:#F7F7DE;">
                    <td style="width:50px;">@(rowIndex + 1)</td>
                    @foreach (var cell in row.ItemArray)
                    {
                        <td>@cell</td>
                    }
                    <td>
                        <form method="post">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="dID" value="@row[0]" />
                            <button type="submit" asp-page-handler="SelectDoctor">Select</button>
                        </form>
                    </td>
                </tr>
                rowIndex++;
            }
        </tbody>
    </table>
}

<br /><br />
