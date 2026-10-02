@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#"
              MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true"
              CodeBehind="PatientNotifications.aspx.cs"
              Inherits="DBProject.PatientNotifications" %>
              replaced with Razor Page @page / @model directives.

   The <%@ Page %> directive, <asp:Content>, and all <asp:Label> server
   controls have been replaced with standard Razor Page constructs.
   The Master Page reference is replaced by Layout = "_PatientLayout".
   Enables stateless, cloud-native deployment on AWS
   (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.PatientNotificationsModel
@{
    ViewData["Title"] = "Notifications";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Notifications</title>
}

<h1><strong style="margin:37%">Notifications</strong></h1>
<br /><br />

<div style="margin-left: 70px">

    @if (!string.IsNullOrEmpty(Model.NotifyMessage))
    {
        <span style="font-weight:bold; font-size:medium">@Model.NotifyMessage</span>
        <br /><br />
    }

    @if (!string.IsNullOrEmpty(Model.NDoctorMessage))
    {
        <span style="font-weight:bold; font-size:medium">@Model.NDoctorMessage</span>
        <br /><br />
    }

    @if (!string.IsNullOrEmpty(Model.NTimingsMessage))
    {
        <span style="font-weight:bold; font-size:medium">@Model.NTimingsMessage</span>
        <br /><br />
    }

</div>
