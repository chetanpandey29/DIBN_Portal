using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class HomeController : Controller
    {
        public readonly ICompanyRepository _companyRepository;
        public readonly IUserRepository _userRepository;
        public readonly IEmployeeServiceRepository _employeeServiceRepository;
        public readonly IPermissionRepository _permissionRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IEnquiryFormRepository _enquiryFormRepository;
        private readonly IReportManagementRepository _reportManagementRepository;

        public HomeController(ICompanyRepository companyRepository, IUserRepository userRepository, IEmployeeServiceRepository employeeServiceRepository, 
            IPermissionRepository permissionRepository,
            IMemoryCache memoryCache,
            IEnquiryFormRepository enquiryFormRepository,
            IReportManagementRepository reportManagementRepository
            )
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _employeeServiceRepository = employeeServiceRepository;
            _permissionRepository = permissionRepository; 
            _memoryCache = memoryCache;
            _enquiryFormRepository = enquiryFormRepository;
            _reportManagementRepository = reportManagementRepository;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            
            if(User.Identity.IsAuthenticated)
            {

                List<string> allowedModule = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "HomePage");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "HomePage");
                    }
                }

                if (allowedPermission.Count == 0)
                {
                    allowedPermission = _permissionRepository.GetRolePermissionedModuleName(Role);
                }

                string _username = GetUserClaims();
                Log.Information("Admin Dashboard");
                var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                
                List<int> _companies = new List<int>();
                _companies = _companyRepository.GetCompaniesCount();

                List<int> _employees = new List<int>();
                _employees = _userRepository.GetEmployeesCount();
                HomePageViewModel homePageViewModel = new HomePageViewModel();
                homePageViewModel.CountofCompany = _companies.Count > 0 ? _companies[2] : 0;
                homePageViewModel.CountOfMainLandCmp = _companies.Count > 0 ? _companies[0] : 0;
                homePageViewModel.CountOfCFreezoneCmp = _companies.Count > 0 ? _companies[1] : 0;
                homePageViewModel.CountofEmployees = _employees.Count > 0 ? _employees[2] : 0;
                homePageViewModel.CountofMainCompanyEmp = _employees.Count > 0 ? _employees[0] : 0;
                homePageViewModel.CountofOtherCompanyEmp = _employees.Count > 0 ? _employees[1] : 0;
                homePageViewModel.CountofOpenServie = _employeeServiceRepository.GetOpenServiceCount();
                homePageViewModel.CountofCloseService = _employeeServiceRepository.GetCloseServiceCount();
                homePageViewModel.CountofRejectedService = _employeeServiceRepository.GetRejectedServiceCount();
                homePageViewModel.CountOfOpenSupportTicket = _employeeServiceRepository.GetOpenSupportTicketCount();
                homePageViewModel.CountOfCloseSupportTicket = _employeeServiceRepository.GetCloseSupportTicketCount();
                homePageViewModel.getPortalBalance = _userRepository.GetTotalPortalBalance();
                homePageViewModel.Module = name;
                homePageViewModel.allowedModule = allowedModule;
                homePageViewModel.allowedPermission = allowedPermission;
                return View(homePageViewModel);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account", new {area=""});
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllowedModuleList()
        {

            List<string> allowedPermission = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);

            if (allowedPermission.Count == 0)
            {
                allowedPermission = _permissionRepository.GetRolePermissionedModuleName(Role);
            }
            return new JsonResult(allowedPermission);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ClearCache()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetPortalBalance()
        {
            List<decimal> getPortalBalance = new List<decimal>();
            getPortalBalance = _userRepository.GetTotalPortalBalance();
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("[");
            for (int i = 0; i < getPortalBalance.Count; i++)
            {
                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _text.Add("Current Balance");
                _color.Add("#7FFF00");
                _color.Add("#D60000");
                _color.Add("#0000FF");
                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                string color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[i], getPortalBalance[i], _color[i]));
                stringBuilder.Append("},");
            }
            stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("]");
            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTotalCompanies()
        {
            List<int> _companies = new List<int>();
            _companies = _companyRepository.GetCompaniesCount();
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("[");
            for (int i = 0; i < _companies.Count; i++)
            {
                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                
                _text.Add("Total Mainland Company(s)");
                _text.Add("Total Freezone Company(s)");
                _text.Add("Total Company(s)");

                _color.Add("#00395D");
                _color.Add("#FFC0CB");
                _color.Add("#008000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                string color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[i], _companies[i], _color[i]));
                stringBuilder.Append("},");
            }
            stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("]");
            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetWeeklyCompanySubTypeReport()
        {
            List<GetWeeklyCompanySubTypeReportModel> model = new List<GetWeeklyCompanySubTypeReportModel>();
            model = await _companyRepository.GetWeeklyCompanySubTypeReport();
            return new JsonResult(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetMonthlyCompanySubTypeReport()
        {
            List<GetMonthlyCompanySubTypeReportModel> model = new List<GetMonthlyCompanySubTypeReportModel>();
            model = await _companyRepository.GetMonthlyCompanySubTypeReport();
            return new JsonResult(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetYearlyCompanySubTypeReport()
        {
            List<GetYearlyCompanySubTypeReportModel> model = new List<GetYearlyCompanySubTypeReportModel>();
            model = await _companyRepository.GetYearlyCompanySubTypeReport();
            return new JsonResult(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossYearlyGraphOfMainCompany(string year)
        {
            ProfitLossYearlyReportModel profitLoss = new ProfitLossYearlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossYearlyReportTotalDetails(year);

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}',year:'{3}'", _text[0], profitLoss.TotalCredit, _color[0],year));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}',year:'{3}'", _text[1], profitLoss.TotalDebit, _color[1], year));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetYearlyGraphOfMainCompany()
        {
            GetProfitLossGraphReportForDashboardModel profitLoss = new GetProfitLossGraphReportForDashboardModel();
            profitLoss = await _reportManagementRepository.GetProfitLossGraphReportForDashboard();

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.CreditedAmount, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.DeditedAmount, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTotalEmployees()
        {
            List<int> _employees = new List<int>();
            _employees = _userRepository.GetEmployeesCount();
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("[");
            for (int i = 0; i < _employees.Count; i++)
            {
                List<string> _text = new List<string>();
                _text.Add("Total Employees");
                _text.Add("Total DIBN Employees");
                _text.Add("Total Other Company's Employees");
                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                string color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[i], _employees[i], color));
                stringBuilder.Append("},");
            }
            stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("]");
            return Content(stringBuilder.ToString());
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetCurrentNotificationCount()
        {
            int _count = 0;
            string _username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(_username);
            _count = _userRepository.GetCurrentNotificationCount(userId);
            return new JsonResult(_count);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetEmployeesToAssign(int RequestId, string EnquiryNumber)
        {
            int _userId = 0;
            string _module = "Enquiry";
            SaveAssignUserForEnquiry users = new SaveAssignUserForEnquiry();
            List<GetMainCompanyEmployees> employees = new List<GetMainCompanyEmployees>();
            List<GetMainCompanyEmployees> employeeswithpermission = new List<GetMainCompanyEmployees>();
            List<int> AssignedUsers = new List<int>();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            string _username = GetUserClaims();
            _userId = _permissionRepository.GetUserIdForPermission(_username);
            employees = _userRepository.GetMainCompanyEmployees(CompanyId);
            AssignedUsers = _enquiryFormRepository.GetAllAssignedUsers(RequestId);
            if (AssignedUsers.Count > 0 && AssignedUsers != null)
            {
                foreach (var item in employees)
                {
                    for (int index = 0; index < AssignedUsers.Count; index++)
                    {
                        if (item.Id == AssignedUsers[index])
                        {
                            item._checked = "checked";
                        }
                    }
                }
            }

            for (int index = 0; index < employees.Count; index++)
            {
                int _permissionCount = 0;
                List<string> userModule = new List<string>();
                List<string> roleModule = new List<string>();
                userModule = _permissionRepository.GetUserPermissionName(employees[index].Id, _module);
                roleModule = _permissionRepository.GetRolePermissionName(employees[index].Designation.Substring(1), _module, null, null);
                _permissionCount = _permissionRepository.GetUserPermissionCount(employees[index].Id);
                if (_permissionCount > 0)
                {
                    if (userModule.Count > 0)
                    {
                        if (employees[index].Id != _userId)
                            employeeswithpermission.Add(employees[index]);
                    }
                }
                else
                {
                    if (roleModule.Count > 0)
                    {
                        if (employees[index].Id != _userId)
                            employeeswithpermission.Add(employees[index]);
                    }
                }

            }

            users.getMainCompanyEmployees = employeeswithpermission;
            users.RequestId = RequestId;
            users._assignedUsers = AssignedUsers;
            users.EnquiryNumber = EnquiryNumber;
            return PartialView("_GetEmployeesToAssign", users);
        }
        [HttpPost]
        [Route("[action]")]

        public IActionResult SaveAssignedUser(SaveAssignUserForEnquiry user)
        {
            int _returnId = 0;
            string _user = GetUserClaims();
            int _userId = _permissionRepository.GetUserIdForPermission(_user);
            user.CreatedBy = _userId;
            _returnId = _enquiryFormRepository.SaveAssignUserOfService(user);
            return RedirectToAction("WebsiteEnquiries", "Home", new { name = "Enquiry" });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult ChangeEnquiryStatus(int RequestId, string EnquiryNumber)
        {
            string lastStatus = "";
            lastStatus = _enquiryFormRepository.GetStatusOfEnquiryForm(RequestId);
            List<SelectListItem> StatusType = new List<SelectListItem>();
            StatusType.Add(new SelectListItem
            {
                Text = "Select Status of Request",
                Value = "Pending"
            });
            StatusType.Add(new SelectListItem
            {
                Text = "Pending",
                Value = "Pending"
            });
            StatusType.Add(new SelectListItem
            {
                Text = "Approved",
                Value = "Approved"
            });
            StatusType.Add(new SelectListItem
            {
                Text = "Rejected",
                Value = "Rejected"
            });
            ChangeEnquiryStatusModel model = new ChangeEnquiryStatusModel();
            model.Status = lastStatus;
            model.RequestId = RequestId;
            model.StatusItems = StatusType;
            model.EnquiryNumber = EnquiryNumber;
            return PartialView("_ChangeEnquiryStatus", model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult SaveChangedStatusOfEnquiry(ChangeEnquiryStatusModel model)
        {
            int _returnId = 0;
            int userId = 0;
            string username = GetUserClaims();
            userId = _permissionRepository.GetUserIdForPermission(username);
            model.UserId = userId;
            _returnId = _enquiryFormRepository.SaveStatusOfEnquiryForm(model);
            return RedirectToAction("WebsiteEnquiries", "Home", new { name = "Enquiry" });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id)
        {
            int _returnId = 0;
            _returnId = _enquiryFormRepository.DeleteEnquiry(Id);
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetLoggedInUser()
        {
            string UserDetails = "";
            UserDetails = GetUserClaims();
            return new JsonResult(UserDetails);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetLoggedInUserStatus()
        {
            string Role = GetClaims();
            string _user = GetUserClaims();
            string _companyId = GetActorClaims();

            LoggedinStatus model = new LoggedinStatus();
            model = _userRepository.GetLoggedinStatus(Convert.ToInt32(_companyId), Role, _user);
            if (Role == null || Role == "")
            {
                model.IsLoggedIn = 1;
            }
            return new JsonResult(model);
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
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyName()
        {
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            CompanyViewModel company = new CompanyViewModel();
            company = _companyRepository.GetCompanyById(CompanyId);
            return new JsonResult(company);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserName()
        {
            string user = GetUserClaims();
            string userName = string.Empty;
            int userId = _permissionRepository.GetUserIdForPermission(user);
            UserViewModel usermodel = new UserViewModel();
            usermodel = _userRepository.GetUserDetail(userId);
            userName = usermodel.FirstName +" " + usermodel.LastName;
            return new JsonResult(userName);
        }
    }
}
