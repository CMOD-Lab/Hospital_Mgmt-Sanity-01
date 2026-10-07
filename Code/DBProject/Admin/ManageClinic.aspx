@page "/Admin/ManageClinic"
@model DBProject.Admin.ManageClinicModel
@{
    ViewData["Title"] = "Manage Clinic";
    Layout = "~/Admin/_AdminLayout.cshtml";
}
@* Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
   GridView replaced with async Razor HTML table. Data is bound asynchronously
   via OnGetAsync()/OnPostAsync() using Amazon RDS Task-based API, preventing
   thread pool exhaustion under load and enabling cloud auto-scaling. *@

@section Head {
<style type="text/css">
    .outer {
       margin-left:20%;
       display:inline-block;
    }
    .mydiv
    {
        display:inline-block;
    }
</style>
}

<form method="post">
    <div class="outer">
        <div>
            <h4 style="font:bold">Select Catagory :</h4>
            <br />
            <input type="radio" name="Category" id="Doctor" value="D" @(Model.SelectedCategory == "D" ? "checked" : "") onchange="this.form.submit()" /> Doctor
            <input type="radio" name="Category" id="Patient" value="P" @(Model.SelectedCategory == "P" ? "checked" : "") onchange="this.form.submit()" /> Patient
            <input type="radio" name="Category" id="OtherStaff" value="O" @(Model.SelectedCategory == "O" ? "checked" : "") onchange="this.form.submit()" /> Other Staff
        </div>

        <div>
            <input asp-for="SearchQuery" type="text" />
            <button type="submit" asp-page-handler="Search" class="btn btn-primary">Search</button>
            <br />
            <span style="font-weight:bold">@Model.Message</span>

            @if (Model.GridData != null && Model.GridData.Rows.Count > 0)
            {
                <table class="table table-bordered" style="background-color:white; border-color:#336666; width:380px; text-align:center;">
                    <caption style="caption-side:top; text-align:center; font-weight:bold;">@Model.GridCaption</caption>
                    <thead style="background-color:#336666; color:white; font-weight:bold;">
                        <tr>
                            <th>Delete</th>
                            <th>Select</th>
                            @foreach (System.Data.DataColumn col in Model.GridData.Columns)
                            {
                                <th>@col.ColumnName</th>
                            }
                        </tr>
                    </thead>
                    <tbody>
                        @for (int i = 0; i < Model.GridData.Rows.Count; i++)
                        {
                            var row = Model.GridData.Rows[i];
                            <tr style="background-color:white;">
                                <td>
                                    <button type="submit" asp-page-handler="Delete" name="rowIndex" value="@i" class="btn btn-danger btn-xs">Delete</button>
                                </td>
                                <td>
                                    <button type="submit" asp-page-handler="Select" name="rowIndex" value="@i" class="btn btn-info btn-xs">Select</button>
                                </td>
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
    </div>

    @if (!string.IsNullOrEmpty(Model.DetailHtml))
    {
        <div style="display:inline-block; float:right; margin-right:10%">
            @Html.Raw(Model.DetailHtml)
        </div>
    }
</form>
