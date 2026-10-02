@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true" CodeBehind="PatientHome.aspx.cs"
              Inherits="DBProject.PatientHome" %>
              replaced with Razor Page directive and layout reference (occurrence 5)

   The Web Forms <%@ Page %> directive, <asp:Content>, and <asp:Label>
   server controls have been replaced with standard Razor Page syntax,
   enabling stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.PatientHomeModel
@{
    ViewData["Title"] = "Patient's Home";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Patient's Home</title>
}

<div style="background-image:url(/assets/img/backgrounds/PatientHome.jpg); background-position:center; background-size:20px">

    <br />
    <h1><strong style="margin:37%">Your Information</strong></h1>
    <br /><br />

    @* Show error alert if patient info could not be retrieved *@
    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
    {
        <div class="alert alert-danger" role="alert">
            @Model.ErrorMessage
        </div>
    }

    <div style="margin-left: 70px">

        @* Replaces <asp:Label ID="PName" runat="server"> *@
        <h4><strong>Name: </strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.PName</p>
        <br /><br />

        @* Replaces <asp:Label ID="PPhone" runat="server"> *@
        <h4><strong>Phone: </strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.PPhone</p>
        <br /><br />

        @* Replaces <asp:Label ID="PBirthDate" runat="server"> *@
        <h4><strong>Birth Date: </strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.PBirthDate</p>
        <br /><br />

        @* Replaces <asp:Label ID="PatientAge" runat="server"> *@
        <h4><strong>Age: </strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.PatientAge</p>
        <br /><br />

        @* Replaces <asp:Label ID="PGender" runat="server"> *@
        <h4><strong>Gender:</strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.PGender</p>
        <br /><br />

        @* Replaces <asp:Label ID="PAddress" runat="server"> *@
        <h4><strong>Address: </strong></h4>
        <p style="font-weight:bold; font-size:medium;">@Model.PAddress</p>
        <br /><br />

    </div>

</div>
