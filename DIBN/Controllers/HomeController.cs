using DIBN.IService;
using DIBN.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserCompanyService _userCompanyService;
        private readonly IUserPermissionService _userPermissionService;
        private readonly IUserEmployeesService _userEmployeesService;
        private readonly IImportantReminderService _importantReminderService;
        private readonly IEmployeeServiceList _employeeServiceList;
        private readonly IServicesFormService _servicesFormService; 
        private readonly IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger,IUserCompanyService userCompanyService, 
            IUserPermissionService userPermissionService, IUserEmployeesService userEmployeesService,
            IImportantReminderService importantReminderService,
            IEmployeeServiceList employeeServiceList,
            IMemoryCache memoryCache,
            IServicesFormService servicesFormService)
        {
            _logger = logger;
            _userCompanyService = userCompanyService;
            _userPermissionService = userPermissionService;
            _userEmployeesService = userEmployeesService;
            _importantReminderService = importantReminderService;
            _employeeServiceList = employeeServiceList;
            _memoryCache = memoryCache; 
            _servicesFormService = servicesFormService;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            string strcompany = GetActorClaims();
            
            string username = GetUserClaims();
            
            string Role = GetClaims();
            if(Role==null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }

            List<string> allowedModule = new List<string>();

            string User = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(User);
            allowedModule = _userPermissionService.GetUserPermissionName(UserId, "HomePage");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _userPermissionService.GetCurrentRolePermissionName(Role, "HomePage");
                }
            }
            if(allowedPermission.Count == 0)
            {
                allowedPermission = _userPermissionService.GetRolePermissionedModuleName(Role);
            }

            int companyId = Convert.ToInt32(strcompany);
            int userId = _userPermissionService.GetUserIdForPermission(username);
            UserCompanyViewModel company = new UserCompanyViewModel();

            company = _userCompanyService.GetCompanyById(companyId);

            ImportantReminderViewModel importantReminderViewModel = new ImportantReminderViewModel();
            importantReminderViewModel = _importantReminderService.GetImportantReminderMessages(companyId);

            HomePageViewModel model = new HomePageViewModel();
            model.CompanyId = companyId;
            model.Username = username;
            model.UserId = userId;
            model.Module = name;
            model.Role = Role;
            model.CountOfManager = _userEmployeesService.GetCountOfManagers(companyId);
            model.CountOfShareholders = _userEmployeesService.GetCountOfShareholders(companyId);
            model.CountOfEmployee = _userEmployeesService.GetCountOfEmployees(companyId, userId);
            if (companyId == 1)
            {
                model.CountOfOpenService = _employeeServiceList.GetOpenServiceCountForAssignedUser(userId);
            }
            else
            {
                if (Role.StartsWith("Sales"))
                {
                    model.CountOfOpenService = _employeeServiceList.GetOpenServiceCount(0,userId,0);
                }
                else if(Role == "RM Team")
                {
                    model.CountOfOpenService = _employeeServiceList.GetOpenServiceCount(0, 0, userId);
                }
                else
                {
                    model.CountOfOpenService = _employeeServiceList.GetOpenServiceCount(companyId,0,0);
                }
            }
            if (companyId == 1)
            {
                model.CountOfRejectedService = _employeeServiceList.GetRejectedServiceCountForAssignedUser(userId);
            }
            else
            {
                if (Role.StartsWith("Sales"))
                {
                    model.CountOfRejectedService = _employeeServiceList.GetRejectedServiceCount(0, userId,0);
                }
                else if (Role == "RM Team")
                {
                    model.CountOfRejectedService = _employeeServiceList.GetRejectedServiceCount(0, 0, userId);
                }
                else
                {
                    model.CountOfRejectedService = _employeeServiceList.GetRejectedServiceCount(companyId, 0,0);
                }
            }
            if (companyId == 1)
            {
                model.CountOfCloseService = _employeeServiceList.GetCloseServiceCountForAssignedUser(userId);
            }
            else
            {
                if (Role.StartsWith("Sales"))
                {
                    model.CountOfCloseService = _employeeServiceList.GetCloseServiceCount(0,userId,0);
                }
                else if (Role == "RM Team")
                {
                    model.CountOfCloseService = _employeeServiceList.GetCloseServiceCount(0, 0,userId);
                }
                else
                {
                    model.CountOfCloseService = _employeeServiceList.GetCloseServiceCount(companyId,0,0);
                }
            }
            model.CountOfOpenSupportTicket = _employeeServiceList.GetOpenSupportTicketCount(companyId);
            model.CountOfCloseSupportTicket = _employeeServiceList.GetCloseSupportTicketCount(companyId);
            model.CompanyName = company.CompanyName;
            model.CompanyType = company.CompanyType;
            model.allowedModule = allowedModule;
            model.allowedPermission = allowedPermission;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllowedModuleList()
        {
            try
            {
                List<string> allowedPermission = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _userPermissionService.GetUserIdForPermission(User);
                allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);

                if (allowedPermission.Count == 0)
                {
                    allowedPermission = _userPermissionService.GetRolePermissionedModuleName(Role);
                }
                return new JsonResult(allowedPermission);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyName()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            UserCompanyViewModel company = new UserCompanyViewModel();
            company = _userCompanyService.GetCompanyById(CompanyId);
            return new JsonResult(company);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetAssignedServicesNotification()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _user = GetUserClaims();
            int _userId = _userPermissionService.GetUserIdForPermission(_user);
            int _returnId = _importantReminderService.GetAssignedServicesCount(_userId);
            return new JsonResult(_returnId);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetAssignedSupportTicketNotification()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _user = GetUserClaims();
            int _userId = _userPermissionService.GetUserIdForPermission(_user);
            int _returnId = _importantReminderService.GetAssignedSupportTicketCount(_userId);
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRequestNotifications(string? name, string? StartDate, string? EndDate)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            List<GetRequestNotificationModel> listData = new List<GetRequestNotificationModel>();
            string _user = GetUserClaims();
            int _userId = _userPermissionService.GetUserIdForPermission(_user);
            listData = _importantReminderService.GetRequestNotifications(_userId,StartDate, EndDate);
            if (listData.Count > 0 && listData != null)
            {
                foreach (GetRequestNotificationModel item in listData)
                {
                    item.Module = name;
                }
            }
            else
            {
                GetRequestNotificationModel model = new GetRequestNotificationModel();
                model.Module = name;
                listData.Add(model);
            }

            return View(listData);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult ChangeRequestStatus(int Status, int Id)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            _returnId = _importantReminderService.ChangeRequestNotificationStatus(Status, Id);
            return new JsonResult(_returnId);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetCurrentNotificationCount()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _count = 0;
            string _user = GetUserClaims();
            int _userId = _userPermissionService.GetUserIdForPermission(_user);
            _count = _employeeServiceList.GetCurrentNotificationCount(_userId);
            return new JsonResult(_count);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetLoggedInCompany()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _company = GetActorClaims();
            int CompanyId = Convert.ToInt32(_company);
            return new JsonResult(CompanyId);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetLoggedInRole()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            return new JsonResult(Role);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetNoticeMessage()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string strcompany = GetActorClaims();
            int companyId = Convert.ToInt32(strcompany);

            ImportantReminderViewModel importantReminderViewModel = new ImportantReminderViewModel();
            if (companyId!=1)
                importantReminderViewModel = _importantReminderService.GetImportantReminderMessages(companyId);
            return new JsonResult(importantReminderViewModel);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllNotification()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string strcompany = GetActorClaims();
            int companyId = Convert.ToInt32(strcompany);
            List<ImportantReminderViewModel> importantReminderViewModel = new List<ImportantReminderViewModel>();
            if (companyId != 1)
                importantReminderViewModel = _importantReminderService.GetImportantReminderMessagesList(companyId);
            return View("GetAllNotices", importantReminderViewModel);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult MarkAsReadNotification(int CompanyId,int Id)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            _returnId = _importantReminderService.MarkAsReadNotification(CompanyId,Id);
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult MarkAsReadServiceNotification(string serialNumber)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            string _user = GetUserClaims();
            int _userId = _userPermissionService.GetUserIdForPermission(_user);
            _returnId = _importantReminderService.MarkAsReadServiceNotification(_userId, serialNumber);
            return new JsonResult(_returnId);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetLoggedInUserStatus()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _user = GetUserClaims();
            string _companyId = GetActorClaims();

            LoggedinStatus model = new LoggedinStatus();
            model = _userEmployeesService.GetLoggedinStatus(Convert.ToInt32(_companyId), Role, _user);
            return new JsonResult(model);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        [Route("PageNotFound")]
        [AllowAnonymous]
        public IActionResult PageNotFound()
        {
            return View();
        }
        [HttpGet]
        [Route("[action]")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCurrentCompanyId()
        {
            string companyId = GetActorClaims();
            int _companyId = Convert.ToInt32(companyId);
            return new JsonResult(_companyId);
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
            var currentActor = actor.Where(x => !x.Value.Contains("_DIBN")).ToList();
            if (currentActor.Count > 0)
            {
                Company = currentActor[0].Value;
            }
            return Company;
        }
        public string GetUserClaims()
        {
            string UserDetails = "";
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var userClaimType = userIdentity.NameClaimType;
            var users = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
            var currentUser = users.Where(x => !x.Value.Contains("_DIBN")).ToList();
            if(currentUser.Count > 0)
            {
                UserDetails = currentUser[0].Value;
            }
            return UserDetails;
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
        public IActionResult RoleChange()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserName()
        {
            string user = GetUserClaims();
            string userName = null;
            int userId = _userPermissionService.GetUserIdForPermission(user);
            UserEmployeeViewModel usermodel = new UserEmployeeViewModel();
            usermodel = _userEmployeesService.GetUserDetail(userId);
            userName = usermodel.FirstName + " " + usermodel.LastName;
            return new JsonResult(userName);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserNameSalesPerson()
        {
            string user = GetUserClaims();
            string userName = null;
            userName = _userPermissionService.GetSalesPersonName(user);
            return new JsonResult(userName);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRMTeamName()
        {
            string user = GetUserClaims();
            string userName = null;
            userName = _userPermissionService.GetRMTeamName(user);
            return new JsonResult(userName);
        }
    }
}
