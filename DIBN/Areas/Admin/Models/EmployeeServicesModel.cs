using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DIBN.Areas.Admin.Models
{
    public class MainEmployeeServicesModel
    {
        public List<EmployeeServicesModel> employeeService { get; set; }
        public List<string> allowedModule {  get; set; }
        public string Module {  get; set; }
    }

    public class EmployeeServicesModel
    {
        public int ID { get; set; }
        public string SerialNumber { get; set; }
        public string ServiceName { get; set; }
        [DisplayName("Parent Category")]
        public int ParentId { get; set; }
        [DisplayName("Company Type")]
        public IList<SelectListItem> CompanyType { get; set; }
        [DisplayName("Parent Category")]
        public IList<SelectListItem> ParentCategory { get; set; }
        [DisplayName("Company Type")]
        public string CompanyTypeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string Module { get; set; }
        public int UserId { get; set; }
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
        [DisplayName("Service Id")]
        public int ServiceId { get; set; }
        [DisplayName("Service")]
        public string ServiceName { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Message")]
        [Required(ErrorMessage ="Please enter your Request/Response.")]
        public string Description { get; set; }
        [DisplayName("Request Status")]
        public string RequestStatus { get; set; }
        [DisplayName("Request Status")]
        public int RequestStatusId { get; set; }
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
        [DisplayName("Extension")]
        public string Extension { get; set; }
        [DisplayName("Employee Service Request Id")]
        public int EmployeeServiceRequestId { get; set; }
        public byte[] DataBinary { get; set; }
        public List<IFormFile> FormFile { get; set; }
        public List<GetResponseByParentId> getResponseByParentIds { get; set; }
        public List<EmployeeServiceRequestDocument> DocumentList { get; set; }
        public int ParentId { get; set; }
        public string RequestAssignTo { get; set; }
        public string Designation { get; set; }
        public string NewRequestStatus { get; set; }
        public string[] ChangedRequestStatus { get; set; }
    }
    public class GetResponseByParentId
    {
        [DisplayName("Parent Id")]
        public int ParentId { get; set; }
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("Company Id")]
        public int CompanyId { get; set; }
        [DisplayName("User Id")]
        public int UserId { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Username")]
        public string Username { get; set; }
        [DisplayName("Serial Nuber")]
        public string SerialNumber { get; set; }
        [DisplayName("Request Code")]
        public string RequestNumber { get; set; }
        [DisplayName("Service Id")]
        public int ServiceId { get; set; }
        [DisplayName("Service")]
        public string ServiceName { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Message")]
        [Required(ErrorMessage = "Please enter your Request/Response.")]
        public string Description { get; set; }
        [DisplayName("Request Status")]
        public string RequestStatus { get; set; }
        public List<EmployeeServiceRequestDocument> DocumentList { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string RequestAssignTo { get; set; }
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
    public class AssignEmployeeRequest
    {
        public int ID { get; set; }
        public int RequestId { get; set; }
        [DisplayName("Assign Employee Request To")]
        public int[] UserId { get; set; }
        public string RequestNumber { get; set; }
        [DisplayName("Assign Employee Request To")]
        public IList<SelectListItem> Username { get; set; }
        public string Module { get; set; }
    }
}
