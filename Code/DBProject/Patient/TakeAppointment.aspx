@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#"
              MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true"
              CodeBehind="TakeAppointment.aspx.cs"
              Inherits="DBProject.TakeAppointment" %>
              replaced with Razor Page @page / @model directives.

   The <%@ Page %> directive, <asp:Content>, <asp:Label>, and <asp:GridView>
   server controls have been replaced with standard Razor Page constructs.
   The Master Page reference is replaced by Layout = "_PatientLayout".
   Enables stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.TakeAppointmentModel
@{
    ViewData["Title"] = "Departments";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Departments</title>
}

<!------------------Styling---------------->
<link rel="stylesheet" href="/assets/css/grid-view.css" />

<h1><strong style="margin:23%">Select a Department to view its Doctors</strong></h1>
<br /><br />

@if (!string.IsNullOrEmpty(Model.TDeptMessage))
{
    <span>@Model.TDeptMessage</span>
}
<br /><br />

@if (Model.Departments != null && Model.Departments.Rows.Count > 0)
{
    <table class="GridView-d" cellpadding="4" style="color:black; border-style:none; border-width:1px; width:1000px;">
        <thead>
            <tr style="background-color:#6B696B; font-weight:bold; color:white;">
                <th>No.</th>
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
                rowIndex++;
                string rowStyle = (rowIndex % 2 == 0) ? "background-color:white;" : "background-color:#F7F7DE;";
                <tr style="@rowStyle">
                    <td style="width:50px;">@rowIndex</td>
                    @foreach (var cell in row.ItemArray)
                    {
                        <td>@cell</td>
                    }
                    <td>
                        <a href="/Patient/ViewDoctors?dept=@Uri.EscapeDataString(row[1].ToString())"
                           style="color:#CE5D5A; font-weight:bold;">Select</a>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

<br /><br />
