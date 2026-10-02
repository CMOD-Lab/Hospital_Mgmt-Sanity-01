@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true" CodeBehind="AppointmentTaker.aspx.cs"
              Inherits="DBProject.AppointmentTaker" %>
              replaced with Razor Page directive and layout reference (occurrence 9)

   The Web Forms <%@ Page %> directive, <asp:Content>, <asp:Label>, and <asp:GridView>
   server controls have been replaced with standard Razor Page syntax and an HTML table,
   enabling stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.AppointmentTakerModel
@{
    ViewData["Title"] = "Appointment Taker";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Appointment Taker</title>
}

<!------------------Styling----------------->
<link rel="stylesheet" href="/assets/css/grid-view.css" />

<h1><strong style="margin:37%">Free Time Slots</strong></h1>
<br /><br />

@* Replaces <asp:Label ID="PAppointment" runat="server"> status/error label *@
@if (!string.IsNullOrEmpty(Model.StatusMessage))
{
    <p>@Model.StatusMessage</p>
}

<br /><br />

@* Replaces <asp:GridView ID="PAppointmentGrid" runat="server"> *@
@if (Model.FreeSlots != null && Model.FreeSlots.Rows.Count > 0)
{
    <table class="GridView-d table" cellpadding="4" style="width:800px;
           background-color:white; border-color:#DEDFDE; border-style:none; border-width:1px; color:black;">
        <thead style="background-color:#6B696B; color:white; font-weight:bold;">
            <tr>
                @* Replaces asp:TemplateField HeaderText="No." *@
                <th style="width:50px;">No.</th>
                @foreach (System.Data.DataColumn col in Model.FreeSlots.Columns)
                {
                    <th>@col.ColumnName</th>
                }
                @* Replaces AutoGenerateSelectButton="true" *@
                <th>Select</th>
            </tr>
        </thead>
        <tbody>
            @{ int rowIndex = 1; }
            @foreach (System.Data.DataRow row in Model.FreeSlots.Rows)
            {
                <tr style="background-color:#F7F7DE;">
                    @* Replaces Container.DataItemIndex + 1 row number label *@
                    <td style="width:50px;">@rowIndex</td>
                    @foreach (var item in row.ItemArray)
                    {
                        <td>@item</td>
                    }
                    @* Replaces OnRowCommand="PAppointmentGrid_RowCommand" Select command *@
                    <td>
                        <form method="post">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="rowIndex" value="@(rowIndex - 1)" />
                            <input type="hidden" name="slotValue" value="@row.ItemArray[0]" />
                            <button type="submit" asp-page-handler="SelectSlot">Select</button>
                        </form>
                    </td>
                </tr>
                rowIndex++;
            }
        </tbody>
    </table>
}

<br /><br />
