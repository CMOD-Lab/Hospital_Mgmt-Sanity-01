@page "/Admin/DoctorRegistrationForm"
@model DBProject.Admin.DoctorRegistrationFormModel
@{
    ViewData["Title"] = "Doctor Registration";
    Layout = "~/Admin/_AdminLayout.cshtml";
}

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head>
    <title>@ViewData["Title"]</title>

<style type="text/css">
    html
    {
      background-image:url("/assets/Doctor.jpg");   
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

    .grad{
        background: linear-gradient(to right, rgba(30, 160, 130 , 0.15),rgba(0, 148, 255 , 1)); 
        border-radius:8px;
    }
 
    #space
    {
        padding-bottom:50px;
    }
</style>

</head>
<body>
    <form method="post">
        <div id="myclass">
        
        <link rel="stylesheet" href="http://fonts.googleapis.com/css?family=Roboto:400,100,300,500"/>
        <link rel="stylesheet" href="/assets/bootstrap/css/bootstrap.min.css"/>
        <link rel="stylesheet" href="/assets/font-awesome/css/font-awesome.min.css"/>
        <link rel="stylesheet" href="/assets/css/form-elements.css"/>
        <link rel="stylesheet" href="/assets/css/style.css"/>

        <!-- Top content -->
        <div class="top-content">
            <div class="inner-bg">
                <div class="container grad">
                    <div class="row">
                        <div class="col-sm-8 col-sm-offset-2 text">
                            <h1><strong>Medix4 Health Care</strong> Doctor Registration</h1>
                            <div class="description">
                                <p>
                                    This is a free <strong>"Doctor registeration form"</strong> for Medix4 health Care. 
                                    Fill out the information of the Doctor to Register 
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="container myclass">
            <div class="row">
                <div class="col-sm-2"></div>
                <div class="col-sm-8">
                    <div class="form-box" id="spaces">
                        <div class="form-top">
                            <div class="form-top-left">
                                <h3>Sign up now</h3>
                                @if (Model.IsSuccess)
                                {
                                    <span style="color:blue; font-weight:bold; font-size:large">@Model.Message</span>
                                }
                            </div>
                        </div>
                        <div class="form-bottom">
                            <div class="form-group">
                                <span asp-validation-for="Name" class="text-danger"></span>
                                <input asp-for="Name" type="text" class="form-username form-control" placeholder="Name" />
                            </div>
                            <div class="form-group">
                                <span asp-validation-for="BirthDate" class="text-danger"></span>
                                <input asp-for="BirthDate" type="text" class="form-username form-control" placeholder="Birth Date (mm/dd/yyyy)" />
                            </div>
                            <div class="form-group">
                                <span asp-validation-for="Email" class="text-danger"></span>
                                @if (!string.IsNullOrEmpty(Model.EmailError))
                                {
                                    <span class="text-danger">@Model.EmailError</span>
                                }
                                <input asp-for="Email" type="text" class="form-username form-control" placeholder="Email : person@example.com" />
                            </div>
                            <div class="form-group">
                                <span asp-validation-for="Password" class="text-danger"></span>
                                <input asp-for="Password" type="password" class="form-username form-control" placeholder="Enter New Password" />
                            </div>
                            <div class="form-group">
                                <span asp-validation-for="ConfirmPassword" class="text-danger"></span>
                                <input asp-for="ConfirmPassword" type="password" class="form-username form-control" placeholder="Confirm Password" />
                            </div>
                            <div class="form-group">
                                <span asp-validation-for="Phone" class="text-danger"></span>
                                <input asp-for="Phone" type="text" class="form-username form-control" placeholder="Phone Number" />
                            </div>
                            <div class="form-group">
                                <span asp-validation-for="Salary" class="text-danger"></span>
                                <input asp-for="Salary" type="text" placeholder="Salary in Rupees" style="width:221px" />
                                <span asp-validation-for="ChargesPerVisit" class="text-danger"></span>
                                <input asp-for="ChargesPerVisit" type="text" placeholder="Charges_per_visit in Rupees" style="width:227px" />
                                <span asp-validation-for="Exp" class="text-danger"></span>
                                <input asp-for="Exp" type="text" placeholder="Experience (0-5)" style="width:229px" />
                            </div>
                            @if (!string.IsNullOrEmpty(Model.DepartmentError))
                            {
                                <span class="text-danger">@Model.DepartmentError</span>
                            }
                            <div class="form-group">
                                <select asp-for="Department" style="width:228px; height:39px">
                                    <option value="0">Select Depatment</option>
                                    <option value="1">Cardiology</option>
                                    <option value="2">Orthopaedics</option>
                                    <option value="3">ENT</option>
                                    <option value="4">Physiotherapy</option>
                                    <option value="5">Neurology</option>
                                </select>
                                <input asp-for="Qualification" type="text" placeholder="Qualification" style="width:228px" />
                                <input asp-for="Spec" type="text" placeholder="Specialization" style="width:228px" />
                            </div>
                            <div class="form-group">
                                <input asp-for="Address" class="form-username form-control" type="text" placeholder="Address" />
                            </div>
                            <div class="form-group">
                                <input type="radio" name="Gender" id="Male" value="M" checked="checked" /> Male
                                <input type="radio" name="Gender" id="Female" value="F" /> Female
                            </div>

                            <button type="submit" class="btn btn-primary">Add</button>
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

        </div>
    </form>
</body>
</html>
