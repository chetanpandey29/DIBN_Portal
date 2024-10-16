using DIBN.IService;
using DIBN.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DIBN.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CompanyAccountController : Controller
    {
        private readonly IUserCompanyService _userCompanyService;
        private readonly IPortalBalanceService _portalBalanceService;
        private readonly IUserPermissionService _userPermissionService;
        public CompanyAccountController(IPortalBalanceService portalBalanceService, IUserCompanyService userCompanyService, IUserPermissionService userPermissionService)
        {
            _portalBalanceService = portalBalanceService;
            _userCompanyService = userCompanyService;
            _userPermissionService = userPermissionService;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name,string? FromDate,string? ToDate)
        {
            string Role = GetClaims();
            if(Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _company = GetActorClaims();
            int CompanyId = Convert.ToInt32(_company);
            CompanyAccountDetails model = new CompanyAccountDetails();
            model = _portalBalanceService.GetCompanyAccount(CompanyId, FromDate,ToDate);
            model.CompanyId = CompanyId;
            model.Module = name;
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            return View(model);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetPaymentReceipts(string? name)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _company = GetActorClaims();
            int CompanyId = Convert.ToInt32(_company);
            GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
            transactionReceipt.Module = name;
            transactionReceipt.CompanyId = CompanyId;
            return View(transactionReceipt);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyPaymentReceiptsByCompanyId(int companyId)
        {
            try
            {
                string searchBy = "";
                GetPaymentReceiptsByCompanyIdWithPaginationModel model = new GetPaymentReceiptsByCompanyIdWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _portalBalanceService.GetPaymentReceiptsByCompanyIdWithPagination(companyId, skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalPaymentReceipts;
                totalRecord = model.totalPaymentReceipts;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getPaymentTransactions
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
        public IActionResult GetInvoices(string? name)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string _company = GetActorClaims();
            int CompanyId = Convert.ToInt32(_company);
            GetPIAndFinalInvoices model = new GetPIAndFinalInvoices();
            model.CompanyId = CompanyId;
            return View(model);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyProformaInvoicesWithPagination(int companyId)
        {
            try
            {
                string searchBy = "";
                GetCompanyProformaInvoiceByCompanyIdWithPaginationModel model = new GetCompanyProformaInvoiceByCompanyIdWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _userCompanyService.GetCompanyProformaInvoiceByCompanyIdWithPagination(companyId, skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalInvoices;
                totalRecord = model.totalInvoices;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.invoices
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
        public async Task<IActionResult> GetAllCompanyFinalInvoicesWithPagination(int companyId)
        {
            try
            {
                string searchBy = "";
                GetCompanyFinalInvoiceByCompanyIdWithPaginationModel model = new GetCompanyFinalInvoiceByCompanyIdWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _userPermissionService.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _userCompanyService.GetCompanyFinalInvoiceByCompanyIdWithPagination(companyId, skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalInvoices;
                totalRecord = model.totalInvoices;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.invoices
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
        public IActionResult ProformaInvoice(string InvoiceNumber)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            GetInvoiceDeatils getInvoiceDeatils = new GetInvoiceDeatils();
            getInvoiceDeatils = _userCompanyService.GetInvoiceDeatils(InvoiceNumber);
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 0f, 0f, 0f, 0f);
            MemoryStream stream = new MemoryStream();
            try
            {
                int i = 1;
                Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                Match result = re.Match(InvoiceNumber);
                string alphaPart = result.Groups[1].Value;
                long numberPart = Convert.ToInt64(result.Groups[2].Value);
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
                if (!InvoiceNumber.Contains("DIBN"))
                    pdfWriter.PageEvent = new Border();
                else
                    pdfWriter.PageEvent = new DIBNBorder();
                document.SetMargins(20f, 20f, 20f, 20f);
                document.Open();
                Font titleFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font headerFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font pdfTitleFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                string fontLoc = "wwwroot/PDF/Arial.ttf";
                BaseFont bf = BaseFont.CreateFont(fontLoc, BaseFont.IDENTITY_H, true);
                Font f = new Font(bf, 10);
                table.HeaderRows = 1;
                table.HeaderRows = 2;
                PdfPTable headerTable = new PdfPTable(1);
                headerTable.TotalWidth = 550f;
                headerTable.LockedWidth = true;

                // Logo
                if (!InvoiceNumber.Contains("DIBN"))
                {
                    string path = "wwwroot/dibn_business_image/CORPORATE SERVICE PROVIDER.png";
                    FileStream fs1 = new FileStream(path, FileMode.Open);
                    iTextSharp.text.Image JPG = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Jpeg);
                    JPG.ScalePercent(5.5f);
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
                }
                else
                {
                    string path = "wwwroot/PDF/DIBN_Logo.png";
                    FileStream fs1 = new FileStream(path, FileMode.Open);
                    iTextSharp.text.Image JPG = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Jpeg);
                    JPG.ScalePercent(5.5f);
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
                }

                PdfPTable headerAddressTable = new PdfPTable(2);
                headerAddressTable.TotalWidth = 550f;
                headerAddressTable.LockedWidth = true;
                headerAddressTable.SetWidths(new float[] { 1.5f, 1f });
                // Address
                if (!InvoiceNumber.Contains("DIBN"))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("DEVOTION CORPORATE SERVICES L.L.C\n", titleFont));
                    phrase.Add(new Chunk("1901 - Al Fahidi Heights Tower,\n", headerFont));
                    phrase.Add(new Chunk("Near Sharaf DG Merto Station exit-4,\n", headerFont));
                    phrase.Add(new Chunk("Bur Dubai\n", headerFont));
                    phrase.Add(new Chunk("Emirate: Dubai\n", headerFont));
                    phrase.Add(new Chunk("TRN: 100341026100003\n", headerFont));
                    phrase.Add(new Chunk("Email:info@devotionbusiness.com\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.Border = PdfPCell.NO_BORDER;
                    headerAddressTable.AddCell(cell);
                    // Address
                    phrase = new Phrase();
                    phrase.Add(new Chunk("ديفوشن لخدمات الشركات ش.ذ.م.م\n", f));
                    phrase.Add(new Chunk("1901 - برج الفهيدي هايتس ،\r\nبالقرب من محطة شرف دي جي ميرتو مخرج 4 ،\r\nبر دبي\r\nالإمارة: دبي", f));
                    phrase.Add(new Chunk("رقم التسجيل الضريبي: 100341026100003\n", f));
                    phrase.Add(new Chunk("البريد الإلكتروني: info@devotionbusiness.com\n", f));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    cell.Border = PdfPCell.NO_BORDER;
                    cell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                    headerAddressTable.AddCell(cell);
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("DIBN BUSINESS SERVICES\n", titleFont));
                    phrase.Add(new Chunk("1901 - Al Fahidi Heights Tower,\n", headerFont));
                    phrase.Add(new Chunk("Near Sharaf DG Merto Station exit-4,\n", headerFont));
                    phrase.Add(new Chunk("Bur Dubai\n", headerFont));
                    phrase.Add(new Chunk("Emirate: Dubai\n", headerFont));
                    phrase.Add(new Chunk("TRN: 100341026100003\n", headerFont));
                    phrase.Add(new Chunk("Email:info@dibnbusiness.com\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.Border = PdfPCell.NO_BORDER;
                    headerAddressTable.AddCell(cell);
                    // Address
                    phrase = new Phrase();
                    phrase.Add(new Chunk("ديبن لخدمات الأعمال\n", f));
                    phrase.Add(new Chunk("1901 - برج الفهيدي هايتس ،\r\nبالقرب من محطة شرف دي جي ميرتو مخرج 4 ،\r\nبر دبي\r\nالإمارة: دبي", f));
                    phrase.Add(new Chunk("رقم التسجيل الضريبي: 100341026100003\n", f));
                    phrase.Add(new Chunk("البريد الإلكتروني: info@dibnbusiness.com\n\n", f));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    cell.Border = PdfPCell.NO_BORDER;
                    cell.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                    headerAddressTable.AddCell(cell);
                }
                headerTable.DefaultCell.Border = Rectangle.NO_BORDER;
                headerTable.AddCell(headerAddressTable);

                //Pdf Title
                phrase = new Phrase();
                phrase.Add(new Chunk("PROFORMA INVOICE\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = PdfPCell.NO_BORDER;
                headerTable.AddCell(cell);

                table.DefaultCell.Border = PdfPCell.BOTTOM_BORDER;

                table.AddCell(headerTable);

                //Second Line of Pdf
                PdfPTable secondTable = new PdfPTable(2);
                secondTable.TotalWidth = 555f;
                secondTable.LockedWidth = true;
                secondTable.SetWidths(new float[] { 1f, 2f });
                secondTable.DefaultCell.Padding = 2f;

                PdfPTable childTable1 = new PdfPTable(1);
                phrase = new Phrase();
                phrase.Add(new Chunk("Bill To : ", titleFont));
                phrase.Add(new Chunk(getInvoiceDeatils.CompanyName + "\n", headerFont));
                phrase.Add(new Chunk("Emirate : " + getInvoiceDeatils.Location + "\n", headerFont));
                phrase.Add(new Chunk("Country : " + getInvoiceDeatils.Country + "\n", headerFont));
                phrase.Add(new Chunk("Place of Supply : " + getInvoiceDeatils.Location + " , " + getInvoiceDeatils.Country + "\n", headerFont));
                if (getInvoiceDeatils.InvoiceCreatedOn != "---" && getInvoiceDeatils.TRNCreationDate != "---")
                    if (getInvoiceDeatils.IsTRN == true && Convert.ToDateTime(getInvoiceDeatils.InvoiceCreatedOn) >= Convert.ToDateTime(getInvoiceDeatils.TRNCreationDate))
                        phrase.Add(new Chunk("TRN : " + getInvoiceDeatils.TRN + "\n", headerFont));

                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 3;
                childTable1.AddCell(cell);

                secondTable.AddCell(childTable1);

                PdfPTable childTable = new PdfPTable(2);

                phrase = new Phrase();
                phrase.Add(new Chunk("Invoice No.\n ", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 3;
                phrase.Add(new Chunk("" + InvoiceNumber + "\n", headerFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 3;
                childTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Date.\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 3;
                phrase.Add(new Chunk("" + getInvoiceDeatils.Date + "\n", headerFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 3;
                childTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Service : ", titleFont));
                phrase.Add(new Chunk(getInvoiceDeatils.Service + "\n", headerFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.Colspan = 2;
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 3;
                childTable.AddCell(cell);



                secondTable.AddCell(childTable);

                table.DefaultCell.Border = PdfPCell.BOX;

                table.AddCell(secondTable);

                PdfPTable productTable = new PdfPTable(4);
                productTable.TotalWidth = 555f;
                productTable.LockedWidth = true;
                productTable.SetWidths(new float[] { 0.5f, 4f, 1f, 0.5f });
                productTable.HeaderRows = 1;
                phrase = new Phrase();
                phrase.Add(new Chunk("SR No.\n\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 5;
                productTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Particulars.\n\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 5;
                productTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Amount.\n\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 5;
                productTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("VAT %.\n\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 5;
                productTable.AddCell(cell);

                int checkIndex = 13;

                int lastIndex = (getInvoiceDeatils.invoiceVatDetails != null && getInvoiceDeatils.invoiceVatDetails.Count > 0) ? -1 : getInvoiceDeatils.invoiceModels.Count - 1;

                for (int index = 0; index < getInvoiceDeatils.invoiceModels.Count; index++)
                {
                    if (checkIndex == i)
                    {
                        checkIndex = checkIndex + 13;

                        table.DefaultCell.Border = Rectangle.NO_BORDER;

                        table.AddCell(productTable);

                        document.Add(table);

                        table.Rows.Clear();

                        productTable.Rows.Clear();

                        float _currentPosition = pdfWriter.GetVerticalPosition(false);

                        float _padding = _currentPosition - (document.BottomMargin + 40);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.PaddingTop = _padding;
                        
                        cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                        cell.DisableBorderSide(Rectangle.TOP_BORDER);
                        productTable.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.PaddingTop = _padding;
                        
                        cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                        cell.DisableBorderSide(Rectangle.TOP_BORDER);
                        productTable.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.PaddingTop = _padding;
                        
                        
                        cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                        cell.DisableBorderSide(Rectangle.TOP_BORDER);
                        productTable.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.PaddingTop = _padding;
                        
                        cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                        cell.DisableBorderSide(Rectangle.TOP_BORDER);
                        productTable.AddCell(cell);

                        table.DefaultCell.Border = Rectangle.NO_BORDER;

                        table.AddCell(productTable);

                        productTable.WriteSelectedRows(0, -1, document.LeftMargin, _currentPosition, pdfWriter.DirectContent);

                        document.Add(table);

                        table.Rows.Clear();

                        productTable.Rows.Clear();

                        phrase = new Phrase();
                        phrase.Add(new Chunk("Continued..", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.Colspan = 4;
                        cell.Padding = 5;
                        cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                        cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                        cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                        productTable.AddCell(cell);

                        table.AddCell(productTable);

                        productTable.WriteSelectedRows(0, -1, document.TopMargin, document.BottomMargin + 18, pdfWriter.DirectContent);

                        document.Add(table);

                        table.Rows.Clear();

                        productTable.Rows.Clear();

                        phrase = new Phrase();
                        phrase.Add(new Chunk("SR No.\n\n", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.Padding = 5;
                        productTable.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("Particulars.\n\n", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.Padding = 5;
                        productTable.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("Amount.\n\n", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.Padding = 5;
                        productTable.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("VAT %.\n\n", titleFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                        cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.Padding = 5;
                        productTable.AddCell(cell);

                        table.DefaultCell.Border = PdfPCell.BOTTOM_BORDER;

                        table.AddCell(headerTable);

                        table.DefaultCell.Border = PdfPCell.BOX;

                        table.AddCell(secondTable);

                        document.NewPage();

                    }

                    phrase = new Phrase();
                    phrase.Add(new Chunk("" + i + ".\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    if (lastIndex != -1)
                    {
                        if (index != lastIndex)
                        {
                            cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                        }
                    }
                    else
                    {
                        cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                    }

                    cell.DisableBorderSide(Rectangle.TOP_BORDER);
                    cell.BorderWidth = 1;
                    cell.Padding = 8;
                    productTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("" + getInvoiceDeatils.invoiceModels[index].ProductName + "\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 8;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                    cell.DisableBorderSide(Rectangle.TOP_BORDER);
                    productTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("" + getInvoiceDeatils.invoiceModels[index].TotalAmount + "\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.Padding = 8;
                    cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                    cell.DisableBorderSide(Rectangle.TOP_BORDER);
                    productTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("" + getInvoiceDeatils.invoiceModels[index].Vat != "0" ? getInvoiceDeatils.invoiceModels[index].Vat + "%" : "" + "\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.Padding = 8;
                    cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                    cell.DisableBorderSide(Rectangle.TOP_BORDER);
                    productTable.AddCell(cell);

                    i++;
                }
                if (getInvoiceDeatils.invoiceVatDetails != null && getInvoiceDeatils.invoiceVatDetails.Count > 0)
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("" + i + ".\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.Padding = 8;
                    cell.DisableBorderSide(Rectangle.TOP_BORDER);
                    cell.BorderWidth = 1;

                    productTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Output Vat @A/C\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.Padding = 8;
                    cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                    cell.DisableBorderSide(Rectangle.TOP_BORDER);
                    productTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("" + getInvoiceDeatils.VatAmount + "\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.Padding = 8;
                    cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                    cell.DisableBorderSide(Rectangle.TOP_BORDER);
                    productTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.Padding = 8;
                    cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                    cell.DisableBorderSide(Rectangle.TOP_BORDER);
                    productTable.AddCell(cell);
                }
                phrase = new Phrase();
                phrase.Add(new Chunk("\n\n", headerFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 5;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                productTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("Total\n\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 5;
                productTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk(getInvoiceDeatils.Currency+" " + getInvoiceDeatils.GrandTotal + "\n\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 5;
                productTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n\n", headerFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 5;
                productTable.AddCell(cell);
                table.DefaultCell.Border = PdfPCell.BOX;
                table.AddCell(productTable);

                string[] amount = getInvoiceDeatils.GrandTotal.Split(".");
                string[] vatAmount = { }; string taxrupeesInwords = "";
                if (getInvoiceDeatils.VatAmount != null && getInvoiceDeatils.VatAmount != "0.00" && getInvoiceDeatils.VatAmount != "")
                {
                    vatAmount = getInvoiceDeatils.VatAmount.Split(".");
                    taxrupeesInwords = ConvertNumbertoWords(Convert.ToInt64(vatAmount[0]));
                }

                string rupeesInwords = ConvertNumbertoWords(Convert.ToInt64(amount[0]));



                PdfPTable MainLastTable = new PdfPTable(1);
                MainLastTable.TotalWidth = 555f;
                MainLastTable.LockedWidth = true;
                MainLastTable.LockedWidth = true;
                MainLastTable.SetWidths(new float[] { 1f });

                PdfPTable LastTable = new PdfPTable(2);
                LastTable.TotalWidth = 555f;
                LastTable.LockedWidth = true;
                LastTable.SetWidths(new float[] { 3f, 2f });
                LastTable.DefaultCell.PaddingTop = 10f;
                phrase = new Phrase();
                phrase.Add(new Chunk("Amount Chargeable (in words)\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                if (getInvoiceDeatils.Currency == "AED")
                {
                    phrase.Add(new Chunk("UAE Dirham "
                    + rupeesInwords
                    + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.GrandTotal + ")\n\n", headerFont));
                }
                else if (getInvoiceDeatils.Currency == "USD")
                {
                    phrase.Add(new Chunk("USD "
                    + rupeesInwords
                    + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.GrandTotal + ")\n\n", headerFont));
                }
                else if (getInvoiceDeatils.Currency == "INR")
                {
                    phrase.Add(new Chunk("Indian Rupees "
                    + rupeesInwords
                    + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.GrandTotal + ")\n\n", headerFont));
                }
                else if (getInvoiceDeatils.Currency == "CAD")
                {
                    phrase.Add(new Chunk("CAD "
                    + rupeesInwords
                    + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.GrandTotal + ")\n\n", headerFont));
                }
                else if (getInvoiceDeatils.Currency == "GBP")
                {
                    phrase.Add(new Chunk("GBP "
                    + rupeesInwords
                    + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.GrandTotal + ")\n\n", headerFont));
                }

                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                if (taxrupeesInwords != null && taxrupeesInwords != "")
                {
                    phrase.Add(new Chunk("VAT Amount in words\n", titleFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;

                    if (getInvoiceDeatils.Currency == "AED")
                    {
                        phrase.Add(new Chunk("UAE Dirham "
                        + taxrupeesInwords
                        + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.VatAmount + ")\n\n", headerFont));
                    }
                    else if (getInvoiceDeatils.Currency == "USD")
                    {
                        phrase.Add(new Chunk("USD "
                        + taxrupeesInwords
                        + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.VatAmount + ")\n\n", headerFont));
                    }
                    else if (getInvoiceDeatils.Currency == "INR")
                    {
                        phrase.Add(new Chunk("Indian Rupees "
                        + taxrupeesInwords
                        + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.VatAmount + ")\n\n", headerFont));
                    }
                    else if (getInvoiceDeatils.Currency == "CAD")
                    {
                        phrase.Add(new Chunk("CAD "
                        + taxrupeesInwords
                        + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.VatAmount + ")\n\n", headerFont));
                    }
                    else if (getInvoiceDeatils.Currency == "GBP")
                    {
                        phrase.Add(new Chunk("GBP "
                        + taxrupeesInwords
                        + " Only (" + getInvoiceDeatils.Currency + " " + getInvoiceDeatils.VatAmount + ")\n\n", headerFont));
                    }
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                }

                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding = 3;
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                cell.PaddingTop = 10f;
                LastTable.AddCell(cell);

                PdfPTable VatTable = new PdfPTable(3);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nVAT %\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BackgroundColor = new BaseColor(255, 255, 255);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                cell.Padding = 3;
                VatTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nTaxable Amt\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BackgroundColor = new BaseColor(255, 255, 255);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                cell.Padding = 3;
                VatTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nTax Amt\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BackgroundColor = new BaseColor(255, 255, 255);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                cell.Padding = 3;
                VatTable.AddCell(cell);
                int j = 0;
                if (getInvoiceDeatils.invoiceVatDetails != null && getInvoiceDeatils.invoiceVatDetails.Count > 0)
                {
                    if (getInvoiceDeatils.invoiceVatDetails.Count > 0)
                    {
                        for (int index = 0; index < getInvoiceDeatils.invoiceVatDetails.Count; index++)
                        {
                            if (getInvoiceDeatils.invoiceVatDetails[index].Vat != "0")
                            {
                                phrase = new Phrase();
                                phrase.Add(new Chunk("" + getInvoiceDeatils.invoiceVatDetails[index].Vat + " %" + "\n", headerFont));
                                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                                cell.BackgroundColor = new BaseColor(255, 255, 255);
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.Border = Rectangle.BOX;
                                cell.BorderWidth = 1;
                                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                                cell.Padding = 3;
                                VatTable.AddCell(cell);

                                phrase = new Phrase();
                                phrase.Add(new Chunk("" + getInvoiceDeatils.invoiceVatDetails[index].TotalAmount + "\n", headerFont));
                                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                                cell.BackgroundColor = new BaseColor(255, 255, 255);
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.Border = Rectangle.BOX;
                                cell.BorderWidth = 1;
                                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                                cell.Padding = 3;
                                VatTable.AddCell(cell);

                                phrase = new Phrase();
                                phrase.Add(new Chunk("" + getInvoiceDeatils.invoiceVatDetails[index].VatAmount + "\n", headerFont));
                                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                                cell.BackgroundColor = new BaseColor(255, 255, 255);
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.Border = Rectangle.BOX;
                                cell.BorderWidth = 1;
                                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                                cell.Padding = 3;

                                VatTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                                VatTable.AddCell(cell);
                            }
                        }
                    }
                    
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Total\n", titleFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;
                    VatTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("" + getInvoiceDeatils.TotalTaxableAmount + "\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;
                    VatTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("" + getInvoiceDeatils.VatAmount + "\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;

                    VatTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                    VatTable.AddCell(cell);
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("---\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;
                    VatTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;
                    VatTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;

                    VatTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                    VatTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Total\n", titleFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;
                    VatTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;
                    VatTable.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                    cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                    cell.Padding = 3;

                    VatTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                    VatTable.AddCell(cell);

                }

                LastTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                LastTable.AddCell(VatTable);
                MainLastTable.DefaultCell.Border = Rectangle.NO_BORDER;
                MainLastTable.AddCell(LastTable);

                #region Bank Details
                PdfPTable accountDetails = new PdfPTable(2);
                accountDetails.TotalWidth = 555f;
                accountDetails.LockedWidth = true;
                accountDetails.SetWidths(new float[] { 1f, 1f });
                accountDetails.DefaultCell.PaddingTop = 90f;
                accountDetails.HorizontalAlignment = Element.ALIGN_LEFT;

                phrase = new Phrase();
                phrase.Add(new Chunk("\n\nCompany's Bank Details (1)\t\t\t\n", titleFont));
                phrase.Add(new Chunk("\nBank Name :", titleFont));
                phrase.Add(new Chunk(" Emirates Islamic Bank\n", headerFont));
                phrase.Add(new Chunk("\nAccount No :", titleFont));
                phrase.Add(new Chunk(" 3707484205601\n", headerFont));
                phrase.Add(new Chunk("\nBank Account Name :", titleFont));
                if (!InvoiceNumber.Contains("DIBN"))
                {
                    phrase.Add(new Chunk(" Devotion Corporate Services LLC\n", headerFont));
                }
                else
                {
                    phrase.Add(new Chunk(" DIBN BUSINESS SERVICES\n", headerFont));
                }
                phrase.Add(new Chunk("\nIBAN :", titleFont));
                phrase.Add(new Chunk(" AE500340003707484205601", headerFont));
                phrase.Add(new Chunk("\r\t\t", titleFont));
                phrase.Add(new Chunk("\nBranch Name :", titleFont));
                phrase.Add(new Chunk(" Dubai Mall , Dubai\n", headerFont));
                phrase.Add(new Chunk("\nSWIFT Code :", titleFont));
                phrase.Add(new Chunk(" MEBLAEADXXX\n", headerFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                if (!InvoiceNumber.Contains("DIBN"))
                {
                    cell.Colspan = 2;
                }
                accountDetails.AddCell(cell);

                if (InvoiceNumber.Contains("DIBN"))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\nCompany's Bank Details (2)\t\t\t\n", titleFont));
                    phrase.Add(new Chunk("\nBank Name :", titleFont));
                    phrase.Add(new Chunk(" ADCB Bank (Abu Dhabi Commercial Bank)\n", headerFont));
                    phrase.Add(new Chunk("\nAccount No :", titleFont));
                    phrase.Add(new Chunk(" 11320988820001\n", headerFont));
                    phrase.Add(new Chunk("\nBank Account Name :", titleFont));
                    //if (!InvoiceNumber.Contains("DIBN"))
                    //{
                    //    phrase.Add(new Chunk(" DEVOTION CORPORATE SERVICES L.L.C\n", headerFont));
                    //}
                    //else
                    //{
                    phrase.Add(new Chunk(" DIBN BUSINESS SERVICES\n", headerFont));
                    //}
                    phrase.Add(new Chunk("\nIBAN :", titleFont));
                    phrase.Add(new Chunk(" AE340030011320988820001", headerFont));
                    phrase.Add(new Chunk("\r\t\t", titleFont));
                    phrase.Add(new Chunk("\nBranch Name :", titleFont));
                    phrase.Add(new Chunk(" Khaled bin Waleed street, bur Dubai\n", headerFont));
                    phrase.Add(new Chunk("\nSWIFT Code :", titleFont));
                    phrase.Add(new Chunk(" ADCBAEAAXXX\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                    cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    accountDetails.AddCell(cell);
                }
                MainLastTable.DefaultCell.Border = Rectangle.NO_BORDER;
                MainLastTable.AddCell(accountDetails);
                #endregion

                PdfPTable authorityTable = new PdfPTable(1);
                authorityTable.TotalWidth = 260f;
                authorityTable.LockedWidth = true;
                authorityTable.SetWidths(new float[] { 1f });
                authorityTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                phrase = new Phrase();
                if (!InvoiceNumber.Contains("DIBN"))
                {
                    phrase.Add(new Chunk("For DEVOTION CORPORATE SERVICES L.L.C", titleFont));
                }
                else
                {
                    phrase.Add(new Chunk("For DIBN BUSINESS SERVICES", titleFont));
                }
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BackgroundColor = new BaseColor(255, 255, 255);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.PaddingBottom = 3f;


                authorityTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n\nAuthorized Signatory", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BackgroundColor = new BaseColor(255, 255, 255);
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 11f;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);

                authorityTable.AddCell(cell);
                authorityTable.KeepTogether = true;
                MainLastTable.DefaultCell.Border = Rectangle.NO_BORDER;
                MainLastTable.DefaultCell.PaddingTop = 15f;
                MainLastTable.AddCell(authorityTable);
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.AddCell(MainLastTable);

                table.SplitLate = false;
                table.SplitRows = true;

                document.Add(table);
               
                document.Close();
                stream.Flush(); 
                stream.Position = 0;
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadSelectedDocument(int Id)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            SaveFinalPdf document = new SaveFinalPdf();
            document = _userCompanyService.GetFinalPdfById(Id);
            MemoryStream ms = new MemoryStream(document.DataBinary);
            return File(document.DataBinary, System.Net.Mime.MediaTypeNames.Application.Octet, document.PdfName + document.Extension);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCompanyExpenses(string FromDate,string ToDate)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string minDate = null, maxDate = null;
            string _company = GetActorClaims();
            int CompanyId = Convert.ToInt32(_company);

            UserCompanyViewModel company = new UserCompanyViewModel();
            company = _userCompanyService.GetCompanyById(CompanyId);

            CompanyAccountDetails model = new CompanyAccountDetails();
            model = _portalBalanceService.GetCompanyAccount(CompanyId,FromDate,ToDate);

            DateTime fromDate = Convert.ToDateTime(FromDate), toEnd = Convert.ToDateTime(ToDate);
            minDate = fromDate.ToString("dd-MM-yyyy");

            maxDate = toEnd.ToString("dd-MM-yyyy");
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 88f, 88f, 10f, 280f);
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                Phrase phrase = null;
                PdfPCell cell = null;
                PdfPTable table = null;
                pdfWriter.CloseStream = false;

                pdfWriter.PageEvent = new BottomLayout();
                document.SetMargins(5f, 5f, 10f, 45f);
                document.Open();
                Font titleFont = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                iTextSharp.text.Font mainHeaderFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                Font headerFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font pdfTitleFont = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                //Header Table
                table = new PdfPTable(1);
                table.TotalWidth = 570f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 1f });
                table.DefaultCell.Padding = 0;
                table.HeaderRows = 1;
                table.HeaderRows = 2;

                PdfPTable info = new PdfPTable(2);
                string path = "wwwroot/DIBN_Logo.png";
                FileStream fs1 = new FileStream(path, FileMode.Open);
                iTextSharp.text.Image JPG = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Jpeg);
                JPG.ScalePercent(5.5f);
                fs1.Close();

                phrase = new Phrase();
                phrase.Add(new Chunk("", titleFont));
                cell = new PdfPCell(phrase);
                cell = new PdfPCell(JPG);
                cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = PdfPCell.NO_BORDER;
                cell.NoWrap = true;
                cell.PaddingTop = 10f;
                cell.Colspan = 2;
                info.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n\nName: " + company.CompanyName + "\t\t\t\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.PaddingTop = 5f;
                info.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n\nDate : " + minDate + " To " + maxDate + "\t\t\t\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_RIGHT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.PaddingTop = 5f;
                info.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nAccount No : " + company.AccountNumber + "\t\t\t\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.PaddingTop = 5f;
                cell.PaddingBottom = 10f;
                cell.Colspan = 2;
                info.AddCell(cell);


                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.AddCell(info);

                PdfPTable accountDetails = new PdfPTable(7);
                accountDetails.TotalWidth = 570f;
                accountDetails.LockedWidth = true;
                accountDetails.SetWidths(new float[] { 0.5f, 1f, 1f, 3f, 1.5f, 1.5f, 1.5f });
                accountDetails.PaddingTop = 30f;
                phrase = new Phrase();
                phrase.Add(new Chunk("\nNo.\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 5f;
                accountDetails.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nTRN No.\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 5f;
                accountDetails.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nDate.\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 5f;
                accountDetails.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nDescription.\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 5f;
                accountDetails.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nCredit(AED).\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 5f;
                accountDetails.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nDebit(AED).\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 5f;
                accountDetails.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nBalance(AED).\n\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 5f;
                cell.PaddingTop = 5f;
                accountDetails.AddCell(cell);

                table.DefaultCell.Border = Rectangle.BOX;
                table.AddCell(accountDetails);

                PdfPTable account = new PdfPTable(7);
                account.TotalWidth = 570f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 3f, 1.5f, 1.5f, 1.5f });
                int index = 1;
                // int checkIndex = 8;
                if (model.getexpensesofCompanies.Count > 0)
                {
                    for (int i = 0; i < model.getexpensesofCompanies.Count; i++)
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n" + index + ".\t\t\t\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        cell.PaddingBottom = 14;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n" + model.getexpensesofCompanies[i].TransactionId + "\t\t\t\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        cell.PaddingBottom = 14;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n" + model.getexpensesofCompanies[i].Date + "\t\t\t\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        cell.PaddingBottom = 14;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n" + model.getexpensesofCompanies[i].Description + "\t\t\t\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.PaddingBottom = 14;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n" + model.getexpensesofCompanies[i].Credit + "\t\t\t\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        cell.PaddingBottom = 14;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n" + model.getexpensesofCompanies[i].Debit + "\t\t\t\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        cell.PaddingBottom = 14;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n\n" + model.getexpensesofCompanies[i].Balance + "\t\t\t\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        cell.PaddingBottom = 14;
                        account.AddCell(cell);

                        index += 1;
                    }
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\t\t\t\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    cell.PaddingBottom = 5;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\t\t\t\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    cell.PaddingBottom = 5;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\t\t\t\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    cell.PaddingBottom = 5;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\t\t\t\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.PaddingBottom = 5;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\t\t\t\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    cell.PaddingBottom = 5;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\t\t\t\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    cell.PaddingBottom = 5;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\t\t\t\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    cell.PaddingBottom = 5;
                    account.AddCell(cell);
                }
                table.AddCell(account);

                PdfPTable totalAccount = new PdfPTable(4);
                totalAccount.TotalWidth = 570f;
                totalAccount.LockedWidth = true;
                totalAccount.SetWidths(new float[] { 5.5f, 1.5f, 1.5f, 1.5f });

                phrase = new Phrase();
                phrase.Add(new Chunk("\nTotal.\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 10f;
                totalAccount.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n" + model.TotalCredit + "\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 10f;
                totalAccount.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n" + model.TotalDebit + "\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 10f;
                totalAccount.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n" + model.TotalBalance + "\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                cell.PaddingBottom = 10f;
                totalAccount.AddCell(cell);

                table.AddCell(totalAccount);
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                document.Add(table);
                document.Close();
                stream.Flush();
                stream.Position = 0;
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
