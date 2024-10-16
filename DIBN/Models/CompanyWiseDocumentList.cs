using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using static DIBN.Models.UserEmployeeViewModel;

namespace DIBN.Models
{
    public class CompanyWiseDocumentList
    {
        public List<CompanyDocumentTypeModel> companyDocumentTypes { get; set; }
        public List<UserDocumentsViewModel> CompanyDocuments { get; set; }
        public int CompanyId { get; set; }
        public string CompanyType { get; set; }
        public string Company { get; set; }
        public List<SelectListItem> DubaiMainlandEmpDocumentType { get; set; }
        public List<string> FreezoneEmpDocumentType { get; set; }
        public List<ShareholderDocuments> shareholderDocuments { get; set; }
        public List<GetActiveEmployees> GetActiveEmployees { get; set; }
        public List<GetInActiveEmployees> GetInActiveEmployees { get; set; }
        public List<ShareholderViewModel> shareholders { get; set; }
        public List<GetActiveEmployeesWithDocument> GetActiveEmpDocuments { get; set; }
        public List<GetInActiveEmployeesWithDocument> GetInActiveEmpDocuments { get; set; }
        public List<GetShareholderWithDocument> GetshareholderDocuments { get; set; }
        public List<companyDocumentUploaded> getCompanyUploadedDocument { get; set; }
        public string Module { get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class companyDocumentUploaded
    {
        public string Document { get; set; }
        public string uploaded { get; set; }
    }

    public class GetActiveEmployeesWithDocument
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Uploaded { get; set; }
        public string Designation { get; set; }
        public List<UserDocumentsViewModel> empDocuments { get; set; }
        public List<GetActiveEmpDocumentDetails> documentDetails { get; set; }

    }

    public class GetInActiveEmpDocumentDetails
    {
        public string Uploaded { get; set; }
        public string Document { get; set; }
    }
    public class GetInActiveEmployeesWithDocument
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Uploaded { get; set; }
        public string Designation { get; set; }
        public List<UserDocumentsViewModel> empDocuments { get; set; }
        public List<GetInActiveEmpDocumentDetails> documentDetails { get; set; }

    }

    public class GetActiveEmpDocumentDetails
    {
        public string Uploaded { get; set; }
        public string Document { get; set; }
    }
    public class GetShareholderWithDocument
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<ShareholderDocuments> shareholderDocuments { get; set; }

    }
}
