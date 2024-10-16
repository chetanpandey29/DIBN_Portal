using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class GetSupportsTicketsWithPaginationForAdminModel
    {
        public GetSupportsTicketsWithPaginationForAdminModel()
        {
            supportTickets = new List<SupportTicketListModel>();
        }
        public List<SupportTicketListModel> supportTickets { get; set; }
        public int totalSupportTickets { get; set; }
    }

    public class SupportTicketListModel
    {
        public string TrackingNumber { get; set; }
        public string CompanyName { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string CreatedOn { get; set; }
        public int TicketStatusId { get; set; }
        public string TicketStatus { get; set; }
        public string AssignedUser { get; set; }
        public string AssignedOn { get; set; }
    }
}
