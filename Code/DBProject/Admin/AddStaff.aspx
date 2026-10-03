@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
   Original: <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddStaff.aspx.cs" Inherits="DBProject.AddStaff" %>
   Replacement: ASP.NET Core Razor Page - AddStaff.cshtml
   The Web Forms <%@ Page %> directive, runat="server" controls, and server-side
   validation controls have been replaced with Razor Page syntax and HTML5 form
   elements with tag helpers, preserving all original business logic and UI layout.
*@
@page "/Admin/AddStaff"
@model DBProject.Admin.AddStaffModel
@{
    ViewData["Title"] = "Staff Registration";
    Layout = "~/Admin/_AdminLayout.cshtml";
}

<style type="text/css">
    html {
        background-image: url("/assets/staff9.jpg");
        background-size: cover;
        background-position: 0 -90px;
        background-attachment: fixed;
    }
    body {
        background: none !important;
    }
    .backstretch {
        display: none !important;
    }
    .grad {
        background: linear-gradient(to right, rgba(30, 160, 100, 0.15), rgba(150, 148, 255, 1));
        border-radius: 8px;
    }
    #space {
        padding-bottom: 50px;
    }
</style>

<div id="myclass">
    <link rel="stylesheet" href="http://fonts.googleapis.com/css?family=Roboto:400,100,300,500" />
    <link rel="stylesheet" href="/assets/bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" href="/assets/font-awesome/css/font-awesome.min.css" />
    <link rel="stylesheet" href="/assets/css/form-elements.css" />
    <link rel="stylesheet" href="/assets/css/style.css" />

    <!-- Top content -->
    <div class="top-content">
        <div class="inner-bg">
            <div class="container grad">
                <div class="row">
                    <div class="col-sm-8 col-sm-offset-2 text">
                        <h1><strong>Medix4 Health Care</strong> Staff Registration Form</h1>
                        <div class="description">
                            <p>
                                This is a <strong>"registeration form"</strong> for Medix4 Health Care.
                                Fill out the information to register the staff member
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Staff Registration Form -->
    <div class="container myclass">
        <div class="row">
            <div class="col-sm-2"></div>
            <div class="col-sm-8">
                <div class="form-box" id="spaces">
                    <div class="form-top">
                        <div class="form-top-left">
                            <h3 style="font-family:Algerian">Staff Registration Form</h3>

                            @if (Model.SuccessMessage != null)
                            {
                                <div class="alert alert-success" role="alert">
                                    <strong>@Model.SuccessMessage</strong>
                                </div>
                            }
                        </div>
                    </div>
                    <div class="form-bottom">
                        <form method="post" asp-page-handler="StaffRegister">
                            @Html.AntiForgeryToken()

                            <div class="form-group">
                                <span asp-validation-for="Input.Name" class="text-danger"></span>
                                <input asp-for="Input.Name" type="text" class="form-username form-control" placeholder="Name" />
                            </div>

                            <div class="form-group">
                                <span asp-validation-for="Input.BirthDate" class="text-danger"></span>
                                <input asp-for="Input.BirthDate" type="text" class="form-username form-control" placeholder="Birth Date (mm/dd/yyyy)" />
                            </div>

                            <div class="form-group">
                                <span asp-validation-for="Input.Phone" class="text-danger"></span>
                                <input asp-for="Input.Phone" type="text" class="form-username form-control" placeholder="Phone Number" />
                            </div>

                            <div class="form-group">
                                <span asp-validation-for="Input.Salary" class="text-danger"></span>
                                <span asp-validation-for="Input.Designation" class="text-danger"></span>
                                <input asp-for="Input.Salary" type="text" placeholder="Salary in Rupees" style="width:221px" />
                                <input asp-for="Input.Qual" type="text" placeholder="Qualification" />
                                <input asp-for="Input.Designation" type="text" placeholder="Designation" style="width:243px" />
                            </div>

                            <div class="form-group">
                                <input asp-for="Input.Address" type="text" class="form-username form-control" placeholder="Address" />
                            </div>

                            <div class="form-group">
                                <input type="radio" name="Gender" id="Male" value="M" checked="checked" /> <label for="Male">Male</label>
                                <input type="radio" name="Gender" id="Female" value="F" /> <label for="Female">Female</label>
                            </div>

                            <button type="submit" class="btn btn-primary">Add</button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Footer -->
<footer>
    <div class="container">
        <div class="row">
            <div class="col-sm-8 col-sm-offset-2">
                <div class="footer-border"></div>
                <p style="color:darkslategrey">if You have Any Query
                    Please Feel Free to Contact US. <i class="fa fa-smile-o"></i></p>
            </div>
        </div>
    </div>
</footer>

<!-- Javascript -->
<script src="/assets/js/jquery-1.11.1.min.js"></script>
<script src="/assets/bootstrap/js/bootstrap.min.js"></script>
<script src="/assets/js/jquery.backstretch.min.js"></script>
<script src="/assets/js/scripts.js"></script>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
