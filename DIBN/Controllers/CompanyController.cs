using DIBN.IService;
using DIBN.Models;
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
    public class CompanyController : Controller
    {

        private readonly IUserCompanyService _userCompanyService;
        private readonly IUserPermissionService _userPermissionService;
        public CompanyController(IUserCompanyService userCompanyService, IUserPermissionService userPermissionService)
        {
            _userCompanyService = userCompanyService;
            _userPermissionService = userPermissionService; 
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(User);
            allowedModule = _userPermissionService.GetUserPermissionName(UserId, "Company");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _userPermissionService.GetCurrentRolePermissionName(Role, "Company");
                }
            }

            if(allowedPermission.Count == 0)
            {
                allowedPermission = _userPermissionService.GetRolePermissionedModuleName(Role);
            }

            CompanyViewModel companyViewModel = new CompanyViewModel();
            string role = GetClaims();
            if (role == null || role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string UserName = GetUserClaims();
            string _companyId = GetActorClaims();
            
            if (role.StartsWith("Sales"))
            {
                List<UserCompanyViewModel> userCompanies = new List<UserCompanyViewModel>();
                string SalesPerson = GetUserClaims();
                int SalesPersonId = _userPermissionService.GetSalesPersonIdForPermission(SalesPerson);
                userCompanies = _userCompanyService.GetComapnyBySalesPersonId(SalesPersonId);
                companyViewModel.userCompanies = userCompanies;
            }
            else if(role == "RM Team")
            {
                List<UserCompanyViewModel> userCompanies = new List<UserCompanyViewModel>();
                string RMTeam = GetUserClaims();
                int RMTeamId = _userPermissionService.GetRMTeamIdForPermission(RMTeam);
                userCompanies = _userCompanyService.GetCompanyByRMTeamId(RMTeamId);
                companyViewModel.userCompanies = userCompanies;
            }
            else
            {
                UserCompanyViewModel company = new UserCompanyViewModel();
                string strcompany = GetActorClaims();
                int companyId = Convert.ToInt32(strcompany);
                company = _userCompanyService.GetCompanyById(companyId);
                companyViewModel.userCompanyViewModel = company;
            }
            companyViewModel.Module = name;
            companyViewModel.allowedModule = allowedModule;
            companyViewModel.allowedPermissions = allowedPermission;
            return View(companyViewModel);
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
        [HttpGet]
        [Route("[action]")]
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
    }
}
