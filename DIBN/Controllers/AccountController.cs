using DIBN.IService;
using DIBN.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DIBN.Models.AccountViewModel;

namespace DIBN.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IBannerImageService _bannerImageService;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserPermissionService _userPermissionService;

        public AccountController(IAccountService accountService, IBannerImageService bannerImageService, ILogger<AccountController> logger, IUserPermissionService userPermissionService)
        {
            _accountService = accountService;
            _bannerImageService = bannerImageService;
            _logger = logger;
            _userPermissionService = userPermissionService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            LoginViewModel model = new LoginViewModel();
           // model.banners = _bannerImageService.GetBanners();
            return View(model);
        }

        [HttpPost]
        public IActionResult UserLoginDetails(LoginViewModel login1)
        {
            return UserLogin(login1);
        }

        public IActionResult UserLogin(LoginViewModel login)
        {
            string previousActor = "", previousRole = "", previousUser = "";
            if (ModelState.IsValid)
            {
                List<string> returnResult = new List<string>();
                Log.Information("Started --Login");
                Regex re = new Regex(@"(@)(.+)$");
                Match result = re.Match(login.Email);
                if (result.Success)
                {
                    int id = _accountService.CheckSalesPerson(login.Email);
                    if (id > 0)
                    {
                        returnResult = _accountService.SalesPersonLogin(login);
                        if (returnResult != null && returnResult.Contains("-2"))
                        {
                            login.banners = _bannerImageService.GetBanners();
                            Log.Error("Error while login using Username=" + login.Email + ", Error Message :Something went wrong, Please ask Admin for Login.");
                            ModelState.AddModelError(login.Email, "Something went wrong, Please ask Admin for Login.");
                            return View("Login", login);
                        }
                        else
                        {
                            if (returnResult != null && !returnResult.Contains("-1") && returnResult.Count > 0)
                            {
                                var userIdentity = (ClaimsIdentity)User.Identity;
                                var claims = userIdentity.Claims;

                                var roleClaimType = userIdentity.RoleClaimType;
                                var role = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                                if (role != null && role.Count > 0)
                                {
                                    if (role.Count > 1 && role[0].Value != "DIBN")
                                    {
                                        userIdentity.RemoveClaim(role[0]);
                                        previousRole = role[1].Value;
                                    }
                                    else
                                    {
                                        previousRole = role[0].Value;
                                    }
                                }

                                var actorClaimType = userIdentity.Actor;
                                var actor = claims.Where(c => c.Type == ClaimTypes.Actor).ToList();
                                if (actor != null && actor.Count > 0)
                                {
                                    if (actor.Count > 1 && role[0].Value != "DIBN")
                                    {
                                        userIdentity.RemoveClaim(actor[0]);
                                        previousActor = actor[1].Value;
                                    }
                                    else
                                    {
                                        previousActor = actor[0].Value + "_DIBN";
                                    }
                                }

                                var userClaimType = userIdentity.NameClaimType;
                                var user = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
                                if (user != null && user.Count > 0)
                                {
                                    if (user.Count > 1 && role[0].Value != "DIBN")
                                    {
                                        userIdentity.RemoveClaim(user[0]);
                                        previousUser = user[1].Value;
                                    }
                                    else
                                    {
                                        previousUser = user[0].Value + "_DIBN";
                                    }
                                }

                                var userClaim = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, login.Email),
                                    new Claim(ClaimTypes.Name, previousUser),
                                    new Claim(ClaimTypes.Role,returnResult[0]),
                                    new Claim(ClaimTypes.Role,previousRole),
                                    new Claim(ClaimTypes.Actor, returnResult[1]),
                                    new Claim(ClaimTypes.Actor,previousActor)
                                };
                                var personIdentity = new ClaimsIdentity(userClaim, "identitycard");
                                var principle = new ClaimsPrincipal(new[] { personIdentity });

                                var loginData = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);

                                HttpContext.Session.SetString("UserEmail", login.Email);

                                if (previousRole == "")
                                {
                                    if (returnResult[0] == "DIBN" && previousRole == "DIBN")
                                    {
                                        Log.Information("Login using Username=" + login.Email);
                                        return RedirectToAction("Index", "Home", new { area = "admin", name = "HomePage" });
                                    }
                                    else if (returnResult[0] != "DIBN" && previousRole == "DIBN")
                                    {
                                        Log.Information("Login using Username=" + login.Email);
                                        return RedirectToAction("Index", "Home", new { area = "admin",name="HomePage" });
                                    }
                                    else if (returnResult[0] == "DIBN")
                                    {
                                        Log.Information("Login using Username=" + login.Email);
                                        return RedirectToAction("Index", "Home", new { area = "admin", name = "HomePage" });
                                    }

                                }
                                if (returnResult[1] == "" || returnResult[1] == null)
                                {
                                    login.banners = _bannerImageService.GetBanners();
                                    Log.Error("Error while login using Username=" + login.Email + ", Error Message :Please wait till admin assign you a Company.");
                                    ModelState.AddModelError(login.Email, "Please wait till admin assign you a Company.");
                                    return View("Login", login);
                                }
                                Log.Information("Login using Username=" + login.Email);
                                return RedirectToAction("Index", "Home", new { name = "HomePage" });
                            }
                            else
                            {
                                login.banners = _bannerImageService.GetBanners();
                                Log.Error("Error while login using Username=" + login.Email + ", Error Message :Your Username or Password were wrong.Please try Again.");
                                ModelState.AddModelError("Email", "Your Username or Password were wrong.Please try Again.");
                                return View("Login", login);
                            }
                        }
                    }
                    else
                    {
                        login.banners = _bannerImageService.GetBanners();
                        Log.Error("Error while login using Username=" + login.Email + ", Error Message :Please Provide Your Company Code/User Code and Password.");
                        ModelState.AddModelError(login.Email, "Please Provide Your Company Code/User Code and Password.");
                        return View("Login", login);
                    }
                }
                else
                {
                    returnResult = _accountService.Login(login);
                    if (returnResult != null && returnResult.Contains("-2"))
                    {
                        login.banners = _bannerImageService.GetBanners();
                        Log.Error("Error while login using Username=" + login.Email + ", Error Message :Something went wrong, Please ask Admin for Login.");
                        ModelState.AddModelError(login.Email, "Something went wrong, Please ask Admin for Login.");
                        return View("Login", login);
                    }
                    else
                    {
                        if (returnResult != null && !returnResult.Contains("-1") && returnResult.Count > 0)
                        {
                            var userIdentity = (ClaimsIdentity)User.Identity;
                            var claims = userIdentity.Claims;

                            var roleClaimType = userIdentity.RoleClaimType;
                            var role = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                            if (role != null && role.Count > 0)
                            {
                                if (role.Count > 1 && role[0].Value != "DIBN")
                                {
                                    userIdentity.RemoveClaim(role[0]);
                                    previousRole = role[1].Value;
                                }
                                else
                                {
                                    previousRole = role[0].Value;
                                }
                            }

                            var actorClaimType = userIdentity.Actor;
                            var actor = claims.Where(c => c.Type == ClaimTypes.Actor).ToList();
                            if (actor != null && actor.Count > 0)
                            {
                                if (actor.Count > 1 && role[0].Value != "DIBN")
                                {
                                    userIdentity.RemoveClaim(actor[0]);
                                    previousActor = actor[1].Value;
                                }
                                else
                                {
                                    previousActor = actor[0].Value + "_DIBN";
                                }
                            }

                            var userClaimType = userIdentity.NameClaimType;
                            var user = claims.Where(c => c.Type == ClaimTypes.Name).ToList();
                            if (user != null && user.Count > 0)
                            {
                                if (user.Count > 1 && role[0].Value != "DIBN")
                                {
                                    userIdentity.RemoveClaim(user[0]);
                                    previousUser = user[1].Value;
                                }
                                else
                                {
                                    previousUser = user[0].Value + "_DIBN";
                                }
                            }

                            var userClaim = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, login.Email),
                                new Claim(ClaimTypes.Name, previousUser),
                                new Claim(ClaimTypes.Role,returnResult[0]),
                                new Claim(ClaimTypes.Role,previousRole),
                                new Claim(ClaimTypes.Actor, returnResult[1]),
                                new Claim(ClaimTypes.Actor,previousActor)
                            };

                            var personIdentity = new ClaimsIdentity(userClaim, "identitycard");
                            var principle = new ClaimsPrincipal(new[] { personIdentity });

                            var loginData = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);

                            HttpContext.Session.SetString("UserEmail", login.Email);

                            if (previousRole == "")
                            {
                                if(login.Email=="DIBN10" && login.Password == "meryna@123")
                                {
                                    Log.Information("Login using Username=" + login.Email);
                                    return RedirectToAction("WebsiteEnquiries", "Home", new { area = "admin", name = "Enquiry" });
                                }
                                else
                                {
                                    if (returnResult[0] == "DIBN" && previousRole == "DIBN")
                                    {
                                        Log.Information("Login using Username=" + login.Email);
                                        return RedirectToAction("Index", "Home", new { area = "admin", name = "HomePage" });
                                    }
                                    else if (returnResult[0] != "DIBN" && previousRole == "DIBN")
                                    {
                                        Log.Information("Login using Username=" + login.Email);
                                        return RedirectToAction("Index", "Home", new { area = "admin", name = "HomePage" });
                                    }
                                    else if (returnResult[0] == "DIBN")
                                    {
                                        Log.Information("Login using Username=" + login.Email);
                                        return RedirectToAction("Index", "Home", new { area = "admin", name = "HomePage" });
                                    }
                                }
                            }
                            if (login.Email == "DIBN10" && login.Password == "meryna@123")
                            {
                                Log.Information("Login using Username=" + login.Email);
                                return RedirectToAction("WebsiteEnquiries", "Home", new { area = "admin", name = "Enquiry" });
                            }
                            else
                            {
                                if (returnResult[0] == "DIBN")
                                {
                                    Log.Information("Login using Username=" + login.Email);
                                    return RedirectToAction("Index", "Home", new { area = "admin", name = "HomePage" });
                                }
                                if (returnResult[1] == "" || returnResult[1] == null)
                                {
                                    login.banners = _bannerImageService.GetBanners();
                                    Log.Error("Error while login using Username=" + login.Email + ", Error Message :Please wait till admin assign you a Company.");
                                    ModelState.AddModelError(login.Email, "Please wait till admin assign you a Company.");
                                    return View("Login", login);
                                }
                                Log.Information("Login using Username=" + login.Email);
                                return RedirectToAction("Index", "Home", new { name = "HomePage" });

                            }
                            
                        }
                        else
                        {
                            login.banners = _bannerImageService.GetBanners();
                            Log.Error("Error while login using Username=" + login.Email + ", Error Message :Your Username or Password were wrong.Please try Again.");
                            ModelState.AddModelError("Email", "Your Username or Password were wrong.Please try Again.");
                            return View("Login", login);
                        }
                    }
                }

                return View("Login", login);
            }
            else
            {
                login.banners = _bannerImageService.GetBanners();
                Log.Error("Error while login using Username=" + login.Email + ", Error Message :Your Username or Password were wrong.Please try Again.");
                ModelState.AddModelError("Email", "Your Username or Password were wrong.Please try Again.");
                return View("Login", login);
            }

        }
        [HttpGet]
        public IActionResult Logout()//int? hasUser
        {
            //if(hasUser != null && Convert.ToInt32(hasUser)>1)
            //{
            //    var userIdentity = (ClaimsIdentity)User.Identity;
            //    var claims = userIdentity.Claims;
            //    MainUser(userIdentity);
            //}
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
            return RedirectToAction("Login", "Account");
        }
        //[HttpGet]
        //public void MainUser(ClaimsIdentity claimsIdentity)//int companyId,string email
        //{
        //    var principle = new ClaimsPrincipal(new[] { claimsIdentity });
        //    var loginData = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
        //}
        [HttpGet]
        public IActionResult LockScreen()
        {
            return View();
        }
        [HttpGet]
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword(int? message)
        {
            if (message != null)
            {
                if (message <= 0)
                {
                    ViewData["Message"] = "Please Provide Valid Email Address.";
                }
                else
                {
                    ViewData["Message"] = "Email Send Successfully with Instructions to Change Your Password.";
                }
            }

            return View();
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string Email)
        {
            List<string> _user = new List<string>();
            _user = _accountService.CheckExistanceOfEmail(Email);
            if (_user.Count > 0)
            {
                if (Convert.ToInt32(_user[0]) > 0)
                {
                    bool _host = HttpContext.Request.IsHttps;
                    string host = _host ? "https://" + HttpContext.Request.Host.Value : "http://" + HttpContext.Request.Host.Value;

                    string url = Url.Action("ChangePassword", "Account", new { User = Email, Id = _user[0] });
                    url = host + url;

                    string result = await _accountService.sendMail(_user[1], Email, url);
                    return RedirectToAction("ForgotPassword", "Account", new { message = Convert.ToInt32(result) });
                }
                else
                {
                    return RedirectToAction("ForgotPassword", "Account", new { message = Convert.ToInt32(_user[0]) });
                }
            }
            else
            {
                return RedirectToAction("ForgotPassword", "Account", new { message = _user.Count });
            }
        }

        [HttpGet]
        [Route("ChangePassword")]
        public IActionResult ChangePassword(string User, int Id)
        {
            string _accountType = _accountService.GetAccountType(User);
            ChangePasswordModel model = new ChangePasswordModel();
            model.AccountType = _accountType;
            model.User = User;
            model.Id = Id;
            return View(model);
        }

        [HttpGet]
        [Route("ChangePasswords")]
        public IActionResult ChangePasswords(ChangePasswordModel model)
        {
            int _returnId = 0;
            _returnId = _accountService.ChangePassword(model);
            return RedirectToAction("Login", "Account");
        }

    }
}
