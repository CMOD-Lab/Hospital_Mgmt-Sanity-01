<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (CurrentAppointment.aspx) was an ASP.NET Web Forms page.
    It has been migrated to ASP.NET Core MVC Razor View:

      • CurrentAppointment.aspx    → Views/Patient/CurrentAppointment.cshtml  (Razor View)
      • CurrentAppointment.aspx.cs → Controllers/CurrentAppointmentController.cs (MVC Controller)
      • (new)                      → Models/CurrentAppointmentViewModel.cs   (ViewModel)

    Web Forms patterns removed / replaced:
      Line 1 – <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                        AutoEventWireup="true" CodeBehind="CurrentAppointment.aspx.cs"
                        Inherits="DBProject.CurrentAppointment" %>
               → @{ Layout = "~/Views/Shared/_PatientLayout.cshtml"; }
      <asp:Content ContentPlaceHolderID="head">        → @section head { ... }
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → main content block (@RenderBody)
      asp:Label ID="Appointment"                       → @Model.AppointmentMessage display
      asp:Label ID="ADoctor"                           → @Model.DoctorMessage display
      asp:Label ID="ATimings"                          → @Model.TimingsMessage display

    The active Razor view is at Views/Patient/CurrentAppointment.cshtml.
--%>
