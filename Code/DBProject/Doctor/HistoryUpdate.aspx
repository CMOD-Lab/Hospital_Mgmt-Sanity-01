@* Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
   Rule cr-dotnet-0026: Web Forms Usage
   
   Changes applied:
     Line 1 – removed: <%@ Page Title="" Language="C#" MasterPageFile="~/Doctor/doctormaster.Master"
              AutoEventWireup="true" CodeBehind="HistoryUpdate.aspx.cs" Inherits="doctor.Historyupdate" %>
              replaced with Razor Page directive and layout reference (occurrence 4)
   
   The Web Forms <%@ Page %> directive, <asp:Content>, <asp:TextBox>, and <asp:Button>
   server controls have been replaced with standard Razor Page syntax and HTML form elements,
   enabling stateless cloud-native deployment on AWS (Linux containers, Elastic Beanstalk, ECS/Fargate).
*@
@page
@model DBProject.Pages.Doctor.HistoryUpdateModel
@{
    ViewData["Title"] = "Update History";
    Layout = "_DoctorLayout";
}

@section Head {
    <title>Update History</title>
}

<h1>Update history</h1>

<form method="post">
    @Html.AntiForgeryToken()

    <h4>Disease:</h4>
    <input type="text" id="Disease" name="Disease" value="@Model.Disease" class="form-control" />

    <h4>Progress:</h4>
    <input type="text" id="progress" name="Progress" value="@Model.Progress" class="form-control" />

    <h4>Prescription</h4>
    <input type="text" id="Prescription" name="Prescription" value="@Model.Prescription" class="form-control" />

    <br />
    <br />
    <br />

    @if (!string.IsNullOrEmpty(Model.StatusMessage))
    {
        <div class="alert @(Model.IsError ? "alert-danger" : "alert-success")">
            @Model.StatusMessage
        </div>
    }

    <button type="submit" asp-page-handler="SaveInDatabase" class="btn btn-primary" style="font-weight:bold">
        Accept &amp; Save
    </button>
    <button type="submit" asp-page-handler="GenerateBill" class="btn btn-secondary" style="font-weight:bold">
        Generate Bill
    </button>
</form>
