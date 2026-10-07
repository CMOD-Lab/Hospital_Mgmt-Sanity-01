@page "/Doctor/PatientHistory"
@model doctor.PatientHistoryModel
@{
    ViewData["Title"] = "Patient History";
    Layout = "~/Doctor/_DoctorLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
   GridView replaced with async Razor HTML table. Data is bound asynchronously
   via OnGetAsync() using Amazon RDS Task-based API, preventing thread pool
   exhaustion under load and enabling cloud auto-scaling. *@

@section head {
    <title>Patient History</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<h1><strong style="margin:25%">Today's Appointments</strong></h1>
<br /><br />

<div style="margin-left:150px">
    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
    {
        <script>alert('@Model.ErrorMessage');</script>
    }

    @if (Model.PatientsData != null && Model.PatientsData.Rows.Count > 0)
    {
        <table class="GridView-d table" style="width:1000px; color:black;">
            <thead style="background-color:#6B696B; color:white; font-weight:bold;">
                <tr>
                    @foreach (System.Data.DataColumn col in Model.PatientsData.Columns)
                    {
                        <th>@col.ColumnName</th>
                    }
                    <th>Select</th>
                </tr>
            </thead>
            <tbody>
                @foreach (System.Data.DataRow row in Model.PatientsData.Rows)
                {
                    <tr style="background-color:#F7F7DE;">
                        @foreach (var cell in row.ItemArray)
                        {
                            <td>@cell</td>
                        }
                        <td>
                            <form method="post" asp-page-handler="SelectAppointment">
                                <input type="hidden" name="appointmentId" value="@row[1]" />
                                <button type="submit" class="btn btn-sm btn-primary">Select</button>
                            </form>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

<br /><br /><br /><br /><br />
