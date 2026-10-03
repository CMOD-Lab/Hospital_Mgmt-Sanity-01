<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (DoctorProfile.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Original Web Forms directive (Line 1 – removed):
      <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
               AutoEventWireup="true" CodeBehind="DoctorProfile.aspx.cs"
               Inherits="DBProject.DoctorProfile" %>

    Migration mapping:
      DoctorProfile.aspx      → Views/Patient/DoctorProfile.cshtml  (Razor View)
      DoctorProfile.aspx.cs   → Controllers/DoctorProfileController.cs (MVC Controller)
      (new)                   → Models/DoctorProfileViewModel.cs (ViewModel)

    Web Forms server controls replaced:
      <asp:Content>                    → Razor @section / @RenderBody()
      <asp:Label ID="DName">           → @Model.Name
      <asp:Label ID="DPhone">          → @Model.Phone
      <asp:Label ID="DQualification">  → @Model.Qualification
      <asp:Label ID="DSpecialization"> → @Model.Specialization
      <asp:Label ID="DWork">           → @Model.WorkExperience
      <asp:Label ID="DAge">            → @Model.Age
      <asp:Label ID="DGender">         → @Model.Gender
      <asp:Label ID="DDept">           → @Model.Department
      <asp:Label ID="DCharges">        → @Model.ChargesPerVisit
      <asp:Label ID="DRI">             → @Model.ReputeIndex
      <asp:Label ID="DPT">             → @Model.PatientsTreated
      <asp:Button OnClick="RedirectToAppointmentTaker"> → <a asp-controller="AppointmentTaker" asp-action="Index">

    Active implementation: Views/Patient/DoctorProfile.cshtml
--%>
