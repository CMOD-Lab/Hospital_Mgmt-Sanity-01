<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (PatientHome.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Replacement:
      PatientHome.aspx → Views/Patient/PatientHome.cshtml

    Web Forms patterns removed / replaced (cr-dotnet-0026):
      Line 1: <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
                       AutoEventWireup="true" CodeBehind="PatientHome.aspx.cs"
                       Inherits="DBProject.PatientHome" %>
              → @{ Layout = "~/Views/Shared/_PatientLayout.cshtml"; }

      <asp:Content ContentPlaceHolderID="head">  → @section head { ... }
      <asp:Content ContentPlaceHolderID="ContentPlaceHolder1"> → main content block (@RenderBody)

      asp:Label ID="PName"      → @Model.Name
      asp:Label ID="PPhone"     → @Model.Phone
      asp:Label ID="PBirthDate" → @Model.BirthDate
      asp:Label ID="PatientAge" → @Model.Age
      asp:Label ID="PGender"    → @Model.Gender
      asp:Label ID="PAddress"   → @Model.Address

    Code-behind logic moved to Controllers/PatientHomeController.cs.
    Active Razor view is at Views/Patient/PatientHome.cshtml.
--%>
