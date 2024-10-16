using DIBN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.IService
{
    public interface IPortalBalanceService
    {
        int AddPortalBalance(string BalanceAmount, int CompanyId, int UserId);
        PortalBalanceViewModel GetPortalBalance(int CompanyId);
        List<PortalBalanceExpenses> GetAllExpensesForCompany(int CompanyId);
        List<PaymentTransection> GetTransectionsDetails(int CompanyId);
        ExpenseReceipt GetCompanyExpenseReceipt(int ReceiptId);
        CompanyAccountDetails GetCompanyAccount(int CompanyId, string FromDate, string ToDate);
        int AddCompanyExpensesAccount(List<SaveCompanyExpenses> expenses);
        GetPaymentTransactionReceipt GetPaymentTransactionReceiptDetails(int Id);
        List<GetPaymentTransactionReceipt> GetAllPaymentTransactionReceipt(int CompanyId);
        Task<GetPaymentReceiptsByCompanyIdWithPaginationModel> GetPaymentReceiptsByCompanyIdWithPagination(int CompanyId, int skipRows, int takeRows, string searchBy, string searchValue, string sortBy, string sortingDirection);
        List<string> GetTaskList(string prefix);
    }
}
