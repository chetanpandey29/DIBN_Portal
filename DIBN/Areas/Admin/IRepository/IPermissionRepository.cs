using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using static DIBN.Areas.Admin.Models.PermissionViewModel;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IPermissionRepository
    {
        List<PermissionViewModel> GetPermissions();
        int SaveRolePermission(SaveRolePermission saveRolePermission);
        int SaveUserPermission(SaveUserPermission saveUserPermission);
        int SaveCompanyPermission(SaveCompanyPermission saveCompanyPermission);
        List<GetRolePermissionByRoleId> GetRolePermissionByRoleId(int RoleId);
        List<GetUserPermissionByUserId> GetUserPermissionByUserId(int UserId);
        List<GetCompanyPermissionByCompanyId> GetCompanyPermissionByCompanyId(int CompanyId);
        int RemoveRolePermission(SaveRolePermission permission);
        int RemoveUserPermission(SaveUserPermission permission);
        int RemoveCompanyPermission(SaveCompanyPermission permission);
        int GetUserIdForPermission(string UserNumber);
        List<string> GetRolePermissionModuleByRoleId(string Role);
        List<string> GetRolePermissionName(string Role, string Module,int? UserId,int? rolePermission);

        List<string> GetUserPermissionModuleByUserId(int UserId);

        List<string> GetUserPermissionName(int UserId, string Module);
        List<string> GetCompanyPermissionModuleByCompanyId(int CompanyId);
        List<string> GetCompanyPermissionName(int CompanyId, string Module);
        List<string> CheckUserAndCompanyPermissionAllowedOrNot(string Name);
        List<string> CheckCompanyPermissionAllowedOrNot(int CompanyId);
        List<string> CheckUserPermissionAllowedOrNot(int UserId);
        int GetUserRole(int UserId);
        int CheckRolePermission(SaveRolePermission permission);
        int GetUserPermissionCount(int UserId);
        int DeleteRolePermission(int roleId);
        string GetUserRoleName(int UserId);
        int DeleteUserPermission(int userId);
        List<string> GetUserPermissionedModuleName(int UserId);
        List<string> GetRolePermissionedModuleName(string Role);
        List<string> GetCurrentRolePermissionName(string Role, string Module);
    }
}
