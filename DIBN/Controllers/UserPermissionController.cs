using DIBN.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserPermissionController : Controller
    {
        private readonly IUserPermissionService _userPermissionService;
        public UserPermissionController(IUserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRolePermissionsName(string Module)
        {
            string Role = GetClaims();

            List<string> Modules = new List<string>();
            Modules = _userPermissionService.GetRolePermissionName(Role, Module);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserPermissionsName(string Module)
        {
            string UserName = GetUserClaims();

            int UserId = _userPermissionService.GetUserIdForPermission(UserName);

            List<string> Modules = new List<string>();
            Modules = _userPermissionService.GetUserPermissionName(UserId, Module);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyPermissionsName(string Module)
        {
            string Company = GetActorClaims();

            int CompanyId = Convert.ToInt32(Company);
            List<string> Modules = new List<string>();
            Modules = _userPermissionService.GetCompanyPermissionName(CompanyId, Module);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRolePermissionModule()
        {
            string Role = GetClaims();

            List<string> Modules = new List<string>();
            Modules = _userPermissionService.GetRolePermissionModuleByRoleId(Role);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyPermissionModule()
        {
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            List<string> Modules = new List<string>();
            Modules = _userPermissionService.GetCompanyPermissionModuleByCompanyId(CompanyId);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserPermissionModule()
        {
            string Usern = GetUserClaims();
            string role = GetClaims();
            List<string> Modules = new List<string>();
            if (role!="Sales Person" && role != "RM Team")
            {
                int UserId = _userPermissionService.GetUserIdForPermission(Usern);
                
                Modules = _userPermissionService.GetUserPermissionModuleByUserId(UserId);
            }
            
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckUserPermissionAllowedOrNot()
        {
            string User = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(User);
            List<string> Modules = new List<string>();
            Modules = _userPermissionService.CheckUserPermissionAllowedOrNot(UserId);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckCompanyPermissionAllowedOrNot()
        {
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            List<string> Modules = new List<string>();
            Modules = _userPermissionService.CheckCompanyPermissionAllowedOrNot(CompanyId);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckUserAndCompanyPermissionAllowedOrNot()
        {
            string Role = GetClaims();
            List<string> Modules = new List<string>();
            Modules = _userPermissionService.CheckUserAndCompanyPermissionAllowedOrNot(Role);
            return new JsonResult(Modules);
        }
        public string GetClaims()
        {
            string Role = "";
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            var currentRole = roles.Where(x => !x.Value.Contains("DIBN")).ToList();
            var companyId = GetActorClaims();
            if (currentRole.Count > 0)
            {
                if (!currentRole[0].Value.StartsWith("Sales") && !currentRole[0].Value.StartsWith("RM"))
                {
                    int companyOwnerCount = 0;
                    var user = GetUserClaims();
                    int userId = _userPermissionService.GetUserIdForPermission(user);
                    int _companyId = _userPermissionService.GetCompanyIdForPermission(user);
                    string role = _userPermissionService.GetUserRoleName(userId);
                    if (companyId != null)
                    {
                        companyOwnerCount = _userPermissionService.GetCompanyUsersCount(Convert.ToInt32(companyId));
                    }
                    if (role == currentRole[0].Value)
                    {
                        Role = currentRole[0].Value;
                    }
                    else if (companyOwnerCount == 0 && _companyId != 0) //companyOwnerCount == 0 && companyId != null
                    {
                        Role = currentRole[0].Value;
                    }
                    else if (_companyId != 0 && userId == 0)
                    {
                        Role = currentRole[0].Value;
                    }
                }
                else
                {
                    Role = currentRole[0].Value;
                }
            }
            return Role;
        }
        public string GetUserClaims()
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var userClaimType = userIdentity.NameClaimType;
            var users = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
            var currentUser = users.Where(x => !x.Value.Contains("_DIBN")).ToList();
            string UserDetails = currentUser[0].Value;
            return UserDetails;
        }
        [HttpGet]
        [Route("[action]")]
        public string GetActorClaims()
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var actorClaimType = userIdentity.Actor;
            var actor = claims.Where(c => c.Type == ClaimTypes.Actor).ToList();
            var currentActor = actor.Where(x => !x.Value.Contains("_DIBN")).ToList();
            string Company = currentActor[0].Value;
            return Company;
        }
    }
}
