<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (ViewDoctors.aspx) was the Web Forms page for viewing doctors.
    It has been migrated to ASP.NET Core MVC:

      • ViewDoctors.aspx    → Views/Patient/ViewDoctors.cshtml (Razor View)
      • ViewDoctors.aspx.cs → Controllers/ViewDoctorsController.cs (MVC Controller)

    Web Forms patterns removed / replaced (cr-dotnet-0026):
      Line 1 – <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                        AutoEventWireup="true" CodeBehind="ViewDoctors.aspx.cs"
                        Inherits="DBProject.ViewDoctors" %>
               → @{ Layout = "~/Views/Shared/_PatientLayout.cshtml"; }
      <asp:Content ContentPlaceHolderID="head">       → @section head { ... }
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → @RenderBody() in layout
      asp:Label ID="TDoctor"                          → @Model.StatusMessage
      asp:GridView ID="TDoctorGrid"                   → HTML table from Model.DoctorData (async)
        AutoGenerateSelectButton="True"               → Select button column in each row
        OnRowCommand="TDoctorGrid_RowCommand"          → POST to SelectDoctor action
      asp:TemplateField / asp:Label ID="lblRowNumber" → @(i + 1) in table cell

    cr-dotnet-1034 – Async GridView Data Binding with RDS via Entity Framework Core:
      Line 28: <asp:GridView ID="TDoctorGrid" ... AutoGenerateSelectButton="True"
                             OnRowCommand="TDoctorGrid_RowCommand" ...>
               Synchronous TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind() in code-behind
               → Replaced with async Task-based data loading in ViewDoctorsController.IndexAsync()
                 using getDeptDoctorInfo_Async() DAL method backed by Amazon RDS Proxy.
                 Prevents thread pool exhaustion under cloud load; enables auto-scaling.

      Line 57: <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
               Synchronous GridView row-number label binding
               → Replaced with @(i + 1) in async-rendered HTML table in
                 Views/Patient/ViewDoctors.cshtml (no server-side data binding).

    The active Razor view is at Views/Patient/ViewDoctors.cshtml.
    The active MVC controller is at Controllers/ViewDoctorsController.cs.
--%>
