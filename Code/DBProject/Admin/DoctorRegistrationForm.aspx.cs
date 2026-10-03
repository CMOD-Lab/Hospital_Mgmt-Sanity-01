// Migrated from ASP.NET Web Forms to ASP.NET Core MVC Controller (cr-dotnet-0026).
//
// Original Web Forms code-behind violations removed:
//   Line 2  – using System.Web.UI;             (Web Forms namespace – removed)
//   Line 3  – using System.Web.UI.WebControls; (Web Forms namespace – removed)
//   Line 8  – class DoctorRegistrationForm : System.Web.UI.Page  (Page inheritance – removed)
//   Line 10 – protected void Page_Load(...)    (Web Forms lifecycle event – replaced with MVC action)
//
// Replacement: ASP.NET Core MVC Controller with GET/POST actions.
// All original business logic (email validation, doctor registration, field clearing)
// is preserved and mapped to the MVC request/response model.

using System;
using System.ComponentModel.DataAnnotations;
using DBProject.DAL;
using DBProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace DBProject.Controllers
{
    /// <summary>
    /// ASP.NET Core MVC Controller for Doctor Registration.
    /// Replaces the Web Forms DoctorRegistrationForm code-behind (DoctorRegistrationForm.aspx.cs).
    /// </summary>
    public class DoctorRegistrationController : Controller
    {
        // GET: /DoctorRegistration
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Admin/DoctorRegistrationForm.cshtml", new DoctorRegistrationViewModel());
        }

        // POST: /DoctorRegistration/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(DoctorRegistrationViewModel model)
        {
            // Validate that a department was selected (replaces DepartmentValidate CustomValidator)
            if (model.DepartmentId == 0)
            {
                ModelState.AddModelError(nameof(model.DepartmentId), "Please Select Department");
            }

            // Validate that the doctor email does not already exist (replaces ValidateDoctorEmail CustomValidator)
            if (!string.IsNullOrEmpty(model.Email))
            {
                myDAL objmyDAL = new myDAL();
                if (objmyDAL.DoctorEmailAlreadyExist(model.Email) == 1)
                {
                    ModelState.AddModelError(nameof(model.Email),
                        "This Email already exists, kindly choose a different one!");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/DoctorRegistrationForm.cshtml", model);
            }

            // Determine gender from form (replaces Request.Form["Gender"] in Web Forms)
            string genderValue = Request.Form["Gender"].ToString();
            char gender = (genderValue.Length > 0) ? genderValue[0] : 'M';

            myDAL dal = new myDAL();
            dal.AddDoctor(
                model.Name,
                model.Email,
                model.Password,
                model.BirthDate,
                model.DepartmentId,
                model.Phone,
                gender,
                model.Address,
                model.Experience,
                model.Salary,
                model.ChargesPerVisit,
                model.Specialization,
                model.Qualification
            );

            TempData["SuccessMessage"] = "Doctor Added Successfully";

            // Redirect to GET to prevent duplicate form submission (Post/Redirect/Get pattern)
            return RedirectToAction(nameof(Register));
        }
    }
}
