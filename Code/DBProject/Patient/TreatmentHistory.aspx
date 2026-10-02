@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#"
              MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true"
              CodeBehind="TreatmentHistory.aspx.cs"
              Inherits="DBProject.TreatmentHistory" %>
              replaced with Razor Page @page / @model directives.

   The <%@ Page %> directive, <asp:Content>, <asp:Label>, and <asp:GridView>
   server controls have been replaced with standard Razor Page constructs.
   The Master Page reference is replaced by Layout = "_PatientLayout".
   Enables stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.TreatmentHistoryModel
@{
    ViewData["Title"] = "Treatment History";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Treatment History</title>
}

<!------------------Styling---------------->
<link rel="stylesheet" href="/assets/css/grid-view.css" />

<h1><strong style="margin:35%">Your Treatment History</strong></h1>
<br /><br />

@if (!string.IsNullOrEmpty(Model.THistoryMessage))
{
    <span>@Model.THistoryMessage</span>
}
<br /><br />

@if (Model.TreatmentHistoryData != null && Model.TreatmentHistoryData.Rows.Count > 0)
{
    <table class="GridView-d" cellpadding="4" style="color:black; border-style:none; border-width:1px; width:1000px;">
        <thead>
            <tr style="background-color:#6B696B; font-weight:bold; color:white;">
                <th style="width:50px;">No.</th>
                @foreach (System.Data.DataColumn col in Model.TreatmentHistoryData.Columns)
                {
                    <th>@col.ColumnName</th>
                }
            </tr>
        </thead>
        <tbody>
            @{ int rowIndex = 0; }
            @foreach (System.Data.DataRow row in Model.TreatmentHistoryData.Rows)
            {
                rowIndex++;
                string rowStyle = (rowIndex % 2 == 0) ? "background-color:white;" : "background-color:#F7F7DE;";
                <tr style="@rowStyle">
                    <td style="width:50px;">@rowIndex</td>
                    @foreach (var cell in row.ItemArray)
                    {
                        <td>@cell</td>
                    }
                </tr>
            }
        </tbody>
    </table>
}

<br /><br />
