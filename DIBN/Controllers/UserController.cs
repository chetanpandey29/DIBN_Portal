using ClosedXML.Excel;
using DIBN.IService;
using DIBN.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using static DIBN.Models.UserEmployeeViewModel;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserController : Controller
    {
        private readonly IUserEmployeesService _userEmployeesService;
        private readonly IUserPermissionService _userPermissionService;
        private readonly IFileUploaderService _fileUploaderService;

        public UserController(IUserEmployeesService userEmployeesService, IFileUploaderService fileUploaderService, IUserPermissionService userPermissionService)
        {
            _userEmployeesService = userEmployeesService;
            _fileUploaderService = fileUploaderService;
            _userPermissionService = userPermissionService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            string strcompany = GetActorClaims();
            int companyId = Convert.ToInt32(strcompany);

            string userId = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(userId);

            string role = GetClaims();
            if(role==null || role == "")
            {
                return RedirectToAction("Logout", "Account");
            }

            GetEmployeesForCompany getEmployeesForCompany = new GetEmployeesForCompany();
            getEmployeesForCompany.getActiveEmployees = new List<GetActiveEmployees>();
            getEmployeesForCompany.getInActiveEmployees = new List<GetInActiveEmployees>();
            getEmployeesForCompany.getUserDetails = new List<UserEmployeeViewModel>();
            if (role != "Employee")
            {
                getEmployeesForCompany.getActiveEmployees = _userEmployeesService.GetAllActiveEmployees(companyId);
                getEmployeesForCompany.getInActiveEmployees = _userEmployeesService.GetAllInActiveEmployees(companyId);
            }
            else
            {
                UserEmployeeViewModel user = new UserEmployeeViewModel();
                user = _userEmployeesService.GetUserDetail(UserId);
                user.Module = name;
                getEmployeesForCompany.getUserDetails.Add(user);
            }
            getEmployeesForCompany.Role = role;
            getEmployeesForCompany.Module = name;
            return View(getEmployeesForCompany);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult UserDetail(int UserId, string? name)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                UserEmployeeViewModel user = new UserEmployeeViewModel();
                user = _userEmployeesService.GetUserDetail(UserId);
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
        public IActionResult GetMyDocuments(int? Id,string? Type,string? name)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            GetUserDocuments getUserDocuments = new GetUserDocuments();
            string strcompany = GetActorClaims();
            int companyId = Convert.ToInt32(strcompany);
           
            List<UserDocumentsViewModel> documents = new List<UserDocumentsViewModel>();
            documents = _fileUploaderService.GetAllDocumentsByUserId(companyId, (int)Id);
            getUserDocuments.EmployeeId = (int)Id;
            getUserDocuments.UserType = Type;
            getUserDocuments.CompanyId = companyId;
            getUserDocuments.Module = name;
            getUserDocuments.Documents = documents;
            return View(getUserDocuments);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetMyDocumentsOfEmployee(GetUserDocuments Userdocuments)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            List<UserDocumentsViewModel> documents = new List<UserDocumentsViewModel>();
            documents = _fileUploaderService.GetAllDocumentOfEmployee(Userdocuments.CompanyId, Userdocuments.EmployeeId, Userdocuments.SelectedDocumentId);
            Userdocuments.Documents = documents;
            return View("GetMyDocuments", Userdocuments);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadSelectedDocument(int Id, int CompanyId)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            UserDocumentsViewModel document = new UserDocumentsViewModel();
            document = _fileUploaderService.DownloadDocument(Id, CompanyId);
            string Files = document.FileName;
            //System.IO.File.WriteAllBytes(Files, document.Data);
            MemoryStream ms = new MemoryStream(document.Data);
            return File(document.Data, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName + document.Extension);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ExportToExcel(int Id)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    DataTable dt = new DataTable();
                    dt = _userEmployeesService.GetEmployeesForExport(Id).Tables[0];
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
        public IActionResult ExportToPdf(int Id)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                DataTable dt = new DataTable();
                dt = _userEmployeesService.GetEmployeesForExport(Id).Tables[0];
                byte[] filecontent = exportpdf(dt);
                return File(filecontent, "application/pdf", "Employees.pdf");
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
            doc.SetMargins(0f, 0f, 5f, 5f);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();
            //Adding paragraph for report generated by  
            Paragraph prgGeneratedBY = new Paragraph();
            prgGeneratedBY.Alignment = Element.ALIGN_RIGHT;
            doc.Add(prgGeneratedBY);

            //Adding  PdfPTable  
            PdfPTable table = new PdfPTable(dataTable.Columns.Count);
            table.SetWidths(new float[] { 0.8f, 1f, 1f, 1.5f, 1f, 1.5f, 1f, 1f, 0.8f, 0.8f, 1f, 1f, 0.8f });
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

        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult Create(string? name)
        //{
        //    string strcompany = GetActorClaims();
        //    int companyId = Convert.ToInt32(strcompany);
        //    UserEmployeeViewModel user = new UserEmployeeViewModel();
        //    List<SelectListItem> roles = new List<SelectListItem>();
        //    var role = _userEmployeesService.GetActiveRoles();

        //    for (int i = 0; i < role.Count; i++)
        //    {
        //        roles.Add(new SelectListItem
        //        {
        //            Text = role[i].RoleName,
        //            Value = role[i].RoleID.ToString()
        //        });
        //    }
        //    user.Roles = roles;
        //    user.Module = name;
        //    user.CompanyId = companyId;
        //    return View(user);
        //}
        //[HttpPost]
        //[Route("[action]")]
        //public IActionResult Creates(UserEmployeeViewModel user)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            int id = 0;
        //            int returnId = 0;
        //            int emailExsitance = 0;
        //            returnId = _userEmployeesService.CheckExistanceOfUserAccountNumber(user.AccountNumber);
        //            emailExsitance = _userEmployeesService.CheckExistanceOfEmail(user.EmailID);
        //            if (returnId > 0 && emailExsitance > 0)
        //            {
        //                id = _userEmployeesService.CreateUser(user);
        //                return RedirectToAction("Index", "User", new { name = user.Module });
        //            }
        //            else
        //            {
        //                if (returnId == -1)
        //                    ModelState.AddModelError(user.AccountNumber, user.AccountNumber + " already Exists.");
        //                if (emailExsitance == -1)
        //                    ModelState.AddModelError(user.EmailID, user.EmailID + " already Exists.");
        //                List<SelectListItem> roleList = new List<SelectListItem>();
        //                var getRoles = _userEmployeesService.GetActiveRoles();
        //                for (int i = 0; i < getRoles.Count; i++)
        //                {
        //                    roleList.Add(new SelectListItem
        //                    {
        //                        Text = getRoles[i].RoleName,
        //                        Value = getRoles[i].RoleID.ToString()
        //                    });
        //                }
        //                user.Roles = roleList;
        //                return View("Create", user);
        //            }

        //        }
        //        List<SelectListItem> roles = new List<SelectListItem>();
        //        var role = _userEmployeesService.GetActiveRoles();
        //        for (int i = 0; i < role.Count; i++)
        //        {
        //            roles.Add(new SelectListItem
        //            {
        //                Text = role[i].RoleName,
        //                Value = role[i].RoleID.ToString()
        //            });
        //        }
        //        user.Roles = roles;
        //        return View("Create", user);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult Edit(int Id, string? name)
        //{
        //    try
        //    {
        //        string strcompany = GetActorClaims();
        //        int companyId = Convert.ToInt32(strcompany);
        //        UserEmployeeViewModel user = new UserEmployeeViewModel();
        //        List<SelectListItem> roles = new List<SelectListItem>();
        //        var role = _userEmployeesService.GetActiveRoles();

        //        for (int i = 0; i < role.Count; i++)
        //        {
        //            roles.Add(new SelectListItem
        //            {
        //                Text = role[i].RoleName,
        //                Value = role[i].RoleID.ToString()
        //            });
        //        }
        //        user = _userEmployeesService.GetUserDetail(Id);
        //        user.Roles = roles;
        //        user.Module = name;
        //        user.CompanyId = companyId;
        //        return View(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        //[HttpPost]
        //[Route("[action]")]
        //public IActionResult Edits(UserEmployeeViewModel user)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            int id = 0;
        //            id = _userEmployeesService.UpdateUser(user);
        //            return RedirectToAction("Index", "User", new { name = user.Module });
        //        }
        //        List<SelectListItem> roles = new List<SelectListItem>();
        //        var role = _userEmployeesService.GetActiveRoles();
        //        for (int i = 0; i < role.Count; i++)
        //        {
        //            roles.Add(new SelectListItem
        //            {
        //                Text = role[i].RoleName,
        //                Value = role[i].RoleID.ToString()
        //            });
        //        }
        //        user.Roles = roles;
        //        return View("Edit", user);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult Delete(int Id)
        //{
        //    try
        //    {
        //        int returnId = 0;
        //        returnId = _userEmployeesService.DeleteUser(Id);
        //        return new JsonResult(returnId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult CheckAccountNumberExistance(string AccountNumber)
        //{
        //    int returnId = 0;
        //    returnId = _userEmployeesService.CheckExistanceOfUserAccountNumber(AccountNumber);
        //    return new JsonResult(returnId);
        //}

        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult CheckEmailExistance(string Email)
        //{
        //    int returnId = 0;
        //    returnId = _userEmployeesService.CheckExistanceOfEmail(Email);
        //    return new JsonResult(returnId);
        //}

        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult GetAccountNumber()
        //{
        //    string accountNumber = _userEmployeesService.GetLastAccountNumber();
        //    Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
        //    Match result = re.Match(accountNumber);
        //    string alphaPart = result.Groups[1].Value;
        //    int numberPart = Convert.ToInt32(result.Groups[2].Value);
        //    int Id = numberPart + 1;
        //    accountNumber = alphaPart + Id.ToString();
        //    return new JsonResult(accountNumber);
        //}
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
