<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (PreviousHistory.aspx) has been migrated to ASP.NET Core MVC.

    Replacement files:
      • Views/Doctor/PreviousHistory.cshtml  (Razor View)
      • Controllers/PreviousHistoryController.cs (MVC Controller)
      • Models/PreviousHistoryViewModel.cs   (ViewModel)

    Web Forms patterns removed / replaced:
      Line 1 – <%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/DoctorMaster.Master"
                        AutoEventWireup="true" CodeBehind="PreviousHistory.aspx.cs"
                        Inherits="DBProject.Doctor.PreviousHistory" %>
               → @{ Layout = "~/Views/Shared/_DoctorLayout.cshtml"; } in Razor View

      <asp:Content ContentPlaceHolderID="head">  → @section head { ... } in Razor View
      <asp:Content ContentPlaceHolderID="Cp1">   → main content block in Razor View
      asp:Label ID="PHistory"                    → @Model.ErrorMessage alert div

      Line 24 – asp:GridView ID="PHistoryGrid"
        asp:TemplateField HeaderText="No."       → row-number column (loop index + 1)
        asp:Label ID="lblRowNumber"              → @(i + 1) in table cell

    MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
      Line 24 – asp:GridView ID="PHistoryGrid" with synchronous DataBind()
              → Replaced with async Task<IActionResult> Index() in PreviousHistoryController.cs
                using async EF Core data access (getPHistory_Async) connected to
                Amazon RDS, preventing thread pool exhaustion under cloud load.

      Line 50 – PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind();
              → Replaced with async ViewModel population:
                  var (status, dt) = await objmyDAl.getPHistory_Async(id);
                  vm.HistoryData = dt;
                No synchronous DataBind() – non-blocking async pattern enables
                efficient auto-scaling in cloud deployments.

    This .aspx file is retained for reference only.
    The active view is Views/Doctor/PreviousHistory.cshtml.
--%>
