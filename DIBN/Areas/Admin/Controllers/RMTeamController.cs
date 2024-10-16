using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using DIBN.Areas.Admin.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nager.Country;
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
    public class RMTeamController : Controller
    {
        private readonly IRMTeamManagementRepository _rMTeamManagementRepository;
        public readonly ICompanyRepository _companyRepository;
        public readonly IPermissionRepository _permissionRepository;
        public RMTeamController(IRMTeamManagementRepository rMTeamManagementRepository,
            ICompanyRepository companyRepository,
            IPermissionRepository permissionRepository)
        {
            _rMTeamManagementRepository = rMTeamManagementRepository;
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name,string? message)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "RMTeam");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "RMTeam");
                }
            }

            MainRMTeamModel model = new MainRMTeamModel();
            model.Module = name;
            model.message = message;
            model.allowedModule = allowedModule;    
            return View(model);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllRMTeamListWithPagination()
        {
            try
            {
                GetRMTeamListWithPaginationModel model = new GetRMTeamListWithPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _rMTeamManagementRepository.GetRMTeamListWithPagination(pageSize, skip, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalCount;
                totalRecord = model.totalCount;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getRMTeamsList
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
        public IActionResult Create(string name)
        {
            SaveRMTeamModel model = new SaveRMTeamModel();

            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = ""
            });
            for (int i = 0; i < countryList.Count; i++)
            {
                _countries.Add(new SelectListItem
                {
                    Text = countryList[i].CommonName,
                    Value = countryList[i].CommonName.ToString()
                });
            }

            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();
            for (int i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i].CompanyName,
                    Value = company[i].Id.ToString()
                });
            }
            model.companies = companies;
            model.Countries = _countries;
            model.Module = name;
            return View(model);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Creates(SaveRMTeamModel model)
        {
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = ""
            });
            for (int i = 0; i < countryList.Count; i++)
            {
                _countries.Add(new SelectListItem
                {
                    Text = countryList[i].CommonName,
                    Value = countryList[i].CommonName.ToString()
                });
            }

            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();
            for (int i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i].CompanyName,
                    Value = company[i].Id.ToString()
                });
            }
            model.companies = companies;
            model.Countries = _countries;

            if (ModelState.IsValid)
            {
                int _returnId = 0;
                _returnId = await _rMTeamManagementRepository.SaveRMTeamDetails(model);
                if(_returnId > 0)
                {
                    return RedirectToAction("Index", "RMTeam", new {name = model.Module,message = "RM Team Details Saved Successfully."});
                }
                ModelState.AddModelError(model.EmailAddress, "Please provide different email address , currently added email address already exists.!");
            }
            return View("Create", model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Edit(int Id)
        {
            UpdateRMTeamModel model = new UpdateRMTeamModel();
            model = await _rMTeamManagementRepository.GetRMTeamDetails(Id);

            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = ""
            });
            for (int i = 0; i < countryList.Count; i++)
            {
                _countries.Add(new SelectListItem
                {
                    Text = countryList[i].CommonName,
                    Value = countryList[i].CommonName.ToString()
                });
            }

            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();
            for (int i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i].CompanyName,
                    Value = company[i].Id.ToString()
                });
            }
            model.companies = companies;
            model.Countries = _countries;
            return View(model);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Edits(UpdateRMTeamModel model)
        {
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = ""
            });
            for (int i = 0; i < countryList.Count; i++)
            {
                _countries.Add(new SelectListItem
                {
                    Text = countryList[i].CommonName,
                    Value = countryList[i].CommonName.ToString()
                });
            }

            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();
            for (int i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i].CompanyName,
                    Value = company[i].Id.ToString()
                });
            }
            model.companies = companies;
            model.Countries = _countries;
            if (ModelState.IsValid)
            {
                int _returnId = 0;
                _returnId = await _rMTeamManagementRepository.UpdateRMTeamDetails(model);
                if (_returnId > 0)
                {
                    return RedirectToAction("Index", "RMTeam", new { name = "RMTeam", message = "RM Team Details Updated Successfully." });
                }
                ModelState.AddModelError(model.EmailAddress, "Please provide different email address , currently added email address already exists.!");
            }
            return View("Edit", model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Delete(int Id)
        {
            int _returnId = 0;
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            await _rMTeamManagementRepository.Delete(Id, UserId);
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
