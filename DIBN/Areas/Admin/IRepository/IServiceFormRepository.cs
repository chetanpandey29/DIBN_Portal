using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IServiceFormRepository
    {
        FormDetail GetServiceFormDetails(int ServiceId, bool IsCompanyService, bool IsEmployeeService);
        int CreateForm(ServiceFormModel model);
        List<GetServiceFormFieldModel> GetServiceFormFields(int ServiceId, int FormId);
        int CreateFormField(SaveFormFieldsModel model);
        List<GetRequestsModel> GetAllRequests(int CompanyId, int? Status);
        GetRequestCompleteDetails GetRequestDetail(string serialNumber);
        int GetCountOfResponse(string SerialNumber);
        int SaveStatusOfService(GetRequestResponsesModel getRequestResponses);
        int ChangeStatusOfService(string SerialNumber, int Status, int UserId);
        int LastStepNumber(string SerialNumber);
        int SaveServiceRequestDocument(IFormFile formFile, string SerialNumber, int ServiceId, int CompanyId, int ServiceResponseId, int UserId);
        int SaveAssignUserOfService(SaveAssignUser saveAssignUser);
        int GetLatestStatusOfRequest(string SerialNumber);
        string GetFormNameById(int FormId);
        int RemoveServiceRequest(string SerialNumber, int ServiceId, int UserId);
        List<int> GetAllAssignedUsers(string SerialNumber);
        int GetlastStepNumber(int ServiceId, int FormId);
        List<GetRequestsModel> GetAllEmployeeRequests(int CompanyId, int? Status);
        List<GetRequestsModel> GetAllCompanyRequests(int CompanyId, int? Status);
		List<DownloadServiceRequestUploadedDocument> DownloadUploadedServiceRequestDocument(string serialNumber, int companyId, string FileName, int ServiceId);
        GetAllCompanyServiceRequestModel GetAllCompanyServiceRequests(int? Status, int page, int pageSize, string sortBy, string sortDirection, string searchString,int count);
        Task<GetAllEmployeeServiceRequestModel> GetAllEmployeeServiceRequests(int? Status, int skip, int pageSize, string sortBy, string sortDirection, string searchString,int count);
        Task<GetCompanyServiceRequestsByCompanyId> GetAllCompanyRequestByCompanyId(int CompanyId, int? status, int page, int pageSize, string sortBy,
            string sortDirection, string searchBy, string searchString);
        Task<GetCompanyServiceRequestsByCompanyId> GetAllEmployeeRequestByCompanyId(int CompanyId, int? status, int page, int pageSize, string sortBy,
            string sortDirection, string searchBy, string searchString);
        GetAllCompanyServiceRequestModel GetAllCompanyServiceRequests_TEST(int? Status, int page, int pageSize, string sortBy, string sortDirection, string searchString, int count);
        Task<GetAllEmployeeServiceRequestModel> GetAllEmployeeServiceRequests_TEST(int? Status, int skip, int pageSize, string sortBy, string sortDirection, string searchString, int count);
        Task<GetAllCompanyServiceRequestModel> GetAllCompanyServiceRequestFilter(int? Status, int skip, int take, string sortBy, string sortDirection, string searchBy, string searchingStr);
        Task<GetAllEmployeeServiceRequestModel> GetAllEmployeeServiceRequestFilter(int? Status, int skip, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString);
        Task<GetAllCompanyServiceRequestModel> GetAllCompanyServiceRequestTempData(int? Status, int skip, int take, string sortBy, string sortDirection, string searchBy, string searchingStr);
        Task<GetAllEmployeeServiceRequestModel> GetAllEmployeeServiceRequestTempData(int? Status, int skip, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString);
    }
}
