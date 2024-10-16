using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Areas.Admin.Models
{
    public class ServiceFormModel
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
        public string service { get; set; }
        public List<GetServiceFormFieldModel> getServiceFormFields { get; set; }
        public List<string> allowedModule {  get; set; }
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
        public int StepNumber { get; set; }
    }

    public class SaveFormFieldsModel
    {
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
        public string RequiredMessage { get; set; }
        public bool IsDocumentUpload { get; set; }
        public int StepNumber { get; set; }
        public int ServiceId { get; set; }
        public int FormId { get; set; }
        public int UserId { get; set; }
    }

    public class GetRequestsModel
    {
        public int ServiceId { get; set; }
        public string SerialNumber { get; set; }
        public int SerialNo { get; set; }
        public string RequestedService { get; set; }
        public bool IsCompanyService { get; set; }
        public bool IsEmployeeService { get; set; }
        public string CompanyName { get; set; }
        public string RequestedBy { get; set; }
        public string CreatedOn { get; set; }
        public string AssignedUser { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public string ApplicantName { get; set; }
        public string AssignedOn { get; set; }
        public string SalesPerson { get; set; }
        public int? SelectedStatus { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime AssignOnUtc { get; set;}
    }

    public class GetAllCompanyServiceRequestModel
    {
        public int totalRecords { get; set; }
        public List<GetRequestsModel> getRequests { get; set; }
    }

    public class GetAllEmployeeServiceRequestModel
    {
        public int totalRecords { get; set; }
        public List<GetRequestsModel> getRequests { get; set; }
    }

    public class GetBothRequestModel
    {
        public List<GetRequestsModel> getCompanyRequestsModels { get; set; }
        public List<GetRequestsModel> getEmployeeRequestsModels { get; set; }
        public int? SelectedStatus { get; set; }
        public string Module { get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class GetServiceRequestForCompany
    {
        public List<string> allowedModule {  get; set; }
        public List<GetRequestsModel> getServiceRequest { get; set; }
        public string Company { get; set; }
        public int companyId { get; set; }
    }
    public class SaveAssignUser
    {
        public int ServiceId { get; set; }
        public int[] UserId { get; set; }
        public string SerialNumber { get; set; }
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public int companyId { get; set; }
        public string actionMethod { get; set; }
        public List<GetMainCompanyEmployees> getMainCompanyEmployees { get; set; }
        public List<int> _assignedUsers { get; set; }
        public string? ServiceRequestType { get; set; }
    }
    public class GetRequestDetailsModel
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

    public class DownloadServiceRequestUploadedDocument
    {
        public string FileName { get; set; }
        public byte[] FieldFileValue { get; set; }
    }
    public class GetRequestResponsesModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int FormId { get; set; }
        [DisplayName("Service")]
        public string Service { get; set; }
        public int CompanyId { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Requested By")]
        public string RequestedBy { get; set; }
        public int UserId { get; set; }
        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Assigned To")]
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
        public List<ServiceRequestDocument> serviceRequestDocuments { get; set; }
        public string? serviceRequestType { get; set; }
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
        public List<GetRequestDetailsModel> getRequestDetails { get; set; }
        public List<GetRequestResponsesModel> getRequestResponses { get; set; }
        public string lastStatus { get; set; }
        public int? FilterStatus { get; set; }
        public int companyId { get; set; }
        public string actionMethod { get; set; }
        public string serviceRequestType { get; set; }
        public List<string> allowedModule {  get; set; }
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
}
