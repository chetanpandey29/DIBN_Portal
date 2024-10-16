using DIBN.IService;
using DIBN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class SupportTicketController : Controller
    {
        private readonly ISupportTicketService _supportTicketService;
        private readonly IUserPermissionService _userPermissionService;

        public SupportTicketController(ISupportTicketService supportTicketService, IUserPermissionService userPermissionService)
        {
            _supportTicketService = supportTicketService;
            _userPermissionService = userPermissionService;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? Status)
        {
            string Role = "";
            int _userId = 0;
            Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);

            string Username = GetUserClaims();
            _userId = _userPermissionService.GetUserIdForPermission(Username);

            SupportTicketViewModel GetAllSupportTickets = new SupportTicketViewModel();
            
            GetAllSupportTickets.Module = name;
            GetAllSupportTickets.Role = Role;
            GetAllSupportTickets.CompanyId = CompanyId;
            GetAllSupportTickets.SelectedStatus = Status;
            
            return View(GetAllSupportTickets);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CreateNewSupportTicket(string? name)
        {
            string role = GetClaims();
            if (role == null || role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string Username = GetUserClaims();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            SupportTicketViewModel model = new SupportTicketViewModel();
            model.Module = name;
            model.CompanyId = CompanyId;

            List<SalesPersonCompany> company = new List<SalesPersonCompany>();
            if (role.StartsWith("Sales"))
            {
                int salesPersonId = _userPermissionService.GetSalesPersonIdForPermission(Username);
                company = _userPermissionService.GetAllAssignedCompaniesSalesPerson(salesPersonId);
                List<SelectListItem> companies = new List<SelectListItem>();
                companies.Add(new SelectListItem
                {
                    Text = "Select Company",
                    Value = ""
                });
                for (var i = 0; i < company.Count; i++)
                {
                    companies.Add(new SelectListItem
                    {
                        Text = company[i].CompanyName,
                        Value = company[i].CompanyId.ToString()
                    });
                }
                model.Companies = companies;
                model.CompanyId = 0;
                model.SalesPersonId = salesPersonId;
            }
            else
            {
                model.UserId = UserId;
            }

            model.Role = role;
            return PartialView("_AddNewSupportTicket", model);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddSupportTicket(SupportTicketViewModel model)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            string _username = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(_username);
            model.CreatedBy = UserId;
            _returnId = _supportTicketService.AddNewSupportTicket(model);

            Log.Information("New Support Ticket " + model.TrackingNumber + " is Created by " + _username);
            return RedirectToAction("Index", "SupportTicket", new { name = model.Module });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTrackingNumber()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _returnId = null;
            _returnId = _supportTicketService.GetTrackingNumber();
            //Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            //Match result = re.Match(_returnId);
            //string alphaPart = result.Groups[1].Value;
            //int numberPart = Convert.ToInt32(result.Groups[2].Value);
            //int Id = numberPart + 1;
            //_returnId = alphaPart + Id.ToString();
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult TrackYourTicket(string? Id, string? name, int? message)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                string currentCompany = GetActorClaims();
                int currentCompanyId = Convert.ToInt32(currentCompany);
                List<SupportTicketViewModel> supportTicketRequest = new List<SupportTicketViewModel>();
                supportTicketRequest = _supportTicketService.GetSupportTicketDetail(Id);
                if (supportTicketRequest.Count > 0)
                {
                    foreach (var item in supportTicketRequest)
                    {
                        item.Module = name;
                        if (item.NewTicketStatus != null && item.NewTicketStatus != "")
                        {
                            var data = item.NewTicketStatus.Split("-");
                            item.ChangedRequestStatus = data;
                            item.CurrentCompany = currentCompanyId;
                        }
                    }
                }
                else
                {
                    SupportTicketViewModel request = new SupportTicketViewModel();
                    request.Module = name;
                    request.CurrentCompany = currentCompanyId;
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
                return View(supportTicketRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetSupportTicketRequestDocument(int Id)
        {
            try
            {
                SupportTicketDocunents document = new SupportTicketDocunents();
                document = _supportTicketService.GetUploadedDocumets(Id);
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
        public async Task<IActionResult> SendSupportTicket(string? name, string TrackingNumber)
        {
            string role = GetClaims();
            string Username = GetUserClaims();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            int ticketStatus = await _supportTicketService.GetLastStatusOfSupportTicket(TrackingNumber);
            SupportTicketViewModel model = new SupportTicketViewModel();
            model.Module = name;
            model.CompanyId = CompanyId;
            if (role.StartsWith("Sales"))
                model.SalesPersonId = UserId;
            else
                model.UserId = UserId;
            model.TrackingNumber = TrackingNumber;
            model.TicketStatusId = ticketStatus;
            return PartialView("_SendSupportTicket", model);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SendSupportTicketResponse(string? name, string TrackingNumber, int ParentId)
        {
            string role = GetClaims();
            string Username = GetUserClaims();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            int ticketStatus = await _supportTicketService.GetLastStatusOfSupportTicket(TrackingNumber);
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            SupportTicketViewModel model = new SupportTicketViewModel();
            model.Module = name;
            model.CompanyId = CompanyId;
            if (role.StartsWith("Sales"))
                model.SalesPersonId = UserId;
            else
                model.UserId = UserId;
            model.TrackingNumber = TrackingNumber;
            model.ParentId = ParentId;
            model.TicketStatusId = ticketStatus;
            return PartialView("_SendSupportTicketResponse", model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult SendResponseBody(SupportTicketViewModel request)
        {
            try
            {
                int _returnId = 0;
                string Username = GetUserClaims();
                int UserId = _userPermissionService.GetUserIdForPermission(Username);
                request.CreatedBy = UserId;
                _returnId = _supportTicketService.AddNewSupportTicket(request);
                Log.Information("New Response (" + request.Description + ") added to " + request.TrackingNumber);
                return RedirectToAction("TrackYourTicket", "SupportTicket", new { Id = request.TrackingNumber, name = request.Module, message = _returnId });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllAssignedSupportTickets(int? status, int page)
        {
            try
            {
                GetAssignedSupportTicketsByUserWithPaginationModel model = new GetAssignedSupportTicketsByUserWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model =await _supportTicketService.GetAssignedSupportTicketsByUserWithPagination(_userId, status, skip, pageSize, searchValue,sortColumn, sortColumnDirection);
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllSupportTicketsBySalesPerson(int? status, int page)
        {
            try
            {
                GetSupportTicketsBySalesPersonWithPaginationModel model = new GetSupportTicketsBySalesPersonWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetSalesPersonIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _supportTicketService.GetSupportTicketsBySalesPersonWithPagination(_userId, status, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetCompanyWiseSupportTicket(int? status, int page)
        {
            try
            {
                GetSupportTicketsByCompanyIdWithPaginationModel model = new GetSupportTicketsByCompanyIdWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _supportTicketService.GetSupportTicketsByCompanyIdWithPagination(CompanyId, status, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
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

        /// <summary>
        /// Get Currently Logged in Company / User Role    -- Yashasvi TBC (24-11-2022)
        /// </summary>
        /// <returns></returns>
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
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckUsers()
        {
            int _returnId = 0;
            string role = GetClaims();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            if (!role.StartsWith("Sales") && role != "RM Team")
                _returnId = _userPermissionService.GetEmployeesCount(CompanyId, role);
            else
                _returnId = 1;
            return new JsonResult(_returnId);
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
    }
}
