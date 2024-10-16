using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Areas.Admin.Models
{
    public class ShareholderViewModel
    {
        public int? Index { get; set; }
        [DisplayName("ID")]
        public int ID { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please Enter First Name.")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Enter Last Name.")]
        public string LastName { get; set; }
        [DisplayName("Passport Number")]
        public string PassportNumber { get; set; }
        [DisplayName("Nationality")]
        [Required(ErrorMessage = "Please Enter Nationality.")]
        public string Nationality { get; set; }
        [DisplayName("Designation")]
        [Required(ErrorMessage = "Please Enter Designation.")]
        public string Designation { get; set; }
        [DisplayName("Visa Expiry Date")]
        public string VisaExpiryDate { get; set; }
        [DisplayName("Insurance Company")]
        public string InsuranceCompany { get; set; }
        [DisplayName("Insurance Expiry Date")]
        public string InsuranceExpiryDate { get; set; }
        [DisplayName("Share (In Percentage)")]
        [Required(ErrorMessage = "Please Enter Share Amount.")]
        public string SharePercentage { get; set; }
        [DisplayName("Assign to Company")]
        public IList<SelectListItem> Companies { get; set; }
        [DisplayName("Assign to Company")]
        [Required(ErrorMessage = "Please Select Company from below list.")]
        public int CompanyId { get; set; }
        [DisplayName("Assign to Company")]
        public string Company { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [DisplayName("IsDelete")]
        public bool IsDelete { get; set; }
        [DisplayName("Created On Utc")]
        public string CreatedOnUtc { get; set; }
        [DisplayName("Modify On Utc")]
        public string ModifyOnUtc { get; set; }
        [DisplayName("Passport Expiry Date")]
        public string PassportExpiryDate { get; set; }
        [DisplayName("Module")]
        public string Module { get; set; }
        public string RemainingSharePercentage { get; set; }
        public string PreviousShare { get; set; }
        public IList<SelectListItem> Countries { get; set; }
        public int CreatedBy { get; set; }
    }
    public class MainShareholderDocumentsModel
    {
        public List<string> allowedModule {  get; set; }
        public List<ShareholderDocuments> shareholderDocuments { get; set; }
    }
    public class ShareholderDocuments
    {
        public IFormFile formFile { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public byte[] DataBinary { get; set; }
        public int ShareholderId { get; set; }
        public string Module { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get;set; }
        public string CreatedOnUtc { get;set; }
        public string ModifyOnUtc { get;set; }
        public int CreatedBy { get;set; }
    }

    public class ShareholderWithFilter
    {
        public IList<SelectListItem> Companies { get; set; }
        public List<ShareholderViewModel> ShareholderViewModels { get; set; }
        public int CompanyId { get; set; }
        public string Module { get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class GetAllActiveShareholdersWithPaginationModel
    {
        public List<ShareholderViewModel> shareholders { get; set; }
        public int totalActiveShareholder { get; set; }
    }
    public class GetAllInActiveShareholdersWithPaginationModel
    {
        public List<ShareholderViewModel> shareholders { get; set; }
        public int totalInActiveShareholder { get; set; }
    }
}
