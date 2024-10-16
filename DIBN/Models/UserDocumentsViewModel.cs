using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DIBN.Models
{
    public class UserDocumentsViewModel
    {
        public IFormFile formFile { get; set; }
        public string Module { get; set; }
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string BinaryData { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string AuthorityName { get; set; }
        public int SelectedDocumentType { get; set; }
        public string DocumentTypeName { get; set; }
    }

    public class GetUserDocuments
    {
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public string Module { get; set; }
        public int SelectedDocumentId { get; set; }
        public string UserType { get; set; }
        public List<UserDocumentsViewModel> Documents { get; set; }
    }
}
