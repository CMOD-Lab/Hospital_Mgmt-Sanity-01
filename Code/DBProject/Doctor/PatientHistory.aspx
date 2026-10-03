<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (PatientHistory.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Replacement file:
      • Views/Doctor/PatientHistory.cshtml  – Razor View (replaces this .aspx)

    Web Forms page directives and server controls replaced:
      Line 1: <%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/doctormaster.Master"
                       AutoEventWireup="true" CodeBehind="PatientHistory.aspx.cs"
                       Inherits="doctor.patienthistory" %>
              → Standard Razor View with @{ Layout = "~/Views/Shared/_DoctorLayout.cshtml"; }

      <asp:Content ContentPlaceHolderID="head">   → @section head { <title>Patient History</title> }
      <asp:Content ContentPlaceHolderID="Cp1">    → main Razor content block

      Line 23 – asp:GridView ID="patientsgrid"
        AutoGenerateSelectButton="True"           → HTML table with per-row Select form
        OnRowCommand="patientsgrid_RowCommand"    → POST to /PatientHistory/Select
        DataSource / DataBind                     → Model.Patients (DataTable)

    MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
      Line 23 – asp:GridView ID="patientsgrid" with synchronous DataBind()
              → Replaced with async Task<IActionResult> Index() in PatientHistoryController.cs
                using async EF Core data access (search_patient_DAL_Async) connected to
                Amazon RDS, preventing thread pool exhaustion under cloud load.

      Line 41 – patientsgrid.DataSource = dt; patientsgrid.DataBind();
              → Replaced with async ViewModel population:
                  var (found, dt) = await objmydal.search_patient_DAL_Async(did);
                  vm.Patients = dt;
                No synchronous DataBind() – non-blocking async pattern enables
                efficient auto-scaling in cloud deployments.

    The active view is now Views/Doctor/PatientHistory.cshtml.
    The active controller is Controllers/PatientHistoryController.cs.
--%>
