@page "/Doctor/PreviousHistory"
@model DBProject.Doctor.PreviousHistoryModel
@{
    ViewData["Title"] = "Previous History";
    Layout = "~/Doctor/_DoctorLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
   GridView replaced with async Razor HTML table. Data is bound asynchronously
   via OnGetAsync() using Amazon RDS Task-based API, preventing thread pool
   exhaustion under load and enabling cloud auto-scaling. *@

@section head {
    <title>Previous History</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<h1><strong style="margin:25%">History of Treated Patients</strong></h1>
<br /><br />

@if (!string.IsNullOrEmpty(Model.ErrorMessage))
{
    <p>@Model.ErrorMessage</p>
}

@if (Model.HistoryData != null && Model.HistoryData.Rows.Count > 0)
{
    <table class="GridView-d table" style="width:800px; color:black;">
        <thead style="background-color:#6B696B; color:white; font-weight:bold;">
            <tr>
                <th style="width:50px">No.</th>
                @foreach (System.Data.DataColumn col in Model.HistoryData.Columns)
                {
                    <th>@col.ColumnName</th>
                }
            </tr>
        </thead>
        <tbody>
            @{ int rowNumber = 1; }
            @foreach (System.Data.DataRow row in Model.HistoryData.Rows)
            {
                <tr style="background-color:#F7F7DE;">
                    <td style="width:50px">@rowNumber</td>
                    @foreach (var cell in row.ItemArray)
                    {
                        <td>@cell</td>
                    }
                </tr>
                rowNumber++;
            }
        </tbody>
    </table>
}

<br /><br />
