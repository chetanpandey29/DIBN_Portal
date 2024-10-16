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
    public class BannerController : Controller
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IPermissionRepository _permissionRepository;

        public BannerController(IBannerRepository bannerRepository, IPermissionRepository permissionRepository)
        {
            _bannerRepository = bannerRepository;
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? message)
        {
            List<BannerViewModel> banners = new List<BannerViewModel>();
            banners = _bannerRepository.GetBanners();
            if (banners.Count > 0)
            {
                foreach (var item in banners)
                {
                    item.Module = name;
                }
            }
            else
            {
                BannerViewModel model = new BannerViewModel();
                model.Module = name;
                banners.Add(model);
            }
            if (message > 0)
            {
                ViewData["Message"] = "Image Uploaded Successfully..!!";
            }
            else if (message < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Image. Please try again later..!!";
            }
            return View(banners);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadBanner(BannerViewModel banner)
        {
            int returnId = 0;
            string _user = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(_user);
            banner.UserId = UserId;
            returnId = _bannerRepository.CreateNewBanner(banner);
            if (returnId > 0)
            {
                ViewData["Message"] = "Image Uploaded Successfully..!!";
            }
            else if (returnId < 0)
            {
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Image. Please try again later..!!";
            }
            return RedirectToAction("Index", "Banner", new { message = returnId, name = "Banner" });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult ActiveBanner(int Id, bool IsActive)
        {
            int returnId = 0;
            string _user = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(_user);
            BannerViewModel banner = new BannerViewModel();
            banner.Id = Id;
            banner.IsActive = IsActive;
            banner.UserId = UserId;
            returnId = _bannerRepository.UpdateBanner(banner);
            return new JsonResult(returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DeActiveBanner(int Id, bool IsActive)
        {
            int returnId = 0;
            string _user = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(_user);
            BannerViewModel banner = new BannerViewModel();
            banner.Id = Id;
            banner.IsActive = IsActive;
            banner.UserId = UserId;
            returnId = _bannerRepository.DeActivateBanner(banner);
            return new JsonResult(returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DeleteBanner(int Id)
        {
            int returnId = 0;
            string _user = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(_user);
            returnId = _bannerRepository.DeleteBanner(Id, UserId);
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
