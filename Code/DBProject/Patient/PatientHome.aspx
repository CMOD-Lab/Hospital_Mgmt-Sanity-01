@page "/Patient/PatientHome"
@model DBProject.Patient.PatientHomeModel
@{
    ViewData["Title"] = "Patient's Home";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@

@section head {
    <title>Patient's Home</title>
}

<div style="background-image:url(/assets/img/backgrounds/PatientHome.jpg); background-position:center; background-size:20px">

    <br />
    <h1><strong style="margin:37%">Your Information</strong></h1>
    <br /><br />

    <div style="margin-left: 70px">

        <h4><strong>Name: </strong></h4>
        <span style="font-weight:bold; font-size:medium;">@Model.PName</span>
        <br /><br />

        <h4><strong>Phone: </strong></h4>
        <span style="font-weight:bold; font-size:medium;">@Model.PPhone</span>
        <br /><br />

        <h4><strong>Birth Date: </strong></h4>
        <span style="font-weight:bold; font-size:medium;">@Model.PBirthDate</span>
        <br /><br />

        <h4><strong>Age: </strong></h4>
        <span style="font-weight:bold; font-size:medium;">@Model.PatientAge</span>
        <br /><br />

        <h4><strong>Gender:</strong></h4>
        <span style="font-weight:bold; font-size:medium;">@Model.PGender</span>
        <br /><br />

        <h4><strong>Address: </strong></h4>
        <span style="font-weight:bold; font-size:medium;">@Model.PAddress</span>
        <br /><br />

    </div>

</div>
