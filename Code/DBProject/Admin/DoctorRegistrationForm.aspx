@*
    DoctorRegistrationForm.cshtml — ASP.NET Core Razor Page (replaces DoctorRegistrationForm.aspx)
    Rule cr-dotnet-0026: Web Forms Usage → Migrate to ASP.NET Core MVC/Razor Pages

    Migration notes:
    - <%@ Page %> directive removed; replaced with @page / @model Razor directives.
    - <form id="form1" runat="server"> replaced with a standard HTML <form> using
      asp-page POST handler and ASP.NET Core Tag Helpers.
    - <asp:TextBox>, <asp:DropDownList>, <asp:RadioButton> server controls replaced
      with standard HTML input elements bound to the PageModel's InputModel.
    - <asp:RequiredFieldValidator>, <asp:RegularExpressionValidator>,
      <asp:CompareValidator>, <asp:RangeValidator>, <asp:CustomValidator> replaced
      with ASP.NET Core model validation (DataAnnotations + Tag Helpers).
    - <asp:Label ID="Msg"> replaced with a TempData / ModelState message rendered inline.
    - All business logic (ValidateDoctorEmail, DoctorRegister, DepartmentValidate,
      flushInformation) is preserved in the PageModel.
    - <head runat="server"> replaced with a plain <head>; styles moved inline.
*@
@page
@model DBProject.Pages.Admin.DoctorRegistrationFormModel
@{
    ViewData["Title"] = "Doctor Registration";
    Layout = "~/Admin/_AdminLayout.cshtml";
}

@section Head {
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
}

<div id="myclass">

    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:400,100,300,500" />
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

                            @* Replaces <asp:Label ID="Msg"> — shows success or error message *@
                            @if (Model.SuccessMessage != null)
                            {
                                <p style="color:blue; font-weight:bold; font-size:large;">@Model.SuccessMessage</p>
                            }
                            @if (!ViewData.ModelState.IsValid)
                            {
                                <div asp-validation-summary="ModelOnly" class="text-danger"></div>
                            }
                        </div>
                    </div>

                    <div class="form-bottom">
                        <form method="post">
                            @Html.AntiForgeryToken()

                            <!-- Name -->
                            <div class="form-group">
                                <span asp-validation-for="Input.Name" class="text-danger"></span>
                                <input asp-for="Input.Name" type="text"
                                       class="form-username form-control"
                                       placeholder="Name" />
                            </div>

                            <!-- Birth Date -->
                            <div class="form-group">
                                <span asp-validation-for="Input.BirthDate" class="text-danger"></span>
                                <input asp-for="Input.BirthDate" type="text"
                                       class="form-username form-control"
                                       placeholder="Birth Date (mm/dd/yyyy)" />
                            </div>

                            <!-- Email -->
                            <div class="form-group">
                                <span asp-validation-for="Input.Email" class="text-danger"></span>
                                <input asp-for="Input.Email" type="text"
                                       class="form-username form-control"
                                       placeholder="Email : person@example.com" />
                            </div>

                            <!-- Password -->
                            <div class="form-group">
                                <span asp-validation-for="Input.Password" class="text-danger"></span>
                                <input asp-for="Input.Password" type="password"
                                       class="form-username form-control"
                                       placeholder="Enter New Password" />
                            </div>

                            <!-- Confirm Password -->
                            <div class="form-group">
                                <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
                                <input asp-for="Input.ConfirmPassword" type="password"
                                       class="form-username form-control"
                                       placeholder="Confirm Password" />
                            </div>

                            <!-- Phone -->
                            <div class="form-group">
                                <span asp-validation-for="Input.Phone" class="text-danger"></span>
                                <input asp-for="Input.Phone" type="text"
                                       class="form-username form-control"
                                       placeholder="Phone Number" />
                            </div>

                            <!-- Salary / Charges / Experience -->
                            <span asp-validation-for="Input.Salary" class="text-danger"></span>
                            <span asp-validation-for="Input.ChargesPerVisit" class="text-danger"></span>
                            <span asp-validation-for="Input.Experience" class="text-danger"></span>

                            <div class="form-group">
                                <input asp-for="Input.Salary" type="text"
                                       placeholder="Salary in Rupees" style="width:221px;" />
                                <input asp-for="Input.ChargesPerVisit" type="text"
                                       placeholder="Charges per visit in Rupees" style="width:227px;" />
                                <input asp-for="Input.Experience" type="text"
                                       placeholder="Experience (0-5)" style="width:229px;" />
                            </div>

                            <!-- Department / Qualification / Specialization -->
                            <span asp-validation-for="Input.Department" class="text-danger"></span>

                            <div class="form-group">
                                <select asp-for="Input.Department" style="width:228px; height:39px;">
                                    <option value="0">Select Department</option>
                                    <option value="1">Cardiology</option>
                                    <option value="2">Orthopaedics</option>
                                    <option value="3">ENT</option>
                                    <option value="4">Physiotherapy</option>
                                    <option value="5">Neurology</option>
                                </select>
                                <input asp-for="Input.Qualification" type="text"
                                       placeholder="Qualification" style="width:228px;" />
                                <input asp-for="Input.Specialization" type="text"
                                       placeholder="Specialization" style="width:228px;" />
                            </div>

                            <!-- Address -->
                            <div class="form-group">
                                <input asp-for="Input.Address" type="text"
                                       class="form-username form-control"
                                       placeholder="Address" />
                            </div>

                            <!-- Gender -->
                            <div class="form-group">
                                <label>
                                    <input asp-for="Input.Gender" type="radio" value="M" checked="checked" /> Male
                                </label>
                                <label>
                                    <input asp-for="Input.Gender" type="radio" value="F" /> Female
                                </label>
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
