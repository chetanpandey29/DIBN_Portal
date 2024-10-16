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
    public class CompanyServiceController : Controller
    {
        private readonly ICompanyServiceList _companyServiceList;
        private readonly IUserPermissionService _userPermissionService;
        private readonly IServicesFormService _servicesFormService;
        public CompanyServiceController(ICompanyServiceList companyServiceList, IUserPermissionService userPermissionService, IServicesFormService servicesFormService)
        {
            _companyServiceList = companyServiceList;
            _userPermissionService = userPermissionService;
            _servicesFormService = servicesFormService;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? message,int CompanyId)
        {
            string Role = GetClaims();
            if(Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            List<string> allowedModule = new List<string>();

            string User = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(User);
            allowedModule = _userPermissionService.GetUserPermissionName(UserId, "CompanyServices");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _userPermissionService.GetCurrentRolePermissionName(Role, "CompanyServices");
                }
            }

            string _companyId = GetActorClaims();
            int _CompanyId = CompanyId;
            if(CompanyId==0)
                CompanyId = Convert.ToInt32(_companyId);
            MainCompanyServicesModel mainModel = new MainCompanyServicesModel();
            List<CompanyServices> companyServices = new List<CompanyServices>();
            companyServices = _companyServiceList.GetAllCompanyService(CompanyId);

            if (companyServices.Count > 0)
            {
                foreach (var item in companyServices)
                {
                    item.Module = name;
                    item.CompanyId = CompanyId;
                    item.SendCompanyId = _CompanyId;
                }
            }
            else
            {
                CompanyServices services = new CompanyServices();
                services.Module = name;
                services.CompanyId = CompanyId;
                services.SendCompanyId = _CompanyId;
                companyServices.Add(services);
            }
            if (message > 0)
            {
                ViewData["Message"] = "Your Request Sended Successfully. You can check Your Request from My Request.";
            }
            else if (message < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to Send your Request. Please try again later..!!";
            }
            mainModel.companyServices = companyServices;
            mainModel.allowedModule = allowedModule;
            return View(mainModel);
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
        public IActionResult GetServiceForm(int ServiceId, int FormId, string? name,string? Value,int? CompanyId,string? serviceName)
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
                serviceFormField.HasMultipleForms = Value!=null && Value!="" ? false:data.HasMultipleForms;
                serviceFormField.CompanyType = data.CompanyType;
                if(CompanyId!=null)
                    serviceFormField.CompanyId = Convert.ToInt32(CompanyId);
                else
                    serviceFormField.CompanyId = 0;
                break;
            }

            serviceFormField.getServiceFormFields = new List<GetServiceFormFieldModel>();
            serviceFormField.getServiceFormFields = formFields;
            serviceFormField.ServiceId = ServiceId;
            serviceFormField.FormId = FormId;
            serviceFormField.Module = name;
            if (CompanyId != null)
                serviceFormField.CompanyId = Convert.ToInt32(CompanyId);
            else
                serviceFormField.CompanyId = 0;
            serviceFormField.serviceName = serviceName;
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
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetLastSerialNumber()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _returnId = null;
            _returnId = _servicesFormService.GetLastSerialNumber();
            return new JsonResult(_returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetServiceId(string CompanyType,string Service,string Value)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _serviceId = 0;
            _serviceId = _companyServiceList.GetServiceId(Service, CompanyType);
            return new JsonResult(_serviceId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult SearchService(string prefix)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            List<string> services = new List<string>();
            services = _companyServiceList.GetServiceListBySearchName(prefix);
            return new JsonResult(services);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult SearchServiceId(string Service)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            List<string> services = new List<string>();
            services = _companyServiceList.GetServiceIdByName(Service);
            return new JsonResult(services);
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
            else if(role == "RM Team")
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
