@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage
   Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/doctormaster.Master"
              AutoEventWireup="true" CodeBehind="PendingAppointment.aspx.cs" Inherits="doctor.pendingappointment" %>
              replaced with Razor Page directive and layout reference (occurrence 4)

   cr-dotnet-1034 (Lines 22, 40):
     The synchronous <asp:GridView> DataBind() pattern has been replaced with an async
     Task-based HTML table bound to Model.Appointments, populated via async EF Core queries
     (OnGetAsync / GetAllpendingappointments_DAL_Async) connected to Amazon RDS.
     This prevents thread-pool exhaustion under cloud load and enables efficient
     auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).

   The Web Forms <%@ Page %> directive, <asp:Content>, and <asp:GridView> server controls
   have been replaced with standard Razor Page syntax and an HTML table, enabling stateless
   cloud-native deployment on AWS (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Doctor.PendingAppointmentModel
@{
    ViewData["Title"] = "Pending Appointments";
    Layout = "_DoctorLayout";
}

@section Head {
    <title>Pending Appointments</title>
    <link rel="stylesheet" href="/assets/css/grid-view.css" />
}

<h1><strong style="margin:25%">Pending Appointments</strong></h1>
<br /><br />

@*
   cr-dotnet-1034 (Line 22): Replaced synchronous <asp:GridView ID="pendingappointments"
   runat="server" OnRowCommand="update_appointment" OnRowDeleting="Delete_appointment" ...>
   with async-loaded HTML table. Data is populated by OnGetAsync() using
   await GetAllpendingappointments_DAL_Async() via EF Core connected to Amazon RDS,
   preventing thread-pool exhaustion under cloud load.
*@
<div style="margin-left:300px">
    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
    {
        <div class="alert alert-danger">@Model.ErrorMessage</div>
    }
    else if (Model.Appointments != null && Model.Appointments.Rows.Count > 0)
    {
        @*
           cr-dotnet-1034 (Line 40): Replaced synchronous pendingappointments.DataBind()
           with async HTML table rendering. Model.Appointments is populated asynchronously
           via await _dbContext.Database.SqlQueryRaw<T>(...).ToListAsync() connected to
           Amazon RDS, enabling non-blocking request handling under cloud load.
        *@
        <table class="GridView-d table" cellpadding="4"
               style="background-color:white; border-color:#DEDFDE; border-style:none; border-width:1px; color:black;">
            <thead style="background-color:#6B696B; color:white; font-weight:bold;">
                <tr>
                    @foreach (System.Data.DataColumn col in Model.Appointments.Columns)
                    {
                        <th>@col.ColumnName</th>
                    }
                    <th>Update</th>
                    <th>Delete</th>
                </tr>
            </thead>
            <tbody>
                @foreach (System.Data.DataRow row in Model.Appointments.Rows)
                {
                    <tr style="background-color:#F7F7DE;">
                        @foreach (var item in row.ItemArray)
                        {
                            <td>@item</td>
                        }
                        @{
                            // The appointment id is in column index 1 (same as original GridView Cells[1])
                            var appointmentId = row[1].ToString();
                        }
                        <td>
                            <form method="post" style="display:inline">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="AppointmentId" value="@appointmentId" />
                                <button type="submit" asp-page-handler="UpdateAppointment"
                                        class="btn btn-sm btn-primary">Select</button>
                            </form>
                        </td>
                        <td>
                            <form method="post" style="display:inline">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="AppointmentId" value="@appointmentId" />
                                <button type="submit" asp-page-handler="DeleteAppointment"
                                        class="btn btn-sm btn-danger">Delete</button>
                            </form>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
    else
    {
        <p>No pending appointments found.</p>
    }
</div>

<br />
<br />
<br />
<br />
<br />
