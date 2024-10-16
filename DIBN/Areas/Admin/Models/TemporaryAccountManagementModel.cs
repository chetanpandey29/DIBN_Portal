using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class TemporaryAccountManagementModel
    {
        public TemporaryAccountManagementModel()
        {
            Companies = new List<SelectListItem>();
        }
        public IList<SelectListItem> Companies { get; set; }
        public int CompanyId { get; set; }
        public string message { get; set; }
    }
    public class GetTemporaryAccountManagementPaginationModel
    {
        public GetTemporaryAccountManagementPaginationModel()
        {
            getTemporaryAccounts = new List<GetTemporaryAccountManagementListModel>();
        }
        public List<GetTemporaryAccountManagementListModel> getTemporaryAccounts { get; set; }
        public int totalAccountCount { get; set; }
    }
    public class GetTemporaryAccountManagementListModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string TransactionId { get; set; }
        public int TransactionIdNo { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public double Vat { get; set; }
        public double VatAmount { get; set; }
        public double GrandTotal { get; set; }
        public string EntryType { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOnUtc { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedOnUtc { get; set; }
        public bool IsRejected { get; set; }
        public string RejectedBy { get; set; }
        public string RejectedOnUtc { get; set; }
    }

    public class SaveTemporaryAccountExpenseModel
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
        public string PaymentMode { get; set; }
        public string CreatedBy { get; set; }
        public string GrandTotal { get; set; }
        public string TransactionId { get; set; }
        public int ExpenseId { get; set; }
    }

    public class GetTemporaryAccountDetailByIdModel
    {
        public GetTemporaryAccountDetailByIdModel()
        {
            ExpenseType = new List<SelectListItem>();
            PaymentModeList = new List<SelectListItem>();
            Companies = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string TransactionId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public double Vat { get; set; }
        public double VatAmount { get; set; }
        public double GrandTotal { get; set; }
        public string EntryType { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionDate { get; set; }
        public int ModifyBy { get; set; }
        public List<SelectListItem> ExpenseType {  get; set; }
        public List<SelectListItem> PaymentModeList {  get; set; }
        public List<SelectListItem> Companies {  get; set; }
    }

    public class GetTemporaryAccountManagementLogPaginationModel
    {
        public GetTemporaryAccountManagementLogPaginationModel()
        {
            logs = new List<GetTemporaryAccountManagementLogModel>();
        }
        public List<GetTemporaryAccountManagementLogModel> logs { get; set; }
        public int totalLogs { get;set; }
    }
    public class GetTemporaryAccountManagementLogModel
    {
        public int Index { get; set; }
        public string message { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyBy { get; set; }
        public string ModifyOn { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedOn { get;set; }
        public string RejectedBy { get; set; }
        public string RejectedOn { get; set; }
        public DateTime CreatedOnUtc { get;set; }
        public DateTime ModifyOnUtc { get;set; }
        public DateTime ApprovedOnUtc { get;set; }
        public DateTime RejectedOnUtc { get;set; }
    }
    public class GetAccountTypeModel
    {
        public string accountType { get; set; }
        public int Id {  get; set; }
        public int CompanyId {  get; set; }
        public string TotalAmount { get; set; }
    }
}
