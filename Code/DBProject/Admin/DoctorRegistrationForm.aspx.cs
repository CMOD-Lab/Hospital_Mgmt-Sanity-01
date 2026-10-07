using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

// Migrated from ASP.NET Web Forms (System.Web.UI.Page) to ASP.NET Core Razor Pages (PageModel)
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages

namespace DBProject.Admin
{
    public class DoctorRegistrationFormModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "*Required")]
        [RegularExpression(@"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})", ErrorMessage = "Birth Date Format Not Correct")]
        public string BirthDate { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "*Required")]
        [EmailAddress(ErrorMessage = "Incorrect Email Format")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "*Required")]
        public string Password { get; set; }

        [BindProperty]
        [Compare("Password", ErrorMessage = "Passwords Do not Match")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public string Phone { get; set; }

        [BindProperty]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public string Salary { get; set; }

        [BindProperty]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public string ChargesPerVisit { get; set; }

        [BindProperty]
        [Range(0, 5, ErrorMessage = "Experience Range should be (0-5)")]
        public string Exp { get; set; }

        [BindProperty]
        public int Department { get; set; }

        [BindProperty]
        public string Qualification { get; set; }

        [BindProperty]
        public string Spec { get; set; }

        [BindProperty]
        public string Address { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string EmailError { get; set; }
        public string DepartmentError { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            myDAL objmyDAL = new myDAL();

            // Validate doctor email uniqueness
            if (objmyDAL.DoctorEmailAlreadyExist(Email) == 1)
            {
                EmailError = "This Email Already exist , kindly choose a different one !";
                ModelState.AddModelError("Email", EmailError);
            }

            // Validate department selection
            if (Department == 0)
            {
                DepartmentError = "Please Select Department";
                ModelState.AddModelError("Department", DepartmentError);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            int exp = Convert.ToInt32(Exp);
            int salary = Convert.ToInt32(Salary);
            int chargesPerVisit = Convert.ToInt32(ChargesPerVisit);
            string genderForm = Request.Form["Gender"].ToString();
            char gender = genderForm.Length > 0 ? genderForm[0] : 'M';

            objmyDAL.AddDoctor(Name, Email, Password, BirthDate, Department, Phone, gender, Address, exp, salary, chargesPerVisit, Spec, Qualification);

            IsSuccess = true;
            Message = "doctor Added Succesfully";
            FlushInformation();

            return Page();
        }

        private void FlushInformation()
        {
            Name = "";
            Email = "";
            Password = "";
            BirthDate = "";
            Department = 0;
            Phone = "";
            Address = "";
            Exp = "";
            Salary = "";
            ChargesPerVisit = "";
            Spec = "";
            Qualification = "";
        }
    }
}
