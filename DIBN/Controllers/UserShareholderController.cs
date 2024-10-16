using ClosedXML.Excel;
using DIBN.IService;
using DIBN.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserShareholderController : Controller
    {
        private readonly IUserShareholderService _userShareholderService;
        private readonly IUserCompanyService _userCompanyService;
        private readonly IUserPermissionService _userPermissionService;
        public UserShareholderController(IUserShareholderService userShareholderService, IUserCompanyService userCompanyService, IUserPermissionService userPermissionService)
        {
            _userShareholderService = userShareholderService;
            _userCompanyService = userCompanyService;
            _userPermissionService = userPermissionService;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            UserCompanyViewModel company = new UserCompanyViewModel();
            company = _userCompanyService.GetCompanyById(CompanyId);
            List<ShareholderViewModel> shareholders = new List<ShareholderViewModel>();
            shareholders = _userShareholderService.GetAllShareholders(CompanyId);
            
            foreach (var shareholder in shareholders)
            {
                shareholder.Module = name;
                shareholder.CompanyName = company.CompanyName;
                shareholder.CompanyType = company.CompanyType;
            }
            if(shareholders==null || shareholders.Count == 0)
            {
                ShareholderViewModel model = new ShareholderViewModel();
                model.CompanyName = company.CompanyName;
                model.CompanyType = company.CompanyType;
                model.Module = name;
                shareholders.Add(model);
            }
            return View(shareholders);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUploadDocuments(string? name,int Id, int? DocumentId)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            UserCompanyViewModel company = new UserCompanyViewModel();
            company = _userCompanyService.GetCompanyById(CompanyId);
            List<ShareholderDocuments> documents = new List<ShareholderDocuments>();
            documents = _userShareholderService.GetShareholdersDocuments(Id,DocumentId);
            if (documents.Count > 0)
            {
                foreach (var item in documents)
                {
                    item.Module = name;
                    item.ShareholderId = Id;
                    item.CompanyName = company.CompanyName;
                    item.CompanyType = company.CompanyType;
                }
            }
            else
            {
                ShareholderDocuments document = new ShareholderDocuments();
                document.Module = name;
                document.ShareholderId = Id;
                document.CompanyName = company.CompanyName;
                document.CompanyType = company.CompanyType;
                documents.Add(document);
            }

            return View(documents);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadSelectedDocument(int Id,int? DocumentId)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            List<ShareholderDocuments> document = new List<ShareholderDocuments>();
            document = _userShareholderService.GetShareholdersDocuments(Id,DocumentId);
            byte[] data = null;
            string Filename = null, Extension = null;
            foreach (var item in document)
            {
                string Files = item.FileName;
                //System.IO.File.WriteAllBytes(Files, item.DataBinary);
                data = item.DataBinary;
                Filename = item.FileName;
                Extension = item.Extension;
                MemoryStream ms = new MemoryStream(item.DataBinary);
            }

            return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, Filename + Extension);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ExportShareholdersToExcel(int Id)
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
                    dt = _userShareholderService.GetShareholdersForExport(Id).Tables[0];
                    wb.Worksheets.Add(dt);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ShareholderOrDirectors.xlsx");
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
        public IActionResult ExportShareholdersToPdf(int Id)
        {
            try
            {
                string Role = GetClaims();
                if (Role == null || Role == "")
                {
                    return RedirectToAction("Logout", "Account");
                }
                DataTable dt = new DataTable();
                dt = _userShareholderService.GetShareholdersForExport(Id).Tables[0];
                byte[] filecontent = exportpdf(dt);
                return File(filecontent, "application/pdf", "ShareholderOrDirectors.pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Obsolete]
        private byte[] exportpdf(DataTable dataTable)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.SetMargins(0f, 0f, 5f, 5f);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open(); 
            Paragraph prgGeneratedBY = new Paragraph();
            BaseFont btnAuthor = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fntAuthor = new iTextSharp.text.Font(btnAuthor, 8, 2, iTextSharp.text.BaseColor.BLUE);
            prgGeneratedBY.Alignment = Element.ALIGN_RIGHT;
            doc.Add(prgGeneratedBY);

            //Adding  PdfPTable  
            PdfPTable table = new PdfPTable(dataTable.Columns.Count);
            table.SetWidths(new float[] { 1f, 1f, 0.5f, 1f, 1f, 0.5f, 1f, 1f, 2f, 0.8f });
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

            byte[] result = ms.ToArray();
            return result;

        }
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
