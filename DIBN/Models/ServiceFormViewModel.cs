using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Models
{
    public class ServiceFormViewModel
    {
        public int Id { get; set; }
        public string FormName { get; set; }
        public int ServiceId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int UserId { get; set; }
    }
    public class FormDetail
    {
        public int ServiceFormId { get; set; }
        public int CountOfFields { get; set; }
        public int FormId { get; set; }
        public string FormName { get; set; }
    }
    public class ServiceFormFieldModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int FormId { get; set; }
        public int FieldNumber { get; set; }
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsDocumentUpload { get; set; }
        public string IsRequiredMessage { get; set; }
        public bool IsFileUpload { get; set; }
        public int UserId { get; set; }
        public string Module { get; set; }
        public string FormName { get; set; }
        public string SerialNumber { get; set; }
        public List<GetServiceFormFieldModel> getServiceFormFields { get; set; }
        public bool AllowDisplay { get; set; }
        public string DisplayName { get; set; }
        public bool HasMultipleForms { get; set; }
        public string FormConstrains { get; set; }
        public string CompanyType { get; set; }
        public int CompanyId { get; set; }
        public string serviceName { get; set; }
    }
    public class GetServiceFormFieldModel
    {
        public int FieldId { get; set; }
        public int FieldNumber { get; set; }
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
        public string IsRequiredMessage { get; set; }
        public bool IsDocumentUpload { get; set; }
        public int ServiceId { get; set; }
        public int FormId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string ModifyOn { get; set; }
        public int ModifyBy { get; set; }
        public string FormName { get; set; }
        public bool AllowDisplay { get; set; }
        public string DisplayName { get; set; }
        public bool HasMultipleForms { get; set; }
        public string FormConstrains { get; set; }
        public string CompanyType { get; set; }
    }

    public class SaveFormFieldsModel
    {
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
        public string RequiredMessage { get; set; }
        public bool IsDocumentUpload { get; set; }
        public int ServiceId { get; set; }
        public int FormId { get; set; }
        public int UserId { get; set; }
    }

    public class GetFieldFormData
    {
        public int FieldId { get; set; }
        public string FieldValue { get; set; }
        public int FormId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int SalesPersonId { get; set; }
        public int RMTeamId { get; set; }
        public int ServiceId { get; set; }
        public List<IFormFile> formFile { get; set; }
        public string SerialNumber { get; set; }
        public int Last { get; set; }
        public string serviceName { get; set; }
        public bool IsEmployeeService { get; set; }
        public bool IsCompanyService { get; set; }
    }

    public class GetRequests
    {
        public string SerialNumber { get; set; }
        public string ApplicantName { get; set; }
        public string RequestedService { get; set; }
        public bool IsCompanyService { get; set; }
        public bool IsEmployeeService { get; set; }
        public string CompanyName { get; set; }
        public string RequestedBy { get; set; }
        public string CreatedOn { get; set; }
        public int CompanyId { get; set; }
        public string Module { get; set; }
        public string Role { get; set; }
        public int StatusId { get; set; }   
        public string Status { get; set; }
        public string AssignedUser { get; set; }
        public string SalesPerson { get; set; }

    }

    public class GetCompanyServiceRequestsByCompanyId
    {
        public int totalRecords { get; set; }
        public List<GetRequests> getRequests { get; set; }
    }

    public class GetEmployeeServiceRequestsByCompanyId
    {
        public int totalRecords { get; set; }
        public List<GetRequests> getRequests { get; set; }
    }

    public class GetAssignedCompanyServiceRequests
    {
        public int totalRecords { get; set; }
        public List<GetRequests> getRequests { get; set; }
    }

    public class GetAssignedEmployeeServiceRequests
    {
        public int totalRecords { get; set; }
        public List<GetRequests> getRequests { get; set; }
    }
    public class DownloadServiceRequestUploadedDocumentCmp
    {
        public string FileName { get; set; }
        public byte[] FieldFileValue { get; set; }
    }
    public class GetAllRequests
    {
        public List<GetRequests> getRequests { get; set; }
        public List<SelectListItem> Companies { get; set; }
        public string Company { get; set; }
        public string Module { get; set; }
        public string Role { get; set; }
        public int CompanyId { get; set; }
        public int CurrentCompanyId { get; set; }
        public int SendCompanyId { get; set; }
        public int? SelectedStatus { get; set; }
        public List<string> allowedModule {  get; set; }
    }



    public class GetRequestDetails
    {
        public int ServiceId { get; set; }
        public int FormId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string SerialNumber { get; set; }
        public string RequestedService { get; set; }
        public bool IsCompanyService { get; set; }
        public bool IsEmployeeService { get; set; }
        public string CompanyName { get; set; }
        public string RequestedBy { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedTime { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FileName { get; set; }
        public byte[] FieldFileValue { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
    }
    public class GetRequestResponses
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int FormId { get; set; }
        public string Service { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string RequestedBy { get; set; }
        public int UserId { get; set; }
        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        public int AssignedTo { get; set; }
        [DisplayName("Status")]
        public int StatusId { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }
        [DisplayName("Status")]
        public List<SelectListItem> StatusList { get; set; }
        [DisplayName("Step Id")]
        public int StepId { get; set; }
        [DisplayName("Created By")]
        public int CreatedBy { get; set; }
        [DisplayName("Created On")]
        public string CreatedOn { get; set; }
        [DisplayName("Created Time")]
        public string CreatedTime { get; set; }
        public IFormFile formFile { get; set; }
        public string Module { get; set; }
        public List<ServiceRequestDocument> serviceRequestDocuments { get; set; }
        public string? serviceRequestType { get; set; }
        public bool? isCompany { get; set; }

    }
    public class ServiceRequestDocument
    {
        public int Id { get; set; }
        public int ServiceResponseId { get; set; }
        public string FileName { get; set; }
        public byte[] DataBinary { get; set; }
        public int CompanyId { get; set; }
        public int ServiceId { get; set; }
    }

    public class GetRequestCompleteDetails
    {
        public int CountOfFields { get; set; }
        public List<GetRequestDetails> getRequestDetails { get; set; }
        public List<GetRequestResponses> getRequestResponses { get; set; }
        public int CompanyId { get; set; }
        public string Module { get; set; }
        public string lastStatus { get; set; }
        public string? serviceRequestType { get; set; }
        public bool? isCompany { get; set; }
        public List<string> allowedModule {  get; set; }
    }
}
