using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Models
{
    public class MainEmployeeServicesModel
    {
        public List<EmployeeServices> employeeService { get;set; }
        public List<string> allowedModule {  get;set; }
    }
    public class EmployeeServices
    {
        public int ID { get; set; }
        public string SerialNumber { get; set; }
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string Module { get; set; }
        public int ParentId { get; set; }
        public int CompanyId { get; set; }
        public int SendCompanyId { get; set; }
        public string CompanyType { get; set; }
        public bool HasMultipleForm { get; set; }
        public string FormConstrains { get; set; }
        public List<EmployeeServices> getChildEmployeeService { get; set; }
    }

    public class EmployeeServiceRequest
    {
        [DisplayName("ID")]
        public int ID { get; set; }
        [DisplayName("Company Id")]
        public int CompanyId { get; set; }
        [DisplayName("User Id")]
        public int UserId { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Username")]
        public string Username { get; set; }
        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }
        [DisplayName("Request Code")]
        public string RequestNumber { get; set; }
        [DisplayName("Request Status")]
        public int RequestStatusId { get; set; }
        [DisplayName("Service Id")]
        public int ServiceId { get; set; }
        [DisplayName("Service")]
        public string ServiceName { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Request Status")]
        public string RequestStatus { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [DisplayName("IsDelete")]
        public bool IsDelete { get; set; }
        [DisplayName("Requested On")]
        public string CreatedOnUtc { get; set; }
        [DisplayName("Last Modify On")]
        public string ModifyOnUtc { get; set; }
        [DisplayName("Module")]
        public string Module { get; set; }
        [DisplayName("FileName")]
        public string FileName { get; set; }
        public string Extension { get; set; }
        public int EmployeeServiceRequestId { get; set; }
        public byte[] DataBinary { get; set; }
        public List<IFormFile> FormFile { get; set; }
        public List<EmployeeServiceRequestDocument> DocumentList { get; set; }
        public List<GetResponseByParentId> getResponseByParentIds { get; set; }
        public int ParentId { get; set; }
        public string NewRequestStatus { get; set; }
        public string[] ChangedRequestStatus { get; set; }
    }
    public class GetResponseByParentId
    {
        public int ParentId { get; set; }
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string Username { get; set; }
        public string SerialNumber { get; set; }
        public string RequestNumber { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RequestStatus { get; set; }
        public List<EmployeeServiceRequestDocument> DocumentList { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
    }
    public class EmployeeServiceRequestDocument
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public int EmployeeServiceRequestId { get; set; }
        public byte[] DataBinary { get; set; }
        public List<IFormFile> FormFile { get; set; }
    }
}
