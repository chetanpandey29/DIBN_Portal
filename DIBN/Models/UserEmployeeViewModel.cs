using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Models
{
    public class UserEmployeeViewModel
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("Account Number")]
        [Required(ErrorMessage = "Please enter Account number.")]
        public string AccountNumber { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please enter Password.")]
        public string Password { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please enter First name.")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please enter Last name.")]
        public string LastName { get; set; }
        [DisplayName("Nationality")]
        [Required(ErrorMessage = "Please enter Nationality.")]
        public string Nationality { get; set; }
        [DisplayName("Email ID")]
        [Required(ErrorMessage = "Please enter Email ID.")]
        public string EmailID { get; set; }
        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Please enter phone number.")]
        public string PhoneNumber { get; set; }
        [DisplayName("Country of Residence")]
        [Required(ErrorMessage = "Please enter Country of Residence.")]
        public string CountryOfRecidence { get; set; }

        [DisplayName("Telephone Number")]
        [Required(ErrorMessage = "Please enter Telephone Number.")]
        public string TelephoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUTC { get; set; }
        public string ModifyOnUTC { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        [DisplayName("Roles")]
        public IList<SelectListItem> Roles { get; set; }
        [DisplayName("Roles")]
        [Required(ErrorMessage = "Please Select Role from below list.")]
        public int RoleId { get; set; }
        [DisplayName("Roles")]
        public string Role { get; set; }
        [DisplayName("Assign to Company")]
        public IList<SelectListItem> Companies { get; set; }
        [DisplayName("Assign to Company")]
        [Required(ErrorMessage = "Please Select Company from below list.")]
        public int CompanyId { get; set; }
        [DisplayName("Assign to Company")]
        public string Company { get; set; }
        [DisplayName("Passport Number")]
        public string PassportNumber { get; set; }
        [DisplayName("Designation")]
        public string Designation { get; set; }
        [DisplayName("Visa Expiry Date")]
        public string VisaExpiryDate { get; set; }
        [DisplayName("Insurance Company")]
        public string InsuranceCompany { get; set; }
        [DisplayName("Insurance Expiry Date")]
        public string InsuranceExpiryDate { get; set; }
        [DisplayName("Passport Expiry Date")]
        public string PassportExpiryDate { get; set; }



        #region Nested Classes

        public class GetEmployeesByCompanyId
        {
            [DisplayName("Id")]
            public int Id { get; set; }
            [DisplayName("Username")]
            public string Username { get; set; }
            [DisplayName("Account Number")]
            public string AccountNumber { get; set; }
            [DisplayName("Nationality")]
            public string Nationality { get; set; }
            [DisplayName("EmailID")]
            public string EmailID { get; set; }
            [DisplayName("Phone Number")]
            public string PhoneNumber { get; set; }
            [DisplayName("Country Of Residence")]
            public string CountryOfRecidence { get; set; }
            [DisplayName("Telephone Number")]
            public string TelephoneNumber { get; set; }
            [DisplayName("Role Id")]
            public int RoleId { get; set; }
            [DisplayName("Role Name")]
            public string RoleName { get; set; }
            [DisplayName("Is Active")]
            public bool IsActive { get; set; }
            [DisplayName("Is Delete")]
            public bool IsDelete { get; set; }
            [DisplayName("Created On UTC")]
            public string CreatedOnUTC { get; set; }
            [DisplayName("Modify On UTC")]
            public string ModifyOnUTC { get; set; }
            public string Module { get; set; }
            public string PassportNumber { get; set; }
            public string Designation { get; set; }
            public string VisaExpiryDate { get; set; }
            public string InsuranceCompany { get; set; }
            public string InsuranceExpiryDate { get; set; }
            [DisplayName("Passport Expiry Date")]
            public string PassportExpiryDate { get; set; }
        }


        public class GetRoles
        {
            public int RoleID { get; set; }
            [DisplayName("Role Name")]
            [Required(ErrorMessage = "Please enter Role Name.")]
            public string RoleName { get; set; }
            [DisplayName("Is Active")]
            public bool IsActive { get; set; }
            public bool IsDelete { get; set; }
            public string CreatedOn { get; set; }
            public string ModifyOn { get; set; }
            public string Module { get; set; }
        }
        #endregion


        public class GetActiveEmployees
        {
            public int Id { get; set; }
            public string AccountNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Nationality { get; set; }
            public string EmailID { get; set; }
            public string PhoneNumber { get; set; }
            public string CountryOfRecidence { get; set; }
            public string TelephoneNumber { get; set; }
            public bool IsActive { get; set; }
            public bool IsDelete { get;set; }
            public string CreatedOnUtc { get; set;}
            public string ModifyOnUtc { get; set; }
            public string Role { get; set; }
            public string Company { get; set; }
            public int CompanyId { get; set; }
            public string PassportNumber { get; set; }
            public string Designation { get; set; }
            public string VisaExpiryDate { get; set; }
            public string InsuranceCompany { get; set; }
            public string InsuranceExpiryDate { get; set; }
            [DisplayName("Passport Expiry Date")]
            public string PassportExpiryDate { get; set; }
        }

        public class GetInActiveEmployees
        {
            public int Id { get; set; }
            public string AccountNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Nationality { get; set; }
            public string EmailID { get; set; }
            public string PhoneNumber { get; set; }
            public string CountryOfRecidence { get; set; }
            public string TelephoneNumber { get; set; }
            public bool IsActive { get; set; }
            public bool IsDelete { get; set; }
            public string CreatedOnUtc { get; set; }
            public string ModifyOnUtc { get; set; }
            public string Role { get; set; }
            public string Company { get; set; }
            public int CompanyId { get; set; }
            public string PassportNumber { get; set; }
            public string Designation { get; set; }
            public string VisaExpiryDate { get; set; }
            public string InsuranceCompany { get; set; }
            public string InsuranceExpiryDate { get; set; }
            [DisplayName("Passport Expiry Date")]
            public string PassportExpiryDate { get; set; }
        }

        public class GetEmployeesForCompany
        {
            public string Role { get; set;}
            public List<GetActiveEmployees> getActiveEmployees { get; set; }
            public List<GetInActiveEmployees> getInActiveEmployees { get; set; }
            public List<UserEmployeeViewModel> getUserDetails { get; set; }
            public string Module { get; set; }
        }
    }
}
