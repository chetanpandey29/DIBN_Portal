using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static DIBN.Areas.Admin.Models.PermissionViewModel;

namespace DIBN.Areas.Admin.Models
{
    public class CompanyViewModel
    {
        public int Id { get; set; }
        [DisplayName("Company Type")]
        public IList<SelectListItem> CompanyType { get; set; }
        [DisplayName("Company Type")]
        public string CompanyTypeName { get; set; }
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        [DisplayName("DIBN User Number")]
        public string DIBNUserNumber { get; set; }
        [DisplayName("User of Company")]
        public string Username { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Share Capital")]
        public string ShareCapital { get; set; }
        [DisplayName("Company Registration Number")]
        public string CompanyRegistrationNumber { get; set; }
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }
        [DisplayName("Emergency Mobile Number")]
        public string EmergencyNumber { get; set; }
        [DisplayName("Email ID")]
        public string EmailID { get; set; }
        [DisplayName("Second Email ID")]
        public string SecondEmailID { get; set; }
        [DisplayName("Authority")]
        public string LicenseType { get; set; }
        [DisplayName("License Number")]
        public string LicenseNumber { get; set; }
        [DisplayName("License Status")]
        public string LicenseStatus { get; set; }
        [DisplayName("License Issue Date")]
        public string LicenseIssueDate { get; set; }
        [DisplayName("License Expiry Date")]
        public string LicenseExpiryDate { get; set; }
        [DisplayName("Lease Facility Type")]
        public string LeaseFacilityType { get; set; }
        [DisplayName("Lease Start Date")]
        public string LeaseStartDate { get; set; }
        [DisplayName("Lease End Date")]
        public string LeaseExpiryDate { get; set; }
        [DisplayName("Lease Status")]
        public string LeaseStatus { get; set; }
        [DisplayName("Company Starting Date")]
        public string CompanyStartingDate { get; set; }
        [DisplayName("Unit Location")]
        public string UnitLocation { get; set; }
        [DisplayName("Shareholder")]
        public string ShareholderName { get; set; }
        [DisplayName("Shareholder Share(%)")]
        public string ShareholderSharePercentage { get; set; }
        public bool ShareholderIsActive { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("Is Company have TRN Number?")]
        public bool IsTRN { get; set; }
        [DisplayName("TRN Number")]
        public string TRN { get; set; }
        [DisplayName("TRN Creation Date")]
        public string TRNCreationDate { get; set; }
        [DisplayName("Labour File No.")]
        public string LabourFileNo { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        public string Module { get; set; }
        public List<CompanyShareholders> companyShareholders { get; set; }
        public List<CompanyUsers> companyUsers { get; set; }


        #region Nested Classes
        public class CompanyUsers
        {
            public int UserId { get; set; }
            public bool IsActive { get; set; }
            public bool IsLogin { get; set; }
            public string UserName { get; set; }
            public string UserAccountNumber { get; set; }
        }
        public class SaveCompany
        {
            public int Id { get; set; }
            [DisplayName("Company Type")]
            public IList<SelectListItem> CompanyType { get; set; }
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
            public int UserId { get; set; }

            [DisplayName("Company Name")]
            [Required(ErrorMessage = "Please enter Company Name.")]
            public string CompanyName { get; set; }
            [DisplayName("Company Password")]
            //[Required(ErrorMessage = "Please enter Company Password.")]
            public string? CompanyPassword { get; set; }
            [DisplayName("Share Capital")]
            //[Required(ErrorMessage = "Please enter Share Capital.")]
            public string ShareCapital { get; set; }
            [DisplayName("Company Registration Number")]
            //[Required(ErrorMessage = "Please enter Company Registration Number.")]
            public string CompanyRegistartionNumber { get; set; }
            [DisplayName("Mobile Number")]
            //[Required(ErrorMessage = "Please enter Mobile Number.")]
            public string? MobileNumber { get; set; }
            [DisplayName("Emergency Mobile Number")]
            public string EmergencyNumber { get; set; }
            public List<string> OtherContactNumbers { get; set; }
            public List<string> OtherContactNumbersCode { get; set; }

            public string? MainContactNumberCountry { get; set; }
            public string EmergencyContactNumberCountry { get; set; }
            [DisplayName("Email ID")]
            //[Required(ErrorMessage = "Please enter Email ID.")]
            public string? EmailID { get; set; }
            public List<string> OtherEmailID { get; set; }
            public string OtherEmailIdValues { get; set; }
            public IList<SelectListItem> Countries { get; set; }
            [DisplayName("Second Email ID")]
            public string SecondEmailID { get; set; }
            [DisplayName("Authority")]
            //[Required(ErrorMessage = "Please enter Authority Name.")]
            public string LicenseType { get; set; }
            [DisplayName("License Number")]
            public string LicenseNumber { get; set; }
            [DisplayName("License Status")]
            //[Required(ErrorMessage = "Please enter License Status.")]
            public string LicenseStatus { get; set; }
            [DisplayName("License Issue Date")]
            //[Required(ErrorMessage = "Please enter License Issue Date.")]
            public string LicenseIssueDate { get; set; }
            [DisplayName("License Expiry Date")]
            //[Required(ErrorMessage = "Please enter License Expiry Date.")]
            public string LicenseExpiryDate { get; set; }
            [DisplayName("Lease Facility Type")]
            //[Required(ErrorMessage = "Please enter Lease Facility Type.")]
            public string LeaseFacilityType { get; set; }
            [DisplayName("Lease Status")]
            //[Required(ErrorMessage = "Please enter Lease Status.")]
            public string LeaseStatus { get; set; }
            [DisplayName("Lease Start Date")]
            //[Required(ErrorMessage = "Please enter Lease Start Date.")]
            public string LeaseStartDate { get; set; }
            [DisplayName("Lease End Date")]
            //[Required(ErrorMessage = "Please enter Lease End Date.")]
            public string LeaseEndDate { get; set; }
            [DisplayName("Unit Location")]
            //[Required(ErrorMessage = "Please enter Unit Location.")]
            public string UnitLocation { get; set; }
            [DisplayName("Company Starting Date")]
            [Required(ErrorMessage = "Please enter Company Formation Date.")]
            public string CompanyStartingDate { get; set; }
            [DisplayName("Is Active")]
            public bool IsActive { get; set; }
            public string Module { get; set; }
            [DisplayName("Shareholder")]
            public string ShareholderName { get; set; }
            [DisplayName("Shareholder Share(%)")]
            public string ShareholderSharePercentage { get; set; }
            [DisplayName("Country")]
            //[Required(ErrorMessage = "Please Enter Country.")]
            public string? Country { get; set; }
            [DisplayName("Is Company have TRN Number?")]
            public bool IsTRN { get; set; }
            [DisplayName("TRN Number")]
            public string TRN { get; set; }
            [DisplayName("TRN Creation Date")]
            public string TRNCreationDate { get; set; }
            [DisplayName("City")]
            public string City { get; set; }
            [DisplayName("Labour File No.")]
            public string LabourFileNo { get; set; }
            public string OldPassword { get; set; }
            [DisplayName("Sales Person")]
            public IList<SelectListItem> SalesPersons { get; set; }
            [DisplayName("Sales Person")]
            public List<int> SalesPersonId { get; set; }
            public int CreatedBy { get; set; }
            [DisplayName("Does Company has Corporate Text?")]
            public bool IsCorporateText { get; set; }
            [DisplayName("Corporate Text")]
            public string CorporateText { get; set; }
            [DisplayName("Sub Company Type")]
            [Required(ErrorMessage = "Please enter company sub type.")]
            public string CompanySubType { get; set; }
        }
        public class SaveNewCompany
        {
            public int Id { get; set; }
            [DisplayName("Company Type")]
            public IList<SelectListItem> CompanyType { get; set; }
            public IList<SelectListItem> Countries { get; set; }
            public string CompanyTypeName { get; set; }
            public string? MainContactNumberCountry { get; set; }
            public string EmergencyContactNumberCountry { get; set; }
            [DisplayName("Account Number")]
            [Required(ErrorMessage = "Please Enter Account Number of Company")]
            public string AccountNumber { get; set; }
            [DisplayName("DIBN User Number")]
            public string DIBNUserNumber { get; set; }
            [DisplayName("User of Company")]
            public IList<SelectListItem> Users { get; set; }

            [DisplayName("Sales Person")]
            public IList<SelectListItem> SalesPersons { get; set; }
            [DisplayName("Sales Person")]
            public List<int> SalesPersonId { get; set; }
            public string Username { get; set; }
            [DisplayName("User of Company")]
            public int UserId { get; set; }
            [DisplayName("Company Name")]
            [Required(ErrorMessage = "Please enter Company Name.")]
            public string CompanyName { get; set; }
            [DisplayName("Company Password")]
           // [Required(ErrorMessage = "Please enter Company Password.")]
            public string? CompanyPassword { get; set; }

            [DisplayName("Mobile Number")]
            public string? MobileNumber { get; set; }
            [DisplayName("Emergency Mobile Number")]
            public string EmergencyNumber { get; set; }
            public List<string> OtherContactNumbers { get; set; }
            public List<string> OtherContactNumbersCode { get; set; }

            [DisplayName("Email ID")]
            public string? EmailID { get; set; }
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
            //[Required(ErrorMessage = "Please Enter Country.")]
            public string? Country { get; set; }

            [DisplayName("Company Starting Date")]
            [Required(ErrorMessage = "Please enter Company Formation Date.")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
            public DateTime companyStartingDate { get; set; }
            [DisplayName("Company Starting Date")]
            public string CompanyStartingDate { get; set; }
            public string OldPassword { get; set; }
            [DisplayName("City")]
            public string City { get; set; }
            [DisplayName("Labour File No.")]
            public string LabourFileNo { get; set; }
            [DisplayName("Does Company has Corporate Text?")]
            public bool IsCorporateText { get; set; }
            [DisplayName("Corporate Text")]
            public string CorporateText { get; set; }
            [DisplayName("Sub Company Type")]
            [Required(ErrorMessage = "Please enter company sub type.")]
            public string CompanySubType {  get; set; }
            public int CreatedBy { get; set; }
        }
        public class CompanyPermissionList
        {
            public int Id { get; set; }
            public int CompanyId { get; set; }
            [DisplayName("Modules")]
            public List<ModuleViewModel> Modules { get; set; }
            public List<PermissionViewModel> Permissions { get; set; }
            public int permissionCount { get; set; }
            public string Module { get; set; }
            public List<GetCompanyPermissionByCompanyId> getCompanyPermissionByCompanyIds { get; set; }

        }

        #endregion
    }
    public class GetCompaniesWithMainCompany
    {
        public List<CompanyViewModel> GetMainCompany { get; set; }
        public List<CompanyViewModel> GetOtherCompanies { get; set; }
        public string Module { get; set; }
        public List<string> allowedPermission {  get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class CompanyShareholders
    {
        [DisplayName("Shareholder")]
        public string ShareholderName { get; set; }
        [DisplayName("Shareholder Share(%)")]
        public string ShareholderSharePercentage { get; set; }
    }

    public class GetCompaniesForAccountSummary
    {
        public int index { get; set; }
        public int CompanyId { get; set; }
        public string AccountNumber { get; set; }
        public string CompanyName { get; set; }
        public string EmailID { get; set; }
        public string SalesPerson { get; set; }
        public decimal PortalBalance { get; set; }
    }

    public class GetCompanyForAccountSummaryWithPagination
    {
        public GetCompanyForAccountSummaryWithPagination()
        {
            getCompaniesForAccounts = new List<GetCompaniesForAccountSummary>();
        }
        public List<GetCompaniesForAccountSummary> getCompaniesForAccounts { get; set; }
        public int totalCompanies { get; set; }
    }
    public class GetCompanyLog
    {
        public string LogMessage { get; set; }
        public string CreatedBy { get; set; }
        public string ModifyBy { get; set; }
        public string CreatedOnDate { get; set; }
        public string CreatedOnTime { get; set; }
        public string ModifyOnDate { get; set; }
        public string ModifyOnTime { get; set; }
        public DateTime DateOnUtc { get; set; }
    }

    public class GetCompanyLogDetails
    {
        public string Module { get; set; }
        public List<GetCompanyLog> logs { get; set; }
    }

    public class GetCompanyDetailsWithPagination
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string CompanyName { get; set;}
        public string CompanyType { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string LicenseType { get; set; }
        public string ShareholderName { get; set; }
        public string ShareholderSharePercentage { get; set; }
        public bool ShareholderIsActive { get; set; }
        public bool IsActive { get; set; }
    }
    public class GetCompanyDetailsWithPaginationModel
    {
        public List<GetCompanyDetailsWithPagination> getCompanyDetails { get; set; }
        public int totalCompanies { get; set; }
    }

    public class GetCompanyListForExport
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }
}
