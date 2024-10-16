using DIBN.Areas.Admin.Data;
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
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Models.CompanyViewModel;
using static DIBN.Areas.Admin.Models.PermissionViewModel;
using static DIBN.Areas.Admin.Models.UserViewModel;
using static DIBN.Models.AccountViewModel;
using System.Data;
using ClosedXML.Excel;
using Microsoft.Extensions.Caching.Memory;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class CompanyController : Controller
    {
        public readonly ICompanyRepository _companyRepository;
        public readonly IUserRepository _userRepository;
        public readonly IRoleRepository _roleRepository;
        public readonly IPermissionRepository _permissionRepository;
        public readonly IModuleRepository _moduleRepository;
        public readonly ICompanyDocumentTypeRepository _companyDocumentTypeRepository;
        private readonly EncryptionService _encryptionService;
        private readonly ICompanyInvoiceRepository _companyInvoiceRepository;
        private readonly IServiceFormRepository _serviceFormRepository;
        private readonly ISupportTicketRepository _supportTicketRepository;
        private readonly ISalesPersonRepository _salesPersonRepository;
        private IMemoryCache _cache;
        public CompanyController(
            ICompanyRepository companyRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IModuleRepository moduleRepository,
            EncryptionService encryptionService,
            ICompanyDocumentTypeRepository companyDocumentTypeRepository,
            ICompanyInvoiceRepository companyInvoiceRepository,
            IServiceFormRepository serviceFormRepository,
            ISupportTicketRepository supportTicketRepository,
            ISalesPersonRepository salesPersonRepository,
            IMemoryCache cache
            )
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _moduleRepository = moduleRepository;
            _encryptionService = encryptionService;
            _companyDocumentTypeRepository = companyDocumentTypeRepository;
            _companyInvoiceRepository = companyInvoiceRepository;
            _serviceFormRepository = serviceFormRepository;
            _supportTicketRepository = supportTicketRepository;
            _salesPersonRepository = salesPersonRepository;
            _cache = cache;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? returnId)
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

            if (allowedPermission.Count == 0)
            {
                allowedPermission = _permissionRepository.GetRolePermissionedModuleName(Role);
            }

            GetCompaniesWithMainCompany companies = new GetCompaniesWithMainCompany();

            companies.Module = name;
            if (returnId != null)
            {
                ViewData["Message"] = "Email Send Successfully.";
            }
            companies.allowedModule = allowedModule;
            companies.allowedPermission = allowedPermission;
            return View(companies);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Create(string? name)
        {
            int _companyId = 0;
            List<SelectListItem> users = new List<SelectListItem>();
            var userList = _userRepository.GetUserListForCompany(_companyId);

            users.Add(new SelectListItem
            {
                Text = "Select Owner of this Company",
                Value = "0"
            });
            for (int i = 0; i < userList.Count; i++)
            {
                users.Add(new SelectListItem
                {
                    Text = userList[i].Username + " (" + userList[i].AccountNumber + ")",
                    Value = userList[i].Id.ToString()
                });
            }

            List<SelectListItem> salesPersons = new List<SelectListItem>();
            var salesPersonsList = _salesPersonRepository.GetAllSalesPersons();

            for (int i = 0; i < salesPersonsList.Count; i++)
            {
                salesPersons.Add(new SelectListItem
                {
                    Text = salesPersonsList[i].FirstName +" "+ salesPersonsList[i].LastName + " (" + salesPersonsList[i].EmailId + ")",
                    Value = salesPersonsList[i].Id.ToString()
                });
            }

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
            DateTime date = DateTime.Now;

            SaveNewCompany saveCompany = new SaveNewCompany();
            saveCompany.Countries = _countries;
            saveCompany.Users = users;
            saveCompany.SalesPersons = salesPersons;
            saveCompany.Module = name;
            saveCompany.CompanyStartingDate = date.ToString("yyyy-MM-dd");
            saveCompany.CompanyType = companyType;

            return View(saveCompany);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Creates(SaveNewCompany company)
        {
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            List<SelectListItem> salesPersons = new List<SelectListItem>();
            var salesPersonsList = _salesPersonRepository.GetAllSalesPersons();

            for (int i = 0; i < salesPersonsList.Count; i++)
            {
                salesPersons.Add(new SelectListItem
                {
                    Text = salesPersonsList[i].FirstName + " " + salesPersonsList[i].LastName + " (" + salesPersonsList[i].EmailId + ")",
                    Value = salesPersonsList[i].Id.ToString()
                });
            }

            if (ModelState.IsValid)
            {
                int returnId = 0, _companyId = 0;
                int PrimaryEmailExistance = 1;
                int AccountNumber = _companyRepository.CheckExistanceOfCompanyAccountNumber(company.AccountNumber);

                if (AccountNumber < 1)
                {
                    Log.Error(company.AccountNumber + " Account number already exists ");
                    ViewBag.ErrorMessage = company.AccountNumber + " Account number already exists.";
                    ModelState.AddModelError(company.AccountNumber, AccountNumber + " Already Exists.!");
                    List<SelectListItem> user = new List<SelectListItem>();
                    var userLists = _userRepository.GetUserListForCompany(_companyId);
                    user.Add(new SelectListItem
                    {
                        Text = "Select Owner of this Company",
                        Value = ""
                    });
                    for (int i = 0; i < userLists.Count; i++)
                    {
                        user.Add(new SelectListItem
                        {
                            Text = userLists[i].Username + " (" + userLists[i].AccountNumber + ")",
                            Value = userLists[i].Id.ToString()
                        });
                    }
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
                    company.Users = user;
                    company.CompanyType = companyT;
                    company.AccountNumber = company.AccountNumber;
                    company.SalesPersons = salesPersons;
                    return View("Create", company);
                }
                if (PrimaryEmailExistance > 0)
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
                    company.CreatedBy = UserId;
                    returnId = _companyRepository.AddNewCompany(company);
                    Log.Information("Added New Company " + company.CompanyName);
                    return RedirectToAction("Index", "Company", new { name = company.Module });

                }
            }


            int companyId = 0;
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            List<SelectListItem> users = new List<SelectListItem>();
            var userList = _userRepository.GetUserListForCompany(companyId);
            users.Add(new SelectListItem
            {
                Text = "Select Owner of this Company",
                Value = "0"
            });
            for (int i = 0; i < userList.Count; i++)
            {
                users.Add(new SelectListItem
                {
                    Text = userList[i].Username,
                    Value = userList[i].Id.ToString()
                });
            }
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
            company.SalesPersons = salesPersons;
            company.Countries = _countries;
            company.Users = users;
            company.CompanyType = companyType;
            return View("Create", company);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(int Id, string? name)
        {
            SaveCompany companyDetails = new SaveCompany();
            companyDetails = _companyRepository.GetCompanyDetails(Id);

            List<SelectListItem> users = new List<SelectListItem>();
            var userList = _userRepository.GetAssignedUserListForCompany(Id);
            if (userList.Count == 0)
            {
                userList = _userRepository.GetUserListForCompany(Id);
            }
            users.Add(new SelectListItem
            {
                Text = "Select Owner of this Company",
                Value = "0"
            });
            for (int i = 0; i < userList.Count; i++)
            {
                users.Add(new SelectListItem
                {
                    Text = userList[i].Username + " (" + userList[i].AccountNumber + ")",
                    Value = userList[i].Id.ToString()
                });
            }
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
            List<SelectListItem> salesPersons = new List<SelectListItem>();
            var salesPersonsList = _salesPersonRepository.GetAllSalesPersons();

            for (int i = 0; i < salesPersonsList.Count; i++)
            {
                salesPersons.Add(new SelectListItem
                {
                    Text = salesPersonsList[i].FirstName + " " + salesPersonsList[i].LastName + " (" + salesPersonsList[i].EmailId + ")",
                    Value = salesPersonsList[i].Id.ToString()
                });
            }
            companyDetails.Users = users;
            companyDetails.Module = name;
            companyDetails.Countries = _countries;
            companyDetails.CompanyType = companyType;
            companyDetails.SalesPersons = salesPersons;
            return View(companyDetails);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(SaveCompany company)
        {
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            List<SelectListItem> salesPersons = new List<SelectListItem>();
            var salesPersonsList = _salesPersonRepository.GetAllSalesPersons();

            for (int i = 0; i < salesPersonsList.Count; i++)
            {
                salesPersons.Add(new SelectListItem
                {
                    Text = salesPersonsList[i].FirstName + " " + salesPersonsList[i].LastName + " (" + salesPersonsList[i].EmailId + ")",
                    Value = salesPersonsList[i].Id.ToString()
                });
            }
            if (ModelState.IsValid)
            {
                int returnId = 0;
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
                company.CreatedBy = UserId;
                returnId = _companyRepository.UpdateCompanyDetails(company);
                if (company.OldPassword != company.CompanyPassword)
                {
                    Log.Information("Change Password of " + company.CompanyName + " From " + company.OldPassword + " To " + company.CompanyPassword);
                    //await _companyRepository.SendChangePasswordMail(company.CompanyName, company.OldPassword, company.CompanyPassword, company.EmailID);
                }
                Log.Information("Change Details of " + company.CompanyName);

                return RedirectToAction("Index", "Company", new { name = company.Module });
            }
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            List<SelectListItem> users = new List<SelectListItem>();
            var userList = _userRepository.GetAssignedUserListForCompany(company.Id);
            if (userList.Count == 0)
            {
                userList = _userRepository.GetUserListForCompany(company.Id);
            }
            users.Add(new SelectListItem
            {
                Text = "Select Owner of this Company",
                Value = ""
            });
            for (int i = 0; i < userList.Count; i++)
            {
                users.Add(new SelectListItem
                {
                    Text = userList[i].Username + " (" + userList[i].AccountNumber + ")",
                    Value = userList[i].Id.ToString()
                });
            }
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
            company.Users = users;
            company.CompanyType = companyType;
            company.SalesPersons = salesPersons;
            return View("Edit", company);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult AddUserForCompany(string? name, string? actionMethod)
        {
            SaveNewUser user = new SaveNewUser();
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
            user.Action = actionMethod;
            return View(user);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddUserForCompanys(SaveNewUser user)
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
            if (ModelState.IsValid)
            {
                int returnId = 0;
                returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber,null);

                if (returnId > 0)
                {
                    user.CreatedBy = userId;
                    returnId = _userRepository.CreateEmployee(user);
                    return RedirectToAction(user.Action, "Company", new { name = user.Module });
                }
                else
                {
                    if (returnId == -1)
                        ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");


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
                    for (int i = 0; i < countryList.Count; i++)
                    {
                        _countries.Add(new SelectListItem
                        {
                            Text = countryList[i].CommonName,
                            Value = countryList[i].CommonName.ToString()
                        });
                    }
                    user.Countries = _countries;
                    user.Roles = roleList;
                    user.MCountry = user.MCountry;
                    return View("AddUserForCompany", user);
                }
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
            user.MCountry = user.MCountry;
            return View("AddUserForCompany", user);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyDetails(int Id, string? name)
        {
            CompanyViewModel company = new CompanyViewModel();
            company = _companyRepository.GetCompanyById(Id);
            company.Module = name;
            return View(company);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAccountNumber(int UserId)
        {
            List<string> AccountNumber = new List<string>();
            AccountNumber = _companyRepository.GetAccountNumber(UserId);
            return new JsonResult(AccountNumber);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckCompanyAssociation(int Id)
        {
            int returnId = 0;
            CompanyAssociationData data = new CompanyAssociationData();
            CompanyViewModel model = new CompanyViewModel();
            model = _companyRepository.GetCompanyById(Id);
            data = _companyRepository.CheckCompanyAssociation(Id);
            if (data.Users > 1 || data.Shareholders > 0 || data.Documents > 0)
            {
                Log.Error(model.CompanyName + "can not deleted as Multiple Employees/Documents are associated with it.");
                returnId = -1;
            }
            if (data.Users == 1)
            {
                Log.Warning(model.CompanyName + " has one user associated with it. If User delete this company then that user is also deleted with it.");
                returnId = -2;
            }
            return new JsonResult(returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id)
        {
            int returnId = 0;
            CompanyViewModel model = new CompanyViewModel();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            model = _companyRepository.GetCompanyById(Id);
            returnId = _companyRepository.DeleteCompany(Id,UserId);
            Log.Information(model.CompanyName + " was Deleted.");
            return new JsonResult(returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckCompanyExistance(string name)
        {
            int returnId = 0;
            returnId = _companyRepository.CheckExistanceOfCompany(name);
            return new JsonResult(returnId);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckEmailExistance(string Email)
        {
            int returnId = 0;
            returnId = _companyRepository.CheckExistanceOfEmail(Email);
            return new JsonResult(returnId);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadUserDocument(DocumentsViewModel document)
        {
            int returnId = 0;
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            document.CreatedBy = UserId;
            returnId = _companyRepository.UploadSelectedFile(document, document.CompanyId);
            if (returnId > 0)
            {
                ViewData["Message"] = "Document Uploaded Successfully..!!";
            }
            else if (returnId < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
            }
            return RedirectToAction("UserDocuments", "Company", new { Id = document.CompanyId, name = document.Module, message = returnId });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadSelectedDocument(int Id, int CompanyId)
        {
            DocumentsViewModel document = new DocumentsViewModel();
            document = _companyRepository.DownloadDocument(Id, CompanyId);
            string Files = document.FileName;
            //System.IO.File.WriteAllBytes(Files, document.Data);
            MemoryStream ms = new MemoryStream(document.Data);
            return File(document.Data, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName + document.Extension);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult UserDocuments(int Id, string? name, int? message)
        {
            List<DocumentsViewModel> documents = new List<DocumentsViewModel>();
            documents = _companyRepository.GetAllDocuments(Id, 0);
            foreach (var document in documents)
            {
                document.Module = name;
                document.CompanyId = Id;
            }
            if (message > 0)
            {
                ViewData["Message"] = "Document Uploaded Successfully..!!";
            }
            else if (message < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
            }
            return View(documents);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CompanyDocuments(int Id, string? name, int? Message)
        {
            List<DocumentsViewModel> documents = new List<DocumentsViewModel>();
            documents = _companyRepository.GetCompanyDocuments(Id);
            List<CompanyDocumentTypeModel> documentType = new List<CompanyDocumentTypeModel>();
            documentType = _companyDocumentTypeRepository.GetCompanyDocuments();
            List<SelectListItem> documentTypes = new List<SelectListItem>();
            for (int i = 0; i < documentType.Count; i++)
            {
                documentTypes.Add(new SelectListItem
                {
                    Text = documentType[i].DocumentName,
                    Value = documentType[i].ID.ToString()
                });
            }

            CompanyViewModel companyViewModel = new CompanyViewModel();
            companyViewModel = _companyRepository.GetCompanyById(Id);

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

            GetCompanyDocuments companyDocuments = new GetCompanyDocuments();
            companyDocuments.CompanyId = Id;
            companyDocuments.Company = companyViewModel.CompanyName;
            companyDocuments.CompanyDocuments = documents;
            companyDocuments.Module = name;
            companyDocuments.allowedModule = allowedModule;
            companyDocuments.DocumentTypes = documentTypes;
            if (Message > 0)
            {
                if (Message == 1)
                {
                    ViewData["Message"] = "Document Uploaded Successfully..!!";
                }
                else if (Message == 2)
                {
                    ViewData["Message"] = "Please fill all required fields before uploading any Company Documents";
                }
            }
            else if (Message < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
            }
            return View(companyDocuments);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadCompanyDocuments(GetCompanyDocuments documents)
        {
            try
            {
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                List<CompanyDocumentTypeModel> documentType = new List<CompanyDocumentTypeModel>();
                documentType = _companyDocumentTypeRepository.GetCompanyDocuments();
                List<SelectListItem> documentTypes = new List<SelectListItem>();
                for (int i = 0; i < documentType.Count; i++)
                {
                    documentTypes.Add(new SelectListItem
                    {
                        Text = documentType[i].DocumentName,
                        Value = documentType[i].ID.ToString()
                    });
                }
                if (ModelState.IsValid)
                {
                    CompanyViewModel model = new CompanyViewModel();
                    documents.CreatedBy = UserId;
                    model = _companyRepository.GetCompanyById(documents.CompanyId);
                    int returnId = 0;
                    returnId = _companyRepository.UploadCompanyDocuments(documents);
                    Log.Information("New Company Document was uploaded for " + model.CompanyName);
                    return RedirectToAction("CompanyDocuments", "Company", new { Id = documents.CompanyId, name = documents.Module, Message = 1 });
                }
                else
                {
                    ModelState.AddModelError(documents.SelectedDocumentType.ToString(), "Please Select Document Type");
                    documents.DocumentTypes = documentTypes;
                    return RedirectToAction("CompanyDocuments", "Company", new { Id = documents.CompanyId, name = documents.Module, Message = 2 });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadCompanyDocuments(int ID, int CompanyId)
        {
            try
            {
                GetCompanyDocuments document = new GetCompanyDocuments();
                document = _companyRepository.DownloadCompanyDocuments(ID, CompanyId);
                string Files = document.FileName;
                //System.IO.File.WriteAllBytes(Files, document.Data);
                MemoryStream ms = new MemoryStream(document.Data);
                return File(document.Data, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName + document.Extension);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult RemoveCompanyDocument(int ID)
        {
            try
            {
                int _returnId = 0;
                string username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(username);
                _returnId = _companyRepository.RemoveCompanyDocuments(ID, userId);
                return new JsonResult(_returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetSelectedCompanyDetails(int CompanyId)
        {
            try
            {
                List<string> details = new List<string>();
                details = _companyRepository.GetSelectedCompanyDetails(CompanyId);
                LoginViewModel login1 = new LoginViewModel();
                login1.Email = details[0];
                login1.Password = details[1];
                return RedirectToAction("UserLogin", "Account", login1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult AddCompanyPermission(int Id, string? name)
        {
            List<PermissionViewModel> permissions = new List<PermissionViewModel>();
            permissions = _permissionRepository.GetPermissions();
            List<ModuleViewModel> modules = new List<ModuleViewModel>();
            modules = _moduleRepository.GetModules();
            CompanyPermissionList companyPermission = new CompanyPermissionList();
            companyPermission.Permissions = permissions;
            companyPermission.Modules = modules;
            if (Id != 0 && name != null)
            {
                var getCompanyPermission = _permissionRepository.GetCompanyPermissionByCompanyId(Id);
                companyPermission.Id = Id;
                companyPermission.Modules = modules;
                companyPermission.Permissions = permissions;
                companyPermission.getCompanyPermissionByCompanyIds = getCompanyPermission;
                companyPermission.permissionCount = permissions.Count();
                companyPermission.CompanyId = Id;
            }
            return View(companyPermission);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult SaveCompanyPermission(int[] InsertPermission,
            int[] UpdatePermission,
            int[] ViewPermission,
            int[] DeletePermission,
            int CompanyId)
        {
            try
            {
                int returnId = 0;
                // Store Insert Values for Selected Module

                for (int i = 0; i < InsertPermission.Length; i++)
                {
                    SaveCompanyPermission permissions = new SaveCompanyPermission();
                    permissions.CompanyId = CompanyId;
                    permissions.ModuleId = InsertPermission[i];
                    permissions.PermissionId = (int)Permission.Insert;
                    returnId = _permissionRepository.SaveCompanyPermission(permissions);
                }
                for (int i = 0; i < UpdatePermission.Length; i++)
                {
                    SaveCompanyPermission permissions = new SaveCompanyPermission();
                    permissions.CompanyId = CompanyId;
                    permissions.ModuleId = UpdatePermission[i];
                    permissions.PermissionId = (int)Permission.Update;
                    returnId = _permissionRepository.SaveCompanyPermission(permissions);
                }
                for (int i = 0; i < ViewPermission.Length; i++)
                {
                    SaveCompanyPermission permissions = new SaveCompanyPermission();
                    permissions.CompanyId = CompanyId;
                    permissions.ModuleId = ViewPermission[i];
                    permissions.PermissionId = (int)Permission.Show;
                    returnId = _permissionRepository.SaveCompanyPermission(permissions);
                }
                for (int i = 0; i < DeletePermission.Length; i++)
                {
                    SaveCompanyPermission permissions = new SaveCompanyPermission();
                    permissions.CompanyId = CompanyId;
                    permissions.ModuleId = DeletePermission[i];
                    permissions.PermissionId = (int)Permission.Delete;
                    returnId = _permissionRepository.SaveCompanyPermission(permissions);
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
        public IActionResult RemoveCompanyPermission(int CompanyId, int ModuleId, int PermissionId)
        {
            try
            {
                int returnId = 0;
                SaveCompanyPermission permission = new SaveCompanyPermission();
                permission.CompanyId = CompanyId;
                permission.ModuleId = ModuleId;
                permission.PermissionId = PermissionId;
                returnId = _permissionRepository.RemoveCompanyPermission(permission);
                return new JsonResult(returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyEmployees(int Id, string? name)
        {
            try
            {
                List<string> allowedModule = new List<string>();

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

                CompanyViewModel companyViewModel = new CompanyViewModel();
                companyViewModel = _companyRepository.GetCompanyById(Id);

                GetCompanyEmployeesModel model = new GetCompanyEmployeesModel();
                model.Module = name;
                model.CompanyId = Id;
                model.Company = companyViewModel.CompanyName;
                model.allowedModule = allowedModule;
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyUsers(int Id, string? name)
        {
            try
            {
                List<string> allowedModule = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "OtherCompanyOwner");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "OtherCompanyOwner");
                    }
                }

                CompanyViewModel companyViewModel = new CompanyViewModel();
                companyViewModel = _companyRepository.GetCompanyById(Id);

                GetCompanyEmployeesModel model = new GetCompanyEmployeesModel();
                model.Module = name;
                model.CompanyId = Id;
                model.Company = companyViewModel.CompanyName;
                model.allowedModule = allowedModule;
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CreateNewEmployeeForCompany(int Id, string? name)
        {
            try
            {
                SaveUser user = new SaveUser();
                List<SelectListItem> roles = new List<SelectListItem>();
                var role = _roleRepository.GetActiveRoles();
                for (int i = 0; i < role.Count; i++)
                {
                    if (role[i].RoleName != "Company Owner" && !role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "DIBN" && role[i].RoleName != "RM Team")
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
                user.Module = name;
                user.CompanyId = Id;
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateNewEmployeeForCompanys(SaveUser user)
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


                List<SelectListItem> roles = new List<SelectListItem>();
                var role = _roleRepository.GetActiveRoles();
                for (int i = 0; i < role.Count; i++)
                {
                    if (role[i].RoleName != "DIBN" &&  role[i].RoleName != "Company Owner" && !role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "RM Team")
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
                    int returnId = 0;
                    returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber,null);

                    if (returnId > 0)
                    {
                        user.CreatedBy = userId;
                        returnId = _userRepository.CreateUserForCompany(user);
                        return RedirectToAction("GetCompanyEmployees", "Company", new { Id = user.CompanyId, name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");

                        user.Countries = _countries;
                        user.Roles = roles;
                        return View("CreateNewEmployeeForCompany", user);
                    }
                }

                user.Countries = _countries;
                user.Roles = roles;

                return View("CreateNewEmployeeForCompany", user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CreateNewUserForCompany(int Id, string? name)
        {
            SaveNewUser user = new SaveNewUser();
            List<SelectListItem> roles = new List<SelectListItem>();
            var role = _roleRepository.GetActiveRoles();

            for (int i = 0; i < role.Count; i++)
            {
                if (role[i].RoleName != "Company Owner")
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
            user.RoleId = new List<int>();
            user.RoleId.Add(_roleRepository.GetCompanyOwnerId());
            user.Module = name;
            user.CompanyId = Id;
            return View(user);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateNewUserForCompanys(SaveNewUser user)
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
                    int id = 0;
                    int returnId = 0;
                    int emailExsitance = 1;
                    returnId = _userRepository.CheckExistanceOfUserAccountNumber(user.AccountNumber,null);
                    if (returnId > 0 && emailExsitance > 0)
                    {
                        user.CreatedBy = userId;
                        id = _userRepository.CreateEmployee(user);
                        return RedirectToAction("GetCompanyUsers", "Company", new { Id = user.CompanyId, name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");

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
                        user.Roles = roleList;
                        user.Companies = companyList;
                        user.RoleId = new List<int>();
                        user.RoleId.Add(_roleRepository.GetCompanyOwnerId());
                        return View("CreateNewUserForCompany", user);
                    }

                }
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
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
                user.Countries = _countries;
                user.Roles = roles;
                user.Companies = companies;
                user.RoleId = new List<int>();
                user.RoleId.Add(_roleRepository.GetCompanyOwnerId());
                return View("CreateNewUserForCompany", user);
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
                user = _userRepository.GetUserDetail(Id);
                user.Countries = _countries;
                user.Roles = roles;
                user.Module = name;
                user.Companies = companies;
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult EditCompanyEmployee(int Id, string? name)
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
                    if (role[i].RoleName != "DIBN" && role[i].RoleName != "Company Owner" && !role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "RM Team")
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
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EditCompanyEmployees(UserViewModel user)
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
                    if (role[i].RoleName != "DIBN" && role[i].RoleName != "Company Owner" && !role[i].RoleName.StartsWith("Sales") && role[i].RoleName != "RM Team")
                    {
                        roles.Add(new SelectListItem
                        {
                            Text = role[i].RoleName,
                            Value = role[i].RoleID.ToString()
                        });
                    }
                }
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
                user.Companies = companies;

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
                                var userIdentity = (ClaimsIdentity)User.Identity;
                                var claims = userIdentity.Claims;
                                var roleClaimType = userIdentity.RoleClaimType;
                                var _roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                                var _users = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
                                var _actors = claims.Where(c => c.Type == ClaimTypes.Actor).ToList();
                                for (int i = 0; i < loggedInEmail.Count; i++)
                                {
                                    if (!loggedInEmail[i].Contains("DIBN"))
                                    {
                                        userIdentity.RemoveClaim(_roles[i]);
                                        userIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));

                                        var principle = new ClaimsPrincipal(new[] { userIdentity });
                                        var loginData = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
                                    }
                                }
                            }
                        }
                        return RedirectToAction(user.Action, "Company", new { Id = user.CompanyId, name = user.Module });
                    }
                    else
                    {
                        if (returnId == -1)
                            ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");
                        return View(user.returnAction, user);
                    }
                }
                
                return View(user.returnAction, user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyUserDetail(int UserId, string? name, string returnaction)
        {
            try
            {
                UserViewModel user = new UserViewModel();
                user = _userRepository.GetUserDetail(UserId);
                user.Module = name;
                user.Action = returnaction;
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DeleteCompanyEmployee(int Id)
        {
            try
            {
                int returnId = 0;
                string Username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(Username);
                returnId = _userRepository.DeleteUser(Id, null, userId);
                return new JsonResult(returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UploadUserDocument(int UserId, int CompanyId, int? message, string? name, string? actionName)
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

            UploadUserDocuments model = new UploadUserDocuments();
            model.UserId = UserId;
            model.CompanyId = CompanyId;
            model.Module = name;
            model.IsActive = await _userRepository.GetCompanyEmployeeActiveStatus(UserId);
            model.actionName = actionName;
            model.Role = actionName;
            if (model.CompanyId > 0)
            {
                List<DocumentsViewModel> documents = new List<DocumentsViewModel>();
                documents = _companyRepository.GetAllDocuments(CompanyId, model.UserId);
                model.userDocuments = documents;
            }
            else
            {
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
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            document.CreatedBy = UserId;
            returnId = _companyRepository.UploadSelectedFile(document, document.CompanyId);
            if (returnId > 0)
            {
                ViewData["Message"] = "Document Uploaded Successfully..!!";
            }
            else if (returnId < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
            }
            return RedirectToAction("UploadUserDocument", "Company", new { UserId = document.UserId, CompanyId = document.CompanyId, message = returnId, name = document.Module, actionName=document.actionName });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyPermissionModule(int Id)
        {
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetCompanyPermissionModuleByCompanyId(Id);
            return new JsonResult(Modules);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyPermissionsName(int Id, string Module)
        {
            List<string> Modules = new List<string>();
            Modules = _permissionRepository.GetCompanyPermissionName(Id, Module);
            return new JsonResult(Modules);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyServiceRequests(int Id, string Module)
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

            GetServiceRequestForCompany getServiceRequestForCompany = new GetServiceRequestForCompany();

            CompanyViewModel companyViewModel = new CompanyViewModel();
            companyViewModel = _companyRepository.GetCompanyById(Id);

            List<GetRequestsModel> getRequests = new List<GetRequestsModel>();
            getRequests = _serviceFormRepository.GetAllRequests(Id, 0);

            getServiceRequestForCompany.getServiceRequest = getRequests;
            getServiceRequestForCompany.Company = companyViewModel.CompanyName;
            getServiceRequestForCompany.companyId = Id;
            getServiceRequestForCompany.allowedModule = allowedModule;
            return View(getServiceRequestForCompany);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyServiceRequests(int CompanyId)
        {
            try
            {
                string searchBy = "";
                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                string role = GetClaims();
                string _companyId = GetActorClaims();
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
                model = await _serviceFormRepository.GetAllCompanyRequestByCompanyId(CompanyId, null, skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchValue);
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
        public async Task<IActionResult> GetAllCompanyServiceRequestsFilter(int CompanyId,string sortColumn,string sortColumnDirection,string? searchBy,string? searchedValue)
        {
            try
            {
                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _serviceFormRepository.GetAllCompanyRequestByCompanyId(CompanyId, null, skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchedValue);
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
        public async Task<IActionResult> GetAllEmployeeServiceRequests(int CompanyId)
        {
            try
            {
                string searchBy = "";
                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                string role = GetClaims();
                string _companyId = GetActorClaims();
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
                model = await _serviceFormRepository.GetAllEmployeeRequestByCompanyId(CompanyId, null, skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchValue);
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
        public async Task<IActionResult> GetAllEmployeeServiceRequestsFilter(int CompanyId, string sortColumn, string sortColumnDirection, string? searchBy, string? searchedValue)
        {
            try
            {
                GetCompanyServiceRequestsByCompanyId model = new GetCompanyServiceRequestsByCompanyId();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _serviceFormRepository.GetAllEmployeeRequestByCompanyId(CompanyId, null, skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchedValue);
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

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanySupportTickets(int Id, string Module, int? Status)
        {
            List<SupportTicketRequest> GetAllSupportTickets = new List<SupportTicketRequest>();
            GetAllSupportTickets = _supportTicketRepository.GetAllSupportTickets(Id, Status);
            if (GetAllSupportTickets.Count > 0)
            {
                foreach (var ticket in GetAllSupportTickets)
                {
                    ticket.Module = "SupportTickets";
                    ticket.CompanyId  = Id;
                }
            }
            else
            {
                SupportTicketRequest supportTicketRequest = new SupportTicketRequest();
                CompanyViewModel companyViewModel = new CompanyViewModel();
                companyViewModel = _companyRepository.GetCompanyById(Id);
                supportTicketRequest.CompanyName = companyViewModel.CompanyName;
                supportTicketRequest.Module = "SupportTickets";
                supportTicketRequest.CompanyId = Id;
                GetAllSupportTickets.Add(supportTicketRequest);
            }
            return View(GetAllSupportTickets);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAuthorityNames(string prefix)
        {
            List<string> names = new List<string>();
            string? _half = null;
            int length = 0;
            if (prefix != null)
                length = prefix.Length;
            names = _companyRepository.GetAuthorityNames();
            var name = (from N in names
                        where N.StartsWith(prefix)

                        select new { N });
            if (name.Count() == 0)
            {
                if (length > 1)
                {
                    _half = prefix.Substring(0, 1);
                }
                name = (from N in names
                        where N.StartsWith(_half == null ? prefix.ToUpper() : _half.ToUpper())

                        select new { N });
            }
            if (name.Count() == 0)
            {
                if (length > 1)
                {
                    _half = prefix.Substring(0, 1);
                }
                name = (from N in names
                        where N.StartsWith(_half == null ? prefix.ToLower() : _half.ToLower())

                        select new { N });
            }
            return new JsonResult(name);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetLastAccountNumber()
        {
            string accountNumber = _companyRepository.GetLastAccountNumber();
            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(accountNumber);
            string alphaPart = result.Groups[1].Value;
            int numberPart = Convert.ToInt32(result.Groups[2].Value);
            int Id = numberPart + 1;
            accountNumber = alphaPart + Id.ToString();
            return new JsonResult(accountNumber);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCountries()
        {
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();
            return new JsonResult(countryList);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult CheckAccountNumberExistance(string AccountNumber)
        {
            int returnId = 0;
            returnId = _companyRepository.CheckExistanceOfCompanyAccountNumber(AccountNumber);
            return new JsonResult(returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetDialingCodeOfCountry(string Country)
        {
            ICountryProvider countryProvider = new CountryProvider();
            var countryInfo = countryProvider.GetCountryByName(Country);
            return new JsonResult("+" + countryInfo.CallingCodes[0] + " ");
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyInvoices(int Id, string name)
        {
            int _companyId = 0;
            _companyId = Id;

            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "Invoice");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "Invoice");
                }
            }

            GetPIAndFinalInvoices model = new GetPIAndFinalInvoices();

            CompanyViewModel companyViewModel = new CompanyViewModel();
            companyViewModel = _companyRepository.GetCompanyById(Id);

            model.Module = name;
            model.Company = companyViewModel.CompanyName;
            model.CompanyId = _companyId;
            model.allowedModule = allowedModule;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyName(int Id)
        {
            CompanyViewModel model = new CompanyViewModel();
            model = _companyRepository.GetCompanyById(Id);
            return new JsonResult(model.CompanyName);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyWiseLog(int companyId, string? name)
        {
            try
            {
                GetCompanyLogDetails model = new GetCompanyLogDetails();
                model.logs = new List<GetCompanyLog>();
                List<GetCompanyLog> logs = new List<GetCompanyLog>();
                logs = _companyRepository.GetAllLogByCompanyId(companyId);
                model.logs = logs;
                model.Module = name;
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyListForExport()
        {
            List<GetCompanyListForExport> model = new List<GetCompanyListForExport>();
            model = _companyRepository.GetCompanyListForExcelExport();
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("Company Code\r\n"),
                                        new DataColumn("Company Name\r\n"),});
            foreach(var item in model)
            {
                dt.Rows.Add(item.CompanyCode,item.CompanyName);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DIBN_Companys.xlsx");
                }
            };
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCompanySubTypePrefix(string companyType,string prefix)
        {
            List<string> subTypes = new List<string>();
            try
            {
                subTypes = await _companyRepository.GetCompanySubTypePrefix(companyType, prefix);
                return new JsonResult(subTypes);
            }
            catch(Exception ex)
            {
                return new JsonResult(subTypes);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRoleCounts()
        {
            int _counts = 0;
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            var users = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
            var actors = claims.Where(c => c.Type == ClaimTypes.Actor).ToList();
            if (roles.Count > 1 && roles[0].Value != "DIBN")
            {
                userIdentity.RemoveClaim(roles[0]);
                userIdentity.RemoveClaim(users[0]);
                userIdentity.RemoveClaim(actors[0]);
            }
            roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            return new JsonResult(_counts);

        }

        /// <summary>
        /// GetAllCompanyListWithPagination - Return Company List with pagination                                                                   -- Yashasvi TBC (22-12-2022)
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyListWithPagination()
        {
            try
            {
                GetCompanyDetailsWithPaginationModel model = new GetCompanyDetailsWithPaginationModel();
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
                model =await _companyRepository.GetCompanyListWithPagination(skip, pageSize, sortColumn, sortColumnDirection,searchValue);
                filterRecord = model.totalCompanies;
                totalRecord = model.totalCompanies;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getCompanyDetails
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
    }
}
