using DIBN.Models;
using System.Collections.Generic;

namespace DIBN.IService
{
    public interface IAccountManagementService
    {
        int AddCompanyExpensesAccount(List<SaveCompanyExpenses> expenses);
        string UpdateExpenseDetail(UpdateCompanyExpenses model);
        UpdateCompanyExpenses GetExpenseDetails(int Id);
        List<GetHistoryOfCompanyExpenses> GetHistoryOfAllCompanyExpenses(string FromDate, string ToDate);
        string DeleteExpenses(int Id, string Amount, int CompanyId, int UserId);
        PaymentTransaction GetDetailsOfPayment(int Id, int CompanyId);
        string UpdatePaymentDetails(PaymentTransaction paymentTransaction);
        string DeletePaymentTransaction(int Id, int CompanyId, string Amount, int UserId);
        ExpenseReceipt GetCompanyExpenseReceipt(int ReceiptId);
        GetPaymentTransactionReceipt GetPaymentTransactionReceiptDetails(int Id);
        GetAccountHistoryData GetHistoryOfAllCompanyExpensesTest(int pageNumber, int fetchRows, string searchKey, string sortColumn, string sortDirection);
        GetAccountHistoryData GetHistoryOfAllCompanyExpensesFilter(int skip, int take,string? searchBy ,string? searchValue, string sortColumn, string sortDirection);
        ResponseModel GetExpenseModificationDetails(int Id);
        ResponseModel GetTransactionModificationDetails(int Id);
    }
}
