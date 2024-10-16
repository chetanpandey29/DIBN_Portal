using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Areas.Admin.Models
{
    public class DocumentsViewModel
    {
        public IFormFile formFile { get; set; }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string BinaryData { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
        public string Module { get; set; }
        public string Role { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string AuthorityName { get; set; }
        public int SelectedDocumentType { get; set; }
        public string DocumentTypeName { get; set; }
        public string actionName { get; set; }
        public IList<SelectListItem> DocumentTypes { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UploadUserDocuments
    {
        public IFormFile formFile { get; set; }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string BinaryData { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
        public string Module { get; set; }
        public string Role { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int SelectedDocumentType { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string AuthorityName { get; set; }
        public string actionName { get; set; }
        public int uploaded { get; set; }
        public bool IsActive {  get; set; }
        public List<DocumentsViewModel> userDocuments { get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class GetCompanyDocuments
    {
        public List<string> allowedModule {  get; set; }
        public IFormFile formFile { get; set; }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string BinaryData { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
        public string Module { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public int UserId { get; set; }
        [DisplayName("Document Type")]
        [Required(ErrorMessage = "Please Select Document Type.")]
        public int SelectedDocumentType { get; set; }
        [DisplayName("Issue Date")]
        public string IssueDate { get; set; }
        [DisplayName("Expiry Date")]
        public string ExpiryDate { get; set; }
        [DisplayName("Authority Name")]
        [Required(ErrorMessage = "Please enter Authority Name.")]
        public string AuthorityName { get; set; }
        public List<DocumentsViewModel> CompanyDocuments { get; set; }
        public IList<SelectListItem> DocumentTypes { get; set; }
        public int CreatedBy { get; set; }
    }
    public class CompanyAssociationData
    {
        public int Users { get; set; }
        public int Shareholders { get; set; }
        public int Documents { get; set; }
    }
}
