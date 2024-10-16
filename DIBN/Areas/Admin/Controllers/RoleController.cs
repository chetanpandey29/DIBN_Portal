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
    #nullable enable
    public class RoleController : Controller
    {
        public readonly IRoleRepository _roleRepository;
        public readonly IModuleRepository _moduleRepository;
        public readonly IPermissionRepository _permissionRepository;
        public RoleController(IRoleRepository roleRepository, IModuleRepository moduleRepository, IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _moduleRepository = moduleRepository;
            _permissionRepository = permissionRepository;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            MainRoleViewModel mainModel = new MainRoleViewModel();
            List<RoleViewModel> roles = new List<RoleViewModel>();
            roles = _roleRepository.GetRoles();
            mainModel.roles = roles;
            mainModel.Module = name;
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "RoleManagement");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "RoleManagement");
                }
            }
            mainModel.allowedModule = allowedModule;
            return View(mainModel);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Create(string? name)
        {
            RoleViewModel role = new RoleViewModel();
            role.Module = name;
            return PartialView("_Create", role);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Creates(RoleViewModel role)
        {
            int returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            role.CreatedBy = userId;
            returnId = _roleRepository.CreateNewRole(role);
            Log.Information("New Role Created" + role.RoleName);
            return RedirectToAction("Index", "Role", new { name = role.Module });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(int Id, string? name)
        {
            RoleViewModel role = new RoleViewModel();
            role = _roleRepository.GetRoleDetail(Id);
            role.Module = name;
            return PartialView("_Edit", role);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(RoleViewModel role)
        {
            int returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            role.CreatedBy = userId;
            returnId = _roleRepository.UpdateRoleDetail(role);
            Log.Information("Changed details of Role");
            Log.Information("Role : " + role.RoleName + ", IsActive : " + role.IsActive);
            return RedirectToAction("Index", "Role", new { name = role.Module });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id)
        {
            int returnId = 0;
            RoleViewModel role = new RoleViewModel();
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            role = _roleRepository.GetRoleDetail(Id);
            Log.Information("Delete Role " + role.RoleName);
            returnId = _roleRepository.DeleteRole(Id, userId);
            return new JsonResult(returnId);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckExistanceOfRole(string Name,int roleId)
        {
            int returnId = 0;
            returnId = _roleRepository.CheckExistanceOfRole(Name,roleId);
            return new JsonResult(returnId);
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
            for (int i = 0; i < roles.Count; i++)
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
                    if (users[i].Value != "")
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
