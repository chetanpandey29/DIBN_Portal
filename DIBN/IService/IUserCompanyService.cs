using DIBN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.IService
{
    public interface IUserCompanyService
    {
        UserCompanyViewModel GetCompanyById(int id);
        List<UserCompanyViewModel> GetComapnyBySalesPersonId(int Id);
        List<GetAllCompanyInvoices> GetAllCompanyInvoice(int CompanyId);
        List<SaveFinalPdf> GetAllFinalPdf(int CompanyId);
        SaveFinalPdf GetFinalPdfById(int Id);
        GetInvoiceDeatils GetInvoiceDeatils(string InvoiceNumber);
        List<UserCompanyViewModel> GetCompanies();
        List<CompanyDocumentTypeModel> GetCompanyDocuments(); 
        List<UserCompanyViewModel> GetCompanyByRMTeamId(int Id);
        Task<GetCompanyProformaInvoiceByCompanyIdWithPaginationModel> GetCompanyProformaInvoiceByCompanyIdWithPagination(int CompanyId, int SkipRows, int RowsOfPage, string searchBy, string searchPrefix, string sortBy, string sortingDirection);
        Task<GetCompanyFinalInvoiceByCompanyIdWithPaginationModel> GetCompanyFinalInvoiceByCompanyIdWithPagination(int CompanyId, int SkipRows, int RowsOfPage, string searchBy, string searchPrefix, string sortBy, string sortingDirection);
    }
}
