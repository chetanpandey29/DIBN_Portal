using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IPortalBalanceExpensesRepository
    {
        int UpdateRepeatedAccountEntries();
        int AddCompanyExpensesAccountToTemp(SaveCompanyExpenses expense);
        int SaveCompanyExpensesAccountFromTemp(List<int> ids);
        int RemoveCompanyExpensesAccountFromTemp(int id);
        ExpenseReceipt GetCompanyExpenseReceipt(int ReceiptId);
        GetPortalBalanceDetails GetPortalBalanceDetailsForCompany(int CompanyId);
        List<PortalBalanceExpenses> GetAllExpensesForCompany(int CompanyId);
        int AddExpensesOfCompany(List<PortalBalanceExpenses> expenses);
        PortalBalanceExpenses GetPortalBalanceExpenseDetail(int CompanyId, int ExpenseId);
        int UpdateExpenseDetail(PortalBalanceExpenses expenses);
        string DeleteExpenses(int Id, string Amount, int CompanyId, int UserId);
        int AddPortalBalance(string TransactionId, string Amount, string BalanceAmount, string PaymentMode, string Description, int CompanyId, int UserId, string Date, int Quantity, string TotalAmount);
        ExpenseReceipt GetCompanyExpenseReceipt(int ReceiptId, int ExpenseReceiptId);
        List<PaymentTransaction> GetTransectionsDetails(int CompanyId);
        int UpdatePortalBalance(int CompanyId, string Amount);
        GetCompanyAccountDetailModel GetCompanyAccount(int CompanyId, string FromDate, string ToDate);
        int AddCompanyExpensesAccount(List<SaveCompanyExpenses> expenses);
        List<GetHistoryOfCompanyExpenses> GetHistoryOfAllCompanyExpenses(string FromDate, string ToDate);
        List<string> GetTaskList(string prefix);
        UpdateCompanyExpenses GetExpenseDetails(int Id);
        string UpdateExpenseDetail(UpdateCompanyExpenses model);
        PaymentTransaction GetDetailsOfPayment(int Id, int CompanyId);
        string UpdatePaymentDetails(PaymentTransaction paymentTransaction);
        string DeletePaymentTransaction(int Id, int CompanyId, string Amount, int UserId);
        List<GetPaymentTransactionReceipt> GetAllPaymentTransactionReceipt();
        GetPaymentTransactionReceipt GetPaymentTransactionReceiptDetails(int Id);
        GetPaymentTransactionReceipt GetPaymentTransactionReceiptDetailsForEdit(int Id);
        List<GetAccountHistory> GetAccountHistoryDetails(int? CompanyId);
        List<GetPaymentTransactionReceipt> GetPaymentTransactionReceiptHistory();
        GetAccountHistoryDataModel GetHistoryOfAllCompanyExpensesTest(int pageNumber, int fetchRows, string searchKey, string sortColumn, string sortDirection);
        Task<GetPaymentReceiptWithPaginationModel> GetAllPaymentTransactionReceiptWithPagination(int page, int pageSize,string searchBy, string searchString, string sortBy, string sortDirection, int count);
        Task<GetPaymentReceiptWithPaginationModel> GetAllPaymentTransactionReceiptHistoryWithPagination(int page, int pageSize, string searchBy, string searchString, string sortBy, string sortDirection, int count);
        GetAccountHistoryDataModel GetHistoryOfAllCompanyExpensesFilter(int skip, int take, string? searchBy, string? searchValue, string sortColumn, string sortDirection);
        Task<GetCompanyAccountDetailModel> GetCompanyTotalAccount(int CompanyId);
        Task<GetCompanyAccountDetailPaginationModel> GetCompanyAccountWithPagination(int CompanyId, int skipRows, int takeRows, string sortBy, string sortingDirection, string searchBy, string searchPrefix);
        ResponseModel GetExpenseModificationDetails(int Id);
        ResponseModel GetTransactionModificationDetails(int Id);
        Task<GetTemporaryAccountManagementPaginationModel> GetTemporaryAccountManagementPagination(int page, int pageSize, string searchString, string sortBy, string sortDirection);
        Task<int> AddTemporaryAccountExpenses(List<SaveTemporaryAccountExpenseModel> expenses);
        Task<GetTemporaryAccountDetailByIdModel> GetTemporaryAccountDetailById(int Id);
        Task<int> UpdateTemporaryAccountExpenses(GetTemporaryAccountDetailByIdModel expenses);
        Task<int> DeleteTemporaryAccount(int Id, int UserId);
        Task<int> ApproveTemporaryAccount(int Id, int UserId);
        Task<int> RejectTemporaryAccount(int Id, int UserId);
        Task<GetTemporaryAccountManagementLogPaginationModel> GetTemporaryAccountManagementLogPagination(int page, int pageSize, string searchString, string sortBy, string sortDirection);
        string SaveDirectCompanyExpensesAccount(List<SaveCompanyExpenses> expenses);
        Task<GetAccountTypeModel> CheckAccountEntryType(string serialCode);
    }
}
