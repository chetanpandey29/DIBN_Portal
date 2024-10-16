using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IEmployeeServiceRepository
    {
        List<EmployeeServicesModel> GetAllEmployeeService();
        EmployeeServicesModel GetEmployeeServiceById(int ID);
        int CreateNew(EmployeeServicesModel service);
        int UpdateEmployeeService(EmployeeServicesModel service);
        int DeleteEmployeeService(int ID, int UserId);
        string GetSerialNumber();
        int GetCloseServiceCount();
        int GetOpenServiceCount();
        int GetRejectedServiceCount();
        int GetOpenSupportTicketCount();
        int GetCloseSupportTicketCount();
        List<EmployeeServicesModel> GetAllParentEmployeeService();
        List<GetEmployeeDetailsforChart> GetEmployeeDetailsForChartsMonthly();
        List<GetEmployeeDetailsforChart> GetEmployeeDetailsForChartsYearly();
        GetServicesCountForChart GetServicesCountYearly();
        GetServicesCountForChart GetServicesCountMonthly();
    }
}
