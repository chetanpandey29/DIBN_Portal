using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Models
{
    public class MainDocumentCheckListModel
    {
        public List<UserCompanyViewModel> companies { get; set; }
        public List<string> allowedModule {  get; set; }
        public string Module {  get; set; }
    }
    public class UserCompanyViewModel
    {
        public int Id { get; set; }
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
        [DisplayName("Unit Location")]
        public string UnitLocation { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [DisplayName("Company Type")]
        public string CompanyTypeName { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        public string Module { get; set; }
        [DisplayName("Shareholder")]
        public string ShareholderName { get; set; }
        [DisplayName("Shareholder Share(%)")]
        public string ShareholderSharePercentage { get; set; }
        public bool ShareholderIsActive { get; set; }

        [DisplayName("Company Type")]
        public string CompanyType { get; set; }
        [DisplayName("Labour File No.")]
        public string LabourFileNo { get; set; }
        public string CompanyStartingDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool IsTRN { get; set; }
        public string TRN { get; set; }
        public string TRNCreationDate { get; set; }

        public string Role { get; set; }
        public List<CompanyShareholders> companyShareholders { get; set; }
    }

    public class CompanyViewModel
    {
        public UserCompanyViewModel userCompanyViewModel { get; set; }
        public List<UserCompanyViewModel> userCompanies { get; set; }
        public string Module { get; set; }
        public List<string> allowedModule {  get; set; }
        public List<string> allowedPermissions { get; set; }
    }
    public class CompanyShareholders
    {
        [DisplayName("Shareholder")]
        public string ShareholderName { get; set; }
        [DisplayName("Shareholder Share(%)")]
        public string ShareholderSharePercentage { get; set; }
    }
}
