using DIBN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.IService
{
    public interface IServicesFormService
    {
        FormDetail GetServiceFormDetails(int ServiceId, bool IsCompanyService, bool IsEmployeeService);
        List<GetServiceFormFieldModel> GetServiceFormFields(int ServiceId, int FormId);
        string SaveServiceFormData(GetFieldFormData formData);
        string GetSerialNumber();
        List<GetRequests> GetAllRequests(int CompanyId, int UserId, string Role, string Company, int? Status);
        int GetCountOfResponse(string SerialNumber);
        int SaveStatusOfService(GetRequestResponses getRequestResponses);
        GetRequestCompleteDetails GetRequestDetail(string serialNumber);
        int LastStepNumber(string SerialNumber);
        int GetLatestStatusOfRequest(string SerialNumber);
        string GetLastSerialNumber();
        int ChangeStatusOfService(string SerialNumber, int Status, int UserId);
        List<ServiceRequestStatusModel> GetServiceRequestStatus();
        Task<GetCompanyServiceRequestsByCompanyId> GetAllCompanyRequestByCompanyId(int CompanyId,int currentCompanyId, int userId, string Role, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchString,int count);
        Task<GetEmployeeServiceRequestsByCompanyId> GetAllEmployeeRequestByCompanyId(int CompanyId, int currentCompanyId, int userId, string Role, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchString, int count);
        List<DownloadServiceRequestUploadedDocumentCmp> DownloadUploadedServiceRequestDocument(string serialNumber, int companyId, string FileName, int ServiceId);
        Task<GetAssignedCompanyServiceRequests> GetAllAssignedCompanyServiceRequest(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchString, int count);
        Task<GetAssignedEmployeeServiceRequests> GetAllAssignedEmployeeServiceRequest(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchString, int count);
        Task<GetAssignedCompanyServiceRequests> GetAllAssignedCompanyServiceRequestTempData(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString);
        Task<GetAssignedEmployeeServiceRequests> GetAllAssignedEmployeeServiceRequestTempData(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString);
        Task<GetCompanyServiceRequestsByCompanyId> GetAllCompanyRequestByCompanyIdTemp(int CompanyId, int currentCompanyId, int userId, string Role, int? status, int page, int pageSize, string sortBy,
            string sortDirection, string searchString);
        Task<GetEmployeeServiceRequestsByCompanyId> GetAllEmployeeRequestByCompanyIdTemp(int CompanyId, int currentCompanyId, int userId, string Role, int? status, int page, int pageSize, string sortBy, string sortDirection,
            string searchString);
        //Task<GetAssignedCompanyServiceRequests> GetAllAssignedCompanyServiceRequestFilter(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString);
        //Task<GetAssignedEmployeeServiceRequests> GetAllAssignedEmployeeServiceRequestFilter(int userId, int? status, int page, int pageSize, string sortBy, string sortDirection, string searchBy, string searchString);
        //Task<GetCompanyServiceRequestsByCompanyId> GetAllCompanyRequestByCompanyIdFilter(int CompanyId, int currentCompanyId, int userId, string Role, int? status, int page, int pageSize, string sortBy,
        //    string sortDirection, string searchBy, string searchString);
        //Task<GetEmployeeServiceRequestsByCompanyId> GetAllEmployeeRequestByCompanyIdFilter(int CompanyId, int currentCompanyId, int userId, string Role, int? status, int page, int pageSize,
        //    string sortBy, string sortDirection, string searchBy, string searchString);
    }
}
