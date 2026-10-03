<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (TreatmentHistory.aspx) was the Web Forms page for Treatment History.
    It has been migrated to ASP.NET Core MVC:

      • TreatmentHistory.aspx    → Views/Patient/TreatmentHistory.cshtml (Razor View)
      • TreatmentHistory.aspx.cs → Controllers/TreatmentHistoryController.cs (MVC Controller)

    Web Forms patterns removed / replaced (cr-dotnet-0026):
      Line 1 – <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                        AutoEventWireup="true" CodeBehind="TreatmentHistory.aspx.cs"
                        Inherits="DBProject.TreatmentHistory" %>
               → @{ Layout = "~/Views/Shared/_PatientLayout.cshtml"; }
      <asp:Content ContentPlaceHolderID="head">       → @section head { ... }
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → @RenderBody() in layout
      asp:Label ID="THistory"                         → @Model.StatusMessage
      asp:GridView ID="THistoryGrid"                  → HTML table from Model.TreatmentData (async)
      asp:TemplateField / asp:Label ID="lblRowNumber" → @(i + 1) in table cell

    cr-dotnet-1034 – Async GridView Data Binding with RDS via Entity Framework Core:
      Line 29: <asp:GridView ID="THistoryGrid" ... >
               Synchronous THistoryGrid.DataSource = DT; THistoryGrid.DataBind() in code-behind
               → Replaced with async Task-based data loading in TreatmentHistoryController.IndexAsync()
                 using getTreatmentHistory_Async() DAL method backed by Amazon RDS Proxy.
                 Prevents thread pool exhaustion under cloud load; enables auto-scaling.

      Line 55: <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
               Synchronous GridView row-number label binding
               → Replaced with @(i + 1) in async-rendered HTML table in
                 Views/Patient/TreatmentHistory.cshtml (no server-side data binding).

    The active Razor view is at Views/Patient/TreatmentHistory.cshtml.
    The active MVC controller is at Controllers/TreatmentHistoryController.cs.
--%>
