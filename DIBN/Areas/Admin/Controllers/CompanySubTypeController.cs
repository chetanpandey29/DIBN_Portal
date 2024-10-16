using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using DIBN.Areas.Admin.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CompanySubTypeController : Controller
    {
        public readonly IPermissionRepository _permissionRepository;
        public readonly ICompanySubTypeRepository _companySubTypeRepository;

        public CompanySubTypeController(
            IPermissionRepository permissionRepository, 
            ICompanySubTypeRepository companySubTypeRepository
        )
        {
            _permissionRepository = permissionRepository;
            _companySubTypeRepository = companySubTypeRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index(string? message)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "Company");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "Company");
                }
            }

            List<GetAllCompanySubTypeModel> subTypes = new List<GetAllCompanySubTypeModel>();
            subTypes = await _companySubTypeRepository.GetAllCompanySubType();

            MainCompanySubTypeModel model = new MainCompanySubTypeModel();
            model.allowedModule = allowedModule;
            model.companySubTypes = subTypes;
            model.message = message;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Create()
        {
            try
            {
                SaveCompanySubTypeModel model = new SaveCompanySubTypeModel(); 
                
                List<SelectListItem> companyType = new List<SelectListItem>();
                companyType.Add(new SelectListItem
                {
                    Text = "Select Company Type",
                    Value = ""
                });
                companyType.Add(new SelectListItem
                {
                    Text = "Mainland",
                    Value = "Dubai Mainland"
                });
                companyType.Add(new SelectListItem
                {
                    Text = "Freezone",
                    Value = "Freezone"
                });
                model.MainTypes = companyType;
                return PartialView("_Create", model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Creates(SaveCompanySubTypeModel model)
        {
            try
            {
                string message = "";
                int _returnId = 0;
                if (ModelState.IsValid)
                {
                    string _user = GetUserClaims();
                    int _userId = _permissionRepository.GetUserIdForPermission(_user);
                    model.UserId = _userId;
                    _returnId = await _companySubTypeRepository.SaveCompanyType(model);
                    if (_returnId > 0)
                        message = "Company Type Created Successfully.!";
                    else
                        message = "Please use different sub type, entered sub type already exists.!";
                }
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors).ToList();
                if (errorMessage.Count > 0)
                {
                    foreach (var error in errorMessage)
                    {
                        if (message == "")
                            message = error.ErrorMessage;
                        else
                            message = message + " , " + error.ErrorMessage;
                    }
                }
                return RedirectToAction("Index", "CompanySubType", new { message = message});
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Edit(int Id)
        {
            try
            {
                UpdateCompanySubTypeModel model = new UpdateCompanySubTypeModel();
                model = await _companySubTypeRepository.GetCompanySubTypeDetails(Id);

                List<SelectListItem> companyType = new List<SelectListItem>();
                companyType.Add(new SelectListItem
                {
                    Text = "Select Company Type",
                    Value = ""
                });
                companyType.Add(new SelectListItem
                {
                    Text = "Mainland",
                    Value = "Dubai Mainland"
                });
                companyType.Add(new SelectListItem
                {
                    Text = "Freezone",
                    Value = "Freezone"
                });
                model.MainTypes = companyType;
                return PartialView("_Edit", model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Edits(UpdateCompanySubTypeModel model)
        {
            try
            {
                string message = "";
                int _returnId = 0;
                if (ModelState.IsValid)
                {
                    string _user = GetUserClaims();
                    int _userId = _permissionRepository.GetUserIdForPermission(_user);
                    model.UserId = _userId;
                    _returnId = await _companySubTypeRepository.UpdateCompanyType(model);
                    if (_returnId > 0)
                        message = "Selected Company Type Updated Successfully.!";
                    else
                        message = "Please use different sub type, entered sub type already exists.!";
                }
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors).ToList();
                if (errorMessage.Count > 0)
                {
                    foreach (var error in errorMessage)
                    {
                        if (message == "")
                            message = error.ErrorMessage;
                        else
                            message = message + " , " + error.ErrorMessage;
                    }
                }
                return RedirectToAction("Index", "CompanySubType", new { message = message });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                await _companySubTypeRepository.Delete(Id,_userId);
                return new JsonResult(0);
            }
            catch(Exception ex)
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
