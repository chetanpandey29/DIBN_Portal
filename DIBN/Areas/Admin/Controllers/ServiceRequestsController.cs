using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class ServiceRequestsController : Controller
    {
        private readonly IEmployeeServiceRepository _employeeServiceRepository;
        private readonly IServiceFormRepository _serviceFormRepository;
        public readonly IPermissionRepository _permissionRepository;
        public readonly IUserRepository _userRepository;
        private readonly IServiceRequestStatusRepository _serviceRequestStatusRepository;
        private IMemoryCache _cache;

        public ServiceRequestsController(IEmployeeServiceRepository employeeServiceRepository, IPermissionRepository permissionRepository, IUserRepository userRepository,
            IServiceFormRepository serviceFormRepository,
            IServiceRequestStatusRepository serviceRequestStatusRepository,
            IMemoryCache cache)
        {
            _employeeServiceRepository = employeeServiceRepository;
            _serviceFormRepository = serviceFormRepository;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _serviceRequestStatusRepository = serviceRequestStatusRepository;
            _cache = cache;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? Status)
        {
            GetBothRequestModel model = new GetBothRequestModel();
            model.SelectedStatus = Status;
            model.Module = name;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult TrackServiceRequest(string SerialNumber, int? Status, int? companyId, string? actionMethod,string? serviceRequestType)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "MyRequests");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "MyRequests");
                }
            }
            GetRequestCompleteDetails getRequest = new GetRequestCompleteDetails();
            getRequest = _serviceFormRepository.GetRequestDetail(SerialNumber);
            int _responseCount = 0;
            _responseCount = _serviceFormRepository.GetCountOfResponse(SerialNumber);
            int lastIndex = 0;

            if (getRequest.getRequestResponses.Count > 0)
            {
                lastIndex = getRequest.getRequestResponses.Count - 1;
                getRequest.lastStatus = getRequest.getRequestResponses[lastIndex].Status;
                getRequest.FilterStatus = Status;
            }
            else
            {
                lastIndex = getRequest.getRequestDetails.Count - 1;
                getRequest.lastStatus = getRequest.getRequestDetails[lastIndex].Status;
                getRequest.FilterStatus = Status;
            }
            if (companyId != null)
                getRequest.companyId = companyId.Value;
            getRequest.actionMethod = actionMethod;
            getRequest.serviceRequestType = serviceRequestType;
            getRequest.CountOfFields = _responseCount;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromDays(1))
                   .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                   .SetPriority(CacheItemPriority.High)
                   .SetSize(1024);
            if (serviceRequestType != null)
            {
                if (serviceRequestType == "Company")
                {
                    _cache.Set("GetAllCompanyServicesLastProcess", true, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllEmployeeServicesLastProcess", true, cacheEntryOptions);
                }
            }
            getRequest.allowedModule = allowedModule;
            return View(getRequest);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult ResponseToService(string SerialNumber, int ServiceId, int FormId, int CompanyId, string RequestedService,string? serviceRequestType)
        {
            GetRequestResponsesModel getRequestResponsesModel = new GetRequestResponsesModel();
            int _returnId = 0;
            _returnId = _serviceFormRepository.LastStepNumber(SerialNumber);
            string _user = GetUserClaims();
            int _userId = _permissionRepository.GetUserIdForPermission(_user);
            List<SelectListItem> StatusType = new List<SelectListItem>();

            List<ServiceRequestStatusModel> status = new List<ServiceRequestStatusModel>();
            status = _serviceRequestStatusRepository.GetServiceRequestStatus();

            StatusType.Add(new SelectListItem
            {
                Text = "Select Status of Request",
                Value = ""
            });
            for (int i = 0; i < status.Count; i++)
            {
                StatusType.Add(new SelectListItem
                {
                    Text = status[i].DisplayName,
                    Value = status[i].DisplayId.ToString()
                });
            }
            getRequestResponsesModel.SerialNumber = SerialNumber;
            getRequestResponsesModel.ServiceId = ServiceId;
            getRequestResponsesModel.FormId = FormId;
            getRequestResponsesModel.CompanyId = CompanyId;
            getRequestResponsesModel.UserId = _userId;
            getRequestResponsesModel.Title = RequestedService;
            getRequestResponsesModel.StepId = _returnId;
            getRequestResponsesModel.StatusId = _serviceFormRepository.GetLatestStatusOfRequest(SerialNumber);
            getRequestResponsesModel.StatusList = StatusType;
            getRequestResponsesModel.serviceRequestType = serviceRequestType;
            return PartialView("_ResponseToService", getRequestResponsesModel);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddServiceResponse(GetRequestResponsesModel getRequestResponsesModel)
        {
            int _returnId = 0;
            string Username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(Username);
            getRequestResponsesModel.CreatedBy = userId;
            _returnId = _serviceFormRepository.SaveStatusOfService(getRequestResponsesModel);
            Log.Information("Added New Service Response (" + getRequestResponsesModel.Description + ") for Service number :" + getRequestResponsesModel.SerialNumber);
            return RedirectToAction("TrackServiceRequest", "ServiceRequests", new { SerialNumber = getRequestResponsesModel.SerialNumber, serviceRequestType=getRequestResponsesModel.serviceRequestType });
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult ApproveRequest(string SerialNumber, int Status, string? name,string? serviceRequestType)
        {
            int _returnId = 0;
            string username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(username);
            _returnId = _serviceFormRepository.ChangeStatusOfService(SerialNumber, Status, UserId);
            Log.Information("Approved Service Request " + SerialNumber);
            return RedirectToAction("TrackServiceRequest", "ServiceRequests", new { SerialNumber = SerialNumber, name = name , serviceRequestType = serviceRequestType });
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult RejectRequest(GetRequestResponsesModel model)
        {
            int _returnId = 0;
            _returnId = _serviceFormRepository.LastStepNumber(model.SerialNumber);
            model.StepId = _returnId;
            string Username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(Username);
            model.CreatedBy = userId;
            _returnId = _serviceFormRepository.SaveStatusOfService(model);
            Log.Information("Rejected Service Request " + model.Description);
            return RedirectToAction("TrackServiceRequest", "ServiceRequests", new { SerialNumber = model.SerialNumber, serviceRequestType=model.serviceRequestType });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetEmployeesToAssign(string serialNumber, int ServiceId, string? actionMethod, int? companyId,string? ServiceRequestType)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

            string _module = "MyRequests";
            int _userId = 0;
            SaveAssignUser users = new SaveAssignUser();
            List<GetMainCompanyEmployees> employees = new List<GetMainCompanyEmployees>();
            List<GetMainCompanyEmployees> employeeswithpermission = new List<GetMainCompanyEmployees>();
            List<int> AssignedUsers = new List<int>();
            string _companyId = GetActorClaims();
            string _username = GetUserClaims();
            _userId = _permissionRepository.GetUserIdForPermission(_username);
            int CompanyId = Convert.ToInt32(_companyId);
            employees = _userRepository.GetMainCompanyEmployees(CompanyId);
            var serviceRequestEmployees = employees.Where(x => !x.Designation.Contains("DIBN")).ToList();
            AssignedUsers = _serviceFormRepository.GetAllAssignedUsers(serialNumber);
            if (AssignedUsers.Count > 0 && AssignedUsers != null)
            {
                foreach (var item in serviceRequestEmployees)
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

            for (int index = 0; index < serviceRequestEmployees.Count; index++)
            {
                int _permissionCount = 0;
                List<string> userModule = new List<string>();
                List<string> roleModule = new List<string>();
                userModule = _permissionRepository.GetUserPermissionName(serviceRequestEmployees[index].Id, _module);
                roleModule = _permissionRepository.GetRolePermissionName(serviceRequestEmployees[index].Designation.Substring(1), _module, serviceRequestEmployees[index].Id, null);
                _permissionCount = _permissionRepository.GetUserPermissionCount(serviceRequestEmployees[index].Id);

                if (_permissionCount > 0)
                {
                    if (userModule.Count > 0)
                    {
                        if (serviceRequestEmployees[index].Id != _userId && !serviceRequestEmployees[index].Designation.Contains("DIBN"))
                            employeeswithpermission.Add(serviceRequestEmployees[index]);
                    }
                }
                else
                {
                    if (roleModule.Count > 0)
                    {
                        if (serviceRequestEmployees[index].Id != _userId && !serviceRequestEmployees[index].Designation.Contains("DIBN"))
                            employeeswithpermission.Add(serviceRequestEmployees[index]);
                    }
                }

            }

            users.getMainCompanyEmployees = employeeswithpermission;
            users.SerialNumber = serialNumber;
            users.ServiceId = ServiceId;
            users._assignedUsers = AssignedUsers;
            users.actionMethod = actionMethod;
            if (ServiceRequestType != null)
            {
                if (ServiceRequestType == "Company")
                {
                    _cache.Set("GetAllCompanyServicesLastProcess", true, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllEmployeeServicesLastProcess", true, cacheEntryOptions);
                }
            }
            if (companyId != null)
                users.companyId = companyId.Value;
            return PartialView("_GetEmployeesToAssign", users);
        }
        [HttpPost]
        [Route("[action]")]

        public IActionResult SaveAssignedUser(SaveAssignUser user)
        {
            int _returnId = 0;
            string _user = GetUserClaims();
            int _userId = _permissionRepository.GetUserIdForPermission(_user);
            user.CreatedBy = _userId;
            _returnId = _serviceFormRepository.SaveAssignUserOfService(user);
            if (user.actionMethod != null && user.actionMethod == "Company")
            {
                return RedirectToAction("GetCompanyServiceRequests", "Company", new { Id = user.companyId, Module = "Company" });
            }
            return RedirectToAction("ServiceRequest", "ServiceRequests");
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetServiceRequestDocument(string SerialNumber, string FileName, int companyId, int serviceId)
        {
            try
            {
                List<DownloadServiceRequestUploadedDocument> GetRequests = new List<DownloadServiceRequestUploadedDocument>();
                GetRequests = _serviceFormRepository.DownloadUploadedServiceRequestDocument(SerialNumber, companyId, FileName, serviceId);
                string FileNames = "";
                byte[] databyte = { };
                foreach (var data in GetRequests)
                {
                    if (data.FileName == FileName)
                    {
                        FileNames = data.FileName;
                        databyte = data.FieldFileValue;
                        MemoryStream ms = new MemoryStream(data.FieldFileValue);
                        return File(data.FieldFileValue, System.Net.Mime.MediaTypeNames.Application.Octet, data.FileName);
                    }
                }
                return File(databyte, System.Net.Mime.MediaTypeNames.Application.Octet, FileNames);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(string SerialNumber, int ServiceId,string? ServiceRequestType)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);
            int _returnId = 0;
            string Username = GetUserClaims();
            int _userId = _permissionRepository.GetUserIdForPermission(Username);

            ////// Set Static User Id of Princy Ma'am to allow Delete any Service Request if Logged in User is Princy Ma'am Only based on Princy Ma'am's Request                    -- Yashasvi TBC [25-11-2022]
            ////// TBC01 is Used By TBC Developers for Testing                                                                                                                      -- Yashasvi TBC [25-11-2022]
            if (Username == "PRINCY05" || Username == "princy05" || Username == "TBC01" || Username == "tbc01")
            {
                _returnId = _serviceFormRepository.RemoveServiceRequest(SerialNumber, ServiceId, _userId);
            }
            if (ServiceRequestType != null)
            {
                if (ServiceRequestType == "Company")
                {
                    _cache.Set("GetAllCompanyServicesLastProcess", true, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllEmployeeServicesLastProcess", true, cacheEntryOptions);
                }
            }
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCurrentlyLoggedInUser()
        {
            string loggedInUser = GetUserClaims();
            return new JsonResult(loggedInUser);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetAllCompanyServices(int? status, int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromDays(1))
                   .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                   .SetPriority(CacheItemPriority.High)
                   .SetSize(1024);

                int count = 0, lastAccessedPage = 0;
                GetAllCompanyServiceRequestModel model = new GetAllCompanyServiceRequestModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllCompanyServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllCompanyServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllCompanyServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllCompanyServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllCompanyServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllCompanyServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllCompanyServicesSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllCompanyServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }

                model = _serviceFormRepository.GetAllCompanyServiceRequests(status, skip, pageSize, sortColumn, sortColumnDirection, searchValue, count);
                filterRecord = model.totalRecords;
                totalRecord = model.totalRecords;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRequests,
                    currentPage = lastAccessedPage,
                    displayData = pageSize,
                    sortDir = sortColumnDirection,
                    sortIndex = sortColumnIndex
                };

                if (lastProcess)
                {
                    _cache.Set("GetAllCompanyServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllCompanyServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllCompanyServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllEmployeeServices(int? status, int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromDays(1))
                   .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                   .SetPriority(CacheItemPriority.High)
                   .SetSize(1024);

                int count = 0, lastAccessedPage=0;
                GetAllEmployeeServiceRequestModel model = new GetAllEmployeeServiceRequestModel();

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
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllEmployeeServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllEmployeeServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllEmployeeServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllEmployeeServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllEmployeeServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllEmployeeServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllEmployeeServicesSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllEmployeeServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }

                model = await _serviceFormRepository.GetAllEmployeeServiceRequests(status, skip, pageSize, sortColumn, sortColumnDirection, searchValue, count);
                filterRecord = model.totalRecords;
                totalRecord =  model.totalRecords;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRequests,
                    currentPage = lastAccessedPage,
                    displayData = pageSize,
                    sortDir = sortColumnDirection,
                    sortIndex = sortColumnIndex
                };

                if (lastProcess)
                {
                    _cache.Set("GetAllEmployeeServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllEmployeeServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllEmployeeServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult ServiceRequest(string? name, int? Status)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "MyRequests");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "MyRequests");
                }
            }

            GetBothRequestModel model = new GetBothRequestModel();
            model.SelectedStatus = Status;
            model.Module = name;
            model.allowedModule = allowedModule;
            return View(model);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetAllCompanyServices_TEST(int? status, int page)
        {
            try
            {
                int count = 0;
                if (_cache.TryGetValue("CompanyServiceRequestStatus", out string statusData))
                {
                    if (statusData == status.ToString())
                    {
                        if (_cache.TryGetValue("CompanyServiceRequestTotalRecords", out int totalRecords))
                        {
                            count = totalRecords;
                        }
                    }
                }

                GetAllCompanyServiceRequestModel model = new GetAllCompanyServiceRequestModel();
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
                model = _serviceFormRepository.GetAllCompanyServiceRequests_TEST(status, page, pageSize, sortColumn, sortColumnDirection, searchValue, count);
                filterRecord = count != 0 ? count : model.totalRecords;
                totalRecord = count != 0 ? count : model.totalRecords;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRequests
                };
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromDays(1))
                   .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                   .SetPriority(CacheItemPriority.High)
                   .SetSize(1024);
                _cache.Set("CompanyServiceRequestStatus", status.ToString(), cacheEntryOptions);
                _cache.Set("CompanyServiceRequestTotalRecords", model.totalRecords, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllEmployeeServices_TEST(int? status, int page)
        {
            try
            {
                int count = 0;
                GetAllEmployeeServiceRequestModel model = new GetAllEmployeeServiceRequestModel();

                if (_cache.TryGetValue("EmployeeServiceRequestStatus", out string statusData))
                {
                    if (statusData == status.ToString())
                    {
                        if (_cache.TryGetValue("EmployeeServiceRequestTotalRecords", out int totalRecords))
                        {
                            count = totalRecords;
                        }
                    }
                }

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
                model = await _serviceFormRepository.GetAllEmployeeServiceRequests_TEST(status, skip, pageSize, sortColumn, sortColumnDirection, searchValue, count);
                filterRecord = count != 0 ? count : model.totalRecords;
                totalRecord = count != 0 ? count : model.totalRecords;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRequests
                };
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromDays(1))
                   .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                   .SetPriority(CacheItemPriority.High)
                   .SetSize(1024);
                _cache.Set("EmployeeServiceRequestStatus", status.ToString(), cacheEntryOptions);
                _cache.Set("EmployeeServiceRequestTotalRecords", model.totalRecords, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyServicesWithFilter(int? status, string sortColumn, string sortColumnDirection, string? searchBy, string? searchedValue)
        {
            try
            {
                GetAllCompanyServiceRequestModel model = new GetAllCompanyServiceRequestModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _serviceFormRepository.GetAllCompanyServiceRequestFilter(status, skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchedValue);
                filterRecord = model.totalRecords;
                totalRecord = model.totalRecords;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRequests
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
        public async Task<IActionResult> GetAllEmployeeServicesWithFilter(int? status, string sortColumn, string sortColumnDirection, string? searchBy, string? searchedValue)
        {
            try
            {
                GetAllEmployeeServiceRequestModel model = new GetAllEmployeeServiceRequestModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _serviceFormRepository.GetAllEmployeeServiceRequestFilter(status, skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchedValue);
                filterRecord = model.totalRecords;
                totalRecord = model.totalRecords;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRequests
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
        public async Task<IActionResult> GetAllCompanyServicesTempDate(int? status, string sortColumn, string sortColumnDirection, string? searchBy, string? searchedValue,int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromDays(1))
                   .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                   .SetPriority(CacheItemPriority.High)
                   .SetSize(1024);

                int lastAccessedPage=0;
                GetAllCompanyServiceRequestModel model = new GetAllCompanyServiceRequestModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                if (_cache.TryGetValue("GetAllCompanyServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllCompanyServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllCompanyServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllCompanyServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllCompanyServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllCompanyServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllCompanyServicesSearchBy", out string searchByData);
                                _cache.TryGetValue("GetAllCompanyServicesSearchByValue", out string searchByValueData);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumn = sortBy;
                                    sortColumnDirection = sortDir;
                                    if (searchByData != null && searchByData != "")
                                        searchBy = searchByData;
                                    if (searchByValueData != null && searchByValueData != "")
                                        searchedValue = searchByValueData;
                                    _cache.Set("GetAllCompanyServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }
                model = await _serviceFormRepository.GetAllCompanyServiceRequestTempData(status, skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchedValue);
                filterRecord = model.totalRecords;
                totalRecord = model.totalRecords;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRequests,
                    currentPage = lastAccessedPage,
                    displayData = pageSize,
                    sortColumnData = sortColumn,
                    sortColumnDirectionData = sortColumnDirection,
                    searchByData = searchBy,
                    searchedValueData = searchedValue 
                };
                if (lastProcess)
                {
                    _cache.Set("GetAllCompanyServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllCompanyServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllCompanyServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesSearchBy", searchBy, cacheEntryOptions);
                _cache.Set("GetAllCompanyServicesSearchByValue", searchedValue!=null? searchedValue :"", cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllEmployeeServicesTempDate(int? status, string sortColumn, string sortColumnDirection, string? searchBy, string? searchedValue,int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

                int lastAccessedPage = 0;

                GetAllEmployeeServiceRequestModel model = new GetAllEmployeeServiceRequestModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                if (_cache.TryGetValue("GetAllEmployeeServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllEmployeeServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllEmployeeServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllEmployeeServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllEmployeeServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllEmployeeServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllEmployeeServicesSearchBy", out string searchByData);
                                _cache.TryGetValue("GetAllEmployeeServicesSearchByValue", out string searchByValueData);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumn = sortBy;
                                    sortColumnDirection = sortDir;
                                    if (searchByData != null && searchByData != "")
                                        searchBy = searchByData;
                                    if (searchByValueData != null && searchByValueData != "")
                                        searchedValue = searchByValueData;
                                    _cache.Set("GetAllEmployeeServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }
                model = await _serviceFormRepository.GetAllEmployeeServiceRequestTempData(status, skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchedValue);
                filterRecord = model.totalRecords;
                totalRecord = model.totalRecords;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRequests,
                    currentPage = lastAccessedPage,
                    displayData = pageSize,
                    sortColumnData = sortColumn,
                    sortColumnDirectionData = sortColumnDirection,
                    searchByData = searchBy,
                    searchedValueData = searchedValue
                };
                if (lastProcess)
                {
                    _cache.Set("GetAllEmployeeServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllEmployeeServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllEmployeeServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesSearchBy", searchBy, cacheEntryOptions);
                _cache.Set("GetAllEmployeeServicesSearchByValue", searchedValue != null ? searchedValue : "", cacheEntryOptions);
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
        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }
    }
}
