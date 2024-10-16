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
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ServiceFormController : Controller
    {
        private readonly IServiceFormRepository _serviceFormRepository;
        public readonly IPermissionRepository _permissionRepository;

        public ServiceFormController(IServiceFormRepository serviceFormRepository, IPermissionRepository permissionRepository)
        {
            _serviceFormRepository = serviceFormRepository;
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(int ServiceId,int FormId,string service)
        {
            string Module = "";
            if (service == "company")
                Module = "CompanyServices";
            else
                Module = "EmployeeServices";
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, Module);
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, Module);
                }
            }

            ServiceFormFieldModel serviceFormField = new ServiceFormFieldModel();
            List<GetServiceFormFieldModel> formFields = new List<GetServiceFormFieldModel>();
            formFields = _serviceFormRepository.GetServiceFormFields(ServiceId,FormId);

            string _user = GetUserClaims();
            int _UserId = _permissionRepository.GetUserIdForPermission(_user);
            foreach (var data in formFields)
            {
                serviceFormField.FormName = data.FormName;
                break;
            }
            
            serviceFormField.getServiceFormFields = new List<GetServiceFormFieldModel>();
            serviceFormField.getServiceFormFields = formFields;
            serviceFormField.ServiceId = ServiceId;
            serviceFormField.FormId = FormId;
            serviceFormField.FormName = _serviceFormRepository.GetFormNameById(FormId);
            serviceFormField.service = service;
            serviceFormField.allowedModule = allowedModule;
            return View(serviceFormField);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetServiceFormDetails(int ServiceId,bool IsCompanyService,bool IsEmployeeService)
        {
            FormDetail formDetail = new FormDetail();
            formDetail = _serviceFormRepository.GetServiceFormDetails(ServiceId, IsCompanyService, IsEmployeeService);
            return new JsonResult(formDetail);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateForm(string FormName,int FormserviceId)
        {
            int _returnId = 0;

            string _user = GetUserClaims();
            int _UserId = _permissionRepository.GetUserIdForPermission(_user);

            ServiceFormModel serviceForm = new ServiceFormModel();
            serviceForm.FormName = FormName;
            serviceForm.ServiceId = FormserviceId;
            serviceForm.UserId = _UserId;

            _returnId = _serviceFormRepository.CreateForm(serviceForm);

            return RedirectToAction("Index", "ServiceForm", new { ServiceId = FormserviceId , FormId =_returnId});
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddFormFields(SaveFormFieldsModel saveFormFields)
        {
            int _returnId = 0;
            string _user = GetUserClaims();
            int _UserId = _permissionRepository.GetUserIdForPermission(_user);
            saveFormFields.UserId = _UserId;
            _returnId = _serviceFormRepository.CreateFormField(saveFormFields);
            return new JsonResult(_returnId);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetlastStepNumber(int ServiceId,int FormId)
        {
            int _returnId = 0;
            _returnId = _serviceFormRepository.GetlastStepNumber(ServiceId,FormId);
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
