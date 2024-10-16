using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace DIBN.Areas.Admin.Models
{
    public class SupportTicketRequest
    {
        [DisplayName("Id")]
        public int ID { get; set; }
        [DisplayName("Company Id")]
        public int CompanyId { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("User Id")]
        public int UserId { get; set; }
        [DisplayName("Username")]
        public string Username { get; set; }
        [DisplayName("Tracking Number")]
        public string TrackingNumber { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Ticket Status")]
        public int TicketStatusId { get; set; }
        [DisplayName("Ticket Status")]
        public string TicketStatus { get; set; }
        [DisplayName("Ticket Status")]
        public string NewTicketStatus { get; set; }
        [DisplayName("Parent Id")]
        public int ParentId { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [DisplayName("IsDelete")]
        public bool IsDelete { get; set; }
        [DisplayName("Created On")]
        public string CreatedOn { get; set; }
        [DisplayName("Assigned On")]
        public string AssignedOn { get; set; }
        [DisplayName("Created Time")]
        public string CreatedTime { get; set; }
        [DisplayName("Created By")]
        public int CreatedBy { get; set; }
        [DisplayName("Modify On")]
        public string ModifyOn { get; set; }
        [DisplayName("Modify By")]
        public int ModifyBy { get; set; }
        [DisplayName("Upload Document(If Any)")]
        public List<IFormFile> FormFile { get; set; }
        public List<SupportTicketDocunents> DocumentList { get; set; }
        public List<GetSupportTicketResponseByParentId> getResponseByParentIds { get; set; }
        public string Module { get; set; }
        public string[] ChangedRequestStatus { get; set; }
        public string AssignedUser { get; set; }
        public int SalesPersonId { get; set; }
        public int companyId { get; set; }
        public string actionMethod { get; set; }
        public string SalesPersonName { get; set; }
        public int? SelectedStatus { get; set; }
    }

    public class SaveAssignUserOfSupportTikcet
    {
        public int SupportTicketId { get; set; }
        public int[] UserId { get; set; }
        public string TrackingNumber { get; set; }
        public int Id { get; set; }
        public string Module { get; set; }
        public IList<SelectListItem> Users { get; set; }
        public int CreatedBy { get; set; }
        public int companyId { get; set; }
        public string actionMethod { get; set; }
        public List<GetMainCompanyEmployees> getMainCompanyEmployees { get; set; }
    }
    public class GetSupportTicketResponseByParentId
    {
        public int ID { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string TrackingNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TicketStatusId { get; set; }
        public string TicketStatus { get; set; }
        public string NewTicketStatus { get; set; }
        public int ParentId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string ModifyOn { get; set; }
        public int ModifyBy { get; set; }
        public List<SupportTicketDocunents> DocumentList { get; set; }
        public string Module { get; set; }
        public int SalesPersonId { get; set; }
        public string SalesPersonName { get; set; }
    }
    public class SupportTicketDocunents
    {
        public int ID { get; set; }
        public int ReplyId { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string TicketId { get; set; }
        public byte[] DataBinary { get; set; }
        public List<IFormFile> FormFile { get; set; }
        public string Module { get; set; }
    }

    public class GetSupportTickets
    {
        [DisplayName("Id")]
        public int ID { get; set; }
        [DisplayName("Company Id")]
        public int CompanyId { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("User Id")]
        public int UserId { get; set; }
        [DisplayName("Username")]
        public string Username { get; set; }
        [DisplayName("Tracking Number")]
        public string TrackingNumber { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Ticket Status")]
        public int TicketStatusId { get; set; }
        [DisplayName("Ticket Status")]
        public string TicketStatus { get; set; }
        [DisplayName("Ticket Status")]
        public string NewTicketStatus { get; set; }
        [DisplayName("Parent Id")]
        public int ParentId { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [DisplayName("IsDelete")]
        public bool IsDelete { get; set; }
        [DisplayName("Created On")]
        public string CreatedOn { get; set; }
        [DisplayName("Assigned On")]
        public string AssignedOn { get; set; }
        [DisplayName("Created Time")]
        public string CreatedTime { get; set; }
        [DisplayName("Created By")]
        public int CreatedBy { get; set; }
        public string AssignedUser { get; set; }
    }
    public class SupportTicketsWithPagination
    {
        public List<GetSupportTickets> supportTickets { get; set; }
        public int totalSupportTickets { get; set; }
    }
}
