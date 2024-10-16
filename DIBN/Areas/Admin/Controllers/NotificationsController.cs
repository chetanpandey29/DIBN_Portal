using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class NotificationsController : Controller
    {
        private readonly IImportReminderNotificationRepository _importReminderNotificationRepository;
        private readonly IPermissionRepository _permissionRepository;
        public NotificationsController(IImportReminderNotificationRepository importReminderNotificationRepository, IPermissionRepository permissionRepository)
        {
            _importReminderNotificationRepository = importReminderNotificationRepository;  
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index(string? name)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "Notification");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "Notification");
                }
            }
            MainNotificationModel model = new MainNotificationModel();
            List<GetNotificationServiceListModel> services = new List<GetNotificationServiceListModel>();
            services = await _importReminderNotificationRepository.GetNotificationServiceList();
            model.services = services;
            model.Module = name;
            model.allowedModule = allowedModule;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ServiceNotification(string service, string? name)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "Notification");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "Notification");
                }
            }
            MainImportReminderNotification mainModel = new MainImportReminderNotification();
            List<ImportReminderNotification> services = new List<ImportReminderNotification>();
            services = await _importReminderNotificationRepository.GetAllServiceNotifications(service);
            mainModel.notifications = services;
            mainModel.allowedModule = allowedModule;
            mainModel.Module = name;
            return View(mainModel);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult UpdateNotificationDay(int Days,int Id)
        {
            ImportReminderNotification importantNotification = new ImportReminderNotification();
            importantNotification.SendNotificationAfter = Days;
            importantNotification.ID = Id;
            return PartialView("_UpdateNotificationDay", importantNotification);
        }
        [HttpPost]
        public IActionResult UpdateNotificationDays(int Days, int Id)
        {
            int _returnId = 1;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            _returnId = _importReminderNotificationRepository.UpdateReminderDays(Days, Id,userId);
            return RedirectToAction("Index", "Notifications");
        }
        [HttpGet]
        public IActionResult RemoveNotifications(int Id)
        {
            int _returnId = 1;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            _returnId = _importReminderNotificationRepository.RemoveNotification(Id,userId);
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRequestNotifications(string? name,string? StartDate,string? EndDate)
        {
            MainRequestNotificationModel mainModel = new MainRequestNotificationModel();
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "Notification");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "Notification");
                }
            }
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            List<GetRequestNotificationModel> listData = new List<GetRequestNotificationModel>();
            listData = _importReminderNotificationRepository.GetRequestNotifications(StartDate,EndDate,userId);
            mainModel.notifications = listData;
            mainModel.Module = name;
            mainModel.allowedModule = allowedModule;
            return View(mainModel);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult ChangeRequestStatus(int Status,int Id)
        {
            int _returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            _returnId = _importReminderNotificationRepository.ChangeRequestNotificationStatus(Status,Id, userId);
            return new JsonResult(_returnId);
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
