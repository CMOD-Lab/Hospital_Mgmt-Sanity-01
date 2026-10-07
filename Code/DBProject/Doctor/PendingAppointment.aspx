@page "/Doctor/PendingAppointment"
@model doctor.PendingAppointmentModel
@{
    ViewData["Title"] = "Pending Appointments";
    Layout = "~/Doctor/_DoctorLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
   GridView replaced with async Razor HTML table. Data is bound asynchronously
   via OnGetAsync() using Amazon RDS Task-based API, preventing thread pool
   exhaustion under load and enabling cloud auto-scaling. *@

@section head {
    <title>Pending Appointments</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<h1><strong style="margin:25%">Pending Appointments</strong></h1>
<br /><br />

<div style="margin-left:300px">
    @if (Model.PendingAppointments != null && Model.PendingAppointments.Rows.Count > 0)
    {
        <table class="GridView-d table" style="color:black;">
            <thead style="background-color:#6B696B; color:white; font-weight:bold;">
                <tr>
                    @foreach (System.Data.DataColumn col in Model.PendingAppointments.Columns)
                    {
                        <th>@col.ColumnName</th>
                    }
                    <th>Select</th>
                    <th>Delete</th>
                </tr>
            </thead>
            <tbody>
                @foreach (System.Data.DataRow row in Model.PendingAppointments.Rows)
                {
                    <tr style="background-color:#F7F7DE;">
                        @foreach (var cell in row.ItemArray)
                        {
                            <td>@cell</td>
                        }
                        <td>
                            <form method="post" asp-page-handler="UpdateAppointment">
                                <input type="hidden" name="appointmentId" value="@row[1]" />
                                <button type="submit" class="btn btn-sm btn-primary">Select</button>
                            </form>
                        </td>
                        <td>
                            <form method="post" asp-page-handler="DeleteAppointment">
                                <input type="hidden" name="appointmentId" value="@row[1]" />
                                <button type="submit" class="btn btn-sm btn-danger">Delete</button>
                            </form>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>
