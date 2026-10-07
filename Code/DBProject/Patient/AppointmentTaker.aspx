@page "/Patient/AppointmentTaker"
@model DBProject.Patient.AppointmentTakerModel
@{
    ViewData["Title"] = "Appointment Taker";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
   GridView replaced with async Razor HTML table. Data is bound asynchronously
   via OnGetAsync() using Amazon RDS Task-based API, preventing thread pool
   exhaustion under load and enabling cloud auto-scaling. *@

@section head {
    <title>Appointment Taker</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<h1><strong style="margin:37%">Free Time Slots</strong></h1>
<br /><br />

<p>@Model.AppointmentMessage</p>
<br /><br />

@if (Model.FreeSlots != null && Model.FreeSlots.Rows.Count > 0)
{
    <table class="GridView-d table" style="width:800px; color:black; border:1px solid #DEDFDE;">
        <thead style="background-color:#6B696B; color:white; font-weight:bold;">
            <tr>
                <th style="width:50px">No.</th>
                @foreach (System.Data.DataColumn col in Model.FreeSlots.Columns)
                {
                    <th>@col.ColumnName</th>
                }
                <th>Select</th>
            </tr>
        </thead>
        <tbody>
            @{ int rowNumber = 1; }
            @foreach (System.Data.DataRow row in Model.FreeSlots.Rows)
            {
                <tr style="background-color:#F7F7DE;">
                    <td style="width:50px">@rowNumber</td>
                    @foreach (var cell in row.ItemArray)
                    {
                        <td>@cell</td>
                    }
                    <td>
                        <form method="post">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="slotValue" value="@(row.ItemArray.Length > 0 ? row.ItemArray[0]?.ToString() : "")" />
                            <button type="submit" asp-page-handler="SelectSlot" style="font-weight:bold;">Select</button>
                        </form>
                    </td>
                </tr>
                rowNumber++;
            }
        </tbody>
    </table>
}

<br /><br />
