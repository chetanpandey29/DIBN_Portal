using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Areas.Admin.Models
{
    public class AccountManagementModel
    {
        public IList<SelectListItem> Companies { get; set; }
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("Company Id")]
        public int CompanyId { get; set; }
        [DisplayName("Task")]
        public string Title { get; set; }
        [DisplayName("Amount")]
        public string Amount { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Total Amount")]
        public string TotalAmount { get; set; }
        [DisplayName("Is Delete")]
        public bool IsDelete { get; set; }
        [DisplayName("Created On")]
        public string CreatedOnUtc { get; set; }
        [DisplayName("Modify On")]
        public string ModifyOnUtc { get; set; }
        [DisplayName("Remaining Portal Balance")]
        public string RemainingPortalBalance { get; set; }
        public int ExpenseReceiptId { get; set; }
        public string Module { get; set; }
        public string CompanyName { get; set; }
    }

    public class SaveCompanyExpenses
    {
        public string Task { get; set; }
        public string Amount { get; set; }
        public string Quantity { get; set; }
        public string TotalAmount { get; set; }
        public string CompanyId { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }
        public string GrandTotal { get; set; }
        public string TransactionId { get; set; }
        public int ExpenseId { get; set; }
    }
    public class GetHistoryOfCompanyExpenses
    {
        public int IndexValue { get; set; }
        public string TransactionId { get; set; }
        public int TransactionIdNo { get; set; }
        public int Id { get; set; }
        public int PaymentTransactionId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string ExpensesTitle { get; set; }
        public string ExpensesAmount { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string Module { get; set; }
        public int ExpenseReceiptId { get; set; }
        public string Type { get; set; }
        public string PaymentCredit { get; set; }
        public string Date { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string GrandTotal { get; set; }
        public string Description { get; set; }
    }
    public class GetHistoryOfCompanyExpensesDatatable
    {
        public int IndexValue { get; set; }
        public string TransactionId { get; set; }
        public int TransactionIdNo { get; set; }
        public int Id { get; set; }
        public int PaymentTransactionId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string ExpensesTitle { get; set; }
        public string ExpensesAmount { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string Module { get; set; }
        public int ExpenseReceiptId { get; set; }
        public string TransactionType { get; set; }
        public string PaymentCredit { get; set; }
        public string Date { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string GrandTotal { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class GetAccountHistoryModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string TransactionId { get; set; }
        public int TransactionIdNo { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public int Quantity { get; set; }
        public string TotalAmount { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public string CreatedOn { get; set; }
        public int ExpenseReceiptId { get; set; }
        public string TransactionType {  get; set; }

    }

    public class GetAccountHistoryDataModel
    {
        public string module { get; set; }
        //public List<(
        //    int IndexValue,int Id,int CompanyId,int ExpenseReceiptId,string CompanyName,string ExpensesTitle,string ExpensesAmount,
        //    int Quantity,double TotalAmount,bool IsDelete,string CreatedOnUtc,int TransactionIdNo,string ModifyOnUtc,string Vat,
        //    string TransactionId,string VatAmount,string GrandTotal,string Type
        //    )> getHistoryOfCompanyExpenses { get; set; }

        public List<GetAccountHistoryModel> getHistoryOfCompanyExpenses { get; set; }
        public int expenseCounts { get; set; }
    }

    public class GetAccountHistoryData
    {
        public string module { get; set; }
        //public List<(
        //    int IndexValue,int Id,int CompanyId,int ExpenseReceiptId,string CompanyName,string ExpensesTitle,string ExpensesAmount,
        //    int Quantity,double TotalAmount,bool IsDelete,string CreatedOnUtc,int TransactionIdNo,string ModifyOnUtc,string Vat,
        //    string TransactionId,string VatAmount,string GrandTotal,string Type
        //    )> getHistoryOfCompanyExpenses { get; set; }

        public List<GetHistoryOfCompanyExpensesDatatable>  getHistoryOfCompanyExpenses{ get; set; }
        public int expenseCounts { get; set; }
    }
    public class UpdateCompanyExpenses
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("Description")]
        public string Task { get; set; }
        [DisplayName("Amount")]
        public string Amount { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Total Amount")]
        public string TotalAmount { get; set; }
        [DisplayName("Company")]
        public int CompanyId { get; set; }
        public int PreviousCompanyId { get; set; }
        [DisplayName("Module")]
        public string Module { get; set; }
        [DisplayName("Vat")]
        public string Vat { get; set; }
        [DisplayName("Vat Amount")]
        public string VatAmount { get; set; }
        [DisplayName("Grand Total")]
        public string GrandTotal { get; set; }
        public string CreatedOnUtc { get; set; }
        public string TransactionId { get; set; }
        [DisplayName("Document")]
        public IFormFile formFile { get; set; }
        public IList<SelectListItem> Companies { get; set; }
        public IList<SelectListItem> expenseType { get; set; }
        public IList<SelectListItem> PaymentModeType { get; set; }
        [DisplayName("Type")]
        public string Type { get; set; }
        public string ActionName { get; set; }
        public string Controller { get; set; }
        [DisplayName("Payment Mode")]
        public string PaymentMode { get; set; }
        public string PreviousAmount { get; set; }
        public int UserId { get; set; }
    }
}
