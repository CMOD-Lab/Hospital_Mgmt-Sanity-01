@*
    Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
    Rule cr-dotnet-0026: Web Forms Usage – replaced <%@ Page %> directive (line 1)
    and all <asp:*> server controls with Razor Pages @page / @model directives
    and standard HTML elements bound to PageModel properties.
    Enables cloud-native, horizontally-scalable deployment on AWS
    (Linux containers / Elastic Beanstalk / ECS/Fargate).
*@
@page "/Doctor/DoctorHome"
@model DBProject.Pages.Doctor.DoctorHomeModel
@{
    ViewData["Title"] = "Doctor's Home";
    Layout = "~/Pages/Doctor/_DoctorLayout.cshtml";
}

@if (!string.IsNullOrEmpty(Model.ErrorMessage))
{
    <div class="alert alert-danger">@Model.ErrorMessage</div>
}

<div style="background-image:url(/assets/img/backgrounds/PatientHome.jpg); background-position:center; background-size:20px; margin-left:50px">

    <h1>Your Profile</h1>
    <br />
    <h3>Name: <strong>@Model.DoctorName</strong><br /><br /></h3>
    <h4>Phone: <strong>@Model.Phone</strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Address: <strong>@Model.Address</strong></h4>
    <br />
    <h4>BirthDate: <strong>@Model.BirthDate</strong></h4>
    <br />
    <br />
    <h4>Gender: <strong>@Model.Gender</strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Department No: <strong>@Model.DepartmentNo</strong></h4>
    <br />
    <br />
    <h4>Charges Per Visit: <strong>@Model.ChargesPerVisit</strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Monthly Salary: <strong>@Model.MonthlySalary</strong></h4>
    <br />
    <br />
    <h4>Repute Index: <strong>@Model.ReputeIndex</strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Patients Treated: <strong>@Model.PatientsTreated</strong></h4>
    <br />
    <br />
    <h4>Qualification: <strong>@Model.Qualification</strong> <br /><br />
        Specialization: <strong>@Model.Specialization</strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</h4>
    <h4>Work Experience: <strong>@Model.WorkExperience</strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Status: <strong>@Model.Status</strong></h4>
    <br />
</div>
