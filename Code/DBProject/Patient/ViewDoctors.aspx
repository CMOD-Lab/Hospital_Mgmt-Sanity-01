@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#"
              MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true"
              CodeBehind="ViewDoctors.aspx.cs"
              Inherits="DBProject.ViewDoctors" %>
              replaced with Razor Page @page / @model directives.

   The <%@ Page %> directive, <asp:Content>, <asp:Label>, and <asp:GridView>
   server controls (including AutoGenerateSelectButton and OnRowCommand) have
   been replaced with standard Razor Page constructs.
   Doctor selection is now handled via a hyperlink that navigates to
   /Patient/DoctorProfile?dID=<doctorId>, replacing the Web Forms
   TDoctorGrid_RowCommand GridViewCommandEventArgs handler.
   The Master Page reference is replaced by Layout = "_PatientLayout".
   Enables stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.ViewDoctorsModel
@{
    ViewData["Title"] = "Doctors";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Doctors</title>
}

<!------------------Styling---------------->
<link rel="stylesheet" href="/assets/css/grid-view.css" />

<h1><strong style="margin:23%">Select a Doctor to view his Profile</strong></h1>
<br /><br />

@if (!string.IsNullOrEmpty(Model.TDoctorMessage))
{
    <span>@Model.TDoctorMessage</span>
}
<br /><br />

@if (Model.Doctors != null && Model.Doctors.Rows.Count > 0)
{
    <table class="GridView-d" cellpadding="4" style="color:black; border-style:none; border-width:1px; width:1000px;">
        <thead>
            <tr style="background-color:#6B696B; font-weight:bold; color:white;">
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
                rowIndex++;
                string rowStyle = (rowIndex % 2 == 0) ? "background-color:white;" : "background-color:#F7F7DE;";
                <tr style="@rowStyle">
                    <td style="width:50px;">@rowIndex</td>
                    @foreach (var cell in row.ItemArray)
                    {
                        <td>@cell</td>
                    }
                    <td>
                        <a href="/Patient/DoctorProfile?dID=@Uri.EscapeDataString(row[0].ToString())"
                           style="color:#CE5D5A; font-weight:bold;">Select</a>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

<br /><br />
