@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true" CodeBehind="DoctorProfile.aspx.cs"
              Inherits="DBProject.DoctorProfile" %>
              replaced with Razor Page directive and layout reference (occurrence 4)

   The Web Forms <%@ Page %> directive, <asp:Content>, and <asp:Label> / <asp:Button>
   server controls have been replaced with standard Razor Page syntax,
   enabling stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.DoctorProfileModel
@{
    ViewData["Title"] = "Doctor's Profile";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Doctor's Profile</title>
}

<!------------------Styling---------------->
<link rel="stylesheet" href="/assets/css/grid-view.css"/>

<div style="background-image:url(/assets/img/backgrounds/PatientHome.jpg); background-position:center; background-size:20px">

    <br />
    <h1><strong style="margin:37%">Doctor's Profile</strong></h1>
    <br /><br />

    <div style="margin-left: 70px">

        @* Replaces <asp:Label ID="DName" runat="server"> *@
        <h4><strong>Name: </strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DName</p>
        <br /><br />

        @* Replaces <asp:Label ID="DPhone" runat="server"> *@
        <h4><strong>Phone: </strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DPhone</p>
        <br /><br />

        @* Replaces <asp:Label ID="DQualification" runat="server"> *@
        <h4><strong>Qualification:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DQualification</p>
        <br /><br />

        @* Replaces <asp:Label ID="DSpecialization" runat="server"> *@
        <h4><strong>Specialization:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DSpecialization</p>
        <br /><br />

        @* Replaces <asp:Label ID="DWork" runat="server"> *@
        <h4><strong>Work Experience:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DWork</p>
        <br /><br />

        @* Replaces <asp:Label ID="DAge" runat="server"> *@
        <h4><strong>Age: </strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DAge</p>
        <br /><br />

        @* Replaces <asp:Label ID="DGender" runat="server"> *@
        <h4><strong>Gender:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DGender</p>
        <br /><br />

        @* Replaces <asp:Label ID="DDept" runat="server"> *@
        <h4><strong>Department:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DDept</p>
        <br /><br />

        @* Replaces <asp:Label ID="DCharges" runat="server"> *@
        <h4><strong>Charges Per Appointment:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DCharges</p>
        <br /><br />

        @* Replaces <asp:Label ID="DRI" runat="server"> *@
        <h4><strong>Repute Index:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DRI</p>
        <br /><br />

        @* Replaces <asp:Label ID="DPT" runat="server"> *@
        <h4><strong>Number of Patients Treated:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.DPT</p>
        <br /><br />

        @* Replaces <asp:Button ID="AppointmentB" runat="server" OnClick="RedirectToAppointmentTaker"> *@
        <a href="/Patient/AppointmentTaker" class="btn btn-default" style="font-weight:bold;">Take Appointment</a>

    </div>

</div>
