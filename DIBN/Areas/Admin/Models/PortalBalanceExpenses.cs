using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Areas.Admin.Models
{
    public class PortalBalanceExpenses
    {
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
        [DisplayName("Vat")]
        public string Vat { get; set; }
        [DisplayName("Vat Amount")]
        public string VatAmount { get; set; }
        [DisplayName("Grand Total")]
        public string GrandAmount { get; set; }
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
        public int UserId { get; set; }
        [DisplayName("Upload Receipt")]
        public IFormFile FormFile { get; set; }
    }

    public class GetPortalBalanceDetails
    {
        public int Id { get; set; }
        public string PortalBalance { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserEmailId { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
    }

    public class GetAllExpenses
    {
        public GetPortalBalanceDetails getPortalBalanceDetails { get; set; }
        public List<PortalBalanceExpenses> portalBalanceExpenses { get; set; }
        public List<PaymentTransaction> paymentTransactions { get; set; }
        public int CompanyId { get; set; }
        public string Module { get; set; }
        public string Company { get; set; }
        public string? message { get; set; }

    }
    public class PortalBalanceViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string BalanceAmount { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public bool IsCompletelyUsed { get; set; }

    }
    public class PaymentTransection
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("User Id")]
        public int UserId { get; set; }
        [DisplayName("Transection Done By")]
        public string Username { get; set; }
        [DisplayName("Company Id")]
        public int CompanyId { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Amount")]
        public string Amount { get; set; }
        [DisplayName("Payment Status")]
        public string PaymentStatus { get; set; }
        [DisplayName("Transection Date")]
        public string TransectionDate { get; set; }
        [DisplayName("Created On")]
        public string CreatedOnUtc { get; set; }
        [DisplayName("Modify On")]
        public string ModifyOnUtc { get; set; }
    }
    public class AddNewCompanyExpenses
    {
        public string Task { get; set; }
        public string Amount { get; set; }
        public string Quantity { get; set; }
        public string TotalAmount { get; set; }
        public string CompanyId { get; set; }
        public string RemainingBalance { get; set; }
        public string name { get; set; }
        public string Date { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string GrandTotal { get; set; }
    }

    public class ExpenseReceipt
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public byte[] DataBinary { get; set; }
        public int ReceiptId { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
    }
    public class PaymentTransaction
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("User Id")]
        public int UserId { get; set; }
        [DisplayName("Transection Done By")]
        public string Username { get; set; }
        [DisplayName("Company Id")]
        public IList<SelectListItem> Companies { get; set; }
        public int CompanyId { get; set; }
        public int PreviousCompanyId { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Amount")]
        public string Amount { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Total Amount")]
        public string TotalAmount { get; set; }
        [DisplayName("Payment Mode")]
        public string PaymentMode { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Payment Status")]
        public string PaymentStatus { get; set; }
        [DisplayName("Transection Date")]
        public string TransectionDate { get; set; }
        [DisplayName("Created On")]
        public string CreatedOnUtc { get; set; }
        [DisplayName("Modify On")]
        public string ModifyOnUtc { get; set; }
        public int ExpenseReceiptId { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public int PaymentReceiptId { get; set; }
        public string Through { get; set; }
        public string TransactionId { get; set; }
        public string OnAccount { get; set; }
        public string PaymentType { get; set; }
        public IList<SelectListItem> expenseType { get; set; }
        public IList<SelectListItem> PaymentModes { get; set; }
        [DisplayName("Type")]
        public string Type { get; set; }
        [DisplayName("Document")]
        public IFormFile formFile { get; set; }
        [DisplayName("Vat")]
        public string Vat { get; set; }
        [DisplayName("Vat Amount")]
        public string VatAmount { get; set; }
        [DisplayName("Grand Total")]
        public string GrandTotal { get; set; }
        public string PreviousAmount { get; set; }
        public string Module { get; set; }
    }
    public class GetAccountHistory
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public int Quantity { get; set; }
        public string TotalAmount { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string GrandTotal { get; set; }
        public string ExpenseType { get; set; }
        public string TransactionDate { get; set; }
        public string CreatedOn { get; set; }
        public string CompanyName { get; set; }
        public string Username { get; set; }
        public string Module { get; set; }
    }
    public class ResponseModel
    {
        public string Username { get; set; }
        public string ModifyTime { get; set; }
        public int ResponseData { get; set; }
        public string TransactionId { get; set; }
    }
    public class ConfirmationModel
    {
        public string? message { get; set; }
        public string? emailAddress { get; set; }
    }
}
