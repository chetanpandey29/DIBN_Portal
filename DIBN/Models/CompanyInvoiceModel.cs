using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DIBN.Models
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
        public string InvoiceDate { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
    }

    public class GetInvoiceDeatils
    {
        public bool IsTRN { get; set; }
        public string TRN { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string TRNCreationDate { get; set; }
        public string Currency { get; set; }
        public string InvoiceCreatedOn { get; set; }
        public string CompanyName { get; set; }
        public string GrandTotal { get; set; }
        public string InvoiceNumber { get; set; }
        public string TotalTaxableAmount { get; set; }
        public string Date { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string Service { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
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
        public string Currency { get; set; }
        public string Service { get; set; }
    }
    public class GetPIAndFinalInvoices
    {
        public List<GetAllCompanyInvoices> getAllCompanyInvoices { get; set; }
        public List<SaveFinalPdf> saveFinalPdfs { get; set; }
        public string Module { get; set; }
        public int CompanyId { get; set; }
    }
    public class SaveData
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public float Amount { get; set; }
        public int Quantity { get; set; }
        public float TotalAmount { get; set; }
        public float GrandTotal { get; set; }
        public int CompanyId { get; set; }
        public string InvoiceNumber { get; set; }
        public int Vat { get; set; }
        public float VatAmount { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
        public string Service { get; set; }
    }
    public class SaveFinalPdf
    {
        public int Id { get; set; }
        public string PdfName { get; set; }
        public string Extension { get; set; }
        public byte[] DataBinary { get; set; }
        public string InvoiceNumber { get; set; }
        public string CreatedOn { get; set; }
        public string CompanyName { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
    }

    public class GetCompanyProformaInvoiceByCompanyIdWithPaginationModel
    {
        public GetCompanyProformaInvoiceByCompanyIdWithPaginationModel()
        {
            invoices = new List<GetAllCompanyInvoices>();
        }
        public int totalInvoices { get; set; }
        public List<GetAllCompanyInvoices> invoices { get; set; }
    }

    public class GetCompanyFinalInvoiceByCompanyIdWithPaginationModel
    {
        public GetCompanyFinalInvoiceByCompanyIdWithPaginationModel()
        {
            invoices = new List<SaveFinalPdf>();
        }
        public int totalInvoices { get; set; }
        public List<SaveFinalPdf> invoices { get; set; }
    }
}
