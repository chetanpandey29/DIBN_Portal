using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class SupportTicketRequestController : Controller
    {
        private readonly ISupportTicketRepository _supportTicketRepository;
        private readonly IPermissionRepository _permissionRepository;
        public readonly IUserRepository _userRepository;
        public SupportTicketRequestController(ISupportTicketRepository supportTicketRepository, IPermissionRepository permissionRepository, IUserRepository userRepository)
        {
            _supportTicketRepository = supportTicketRepository;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? Status)
        {
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);

            SupportTicketRequest GetAllSupportTickets = new SupportTicketRequest();
            GetAllSupportTickets.Module = name;
            GetAllSupportTickets.SelectedStatus = Status;
            
            return View(GetAllSupportTickets);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetSupportTicketRequestDocument(int Id)
        {
            try
            {
                SupportTicketDocunents document = new SupportTicketDocunents();
                document = _supportTicketRepository.GetUploadedDocumets(Id);
                string Files = document.FileName;
                MemoryStream ms = new MemoryStream(document.DataBinary);
                return File(document.DataBinary, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName + document.Extension);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CreateNewSupportTicket(string? name)
        {
            string Username = GetUserClaims();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            SupportTicketRequest model = new SupportTicketRequest();
            model.Module = name;
            model.CompanyId = CompanyId;
            model.UserId = UserId;
            return PartialView("_AddNewSupportTicket", model);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddSupportTicket(SupportTicketRequest model)
        {
            int _returnId = 0;
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            model.CreatedBy = UserId;
            _returnId = _supportTicketRepository.AddNewSupportTicket(model);
            return RedirectToAction("Index", "SupportTicketRequest", new { name = model.Module });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTrackingNumber()
        {
            string _returnId = null;
            _returnId = _supportTicketRepository.GetTrackingNumber();
            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(_returnId);
            string alphaPart = result.Groups[1].Value;
            int numberPart = Convert.ToInt32(result.Groups[2].Value);
            int Id = numberPart + 1;
            _returnId = alphaPart + Id.ToString();
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult TrackYourTicket(string? Id, string? name, int? message,int? companyId,string? actionMethod)
        {
            try
            {
                List<SupportTicketRequest> supportTicketRequest = new List<SupportTicketRequest>();
                supportTicketRequest = _supportTicketRepository.GetSupportTicketDetail(Id);
                if (supportTicketRequest.Count > 0)
                {
                    foreach (var item in supportTicketRequest)
                    {
                        item.Module = name;
                        if (item.NewTicketStatus != null && item.NewTicketStatus != "")
                        {
                            var data = item.NewTicketStatus.Split("-");
                            item.ChangedRequestStatus = data;
                        }
                        if (companyId != null)
                            item.companyId = companyId.Value;
                        item.actionMethod = actionMethod;
                    }
                }
                else
                {
                    SupportTicketRequest request = new SupportTicketRequest();
                    request.Module = name;
                    if (companyId != null)
                        request.companyId = companyId.Value;
                    request.actionMethod = actionMethod;
                    supportTicketRequest.Add(request);
                }
                if (message > 0)
                {
                    ViewData["Message"] = "Your Response Sended Successfully.";
                }
                else if (message < 0)
                {
                    ViewData["Message"] = "Sorry,Due to some problem currently we are unable to Send your Response. Please try again later..!!";
                }
               
                Log.Information("Track Support Ticket with Tracking Number equals to " + Id);
                return View(supportTicketRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SendSupportTicket(string? name, string TrackingNumber)
        {
            string Username = GetUserClaims();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            int ticketStatus = await _supportTicketRepository.GetLastStatusOfSupportTicket(TrackingNumber);
            SupportTicketRequest model = new SupportTicketRequest();
            model.Module = name;
            model.CompanyId = CompanyId;
            model.UserId = UserId;
            model.TrackingNumber = TrackingNumber;
            model.TicketStatusId = ticketStatus;
            return PartialView("_SendSupportTicket", model);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SendSupportTicketResponse(string? name, string TrackingNumber, int ParentId)
        {
            string Username = GetUserClaims();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            int ticketStatus = await _supportTicketRepository.GetLastStatusOfSupportTicket(TrackingNumber);
            SupportTicketRequest model = new SupportTicketRequest();
            model.Module = name;
            model.CompanyId = CompanyId;
            model.UserId = UserId;
            model.TrackingNumber = TrackingNumber;
            model.ParentId = ParentId;
            model.TicketStatusId = ticketStatus;
            return PartialView("_SendSupportTicketResponse", model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult SendResponseBody(SupportTicketRequest request)
        {
            try
            {
                int _returnId = 0;
                string username = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(username);
                request.CreatedBy = UserId;
                _returnId = _supportTicketRepository.AddNewSupportTicket(request);
                Log.Information("New Response (" + request.Description + ") added to " + request.TrackingNumber);
                return RedirectToAction("TrackYourTicket", "SupportTicketRequest", new { Id = request.TrackingNumber, name = request.Module, message = _returnId });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetEmployeesToAssign(string TrackingNumber, string Module, string? actionMethod, int? companyId)
        {
            string _module = "SupportTickets";
            int _userId = 0;
            SaveAssignUserOfSupportTikcet users = new SaveAssignUserOfSupportTikcet();
            List<GetMainCompanyEmployees> employees = new List<GetMainCompanyEmployees>();
            List<GetMainCompanyEmployees> employeeswithpermission = new List<GetMainCompanyEmployees>();
            List<int> AssignedUsers = new List<int>();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            string _username = GetUserClaims();
            _userId = _permissionRepository.GetUserIdForPermission(_username);
            employees = _userRepository.GetMainCompanyEmployees(CompanyId);

            var supportTicketEmployees = employees.Where(x => !x.Designation.Contains("DIBN")).ToList();
            List<SelectListItem> allUsers = new List<SelectListItem>();

            for (int i = 0; i < supportTicketEmployees.Count; i++)
            {
                allUsers.Add(new SelectListItem
                {
                    Text = supportTicketEmployees[i].FirstName + " " + supportTicketEmployees[i].LastName,
                    Value = supportTicketEmployees[i].Id.ToString()
                });
            }
            AssignedUsers = _supportTicketRepository.GetAllAssignedUsers(TrackingNumber);
            if (AssignedUsers.Count > 0 && AssignedUsers != null)
            {
                foreach (var item in supportTicketEmployees)
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
            for (int index = 0; index < supportTicketEmployees.Count; index++)
            {
                int _permissionCount = 0;
                List<string> userModule = new List<string>();
                List<string> roleModule = new List<string>();
                userModule = _permissionRepository.GetUserPermissionName(supportTicketEmployees[index].Id, _module);
                roleModule = _permissionRepository.GetRolePermissionName(supportTicketEmployees[index].Designation.Substring(1), _module, supportTicketEmployees[index].Id, null);
                _permissionCount = _permissionRepository.GetUserPermissionCount(supportTicketEmployees[index].Id);
                if (_permissionCount > 0)
                {
                    if (userModule.Count > 0)
                    {
                        if (supportTicketEmployees[index].Id != _userId)
                            employeeswithpermission.Add(supportTicketEmployees[index]);
                    }
                }
                else
                {
                    if (roleModule.Count > 0)
                    {
                        if (supportTicketEmployees[index].Id != _userId)
                            employeeswithpermission.Add(supportTicketEmployees[index]);
                    }
                }

            }
            users.getMainCompanyEmployees = employeeswithpermission;
            users.TrackingNumber = TrackingNumber;
            users.Module = Module;
            users.Users = allUsers;
            users.actionMethod = actionMethod;
            if(companyId!=null)
                users.companyId = companyId.Value;
            //users.UserId = _supportTicketRepository.GetAssignedUserOfSupportTicket(TrackingNumber).ToArray();
            Log.Information("Get DIBN Employee list to Assign Any Employee for Support Ticket " + TrackingNumber);
            return PartialView("_GetEmployeesToAssign", users);
        }
        [HttpPost]
        [Route("[action]")]

        public IActionResult SaveAssignedUser(SaveAssignUserOfSupportTikcet user)
        {
            int _returnId = 0;
            string _user = GetUserClaims();
            int _userId = _permissionRepository.GetUserIdForPermission(_user);
            user.CreatedBy = _userId;
            _returnId = _supportTicketRepository.SaveAssignUserOfSupportTicket(user);
            if (user.actionMethod != null && user.actionMethod == "Company")
            {
                return RedirectToAction("GetCompanySupportTickets", "Company", new { Id = user.companyId, Module = "Company" });
            }
            return RedirectToAction("Index", "SupportTicketRequest", new { name = user.Module });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(string TrackingNumber)
        {
            int _returnId = 0;
            string _user = GetUserClaims();
            int _userId = _permissionRepository.GetUserIdForPermission(_user);
            _returnId = _supportTicketRepository.RemoveSupportTicketRequest(TrackingNumber, _userId);
            return new JsonResult(_returnId);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllSupportTicketsWithPagination(int? status, int page)
        {
            try
            {
                GetSupportsTicketsWithPaginationForAdminModel model = new GetSupportsTicketsWithPaginationForAdminModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _supportTicketRepository.GetSupportsTicketsWithPaginationForAdmin(status, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalSupportTickets;
                totalRecord = model.totalSupportTickets;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.supportTickets
                };
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
