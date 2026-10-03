<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (BillsHistory.aspx) was an ASP.NET Web Forms page.
    It has been migrated to ASP.NET Core MVC Razor View:

      • BillsHistory.aspx    → Views/Patient/BillsHistory.cshtml  (Razor View)
      • BillsHistory.aspx.cs → Controllers/BillsHistoryController.cs (MVC Controller)
      • (new)                → Models/BillsHistoryViewModel.cs   (ViewModel)

    Web Forms patterns removed / replaced:
      Line 1 – <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                        AutoEventWireup="true" CodeBehind="BillsHistory.aspx.cs"
                        Inherits="DBProject.BillsHistory" %>
               → @{ Layout = "~/Views/Shared/_PatientLayout.cshtml"; }
      <asp:Content ContentPlaceHolderID="head">        → @section head { ... }
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → main content block (@RenderBody)
      asp:Label ID="BHistory"                          → @Model.StatusMessage display
      asp:GridView ID="BHistoryGrid"                   → HTML table rendered from Model.BillHistoryData
        asp:TemplateField HeaderText="No."             → row-number column (loop index + 1)
        asp:Label ID="lblRowNumber"                    → @(i + 1) in table cell

    MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
      Line 25 – asp:GridView ID="BHistoryGrid" with synchronous DataBind()
              → Replaced with async Task<IActionResult> Index() in BillsHistoryController.cs
                using async EF Core data access (getBillHistory_Async) connected to
                Amazon RDS, preventing thread pool exhaustion under cloud load.

      Line 51 – asp:TemplateField HeaderText="No." / asp:Label ID="lblRowNumber"
              → Replaced with row-number column rendered via Razor loop index in
                Views/Patient/BillsHistory.cshtml. No synchronous DataBind() –
                non-blocking async pattern enables efficient auto-scaling in cloud deployments.

    The active Razor view is at Views/Patient/BillsHistory.cshtml.
--%>
