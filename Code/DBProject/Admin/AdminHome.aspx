@page "/Admin/AdminHome"
@model DBProject.Admin.AdminHomeModel
@{
    ViewData["Title"] = "Admin Home";
    Layout = "~/Admin/_AdminLayout.cshtml";
}

@section Head {
}

<form method="post">
    <br />
    <h1 style="font-family: 'Times New Roman', Times, serif;border-radius:5px; text-decoration: underline; background-color: #CCCCCC"><strong style="margin:37%">Clinic Stats</strong></h1>
    <br /><br />

    <div style="margin-left: 70px">
        <h4><strong>Total Number of Regstered Doctors: </strong></h4>
        <span style="font-weight:bold; font-size:medium">@Model.TotalDoctors</span>
        <br /><br />
           
        <h4><strong>Total Registered Patients: </strong></h4>
        <span style="font-weight:bold; font-size:medium">@Model.TotalPatients</span>
        <br /><br />

        <h4><strong>Total Income: </strong></h4>
        <span style="font-weight:bold; font-size:medium">@Model.TotalIncome</span>
        <br /><br />

        <h3><strong style="margin:5%">Current Appointments</strong></h3>
        
        @if (Model.AppointmentData != null && Model.AppointmentData.Rows.Count > 0)
        {
            <table class="table table-bordered" style="background-color:white; border-color:#DEDFDE;">
                <thead style="background-color:#6B696B; color:white; font-weight:bold;">
                    <tr>
                        @foreach (System.Data.DataColumn col in Model.AppointmentData.Columns)
                        {
                            <th>@col.ColumnName</th>
                        }
                    </tr>
                </thead>
                <tbody>
                    @foreach (System.Data.DataRow row in Model.AppointmentData.Rows)
                    {
                        <tr style="background-color:#F7F7DE;">
                            @foreach (var item in row.ItemArray)
                            {
                                <td>@item</td>
                            }
                        </tr>
                    }
                </tbody>
            </table>
        }
    </div>

    <div style="margin:20%">
        <h2><strong style="margin:20%">Department Information</strong></h2>

        @if (Model.DepartmentData != null && Model.DepartmentData.Rows.Count > 0)
        {
            <table class="table table-bordered" style="background-color:white; border-color:#DEDFDE;">
                <thead style="background-color:#6B696B; color:white; font-weight:bold;">
                    <tr>
                        @foreach (System.Data.DataColumn col in Model.DepartmentData.Columns)
                        {
                            <th>@col.ColumnName</th>
                        }
                    </tr>
                </thead>
                <tbody>
                    @foreach (System.Data.DataRow row in Model.DepartmentData.Rows)
                    {
                        <tr style="background-color:#F7F7DE;">
                            @foreach (var item in row.ItemArray)
                            {
                                <td>@item</td>
                            }
                        </tr>
                    }
                </tbody>
            </table>
        }
    </div>
</form>
