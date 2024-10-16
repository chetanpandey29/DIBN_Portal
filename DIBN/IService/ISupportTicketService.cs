using DIBN.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.IService
{
    public interface ISupportTicketService
    {
        List<SupportTicketViewModel> GetAllSupportTickets(int CompanyId, int _UserId, string role, int? Status);
        List<SupportTicketViewModel> GetSupportTicketDetail(string TrackingNumber);
        List<GetSupportTicketResponseByParentId> GetTicketReplyOfAnyResponse(int CompanyId, int ParentId);
        int AddNewSupportTicket(SupportTicketViewModel supportTicket);
        int UploadTicketDocuments(IFormFile formFile, string Id, int ReplyId, int UserId);
        List<SupportTicketDocunents> GetUploadedDocumetsById(int TicketId);
        string GetTrackingNumber();
        SupportTicketDocunents GetUploadedDocumets(int Id);
        GetAllAssignedSupportTicketWithPagination GetAllAssignedSupportTicket(int _UserId, int? Status, int pageNumber, int rowOfpage, string searchValue, string sortBy, string sortingDir);
        GetAllAssignedSupportTicketWithPagination GetCompanyWiseSupportTicket(int CompanyId, int? Status, int pageNumber, int rowOfpage, string searchValue, string sortBy, string sortingDir);
        GetAllAssignedSupportTicketWithPagination GetCompanySupportTicketBySalesPerson(int SalesPerson, int? Status, int pageNumber, int rowOfpage, string searchValue, string sortBy, string sortingDir);
        Task<int> GetLastStatusOfSupportTicket(string trackingNumber);
        Task<GetAssignedSupportTicketsByUserWithPaginationModel> GetAssignedSupportTicketsByUserWithPagination(
            int userId, int? Status, int skipRows, int rowsOfPage, string? searchPrefix, string? sortColumn, string? sortDirection
        );
        Task<GetSupportTicketsBySalesPersonWithPaginationModel> GetSupportTicketsBySalesPersonWithPagination(
            int salesPersonId, int? Status, int skipRows, int rowsOfPage, string? searchPrefix, string? sortColumn, string? sortDirection
        );
        Task<GetSupportTicketsByCompanyIdWithPaginationModel> GetSupportTicketsByCompanyIdWithPagination(
            int companyId, int? Status, int skipRows, int rowsOfPage, string? searchPrefix, string? sortColumn, string? sortDirection
        );
    }
}
