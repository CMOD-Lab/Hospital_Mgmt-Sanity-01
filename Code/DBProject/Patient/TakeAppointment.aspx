<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (TakeAppointment.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Replacement:
      TakeAppointment.aspx    → Views/Patient/TakeAppointment.cshtml
      TakeAppointment.aspx.cs → Controllers/TakeAppointmentController.cs

    Web Forms patterns removed / replaced (cr-dotnet-0026):
      Line 1: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                       AutoEventWireup="true" CodeBehind="TakeAppointment.aspx.cs"
                       Inherits="DBProject.TakeAppointment" %>
              → (no directive; standard Razor view file)

      <asp:Content ContentPlaceHolderID="head">       → @section head { ... }
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → main content block (@RenderBody)

      asp:Label ID="TDept"                            → @Model.StatusMessage
      asp:GridView ID="TDeptGrid"                     → HTML table rendered from Model.DeptData (async)
        AutoGenerateSelectButton="true"               → Select button column in each row
        OnRowCommand="TDeptGrid_RowCommand"            → POST to SelectDepartment action
        asp:TemplateField HeaderText="No."            → row-number column (loop index + 1)
        asp:Label ID="lblRowNumber"                   → @(i + 1) in table cell

    cr-dotnet-1034 – Async GridView Data Binding with RDS via Entity Framework Core:
      Line 27: <asp:GridView ID="TDeptGrid" ... AutoGenerateSelectButton="True"
                             OnRowCommand="TDeptGrid_RowCommand" ...>
               Synchronous TDeptGrid.DataSource = DT; TDeptGrid.DataBind() in code-behind
               → Replaced with async Task-based data loading in TakeAppointmentController.IndexAsync()
                 using getdeptInfo_Async() DAL method backed by Amazon RDS Proxy.
                 Prevents thread pool exhaustion under cloud load; enables auto-scaling.

      Line 56: <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
               Synchronous GridView row-number label binding
               → Replaced with @(i + 1) in async-rendered HTML table in
                 Views/Patient/TakeAppointment.cshtml (no server-side data binding).

    Active Razor view is at Views/Patient/TakeAppointment.cshtml.
    Active MVC controller is at Controllers/TakeAppointmentController.cs.
--%>
