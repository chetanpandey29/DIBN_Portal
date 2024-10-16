using DIBN.Models;
using System.Collections.Generic;

namespace DIBN.IService
{
    public interface IEmployeeServiceList
    {
        List<EmployeeServices> GetAllEmployeeService(int CompanyId);
        List<EmployeeServices> GetAllChildEmployeeService(int CompanyId, int ParentId);
        List<EmployeeServiceRequest> GetEmployeeServiceRequests(int CompanyId);
        int AddEmployeeServiceRequest(EmployeeServiceRequest request);
        string GetRequestNumber();
        List<EmployeeServiceRequestDocument> GetEmployeeServiceRequestDocuments(int RequestId);
        EmployeeServiceRequestDocument GetEmployeeServiceRequestDocumentById(int RequestId);
        List<EmployeeServiceRequest> GetEmployeeServiceRequestsDetails(string RequestId);
        List<GetResponseByParentId> GetEmployeeResponseReply(int ParentId);
        int GetCloseServiceCount(int CompanyId,int SalesPerson, int RMTeamId);
        int GetOpenServiceCount(int CompanyId,int SalesPerson,int RMTeamId);
        int GetRejectedServiceCount(int CompanyId, int SalesPerson, int RMTeamId);
        int GetOpenSupportTicketCount(int CompanyId);
        int GetCloseSupportTicketCount(int CompanyId);
        int GetServiceId(string ServiceName, string companyType);
        int GetOpenServiceCountForAssignedUser(int UserId);
        int GetRejectedServiceCountForAssignedUser(int UserId);
        int GetCloseServiceCountForAssignedUser(int UserId);
        int GetCurrentNotificationCount(int UserId);
    }
}
