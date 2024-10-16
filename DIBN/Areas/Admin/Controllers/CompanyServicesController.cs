using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class CompanyServicesController : Controller
    {
        private readonly ICompanyServiceRepository _companyServiceRepository;
        public readonly IPermissionRepository _permissionRepository;
        public CompanyServicesController(ICompanyServiceRepository companyServiceRepository, IPermissionRepository permissionRepository)
        {
            _companyServiceRepository = companyServiceRepository;
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            try
            {
                List<string> allowedModule = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "CompanyServices");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "CompanyServices");
                    }
                }

                MainCompanyServiceViewModel mainModel = new MainCompanyServiceViewModel();
                List<CompanyServiceViewModel> companyServices = new List<CompanyServiceViewModel>();
                companyServices = _companyServiceRepository.GetAllCompanyServices();
                List<SelectListItem> parentCategories = new List<SelectListItem>();
                parentCategories.Add(new SelectListItem
                {
                    Text = "Select Parent Category",
                    Value = "0"
                });
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
                if (companyServices.Count > 0)
                {
                    foreach (var item in companyServices)
                    {
                        item.CompanyType = companyType;
                        parentCategories.Add(new SelectListItem
                        {
                            Text = item.ServiceName,
                            Value = item.ID.ToString()
                        });
                    }
                }
                else
                {
                    CompanyServiceViewModel model = new CompanyServiceViewModel();
                    model.CompanyType = companyType;
                    model.ParentCategory = parentCategories;
                    companyServices.Add(model);
                }
                mainModel.companyServices = companyServices;
                mainModel.Module = name;
                mainModel.allowedModule = allowedModule;
                return View(mainModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Create(string? name)
        {
            try
            {
                List<CompanyServiceViewModel> companyServices = new List<CompanyServiceViewModel>();
                companyServices = _companyServiceRepository.GetAllParentCompanyServices();
                List<SelectListItem> parentCategories = new List<SelectListItem>();
                parentCategories.Add(new SelectListItem
                {
                    Text = "Select Parent Category",
                    Value = "0"
                });
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
                if (companyServices.Count > 0)
                {
                    foreach (var item in companyServices)
                    {
                        parentCategories.Add(new SelectListItem
                        {
                            Text = item.ServiceName,
                            Value = item.ID.ToString()
                        });
                    }
                }

                CompanyServiceViewModel model = new CompanyServiceViewModel();
                model.Module = name;
                model.ParentCategory = parentCategories;
                model.CompanyType = companyType;
                return PartialView("_Create", model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Creates(CompanyServiceViewModel model)
        {
            try
            {
                int _returnId = 0;
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                model.UserId = _userId;
                _returnId = _companyServiceRepository.CreateNew(model);
                return RedirectToAction("Index", "CompanyServices", new { name = model.Module });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(string? name, int Id)
        {
            try
            {
                List<CompanyServiceViewModel> companyServices = new List<CompanyServiceViewModel>();
                companyServices = _companyServiceRepository.GetAllParentCompanyServices();
                List<SelectListItem> parentCategories = new List<SelectListItem>();
                parentCategories.Add(new SelectListItem
                {
                    Text = "Select Parent Category",
                    Value = "0"
                });
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
                if (companyServices.Count > 0)
                {
                    foreach (var item in companyServices)
                    {
                        parentCategories.Add(new SelectListItem
                        {
                            Text = item.ServiceName,
                            Value = item.ID.ToString()
                        });
                    }
                }
                CompanyServiceViewModel serviceDetails = new CompanyServiceViewModel();
                serviceDetails = _companyServiceRepository.GetCompanyServicesById(Id);
                serviceDetails.Module = name;
                serviceDetails.CompanyType = companyType;
                serviceDetails.ParentCategory = parentCategories;
                return PartialView("_Edit", serviceDetails);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(CompanyServiceViewModel service)
        {
            try
            {
                int _returnId = 0;
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                service.UserId = _userId;
                _returnId = _companyServiceRepository.UpdateCompanyServices(service);
                return RedirectToAction("Index", "CompanyServices", new { name = service.Module });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id)
        {
            try
            {
                int _returnId = 0;
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                _returnId = _companyServiceRepository.DeleteCompanyServices(Id, _userId);
                return new JsonResult(_returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetSerialNumber()
        {
            try
            {
                string _returnId = null;
                _returnId = _companyServiceRepository.GetSerialNumber();
                Regex re = new Regex(@"([a-zA-Z]+)(\W+)(\d+)");
                Match result = re.Match(_returnId);
                string alphaPart = result.Groups[1].Value;
                string periodSign = result.Groups[2].Value;
                int numberPart = Convert.ToInt32(result.Groups[3].Value);
                int Id = numberPart + 1;
                _returnId = alphaPart + periodSign + Id.ToString();
                return new JsonResult(_returnId);
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
