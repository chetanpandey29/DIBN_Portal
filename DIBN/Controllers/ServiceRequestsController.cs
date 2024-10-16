using DIBN.IService;
using DIBN.Models;
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

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ServiceRequestsController : Controller
    {
        private readonly IServicesFormService _servicesFormService;
        private readonly IUserPermissionService _userPermissionService;
        private IMemoryCache _cache;

        public ServiceRequestsController(IServicesFormService servicesFormService, IUserPermissionService userPermissionService, IMemoryCache cache)
        {
            _servicesFormService = servicesFormService;
            _userPermissionService = userPermissionService;
            _cache = cache;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, string? Company, int? Status, int CompanyId)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            GetAllRequests getAllRequests = new GetAllRequests();
            if (name == null)
            {
                name = "MyRequests";
            }
            string role = GetClaims();
            string _companyId = GetActorClaims();

            string _user = GetUserClaims();
            int _userId = _userPermissionService.GetUserIdForPermission(_user);

            int _CompanyId = CompanyId;
            List<string> company = new List<string>();
            if (role.StartsWith("Sales"))
            {
                company = _userPermissionService.GetAllAssignedCompanies(_userId, CompanyId);
                getAllRequests.CurrentCompanyId = CompanyId;
            }
            else if(role == "RM Team")
            {
                company = _userPermissionService.GetAllAssignedCompaniesToRMTeam(_userId, CompanyId);
                getAllRequests.CurrentCompanyId = CompanyId;
            }
            if (CompanyId == 0)
                CompanyId = Convert.ToInt32(_companyId);
            List<GetRequests> GetRequests = new List<GetRequests>();

            List<SelectListItem> companies = new List<SelectListItem>();
            for (var i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i],
                    Value = company[i]
                });
            }
            List<string> allowedModule = new List<string>();

            string User = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(User);
            allowedModule = _userPermissionService.GetUserPermissionName(UserId, "MyRequests");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _userPermissionService.GetCurrentRolePermissionName(Role, "MyRequests");
                }
            }

            getAllRequests.Companies = companies;
            getAllRequests.Module = name;
            getAllRequests.getRequests = GetRequests;
            getAllRequests.Role = role;
            getAllRequests.CompanyId = CompanyId;
            getAllRequests.SendCompanyId = _CompanyId;
            getAllRequests.SelectedStatus = Status;
            getAllRequests.allowedModule = allowedModule;
            return View(getAllRequests);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult TrackServiceRequest(string SerialNumber, string? name,string? serviceRequestType,bool? isCompany)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            List<string> allowedModule = new List<string>();

            string User = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(User);
            allowedModule = _userPermissionService.GetUserPermissionName(UserId, "MyRequests");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _userPermissionService.GetCurrentRolePermissionName(Role, "MyRequests");
                }
            }

            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            GetRequestCompleteDetails getRequest = new GetRequestCompleteDetails();
            getRequest = _servicesFormService.GetRequestDetail(SerialNumber);

            int lastIndex = 0;

            if (getRequest.getRequestResponses.Count > 0)
            {
                lastIndex = getRequest.getRequestResponses.Count - 1;
                getRequest.lastStatus = getRequest.getRequestResponses[lastIndex].Status;
            }
            else
            {
                lastIndex = getRequest.getRequestDetails.Count - 1;
                getRequest.lastStatus = getRequest.getRequestDetails[lastIndex].Status;
            }


            int _responseCount = 0;
            _responseCount = _servicesFormService.GetCountOfResponse(SerialNumber);
            getRequest.CountOfFields = _responseCount;
            getRequest.CompanyId = CompanyId;
            getRequest.Module = name;
            getRequest.serviceRequestType = serviceRequestType;
            getRequest.isCompany = isCompany;
            if (serviceRequestType != null)
            {
                if (isCompany == null)
                {
                    if (serviceRequestType == "Company")
                    {
                        _cache.Set("GetAllAssignedCompanyServicesLastProcess", true, cacheEntryOptions);
                    }
                    else
                    {
                        _cache.Set("GetAllAssignedEmployeeServicesLastProcess", true, cacheEntryOptions);
                    }
                }
                
                if (isCompany != null) 
                {
                    if (isCompany.Value)
                    {
                        if (serviceRequestType == "Company")
                        {
                            _cache.Set("GetAllCMPCompanyServicesLastProcess", true, cacheEntryOptions);
                        }
                        else
                        {
                            _cache.Set("GetAllEMPEmployeeServicesLastProcess", true, cacheEntryOptions);
                        }
                    }
                }
            }
            getRequest.allowedModule = allowedModule;
            return View(getRequest);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult ResponseToService(string SerialNumber, int ServiceId, int FormId, int CompanyId, string RequestedService, string? name, string? serviceRequestType, bool? isCompany)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            GetRequestResponses getRequestResponsesModel = new GetRequestResponses();
            int _returnId = 0;
            _returnId = _servicesFormService.LastStepNumber(SerialNumber);
            string _user = GetUserClaims();
            int _userId = _userPermissionService.GetUserIdForPermission(_user);
            List<SelectListItem> StatusType = new List<SelectListItem>();
            List<ServiceRequestStatusModel> status = new List<ServiceRequestStatusModel>();
            status = _servicesFormService.GetServiceRequestStatus();

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
            getRequestResponsesModel.StatusId = _servicesFormService.GetLatestStatusOfRequest(SerialNumber);
            getRequestResponsesModel.StepId = _returnId;
            getRequestResponsesModel.StatusList = StatusType;
            getRequestResponsesModel.Module = name;
            getRequestResponsesModel.serviceRequestType = serviceRequestType;
            getRequestResponsesModel.isCompany = isCompany;
            return PartialView("_ResponseToService", getRequestResponsesModel);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult ApproveRequest(string SerialNumber, int Status,string? serviceRequestType,bool? isCompany)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            string Username = GetUserClaims(); ;
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            _returnId = _servicesFormService.ChangeStatusOfService(SerialNumber, Status, UserId);
            Log.Information("Approved Service Request " + SerialNumber);
            return RedirectToAction("TrackServiceRequest", "ServiceRequests", new { SerialNumber = SerialNumber, name = "MyRequests",serviceRequestType = serviceRequestType,isCompany = isCompany });
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RejectRequest(GetRequestResponses model)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            _returnId = _servicesFormService.LastStepNumber(model.SerialNumber);
            model.StepId = _returnId;
            string Username = GetUserClaims(); ;
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            model.CreatedBy = UserId;
            _returnId = _servicesFormService.SaveStatusOfService(model);
            Log.Information("Rejected Service Request " + model.Description);
            return RedirectToAction("TrackServiceRequest", "ServiceRequests", new { SerialNumber = model.SerialNumber, name = "MyRequests", serviceRequestType = model.serviceRequestType, isCompany = model.isCompany });
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddServiceResponse(GetRequestResponses getRequestResponsesModel)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            string Username = GetUserClaims(); ;
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            getRequestResponsesModel.CreatedBy = UserId;
            _returnId = _servicesFormService.SaveStatusOfService(getRequestResponsesModel);
            Log.Information("Added New Service Response (" + getRequestResponsesModel.Description + ") for Service number :" + getRequestResponsesModel.SerialNumber);
            return RedirectToAction("TrackServiceRequest", "ServiceRequests", new { SerialNumber = getRequestResponsesModel.SerialNumber, name = getRequestResponsesModel.Module, serviceRequestType = getRequestResponsesModel.serviceRequestType, isCompany=getRequestResponsesModel.isCompany });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetServiceRequestDocument(string SerialNumber, string FileName, int companyId, int serviceId)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            try
            {
                List<DownloadServiceRequestUploadedDocumentCmp> GetRequests = new List<DownloadServiceRequestUploadedDocumentCmp>();
                GetRequests = _servicesFormService.DownloadUploadedServiceRequestDocument(SerialNumber, companyId, FileName, serviceId);
                string contentType = "";
                bool flag = false;
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyServiceRequests(int? status,int? CompanyId, int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

                int _comId = 0, totalRecord = 0,filterRecord = 0,_userId = 0,count = 0, lastAccessedPage = 0;

                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                _comId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                
                if(role.StartsWith("Sales"))
                    _userId = _userPermissionService.GetSalesPersonIdForPermission(_user);
                else
                    _userId = _userPermissionService.GetUserIdForPermission(_user);
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllCMPCompanyServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllCMPCompanyServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllCMPCompanyServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllCMPCompanyServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllCMPCompanyServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllCMPCompanyServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllCMPCompanyServicesSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllCMPCompanyServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }
                model =await _servicesFormService.GetAllCompanyRequestByCompanyId(CompanyId!=null?CompanyId.Value:0, _comId, _userId, role, status, skip, pageSize, sortColumn, sortColumnDirection, searchValue,count);
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
                    _cache.Set("GetAllCMPCompanyServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllCMPCompanyServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllCMPCompanyServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllCMPCompanyServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllCMPCompanyServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllCMPCompanyServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllCMPCompanyServicesSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyServiceRequestsTemp(int? status, int? CompanyId, int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

                int _comId = 0, totalRecord = 0, filterRecord = 0, _userId = 0, count = 0, lastAccessedPage = 0;

                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                _comId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();

                if (role.StartsWith("Sales"))
                    _userId = _userPermissionService.GetSalesPersonIdForPermission(_user);
                else if (role == "RM Team")
                    _userId = _userPermissionService.GetRMTeamIdForPermission(_user);
                else
                    _userId = _userPermissionService.GetUserIdForPermission(_user);
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllCMPCompanyServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllCMPCompanyServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllCMPCompanyServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllCMPCompanyServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllCMPCompanyServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllCMPCompanyServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllCMPCompanyServicesSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllCMPCompanyServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }


                model = await _servicesFormService.GetAllCompanyRequestByCompanyIdTemp(CompanyId != null ? CompanyId.Value : 0, _comId, _userId, role, status, skip, pageSize, sortColumn, sortColumnDirection, searchValue);
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
                    _cache.Set("GetAllCMPCompanyServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllCMPCompanyServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllCMPCompanyServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllCMPCompanyServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllCMPCompanyServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllCMPCompanyServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllCMPCompanyServicesSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllEmployeeServiceRequests(int? status, int? CompanyId, int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);
                int count = 0;
                int _comId = 0, lastAccessedPage = 0;
                GetEmployeeServiceRequestsByCompanyId model = new GetEmployeeServiceRequestsByCompanyId();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                _comId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                if (role.StartsWith("Sales"))
                    _userId = _userPermissionService.GetSalesPersonIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                if (_cache.TryGetValue("GetAllEMPEmployeeServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllEMPEmployeeServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllEMPEmployeeServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllEMPEmployeeServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllEMPEmployeeServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllEMPEmployeeServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllEMPEmployeeServicesSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllEMPEmployeeServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }
                model = await _servicesFormService.GetAllEmployeeRequestByCompanyId(CompanyId != null ? CompanyId.Value : 0, _comId, _userId, role, status,
                    skip, pageSize, sortColumn, sortColumnDirection, searchValue,count);
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
                    _cache.Set("GetAllEMPEmployeeServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllEMPEmployeeServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllEMPEmployeeServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllEMPEmployeeServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllEMPEmployeeServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllEMPEmployeeServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllEMPEmployeeServicesSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllEmployeeServiceRequestsTemp(int? status, int? CompanyId, int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);
                int _comId = 0, lastAccessedPage = 0;
                GetEmployeeServiceRequestsByCompanyId model = new GetEmployeeServiceRequestsByCompanyId();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                _comId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                if (role.StartsWith("Sales"))
                    _userId = _userPermissionService.GetSalesPersonIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllEMPEmployeeServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllEMPEmployeeServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllEMPEmployeeServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllEMPEmployeeServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllEMPEmployeeServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllEMPEmployeeServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllEMPEmployeeServicesSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllEMPEmployeeServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }

                model = await _servicesFormService.GetAllEmployeeRequestByCompanyIdTemp(CompanyId != null ? CompanyId.Value : 0, _comId, _userId, role, status,
                    skip, pageSize, sortColumn, sortColumnDirection, searchValue);
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
                    _cache.Set("GetAllEMPEmployeeServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllEMPEmployeeServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllEMPEmployeeServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllEMPEmployeeServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllEMPEmployeeServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllEMPEmployeeServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllEMPEmployeeServicesSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllAssignedCompanyServices(int? status,int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

                int count = 0, lastAccessedPage = 0;
                GetAssignedCompanyServiceRequests model = new GetAssignedCompanyServiceRequests();
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
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllAssignedCompanyServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllAssignedCompanyServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllAssignedCompanyServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllAssignedCompanyServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllAssignedCompanyServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }

                model =await _servicesFormService.GetAllAssignedCompanyServiceRequest(_userId,status, skip, pageSize, sortColumn, sortColumnDirection, searchValue, count);
                filterRecord =  model.totalRecords;
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
                    _cache.Set("GetAllAssignedCompanyServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllAssignedCompanyServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllAssignedCompanyServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllAssignedCompanyServicesTempData(int? status,string sortColumn,string sortDir, string? searchBy,string? searchString,int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

                int lastAccessedPage = 0;

                GetAssignedCompanyServiceRequests model = new GetAssignedCompanyServiceRequests();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllAssignedCompanyServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllAssignedCompanyServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllAssignedCompanyServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllAssignedCompanyServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSortColumnDirection", out string sortDirc);
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSearchByData", out string searchByData);
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSearchedData", out string searchedData);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortDir = sortDirc;
                                    sortColumn = sortBy;
                                    searchBy = searchByData;
                                    searchString = searchedData;
                                    _cache.Set("GetAllAssignedCompanyServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }

                model = await _servicesFormService.GetAllAssignedCompanyServiceRequestTempData(_userId, status, skip, pageSize, sortColumn, sortDir,searchBy, searchString);
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
                    sortColumnDirectionData = sortDir,
                    searchByData = searchBy,
                    searchedValueData = searchString
                };
                if (lastProcess)
                {
                    _cache.Set("GetAllAssignedCompanyServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllAssignedCompanyServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllAssignedCompanyServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSortColumnDirection", sortDir, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSearchByData", searchBy, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSearchedData", searchString, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllAssignedEmployeeServices(int? status, int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

                int count = 0, lastAccessedPage = 0;

                GetAssignedEmployeeServiceRequests model = new GetAssignedEmployeeServiceRequests();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllAssignedEmployeeServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllAssignedEmployeeServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllAssignedEmployeeServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllAssignedEmployeeServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllAssignedEmployeeServicesSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllAssignedEmployeeServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllAssignedEmployeeServicesSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllAssignedEmployeeServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }

                model = await _servicesFormService.GetAllAssignedEmployeeServiceRequest(_userId, status, skip, pageSize, sortColumn, sortColumnDirection, searchValue,count);
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
                    _cache.Set("GetAllAssignedEmployeeServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllAssignedEmployeeServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllAssignedEmployeeServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllAssignedEmployeeServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllAssignedEmployeeServicesSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllAssignedEmployeeServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllAssignedEmployeeServicesSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllAssignedEmployeeServicesTempData(int? status, string sortColumn, string sortDir, string? searchBy, string? searchString,int page)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromDays(1))
                  .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                  .SetPriority(CacheItemPriority.High)
                  .SetSize(1024);

                int count = 0, lastAccessedPage = 0;

                GetAssignedEmployeeServiceRequests model = new GetAssignedEmployeeServiceRequests();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllAssignedEmployeeServicesLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllAssignedEmployeeServicesStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllAssignedEmployeeServicesLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllAssignedEmployeeServicesLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllAssignedEmployeeServicesSortColumnDirection", out string sortDirc);
                                _cache.TryGetValue("GetAllAssignedEmployeeServicesSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSearchByData", out string searchByData);
                                _cache.TryGetValue("GetAllAssignedCompanyServicesSearchedData", out string searchedData);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortDir = sortDirc;
                                    sortColumn = sortBy;
                                    searchBy = searchByData;
                                    searchString = searchedData;
                                    _cache.Set("GetAllAssignedEmployeeServicesLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }

                model = await _servicesFormService.GetAllAssignedEmployeeServiceRequestTempData(_userId, status, skip, pageSize, sortColumn, sortDir,searchBy, searchString);
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
                    sortColumnDirectionData = sortDir,
                    searchByData = searchBy,
                    searchedValueData = searchString
                };
                if (lastProcess)
                {
                    _cache.Set("GetAllAssignedEmployeeServicesLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllAssignedEmployeeServicesLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllAssignedEmployeeServicesStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllAssignedEmployeeServicesLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllAssignedEmployeeServicesSortColumnDirection", sortDir, cacheEntryOptions);
                _cache.Set("GetAllAssignedEmployeeServicesSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSearchByData", searchBy, cacheEntryOptions);
                _cache.Set("GetAllAssignedCompanyServicesSearchedData", searchString, cacheEntryOptions);
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
        [HttpGet]
        public IActionResult LastStepNumber(string? SerialNumber)
        {
            int _returnId = 0;
            _returnId = _servicesFormService.LastStepNumber(SerialNumber);
            return new JsonResult(_returnId);
        }
    }
}
