using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class ServiceRequestStatusController : Controller
    {
        private readonly IServiceRequestStatusRepository _serviceRequestStatusRepository;
        private readonly IPermissionRepository _permissionRepository;

        public ServiceRequestStatusController(IServiceRequestStatusRepository serviceRequestStatusRepository, IPermissionRepository permissionRepository)
        {
            _serviceRequestStatusRepository = serviceRequestStatusRepository;
            _permissionRepository = permissionRepository;   
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "ServiceRequestStatus");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "ServiceRequestStatus");
                }
            }

            MainServiceRequestStatusModel mainModel = new MainServiceRequestStatusModel();
            List<ServiceRequestStatusModel> model = new List<ServiceRequestStatusModel>();
            model = _serviceRequestStatusRepository.GetServiceRequestStatus();
            mainModel.allowedModule = allowedModule;
            mainModel.Module = name;    
            mainModel.serviceRequestStatus = model;
            return View(mainModel);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Create(string? name)
        {
            ServiceRequestStatusModel model = new ServiceRequestStatusModel();
            model.Module = name;
            model.DisplayId = _serviceRequestStatusRepository.GetlastDisplayId();
            return PartialView("_Create", model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Creates(ServiceRequestStatusModel model)
        {
            int returnId = 0;
            string _username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(_username);
            model.UserId = userId;
            returnId = _serviceRequestStatusRepository.SaveServiceRequestStatus(model);
            return RedirectToAction("Index", "ServiceRequestStatus", new { name = model.Module });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(int Id, string? name)
        {
            ServiceRequestStatusModel model = new ServiceRequestStatusModel();
            model = _serviceRequestStatusRepository.GetServiceRequestStatusById(Id);
            model.Module = name;
            return PartialView("_Edit", model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(ServiceRequestStatusModel model)
        {
            int returnId = 0;
            string _username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(_username);
            model.UserId = userId;
            returnId = _serviceRequestStatusRepository.UpdateServiceRequestStatus(model);
            return RedirectToAction("Index", "ServiceRequestStatus", new { name = model.Module });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id)
        {
            int returnId = 0;
            string _username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(_username);
            returnId = _serviceRequestStatusRepository.DeleteServiceRequestStatus(Id, userId);
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
