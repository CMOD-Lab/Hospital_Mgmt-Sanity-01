@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true" CodeBehind="BillsHistory.aspx.cs"
              Inherits="DBProject.BillsHistory" %>
              replaced with Razor Page directive and layout reference (occurrence 4)

   The Web Forms <%@ Page %> directive, <asp:Content>, <asp:Label>, and <asp:GridView>
   server controls have been replaced with standard Razor Page syntax and an HTML table,
   enabling stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.BillsHistoryModel
@{
    ViewData["Title"] = "Bills History";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Bills History</title>
}

<!------------------Styling----------------->
<link rel="stylesheet" href="/assets/css/grid-view.css" />

<h1><strong style="margin:37%">Your Bill(s) History</strong></h1>
<br /><br />

@* Replaces <asp:Label ID="BHistory" runat="server"> status/error label *@
@if (!string.IsNullOrEmpty(Model.StatusMessage))
{
    <p>@Model.StatusMessage</p>
}

<br /><br />

@* Replaces <asp:GridView ID="BHistoryGrid" runat="server"> *@
@if (Model.BillHistory != null && Model.BillHistory.Rows.Count > 0)
{
    <table class="GridView-d table" cellpadding="4" style="width:800px;
           background-color:white; border-color:#DEDFDE; border-style:none; border-width:1px; color:black;">
        <thead style="background-color:#6B696B; color:white; font-weight:bold;">
            <tr>
                @* Replaces asp:TemplateField HeaderText="No." *@
                <th style="width:50px;">No.</th>
                @foreach (System.Data.DataColumn col in Model.BillHistory.Columns)
                {
                    <th>@col.ColumnName</th>
                }
            </tr>
        </thead>
        <tbody>
            @{ int rowIndex = 1; }
            @foreach (System.Data.DataRow row in Model.BillHistory.Rows)
            {
                <tr style="background-color:#F7F7DE;">
                    @* Replaces Container.DataItemIndex + 1 row number label *@
                    <td style="width:50px;">@rowIndex</td>
                    @foreach (var item in row.ItemArray)
                    {
                        <td>@item</td>
                    }
                </tr>
                rowIndex++;
            }
        </tbody>
    </table>
}

<br /><br />
