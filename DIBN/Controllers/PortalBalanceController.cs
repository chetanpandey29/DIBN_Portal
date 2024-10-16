using DIBN.IService;
using DIBN.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class PortalBalanceController : Controller
    {
        private readonly IUserPermissionService _userPermissionService;
        private readonly IPortalBalanceService _portalBalanceService;
        public PortalBalanceController(IUserPermissionService userPermissionService, IPortalBalanceService portalBalanceService)
        {
            _userPermissionService = userPermissionService;
            _portalBalanceService = portalBalanceService;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetPortalBalance()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            PortalBalanceViewModel model = new PortalBalanceViewModel();
            model = _portalBalanceService.GetPortalBalance(CompanyId);
            return new JsonResult(model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddPortalBalance(string Amount)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);

            string Username = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(Username);

            _returnId = _portalBalanceService.AddPortalBalance(Amount,CompanyId,UserId);    
            return RedirectToAction("Index", "Home", new {name="HomePage"});
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetExpenses()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            List<PortalBalanceExpenses> expenses = new List<PortalBalanceExpenses>();
            string Company = GetActorClaims();
            int CompanyId = Convert.ToInt32(Company);
            expenses = _portalBalanceService.GetAllExpensesForCompany(CompanyId);
            return new JsonResult(expenses);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTransactions()
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string FromDate = null, ToDate = null;
            string _company = GetActorClaims();
            int CompanyId = Convert.ToInt32(_company);
            CompanyAccountDetails model = new CompanyAccountDetails();
            model = _portalBalanceService.GetCompanyAccount(CompanyId, FromDate, ToDate);
            model.CompanyId = CompanyId;

            return View(model);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadReceipt(int ReceiptId)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            ExpenseReceipt document = new ExpenseReceipt();
            document = _portalBalanceService.GetCompanyExpenseReceipt(ReceiptId);
            string Files = document.FileName;
            //System.IO.File.WriteAllBytes(Files, document.DataBinary);
            MemoryStream ms = new MemoryStream(document.DataBinary);
            return File(document.DataBinary, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName + document.Extension);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadPaymentReceipt(int ReceiptId)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            GetPaymentTransactionReceipt model = new GetPaymentTransactionReceipt();
            model = _portalBalanceService.GetPaymentTransactionReceiptDetails(ReceiptId);

            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 88f, 88f, 10f, 10f);
            MemoryStream stream = new MemoryStream();
            try
            {
                long rupees = Convert.ToInt64(Convert.ToDecimal(model.Amount));
                string rupeesInwords = ConvertNumbertoWords(rupees);
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                Phrase phrase = null;
                PdfPCell cell = null;
                PdfPTable table = null;
                pdfWriter.CloseStream = false;
                //Header Table
                table = new PdfPTable(1);
                table.TotalWidth = 555f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 1f });
                table.DefaultCell.Padding = 0;
                table.HeaderRows = 1;
                pdfWriter.PageEvent = new ReceiptBorder();
                document.SetMargins(20f, 20f, 20f, 20f);
                document.Open();

                Font titleFont = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font headerFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font middleHeader = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font title = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font pdfTitleFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                string fontLoc = "wwwroot/PDF/Arial.ttf";
                BaseFont bf = BaseFont.CreateFont(fontLoc, BaseFont.IDENTITY_H, true);
                Font f = new Font(bf, 10);

                PdfPTable headerTable = new PdfPTable(1);
                headerTable.TotalWidth = 550f;
                headerTable.LockedWidth = true;

                // Logo
                string path = "wwwroot/DIBN_Logo.png";
                FileStream fs1 = new FileStream(path, FileMode.Open);
                iTextSharp.text.Image JPG = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Jpeg);
                JPG.ScalePercent(7.0f);
                JPG.PaddingTop = 5f;
                fs1.Close();
                phrase = new Phrase();
                phrase.Add(new Chunk("\n\n", titleFont));
                cell = new PdfPCell(phrase);
                cell = new PdfPCell(JPG);
                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = PdfPCell.NO_BORDER;
                cell.PaddingTop = 5f;
                cell.NoWrap = true;
                headerTable.AddCell(cell);

                // Address
                //phrase = new Phrase();
                //phrase.Add(new Chunk("DEVOTION CORPORATE SERVICES L.L.C\n", titleFont));
                //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                //cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                //cell.Border = PdfPCell.NO_BORDER;
                //cell.PaddingTop = 8.0f;
                //headerTable.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("1901 - Al Musalla Tower,\n", headerFont));
                //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                //cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                //cell.Border = PdfPCell.NO_BORDER;
                //cell.PaddingTop = 1.5f;
                //headerTable.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Near Al Fahidi Metro Station,\n", headerFont));
                //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                //cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                //cell.Border = PdfPCell.NO_BORDER;
                //cell.PaddingTop = 1.5f;
                //headerTable.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Bur Dubai\n", headerFont));
                //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                //cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                //cell.Border = PdfPCell.NO_BORDER;
                //cell.PaddingTop = 1.5f;
                //headerTable.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Emirate: Dubai\n", headerFont));
                //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                //cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                //cell.Border = PdfPCell.NO_BORDER;
                //cell.PaddingTop = 1.5f;
                //headerTable.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("TRN: 100341026100003\n", headerFont));
                //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                //cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                //cell.Border = PdfPCell.NO_BORDER;
                //cell.PaddingTop = 1.5f;
                //headerTable.AddCell(cell);

                //phrase = new Phrase();
                //phrase.Add(new Chunk("Email:info@devotionbusiness.com\n\n", headerFont));
                //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                //cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                //cell.Border = PdfPCell.NO_BORDER;
                //cell.PaddingTop = 1.5f;
                //headerTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Receipt Voucher\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = PdfPCell.NO_BORDER;
                cell.PaddingTop = 6.0f;
                headerTable.AddCell(cell);


                PdfPTable headerAddressTable = new PdfPTable(2);
                headerAddressTable.TotalWidth = 550f;
                headerAddressTable.LockedWidth = true;
                headerAddressTable.SetWidths(new float[] { 1.5f, 1f });

                // Address
                phrase = new Phrase();
                phrase.Add(new Chunk("No. : ", middleHeader));
                phrase.Add(new Chunk(model.SerialNumber, title));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = PdfPCell.NO_BORDER;
                headerAddressTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dated : ", middleHeader));
                phrase.Add(new Chunk(model.CreatedOn, title));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = PdfPCell.NO_BORDER;
                headerAddressTable.AddCell(cell);

                headerTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                headerTable.AddCell(headerAddressTable);

                table.DefaultCell.BorderColor = BaseColor.BLACK;
                table.DefaultCell.Border = Rectangle.BOX;
                table.DefaultCell.BorderWidth = 1;
                table.DefaultCell.Padding = 3;

                table.DefaultCell.Border = PdfPCell.BOTTOM_BORDER;

                table.AddCell(headerTable);

                PdfPTable amountTable = new PdfPTable(2);
                amountTable.TotalWidth = 550f;
                amountTable.LockedWidth = true;
                amountTable.SetWidths(new float[] { 2f, 1f });
                //amountTable.HeaderRows = 1;

                phrase = new Phrase();
                phrase.Add(new Chunk("Particulars", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 10.0f;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Amount", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 10.0f;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nAccount :", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.RIGHT_BORDER;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 0;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n AED " + model.Amount, titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n" + model.CompanyName, middleHeader));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.RIGHT_BORDER;
                cell.BorderWidth = 1;
                cell.PaddingTop = 0;
                cell.PaddingLeft = 50;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);


                phrase = new Phrase();
                phrase.Add(new Chunk("\n Through :", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.RIGHT_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n " + model.Through, middleHeader));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.RIGHT_BORDER;
                cell.BorderWidth = 1;
                cell.PaddingTop = 0;
                cell.PaddingLeft = 50;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n On Account of :", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.RIGHT_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n " + model.OnAccount, middleHeader));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.RIGHT_BORDER;
                cell.BorderWidth = 1;
                cell.PaddingTop = 0;
                cell.PaddingLeft = 50;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n Amount (in words) :", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.RIGHT_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                table.DefaultCell.Border = PdfPCell.BOTTOM_BORDER;
                table.DefaultCell.Padding = 0;

                phrase = new Phrase();
                phrase.Add(new Chunk("\n " + rupeesInwords, middleHeader));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.RIGHT_BORDER;
                cell.BorderWidth = 1;
                cell.PaddingTop = 0;
                cell.PaddingLeft = 50;
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n AED " + model.Amount, titleFont));
                phrase.Add(new Chunk("\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.TOP_BORDER;
                cell.BorderWidth = 1;
                cell.BorderColor = BaseColor.BLACK;
                cell.Padding = 3;
                amountTable.AddCell(cell);

                table.AddCell(amountTable);

                document.Add(table);

                document.Close();
                stream.Flush();
                stream.Position = 0;
                return File(stream.ToArray(), "application/pdf", "PaymentReceipt.pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTaskDetails(string prefix)
        {
            List<string> _tasks = new List<string>();
            _tasks = _portalBalanceService.GetTaskList(prefix);
            return new JsonResult(_tasks);
        }
        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }
        public static string ConvertNumbertoWords(long number)
        {
            if (number == 0)
                return "ZERO";
            if (number < 0)
                return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";

            if ((number / 1000000000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000000000) + " Billion ";
                number %= 1000000000;
            }

            if ((number / 10000000) > 0)
            {
                words += ConvertNumbertoWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000000) + " MILLION ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
                number %= 100;
            }
            if (number > 0)
            {
                if (words != "")
                    words += "AND ";
                var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
                var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            return words;
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
