using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static DIBN.Areas.Admin.Models.PermissionViewModel;
using static DIBN.Areas.Admin.Models.RoleViewModel;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class PermissionController : Controller
    {
        public readonly IPermissionRepository _permissionRepository;
        public readonly IModuleRepository _moduleRepository;
        public readonly IRoleRepository _roleRepository;
        public PermissionController(IPermissionRepository permissionRepository,IModuleRepository moduleRepository,IRoleRepository roleRepository)
        {
            _permissionRepository = permissionRepository;
            _moduleRepository = moduleRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            List<PermissionViewModel> permissions = new List<PermissionViewModel>();
            permissions = _permissionRepository.GetPermissions();
            List<ModuleViewModel> modules = new List<ModuleViewModel>();
            modules = _moduleRepository.GetModules();
            return View();
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult RolePermission(int? Id,string name)
        {
            try
            {
                List<string> allowedModule = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "RolePermission");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "RolePermission");
                    }
                }

                List<SelectListItem> roleList = new List<SelectListItem>();
                var roles = _roleRepository.GetActiveRoles();

                var currentRole = roles.Find(x => x.RoleID == Id);

                for (int i = 0; i < roles.Count; i++)
                {
                    roleList.Add(new SelectListItem
                    {
                        Text = roles[i].RoleName,
                        Value = roles[i].RoleID.ToString()
                    });
                }
                var permissions = _permissionRepository.GetPermissions();
                var modules = _moduleRepository.GetModules();

                RoleViewModel roleDetail = new RoleViewModel();
                if (Id != null)
                    roleDetail = _roleRepository.GetRoleDetail(Convert.ToInt32(Id));

                RolePermissionList getRoles = new RolePermissionList();
                getRoles.Roles = roleList;
                getRoles.Module = name;
                if (Id != 0 && Id != null)
                {
                    // Remove All Modules from Module list to give User Permission which are only accessable from Admin Panel.
                    var getPermissionById = _permissionRepository.GetRolePermissionByRoleId((int)Id);

                    // Remove All Modules from Module list to give User Permission which are only accessable from Admin Panel.
                    if (currentRole.RoleName != "DIBN")
                    {
                        var module = modules.Find(x => x.ModuleName == "RoleManagement");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "RolePermission");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "CompanyPermission");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "UserPermission");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "Banner");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "MessageTemplate");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "Logs");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "ServiceRequestStatus");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "CompanyDocumentType");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "SalesPerson");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "GeneratePI");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "OtherCompanyEmployee");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "DIBNEmployee");
                        modules.Remove(module);

                        module = modules.Find(x => x.ModuleName == "OtherCompanyOwner");
                        modules.Remove(module);
                    }
                    else if (currentRole.RoleName == "DIBN")
                    {
                        var module = modules.Find(x => x.ModuleName == "Employees");
                        modules.Remove(module);
                    }
                    getRoles.permissionCount = permissions.Count;
                    getRoles.Permissions = permissions;
                    getRoles.Modules = modules;
                    getRoles.RoleId = (int)Id;
                    getRoles.Role = roleDetail.RoleName;
                    getRoles.getRolePermissionByRoleIds = getPermissionById;
                    getRoles.Id = (int)Id;
                }
                getRoles.allowedModule = allowedModule;
                return View(getRoles);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SaveRolePermission(int[] InsertPermission,
            int[] UpdatePermission,
            int[] ViewPermission,
            int[] DeletePermission,
            int RoleId)
        {
            try
            {
                List<string> allowedModule = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "RolePermission");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "RolePermission");
                    }
                }

                int returnId = 0;
                if (allowedModule.Contains("Update") && allowedModule.Contains("Insert") && allowedModule.Contains("Delete"))
                {
                    string username = GetUserClaims();
                    int _userId = _permissionRepository.GetUserIdForPermission(username);
                    RoleViewModel model = _roleRepository.GetRoleDetail(RoleId);
                    returnId = _permissionRepository.DeleteRolePermission(RoleId);
                    // Store Insert Values for Selected Module
                    Log.Information("Role Permission change for " + model.RoleName);
                    for (int i = 0; i < InsertPermission.Length; i++)
                    {
                        SaveRolePermission permissions = new SaveRolePermission();
                        permissions.RoleID = RoleId;
                        permissions.ModuleId = InsertPermission[i];
                        permissions.PermissionId = (int)Permission.Insert;
                        permissions.CreatedBy = _userId;
                        returnId = _permissionRepository.SaveRolePermission(permissions);
                    }
                    for (int i = 0; i < UpdatePermission.Length; i++)
                    {
                        SaveRolePermission permissions = new SaveRolePermission();
                        permissions.RoleID = RoleId;
                        permissions.ModuleId = UpdatePermission[i];
                        permissions.PermissionId = (int)Permission.Update;
                        permissions.CreatedBy = _userId;
                        returnId = _permissionRepository.SaveRolePermission(permissions);
                    }
                    for (int i = 0; i < ViewPermission.Length; i++)
                    {
                        SaveRolePermission permissions = new SaveRolePermission();
                        permissions.RoleID = RoleId;
                        permissions.ModuleId = ViewPermission[i];
                        permissions.PermissionId = (int)Permission.Show;
                        permissions.CreatedBy = _userId;
                        returnId = _permissionRepository.SaveRolePermission(permissions);
                    }
                    for (int i = 0; i < DeletePermission.Length; i++)
                    {
                        SaveRolePermission permissions = new SaveRolePermission();
                        permissions.RoleID = RoleId;
                        permissions.ModuleId = DeletePermission[i];
                        permissions.PermissionId = (int)Permission.Delete;
                        permissions.CreatedBy = _userId;
                        returnId = _permissionRepository.SaveRolePermission(permissions);
                    }
                }
                return new JsonResult(returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult RemoveRolePermission(int RoleId, int ModuleId, int PermissionId)
        {
            try
            {
                int returnId = 0;
                List<string> allowedModule = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "RolePermission");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "RolePermission");
                    }
                }
                if (allowedModule.Contains("Update") && allowedModule.Contains("Insert") && allowedModule.Contains("Delete"))
                {
                    string username = GetUserClaims();
                    int _userId = _permissionRepository.GetUserIdForPermission(username);
                    SaveRolePermission permission = new SaveRolePermission();
                    permission.RoleID = RoleId;
                    permission.ModuleId = ModuleId;
                    permission.PermissionId = PermissionId;
                    permission.CreatedBy = _userId;
                    returnId = _permissionRepository.CheckRolePermission(permission);
                    if (returnId > 0)
                    {
                        returnId = _permissionRepository.RemoveRolePermission(permission);
                    }
                }
                return new JsonResult(returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRolePermissionModule()
        {
            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetRolePermissionModuleByRoleId(Role);
            return new JsonResult(Modules);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserPermissionModule()
        {
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetUserPermissionModuleByUserId(UserId);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyPermissionModule()
        {
            string Actor = GetActorClaims();
            int CompanyId = Convert.ToInt32(Actor);
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetCompanyPermissionModuleByCompanyId(CompanyId);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRolePermissionsName(string Module,int rolePermission)
        {
            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetRolePermissionName(Role,Module,UserId, rolePermission);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckUserPermissionAllowedOrNot()
        {
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.CheckUserPermissionAllowedOrNot(UserId);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckCompanyPermissionAllowedOrNot()
        {
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.CheckCompanyPermissionAllowedOrNot(CompanyId);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckUserAndCompanyPermissionAllowedOrNot()
        {
            string Role = GetClaims();
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.CheckUserAndCompanyPermissionAllowedOrNot(Role);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserPermissionsName(string Module)
        {
            string UserName = GetUserClaims();

            int UserId = _permissionRepository.GetUserIdForPermission(UserName);

            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetUserPermissionName(UserId, Module);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyPermissionsName(string Module)
        {
            string Company = GetActorClaims();

            int CompanyId = Convert.ToInt32(Company);
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetCompanyPermissionName(CompanyId, Module);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public string GetClaims()
        {
            string Role = "";
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            for(int i = 0; i < roles.Count; i++)
            {
                if (roles[i].Value == "DIBN")
                {
                    Role = roles[i].Value;
                }
            }
             
            return Role;
        }
        [HttpGet]
        [Route("[action]")]
        public string GetUserClaims()
        {
            string UserDetails = "";
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var userClaimType = userIdentity.NameClaimType;
            var users = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Value.Contains("_DIBN"))
                {
                    var user = users[i].Value.Split("_DIBN");
                    UserDetails = user[0];
                }
                else
                {
                    if(users[i].Value!="")
                        UserDetails = users[i].Value;
                }
            }
            return UserDetails;
        }
        [HttpGet]
        [Route("[action]")]
        public string GetActorClaims()
        {
            string Company = "";
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var actorClaimType = userIdentity.Actor;
            var actor = claims.Where(c => c.Type == ClaimTypes.Actor).ToList();
            for (int i = 0; i < actor.Count; i++)
            {
                if (actor[i].Value.Contains("_DIBN"))
                {
                    var user = actor[i].Value.Split("_DIBN");
                    Company = user[0];
                }
                else
                {
                    if (actor[i].Value != "")
                        Company = actor[i].Value;
                }
            }
            return Company;
        }
    }
}
