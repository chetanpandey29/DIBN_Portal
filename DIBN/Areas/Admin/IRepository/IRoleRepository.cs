using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IRoleRepository
    {
        int CreateNewRole(RoleViewModel model);
        List<RoleViewModel> GetRoles();
        RoleViewModel GetRoleDetail(int RoleId);
        int UpdateRoleDetail(RoleViewModel model);
        int DeleteRole(int RoleId, int UserId);
        List<RoleViewModel> GetActiveRoles();
        int CheckExistanceOfRole(string Name,int roleId);
        int GetCompanyOwnerId();
    }
}
