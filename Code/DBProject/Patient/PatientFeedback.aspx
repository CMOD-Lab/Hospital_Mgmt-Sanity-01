<%--
    MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
    This file (PatientFeedback.aspx) has been migrated to ASP.NET Core MVC Razor View.

    Original Web Forms directive (Line 1 – removed):
      <%@ Page Title="" Language="C#" MasterPageFile="~/Patient/PatientMaster.Master"
               AutoEventWireup="true" CodeBehind="PatientFeedback.aspx.cs"
               Inherits="DBProject.PatientFeedback" %>

    Migration mapping:
      PatientFeedback.aspx    → Views/Patient/PatientFeedback.cshtml  (Razor View)
      PatientFeedback.aspx.cs → Controllers/PatientFeedbackController.cs (MVC Controller)
      (new)                   → Models/PatientFeedbackViewModel.cs (ViewModel)

    Web Forms server controls replaced:
      <asp:Content>                    → Razor @section / @RenderBody()
      <asp:Label ID="Feedback">        → @Model.FeedbackMessage
      <asp:Label ID="FDoctor">         → @Model.DoctorMessage
      <asp:Label ID="FTimings">        → @Model.TimingsMessage
      <asp:Label ID="Message">         → @Model.ShowRatingPrompt (conditional display)
      <asp:DropDownList ID="List">     → <select name="rating"> in HTML form
      <asp:Button ID="button1"
        OnClick="giveFeedback">        → POST to PatientFeedback/GiveFeedback action
      <asp:Label ID="F">               → @Model.ConfirmationMessage
      IsPostBack check                 → GET vs POST action separation
      Session["idoriginal"]            → HttpContext.Session.GetInt32
      Session["aID"]                   → HttpContext.Session.SetInt32 / GetInt32

    Active implementation: Views/Patient/PatientFeedback.cshtml
--%>
