using DIBN.Models;
using System.Collections.Generic;

namespace DIBN.IService
{
    public interface IUserPermissionService
    {
        List<string> GetRolePermissionModuleByRoleId(string Role);
        List<string> GetRolePermissionName(string Role, string Module);
        List<string> GetUserPermissionModuleByUserId(int UserId);

        List<string> GetUserPermissionName(int UserId, string Module);
        List<string> GetCompanyPermissionModuleByCompanyId(int CompanyId);
        List<string> GetCompanyPermissionName(int CompanyId, string Module);
        int GetUserIdForPermission(string UserNumber);
        List<string> CheckUserAndCompanyPermissionAllowedOrNot(string Name);
        List<string> CheckCompanyPermissionAllowedOrNot(int CompanyId);
        List<string> CheckUserPermissionAllowedOrNot(int UserId);
        List<string> GetAllAssignedCompanies(int UserId, int CompanyId);
        int GetEmployeesCount(int CompanyId, string Role);
        string GetSalesPersonName(string UserNumber);
        List<SalesPersonCompany> GetAllAssignedCompaniesSalesPerson(int UserId);
        string GetUserRoleName(int UserId);
        int GetCompanyUsersCount(int companyId);
        int GetCompanyIdForPermission(string UserNumber);
        int GetSalesPersonIdForPermission(string UserNumber);
        string GetRMTeamName(string UserNumber);
        int GetRMTeamIdForPermission(string UserNumber);
        List<string> GetAllAssignedCompaniesToRMTeam(int UserId, int CompanyId);
        List<string> GetUserPermissionedModuleName(int UserId);
        List<string> GetRolePermissionedModuleName(string Role);
        List<string> GetCurrentRolePermissionName(string Role, string Module);
    }
}
