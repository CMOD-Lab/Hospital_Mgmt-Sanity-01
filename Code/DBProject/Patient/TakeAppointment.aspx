@page "/Patient/TakeAppointment"
@model DBProject.Patient.TakeAppointmentModel
@{
    ViewData["Title"] = "Departments";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls - replaced with async Razor Pages model binding *@
@* OnGetAsync() / OnPostSelectDeptAsync() use async DAL (getdeptInfoAsync) connected to Amazon RDS *@
@* preventing thread pool exhaustion under load and enabling efficient auto-scaling. *@

@section head {
    <title>Departments</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<h1><strong style="margin:23%">Select a Department to view its Doctors</strong></h1>
<br /><br />

@if (!string.IsNullOrEmpty(Model.TDept))
{
    <span>@Model.TDept</span>
    <br /><br />
}

@if (Model.Departments != null && Model.Departments.Rows.Count > 0)
{
    <table class="GridView-d" cellpadding="4" style="color:black; width:1000px; border:1px solid #DEDFDE;">
        <thead>
            <tr style="background-color:#6B696B; color:white; font-weight:bold;">
                <th style="width:50px;">No.</th>
                @foreach (System.Data.DataColumn col in Model.Departments.Columns)
                {
                    <th>@col.ColumnName</th>
                }
                <th>Select</th>
            </tr>
        </thead>
        <tbody>
            @{ int rowIndex = 0; }
            @foreach (System.Data.DataRow row in Model.Departments.Rows)
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
                            <input type="hidden" name="deptName" value="@row[1]" />
                            <button type="submit" asp-page-handler="SelectDept">Select</button>
                        </form>
                    </td>
                </tr>
                rowIndex++;
            }
        </tbody>
    </table>
}

<br /><br />
