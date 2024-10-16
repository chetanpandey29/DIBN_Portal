using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nager.Country;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static DIBN.Models.AccountViewModel;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class SalesPersonController : Controller
    {
        private readonly ISalesPersonRepository _salesPersonRepository;
        public readonly ICompanyRepository _companyRepository;
        public readonly IPermissionRepository _permissionRepository;
        public readonly IRoleRepository _roleRepository;
        public readonly IUserRepository _userRepository;

        public SalesPersonController(ISalesPersonRepository salesPersonRepository, ICompanyRepository companyRepository,
            IPermissionRepository permissionRepository, IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _salesPersonRepository = salesPersonRepository;
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "SalesPerson");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "SalesPerson");
                }
            }

            MainSalesPersonViewModel mainModel = new MainSalesPersonViewModel();
            mainModel.Module = name;
            mainModel.allowedModule = allowedModule;
            return View(mainModel);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Create(string? name)
        {
            SalesPersonViewModel user = new SalesPersonViewModel();
            List<SelectListItem> roles = new List<SelectListItem>();
            var role = _roleRepository.GetActiveRoles();
            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();

            foreach (var item in role)
            {
                if (item.RoleName.Contains("Sales Person"))
                {
                    user.Designation = item.RoleID;
                }
            }

            for (int i = 0; i < role.Count; i++)
            {
                roles.Add(new SelectListItem
                {
                    Text = role[i].RoleName,
                    Value = role[i].RoleID.ToString()
                });
            }
            for (int i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i].CompanyName,
                    Value = company[i].Id.ToString()
                });
            }
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
            user.Countries = _countries;
            user.Roles = roles;
            user.Module = name;
            user.companies = companies;
            return View(user);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Creates(SalesPersonViewModel salesPerson)
        {
            try
            {
                string Username = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(Username);
                ICountryProvider countryProvider = new CountryProvider();
                var countryList = countryProvider.GetCountries().ToList();

                List<SelectListItem> _countries = new List<SelectListItem>();
                _countries.Add(new SelectListItem
                {
                    Text = "Select Country",
                    Value = ""
                });
                if (ModelState.IsValid)
                {
                    int returnId = 0;
                    int emailExsitance = 0;
                    emailExsitance = _userRepository.CheckExistanceOfEmail(salesPerson.EmailId);
                    if (emailExsitance > 0)
                    {
                        salesPerson.CreatedBy = UserId;
                        returnId = _salesPersonRepository.CreateNew(salesPerson);
                        Log.Information("Create new Sales Person " + salesPerson.FirstName + " " + salesPerson.LastName);
                        return RedirectToAction("Index", "SalesPerson", new { name = salesPerson.Module });
                    }
                    else
                    {
                        if (emailExsitance == -1)
                            ModelState.AddModelError(salesPerson.EmailId, salesPerson.EmailId + " already Exists.");
                        Log.Error("Failed while Creating new Sales Person " + salesPerson.FirstName + " " + salesPerson.LastName + " as " + salesPerson.EmailId + " is already exists");
                        List<SelectListItem> roles = new List<SelectListItem>();
                        var role = _roleRepository.GetActiveRoles();
                        List<SelectListItem> companies = new List<SelectListItem>();
                        var company = _companyRepository.GetCompanies();

                        for (int i = 0; i < role.Count; i++)
                        {
                            roles.Add(new SelectListItem
                            {
                                Text = role[i].RoleName,
                                Value = role[i].RoleID.ToString()
                            });
                        }
                        for (int i = 0; i < company.Count; i++)
                        {
                            companies.Add(new SelectListItem
                            {
                                Text = company[i].CompanyName,
                                Value = company[i].Id.ToString()
                            });
                        }
                        for (int i = 0; i < countryList.Count; i++)
                        {
                            _countries.Add(new SelectListItem
                            {
                                Text = countryList[i].CommonName,
                                Value = countryList[i].CommonName.ToString()
                            });
                        }
                        salesPerson.Countries = _countries;
                        salesPerson.Roles = roles;
                        salesPerson.companies = companies;
                        return View("Create", salesPerson);
                    }
                }
                else
                {
                    List<SelectListItem> roles = new List<SelectListItem>();
                    var role = _roleRepository.GetActiveRoles();
                    List<SelectListItem> companies = new List<SelectListItem>();
                    var company = _companyRepository.GetCompanies();

                    for (int i = 0; i < role.Count; i++)
                    {
                        roles.Add(new SelectListItem
                        {
                            Text = role[i].RoleName,
                            Value = role[i].RoleID.ToString()
                        });
                    }
                    for (int i = 0; i < company.Count; i++)
                    {
                        companies.Add(new SelectListItem
                        {
                            Text = company[i].CompanyName,
                            Value = company[i].Id.ToString()
                        });
                    }
                    for (int i = 0; i < countryList.Count; i++)
                    {
                        _countries.Add(new SelectListItem
                        {
                            Text = countryList[i].CommonName,
                            Value = countryList[i].CommonName.ToString()
                        });
                    }
                    salesPerson.Countries = _countries;
                    salesPerson.Roles = roles;
                    salesPerson.companies = companies;
                    return View("Create", salesPerson);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(int Id, string? name)
        {
            SalesPersonViewModel salesPerson = new SalesPersonViewModel();
            salesPerson = _salesPersonRepository.GetSalesPersonDetail(Id);
            List<SelectListItem> roles = new List<SelectListItem>();
            var role = _roleRepository.GetActiveRoles();
            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();

            for (int i = 0; i < role.Count; i++)
            {
                roles.Add(new SelectListItem
                {
                    Text = role[i].RoleName,
                    Value = role[i].RoleID.ToString()
                });
            }
            for (int i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i].CompanyName,
                    Value = company[i].Id.ToString()
                });
            }
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
            salesPerson.Countries = _countries;
            salesPerson.Roles = roles;
            salesPerson.Module = name;
            salesPerson.companies = companies;
            return View(salesPerson);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(SalesPersonViewModel salesPerson)
        {
            try
            {
                string Username = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(Username);
                ICountryProvider countryProvider = new CountryProvider();
                var countryList = countryProvider.GetCountries().ToList();

                List<SelectListItem> _countries = new List<SelectListItem>();
                _countries.Add(new SelectListItem
                {
                    Text = "Select Country",
                    Value = ""
                });
                List<SelectListItem> roles = new List<SelectListItem>();
                var role = _roleRepository.GetActiveRoles();
                List<SelectListItem> companies = new List<SelectListItem>();
                var company = _companyRepository.GetCompanies();

                for (int i = 0; i < role.Count; i++)
                {
                    roles.Add(new SelectListItem
                    {
                        Text = role[i].RoleName,
                        Value = role[i].RoleID.ToString()
                    });
                }
                for (int i = 0; i < company.Count; i++)
                {
                    companies.Add(new SelectListItem
                    {
                        Text = company[i].CompanyName,
                        Value = company[i].Id.ToString()
                    });
                }
                for (int i = 0; i < countryList.Count; i++)
                {
                    _countries.Add(new SelectListItem
                    {
                        Text = countryList[i].CommonName,
                        Value = countryList[i].CommonName.ToString()
                    });
                }
                salesPerson.Countries = _countries;
                salesPerson.Roles = roles;
                salesPerson.companies = companies;
                if (ModelState.IsValid)
                {
                    int _returnId = 0;
                    salesPerson.CreatedBy = UserId;
                    _returnId = _salesPersonRepository.Update(salesPerson);
                    if(_returnId > 0)
                    {
                        var loggedIn = GetLoggedUserClaims();
                        if (loggedIn.Contains(salesPerson.EmailId))
                        {
                            var loggedInEmail = loggedIn.Where(x => x == salesPerson.EmailId).ToList();
                            if (loggedInEmail.Count > 0)
                            {
                                if (!loggedInEmail.Contains("_DIBN"))
                                {
                                    var userIdentity = (ClaimsIdentity)User.Identity;
                                    var claims = userIdentity.Claims;
                                    var roleClaimType = userIdentity.RoleClaimType;
                                    var _roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                                    var _users = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
                                    var _actors = claims.Where(c => c.Type == ClaimTypes.Actor).ToList();
                                    for (int i = 0; i < loggedInEmail.Count; i++)
                                    {
                                        //if (!loggedInEmail[i].Contains("DIBN"))
                                        //{
                                        userIdentity.RemoveClaim(_roles.Where(r => r.Value == "Sales Person").FirstOrDefault());
                                        userIdentity.AddClaim(new Claim(ClaimTypes.Role, "Sales Person"));

                                        var principle = new ClaimsPrincipal(new[] { userIdentity });
                                        var loginData = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
                                        //}
                                    }
                                }
                            }
                        }
                        Log.Information("Updating Details of Sales Person " + salesPerson.FirstName + " " + salesPerson.LastName);
                        return RedirectToAction("Index", "SalesPerson", new { name = salesPerson.Module });
                    }
                    else
                    {
                        ModelState.AddModelError(salesPerson.EmailId, "Please provide different email address, currently passed email address already exists.!");
                    }   
                }
                return View("Edit", salesPerson);
                
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
            int _returnId = 0;
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            _returnId = _salesPersonRepository.Delete(Id, UserId);
            return new JsonResult(_returnId);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SalesPersonLogin(int Id)
        {
            GetSalesPersonDetailsForLoginModel salesPerson = new GetSalesPersonDetailsForLoginModel();
            salesPerson = await _salesPersonRepository.GetSalesPersonDetailsForLogin(Id);
            LoginViewModel login1 = new LoginViewModel();
            login1.Email = salesPerson.Email;
            login1.Password = salesPerson.Password;
            Log.Information("Sales Person Login with " + login1.Email + " from Admin Panel.");
            return RedirectToAction("UserLogin", "Account", login1);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult SalesPersonDetails(int Id, string? name)
        {
            SalesPersonViewModel salesPerson = new SalesPersonViewModel();
            salesPerson = _salesPersonRepository.GetSalesPersonDetail(Id);
            salesPerson.Module = name;
            return View(salesPerson);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetAllSalesPersonListWithPagination(int page)
        {
            try
            {
                GetAllSalesPersonsWithPaginationModel model = new GetAllSalesPersonsWithPaginationModel();
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
                model = _salesPersonRepository.GetAllSalesPersonsWithPagination(page, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalSalesPersons;
                totalRecord = model.totalSalesPersons;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.salesPersons
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
            string user = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(user);
            string role = _permissionRepository.GetUserRoleName(userId);
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
        public List<string> GetLoggedUserClaims()
        {
            List<string> UserDetails = new List<string>();
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var userClaimType = userIdentity.NameClaimType;
            var users = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Value != "")
                    UserDetails.Add(users[i].Value);
            }
            return UserDetails;
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
