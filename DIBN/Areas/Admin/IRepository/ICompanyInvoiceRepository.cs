using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface ICompanyInvoiceRepository
    {
        string SaveCompanyInvoiceDetails(List<SaveInvoiceData> model);
        string GetLastInvoiceNumber();
        GetInvoiceDeatils GetInvoiceDeatils(string InvoiceNumber);
        List<GetAllCompanyInvoices> GetAllCompanyInvoice(int companyId);
        int UpdateCompanyInvoiceDetails(CompanyInvoiceModel model);
        CompanyInvoiceModel GetCompanyInvoiceDetailsById(int Id);
        int SaveCompanyFinalInvoice(SaveFinalPdf pdf);
        List<SaveFinalPdf> GetAllFinalPdf(int companyId);
        SaveFinalPdf GetFinalPdfById(int Id);
        int RemovePI(string InvoiceNumber, int UserId);
        int RemoveFinalInvoice(string InvoiceNumber, int UserId);
        int FindFinalInvoice(string InvoiceNumber);
        List<string> GetTaskList(string prefix);
        List<string> GetServicesList(string prefix);
        CompanyInvoiceModel GetCompanyInvoiceDetailsByInvoice(string Invoice);
        int RemoveInvoiceDetails(string InvoiceNumber, string Amount, string Vat, int Id, int UserId);
        Task<GetProformaInvoiceWithPaginationModel> GetAllCompanyInvoiceWithPagination(int? companyId, int page, int pageSize, string searchString, string sortBy, string sortDirection);
        Task<GetFinalInvoiceWithPaginationModel> GetAllCompanyFinalInvoice(int? companyId,int page, int pageSize, string searchString, string sortBy, string sortDirection);
        Task<CheckWhetherInvoiceDetailsIsDeletedModel> CheckWhetherInvoiceDetailsIsDeleted(int Id);
        Task<CheckWhetherInvoiceDetailsIsDeletedModel> CheckWhetherInvoiceIsDeleted(string invoiceNumber);
    }
}
