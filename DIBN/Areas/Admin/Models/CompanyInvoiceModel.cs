using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class CompanyInvoiceModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Amount { get; set; }
        public string TotalAmount { get; set; }
        public int Quantity { get; set; }
        public string GrandTotal { get; set; }
        public string Module { get; set; }
        public int CompanyId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CompanyName { get; set; }
        public List<SelectListItem> companies { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public string InvoiceDate { get; set; }
        public string Currency { get; set; }
        public string Service { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
    }

    public class GetInvoiceDeatils
    {
        public string CompanyName { get; set; }
        public bool IsTRN { get; set; }
        public string TRN { get; set; }
        public string TRNCreationDate { get; set; }
        public string Currency { get; set; }
        public string InvoiceCreatedOn { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string GrandTotal { get; set; }
        public string InvoiceNumber { get; set; }
        public string Date { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string TotalTaxableAmount { get; set; }
        public string Service { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
        public string? message { get; set; }
        public List<CompanyInvoiceModel> invoiceModels { get; set; }
        public List<InvoiceVatDetails> invoiceVatDetails { get; set; }
    }
    public class InvoiceVatDetails
    {
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string TotalAmount { get; set; }
        public int Id { get; set; }
    }
    public class GetAllCompanyInvoices
    {
        public string CompanyName { get; set; }
        public string InvoiceNumber { get; set; }
        public string Date { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
        public string TotalAmount { get; set; }
        public string Service { get; set; }
        public string Currency { get; set; }
    }
    public class GetPIAndFinalInvoices
    {
        public List<string> allowedModule {  get; set; }
        public List<GetAllCompanyInvoices> getAllCompanyInvoices { get; set; }
        public List<SaveFinalPdf> saveFinalPdfs { get; set; }
        public string Module { get; set; }
        public string Company { get; set; }
        public int CompanyId { get; set; }
    }
    public class SaveData
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public int CompanyId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public int Vat { get; set; }
        public decimal VatAmount { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
        public string Service { get; set; }
    }
    public class SaveInvoiceData
    {
        public string Product { get; set; }
        public string Amount { get; set; }
        public string Quantity { get; set; }
        public string TotalAmount { get; set; }
        public string GrandTotal { get; set; }
        public string CompanyId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string Vat { get; set; }
        public decimal VatAmount { get; set; }
        public string CreatedBy { get; set; }
        public string Username { get; set; }
        public string Service { get; set; }
        public string Currency { get; set; }
        public int Count { get; set; }
    }
    public class SavePdfData
    {
        public string Product { get; set; }
        public string Amount { get; set; }
        public string Quantity { get; set; }
        public string TotalAmount { get; set; }
        public string CompanyId { get; set; }
        public string Vat { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string Service { get; set; }
        public string GrandTotal { get; set; }
        public string VatAmount { get; set; }
        public string CreatedBy { get; set; }
        public string Username { get; set; }
    }
    public class SaveFinalPdf
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string PdfName { get; set; }
        public string Extension { get; set; }
        public byte[] DataBinary { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string CreatedOn { get; set; }
        public string CompanyName { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
    }
    public class GetProformaInvoiceWithPaginationModel
    {
        public List<GetAllCompanyInvoices> getAllCompanyInvoices { get; set; }
        public int totalProformaInvoice { get; set; }
    }
    public class GetFinalInvoiceWithPaginationModel
    {
        public List<SaveFinalPdf> getAllCompanyInvoices { get; set; }
        public int totalFinalInvoice { get; set; }
    }
    public class CheckWhetherInvoiceDetailsIsDeletedModel
    {
        public bool IsDelete { get; set; }
        public string ModifyBy { get; set; }
        public string ModifyOnDate { get; set; }
        public string ModifyOnTime { get; set; }
    }
}
