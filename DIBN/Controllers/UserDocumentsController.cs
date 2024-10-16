using DIBN.IService;
using DIBN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserDocumentsController : Controller
    {
        private readonly IFileUploaderService _fileUploaderService; 
        private readonly IUserPermissionService _userPermissionService;
        public UserDocumentsController(IFileUploaderService fileUploaderService, IUserPermissionService userPermissionService)
        {
            _fileUploaderService = fileUploaderService;
            _userPermissionService = userPermissionService;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name,int? message)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string strCompanyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(strCompanyId);
            List<UserDocumentsViewModel> documents = new List<UserDocumentsViewModel>();
            documents = _fileUploaderService.GetAllDocuments(CompanyId);
            foreach(UserDocumentsViewModel document in documents)
            {
                document.Module = name;
            }
            if(documents==null || documents.Count== 0)
            {
                UserDocumentsViewModel model = new UserDocumentsViewModel();
                model.Module = name;
                documents.Add(model);
            }
            if (message > 0)
            {
                ViewData["Message"] = "Document Uploaded Successfully..!!";
            }
            else if(message < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
            }
            return View(documents);
        }

        //[HttpPost]
        //[Route("[action]")]
        //public IActionResult UploadUserDocument(UserDocumentsViewModel document)
        //{
        //    int returnId = 0;
        //    string strCompanyId = GetActorClaims();
        //    int CompanyId = Convert.ToInt32(strCompanyId);
        //    returnId = _fileUploaderService.UploadSelectedFile(document, CompanyId);
        //    if (returnId > 0)
        //    {
        //        ViewData["Message"] = "Document Uploaded Successfully..!!";
        //    }
        //    else
        //    {
        //        ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
        //    }
        //    return RedirectToAction("Index", "UserDocuments", new {message=returnId});
        //}
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadSelectedDocument(int Id)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string strCompanyId = GetActorClaims();
            int CompanyId = Convert.ToInt32(strCompanyId);
            UserDocumentsViewModel document = new UserDocumentsViewModel();
            document = _fileUploaderService.DownloadCompanyDocuments(Id, CompanyId);
            return File(document.Data, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName+document.Extension);
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
    }
}
