using DIBN.IService;
using DIBN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static DIBN.Models.UserEmployeeViewModel;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class DocumentController : Controller
    {
        private readonly IUserCompanyService _userCompanyService;
        private readonly IUserPermissionService _userPermissionService;
        private readonly IUserEmployeesService _userEmployeesService;
        private readonly IFileUploaderService _fileUploaderService;
        private readonly IUserShareholderService _userShareholderService;

        public DocumentController(
            IUserCompanyService userCompanyService,
            IUserPermissionService userPermissionService,
            IUserEmployeesService userEmployeesService,
            IFileUploaderService fileUploaderService,
            IUserShareholderService userShareholderService
        )
        {
            _userCompanyService = userCompanyService;
            _userPermissionService = userPermissionService;
            _userEmployeesService = userEmployeesService;
            _fileUploaderService = fileUploaderService;
            _userShareholderService = userShareholderService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                List<string> allowedModule = new List<string>();

                string User = GetUserClaims();
                int UserId = _userPermissionService.GetUserIdForPermission(User);
                allowedModule = _userPermissionService.GetUserPermissionName(UserId, "DocumentChecklist");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _userPermissionService.GetCurrentRolePermissionName(Role, "DocumentChecklist");
                    }
                }

                List<UserCompanyViewModel> otherCompanies = new List<UserCompanyViewModel>();
                otherCompanies = _userCompanyService.GetCompanies();

                MainDocumentCheckListModel mainModel = new MainDocumentCheckListModel();
                mainModel.companies = otherCompanies;
                mainModel.Module = name;
                mainModel.allowedModule = allowedModule;
                return View(mainModel);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllDocuments(int CompanyId, string? name)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                List<string> allowedModule = new List<string>();

                string User = GetUserClaims();
                int UserId = _userPermissionService.GetUserIdForPermission(User);
                allowedModule = _userPermissionService.GetUserPermissionName(UserId, "DocumentChecklist");
                List<string> allowedPermission = new List<string>();
                allowedPermission = _userPermissionService.GetUserPermissionedModuleName(UserId);
                if (allowedPermission.Count == 0 && allowedModule.Count == 0)
                {
                    if (allowedModule.Count == 0)
                    {
                        allowedModule = _userPermissionService.GetCurrentRolePermissionName(Role, "DocumentChecklist");
                    }
                }
                CompanyWiseDocumentList companyWiseDocumentList = new CompanyWiseDocumentList();

                List<CompanyDocumentTypeModel> companyDocumentTypes = new List<CompanyDocumentTypeModel>();

                List<UserDocumentsViewModel> companyDocuments = new List<UserDocumentsViewModel>();

                List<GetActiveEmployees> getActiveEmployees = new List<GetActiveEmployees>();

                List<GetInActiveEmployees> getInActiveEmployees = new List<GetInActiveEmployees>();

                List<ShareholderViewModel> shareholders = new List<ShareholderViewModel>();

                List<GetActiveEmployeesWithDocument> activeEmpDocuments = new List<GetActiveEmployeesWithDocument>();
                List<GetInActiveEmployeesWithDocument> inactiveEmpDocuments = new List<GetInActiveEmployeesWithDocument>();

                List<GetShareholderWithDocument> shareholderDocuments = new List<GetShareholderWithDocument>();

                List<companyDocumentUploaded> getcompanyDocument = new List<companyDocumentUploaded>();

                List<SelectListItem> _empDocumentType = new List<SelectListItem>();


                UserCompanyViewModel company = new UserCompanyViewModel();
                company = _userCompanyService.GetCompanyById(CompanyId);

                _empDocumentType.Add(new SelectListItem
                {
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

                companyDocumentTypes = _userCompanyService.GetCompanyDocuments();

                companyDocuments = _fileUploaderService.GetAllDocuments(CompanyId);

                getActiveEmployees = _userEmployeesService.GetAllActiveEmployees(CompanyId);

                getInActiveEmployees = _userEmployeesService.GetAllInActiveEmployees(CompanyId);

                shareholders = _userShareholderService.GetAllShareholders(CompanyId);

                if (companyDocuments.Count > 0 && companyDocuments != null)
                {
                    List<string> _uploaded = new List<string>();
                    for (int i = 0; i < companyDocuments.Count; i++)
                    {
                        for (int j = 0; j < companyDocumentTypes.Count; j++)
                        {
                            if (companyDocumentTypes[j].DocumentName == companyDocuments[i].DocumentTypeName)
                            {
                                companyDocumentUploaded documentUploaded = new companyDocumentUploaded();
                                documentUploaded.Document = companyDocumentTypes[j].DocumentName;
                                documentUploaded.uploaded = "true";
                                _uploaded.Add(documentUploaded.Document);

                                companyDocumentUploaded _value = new companyDocumentUploaded();
                                _value = getcompanyDocument.Find(x => x.Document == companyDocumentTypes[j].DocumentName);
                                if (_value != null)
                                {
                                    if (_value.Document == "")
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

                if (getActiveEmployees != null && getActiveEmployees.Count > 0)
                {
                    for (int index = 0; index < getActiveEmployees.Count; index++)
                    {
                        GetActiveEmployeesWithDocument document = new GetActiveEmployeesWithDocument();
                        document.UserId = getActiveEmployees[index].Id;
                        document.FirstName = getActiveEmployees[index].FirstName;
                        document.LastName = getActiveEmployees[index].LastName;
                        document.Designation = getActiveEmployees[index].Role;
                        document.empDocuments = _fileUploaderService.GetAllDocumentsByUserId(CompanyId, getActiveEmployees[index].Id);
                        activeEmpDocuments.Add(document);
                    }
                }

                if (activeEmpDocuments != null && activeEmpDocuments.Count > 0)
                {
                    for (int index = 0; index < activeEmpDocuments.Count; index++)
                    {
                        activeEmpDocuments[index].documentDetails = new List<GetActiveEmpDocumentDetails>();
                        List<string> _uploaded = new List<string>();
                        for (int secIndex = 0; secIndex < _empDocumentType.Count; secIndex++)
                        {
                            if (activeEmpDocuments[index].empDocuments.Count > 0)
                            {
                                for (int i = 0; i < activeEmpDocuments[index].empDocuments.Count; i++)
                                {
                                    if (Convert.ToInt32(_empDocumentType[secIndex].Value) == activeEmpDocuments[index].empDocuments[i].SelectedDocumentType)
                                    {
                                        GetActiveEmpDocumentDetails documentDetails1 = new GetActiveEmpDocumentDetails();
                                        documentDetails1.Uploaded = "true";
                                        _uploaded.Add(_empDocumentType[secIndex].Text);
                                        documentDetails1.Document = _empDocumentType[secIndex].Text;

                                        GetActiveEmpDocumentDetails _value = new GetActiveEmpDocumentDetails();
                                        _value = activeEmpDocuments[index].documentDetails.Find(x => x.Document == documentDetails1.Document);
                                        if (_value != null)
                                        {
                                            if (_value.Document == "")
                                            {
                                                activeEmpDocuments[index].documentDetails.Add(documentDetails1);
                                            }
                                        }
                                        else
                                        {
                                            activeEmpDocuments[index].documentDetails.Add(documentDetails1);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                GetActiveEmpDocumentDetails documentDetails1 = new GetActiveEmpDocumentDetails();
                                documentDetails1.Uploaded = "false";
                                documentDetails1.Document = _empDocumentType[secIndex].Text;
                                activeEmpDocuments[index].documentDetails.Add(documentDetails1);
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
                                    GetActiveEmpDocumentDetails documentDetails1 = new GetActiveEmpDocumentDetails();
                                    documentDetails1.Uploaded = "false";
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
                        document.Designation = getInActiveEmployees[index].Role;
                        document.empDocuments = _fileUploaderService.GetAllDocumentsByUserId(CompanyId, getInActiveEmployees[index].Id);
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
                                        documentDetails1.Uploaded = "true";
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
                                documentDetails1.Uploaded = "false";
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
                                    documentDetails1.Uploaded = "false";
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
                        document.shareholderDocuments = _userShareholderService.GetShareholdersDocuments(shareholders[index].ID, null);
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
                return View(companyWiseDocumentList);
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
