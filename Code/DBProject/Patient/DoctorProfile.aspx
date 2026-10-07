@page "/Patient/DoctorProfile"
@model DBProject.Patient.DoctorProfileModel
@{
    ViewData["Title"] = "Doctor's Profile";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@

@section head {
    <title>Doctor's Profile</title>
}

@* Styling *@
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<div style="background-image:url(/assets/img/backgrounds/PatientHome.jpg); background-position:center; background-size:20px">

    <br />
    <h1><strong style="margin:37%">Doctor's Profile</strong></h1>
    <br /><br />

    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
    {
        <div class="alert alert-danger">@Model.ErrorMessage</div>
    }
    else
    {
        <div style="margin-left: 70px">

            <h4><strong>Name: </strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DName</p>
            <br /><br />

            <h4><strong>Phone: </strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DPhone</p>
            <br /><br />

            <h4><strong>Qualification:</strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DQualification</p>
            <br /><br />

            <h4><strong>Specialization:</strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DSpecialization</p>
            <br /><br />

            <h4><strong>Work Experience:</strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DWork</p>
            <br /><br />

            <h4><strong>Age: </strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DAge</p>
            <br /><br />

            <h4><strong>Gender:</strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DGender</p>
            <br /><br />

            <h4><strong>Department:</strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DDept</p>
            <br /><br />

            <h4><strong>Charges Per Appointment:</strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DCharges</p>
            <br /><br />

            <h4><strong>Repute Index:</strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DRI</p>
            <br /><br />

            <h4><strong>Number of Patients Treated:</strong></h4>
            <p style="font-weight:bold; font-size:medium;">@Model.DPT</p>
            <br /><br />

            <form method="post">
                @Html.AntiForgeryToken()
                <button type="submit" asp-page-handler="TakeAppointment" style="font-weight:bold;">Take Appointment</button>
            </form>

        </div>
    }

</div>
