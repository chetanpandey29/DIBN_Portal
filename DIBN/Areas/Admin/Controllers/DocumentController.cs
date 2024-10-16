using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static DIBN.Areas.Admin.Models.UserViewModel;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class DocumentController : Controller
    {
        public readonly ICompanyRepository _companyRepository;
        public readonly IPermissionRepository _permissionRepository;
        public readonly IUserRepository _userRepository;
        public readonly ICompanyDocumentTypeRepository _companyDocumentTypeRepository;
        private readonly IShareholderRepository _shareholderRepository;

        public DocumentController(
            ICompanyRepository companyRepository,
            IPermissionRepository permissionRepository,
            IUserRepository userRepository,
            ICompanyDocumentTypeRepository companyDocumentTypeRepository,
            IShareholderRepository shareholderRepository
        )
        {
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;   
            _userRepository = userRepository;
            _companyDocumentTypeRepository = companyDocumentTypeRepository;
            _shareholderRepository = shareholderRepository;
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
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "DocumentChecklist");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "DocumentChecklist");
                    }
                }
                GetCompaniesWithMainCompany companies = new GetCompaniesWithMainCompany();

                List<CompanyViewModel> otherCompanies = new List<CompanyViewModel>();
                otherCompanies = _companyRepository.GetCompanies();

                List<CompanyViewModel> mainCompany = new List<CompanyViewModel>();
                mainCompany = _companyRepository.GetMainCompany();

                companies.GetMainCompany = mainCompany;
                companies.GetOtherCompanies = otherCompanies;
                companies.Module = name;
                companies.allowedModule = allowedModule;
                Log.Information("Show list of Companies for Document Checklist");
                return View(companies);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllDocuments(int CompanyId,string? name)
        {
            try
            {
                List<string> allowedModule = new List<string>();

                string Role = GetClaims();
                string User = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(User);
                allowedModule = _permissionRepository.GetUserPermissionName(UserId, "DocumentChecklist");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "DocumentChecklist");
                    }
                }
                CompanyWiseDocumentList companyWiseDocumentList = new CompanyWiseDocumentList();

                List<CompanyDocumentTypeModel> companyDocumentTypes = new List<CompanyDocumentTypeModel>();
                
                List<DocumentsViewModel> companyDocuments = new List<DocumentsViewModel>();

                List<GetActiveEmployees> getActiveEmployees = new List<GetActiveEmployees>();

                List<GetInActiveEmployees> getInActiveEmployees = new List<GetInActiveEmployees>();

                List<ShareholderViewModel> shareholders = new List<ShareholderViewModel>();

                List<GetActiveEmployeesWithDocument> activeEmpDocuments = new List<GetActiveEmployeesWithDocument>();
                List<GetInActiveEmployeesWithDocument> inactiveEmpDocuments = new List<GetInActiveEmployeesWithDocument>();

                List<GetShareholderWithDocument> shareholderDocuments = new List<GetShareholderWithDocument>();

                List<companyDocumentUploaded> getcompanyDocument = new List<companyDocumentUploaded>();

                List<SelectListItem> _empDocumentType = new List<SelectListItem>();

                CompanyViewModel company = new CompanyViewModel();
                company = _companyRepository.GetCompanyById(CompanyId);

                _empDocumentType.Add(new SelectListItem { 
                    Text = "Passport",
                    Value = "1"
                });

                _empDocumentType.Add(new SelectListItem
                {
                    Text = "Photographs",
                    Value = "2"
                });

                _empDocumentType.Add(new SelectListItem
                {
                    Text = "Employment card",
                    Value = "3"
                });

                _empDocumentType.Add(new SelectListItem
                {
                    Text = "Employment contract",
                    Value = "4"
                });

                _empDocumentType.Add(new SelectListItem
                {
                    Text = "Residency Visa",
                    Value = "5"
                });

                _empDocumentType.Add(new SelectListItem
                {
                    Text = "Emirates ID",
                    Value = "6"
                });

                _empDocumentType.Add(new SelectListItem
                {
                    Text = "Insurance policy",
                    Value = "7"
                });

                companyDocuments = _companyRepository.GetCompanyDocuments(CompanyId);

                companyDocumentTypes = _companyDocumentTypeRepository.GetCompanyDocuments();

                getActiveEmployees = _userRepository.GetActiveEmployeesCompanyWise(CompanyId);

                getInActiveEmployees = _userRepository.GetInActiveEmployeesCompanyWise(CompanyId);

                shareholders = _shareholderRepository.GetAllShareholders(CompanyId);

                if(companyDocuments.Count > 0 && companyDocuments != null)
                {
                    List<string> _uploaded = new List<string>();
                    for(int i = 0; i < companyDocuments.Count; i++)
                    {
                        for(int j=0;j<companyDocumentTypes.Count; j++)
                        {
                            if(companyDocumentTypes[j].DocumentName == companyDocuments[i].DocumentTypeName)
                            {
                                companyDocumentUploaded documentUploaded = new companyDocumentUploaded();
                                documentUploaded.Document = companyDocumentTypes[j].DocumentName;
                                documentUploaded.uploaded = "true";
                                _uploaded.Add(documentUploaded.Document);

                                companyDocumentUploaded _value = new companyDocumentUploaded();
                                _value = getcompanyDocument.Find(x => x.Document == companyDocumentTypes[j].DocumentName);
                                if(_value != null)
                                {
                                    if(_value.Document == "")
                                    {
                                        getcompanyDocument.Add(documentUploaded);
                                    }
                                }
                                else
                                {
                                    getcompanyDocument.Add(documentUploaded);
                                }
                            }

                        }
                    }
                    if (_uploaded.Count > 0)
                    {
                        List<string> _notUploaded = new List<string>();

                        for (int i = 0; i < companyDocumentTypes.Count; i++)
                        {
                            if (!_uploaded.Contains(companyDocumentTypes[i].DocumentName))
                            {
                                _notUploaded.Add(companyDocumentTypes[i].DocumentName);
                            }
                        }

                        if (_notUploaded.Count > 0)
                        {
                            for (int j = 0; j < _notUploaded.Count; j++)
                            {
                                companyDocumentUploaded documentUploaded = new companyDocumentUploaded();
                                documentUploaded.Document = _notUploaded[j];
                                documentUploaded.uploaded = "false";
                                getcompanyDocument.Add(documentUploaded);
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < companyDocumentTypes.Count; j++)
                    {
                        companyDocumentUploaded documentUploaded = new companyDocumentUploaded();
                        documentUploaded.Document = companyDocumentTypes[j].DocumentName;
                        documentUploaded.uploaded = "false";
                        getcompanyDocument.Add(documentUploaded);
                    }
                }
                if (getActiveEmployees != null && getActiveEmployees.Count>0)
                {
                    for(int index =0;index< getActiveEmployees.Count; index++)
                    {
                        GetActiveEmployeesWithDocument document = new GetActiveEmployeesWithDocument();
                        document.UserId = getActiveEmployees[index].Id;
                        document.FirstName = getActiveEmployees[index].FirstName;
                        document.LastName = getActiveEmployees[index].LastName;
                        document.Designation = getActiveEmployees[index].Designation;
                        document.Role = getActiveEmployees[index].Role;
                        document.empDocuments = _companyRepository.GetAllDocuments(CompanyId, getActiveEmployees[index].Id);
                        activeEmpDocuments.Add(document);
                    }    
                }

                if(activeEmpDocuments!=null && activeEmpDocuments.Count > 0)
                {
                    for (int index = 0; index < activeEmpDocuments.Count; index++)
                    {
                        activeEmpDocuments[index].documentDetails = new List<GetActiveEmpDocumentDetails>();
                        List<string> _uploaded = new List<string>();
                        for (int secIndex = 0; secIndex < _empDocumentType.Count; secIndex++)
                        {
                            if (activeEmpDocuments[index].empDocuments.Count > 0)
                            {
                                for(int i=0;i< activeEmpDocuments[index].empDocuments.Count; i++)
                                {
                                    if (Convert.ToInt32(_empDocumentType[secIndex].Value) == activeEmpDocuments[index].empDocuments[i].SelectedDocumentType)
                                    {
                                        GetActiveEmpDocumentDetails documentDetails1 = new GetActiveEmpDocumentDetails();
                                        documentDetails1.uploaded = "true";
                                        documentDetails1.Document = _empDocumentType[secIndex].Text;

                                        GetActiveEmpDocumentDetails _value = new GetActiveEmpDocumentDetails();
                                        _value = activeEmpDocuments[index].documentDetails.Find(x=>x.Document == documentDetails1.Document);
                                        if(_value != null)
                                        {
                                            if (_value.Document == "")
                                            {
                                                _uploaded.Add(_empDocumentType[secIndex].Text);
                                                activeEmpDocuments[index].documentDetails.Add(documentDetails1);
                                            }
                                        }
                                        else
                                        {
                                            _uploaded.Add(_empDocumentType[secIndex].Text);
                                            activeEmpDocuments[index].documentDetails.Add(documentDetails1);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                GetActiveEmpDocumentDetails documentDetails1 = new GetActiveEmpDocumentDetails();
                                documentDetails1.uploaded = "false";
                                documentDetails1.Document = _empDocumentType[secIndex].Text;
                                activeEmpDocuments[index].documentDetails.Add(documentDetails1);
                            }
                        }
                        if (_uploaded.Count > 0)
                        {
                            List<string> _notUploaded = new List<string>();
                            
                            for(int i=0;i< _empDocumentType.Count; i++)
                            {
                                if (!_uploaded.Contains(_empDocumentType[i].Text))
                                {
                                    _notUploaded.Add(_empDocumentType[i].Text);
                                }
                            }

                            if (_notUploaded.Count > 0)
                            {
                                for (int j = 0; j < _notUploaded.Count; j++)
                                {
                                    GetActiveEmpDocumentDetails documentDetails1 = new GetActiveEmpDocumentDetails();
                                    documentDetails1.uploaded = "false";
                                    documentDetails1.Document = _notUploaded[j];
                                    activeEmpDocuments[index].documentDetails.Add(documentDetails1);
                                }
                            }
                        }
                    }
                }

                if (getInActiveEmployees != null && getInActiveEmployees.Count > 0)
                {
                    for (int index = 0; index < getInActiveEmployees.Count; index++)
                    {
                        GetInActiveEmployeesWithDocument document = new GetInActiveEmployeesWithDocument();
                        document.UserId = getInActiveEmployees[index].Id;
                        document.FirstName = getInActiveEmployees[index].FirstName;
                        document.LastName = getInActiveEmployees[index].LastName;
                        document.Designation = getInActiveEmployees[index].Designation;
                        document.Role = getInActiveEmployees[index].Role;
                        document.empDocuments = _companyRepository.GetAllDocuments(CompanyId, getInActiveEmployees[index].Id);
                        inactiveEmpDocuments.Add(document);
                    }
                }

                if (getInActiveEmployees != null && getInActiveEmployees.Count > 0)
                {
                    for (int index = 0; index < getInActiveEmployees.Count; index++)
                    {
                        inactiveEmpDocuments[index].documentDetails = new List<GetInActiveEmpDocumentDetails>();
                        List<string> _uploaded = new List<string>();
                        for (int secIndex = 0; secIndex < _empDocumentType.Count; secIndex++)
                        {
                            if (inactiveEmpDocuments[index].empDocuments.Count > 0)
                            {
                                for (int i = 0; i < inactiveEmpDocuments[index].empDocuments.Count; i++)
                                {
                                    if (Convert.ToInt32(_empDocumentType[secIndex].Value) == inactiveEmpDocuments[index].empDocuments[i].SelectedDocumentType)
                                    {
                                        GetInActiveEmpDocumentDetails documentDetails1 = new GetInActiveEmpDocumentDetails();
                                        documentDetails1.uploaded = "true";
                                        _uploaded.Add(_empDocumentType[secIndex].Text);
                                        documentDetails1.Document = _empDocumentType[secIndex].Text;

                                        GetInActiveEmpDocumentDetails _value = new GetInActiveEmpDocumentDetails();
                                        _value = inactiveEmpDocuments[index].documentDetails.Find(x => x.Document == documentDetails1.Document);
                                        if (_value != null)
                                        {
                                            if (_value.Document == "")
                                            {
                                                inactiveEmpDocuments[index].documentDetails.Add(documentDetails1);
                                            }
                                        }
                                        else
                                        {
                                            inactiveEmpDocuments[index].documentDetails.Add(documentDetails1);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                GetInActiveEmpDocumentDetails documentDetails1 = new GetInActiveEmpDocumentDetails();
                                documentDetails1.uploaded = "false";
                                documentDetails1.Document = _empDocumentType[secIndex].Text;
                                inactiveEmpDocuments[index].documentDetails.Add(documentDetails1);
                            }
                        }
                        if (_uploaded.Count > 0)
                        {
                            List<string> _notUploaded = new List<string>();

                            for (int i = 0; i < _empDocumentType.Count; i++)
                            {
                                if (!_uploaded.Contains(_empDocumentType[i].Text))
                                {
                                    _notUploaded.Add(_empDocumentType[i].Text);
                                }
                            }

                            if (_notUploaded.Count > 0)
                            {
                                for (int j = 0; j < _notUploaded.Count; j++)
                                {
                                    GetInActiveEmpDocumentDetails documentDetails1 = new GetInActiveEmpDocumentDetails();
                                    documentDetails1.uploaded = "false";
                                    documentDetails1.Document = _notUploaded[j];
                                    inactiveEmpDocuments[index].documentDetails.Add(documentDetails1);
                                }
                            }
                        }
                    }
                }


                if (shareholders != null && shareholders.Count > 0)
                {
                    for (int index = 0; index < shareholders.Count; index++)
                    {
                        GetShareholderWithDocument document = new GetShareholderWithDocument();
                        document.UserId = shareholders[index].ID;
                        document.FirstName = shareholders[index].FirstName;
                        document.LastName = shareholders[index].LastName;
                        document.shareholderDocuments = _shareholderRepository.GetShareholdersDocuments(shareholders[index].ID,null);
                        shareholderDocuments.Add(document);
                    }
                }

                companyWiseDocumentList.CompanyDocuments = companyDocuments;
                companyWiseDocumentList.companyDocumentTypes = companyDocumentTypes;
                companyWiseDocumentList.GetActiveEmpDocuments = activeEmpDocuments;
                companyWiseDocumentList.GetInActiveEmpDocuments = inactiveEmpDocuments;
                companyWiseDocumentList.GetshareholderDocuments = shareholderDocuments;
                companyWiseDocumentList.DubaiMainlandEmpDocumentType = _empDocumentType;
                companyWiseDocumentList.Company = company.CompanyName;
                companyWiseDocumentList.shareholders = shareholders;
                companyWiseDocumentList.getCompanyUploadedDocument = getcompanyDocument;
                companyWiseDocumentList.Module = name;
                companyWiseDocumentList.allowedModule = allowedModule;
                Log.Information("Check Documents of all shareholders , Employees and Company Documents of "+ company.CompanyName);
                return View(companyWiseDocumentList);
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
