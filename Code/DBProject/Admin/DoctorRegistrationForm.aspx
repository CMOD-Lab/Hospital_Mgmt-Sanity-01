@*
    Migrated from ASP.NET Web Forms to ASP.NET Core MVC Razor View (cr-dotnet-0026).
    Original Web Forms page directive:
        <%@ Page Language="C#" AutoEventWireup="true" UnobtrusiveValidationMode="None"
                 CodeBehind="DoctorRegistrationForm.aspx.cs"
                 Inherits="DB_Project.DoctorRegistrationForm" %>
    Replaced with ASP.NET Core MVC Razor View.
    Server-side Web Forms controls (asp:TextBox, asp:Label, asp:Button, asp:DropDownList,
    asp:RequiredFieldValidator, asp:RegularExpressionValidator, asp:CompareValidator,
    asp:RangeValidator, asp:CustomValidator, asp:RadioButton) replaced with standard
    HTML5 form elements and ASP.NET Core Tag Helpers with DataAnnotations validation.
*@
@model DBProject.Models.DoctorRegistrationViewModel
@{
    ViewData["Title"] = "Doctor Registration";
    Layout = null;
}
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Doctor Registration</title>
    <style type="text/css">
        html {
            background-image: url("/assets/Doctor.jpg");
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
            background: linear-gradient(to right, rgba(30, 160, 130, 0.15), rgba(0, 148, 255, 1));
            border-radius: 8px;
        }
        #space {
            padding-bottom: 50px;
        }
    </style>
</head>
<body>
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
                            <h1><strong>Medix4 Health Care</strong> Doctor Registration</h1>
                            <div class="description">
                                <p>
                                    This is a free <strong>"Doctor registration form"</strong> for Medix4 Health Care.
                                    Fill out the information of the Doctor to Register.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Doctor registration form -->
        <div class="container myclass">
            <div class="row">
                <div class="col-sm-2"></div>
                <div class="col-sm-8">
                    <div class="form-box" id="spaces">
                        <div class="form-top">
                            <div class="form-top-left">
                                <h3>Sign up now</h3>

                                @if (TempData["SuccessMessage"] != null)
                                {
                                    <div class="alert alert-success" style="color:blue; font-weight:bold; font-size:large;">
                                        @TempData["SuccessMessage"]
                                    </div>
                                }
                                @if (!ViewData.ModelState.IsValid)
                                {
                                    <div asp-validation-summary="All" class="text-danger"></div>
                                }
                            </div>
                        </div>

                        <div class="form-bottom">
                            <form asp-controller="DoctorRegistration" asp-action="Register" method="post">
                                @Html.AntiForgeryToken()

                                <!-- Name -->
                                <div class="form-group">
                                    <span asp-validation-for="Name" class="text-danger"></span>
                                    <input asp-for="Name" type="text" class="form-username form-control" placeholder="Name" />
                                </div>

                                <!-- Birth Date -->
                                <div class="form-group">
                                    <span asp-validation-for="BirthDate" class="text-danger"></span>
                                    <input asp-for="BirthDate" type="text" class="form-username form-control" placeholder="Birth Date (mm/dd/yyyy)" />
                                </div>

                                <!-- Email -->
                                <div class="form-group">
                                    <span asp-validation-for="Email" class="text-danger"></span>
                                    <input asp-for="Email" type="text" class="form-username form-control" placeholder="Email : person@example.com" />
                                </div>

                                <!-- Password -->
                                <div class="form-group">
                                    <span asp-validation-for="Password" class="text-danger"></span>
                                    <span asp-validation-for="ConfirmPassword" class="text-danger"></span>
                                    <input asp-for="Password" type="password" class="form-username form-control" placeholder="Enter New Password" />
                                </div>

                                <!-- Confirm Password -->
                                <div class="form-group">
                                    <input asp-for="ConfirmPassword" type="password" class="form-username form-control" placeholder="Confirm Password" />
                                </div>

                                <!-- Phone -->
                                <div class="form-group">
                                    <span asp-validation-for="Phone" class="text-danger"></span>
                                    <input asp-for="Phone" type="text" class="form-username form-control" placeholder="Phone Number" />
                                </div>

                                <!-- Salary, Charges, Experience -->
                                <span asp-validation-for="Salary" class="text-danger"></span>
                                <span asp-validation-for="ChargesPerVisit" class="text-danger"></span>
                                <span asp-validation-for="Experience" class="text-danger"></span>

                                <div class="form-group">
                                    <input asp-for="Salary" type="text" placeholder="Salary in Rupees" style="width:221px;" />
                                    <input asp-for="ChargesPerVisit" type="text" placeholder="Charges per visit in Rupees" style="width:227px;" />
                                    <input asp-for="Experience" type="text" placeholder="Experience (0-5)" style="width:229px;" />
                                </div>

                                <!-- Department, Qualification, Specialization -->
                                <span asp-validation-for="DepartmentId" class="text-danger"></span>

                                <div class="form-group">
                                    <select asp-for="DepartmentId" style="width:228px; height:39px;">
                                        <option value="0">Select Department</option>
                                        <option value="1">Cardiology</option>
                                        <option value="2">Orthopaedics</option>
                                        <option value="3">ENT</option>
                                        <option value="4">Physiotherapy</option>
                                        <option value="5">Neurology</option>
                                    </select>
                                    <input asp-for="Qualification" type="text" placeholder="Qualification" style="width:228px;" />
                                    <input asp-for="Specialization" type="text" placeholder="Specialization" style="width:228px;" />
                                </div>

                                <!-- Address -->
                                <div class="form-group">
                                    <input asp-for="Address" type="text" class="form-username form-control" placeholder="Address" />
                                </div>

                                <!-- Gender -->
                                <div class="form-group">
                                    <input type="radio" name="Gender" id="Male" value="M" checked="checked" /> Male
                                    <input type="radio" name="Gender" id="Female" value="F" /> Female
                                </div>

                                <button type="submit" class="btn btn-primary">Add</button>
                            </form>
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
                        <p style="color:darkslategrey">
                            If you have any query, please feel free to contact us.
                            <i class="fa fa-smile-o"></i>
                        </p>
                    </div>
                </div>
            </div>
        </footer>

        <!-- Javascript -->
        <script src="/assets/js/jquery-1.11.1.min.js"></script>
        <script src="/assets/bootstrap/js/bootstrap.min.js"></script>
        <script src="/assets/js/jquery.backstretch.min.js"></script>
        <script src="/assets/js/scripts.js"></script>

    </div>
</body>
</html>
