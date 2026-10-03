<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (Bill.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Replacement files:
      • Views/Doctor/Bill.cshtml       – Razor View (replaces this .aspx)
      • Controllers/BillController.cs  – MVC Controller (replaces Bill.aspx.cs)
      • Models/BillViewModel.cs        – ViewModel

    Web Forms directives and server controls replaced:
      <%@ Page MasterPageFile="~/Doctor/doctormaster.Master" ... %> → Layout = "~/Views/Shared/_DoctorLayout.cshtml"
      <asp:Label ID="Label1" runat="server">                        → @Model.BillAmount in Razor View
      <asp:Button ID="Bill" OnClick="bill_paid" runat="server">     → <form> POST to BillController.BillPaid()
      <asp:Button ID="Button1" OnClick="bill_Unpaid" runat="server">→ <form> POST to BillController.BillUnpaid()

    The active view is now Views/Doctor/Bill.cshtml.
--%>
