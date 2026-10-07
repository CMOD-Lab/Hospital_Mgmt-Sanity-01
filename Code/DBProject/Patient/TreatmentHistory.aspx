@page "/Patient/TreatmentHistory"
@model DBProject.Patient.TreatmentHistoryModel
@{
    ViewData["Title"] = "Treatment History";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls - replaced with async Razor Pages model binding *@
@* OnGetAsync() uses async DAL (getTreatmentHistoryAsync) connected to Amazon RDS *@
@* preventing thread pool exhaustion under load and enabling efficient auto-scaling. *@

@section head {
    <title>Treatment History</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<h1><strong style="margin:35%">Your Treatment History</strong></h1>
<br /><br />

@if (!string.IsNullOrEmpty(Model.THistory))
{
    <span>@Model.THistory</span>
    <br /><br />
}

@if (Model.TreatmentData != null && Model.TreatmentData.Rows.Count > 0)
{
    <table class="GridView-d" cellpadding="4" style="color:black; width:1000px; border:1px solid #DEDFDE;">
        <thead>
            <tr style="background-color:#6B696B; color:white; font-weight:bold;">
                <th style="width:50px;">No.</th>
                @foreach (System.Data.DataColumn col in Model.TreatmentData.Columns)
                {
                    <th>@col.ColumnName</th>
                }
            </tr>
        </thead>
        <tbody>
            @{ int rowIndex = 0; }
            @foreach (System.Data.DataRow row in Model.TreatmentData.Rows)
            {
                <tr style="background-color:#F7F7DE;">
                    <td style="width:50px;">@(rowIndex + 1)</td>
                    @foreach (var cell in row.ItemArray)
                    {
                        <td>@cell</td>
                    }
                </tr>
                rowIndex++;
            }
        </tbody>
    </table>
}

<br /><br />
