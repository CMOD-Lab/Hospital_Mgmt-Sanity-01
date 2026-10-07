@page "/Patient/PatientFeedback"
@model DBProject.Patient.PatientFeedbackModel
@{
    ViewData["Title"] = "Feedback";
    Layout = "~/Patient/_PatientLayout.cshtml";
}

@* Migrated from ASP.NET Web Forms (<%@ Page %>) to ASP.NET Core Razor Pages *@
@* Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages *@

@section head {
    <title>Feedback</title>
}

<h1><strong style="margin:37%">Feedback</strong></h1>
<br /><br />

<div style="margin-left: 70px">

    @if (!string.IsNullOrEmpty(Model.FeedbackStatus))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.FeedbackStatus</p>
        <br /><br />
    }

    @if (!string.IsNullOrEmpty(Model.FDoctorInfo))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.FDoctorInfo</p>
        <br /><br />
    }

    @if (!string.IsNullOrEmpty(Model.FTimings))
    {
        <p style="font-weight:bold; font-size:medium;">@Model.FTimings</p>
        <br /><br />
    }

    @if (Model.ShowFeedbackForm)
    {
        <br /><br />
        <p style="font-weight:bold; font-size:medium;">Dear Patient, How was your treatment experience with our specialized Doctor on a rating of 1 - 5:</p>

        <form method="post">
            @Html.AntiForgeryToken()
            <div style="margin-left: 790px">
                <select name="SelectedRating" style="font-weight:bold;">
                    <option value="1">1</option>
                    <option value="2">2</option>
                    <option value="3">3</option>
                    <option value="4">4</option>
                    <option value="5">5</option>
                </select>

                <br /><br />

                <button type="submit" asp-page-handler="GiveFeedback" style="font-weight:bold;">Give Feedback</button>

                <br /><br />
                @if (!string.IsNullOrEmpty(Model.FeedbackResult))
                {
                    <p style="font-weight:bold; font-size:medium;">@Model.FeedbackResult</p>
                }
            </div>
        </form>
    }

    <br /><br />
</div>
