using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class MainRequestNotificationModel
    {
        public List<GetRequestNotificationModel> notifications { get; set; }
        public List<string> allowedModule {  get; set; }
        public string Module {  get; set; }
    }
    public class GetRequestNotificationModel
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string EnquiryNumber { get; set; }
        public string RequestCreatedOn { get; set; }
        public string Type { get; set; }
        public int AssignedUserId { get; set; }
        public string AssignedOn { get; set; }
        public int AssignedById { get; set; }
        public string FormName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedUser { get; set; }
        public string AssignedBy { get; set; }
        public string ServiceCreatedBy { get; set; }
        public string ServiceCreatedBySalesPerson { get; set; }
        public string SupportTicketCreatedBySalesPerson { get; set; }
        public int ServiceCreatedById { get; set; }
        public string EnquiryResponseCreatedBy { get; set; }
        public int EnquiryResponseCreatedById { get; set; }
        public int SupportTicketById { get; set; }
        public string TicketCreatedBy { get; set; }
        public string Module { get; set; }
        public string Message { get; set; }
        public string CreatedOn { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string ResponseType { get; set; }
        public bool MarkAsRead { get; set; }
        public string ServiceCreatedByRMTeam {  get; set; }
    }
}
