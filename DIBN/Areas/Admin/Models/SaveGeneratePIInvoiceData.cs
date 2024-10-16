using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class SaveGeneratePIInvoiceData
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCountry { get; set; }
        public string ContactNumber { get; set; }
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
        public string Module { get; set; }
        public int Count { get; set; }
    }
    public class NewCompanyInvoiceModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCountry { get; set; }
        public string ContactNumber { get; set; }
        public string ProductName { get; set; }
        public string Amount { get; set; }
        public string TotalAmount { get; set; }
        public int Quantity { get; set; }
        public string GrandTotal { get; set; }
        public string Module { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string Service { get; set; }
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string Currency { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public IList<SelectListItem> countryList { get; set; }

        public IList<SelectListItem> currencyList { get; set; }
    }
    public class GetAllNewCompanyInvoices
    {
        public string CompanyName { get; set; }
        public string InvoiceNumber { get; set; }
        public string Date { get; set; }
        public int CreatedBy { get; set; }
        public string Username { get; set; }
        public string TotalAmount { get; set; }
        public string Service { get; set; }
        public string Module { get; set; }
        public string Currency { get; set; }
    }
    public class GetNewCompanyInvoiceDeatils
    {
        public string CompanyName { get; set; }
        public bool IsTRN { get; set; }
        public string TRN { get; set; }
        public string TRNCreationDate { get; set; }
        public string InvoiceCreatedOn { get; set; }
        public string Currency { get; set; }
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
        public string Module { get; set; }
        public string message { get; set; }
        public List<NewCompanyInvoiceModel> invoiceModels { get; set; }
        public List<NewCompanyInvoiceVatDetails> invoiceVatDetails { get; set; }
    }
    public class NewCompanyInvoiceVatDetails
    {
        public string Vat { get; set; }
        public string VatAmount { get; set; }
        public string TotalAmount { get; set; }
        public int Id { get; set; }
    }
    public class GetNewCompanyInvoicesWithPagination
    {
        public List<GetAllNewCompanyInvoices> getAllNewCompanyInvoices { get; set; }
        public int totalInvoices { get; set; }
    }
    public class CheckWhetherNewInvoiceDetailsIsDeletedModel
    {
        public bool IsDelete { get; set; }
        public string ModifyBy { get; set; }
        public string ModifyOnDate { get; set; }
        public string ModifyOnTime { get; set; }
    }
}
