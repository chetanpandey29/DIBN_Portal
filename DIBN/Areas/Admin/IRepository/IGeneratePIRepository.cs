using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IGeneratePIRepository
    {
        string SaveCompanyInvoiceDetails(List<SaveGeneratePIInvoiceData> model);
        List<GetAllNewCompanyInvoices> GetAllCompanyInvoice();
        GetNewCompanyInvoiceDeatils GetInvoiceDeatils(string InvoiceNumber);
        List<string> GetTaskList(string prefix);
        int DeleteNewCompanyPI(string InvoiceNumber, int UserId);
        NewCompanyInvoiceModel GetNewInvoiceDetailsById(int Id);
        int UpdateCompanyInvoiceDetails(NewCompanyInvoiceModel model);
        int RemoveInvoiceDetails(string InvoiceNumber, string Amount, string Vat, int Id, int UserId);
        NewCompanyInvoiceModel GetCompanyInvoiceDetailsByInvoice(string Invoice);
        GetNewCompanyInvoicesWithPagination GetAllCompanyInvoiceWithPagination(int page, int pageSize, string searchString, string sortBy, string sortDirection);
        Task<CheckWhetherNewInvoiceDetailsIsDeletedModel> CheckWhetherInvoiceDetailsIsDeleted(int Id);
        Task<CheckWhetherNewInvoiceDetailsIsDeletedModel> CheckWhetherInvoiceIsDeleted(string invoiceNumber);
    }
}
