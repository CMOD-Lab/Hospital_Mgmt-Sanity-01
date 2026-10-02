@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage
   Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/DoctorMaster.Master"
              AutoEventWireup="true" CodeBehind="PreviousHistory.aspx.cs" Inherits="DBProject.Doctor.PreviousHistory" %>
              replaced with Razor Page directive and layout reference (occurrence 9)

   cr-dotnet-1034 (Lines 24, 50):
     The synchronous <asp:GridView> DataBind() pattern has been replaced with an async
     Task-based HTML table bound to Model.History, populated via async EF Core queries
     (OnGetAsync / getPHistory_Async) connected to Amazon RDS.
     This prevents thread-pool exhaustion under cloud load and enables efficient
     auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).

   The Web Forms <%@ Page %> directive, <asp:Content>, <asp:Label>, and <asp:GridView>
   server controls have been replaced with standard Razor Page syntax and an HTML table,
   enabling stateless cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Doctor.PreviousHistoryModel
@{
    ViewData["Title"] = "Previous History";
    Layout = "_DoctorLayout";
}

@section Head {
    <title>Previous History</title>
    <link rel="stylesheet" href="/assets/css/grid-view.css" />
}

<h1><strong style="margin:25%">History of Treated Patients</strong></h1>
<br /><br />

@*
   cr-dotnet-1034 (Line 24): Replaced synchronous <asp:Label ID="PHistory" runat="server">
   and <asp:GridView ID="PHistoryGrid" runat="server" ...> with async-loaded HTML table.
   Data is populated by OnGetAsync() using await getPHistory_Async() via EF Core
   connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
   Replaces <asp:Label> error display with model-bound ErrorMessage property.
*@
@* Replaces <asp:Label ID="PHistory" runat="server"> error label *@
@if (!string.IsNullOrEmpty(Model.ErrorMessage))
{
    <p class="alert alert-danger">@Model.ErrorMessage</p>
}

<br /><br />

@*
   cr-dotnet-1034 (Line 50): Replaced synchronous PHistoryGrid.DataSource = DT;
   PHistoryGrid.DataBind() with async HTML table rendering. Model.History is populated
   asynchronously via await _dbContext.Database.SqlQueryRaw<T>(...).ToListAsync()
   connected to Amazon RDS, enabling non-blocking request handling under cloud load.
   Replaces <asp:TemplateField HeaderText="No."> with inline row counter.
*@
@* Replaces <asp:GridView ID="PHistoryGrid" runat="server"> *@
@if (Model.History != null && Model.History.Rows.Count > 0)
{
    <table class="GridView-d table" cellpadding="4" style="width:800px;
           background-color:white; border-color:#DEDFDE; border-style:none; border-width:1px; color:black;">
        <thead style="background-color:#6B696B; color:white; font-weight:bold;">
            <tr>
                @* Replaces asp:TemplateField HeaderText="No." *@
                <th style="width:50px;">No.</th>
                @foreach (System.Data.DataColumn col in Model.History.Columns)
                {
                    <th>@col.ColumnName</th>
                }
            </tr>
        </thead>
        <tbody>
            @{ int rowIndex = 1; }
            @foreach (System.Data.DataRow row in Model.History.Rows)
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
else if (string.IsNullOrEmpty(Model.ErrorMessage))
{
    <p>No patient history records found.</p>
}

<br /><br />
