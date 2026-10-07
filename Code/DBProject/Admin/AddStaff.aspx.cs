using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

// Migrated from ASP.NET Web Forms (System.Web.UI.Page) to ASP.NET Core Razor Pages (PageModel)
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages

namespace DBProject.Admin
{
    public class AddStaffModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "*Required")]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "*Required")]
        [RegularExpression(@"((?:0[1-9])|(?:1[0-2]))\/((?:0[0-9])|(?:[1-2][0-9])|(?:3[0-1]))\/(\d{4})", ErrorMessage = "Birth Date Format Not Correct")]
        public string BirthDate { get; set; }

        [BindProperty]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public string Phone { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "*Required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Numbers Only !")]
        public string Salary { get; set; }

        [BindProperty]
        public string Qual { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "*Required")]
        public string Designation { get; set; }

        [BindProperty]
        public string Address { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            myDAL objmyDAL = new myDAL();

            int salary = Convert.ToInt32(Salary);
            string genderForm = Request.Form["Gender"].ToString();
            char gender = genderForm.Length > 0 ? genderForm[0] : 'M';

            if (objmyDAL.AddStaff(Name, BirthDate, Phone, gender, Address, salary, Qual, Designation) == 1)
            {
                IsSuccess = true;
                Message = Designation + " Added Succesfully";
                FlushInformation();
            }

            return Page();
        }

        private void FlushInformation()
        {
            Name = "";
            BirthDate = "";
            Phone = "";
            Address = "";
            Salary = "";
            Qual = "";
            Designation = "";
        }
    }
}
