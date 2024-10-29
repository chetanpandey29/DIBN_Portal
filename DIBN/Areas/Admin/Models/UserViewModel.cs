using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static DIBN.Areas.Admin.Models.PermissionViewModel;

namespace DIBN.Areas.Admin.Models
{
    public class UserViewModel
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
        [DisplayName("Country")]
        public string MCountry { get; set; }
        public IList<SelectListItem> Countries { get; set; }

        [DisplayName("Country of Residence")]
        [Required(ErrorMessage = "Please enter Country of Recidence.")]
        public string CountryOfRecidence { get; set; }

        [DisplayName("Telephone Number")]
        public string TelephoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUTC { get; set; }
        public string CompanyType { get; set; }
        public string Action { get; set; }
        public string returnAction { get; set; }
        public string ModifyOnUTC { get; set; }
        public string Module { get; set; }
        [DisplayName("Roles")]
        public IList<SelectListItem> Roles { get; set; }
        [DisplayName("Roles")]
        [Required(ErrorMessage = "Please Select Role from below list.")]
        public List<int> RoleId { get; set; }
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
        [DisplayName("Is Login Allowed?")]
        public bool IsLogin { get; set; }
        [DisplayName("Employment  Card Expiry Date")]
        public string EmployeeCardExpiryDate { get; set; }
        public string companyOwner { get; set; }
        public int CreatedBy { get; set; }

        #region Nested Class
        public class SaveUser
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
            [DisplayName("Country")]
            public string MCountry { get; set; }
            public IList<SelectListItem> Countries { get; set; }
            [DisplayName("Country of Residence")]
            [Required(ErrorMessage = "Please enter Country of Residence.")]
            public string CountryOfRecidence { get; set; }

            [DisplayName("Telephone Number")]
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
            public List<int> RoleId { get; set; }
            [DisplayName("Roles")]
            public string Role { get; set; }
            public int CompanyId { get; set; }
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
            [DisplayName("Employment Card Expiry Date")]
            public string EmployeeCardExpiryDate { get; set; }
            [DisplayName("Is Login Allowed?")]
            public bool IsLogin { get; set; }
            public int CreatedBy { get; set; }
        }
        public class UserListForCompany
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string AccountNumber { get; set; }
        }

        public class User_Role_Mapping
        {
            public int Id { get; set; }
            public int RoleId { get; set; }
            public int UserId { get; set; }
        }
        public class UserPermissionList
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int CompanyId { get; set; }
            public string Role { get; set; }
            public string Username { get; set; }
            [DisplayName("Modules")]
            public List<ModuleViewModel> Modules { get; set; }
            public List<PermissionViewModel> Permissions { get; set; }
            public int permissionCount { get; set; }
            public string Module { get; set; }
            public List<GetUserPermissionByUserId> getUserPermissionByUserIds { get; set; }
            public List<GetRolePermissionByRoleId> getRolePermissionByRoleIds { get; set; }
            public List<string> allowedModule {  get; set; }

        }
        public class SaveUserPermissionValues
        {
            public int[] InsertPermission { get; set; }
            public int[] UpdatePermission { get; set; }
            public int[] ViewPermission { get; set; }
            public int[] DeletePermission { get; set; }
            public int UserId { get; set; }
        }

        public class SaveCompanyForUser
        {
            public string Action { get; set; }
            public int Id { get; set; }
            [DisplayName("Company Type")]
            public IList<SelectListItem> CompanyType { get; set; }
            public IList<SelectListItem> Countries { get; set; }
            public string MainContactNumberCountry { get; set; }
            public string EmergencyContactNumberCountry { get; set; }
            public string CompanyTypeName { get; set; }
            [DisplayName("Account Number")]
            [Required(ErrorMessage = "Please Enter Account Number of Company")]
            public string AccountNumber { get; set; }
            [DisplayName("DIBN User Number")]
            public string DIBNUserNumber { get; set; }
            [DisplayName("User of Company")]
            public IList<SelectListItem> Users { get; set; }
            public string Username { get; set; }
            [DisplayName("User of Company")]
            //[Required(ErrorMessage = "Please Select User for this Company.")]
            public int UserId { get; set; }
            [DisplayName("Company Name")]
            [Required(ErrorMessage = "Please enter Company Name.")]
            public string CompanyName { get; set; }
            [DisplayName("Company Password")]
            [Required(ErrorMessage = "Please enter Company Password.")]
            public string CompanyPassword { get; set; }

            [DisplayName("Mobile Number")]
            [Required(ErrorMessage = "Please enter Mobile Number.")]
            public string MobileNumber { get; set; }
            [DisplayName("Emergency Mobile Number")]
            public string EmergencyNumber { get; set; }
            public List<string> OtherContactNumbers { get; set; }
            public List<string> OtherContactNumbersCode { get; set; }
            [DisplayName("Email ID")]
            [Required(ErrorMessage = "Please enter Email ID.")]
            public string EmailID { get; set; }
            public List<string> OtherEmailID { get; set; }
            public string OtherEmailIdValues { get; set; }

            [DisplayName("Second Email ID")]
            public string SecondEmailID { get; set; }
            [DisplayName("Is Active")]
            public bool IsActive { get; set; }
            public string Module { get; set; }
            [DisplayName("Shareholder")]
            public string ShareholderName { get; set; }
            [DisplayName("Shareholder Share(%)")]
            public string ShareholderSharePercentage { get; set; }
            [DisplayName("Country")]
            [Required(ErrorMessage = "Please Enter Country.")]
            public string Country { get; set; }
            [DisplayName("Company Starting Date")]
            [Required(ErrorMessage = "Please enter Company Formation Date.")]
            public string CompanyStartingDate { get; set; }
            public string OldPassword { get; set; }
            [DisplayName("City")]
            public string City { get; set; }
            [DisplayName("Labour File No.")]
            public string LabourFileNo { get; set; }
            public int CreatedBy { get; set; }

        }
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
            public string EmployeementCardExpiryDate { get; set; }
            public bool IsActive { get; set; }
            public bool IsDelete { get; set; }
            public string CreatedOnUtc { get; set; }
            public string ModifyOnUtc { get; set; }
            public string Role { get; set; }
            public int CompanyId { get; set; }
            public string Company { get; set; }
            [DisplayName("Is Login Allowed?")]
            public bool IsLogin { get; set; }
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
            public string EmployeementCardExpiryDate { get; set; }
            public bool IsActive { get; set; }
            public bool IsDelete { get; set; }
            public string CreatedOnUtc { get; set; }
            public string ModifyOnUtc { get; set; }
            public string Role { get; set; }
            public int CompanyId { get; set; }
            public string Company { get; set; }
            [DisplayName("Is Login Allowed?")]
            public bool IsLogin { get; set; }
        }
        public class GetCompanyEmployeesModel
        {
            public string Module { get; set; }
            public int CompanyId { get; set; }
            public string Company { get; set; }
            public List<SelectListItem> companies { get; set; }
            public List<GetActiveEmployees> GetActiveEmployees { get; set; }
            public List<GetInActiveEmployees> GetInActiveEmployees { get;set; }
            public List<string> allowedModule {  get; set; }
            public List<string> allowedUserPermissionModule {  get; set; }
        }
        public class GetAllActiveEmployeeListWithPaginationModel
        {
            public int totalActiveEmployees { get; set; }
            public List<GetActiveEmployees> GetActiveEmployees { get; set; }
        }

        public class GetAllInActiveEmployeeListWithPaginationModel
        {
            public int totalInActiveEmployees { get; set; }
            public List<GetInActiveEmployees> GetInActiveEmployees { get; set; }
        }
        public class GetAllActiveCompanyOwnerListWithPaginationModel
        {
            public int totalActiveCompanyOwners { get; set; }
            public List<GetActiveEmployees> GetActiveCompanyOwner { get; set; }
        }

        public class GetAllInActiveCompanyOwnerListWithPaginationModel
        {
            public int totalInActiveCompanyOwner { get; set; }
            public List<GetInActiveEmployees> GetInActiveCompanyOwner { get; set; }
        }

        #endregion
    }
    public class SaveNewUser
    {
        public int Id { get; set; }
        [DisplayName("Account Number")]
        [Required(ErrorMessage = "Please enter Account number.")]
        public string AccountNumber { get; set; }
        [DisplayName("Password")]
        public string? Password { get; set; }
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
        [DisplayName("Country")]
        public string MCountry { get; set; }
        public IList<SelectListItem> Countries { get; set; }
        public string Module { get; set; }
        [DisplayName("Roles")]
        public IList<SelectListItem> Roles { get; set; }
        [DisplayName("Roles")]
        [Required(ErrorMessage = "Please Select Role from below list.")]
        public List<int> RoleId { get; set; }
        [DisplayName("Roles")]
        public string Role { get; set; }
        [DisplayName("Assign to Company")]
        public IList<SelectListItem> Companies { get; set; }
        [DisplayName("Assign to Company")]
        [Required(ErrorMessage = "Please Select Company from below list.")]
        public int CompanyId { get; set; }
        [DisplayName("Assign to Company")]
        public string Company { get; set; }
        [DisplayName("Is Login Allowed?")]
        public bool IsLogin { get; set; }
        public bool IsActive { get; set; }
        public string Action { get; set; }
        public int CreatedBy { get; set; }
    }

    public class MainCompanyEmployeesModel
    {
        public List<string> allowedModule {  get; set; }
        public List<string> allowedUserPermissionModule {  get; set; }
        public string Module { get; set; }
    }
    public class GetMainCompanyEmployees
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
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string Role { get; set; }
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string Module { get; set; }
        [DisplayName("Is Login Allowed?")]
        public bool IsLogin { get; set; }
        public string _checked { get; set; }
    }
    public class GetAllActiveMainCompanyEmployeeListWithPaginationModel
    {
        public int totalActiveEmployee { get; set; }
        public List<GetMainCompanyEmployees> GetActiveEmployee { get; set; }
    }

    public class GetAlInlActiveMainCompanyEmployeeListWithPaginationModel
    {
        public int totalInActiveEmployee { get; set; }
        public List<GetMainCompanyEmployees> GetInActiveEmployee { get; set; }
    }
    public class GetUsersForCompanyModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }
}

