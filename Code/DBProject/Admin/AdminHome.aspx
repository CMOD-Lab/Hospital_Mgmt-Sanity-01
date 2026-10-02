@*
    AdminHome.cshtml — ASP.NET Core Razor Page (replaces AdminHome.aspx)
    Rule cr-dotnet-0026: Web Forms Usage → Migrate to ASP.NET Core MVC/Razor Pages

    Migration notes:
    - <%@ Page %> directive removed; replaced with @page / @model Razor directives.
    - MasterPageFile reference removed; layout is now set via _AdminLayout.cshtml.
    - <asp:Content> / <asp:ContentPlaceHolder> replaced with Razor @section blocks.
    - <asp:Label> server controls replaced with plain HTML bound to PageModel properties.
    - <asp:GridView> server controls replaced with HTML <table> elements rendered from
      IEnumerable<DataRow> model properties.
    - <form runat="server"> removed; no postback form is needed on this read-only page.
*@
@page
@model DBProject.Pages.Admin.AdminHomeModel
@{
    ViewData["Title"] = "Admin Home";
    Layout = "~/Admin/_AdminLayout.cshtml";
}

<br />
<h1 style="font-family: 'Times New Roman', Times, serif; border-radius:5px; text-decoration: underline; background-color: #CCCCCC">
    <strong style="margin:37%">Clinic Stats</strong>
</h1>
<br /><br />

<div style="margin-left: 70px">

    <h4><strong>Total Number of Registered Doctors: </strong></h4>
    <strong>@Model.TotalDoctors</strong>
    <br /><br />

    <h4><strong>Total Registered Patients: </strong></h4>
    <strong>@Model.TotalPatients</strong>
    <br /><br />

    <h4><strong>Total Income: </strong></h4>
    <strong>@Model.TotalIncome</strong>
    <br /><br />

    <h3><strong style="margin:5%">Current Appointments</strong></h3>

    @if (Model.Appointments != null && Model.Appointments.Count > 0)
    {
        <table class="table table-bordered table-striped" style="background-color:white; border-color:#DEDFDE; color:black;">
            <thead style="background-color:#6B696B; color:white; font-weight:bold;">
                <tr>
                    @foreach (System.Data.DataColumn col in Model.Appointments[0].Table.Columns)
                    {
                        <th>@col.ColumnName</th>
                    }
                </tr>
            </thead>
            <tbody>
                @foreach (System.Data.DataRow row in Model.Appointments)
                {
                    <tr style="background-color:#F7F7DE;">
                        @foreach (var cell in row.ItemArray)
                        {
                            <td>@cell</td>
                        }
                    </tr>
                }
            </tbody>
        </table>
    }
    else
    {
        <p>No current appointments.</p>
    }

</div>

<div style="margin:20%">

    <h2><strong style="margin:20%">Department Information</strong></h2>

    @if (Model.Departments != null && Model.Departments.Count > 0)
    {
        <table class="table table-bordered table-striped" style="background-color:white; border-color:#DEDFDE; color:black; height:50px;">
            <thead style="background-color:#6B696B; color:white; font-weight:bold;">
                <tr>
                    @foreach (System.Data.DataColumn col in Model.Departments[0].Table.Columns)
                    {
                        <th>@col.ColumnName</th>
                    }
                </tr>
            </thead>
            <tbody>
                @foreach (System.Data.DataRow row in Model.Departments)
                {
                    <tr style="background-color:#F7F7DE;">
                        @foreach (var cell in row.ItemArray)
                        {
                            <td>@cell</td>
                        }
                    </tr>
                }
            </tbody>
        </table>
    }
    else
    {
        <p>No department information available.</p>
    }

</div>
