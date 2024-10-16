using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface ICompanyServiceRepository
    {
        List<CompanyServiceViewModel> GetAllCompanyServices();
        CompanyServiceViewModel GetCompanyServicesById(int ID);
        int CreateNew(CompanyServiceViewModel service);
        int UpdateCompanyServices(CompanyServiceViewModel service);
        int DeleteCompanyServices(int ID, int UserId);
        string GetSerialNumber();
        List<CompanyServiceViewModel> GetAllParentCompanyServices();
    }
}
