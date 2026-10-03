<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (DoctorHome.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Replacement files:
      • Views/Doctor/DoctorHome.cshtml         – Razor View (replaces this .aspx)
      • Controllers/DoctorHomeController.cs    – MVC Controller (replaces DoctorHome.aspx.cs)
      • Models/DoctorHomeViewModel.cs          – ViewModel

    Web Forms directives and server controls replaced:
      Line 1: <%@ Page MasterPageFile="~/Doctor/doctormaster.Master" ... %>
              → @{ Layout = "~/Views/Shared/_DoctorLayout.cshtml"; }

      <asp:Content ContentPlaceHolderID="head">  → @section head { ... }
      <asp:Content ContentPlaceHolderID="Cp1">   → main content block in Razor View

      <asp:label id="Label1"  runat="server" />  → @Model.Name
      <asp:label id="Label2"  runat="server" />  → @Model.Phone
      <asp:label id="Label3"  runat="server" />  → @Model.Address
      <asp:label id="Label4"  runat="server" />  → @Model.BirthDate
      <asp:label id="Label5"  runat="server" />  → @Model.Gender
      <asp:label id="Label6"  runat="server" />  → @Model.DepartmentNo
      <asp:label id="Label7"  runat="server" />  → @Model.ChargesPerVisit
      <asp:label id="Label8"  runat="server" />  → @Model.MonthlySalary
      <asp:label id="Label9"  runat="server" />  → @Model.ReputeIndex
      <asp:label id="Label10" runat="server" />  → @Model.PatientsTreated
      <asp:label id="Label11" runat="server" />  → @Model.Qualification
      <asp:label id="Label12" runat="server" />  → @Model.Specialization
      <asp:label id="Label13" runat="server" />  → @Model.WorkExperience
      <asp:label id="Label14" runat="server" />  → @Model.Status

    The active view is now Views/Doctor/DoctorHome.cshtml.
--%>
