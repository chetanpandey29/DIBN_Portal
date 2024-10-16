using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Areas.Admin.Models
{
    public class GetCompanyAccountDetailModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAccountNumber { get; set; }
        public string DataFrom { get; set; }
        public string CompanyAddress { get; set; }
        public string MobileNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public string PortalBalance { get; set; }
        public int totalAccounts { get; set; }
        public string message { get; set; }
        public List<GetexpensesofCompany> getexpensesofCompanies { get; set; }
    }

    public class GetCompanyAccountDetailPaginationModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAccountNumber { get; set; }
        public string DataFrom { get; set; }
        public string CompanyAddress { get; set; }
        public string MobileNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public string PortalBalance { get; set; }
        public int totalAccounts { get; set; }
        public string message { get; set; }
        public List<GetCompanyWiseExpensesWithPagination> getexpensesofCompanies { get; set; }
    }

    public class GetCompanyWiseExpensesWithPagination
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string TransactionId { get; set; }
        public int TransactionIdNo {  get; set; }
        public decimal Balance { get; set; }
        public string PaymentMode {  get; set; }
        public string Description { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Vat {  get; set; }
        public string VatAmount {  get; set; }
        public string Date {  get; set; }
        public string GrandTotal { get; set; }
        public int ExpenseReceiptId {  get; set; }
        public string TransactionType {  get; set; }
    }

    public class GetexpensesofCompany
    {
        public string TransactionId { get; set; }
        public int TransactionIdNo { get; set; }
        public int ExpenseReceiptId { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string GrandTotal { get; set; }
        public string PaymentCredit { get; set; }
        public string Amount { get; set; }
        public decimal Balance { get; set; }
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int PaymentTransactionId { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class GetPaymentTransactionReceipt
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("Company Id")]
        public int CompanyId { get;set; }
        [DisplayName("Payment Id")]
        public int PaymentId { get; set; }
        [DisplayName("Company")]
        public string CompanyName { get; set; }
        [DisplayName("Receipt No.")]
        public string SerialNumber { get; set; }
        [DisplayName("Amount")]
        public string Amount { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Total Amount")]
        public string TotalAmount { get; set; }
        [DisplayName("On Account of")]
        public string OnAccount { get; set; }
        [DisplayName("Through")]
        public string Through { get; set; }
        [DisplayName("Created On")]
        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }

        public string Module { get; set; }
        public string? message { get; set; }
    }
    public class GetPaymentReceiptWithPaginationModel
    {
        public List<GetPaymentTransactionReceipt> getPaymentTransactionReceipts { get; set; }
        public int totalPaymentReceipts { get; set; }
    }
}
