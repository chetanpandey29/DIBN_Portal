using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Areas.Admin.Models
{
    public class MessageTemplateViewModel
    {
        public int Id { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage = "Please Provide Name of this Email.")]
        public string Name { get; set; }
        [DisplayName("Subject")]
        [Required(ErrorMessage = "Please Provide Subject of this Email.")]
        public string Subject { get; set; }
        [DisplayName("Mail From")]
        [Required(ErrorMessage = "Please Provide Email From Which You want to Send this Mail.")]
        public string FromMail { get; set; }
        [DisplayName("Body")]
        [Required(ErrorMessage = "Please Provide Body of Your Email.")]
        public string Body { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [DisplayName("Is Delete")]
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        public int CreatedBy { get; set; }
        public int ModifyBy { get; set; }
        public string Module { get; set; }
        public int CompanyId { get; set; }
    }


    public class EmailViewModel
    {
        [DisplayName("Subject")]
        [Required(ErrorMessage = "Please Provide Subject of this Email.")]
        public string Subject { get; set; }
        [DisplayName("Mail From")]
        [Required(ErrorMessage = "Please Provide Email From Which You want to Send this Mail.")]
        public string FromMail { get; set; }
        [DisplayName("Password of Mail from which you are sending Email")]
        [Required(ErrorMessage = "Please Provide Password of Email From Which You want to Send this Mail.")]
        public string Password { get; set; }
        [DisplayName("Mail To")]
        [Required(ErrorMessage = "Please Provide Email To Which You want to Send this Mail.")]
        public string ToMail { get; set; }
        [DisplayName("Body")]
        [Required(ErrorMessage = "Please Provide Body of Your Email.")]
        public string Body { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        public int CreatedBy { get; set; }
        public int ModifyBy { get; set; }
        public string Module { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public List<IFormFile> Documents { get; set; }
    }
    public class MailHistory {
        [DisplayName("Subject")]
        [Required(ErrorMessage = "Please Provide Subject of this Email.")]
        public string Subject { get; set; }
        [DisplayName("Mail From")]
        [Required(ErrorMessage = "Please Provide Email From Which You want to Send this Mail.")]
        public string FromMail { get; set; }
        [DisplayName("Password of Mail from which you are sending Email")]
        [Required(ErrorMessage = "Please Provide Password of Email From Which You want to Send this Mail.")]
        public string Password { get; set; }
        [DisplayName("Mail To")]
        [Required(ErrorMessage = "Please Provide Email To Which You want to Send this Mail.")]
        public string ToMail { get; set; }
        [DisplayName("Body")]
        [Required(ErrorMessage = "Please Provide Body of Your Email.")]
        public string Body { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        public int CreatedBy { get; set; }
        public int ModifyBy { get; set; }
        public string Module { get; set; }
        public int CompanyId { get; set; }
        public int Id { get; set; }
        [DisplayName("Company")]
        public string Company { get; set; }
        [DisplayName("Mail Send By ")]
        public string Username { get; set; }
        public List<IFormFile> Documents { get; set; }
        [DisplayName("Company")]
        public List<SelectListItem> companies { get; set; }
        public List<EmailDocument> emailDocuments { get; set; }
    }
    public class EmailDocument
    {
        public int Id { get; set; }
        public int EmailId { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public byte[] DataBinary { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }   
        public int ModifyBy { get; set; }
        public string ModifyOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }

}
