using System.Collections.Generic;

namespace DIBN.Models
{
    public class GetAssignedSupportTicketsByUserWithPaginationModel
    {
        public GetAssignedSupportTicketsByUserWithPaginationModel()
        {
            supportTickets = new List<GetAssignedSupportTicketListByUserModel>();
        }
        public List<GetAssignedSupportTicketListByUserModel> supportTickets { get; set; }
        public int totalSupportTickets { get; set; }
    }

    public class GetAssignedSupportTicketListByUserModel
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

    public class GetSupportTicketsBySalesPersonWithPaginationModel
    {
        public GetSupportTicketsBySalesPersonWithPaginationModel()
        {
            supportTickets = new List<GetSupportTicketListBySalesPersonModel>();
        }
        public List<GetSupportTicketListBySalesPersonModel> supportTickets { get; set; }
        public int totalSupportTickets { get; set; }
    }

    public class GetSupportTicketListBySalesPersonModel
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

    public class GetSupportTicketsByCompanyIdWithPaginationModel
    {
        public GetSupportTicketsByCompanyIdWithPaginationModel()
        {
            supportTickets = new List<GetSupportTicketListByCompanyIdModel>();
        }
        public List<GetSupportTicketListByCompanyIdModel> supportTickets { get; set; }
        public int totalSupportTickets { get; set; }
    }

    public class GetSupportTicketListByCompanyIdModel
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
