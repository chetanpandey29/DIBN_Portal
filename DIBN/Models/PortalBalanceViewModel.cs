using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Models
{
    public class PortalBalanceViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string BalanceAmount { get; set; }
        public string RemainingPortalBalance { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public bool IsCompletelyUsed { get; set; }

    }
    public class PortalBalanceExpenses
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Title { get; set; }
        public string Amount { get; set; }
        public int Quantity { get; set; }
        public string TotalAmount { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string RemainingPortalBalance { get; set; }
        public string Module { get; set; }
        public int ExpenseReceiptId { get; set; }
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
        public int ExpenseReceiptId { get; set; }
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

    public class GetExpensesWithTransaction
    {
        public List<PaymentTransection> paymentTransections { get; set; }
        public List<PortalBalanceExpenses> portalBalanceExpenses { get; set; }
    }
}
