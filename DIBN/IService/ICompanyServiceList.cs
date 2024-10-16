using DIBN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.IService
{
    public interface ICompanyServiceList
    {
        List<CompanyServices> GetAllCompanyService(int CompanyId);
        List<CompanyServices> GetAllChildCompanyService(int CompanyId, int ParentId);
        string ChangestatusOfCompany(int ActiveId);
        Task<string> sendConfirmationMail(string Email);
        List<GetCompanyServiceRequest> GetCompanyServiceRequest(int CompanyId);
        List<GetCompanyServiceRequestDetails> GetCompanyServiceRequestDetails(int CompanyId, string SerialNumber);
        List<string> GetServiceListBySearchName(string prefix);
        List<string> GetServiceIdByName(string Service);
        int GetServiceId(string ServiceName, string companyType);
        
    }
}
