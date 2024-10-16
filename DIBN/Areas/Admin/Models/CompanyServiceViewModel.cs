using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Areas.Admin.Models
{
    public class MainCompanyServiceViewModel
    {
        public List<CompanyServiceViewModel> companyServices {  get; set; }
        public List<string> allowedModule {  get; set; }
        public string Module {  get; set; }
    }
    public class CompanyServiceViewModel
    {
        public int ID { get; set; }
        public string SerialNumber { get; set; }
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string Module { get; set; }
        [DisplayName("Company Type")]
        public IList<SelectListItem> CompanyType { get; set; }
        public IList<SelectListItem> ParentCategory { get; set; }
        public string CompanyTypeName { get; set; }
        public int ParentId { get; set; }
        public int UserId { get; set; }
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
}
