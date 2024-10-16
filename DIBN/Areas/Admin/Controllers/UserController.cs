using ClosedXML.Excel;
using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static DIBN.Areas.Admin.Models.PermissionViewModel;
using static DIBN.Areas.Admin.Models.UserViewModel;
using static DIBN.Models.AccountViewModel;
using System.Threading.Tasks;
using Nager.Country;
using Serilog;
using Color = System.Drawing.Color;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

    public class UserController : Controller
    {
        public readonly IUserRepository _userRepository;
        public readonly IRoleRepository _roleRepository;
        public readonly ICompanyRepository _companyRepository;
        public readonly IPermissionRepository _permissionRepository;
        public readonly IModuleRepository _moduleRepository;
        private readonly EncryptionService _encryptionService;
        public UserController(IUserRepository userRepository, IRoleRepository roleRepository,
            ICompanyRepository companyRepository, IPermissionRepository permissionRepository,
            IModuleRepository moduleRepository, EncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;
            _moduleRepository = moduleRepository;
            _encryptionService = encryptionService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? message, int? CompanyId)
        {

            if (CompanyId == null)
            {
                CompanyId = 0;
            }
            List<string> allowedModule = new List<string>();
            List<string> allowedUserPermissionModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "OtherCompanyEmployee");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "OtherCompanyEmployee");
                }
            }
            allowedUserPermissionModule = _permissionRepository.GetUserPermissionName(UserId, "UserPermission");
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedUserPermissionModule.Count == 0)
                {
                    allowedUserPermissionModule = _permissionRepository.GetCurrentRolePermissionName(Role, "UserPermission");
                }
            }
            List<CompanyViewModel> companys = new List<CompanyViewModel>();
            List<SelectListItem> companies = new List<SelectListItem>();
            companys = _companyRepository.GetCompanies();
            for (int i = 0; i < companys.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = companys[i].CompanyName,
                    Value = companys[i].Id.ToString()
                });
            }
            GetCompanyEmployeesModel model = new GetCompanyEmployeesModel();
            model.Module = name;
            model.companies = companies;
            model.CompanyId = (int)CompanyId;
            model.allowedModule = allowedModule;
            model.allowedUserPermissionModule = allowedUserPermissionModule;
            if (message == 0)
            {
                ViewData["Message"] = "User is not Assign to any Company,Please first Assign any Company to this User.";
            }
            Log.Information("Show List of all Employees for other companies");
            return View(model);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult OtherCompanyOwner(string? name, int? message, int? CompanyId)
        {
            if (CompanyId == null)
            {
                CompanyId = 0;
            }

            List<string> allowedModule = new List<string>();
            List<string> allowedUserPermissionModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "OtherCompanyEmployee");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "OtherCompanyEmployee");
                }
            }
            allowedUserPermissionModule = _permissionRepository.GetUserPermissionName(UserId, "UserPermission");
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedUserPermissionModule.Count == 0)
                {
                    allowedUserPermissionModule = _permissionRepository.GetCurrentRolePermissionName(Role, "UserPermission");
                }
            }

            List<CompanyViewModel> companys = new List<CompanyViewModel>();
            List<SelectListItem> companies = new List<SelectListItem>();
            companys = _companyRepository.GetCompanies();
            for (int i = 0; i < companys.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = companys[i].CompanyName,
                    Value = companys[i].Id.ToString()
                });
            }
            GetCompanyEmployeesModel model = new GetCompanyEmployeesModel();
            model.Module = name;
            model.companies = companies;
            model.CompanyId = (int)CompanyId;
            model.allowedModule = allowedModule;
            model.allowedUserPermissionModule = allowedUserPermissionModule;
            if (message == 0)
            {
                ViewData["Message"] = "User is not Assign to any Company,Please first Assign any Company to this User.";
            }
            Log.Information("Show List of all Company Owner/User");
            return View(model);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult AddCompanyForUser(string? name, string? actionMethod)
        {
            SaveCompanyForUser user = new SaveCompanyForUser();
            List<SelectListItem> companyT = new List<SelectListItem>();
            companyT.Add(new SelectListItem
            {
                Text = "Select Company Type",
                Value = ""
            });
            companyT.Add(new SelectListItem
            {
                Text = "Mainland",
                Value = "Dubai Mainland"
            });
            companyT.Add(new SelectListItem
            {
                Text = "Freezone",
                Value = "Freezone"
            });
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
            user.Module = name;
            user.CompanyType = companyT;
            user.Action = actionMethod;
            return View(user);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddCompanyForUsers(SaveCompanyForUser company)
        {
            try
            {
                string Username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(Username);
                ICountryProvider countryProvider = new CountryProvider();
                var countryList = countryProvider.GetCountries().ToList();

                List<SelectListItem> _countries = new List<SelectListItem>();
                if (ModelState.IsValid)
                {
                    int returnId = 0, _companyId = 0;
                    int PrimaryEmailExistance = 1;
                    int AccountNumber = _userRepository.CheckExistanceOfUserAccountNumber(company.AccountNumber,null);
                    if (AccountNumber > 1)
                    {
                        ModelState.AddModelError(company.AccountNumber, AccountNumber + " Already Exists.!");

                        List<SelectListItem> companyT = new List<SelectListItem>();
                        companyT.Add(new SelectListItem
                        {
                            Text = "Select Company Type",
                            Value = ""
                        });
                        companyT.Add(new SelectListItem
                        {
                            Text = "Mainland",
                            Value = "Dubai Mainland"
                        });
                        companyT.Add(new SelectListItem
                        {
                            Text = "Freezone",
                            Value = "Freezone"
                        });
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
                        company.Countries = _countries;
                        company.CompanyType = companyT;
                        return View("AddCompanyForUser", company);
                    }

                    string _existingEmail = "";
                    //returnId = _companyRepository.CheckExistanceOfCompany(company.CompanyName);
                    if (PrimaryEmailExistance > 0) //returnId > 0 &&
                    {
                        if (company.OtherEmailID != null)
                        {
                            if (company.OtherEmailID.Count > 0)
                            {
                                for (int i = 0; i < company.OtherEmailID.Count; i++)
                                {
                                    if (_existingEmail == "")
                                    {
                                        _existingEmail = company.OtherEmailID[i];
                                    }
                                    else
                                    {
                                        _existingEmail = _existingEmail + "," + company.OtherEmailID[i];
                                    }
                                }
                            }
                        }

                        if (_existingEmail != "")
                        {
                            ModelState.AddModelError(company.EmailID, _existingEmail + " already Exists.");
                            List<SelectListItem> companyTy = new List<SelectListItem>();
                            companyTy.Add(new SelectListItem
                            {
                                Text = "Select Company Type",
                                Value = ""
                            });
                            companyTy.Add(new SelectListItem
                            {
                                Text = "Mainland",
                                Value = "Dubai Mainland"
                            });
                            companyTy.Add(new SelectListItem
                            {
                                Text = "Freezone",
                                Value = "Freezone"
                            });
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
                            company.Countries = _countries;
                            company.OtherEmailIdValues = _existingEmail;
                            company.CompanyType = companyTy;
                            return View("AddCompanyForUser", company);
                        }
                        else
                        {
                            string _emails = "", _mobileNumbers = "", _mobileNumberCode = "";
                            if (company.OtherEmailID != null)
                            {
                                if (company.OtherEmailID.Count > 0)
                                {
                                    for (int i = 0; i < company.OtherEmailID.Count; i++)
                                    {
                                        if (_emails == "")
                                        {
                                            _emails = company.EmailID + "," + company.OtherEmailID[i];
                                        }
                                        else
                                        {
                                            _emails = _emails + "," + company.OtherEmailID[i];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _emails = company.EmailID;
                            }
                            if (company.OtherContactNumbers != null && company.OtherContactNumbersCode != null)
                            {
                                if (company.OtherContactNumbers.Count > 0)
                                {
                                    for (int i = 0; i < company.OtherContactNumbers.Count; i++)
                                    {
                                        if (_mobileNumbers == "")
                                        {
                                            _mobileNumbers = company.MobileNumber + "," + company.OtherContactNumbers[i];
                                            _mobileNumberCode = company.MainContactNumberCountry + "," + company.OtherContactNumbersCode[i];
                                        }
                                        else
                                        {
                                            _mobileNumbers = _mobileNumbers + "," + company.OtherContactNumbers[i];
                                            _mobileNumberCode = _mobileNumberCode + "," + company.OtherContactNumbersCode[i];
                                        }
                                    }
                                }

                            }
                            else
                            {
                                _mobileNumbers = company.MobileNumber;
                                _mobileNumberCode = company.MainContactNumberCountry;
                            }
                            company.SecondEmailID = company.EmailID;
                            company.EmailID = _emails;
                            company.MobileNumber = _mobileNumbers;
                            company.MainContactNumberCountry = _mobileNumberCode;
                            Log.Information("Create new Company " + company.CompanyName);
                            company.CreatedBy = userId;
                            returnId = _userRepository.AddNewCompanyForUser(company);

                            /////// Send Email to Company Owner to Activate Company                                     -- Yashasvi TBC (26-11-2022)
                            //if (returnId > 0)
                            //{
                            //    bool _host = HttpContext.Request.IsHttps;
                            //    string host = _host ? "https://" + HttpContext.Request.Host.Value : "http://" + HttpContext.Request.Host.Value;

                            //    string url = Url.Action("ActivateCompany", "Email", new { ActiveId = returnId });
                            //    url = host + url;

                            //    await _companyRepository.sendMail(company.CompanyName, company.SecondEmailID, url);
                            //}
                            return RedirectToAction(company.Action, "User", new { name = company.Module });
                        }
                    }
                }

                int companyId = 0;
                string messages = string.Join("; ", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
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
                company.Countries = _countries;
                company.CompanyType = companyType;
                return View("AddCompanyForUser", company);
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
            SaveNewUser user = new SaveNewUser();
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
            user.Countries = _countries;
            user.Roles = roles;
            user.RoleId = new List<int>();
            user.RoleId.Add(_roleRepository.GetCompanyOwnerId());
            user.Module = name;
            user.Companies = companies;
            return View(user);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Creates(SaveNewUser user)
        {
            try
            {
                string username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(username);
                ICountryProvider countryProvider = new CountryProvider();
                var countryList = countryProvider.GetCountries().ToList();

                List<SelectListItem> _countries = new List<SelectListItem>();

                List<SelectListItem> roleList = new List<SelectListItem>();
                var getRoles = _roleRepository.GetActiveRoles();
                for (int i = 0; i < getRoles.Count; i++)
                {
                    roleList.Add(new SelectListItem
                    {
                        Text = getRoles[i].RoleName,
                        Value = getRoles[i].RoleID.ToString()
                    });
                }

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

                if (ModelState.IsValid)
                {
                    int id = 0;
                    int returnId = 0;
                    int emailExsitance = 1;
                    returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber,null);
                    if (returnId > 0 && emailExsitance > 0)
                    {
                        user.CreatedBy = userId;
                        id = _userRepository.CreateEmployee(user);
                        Log.Information("Create new Employee " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                        return RedirectToAction("OtherCompanyOwner", "User", new { name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");


                        user.Countries = _countries;
                        user.Roles = roleList;
                        user.Companies = companies;
                        user.RoleId = new List<int>();
                        user.RoleId.Add(_roleRepository.GetCompanyOwnerId());
                        return View("Create", user);
                    }

                }
                string messages = string.Join("; ", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));


                user.Countries = _countries;
                user.Roles = roleList;
                user.Companies = companies;
                user.RoleId = new List<int>();
                user.RoleId.Add(_roleRepository.GetCompanyOwnerId());
                return View("Create", user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CreateEmployee(string? name)
        {
            UserViewModel user = new UserViewModel();
            List<SelectListItem> roles = new List<SelectListItem>();
            var role = _roleRepository.GetActiveRoles();
            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();

            for (int i = 0; i < role.Count; i++)
            {
                if (role[i].RoleName != "Company Owner" && role[i].RoleName != "DIBN" && !role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "RM Team")
                {
                    roles.Add(new SelectListItem
                    {
                        Text = role[i].RoleName,
                        Value = role[i].RoleID.ToString()
                    });
                }

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
            user.Companies = companies;
            return View(user);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateNewEmployees(UserViewModel user)
        {
            try
            {
                string username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(username);
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

                List<SelectListItem> roleList = new List<SelectListItem>();
                var getRoles = _roleRepository.GetActiveRoles();
                for (int i = 0; i < getRoles.Count; i++)
                {
                    if (getRoles[i].RoleName != "Company Owner" && getRoles[i].RoleName != "DIBN" && !getRoles[i].RoleName.StartsWith("Sales") && getRoles[i].RoleName != "RM Team")
                    {
                        roleList.Add(new SelectListItem
                        {
                            Text = getRoles[i].RoleName,
                            Value = getRoles[i].RoleID.ToString()
                        });
                    }

                }
                List<SelectListItem> companyList = new List<SelectListItem>();
                var companyDetails = _companyRepository.GetCompanies();
                for (int i = 0; i < companyDetails.Count; i++)
                {
                    companyList.Add(new SelectListItem
                    {
                        Text = companyDetails[i].CompanyName,
                        Value = companyDetails[i].Id.ToString()
                    });
                }


                if (ModelState.IsValid)
                {
                    int id = 0;
                    int returnId = 0;
                    int emailExsitance = 1;
                    returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber,null);
                    if (returnId > 0 && emailExsitance > 0)
                    {
                        user.CreatedBy = userId;
                        id = _userRepository.CreateUser(user);
                        Log.Information("Create new Employee " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                        return RedirectToAction("Index", "User", new { name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");
                        if (emailExsitance == -1)
                            ModelState.AddModelError(user.EmailID, user.EmailID + " already Exists.");

                        user.Countries = _countries;
                        user.Roles = roleList;
                        user.Companies = companyList;
                        return View("CreateEmployee", user);
                    }

                }
                user.Roles = roleList;
                user.Countries = _countries;
                user.Companies = companyList;
                return View("CreateEmployee", user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult UserDetail(int UserId, string? name)
        {
            try
            {
                UserViewModel user = new UserViewModel();
                user = _userRepository.GetUserDetail(UserId);
                Log.Information("Get details of user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                user.Module = name;
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult EditCompanyOwner(int Id, string? name)
        {
            try
            {
                UserViewModel user = new UserViewModel();
                user = _userRepository.GetUserDetail(Id);
                user.Module = name;

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
                user.Countries = _countries;
                user.Module = name;
                user.Companies = companies;
                Log.Information("Get Details for " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")" + " for update details.");
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult EditCompanyOwners(UserViewModel user)
        {
            try
            {
                string username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(username);
                ICountryProvider countryProvider = new CountryProvider();
                var countryList = countryProvider.GetCountries().ToList();

                List<SelectListItem> _countries = new List<SelectListItem>();
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
                List<SelectListItem> roles = new List<SelectListItem>();
                var role = _roleRepository.GetActiveRoles();
                for (int i = 0; i < role.Count; i++)
                {
                    roles.Add(new SelectListItem
                    {
                        Text = role[i].RoleName,
                        Value = role[i].RoleID.ToString()
                    });
                }
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
                if (ModelState.IsValid)
                {
                    int returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber, user.Id);
                    if (returnId > 0)
                    {
                        int id = 0;
                        var password = _encryptionService.EncryptText(user.Password);
                        user.Password = password;
                        user.CreatedBy = userId;
                        id = _userRepository.UpdateUser(user);
                        var loggedIn = GetLoggedUserClaims();
                        if (loggedIn.Contains(user.EmailID) || loggedIn.Contains(user.AccountNumber))
                        {
                            string roleName = _permissionRepository.GetUserRoleName(user.Id);
                            var loggedInEmail = loggedIn.Where(x => x == user.EmailID || x == user.AccountNumber).ToList();
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
                                        userIdentity.RemoveClaim(_roles[i]);
                                        userIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));

                                        var principle = new ClaimsPrincipal(new[] { userIdentity });
                                        var loginData = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
                                        //}
                                    }
                                }
                            }
                        }
                        Log.Information("Change details of user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                        return RedirectToAction("OtherCompanyOwner", "User", new { name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");
                        user.Roles = roles;
                        user.Countries = _countries;
                        user.Companies = companies;
                        Log.Information("Failed while updating details of user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                        return View("EditCompanyOwner", user);
                    }   
                }
                user.Roles = roles;
                user.Countries = _countries;
                user.Companies = companies;
                Log.Information("Failed while updating details of user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                return View("EditCompanyOwner", user);
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
            try
            {
                UserViewModel user = new UserViewModel();
                List<SelectListItem> roles = new List<SelectListItem>();
                var role = _roleRepository.GetActiveRoles();
                List<SelectListItem> companies = new List<SelectListItem>();
                var company = _companyRepository.GetCompanies();

                for (int i = 0; i < role.Count; i++)
                {
                    if (role[i].RoleName != "Company Owner" && role[i].RoleName != "DIBN" && !role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "RM Team")
                    {
                        roles.Add(new SelectListItem
                        {
                            Text = role[i].RoleName,
                            Value = role[i].RoleID.ToString()
                        });
                    }
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
                user = _userRepository.GetUserDetail(Id);
                user.Countries = _countries;
                user.Roles = roles;
                user.Module = name;
                user.Companies = companies;
                Log.Information("Get details of user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ") to Change");
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(UserViewModel user)
        {
            try
            {
                string username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(username);

                ICountryProvider countryProvider = new CountryProvider();
                var countryList = countryProvider.GetCountries().ToList();

                List<SelectListItem> _countries = new List<SelectListItem>();
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
                List<SelectListItem> roles = new List<SelectListItem>();
                var role = _roleRepository.GetActiveRoles();
                for (int i = 0; i < role.Count; i++)
                {
                    if (role[i].RoleName != "Company Owner" && role[i].RoleName != "DIBN" && !role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "RM Team")
                    {
                        roles.Add(new SelectListItem
                        {
                            Text = role[i].RoleName,
                            Value = role[i].RoleID.ToString()
                        });
                    }
                }
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
                if (ModelState.IsValid)
                {
                    int returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber, user.Id);
                    if (returnId > 0)
                    {
                        int id = 0;
                        var password = _encryptionService.EncryptText(user.Password);
                        user.Password = password;
                        user.CreatedBy = userId;
                        id = _userRepository.UpdateUser(user);
                        var loggedIn = GetLoggedUserClaims();
                        if (loggedIn.Contains(user.EmailID) || loggedIn.Contains(user.AccountNumber))
                        {
                            string roleName = _permissionRepository.GetUserRoleName(user.Id);
                            var loggedInEmail = loggedIn.Where(x => x == user.EmailID || x == user.AccountNumber).ToList();
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
                                        userIdentity.RemoveClaim(_roles[i]);
                                        userIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));

                                        var principle = new ClaimsPrincipal(new[] { userIdentity });
                                        var loginData = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
                                        //}
                                    }
                                }
                            }
                        }
                        Log.Information("Change details of user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                        return RedirectToAction(user.Action, "User", new { name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");

                        user.Roles = roles;
                        user.Countries = _countries;
                        user.Companies = companies;
                        if (user.companyOwner == "true")
                        {
                            user.RoleId = new List<int>();
                            user.RoleId.Add(_roleRepository.GetCompanyOwnerId());
                        }
                        Log.Information("Failed while updating details of user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                        return View(user.returnAction, user);
                    }
                }
                user.Roles = roles;
                user.Countries = _countries;
                user.Companies = companies;
                if (user.companyOwner == "true")
                {
                    user.RoleId = new List<int>();
                    user.RoleId.Add(_roleRepository.GetCompanyOwnerId());
                }
                Log.Information("Failed while updating details of user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                return View(user.returnAction, user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id, int mainCompany)
        {
            try
            {
                int returnId = 0;
                //UserViewModel user = new UserViewModel();
                //user = _userRepository.GetUserDetail(Id);
                //Log.Information("Delete user " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                string Username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(Username);
                returnId = _userRepository.DeleteUser(Id, mainCompany, userId);
                return new JsonResult(returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckAccountNumberExistance(string AccountNumber,int? userId)
        {
            int returnId = 0;
            returnId = _userRepository.CheckExistanceOfUserAccountNumber(AccountNumber, userId);
            return new JsonResult(returnId);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult RemoveDocument(int DocumentId)
        {
            int returnId = 0;
            string Username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(Username);
            returnId = _userRepository.RemoveDocument(DocumentId, userId);
            return new JsonResult(returnId);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckEmailExistance(string Email)
        {
            int returnId = 0;
            returnId = _userRepository.CheckExistanceOfEmail(Email);
            return new JsonResult(returnId);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAccountNumber()
        {
            string accountNumber = _userRepository.GetLastAccountNumber();
            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(accountNumber);
            string alphaPart = result.Groups[1].Value;
            int numberPart = Convert.ToInt32(result.Groups[2].Value);
            int Id = numberPart + 1;
            accountNumber = alphaPart + Id.ToString();
            int returnId = _userRepository.CheckExistanceOfUserAccountNumber(accountNumber, null);
            if (returnId == -1)
            {
                Regex res = new Regex(@"([a-zA-Z]+)(\d+)");
                Match results = res.Match(accountNumber);
                string alphaParts = results.Groups[1].Value;
                int numberParts = Convert.ToInt32(results.Groups[2].Value);
                int Ids = numberParts + 2;
                accountNumber = alphaParts + Ids.ToString();
                return new JsonResult(accountNumber);
            }
            return new JsonResult(accountNumber);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult AddUserPermission(int Id, string? name)
        {
            int _companyId = 0;
            List<string> allowedModule = new List<string>();

            string Roles = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "UserPermission");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Roles, "UserPermission");
                }
            }

            List<PermissionViewModel> permissions = new List<PermissionViewModel>();
            permissions = _permissionRepository.GetPermissions();
            List<ModuleViewModel> modules = new List<ModuleViewModel>();
            modules = _moduleRepository.GetModules();
            UserPermissionList userPermission = new UserPermissionList();
            userPermission.Permissions = permissions;
            userPermission.Modules = modules;

            UserViewModel user = new UserViewModel();
            user = _userRepository.GetUserDetail(Id);
            _companyId = user.CompanyId;

            int Role = _permissionRepository.GetUserRole(Id);
            RoleViewModel role = new RoleViewModel();
            role = _roleRepository.GetRoleDetail(Role);
            if (Id != 0 && name != null)
            {
                var getUserPermission = _permissionRepository.GetUserPermissionByUserId(Id);
                var getRolePermission = _permissionRepository.GetRolePermissionByRoleId(Role);
                if (getUserPermission == null || getUserPermission.Count == 0)
                {
                    if (getRolePermission != null)
                    {
                        if (getRolePermission.Count > 0)
                        {
                            for (int index = 0; index < getRolePermission.Count; index++)
                            {
                                GetUserPermissionByUserId model = new GetUserPermissionByUserId();
                                model.UserPermissionId = getRolePermission[index].RolePermissionId;
                                model.PermissionId = getRolePermission[index].PermissionId;
                                model.ModuleId = getRolePermission[index].ModuleId;
                                getUserPermission.Add(model);
                            }
                        }
                    }
                }

                // Remove All Modules from Module list to give User Permission which are only accessable from Admin Panel.
                if (role.RoleName != "DIBN")
                {
                    var module = modules.Find(x => x.ModuleName == "RoleManagement");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "RolePermission");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "CompanyPermission");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "UserPermission");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "Banner");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "MessageTemplate");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "Logs");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "ServiceRequestStatus");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "CompanyDocumentType");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "SalesPerson");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "GeneratePI");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "OtherCompanyEmployee");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "DIBNEmployee");
                    modules.Remove(module);

                    module = modules.Find(x => x.ModuleName == "OtherCompanyOwner");
                    modules.Remove(module);
                }
                else if (role.RoleName == "DIBN")
                {
                    var module = modules.Find(x => x.ModuleName == "Employees");
                    modules.Remove(module);
                }

                userPermission.Id = Id;
                userPermission.Modules = modules;
                userPermission.Permissions = permissions;
                userPermission.getUserPermissionByUserIds = getUserPermission;
                userPermission.getRolePermissionByRoleIds = getRolePermission;
                userPermission.permissionCount = permissions.Count();
                userPermission.UserId = Id;
                userPermission.Role = role.RoleName;
                userPermission.CompanyId = _companyId;
                userPermission.Username = user.FirstName + " " + user.LastName + " ( " + user.AccountNumber + " ) ";
            }
            userPermission.allowedModule = allowedModule;
            Log.Information("Get user permission for " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
            return View(userPermission);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult SaveUserPermission(int[] InsertPermission,
            int[] UpdatePermission,
            int[] ViewPermission,
            int[] DeletePermission,
            int UserId)
        {
            try
            {
                List<string> allowedModule = new List<string>();

                string Roles = GetClaims();
                string User = GetUserClaims();
                int UserIds = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserIds, "UserPermission");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserIds);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Roles, "UserPermission");
                    }
                }
                int returnId = 0;
                if(allowedModule.Contains("Update") && allowedModule.Contains("Delete") && allowedModule.Contains("Insert"))
                {
                    string username = GetUserClaims();
                    int _userId = _permissionRepository.GetUserIdForPermission(username);
                    UserViewModel model = _userRepository.GetUserDetail(UserId);
                    Log.Information("User Permission change for " + model.FirstName + " " + model.LastName + " User Account Number(" + model.AccountNumber + ")");
                    // Store Insert Values for Selected Module

                    var removePermissions = _permissionRepository.DeleteUserPermission(UserId);

                    for (int i = 0; i < InsertPermission.Length; i++)
                    {
                        SaveUserPermission permissions = new SaveUserPermission();
                        permissions.UserId = UserId;
                        permissions.ModuleId = InsertPermission[i];
                        permissions.PermissionId = (int)Permission.Insert;
                        permissions.CreatedBy = _userId;
                        returnId = _permissionRepository.SaveUserPermission(permissions);
                    }
                    for (int i = 0; i < UpdatePermission.Length; i++)
                    {
                        SaveUserPermission permissions = new SaveUserPermission();
                        permissions.UserId = UserId;
                        permissions.ModuleId = UpdatePermission[i];
                        permissions.PermissionId = (int)Permission.Update;
                        permissions.CreatedBy = _userId;
                        returnId = _permissionRepository.SaveUserPermission(permissions);
                    }
                    for (int i = 0; i < ViewPermission.Length; i++)
                    {
                        SaveUserPermission permissions = new SaveUserPermission();
                        permissions.UserId = UserId;
                        permissions.ModuleId = ViewPermission[i];
                        permissions.PermissionId = (int)Permission.Show;
                        permissions.CreatedBy = _userId;
                        returnId = _permissionRepository.SaveUserPermission(permissions);
                    }
                    for (int i = 0; i < DeletePermission.Length; i++)
                    {
                        SaveUserPermission permissions = new SaveUserPermission();
                        permissions.UserId = UserId;
                        permissions.ModuleId = DeletePermission[i];
                        permissions.PermissionId = (int)Permission.Delete;
                        permissions.CreatedBy = _userId;
                        returnId = _permissionRepository.SaveUserPermission(permissions);
                    }
                }
                
                return new JsonResult(returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult RemoveUserPermission(int UserId, int ModuleId, int PermissionId)
        {
            try
            {
                List<string> allowedModule = new List<string>();

                string Roles = GetClaims();
                string User = GetUserClaims();
                int UserIds = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserIds, "UserPermission");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserIds);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Roles, "UserPermission");
                    }
                }
                int returnId = 0;
                if (allowedModule.Contains("Update") && allowedModule.Contains("Delete") && allowedModule.Contains("Insert"))
                {
                    string username = GetUserClaims();
                    int _userId = _permissionRepository.GetUserIdForPermission(username);

                    SaveUserPermission permission = new SaveUserPermission();
                    permission.UserId = UserId;
                    permission.ModuleId = ModuleId;
                    permission.PermissionId = PermissionId;
                    permission.CreatedBy = _userId;
                    returnId = _permissionRepository.RemoveUserPermission(permission);
                    UserViewModel user = new UserViewModel();
                    user = _userRepository.GetUserDetail(UserId);
                    Log.Information("Remove user permission for " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                }
                return new JsonResult(returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserPermissionModule(int Id)
        {
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetUserPermissionModuleByUserId(Id);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserPermissionsName(int Id, string Module)
        {
            string Role = GetUserClaims();

            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetUserPermissionName(Id, Module);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UploadUserDocument(int UserId, int? message, string? name, string? actionName)
        {
            string Module = "";
            if (actionName == "User")
                Module = "OtherCompanyOwner";
            else
                Module = "OtherCompanyEmployee";
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserIds = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserIds, Module);
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, Module);
                }
            }

            int CompanyId = _userRepository.GetCompanyId(UserId);
            if (CompanyId == 0)
            {
                return RedirectToAction("Index", "User", new { name = name, message = CompanyId });
            }
            UploadUserDocuments model = new UploadUserDocuments();
            model.UserId = UserId;
            model.CompanyId = CompanyId;
            model.Module = name;
            model.actionName = actionName;
            model.IsActive = await _userRepository.GetCompanyEmployeeActiveStatus(UserId);
            List<DocumentsViewModel> documents = new List<DocumentsViewModel>();
            documents = _companyRepository.GetAllDocuments(CompanyId, model.UserId);
            model.userDocuments = documents;

            UserViewModel user = new UserViewModel();
            user = _userRepository.GetUserDetail(model.UserId);

            if (model.CompanyId < 0)
            {
                Log.Error("Currently can not Upload new Document for " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ") as this user is not assigned to any Company");
                ViewData["Message"] = "First Assign Any Company for this User to Upload any Documents.";
            }
            if (message != null)
            {
                if (message > 0)
                {
                    ViewData["Message"] = "Document Uploaded Successfully..!!";
                }
                else
                {
                    ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
                }
            }
            model.allowedModule = allowedModule;
            return View(model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadUserDocuments(DocumentsViewModel document)
        {
            int returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            document.CreatedBy = userId;
            returnId = _companyRepository.UploadSelectedFile(document, document.CompanyId);

            UserViewModel user = new UserViewModel();
            user = _userRepository.GetUserDetail(document.UserId);

            if (returnId > 0)
            {
                Log.Information("Upload new Document for " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                ViewData["Message"] = "Document Uploaded Successfully..!!";
            }
            else if (returnId < 0)
            {
                Log.Error("Failed Upload new Document for " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")");
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
            }
            return RedirectToAction("UploadUserDocument", "User", new { UserId = document.UserId, message = returnId, name = document.Module,actionName=document.actionName });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ExportToExcel(int? Id)
        {
            try
            {
                Log.Information("Export Excel File of Other Employees");
                using (XLWorkbook wb = new XLWorkbook())
                {
                    DataTable dt = new DataTable();
                    dt = _userRepository.GetEmployeesForExport(Id).Tables[0];
                    wb.Worksheets.Add(dt);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [Obsolete]
        public IActionResult ExportToPdf(int? Id)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = _userRepository.GetEmployeesForExport(Id).Tables[0];
                byte[] filecontent = exportpdf(dt);
                return File(filecontent, "application/pdf", "Employees.pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ExportToExcelMain()
        {
            try
            {

                Log.Information("Export Excel File of DIBN Employees");
                using (XLWorkbook wb = new XLWorkbook())
                {
                    DataTable dt = new DataTable();
                    dt = _userRepository.GetMainCompanyEmployeesForExport().Tables[0];
                    wb.Worksheets.Add(dt);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DIBN_Employees.xlsx");
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [Obsolete]
        public IActionResult ExportToPdfMain()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = _userRepository.GetMainCompanyEmployeesForExport().Tables[0];
                byte[] filecontent = exportpdf(dt);
                return File(filecontent, "application/pdf", "DIBN_Employees.pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Obsolete]
        private byte[] exportpdf(DataTable dataTable)
        {

            // creating document object  
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.SetMargins(0f,0f,5f,5f);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();
            //Adding paragraph for report generated by  
            Paragraph prgGeneratedBY = new Paragraph();
            prgGeneratedBY.Alignment = Element.ALIGN_RIGHT; 
            doc.Add(prgGeneratedBY);

            //Adding  PdfPTable  
            PdfPTable table = new PdfPTable(dataTable.Columns.Count);
            table.SetWidths(new float[] { 0.8f, 1f, 1f, 1.5f, 1f, 1.5f, 1f, 1f, 0.8f, 0.8f,1f,1f, 0.8f });
            table.HeaderRows = 1;
            table.TotalWidth = 800f;
            table.LockedWidth = true;

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                string cellText = System.Net.WebUtility.HtmlDecode(dataTable.Columns[i].ColumnName);
                PdfPCell cell = new PdfPCell();
                cell.Phrase = new Phrase(cellText, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, 1, new BaseColor(Color.White))); 
                cell.BackgroundColor = new BaseColor(36, 60, 124);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 5;
                table.AddCell(cell);
            }

            //writing table Data  
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    string cellText = System.Net.WebUtility.HtmlDecode(dataTable.Rows[i][j].ToString());
                    PdfPCell cell = new PdfPCell();
                    cell.Phrase = new Phrase(cellText, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, new BaseColor(Color.Black)));
                    cell.BackgroundColor = new BaseColor(Color.White);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 5;

                    table.AddCell(cell);
                }
            }

            doc.Add(table);
            doc.Close();
            Log.Information("Export pdf of DIBN Employees");
            byte[] result = ms.ToArray();
            return result;

        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetMainCompanyEmployees(string? name)
        {
            try
            {
                List<string> allowedModule = new List<string>();
                List<string> allowedUserPermissionModule = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "OtherCompanyEmployee");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "OtherCompanyEmployee");
                    }
                }
                allowedUserPermissionModule = _permissionRepository.GetUserPermissionName(UserId, "UserPermission");
                if (allowedPermission.Count == 0 && allowedUserPermissionModule.Count == 0)
                {
                    if (allowedUserPermissionModule.Count == 0)
                    {
                        allowedUserPermissionModule = _permissionRepository.GetCurrentRolePermissionName(Role, "UserPermission");
                    }
                }

                MainCompanyEmployeesModel mainModel = new MainCompanyEmployeesModel();
                mainModel.Module = name;
                mainModel.allowedModule = allowedModule;
                mainModel.allowedUserPermissionModule = allowedUserPermissionModule;
                Log.Information("Show all Employees of DIBN");
                return View(mainModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult AddNewEmployeeForMainCompany(string? name)
        {
            UserViewModel employees = new UserViewModel();
            string _companyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(_companyId);
            List<SelectListItem> roles = new List<SelectListItem>();
            var role = _roleRepository.GetActiveRoles();
            for (int i = 0; i < role.Count; i++)
            {
                if (role[i].RoleName != "Company Owner" && !role[i].RoleName.StartsWith("Sales"))
                {
                    roles.Add(new SelectListItem
                    {
                        Text = role[i].RoleName,
                        Value = role[i].RoleID.ToString()
                    });
                }
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
            employees.Countries = _countries;
            employees.Roles = roles;
            employees.Module = name;
            employees.CompanyId = CompanyId;
            Log.Information("Creating new Employee for DIBN");
            return View(employees);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddNewEmployeeForMainCompanys(UserViewModel user)
        {
            try
            {
                string username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(username);
                List<SelectListItem> _countries = new List<SelectListItem>();
                List<SelectListItem> roles = new List<SelectListItem>();
                ICountryProvider countryProvider = new CountryProvider();
                var countryList = countryProvider.GetCountries().ToList();
                var role = _roleRepository.GetActiveRoles();
                var roleName = role.Find(x => x.RoleID == user.RoleId.First());
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

                for (int i = 0; i < role.Count; i++)
                {
                    if (!role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "Company Owner")
                    {
                        roles.Add(new SelectListItem
                        {
                            Text = role[i].RoleName,
                            Value = role[i].RoleID.ToString()
                        });
                    }
                }

                if (ModelState.IsValid)
                {
                    int id = 0;
                    int returnId = 0;
                    int emailExsitance = 1;
                    returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber,null);
                    if (returnId > 0 && emailExsitance > 0)
                    {
                        user.CreatedBy = userId;
                        id = _userRepository.CreateUser(user);
                        Log.Information("Create new Employee for DIBN " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")" + " with Designation " + roleName.RoleName);
                        return RedirectToAction("GetMainCompanyEmployees", "User", new { name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");
                        if (emailExsitance == -1)
                            ModelState.AddModelError(user.EmailID, user.EmailID + " already Exists.");


                        user.Countries = _countries;
                        user.Roles = roles;
                        Log.Error("Failed while Creating new Employee for DIBN " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")" + " with Designation " + roleName.RoleName);
                        return View("AddNewEmployeeForMainCompany", user);
                    }

                }
                user.Countries = _countries;
                user.Roles = roles;
                return View("AddNewEmployeeForMainCompany", user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult EditEmployeeofMainCompany(int Id, string? name)
        {
            try
            {
                UserViewModel user = new UserViewModel();
                user = _userRepository.GetUserDetail(Id);
                user.Module = name;
                List<SelectListItem> roles = new List<SelectListItem>();
                var role = _roleRepository.GetActiveRoles();
                for (int i = 0; i < role.Count; i++)
                {
                    if (!role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "Company Owner")
                    {
                        roles.Add(new SelectListItem
                        {
                            Text = role[i].RoleName,
                            Value = role[i].RoleID.ToString()
                        });
                    }
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
                Log.Information("Get Details for " + user.FirstName + " " + user.LastName + "(" + user.AccountNumber + ")" + " for update details.");
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult EditEmployeeofMainCompanys(UserViewModel user)
        {
            try
            {
                string username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(username);
                List<SelectListItem> roles = new List<SelectListItem>();
                var role = _roleRepository.GetActiveRoles();
                for (int i = 0; i < role.Count; i++)
                {
                    if (!role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "Company Owner")
                    {
                        roles.Add(new SelectListItem
                        {
                            Text = role[i].RoleName,
                            Value = role[i].RoleID.ToString()
                        });
                    }
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
                if (ModelState.IsValid)
                {
                    int returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber, user.Id);
                    if (returnId > 0)
                    {
                        int id = 0;
                        var password = _encryptionService.EncryptText(user.Password);
                        user.Password = password;
                        user.CreatedBy = userId;
                        id = _userRepository.UpdateUser(user);
                        var loggedIn = GetLoggedUserClaims();
                        if (loggedIn.Contains(user.EmailID) || loggedIn.Contains(user.AccountNumber))
                        {
                            string roleName = _permissionRepository.GetUserRoleName(user.Id);
                            var loggedInEmail = loggedIn.Where(x => x == user.EmailID || x == user.AccountNumber).ToList();
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
                                        userIdentity.RemoveClaim(_roles[i]);
                                        userIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));

                                        var principle = new ClaimsPrincipal(new[] { userIdentity });
                                        var loginData = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
                                        //}
                                    }
                                }
                            }
                        }

                        Log.Information("Updating Details for User " + user.FirstName + " " + user.LastName + "( " + user.AccountNumber + " )");
                        return RedirectToAction("GetMainCompanyEmployees", "User", new { name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");
                        Log.Error("Faild while Updating Details for User " + user.FirstName + " " + user.LastName + "( " + user.AccountNumber + " )");
                        return View("EditEmployeeofMainCompany", user);
                    }
                }
                
                Log.Error("Faild while Updating Details for User " + user.FirstName + " " + user.LastName + "( " + user.AccountNumber + " )");
                return View("EditEmployeeofMainCompany", user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UploadMainCompanyUserDocument(int UserId, string? name, int? uploaded)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserIds = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserIds, "DIBNEmployee");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserIds);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "DIBNEmployee");
                }
            }
            int CompanyId = _userRepository.GetCompanyId(UserId);
            UploadUserDocuments model = new UploadUserDocuments();
            model.UserId = UserId;
            model.CompanyId = CompanyId;
            model.Module = name;
            model.uploaded = uploaded ?? 0;
            model.allowedModule = allowedModule;
            model.IsActive = await _userRepository.GetCompanyEmployeeActiveStatus(UserId);
            List<DocumentsViewModel> documents = new List<DocumentsViewModel>();
            documents = _companyRepository.GetAllDocuments(CompanyId, model.UserId);
            model.userDocuments = documents;
            return View(model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadMainCompanyUserDocuments(DocumentsViewModel document)
        {
            int returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            document.CreatedBy = userId;
            returnId = _companyRepository.UploadSelectedFile(document, document.CompanyId);

            CompanyViewModel company = new CompanyViewModel();
            company = _companyRepository.GetCompanyById(document.CompanyId);

            UserViewModel user = new UserViewModel();
            user = _userRepository.GetUserDetail(document.UserId);

            if (returnId > 0)
            {
                ViewData["Message"] = "Document Uploaded Successfully..!!";
                Log.Information("New Document Uploaded with Description : " + document.Description + " for User" + user.FirstName + " " + user.LastName);
            }
            else if (returnId < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
                Log.Error("Failed while uploading Document with Description : " + document.Description + " for User" + user.FirstName + " " + user.LastName);
            }

            return RedirectToAction("UploadMainCompanyUserDocument", "User", new { UserId = document.UserId, name = document.Module, uploaded = returnId });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetDetailsOfUser(int UserId)
        {
            UserViewModel user = new UserViewModel();
            user = _userRepository.GetUserDetail(UserId);
            LoginViewModel login1 = new LoginViewModel();
            login1.Email = user.AccountNumber;
            login1.Password = user.Password;
            Log.Information("Get User Details of " + login1.Email + " for Login from Admin Panel.");
            return RedirectToAction("UserLogin", "Account", login1);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyType(int CompanyId)
        {
            string _companyType = null;
            _companyType = _userRepository.GetCompanyType(CompanyId);
            return new JsonResult(_companyType);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyActiveEmployeesWithPagination(int? companyId)
        {
            try
            {
                GetAllActiveEmployeeListWithPaginationModel model = new GetAllActiveEmployeeListWithPaginationModel();
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
                model = await _userRepository.GetAllActiveEmployeesWithPagination(companyId,skip,pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalActiveEmployees;
                totalRecord = model.totalActiveEmployees;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.GetActiveEmployees
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
        public async Task<IActionResult> GetAllCompanyInActiveEmployeesWithPagination(int? companyId)
        {
            try
            {
                GetAllInActiveEmployeeListWithPaginationModel model = new GetAllInActiveEmployeeListWithPaginationModel();
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
                model = await _userRepository.GetAllInActiveEmployeesWithPagination(companyId,skip,pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalInActiveEmployees;
                totalRecord = model.totalInActiveEmployees;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.GetInActiveEmployees
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
        public async Task<IActionResult> GetAllCompanyActiveCompanyOwnerWithPagination(int? companyId)
        {
            try
            {
                GetAllActiveCompanyOwnerListWithPaginationModel model = new GetAllActiveCompanyOwnerListWithPaginationModel();
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
                model = await _userRepository.GetAllActiveCompanyOwnerWithPagination(companyId,skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalActiveCompanyOwners;
                totalRecord = model.totalActiveCompanyOwners;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.GetActiveCompanyOwner
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
        public async Task<IActionResult> GetAllCompanyInActiveCompanyOwnerWithPagination(int? companyId)
        {
            try
            {
                GetAllInActiveCompanyOwnerListWithPaginationModel model = new GetAllInActiveCompanyOwnerListWithPaginationModel();
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
                model = await _userRepository.GetAllInActiveCompanyOwnerWithPagination(companyId,skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalInActiveCompanyOwner;
                totalRecord = model.totalInActiveCompanyOwner;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.GetInActiveCompanyOwner
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
        public async Task<IActionResult> GetAllCompanyActiveMainCompanyEmployeesWithPagination()
        {
            try
            {
                string searchBy = "";
                GetAllActiveMainCompanyEmployeeListWithPaginationModel model = new GetAllActiveMainCompanyEmployeeListWithPaginationModel();
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
                model = await _userRepository.GetAllActiveMainCompanyEmployeesWithPagination(CompanyId, skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalActiveEmployee;
                totalRecord = model.totalActiveEmployee;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.GetActiveEmployee
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
        public async Task<IActionResult> GetAllCompanyActiveMainCompanyEmployeesWithPaginationFilter(string sortColumn, string sortColumnDirection, string? searchBy, string? searchValue)
        {
            try
            {
                GetAllActiveMainCompanyEmployeeListWithPaginationModel model = new GetAllActiveMainCompanyEmployeeListWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _userRepository.GetAllActiveMainCompanyEmployeesWithPagination(CompanyId, skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalActiveEmployee;
                totalRecord = model.totalActiveEmployee;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.GetActiveEmployee
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
        public async Task<IActionResult> GetAllCompanyInActiveMainCompanyEmployeesWithPagination()
        {
            try
            {
                string searchBy = "";
                GetAlInlActiveMainCompanyEmployeeListWithPaginationModel model = new GetAlInlActiveMainCompanyEmployeeListWithPaginationModel();
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
                model =await _userRepository.GetAllInActiveMainCompanyEmployeesWithPagination(CompanyId, skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalInActiveEmployee;
                totalRecord = model.totalInActiveEmployee;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.GetInActiveEmployee
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
        public async Task<IActionResult> GetAllCompanyInActiveMainCompanyEmployeesWithPaginationFilter(string sortColumn, string sortColumnDirection, string? searchBy, string? searchValue)
        {
            try
            {
                GetAlInlActiveMainCompanyEmployeeListWithPaginationModel model = new GetAlInlActiveMainCompanyEmployeeListWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _userRepository.GetAllInActiveMainCompanyEmployeesWithPagination(CompanyId, skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalInActiveEmployee;
                totalRecord = model.totalInActiveEmployee;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.GetInActiveEmployee
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
