<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (AppointmentTaker.aspx) has been migrated to ASP.NET Core MVC.

    Replacement files:
      • Views/Patient/AppointmentTaker.cshtml  (Razor View)
      • Controllers/AppointmentTakerController.cs (MVC Controller)
      • Models/AppointmentTakerViewModel.cs   (ViewModel)

    Web Forms patterns removed / replaced:
      Line 1 – <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                        AutoEventWireup="true" CodeBehind="AppointmentTaker.aspx.cs"
                        Inherits="DBProject.AppointmentTaker" %>
               → @{ Layout = "~/Views/Shared/_PatientLayout.cshtml"; } in Razor View

      <asp:Content ContentPlaceHolderID="head">       → @section head { ... } in Razor View
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → main content block in Razor View
      asp:Label ID="PAppointment"                     → @Model.StatusMessage display
      asp:GridView ID="PAppointmentGrid"              → HTML table rendered from Model.FreeSlotsData
        AutoGenerateSelectButton="true"               → Select button column in each row
        OnRowCommand="PAppointmentGrid_RowCommand"    → POST to SelectSlot action
        asp:TemplateField HeaderText="No."            → row-number column (loop index + 1)
        asp:Label ID="lblRowNumber"                   → @(i + 1) in table cell

    MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
      Line 27 – asp:GridView ID="PAppointmentGrid" with synchronous DataBind()
              → Replaced with async Task<IActionResult> Index() in AppointmentTakerController.cs
                using async EF Core data access (getFreeSlots_Async) connected to
                Amazon RDS, preventing thread pool exhaustion under cloud load.

      Line 55 – asp:TemplateField HeaderText="No." / asp:Label ID="lblRowNumber"
              → Replaced with row-number column rendered via Razor loop index in
                Views/Patient/AppointmentTaker.cshtml. No synchronous DataBind() –
                non-blocking async pattern enables efficient auto-scaling in cloud deployments.

    This .aspx file is retained for reference only.
    The active view is Views/Patient/AppointmentTaker.cshtml.
--%>
