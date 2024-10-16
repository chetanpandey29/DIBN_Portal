using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface ISupportTicketRepository
    {
        List<SupportTicketRequest> GetAllSupportTickets(int CompanyId, int? Status);
        List<SupportTicketRequest> GetSupportTicketDetail(string TrackingNumber);
        List<GetSupportTicketResponseByParentId> GetTicketReplyOfAnyResponse(int ParentId);
        int AddNewSupportTicket(SupportTicketRequest supportTicket);
        int UploadTicketDocuments(IFormFile formFile, string Id, int _returnId, int UserId);
        List<SupportTicketDocunents> GetUploadedDocumetsById(int TicketId);
        string GetTrackingNumber();
        int SaveAssignUserOfSupportTicket(SaveAssignUserOfSupportTikcet saveAssignUser);
        List<int> GetAssignedUserOfSupportTicket(string TrackingNumber);
        int RemoveSupportTicketRequest(string TrackingNumber, int UserId);
        SupportTicketDocunents GetUploadedDocumets(int Id);
        List<int> GetAllAssignedUsers(string TrackingNumber);
        SupportTicketsWithPagination GetSupportTicketWithPagination(int? Status, int pageNumber, int rowsofPage, string searchValue, string sortBy, string sortDire);
        Task<int> GetLastStatusOfSupportTicket(string trackingNumber);
        Task<GetSupportsTicketsWithPaginationForAdminModel> GetSupportsTicketsWithPaginationForAdmin(int? Status, int skipRows, int rowsOfPage, string? searchPrefix,
            string? sortColumn, string? sortDirection);
    }
}
