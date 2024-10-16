using System.Collections.Generic;

namespace DIBN.Models
{
    public class MainCompanyServicesModel
    {
        public List<CompanyServices> companyServices { get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class CompanyServices
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
        public string CompanyType { get; set; }
        public bool HasMultipleForm { get; set; }
        public string FormConstrains { get; set; }
        public int CompanyId { get; set; }
        public int SendCompanyId { get; set; }
        public List<CompanyServices> getChildCompanyServices { get; set; }
    }
    public class CompanyServiceRequest
    {
        public int ID { get; set; }
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
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string Module { get; set; }
    }

    public class GetCompanyServiceRequest
    {
        public int ServiceId { get; set; }
        public string SerialNumber { get; set; }
        public string Form { get; set; }
        public string CompanyName { get; set; }
        public string RequestedBy { get; set; }
    }
    public class GetCompanyServiceRequestDetails
    {
        public string SerialNumber { get; set; }
        public string Form { get; set; }
        public string CompanyName { get; set; }
        public string RequestedBy { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FileName { get; set; }
        public byte[] FieldFileValue { get; set; }
        public string CreatedOn { get; set; }
    }

    public class CompanyRequestDetails
    {
        public List<GetCompanyServiceRequestDetails> companyRequest { get; set; }
        public string Module { get; set; }
    }
}
