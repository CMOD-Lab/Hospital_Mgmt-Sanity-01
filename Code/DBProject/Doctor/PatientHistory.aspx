@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage
   Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/doctormaster.Master"
              AutoEventWireup="true" CodeBehind="PatientHistory.aspx.cs" Inherits="doctor.patienthistory" %>
              replaced with Razor Page directive and layout reference (occurrence 9)

   cr-dotnet-1034 (Lines 23, 41):
     The synchronous <asp:GridView> DataBind() pattern has been replaced with an async
     Task-based HTML table bound to Model.Patients, populated via async EF Core queries
     (OnGetAsync / search_patient_DAL_Async) connected to Amazon RDS.
     This prevents thread-pool exhaustion under cloud load and enables efficient
     auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).

   The Web Forms <%@ Page %> directive, <asp:Content>, and <asp:GridView> server controls
   have been replaced with standard Razor Page syntax and an HTML table, enabling stateless
   cloud-native deployment on AWS (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Doctor.PatientHistoryModel
@{
    ViewData["Title"] = "Patient History";
    Layout = "_DoctorLayout";
}

@section Head {
    <title>Patient History</title>
    <link rel="stylesheet" href="/assets/css/grid-view.css" />
}

<h1><strong style="margin:25%">Today's Appointments</strong></h1>
<br /><br />

@*
   cr-dotnet-1034 (Line 23): Replaced synchronous <asp:GridView ID="patientsgrid" runat="server"
   OnRowCommand="patientsgrid_RowCommand" ...> with async-loaded HTML table.
   Data is populated by OnGetAsync() using await search_patient_DAL_Async() via EF Core
   connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
*@
<div style="margin-left:150px">
    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
    {
        <div class="alert alert-danger">@Model.ErrorMessage</div>
    }
    else if (Model.Patients != null && Model.Patients.Rows.Count > 0)
    {
        @*
           cr-dotnet-1034 (Line 41): Replaced synchronous GridView.DataBind() with async
           HTML table rendering. Model.Patients is populated asynchronously via
           await _dbContext.Database.SqlQueryRaw<TodaysAppointmentRow>(...).ToListAsync()
           connected to Amazon RDS, enabling non-blocking request handling.
        *@
        <table class="GridView-d table" style="width:1000px" cellpadding="4">
            <thead style="background-color:#6B696B; color:white; font-weight:bold;">
                <tr>
                    @foreach (System.Data.DataColumn col in Model.Patients.Columns)
                    {
                        <th>@col.ColumnName</th>
                    }
                    <th>Select</th>
                </tr>
            </thead>
            <tbody>
                @foreach (System.Data.DataRow row in Model.Patients.Rows)
                {
                    <tr style="background-color:#F7F7DE;">
                        @foreach (var item in row.ItemArray)
                        {
                            <td>@item</td>
                        }
                        <td>
                            @{
                                // The appointment id is in column index 1 (same as original GridView Cells[1])
                                var appointmentId = row[1].ToString();
                            }
                            <form method="post" style="display:inline">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="AppointmentId" value="@appointmentId" />
                                <button type="submit" asp-page-handler="SelectAppointment"
                                        class="btn btn-sm btn-primary">Select</button>
                            </form>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
    else
    {
        <p>No appointments found for today.</p>
    }
</div>

<br />
<br />
<br />
<br />
<br />
