using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Models.CompanyViewModel;

namespace DIBN.Areas.Admin.IRepository
{
    public interface ICompanyRepository
    {
        List<CompanyViewModel> GetMainCompany();
        List<CompanyViewModel> GetCompanies();
        int AddNewCompany(SaveNewCompany company);
        CompanyViewModel GetCompanyById(int id);
        SaveCompany GetCompanyDetails(int id);
        int UpdateCompanyDetails(SaveCompany company);
        int DeleteCompany(int id, int UserId);
        List<string> GetAccountNumber(int id);
        int CheckExistanceOfCompany(string name);
        int CheckExistanceOfEmail(string Email);
        List<string> GetSelectedCompanyDetails(int CompanyId);
        DocumentsViewModel DownloadDocument(int Id, int CompanyId);
        List<DocumentsViewModel> GetAllDocuments(int CompanyId, int UserId);
        int UploadSelectedFile(DocumentsViewModel document, int CompanyId);
        List<DocumentsViewModel> GetCompanyDocuments(int CompanyId);
        int UploadCompanyDocuments(GetCompanyDocuments document);
        GetCompanyDocuments DownloadCompanyDocuments(int Id, int CompanyId);
        List<int> GetCompaniesCount();
        List<string> GetAuthorityNames();
        string GetLastAccountNumber();
        int CheckExistanceOfCompanyAccountNumber(string AccountNumber);
        Task<string> sendMail(string CompanyName, string Email, string url);
        Task<string> SendChangePasswordMail(string CompanyName, string OldPassword, string NewPassword, string Email);
        string ChangestatusOfCompany(int ActiveId);
        CompanyAssociationData CheckCompanyAssociation(int CompanyId);
        int RemoveCompanyDocuments(int DocumentId, int UserId);
        Task<string> SendCompanyMail(EmailViewModel model);
        string GetCompanyName(int CompanyId);
        Task<GetCompanyForAccountSummaryWithPagination> GetCompanyDetailForAccountSummary(int skip, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString);
        List<GetCompanyLog> GetAllLogByCompanyId(int companyId);
        Task<GetCompanyDetailsWithPaginationModel> GetCompanyListWithPagination(int page, int pageSize, string sortBy, string sortingDirection, string searchValue);
        List<GetCompanyListForExport> GetCompanyListForExcelExport();
        Task<List<string>> GetCompanySubTypePrefix(string companyType, string prefix);
        Task<List<GetWeeklyCompanySubTypeReportModel>> GetWeeklyCompanySubTypeReport();
        Task<List<GetMonthlyCompanySubTypeReportModel>> GetMonthlyCompanySubTypeReport();
        Task<List<GetYearlyCompanySubTypeReportModel>> GetYearlyCompanySubTypeReport();
    }
}
