@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Language="C#" AutoEventWireup="true"
              CodeBehind="SignUp.aspx.cs" Inherits="DBProject.SignUp" %>
              replaced with Razor Page @page / @model directives.

   The <%@ Page %> directive, <form runat="server">, and all <asp:TextBox>,
   <asp:Button> server controls have been replaced with standard Razor Page
   constructs using HTML form elements with asp-page-handler tag helpers.
   Server-side alert() calls (Response.Write) are replaced with TempData
   messages rendered via JavaScript on page load.
   The <head runat="server"> has been replaced with a plain <head>.
   Enables stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.SignUpModel
@{
    ViewData["Title"] = "MedicX 4 Health Care Login & Register";
}

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>MedicX 4 Health Care Login &amp; Register</title>

    <script type="text/javascript">

        //----------------------Function1-----------------------------//
        function validateEmail(Email) {
            if (Email == "") {
                alert("Email missing. Enter Email.");
                return false;
            }
            else if (Email.indexOf("@") == -1 || Email.indexOf(".com") == -1) {
                alert("Your email address seems incorrect. Please enter a new one.");
                return false;
            }
            else {
                var location = Email.indexOf("@");
                if (Email[0] == '@' || Email[location + 1] == '.') {
                    alert("Your email address seems incorrect. Please enter a new one.");
                    return false;
                }
                var emailPat = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                var EmailmatchArray = Email.match(emailPat);
                if (EmailmatchArray == null) {
                    alert("Your email address seems incorrect. Please enter a new one.");
                    return false;
                }
            }
            return true;
        }

        //----------------------Function2-----------------------------//
        function validateS() {
            var Name  = document.getElementById('sName').value;
            var Bdate = document.getElementById('sBirthDate').value;
            var Email = document.getElementById('sEmail').value;
            var phone = document.getElementById('phone').value;
            var pass  = document.getElementById('sPassword').value;
            var cpass = document.getElementById('scPassword').value;

            if (Name == "") {
                alert("Name missing. Enter Name.");
                return false;
            }

            var arrDbirth = Bdate.split("-");
            if (Bdate == "") {
                alert("Birth Date missing. Enter Birth Date.");
                return false;
            }
            else if ((Bdate == arrDbirth[0]) || (arrDbirth[0].length != 2) || arrDbirth[1].length != 2 || arrDbirth[2].length != 4 ||
                     !arrDbirth[0].match(/^[0-9]*$/) || !arrDbirth[1].match(/^[0-9]*$/) || !arrDbirth[2].match(/^[0-9]*$/) ||
                     Number(arrDbirth[0]) > 31 || Number(arrDbirth[1]) > 12) {
                alert("Birth Date Format Incorrect or out of Range.");
                return false;
            }

            if (!validateEmail(Email))
                return false;

            if (pass == "" || cpass == "") {
                alert("Password field is empty.");
                return false;
            }
            else if (pass != cpass) {
                alert("Passwords do not match.");
                return false;
            }

            if (phone.length != 11) {
                alert("Phone number should be of 11 digits.");
                return false;
            }

            var genderSelected = document.querySelector('input[name="Gender"]:checked');
            if (!genderSelected) {
                alert("Gender not selected.");
                return false;
            }

            return true;
        }

        //----------------------Function3-----------------------------//
        function validateL() {
            var Email    = document.getElementById('loginEmail').value;
            var Password = document.getElementById('loginPassword').value;

            if (!validateEmail(Email))
                return false;

            if (Password == "") {
                alert("Password missing. Enter Password.");
                return false;
            }
            return true;
        }

        //------------------------------------------------------------------//
        // Display server-side error messages (replaces Response.Write alert)
        //------------------------------------------------------------------//
        @if (TempData["ErrorMessage"] != null)
        {
            <text>
            window.onload = function () {
                alert('@Html.Raw(TempData["ErrorMessage"].ToString().Replace("'", "\\'"))');
            };
            </text>
        }

    </script>

    <!-- CSS -->
    <link rel="stylesheet" href="http://fonts.googleapis.com/css?family=Roboto:400,100,300,500" />
    <link rel="stylesheet" href="assets/bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" href="assets/font-awesome/css/font-awesome.min.css" />
    <link rel="stylesheet" href="assets/css/form-elements.css" />
    <link rel="stylesheet" href="assets/css/style.css" />

    <!-- Favicon and touch icons -->
    <link rel="shortcut icon" href="assets/ico/favicon.png" />
    <link rel="apple-touch-icon-precomposed" sizes="144x144" href="assets/ico/apple-touch-icon-144-precomposed.png" />
    <link rel="apple-touch-icon-precomposed" sizes="114x114" href="assets/ico/apple-touch-icon-114-precomposed.png" />
    <link rel="apple-touch-icon-precomposed" sizes="72x72" href="assets/ico/apple-touch-icon-72-precomposed.png" />
    <link rel="apple-touch-icon-precomposed" href="assets/ico/apple-touch-icon-57-precomposed.png" />

    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Javascript -->
    <script src="assets/js/jquery-1.11.1.min.js"></script>
    <script src="assets/bootstrap/js/bootstrap.min.js"></script>
    <script src="assets/js/jquery.backstretch.min.js"></script>
    <script src="assets/js/scripts.js"></script>
</head>

<body>

    <div>
        <!-- Top content -->
        <div class="top-content">
            <div class="inner-bg">
                <div class="container">

                    <div class="row">
                        <div class="col-sm-8 col-sm-offset-2 text">
                            <h1><strong>MedicX 4 Health Care</strong> Login &amp; Registration Form</h1>
                            <div class="description">
                                <p>
                                    This is a free <strong>"Login and Registration form"</strong> for Health Care Clinic.
                                </p>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <!-- ==================== LOGIN FORM ==================== -->
                        <div class="col-sm-5">
                            <div class="form-box">
                                <div class="form-top">
                                    <div class="form-top-left">
                                        <h3>Login to our Website</h3>
                                        <p>Enter Email and Password to log in:</p>
                                    </div>
                                    <div class="form-top-right">
                                        <i class="fa fa-key"></i>
                                    </div>
                                </div>

                                <div class="form-bottom">
                                    @* Replaces <form runat="server"> with Razor Page form using asp-page-handler *@
                                    <form method="post" asp-page-handler="Login" onsubmit="return validateL();">
                                        @Html.AntiForgeryToken()

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="loginEmail" runat="server" ...> *@
                                            <input type="text" id="loginEmail" name="LoginEmail"
                                                   class="form-username form-control" placeholder="Email"
                                                   value="@Model.LoginEmail" />
                                        </div>

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="loginPassword" runat="server" type="password" ...> *@
                                            <input type="password" id="loginPassword" name="LoginPassword"
                                                   class="form-username form-control" placeholder="Password" />
                                        </div>

                                        @* Replaces <asp:Button ID="loginUserName" runat="server" Text="Login" ...> *@
                                        <button type="submit" class="btn btn-primary">Login</button>
                                    </form>
                                </div>
                            </div>

                            <div class="social-login">
                                <h3>...or login with:</h3>
                                <div class="social-login-buttons">
                                    <a class="btn btn-link-1 btn-link-1-facebook" href="#">
                                        <i class="fa fa-facebook"></i> Facebook
                                    </a>
                                </div>
                            </div>
                        </div>

                        <div class="col-sm-1 middle-border"></div>
                        <div class="col-sm-1"></div>

                        <!-- ==================== SIGN-UP FORM ==================== -->
                        <div class="col-sm-5">
                            <div class="form-box">
                                <div class="form-top">
                                    <div class="form-top-left">
                                        <h3>Sign up now</h3>
                                        <p>Fill in the form below to get instant access:</p>
                                    </div>
                                    <div class="form-top-right">
                                        <i class="fa fa-pencil"></i>
                                    </div>
                                </div>

                                <div class="form-bottom">
                                    @* Replaces <form runat="server"> with Razor Page form using asp-page-handler *@
                                    <form method="post" asp-page-handler="Signup" onsubmit="return validateS();">
                                        @Html.AntiForgeryToken()

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="sName" runat="server" ...> *@
                                            <input type="text" id="sName" name="SName"
                                                   class="form-username form-control" placeholder="Name"
                                                   value="@Model.SName" />
                                        </div>

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="sBirthDate" runat="server" ...> *@
                                            <input type="text" id="sBirthDate" name="SBirthDate"
                                                   class="form-username form-control" placeholder="Birth Date (dd-mm-yyyy)"
                                                   value="@Model.SBirthDate" />
                                        </div>

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="sEmail" runat="server" ...> *@
                                            <input type="text" id="sEmail" name="SEmail"
                                                   class="form-username form-control" placeholder="Email : person@example.com"
                                                   value="@Model.SEmail" />
                                        </div>

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="sPassword" runat="server" type="password" ...> *@
                                            <input type="password" id="sPassword" name="SPassword"
                                                   class="form-username form-control" placeholder="Enter New Password" />
                                        </div>

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="scPassword" runat="server" type="password" ...> *@
                                            <input type="password" id="scPassword" name="ScPassword"
                                                   class="form-username form-control" placeholder="Confirm Password" />
                                        </div>

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="Phone" runat="server" ...> *@
                                            <input type="text" id="phone" name="Phone"
                                                   class="form-username form-control" placeholder="Phone Number (11 Digits)"
                                                   value="@Model.Phone" />
                                        </div>

                                        <div class="form-group">
                                            <input type="radio" name="Gender" value="M" checked="checked" /> Male
                                            <input type="radio" name="Gender" value="F" /> Female
                                        </div>

                                        <div class="form-group">
                                            @* Replaces <asp:TextBox ID="Address" TextMode="multiline" ...> *@
                                            <textarea id="address" name="Address"
                                                      class="form-username form-control" placeholder="Address"
                                                      rows="4" style="height:75px; width:410px;">@Model.Address</textarea>
                                        </div>

                                        @* Replaces <asp:Button Text="SignUp" runat="server" ...> *@
                                        <button type="submit" class="btn btn-primary">SignUp</button>
                                    </form>
                                </div>
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
                        <p>If you have any query, please feel free to contact us. <i class="fa fa-smile-o"></i></p>
                    </div>
                </div>
            </div>
        </footer>
    </div>

</body>
</html>
