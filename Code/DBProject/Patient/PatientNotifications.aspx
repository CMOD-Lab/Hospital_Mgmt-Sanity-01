<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (PatientNotifications.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Replacement:
      PatientNotifications.aspx    → Views/Patient/PatientNotifications.cshtml
      PatientNotifications.aspx.cs → Controllers/PatientNotificationsController.cs

    Web Forms patterns removed / replaced (cr-dotnet-0026):
      Line 1: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                       AutoEventWireup="true" CodeBehind="PatientNotifications.aspx.cs"
                       Inherits="DBProject.PatientNotifications" %>
              → (no directive; standard Razor view file)

      <asp:Content ContentPlaceHolderID="head">       → @section head { ... }
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → main content block (@RenderBody)

      asp:Label ID="Notify"   → @Model.NotifyMessage
      asp:Label ID="NDoctor"  → @Model.DoctorMessage
      asp:Label ID="NTimings" → @Model.TimingsMessage

    Active Razor view is at Views/Patient/PatientNotifications.cshtml.
    Active MVC controller is at Controllers/PatientNotificationsController.cs.
--%>
