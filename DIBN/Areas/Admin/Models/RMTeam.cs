using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DIBN.Areas.Admin.Models
{
    public class MainRMTeamModel
    {
        public string Module {  get; set; }
        public string message { get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class GetRMTeamListWithPaginationModel
    {
        public GetRMTeamListWithPaginationModel()
        {
            getRMTeamsList = new List<GetRMTeamListModel>();
        }
        public List<GetRMTeamListModel> getRMTeamsList { get; set; }
        public int totalCount {  get; set; }
    }
    public class GetRMTeamListModel
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string Company {  get; set; }
        public bool IsLoginAllowed {  get; set; }
        public bool IsActive { get; set; }
    }
    public class SaveRMTeamModel
    {
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please Provide First Name.")]
        public string Firstname { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Provide Last Name.")]
        public string Lastname { get; set; }
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "Please Provide Email Address.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please provide valid email address.")]
        public string EmailAddress {  get; set; }
        [DisplayName("Country")]
        public string MobileCode { get; set; }
        [DisplayName("Mobile No.")]
        [Required(ErrorMessage = "Please Provide Mobile No.")]
        public string MobileNumber { get; set; }
        [DisplayName("Country of Residence")]
        public string CountryofResidence { get; set; }
        public string? PassportNumber {  get; set; }
        public string? PassportExpiryDate {  get; set; }
        public string? VisaExpiryDate {  get; set; }
        public string Designation {  get; set; }
        public string? InsuranceCompany {  get; set; }
        public string? InsuranceExpiryDate {  get; set; }
        [DisplayName("Is Login Allowed?")]
        public bool IsLoginAllowed { get; set; }
        [DisplayName("Is Active?")]
        public bool IsActive { get; set; }
        public int UserId {  get; set; }

        [DisplayName("Assign To Company")]
        [Required(ErrorMessage = "Please Select Company.")]
        public List<int> CompanyId { get; set; }

        [DisplayName("Assigned Company")]
        public string Company { get; set; }
        public List<SelectListItem> companies { get; set; }
        public IList<SelectListItem> Countries { get; set; }
        public string Password {  get; set; }
        public string? Module {  get; set; }
    }

    public class UpdateRMTeamModel
    {
        public int Id {  get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please Provide First Name.")]
        public string Firstname { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Provide Last Name.")]
        public string Lastname { get; set; }
        [DisplayName("Email Address")]
        [Required(ErrorMessage = "Please Provide Email Address.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please provide valid email address.")]
        public string EmailAddress { get; set; }
        [DisplayName("Country")]
        public string MobileCode { get; set; }
        [DisplayName("Mobile No.")]
        [Required(ErrorMessage = "Please Provide Mobile No.")]
        public string MobileNumber { get; set; }
        [DisplayName("Country of Residence")]
        public string CountryofResidence { get; set; }
        public string? PassportNumber { get; set; }
        public string? PassportExpiryDate { get; set; }
        public string? VisaExpiryDate { get; set; }
        public string Designation { get; set; }
        public string? InsuranceCompany { get; set; }
        public string? InsuranceExpiryDate { get; set; }
        [DisplayName("Is Login Allowed?")]
        public bool IsLoginAllowed { get; set; }
        [DisplayName("Is Active?")]
        public bool IsActive { get; set; }
        public int UserId { get; set; }

        [DisplayName("Assign To Company")]
        [Required(ErrorMessage = "Please Select Company.")]
        public List<int> CompanyId { get; set; }

        [DisplayName("Assigned Company")]
        public string Company { get; set; }
        public List<SelectListItem> companies { get; set; }
        public IList<SelectListItem> Countries { get; set; }
        public string Password { get; set; }
    }
    public class GetAllRMPersonModel
    {
        public int Id {  get; set; }
        public string RmTeamName { get; set; }
    }
}
