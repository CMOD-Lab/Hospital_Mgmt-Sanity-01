<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (AppointmentRequestSent.aspx) has been migrated to ASP.NET Core MVC.

    Replacement files:
      • Views/Patient/AppointmentRequestSent.cshtml  (Razor View)
      • Controllers/AppointmentRequestSentController.cs (MVC Controller)
      • Models/AppointmentRequestSentViewModel.cs   (ViewModel)

    Web Forms patterns removed / replaced:
      Line 1 – <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                        AutoEventWireup="true" CodeBehind="AppointmentRequestSent.aspx.cs"
                        Inherits="DBProject.AppointmentNotificationSent" %>
               → @{ Layout = "~/Views/Shared/_PatientLayout.cshtml"; } in Razor View

      <asp:Content ContentPlaceHolderID="head">       → @section head { ... } in Razor View
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → main content block in Razor View
      asp:Button OnClick="sendARequest"               → HTML <form> POST to SendRequest action
      asp:Label ID="Message"                          → @Model.Message display div

    This .aspx file is retained for reference only.
    The active view is Views/Patient/AppointmentRequestSent.cshtml.
--%>
