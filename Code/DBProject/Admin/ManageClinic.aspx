<%--
    cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (ManageClinic.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Replacement files:
      • Views/Admin/ManageClinic.cshtml       – Razor View (replaces this .aspx)
      • Controllers/ManageClinicController.cs – MVC Controller (replaces ManageClinic.aspx.cs)
      • Models/ManageClinicViewModel.cs       – ViewModel

    Web Forms directives and server controls replaced:
      <%@ Page ... %>                  → @model / @{ Layout = ... } in .cshtml
      <asp:RadioButton runat="server"> → <input type="radio"> with form GET parameter
      <asp:TextBox runat="server">     → <input type="text">
      <asp:Button runat="server">      → <button type="submit">
      <asp:Label runat="server">       → @Model.Message in Razor View
      <div runat="server" id="mydiv"> → @Model.SelectedRecord in Razor View

    cr-dotnet-1034 GridView replacements (synchronous DataBind → async ViewModel):
      Line 85:  <asp:GridView ID="Manage" ... OnRowDeleting="DeleteDoctor_Click"
                              OnRowCommand="SelectCommand" runat="server" ...>
                → Replaced: synchronous Manage.DataSource/DataBind() replaced with
                  async ManageClinicViewModel.GridData DataTable populated via
                  async Task-based data access in ManageClinicController connected
                  to Amazon RDS, preventing thread pool exhaustion under load.

      Line 104: </asp:GridView>
                → Replaced: closing tag of the synchronous GridView control removed.
                  Grid data is now rendered from ManageClinicViewModel.GridData
                  in the Razor View (Views/Admin/ManageClinic.cshtml) using
                  async controller action patterns.

    The active view is now Views/Admin/ManageClinic.cshtml.
--%>
