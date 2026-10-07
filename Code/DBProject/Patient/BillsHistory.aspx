@page "/Patient/BillsHistory"
@model DBProject.Patient.BillsHistoryModel
@{
    ViewData["Title"] = "Bills History";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
   GridView replaced with async Razor HTML table. Data is bound asynchronously
   via OnGetAsync() using Amazon RDS Task-based API, preventing thread pool
   exhaustion under load and enabling cloud auto-scaling. *@

@section head {
    <title>Bills History</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<h1><strong style="margin:37%">Your Bill(s) History</strong></h1>
<br /><br />

<p>@Model.BillMessage</p>
<br /><br />

@if (Model.BillData != null && Model.BillData.Rows.Count > 0)
{
    <table class="GridView-d table" style="width:800px; color:black; border:1px solid #DEDFDE;">
        <thead style="background-color:#6B696B; color:white; font-weight:bold;">
            <tr>
                <th style="width:50px">No.</th>
                @foreach (System.Data.DataColumn col in Model.BillData.Columns)
                {
                    <th>@col.ColumnName</th>
                }
            </tr>
        </thead>
        <tbody>
            @{ int rowNumber = 1; }
            @foreach (System.Data.DataRow row in Model.BillData.Rows)
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
