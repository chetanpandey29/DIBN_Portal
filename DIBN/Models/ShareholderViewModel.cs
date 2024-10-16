using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Models
{
    public class ShareholderViewModel
    {
        [DisplayName("ID")]
        public int ID { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please Enter First Name.")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Enter Last Name.")]
        public string LastName { get; set; }
        [DisplayName("Passport Number")]
        [Required(ErrorMessage = "Please Enter Passport Number.")]
        public string PassportNumber { get; set; }
        [DisplayName("Nationality")]
        [Required(ErrorMessage = "Please Enter Nationality.")]
        public string Nationality { get; set; }
        [DisplayName("Designation")]
        [Required(ErrorMessage = "Please Enter Designation.")]
        public string Designation { get; set; }
        [DisplayName("Visa Expiry Date")]
        [Required(ErrorMessage = "Please Enter Visa Expiry Date.")]
        public string VisaExpiryDate { get; set; }
        [DisplayName("Insurance Company")]
        [Required(ErrorMessage = "Please Enter Insurance Company Name.")]
        public string InsuranceCompany { get; set; }
        [DisplayName("Insurance Expiry Date")]
        [Required(ErrorMessage = "Please Enter Insurance Expiry Date.")]
        public string InsuranceExpiryDate { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [DisplayName("IsDelete")]
        public bool IsDelete { get; set; }
        [DisplayName("Created On Utc")]
        public string CreatedOnUtc { get; set; }
        [DisplayName("Modify On Utc")]
        public string ModifyOnUtc { get; set; }
        [DisplayName("Module")]
        public string Module { get; set; }
        public string Share { get; set; }
        [DisplayName("Passport Expiry Date")]
        public string PassportExpiryDate { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyType { get; set; }
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
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string CompanyName { get; set; }
        public string CompanyType { get; set; }
    }
}
