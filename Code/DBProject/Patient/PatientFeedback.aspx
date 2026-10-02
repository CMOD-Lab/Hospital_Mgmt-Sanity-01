@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage

   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
              AutoEventWireup="true" CodeBehind="PatientFeedback.aspx.cs"
              Inherits="DBProject.PatientFeedback" %>
              replaced with Razor Page directive and layout reference (occurrence 9)

   The Web Forms <%@ Page %> directive, <asp:Content>, <asp:Label>,
   <asp:DropDownList>, and <asp:Button> server controls have been replaced
   with standard Razor Page syntax, enabling stateless, cloud-native deployment
   on AWS (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Patient.PatientFeedbackModel
@{
    ViewData["Title"] = "Feedback";
    Layout = "_PatientLayout";
}

@section Head {
    <title>Feedback</title>
}

<h1><strong style="margin:37%">Feedback</strong></h1>
<br /><br />

<div style="margin-left: 70px">

    @* Replaces <asp:Label ID="Feedback" runat="server"> *@
    @if (!string.IsNullOrEmpty(Model.FeedbackMessage))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.FeedbackMessage</p>
    }
    <br /><br />

    @* Replaces <asp:Label ID="FDoctor" runat="server"> *@
    @if (!string.IsNullOrEmpty(Model.FDoctorMessage))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.FDoctorMessage</p>
    }
    <br /><br />

    @* Replaces <asp:Label ID="FTimings" runat="server"> *@
    @if (!string.IsNullOrEmpty(Model.FTimingsMessage))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.FTimingsMessage</p>
    }
    <br /><br />

    @* Replaces <asp:Label ID="Message" runat="server" Visible="false"> and
       <asp:DropDownList ID="List">, <asp:Button ID="button1" OnClick="giveFeedback">,
       <asp:Label ID="F"> — shown only when a pending feedback exists *@
    @if (Model.ShowFeedbackForm)
    {
        <br /><br />
        <p style="font-weight:bold; font-size:medium;">
            Dear Patient, How was your treatment experience with our specialized Doctor on a rating of 1 - 5:
        </p>

        <div style="margin-left: 790px">
            <form method="post">
                @* Hidden field carries the appointment ID for the POST handler *@
                <input type="hidden" name="aID" value="@Model.PendingAppointmentId" />

                @* Replaces <asp:DropDownList ID="List"> *@
                <select name="rating" style="font-weight:bold;">
                    <option value="1">1</option>
                    <option value="2">2</option>
                    <option value="3">3</option>
                    <option value="4">4</option>
                    <option value="5">5</option>
                </select>

                <br /><br />

                @* Replaces <asp:Button ID="button1" OnClick="giveFeedback"> *@
                <button type="submit" style="font-weight:bold;">Give Feedback</button>

                <br /><br />

                @* Replaces <asp:Label ID="F" runat="server"> *@
                @if (!string.IsNullOrEmpty(Model.FeedbackResultMessage))
                {
                    <p style="font-weight:bold; font-size:medium;">@Model.FeedbackResultMessage</p>
                }
            </form>
        </div>
    }

    <br /><br />
</div>
