using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Areas.Admin.Models
{
    public class MainSalesPersonViewModel
    {
        public string Module { get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class SalesPersonViewModel
    {
        public int Id { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please enter your First Name.")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please enter your Last Name.")]
        public string LastName { get; set; }
        [DisplayName("Email Id")]
        [Required(ErrorMessage = "Please enter your Email Id.")]
        public string EmailId { get; set; }
        [DisplayName("Phone Number")]
        [Required(ErrorMessage = "Please enter your Phone Number.")]
        public string PhoneNumber { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please enter your Password.")]
        public string Password { get; set; }
        [DisplayName("Country of Residence")]
        [Required(ErrorMessage = "Please enter your Country of Recidence.")]
        public string CountryOfRecidence { get; set; }
        [DisplayName("Passport Number")]
        public string PassportNumber { get; set; }
        [DisplayName("Passport Expiry Date")]
        public string PassportExpiryDate { get; set; }
        [DisplayName("Designation")]
        [Required(ErrorMessage = "Please Select Designation.")]
        public int Designation { get; set; }
        public string Role { get; set; }
        [DisplayName("Visa Expiry Date")]
        public string VisaExpiryDate { get; set; }
        [DisplayName("Insurance Company")]
        public string InsuarnceCompany { get; set; }
        [DisplayName("Insurance Expiry Date")]
        public string InsuranceExpiryDate { get; set; }
        [DisplayName("Assigned Company")]
        public string Company { get; set; }
        public List<SelectListItem> companies { get; set; }
        public List<SelectListItem> Roles { get; set; }
        [DisplayName("Assign To Company")]
        [Required(ErrorMessage = "Please Select Company.")]
        public List<int> CompanyId { get; set; }
        [DisplayName("Is Login Allowed?")]
        public bool IsLogin { get; set; }
        [DisplayName("Is Active?")]
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        [DisplayName("Country")]
        public string MCountry { get; set; }
        public IList<SelectListItem> Countries { get; set; }
        public string Module { get; set; }
        public int CreatedBy { get; set; }
    }

    public class GetAllSalesPersonsWithPaginationModel
    {
        public List<SalesPersonViewModel> salesPersons { get; set; }
        public int totalSalesPersons { get; set; }
    }
    public class GetSalesPersonDetailsForLoginModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
