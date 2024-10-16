using DIBN.IService;
using DIBN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class EmployeeServiceController : Controller
    {
        private readonly IEmployeeServiceList _employeeServiceList;
        private readonly IUserPermissionService _userPermissionService;
        private readonly IServicesFormService _servicesFormService;
        public EmployeeServiceController(IEmployeeServiceList employeeServiceList, IUserPermissionService userPermissionService, IServicesFormService servicesFormService)
        {
            _employeeServiceList = employeeServiceList; 
            _userPermissionService = userPermissionService;
            _servicesFormService = servicesFormService;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? message, int CompanyId)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            MainEmployeeServicesModel mainModel = new MainEmployeeServicesModel();
            List<string> allowedModule = new List<string>();

            string User = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(User);
            allowedModule = _userPermissionService.GetUserPermissionName(UserId, "EmployeeServices");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _userPermissionService.GetCurrentRolePermissionName(Role, "EmployeeServices");
                }
            }

            string _companyId = GetActorClaims();
            int _CompanyId = CompanyId;
            if (CompanyId == 0)
                CompanyId = Convert.ToInt32(_companyId);
            List<EmployeeServices> employeeServices = new List<EmployeeServices>();
            employeeServices = _employeeServiceList.GetAllEmployeeService(CompanyId);
            
            if (employeeServices.Count > 0)
            {
                foreach (var item in employeeServices)
                {
                    item.Module = name;
                    item.CompanyId = CompanyId;
                    item.SendCompanyId = _CompanyId;
                }
            }
            else
            {
                EmployeeServices services = new EmployeeServices();
                services.Module = name;
                services.CompanyId = CompanyId;
                services.SendCompanyId = _CompanyId;
                employeeServices.Add(services);
            }
            if (message > 0)
            {
                ViewData["Message"] = "Your Request Sended Successfully. You can check Your Request from My Request.";
            }
            else if (message < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to Send your Request. Please try again later..!!";
            }
            mainModel.employeeService = employeeServices;
            mainModel.allowedModule = allowedModule;
            return View(mainModel);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetServiceId(string CompanyType, string Service, string Value)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _serviceId = 0;
            _serviceId = _employeeServiceList.GetServiceId(Service, CompanyType);
            return new JsonResult(_serviceId);
        }
        [HttpGet]
        [Route("[action]")]

        public IActionResult GetServiceFormDetail(int ServiceId, bool IsCompanyService, bool IsEmployeeService,string Value)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            FormDetail formDetail = new FormDetail();
            formDetail = _servicesFormService.GetServiceFormDetails(ServiceId, IsCompanyService, IsEmployeeService);
            return new JsonResult(formDetail);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetServiceForm(int ServiceId, int FormId, string? name,string? Value, int? CompanyId,string? serviceName)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            ServiceFormFieldModel serviceFormField = new ServiceFormFieldModel();
            List<GetServiceFormFieldModel> formFields = new List<GetServiceFormFieldModel>();
            formFields = _servicesFormService.GetServiceFormFields(ServiceId, FormId);

            string _user = GetUserClaims();
            int _UserId = _userPermissionService.GetUserIdForPermission(_user);
            foreach (var data in formFields)
            {
                serviceFormField.FormName = data.FormName;
                serviceFormField.DisplayName = data.DisplayName;
                serviceFormField.AllowDisplay = data.AllowDisplay;
                serviceFormField.FormConstrains = data.FormConstrains;
                serviceFormField.HasMultipleForms = Value != null && Value != "" ? false : data.HasMultipleForms;
                serviceFormField.CompanyType = data.CompanyType;
                if (CompanyId != null)
                    serviceFormField.CompanyId = Convert.ToInt32(CompanyId);
                else
                    serviceFormField.CompanyId = 0;
                break;
            }

            serviceFormField.getServiceFormFields = new List<GetServiceFormFieldModel>();
            serviceFormField.getServiceFormFields = formFields;
            serviceFormField.ServiceId = ServiceId;
            serviceFormField.FormId = FormId;
            serviceFormField.serviceName = serviceName;
            serviceFormField.Module = name;
            if (CompanyId != null)
                serviceFormField.CompanyId = Convert.ToInt32(CompanyId);
            else
                serviceFormField.CompanyId = 0;
            return View(serviceFormField);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetSerialNumber()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _returnId = null;
            _returnId = _servicesFormService.GetSerialNumber();
            return new JsonResult(_returnId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddFormFieldsValue(GetFieldFormData getFieldFormDatas)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _returnId = null;
            string _company = GetActorClaims();
            int _companyId = 0;

            if (getFieldFormDatas.CompanyId == 0)
                _companyId = Convert.ToInt32(_company);
            else
                _companyId = getFieldFormDatas.CompanyId;

            string _user = GetUserClaims();
            int _userId = _userPermissionService.GetUserIdForPermission(_user);
            string role = GetClaims();
            getFieldFormDatas.CompanyId = _companyId;
            if (role.StartsWith("Sales"))
            {
                getFieldFormDatas.SalesPersonId = _userId;
            }
            else if (role == "RM Team")
            {
                getFieldFormDatas.RMTeamId = _userId;
            }
            else
            {
                getFieldFormDatas.UserId = _userId;
            }
            _returnId = _servicesFormService.SaveServiceFormData(getFieldFormDatas);

            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult ServiceRequest(string? name,int Id,string Service)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            EmployeeServiceRequest employeeServiceRequest = new EmployeeServiceRequest();
            string Username = GetUserClaims();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            employeeServiceRequest.CompanyId = CompanyId;
            employeeServiceRequest.UserId = UserId;
            employeeServiceRequest.Module = name;
            employeeServiceRequest.ServiceName = Service;
            employeeServiceRequest.ServiceId = Id;
            return PartialView("_ServiceRequest", employeeServiceRequest);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SendRequestBody(EmployeeServiceRequest request,List<IFormFile> FormFile)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                int _returnId = 0;
                _returnId = _employeeServiceList.AddEmployeeServiceRequest(request);
                return RedirectToAction("Index", "EmployeeService", new { name = request.Module, message = _returnId });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetServiceRequestDocument(int Id,string? RequestId,string? name)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                EmployeeServiceRequestDocument document = new EmployeeServiceRequestDocument();
                document = _employeeServiceList.GetEmployeeServiceRequestDocumentById(Id);
                string contentType = "";
                string File = document.FileName + document.Extension;
                new FileExtensionContentTypeProvider().TryGetContentType(File, out contentType);
                ViewBag.Base64String = "data:" + contentType + ";base64," + Convert.ToBase64String(document.DataBinary, 0, document.DataBinary.Length);
                ViewBag.type = contentType;
                ViewBag.DocumentId = Id;
                ViewBag.RequestId = RequestId;
                ViewBag.Module = name;
                return View(document);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadDocument(int Id)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                EmployeeServiceRequestDocument document = new EmployeeServiceRequestDocument();
                document = _employeeServiceList.GetEmployeeServiceRequestDocumentById(Id);
                string Files = document.FileName + document.Extension;
                //System.IO.File.WriteAllBytes(Files, document.DataBinary);
                MemoryStream ms = new MemoryStream(document.DataBinary);
                return File(document.DataBinary, System.Net.Mime.MediaTypeNames.Application.Octet, Files);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRequestNumber()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _returnId = null;
            _returnId = _employeeServiceList.GetRequestNumber();
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
    }
}
