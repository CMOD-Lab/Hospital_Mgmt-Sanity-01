<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (HistoryUpdate.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Replacement file:
      • Views/Doctor/HistoryUpdate.cshtml  – Razor View (replaces this .aspx)

    Web Forms page directives and server controls replaced:
      Line 1: <%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/doctormaster.Master"
                       AutoEventWireup="true" CodeBehind="HistoryUpdate.aspx.cs"
                       Inherits="doctor.Historyupdate" %>
              → Standard Razor View with @{ Layout = "~/Views/Shared/_DoctorLayout.cshtml"; }

      <asp:Content ContentPlaceHolderID="head">   → @section head { <title>Update History</title> }
      <asp:Content ContentPlaceHolderID="Cp1">    → main Razor content block

      asp:TextBox ID="Disease"       → <input type="text" name="Disease" />
      asp:TextBox ID="progress"      → <input type="text" name="Progress" />
      asp:TextBox ID="Prescription"  → <input type="text" name="Prescription" />
      asp:Button  ID="submit"        → <button> posting to /HistoryUpdate/Save
      asp:Button  ID="Bill"          → <button> posting to /HistoryUpdate/GenerateBill

    The active view is now Views/Doctor/HistoryUpdate.cshtml.
    The active controller is Controllers/HistoryUpdateController.cs.
--%>
