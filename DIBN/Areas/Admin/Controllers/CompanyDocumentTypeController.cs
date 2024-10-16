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
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class CompanyDocumentTypeController : Controller
    {
        private readonly ICompanyDocumentTypeRepository _companyDocumentTypeRepository;
        private readonly IPermissionRepository _permissionRepository;
        public CompanyDocumentTypeController(ICompanyDocumentTypeRepository companyDocumentTypeRepository, IPermissionRepository permissionRepository)
        {
            _companyDocumentTypeRepository = companyDocumentTypeRepository;
            _permissionRepository = permissionRepository;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            MainCompanyDocumentTypeModel mainModel = new MainCompanyDocumentTypeModel();

            List<CompanyDocumentTypeModel> documentTypes = new List<CompanyDocumentTypeModel>();
            documentTypes = _companyDocumentTypeRepository.GetCompanyDocuments();

            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "CompanyDocumentType");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "CompanyDocumentType");
                }
            }

            mainModel.documentTypes = documentTypes;
            mainModel.allowedModule = allowedModule;
            mainModel.Module = name;
            return View(mainModel);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Create(string? name)
        {
            CompanyDocumentTypeModel document = new CompanyDocumentTypeModel();
            document.Module = name;
            return PartialView("Create", document);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Creates(CompanyDocumentTypeModel documentType)
        {
            int returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            documentType.CreatedBy = userId;
            returnId = _companyDocumentTypeRepository.AddCompanyDocumentType(documentType);
            return RedirectToAction("Index", "CompanyDocumentType", new { name = documentType.Module });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(string? name, int Id)
        {
            CompanyDocumentTypeModel documentType = new CompanyDocumentTypeModel();
            documentType = _companyDocumentTypeRepository.GetCompanyDocumentById(Id);
            documentType.Module = name;
            return PartialView("Edit", documentType);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(CompanyDocumentTypeModel documentType)
        {
            int returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            documentType.CreatedBy = userId;
            returnId = _companyDocumentTypeRepository.UpdateCompanyDocumentType(documentType);
            return RedirectToAction("Index", "CompanyDocumentType", new { name = documentType.Module });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Remove(int Id)
        {
            int returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            returnId = _companyDocumentTypeRepository.RemoveCompanyDocumentType(Id, userId);
            return new JsonResult(returnId);
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
