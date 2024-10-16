using ClosedXML.Excel;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using DIBN.Areas.Admin.Repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class CompanyAccountController : Controller
    {
        public readonly ICompanyRepository _companyRepository;
        public readonly IUserRepository _userRepository;
        public readonly IPermissionRepository _permissionRepository;
        private readonly IPortalBalanceExpensesRepository _portalBalanceExpensesRepository;
        private IMemoryCache _cache;

        public CompanyAccountController(ICompanyRepository companyRepository, IUserRepository userRepository,
            IPermissionRepository permissionRepository, IPortalBalanceExpensesRepository portalBalanceExpensesRepository,
            IMemoryCache cache)
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _portalBalanceExpensesRepository = portalBalanceExpensesRepository;
            _cache = cache;
        }



        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllCompanyListForAccountSummary()
        {
            try
            {
                string searchBy = "";
                GetCompanyForAccountSummaryWithPagination model = new GetCompanyForAccountSummaryWithPagination();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _companyRepository.GetCompanyDetailForAccountSummary(skip, pageSize, sortColumn, sortColumnDirection,searchBy,searchValue);
                filterRecord = model.totalCompanies;
                totalRecord = model.totalCompanies;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getCompaniesForAccounts
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
        public async Task<IActionResult> GetAllCompanyListForAccountSummaryFilter(string sortColumn,string sortColumnDirection,string? searchBy,string? searchedValue)
        {
            try
            {
                GetCompanyForAccountSummaryWithPagination model = new GetCompanyForAccountSummaryWithPagination();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _companyRepository.GetCompanyDetailForAccountSummary(skip, pageSize, sortColumn, sortColumnDirection, searchBy, searchedValue);
                filterRecord = model.totalCompanies;
                totalRecord = model.totalCompanies;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getCompaniesForAccounts
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
        public async Task<IActionResult> GetCompanyAccountDetailsPagination(int CompanyId,string sortBy,string sortingDirection,string? searchBy,string searchValue)
        {
            try
            {
                GetCompanyAccountDetailPaginationModel model = new GetCompanyAccountDetailPaginationModel();
                string role = GetClaims();
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _portalBalanceExpensesRepository.GetCompanyAccountWithPagination(CompanyId,skip, pageSize,sortBy,sortingDirection,searchBy,searchValue);
                filterRecord = model.totalAccounts;
                totalRecord = model.totalAccounts;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getexpensesofCompanies
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
        public async Task<IActionResult> GetCompanyExpenseDetails(int Id,string? message)
        {
            GetAllExpenses portalBalance = new GetAllExpenses();
            portalBalance.getPortalBalanceDetails = _portalBalanceExpensesRepository.GetPortalBalanceDetailsForCompany(Id);
            GetCompanyAccountDetailModel model = new GetCompanyAccountDetailModel();
            model = await _portalBalanceExpensesRepository.GetCompanyTotalAccount(Id);
            model.CompanyId = Id;
            CompanyViewModel company = new CompanyViewModel();
            company = _companyRepository.GetCompanyById(Id);
            model.CompanyName = company.CompanyName;
            model.PortalBalance = portalBalance.getPortalBalanceDetails.PortalBalance;
            model.message = message;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetPaymentReceipts(string? name,string? message)
        {
            List<GetPaymentTransactionReceipt> transactionReceipts = new List<GetPaymentTransactionReceipt>();

            GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
            transactionReceipt.Module = name;
            transactionReceipt.message = message;
            transactionReceipts.Add(transactionReceipt);

            return View(transactionReceipts);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetPaymentReceiptsHistory(string? name)
        {
            List<GetPaymentTransactionReceipt> transactionReceipts = new List<GetPaymentTransactionReceipt>();
            
            GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
            transactionReceipt.Module = name;
            transactionReceipts.Add(transactionReceipt);
           
            return View(transactionReceipts);
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult EditPaymentReceipts(int Id)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromDays(1))
                   .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                   .SetPriority(CacheItemPriority.High)
                   .SetSize(1024);
            
            _cache.Set("GetAllPaymentReceiptLastProcess", true, cacheEntryOptions);

            GetPaymentTransactionReceipt transactionReceipts = new GetPaymentTransactionReceipt();
            transactionReceipts = _portalBalanceExpensesRepository.GetPaymentTransactionReceiptDetailsForEdit(Id);
            return PartialView("EditPaymentReceipts", transactionReceipts);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult EditsPaymentReceipt(GetPaymentTransactionReceipt payment)
        {
            PaymentTransaction paymentTransaction = new PaymentTransaction();
            paymentTransaction.CompanyId = payment.CompanyId;
            paymentTransaction.Id = payment.PaymentId;
            paymentTransaction.PaymentReceiptId = payment.Id;
            paymentTransaction.Description = payment.OnAccount;
            paymentTransaction.Through = payment.Through;
            paymentTransaction.CreatedOnUtc = payment.CreatedOn;
            paymentTransaction.Amount = payment.Amount;
            paymentTransaction.Quantity = payment.Quantity;
            paymentTransaction.TotalAmount = payment.TotalAmount;


            string _returnId = "";
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            CompanyViewModel model = new CompanyViewModel();
            model = _companyRepository.GetCompanyById(payment.CompanyId);
            paymentTransaction.UserId = UserId;
            paymentTransaction.PaymentType = "PaymentReceipt";
            _returnId = _portalBalanceExpensesRepository.UpdatePaymentDetails(paymentTransaction);
            if(_returnId.StartsWith("Selected Payment Transaction Updated Successfully."))
            {
                return RedirectToAction("GetPaymentReceipts", "CompanyAccount", new { name = "PaymentReceipt" });
            }
            return RedirectToAction("GetPaymentReceipts","CompanyAccount", new { name = "PaymentReceipt",message = _returnId });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ExportAsExcel(int Id, string FromDate, string ToDate)
        {
            string minDate = null, maxDate = null;
            GetCompanyAccountDetailModel model = new GetCompanyAccountDetailModel();
            model = _portalBalanceExpensesRepository.GetCompanyAccount(Id, FromDate, ToDate);
            model.CompanyId = Id;

            CompanyViewModel company = new CompanyViewModel();
            company = _companyRepository.GetCompanyById(Id);

            if (model.getexpensesofCompanies.Count > 0 && model.getexpensesofCompanies != null)
            {
                int lastIndex = (model.getexpensesofCompanies.Count) - 1;
                minDate = model.getexpensesofCompanies[0].Date;
                maxDate = model.getexpensesofCompanies[lastIndex].Date;
                if (maxDate == null)
                {
                    maxDate = model.getexpensesofCompanies[0].Date;
                }
            }
            if (minDate == null)
            {
                minDate = FromDate;
            }
            if (maxDate == null)
            {
                maxDate = ToDate;
            }
            int index = 1;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("No."),
                                        new DataColumn("TRN No."),
                                        new DataColumn("Date"),
                                        new DataColumn("Description"),
                                        new DataColumn("Credit(AED)"),
                                        new DataColumn("Debit(AED)"),
                                        new DataColumn("Balance(AED)"),
                                        });

            foreach (var data in model.getexpensesofCompanies)
            {
                dt.Rows.Add(index,data.TransactionId, data.Date, data.Description, data.Credit, data.Debit, data.Balance);
                index++;
            }

            dt.Rows.Add("Total", "", "", "", model.TotalCredit, model.TotalDebit, model.TotalBalance);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AccountSummary.xlsx");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public FileStreamResult DownloadPaymentReceipt(int Id)
        {
            GetPaymentTransactionReceipt model = new GetPaymentTransactionReceipt();
            model = _portalBalanceExpensesRepository.GetPaymentTransactionReceiptDetails(Id);

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

                phrase = new Phrase();
                phrase.Add(new Chunk("Particulars", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.BorderColor = BaseColor.BLACK;
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 1;
                cell.Padding =10.0f;
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
                phrase.Add(new Chunk("\n AED "+model.Amount, titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderWidth = 1;
                cell.DisableBorderSide(Rectangle.TOP_BORDER);
                cell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                cell.DisableBorderSide(Rectangle.LEFT_BORDER);
                amountTable.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\n"+model.CompanyName, middleHeader));
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
                phrase.Add(new Chunk("\n "+model.Through, middleHeader));
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
                phrase.Add(new Chunk("\n AED "  +model.Amount, titleFont));
                phrase.Add(new Chunk("\n", titleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                cell.Border = Rectangle.TOP_BORDER;
                cell.BorderWidth = 1;
                cell.BorderColor= BaseColor.BLACK;
                cell.Padding = 3;
                amountTable.AddCell(cell);

                table.AddCell(amountTable);

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
        public FileStreamResult DownloadPaymentTransactionReceipt(int Id)
        {
            GetPaymentTransactionReceipt model = new GetPaymentTransactionReceipt();
            model = _portalBalanceExpensesRepository.GetPaymentTransactionReceiptDetails(Id);

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
                return File(stream, "application/pdf","PaymentReceipt.pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        [Route("[action]")]
        public FileStreamResult GetCompanyExpenses(int Id, string FromDate, string ToDate)
        {
            string minDate = null, maxDate = null;
            GetCompanyAccountDetailModel model = new GetCompanyAccountDetailModel();
            model = _portalBalanceExpensesRepository.GetCompanyAccount(Id, FromDate, ToDate);
            model.CompanyId = Id;

            CompanyViewModel company = new CompanyViewModel();
            company = _companyRepository.GetCompanyById(Id);

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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllPaymentReceiptWithPagination(int page)
        {
            try
            {
                int count = 0, lastAccessedPage = 0;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromDays(1))
                   .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                   .SetPriority(CacheItemPriority.High)
                   .SetSize(1024);

                GetPaymentReceiptWithPaginationModel model = new GetPaymentReceiptWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                int sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

                if (_cache.TryGetValue("GetAllPaymentReceiptLastProcess", out bool lastProcess))
                {
                    if (_cache.TryGetValue("GetAllPaymentReceiptStartLength", out int startLength))
                    {
                        if (_cache.TryGetValue("GetAllPaymentReceiptLastLength", out int lastLength))
                        {
                            if (_cache.TryGetValue("GetAllPaymentReceiptLastAccessedPage", out int currentPage))
                            {
                                _cache.TryGetValue("GetAllPaymentReceiptSortColumnDirection", out string sortDir);
                                _cache.TryGetValue("GetAllPaymentReceiptSortColumn", out string sortBy);
                                _cache.TryGetValue("GetAllPaymentReceiptSortColumnIndex", out int sortIndex);
                                if (lastProcess)
                                {
                                    skip = startLength;
                                    pageSize = lastLength;
                                    lastAccessedPage = currentPage;
                                    sortColumnDirection = sortDir;
                                    sortColumnIndex = sortIndex;
                                    sortColumn = sortBy;
                                    _cache.Set("GetAllPaymentReceiptLastProcess", false, cacheEntryOptions);
                                }
                            }
                        }
                    }
                }
                model = await _portalBalanceExpensesRepository.GetAllPaymentTransactionReceiptWithPagination(skip, pageSize,null, searchValue, sortColumn, sortColumnDirection,count);
                filterRecord = model.totalPaymentReceipts;
                totalRecord = model.totalPaymentReceipts;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getPaymentTransactionReceipts,
                    currentPage = lastAccessedPage,
                    displayData = pageSize,
                    sortDir = sortColumnDirection,
                    sortIndex = sortColumnIndex
                };

                if (lastProcess)
                {
                    _cache.Set("GetAllPaymentReceiptLastAccessedPage", lastAccessedPage, cacheEntryOptions);
                }
                else
                {
                    _cache.Set("GetAllPaymentReceiptLastAccessedPage", page, cacheEntryOptions);
                }

                _cache.Set("GetAllPaymentReceiptStartLength", skip, cacheEntryOptions);
                _cache.Set("GetAllPaymentReceiptLastLength", pageSize, cacheEntryOptions);
                _cache.Set("GetAllPaymentReceiptSortColumnDirection", sortColumnDirection, cacheEntryOptions);
                _cache.Set("GetAllPaymentReceiptSortColumn", sortColumn, cacheEntryOptions);
                _cache.Set("GetAllPaymentReceiptSortColumnIndex", sortColumnIndex, cacheEntryOptions);
                return new JsonResult(returnObj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllPaymentReceiptWithPaginationFilter(string sortColumn,string sortColumnDirection,string? searchBy,string? searchedValue)
        {
            try
            {
                GetPaymentReceiptWithPaginationModel model = new GetPaymentReceiptWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _portalBalanceExpensesRepository.GetAllPaymentTransactionReceiptWithPagination(skip, pageSize,searchBy ,searchedValue, sortColumn, sortColumnDirection, 0);
                filterRecord = model.totalPaymentReceipts;
                totalRecord = model.totalPaymentReceipts;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getPaymentTransactionReceipts
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
        public async Task<IActionResult> GetAllPaymentReceiptHistoryWithPagination()
        {
            try
            {
                int count = 0;
                string searchBy = "";

                GetPaymentReceiptWithPaginationModel model = new GetPaymentReceiptWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _portalBalanceExpensesRepository.GetAllPaymentTransactionReceiptHistoryWithPagination(skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection,count);
                filterRecord = model.totalPaymentReceipts;
                totalRecord = model.totalPaymentReceipts;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getPaymentTransactionReceipts
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
        public async Task<IActionResult> GetAllPaymentReceiptHistoryWithPaginationFilter(string sortColumn, string sortColumnDirection, string? searchBy, string? searchedValue)
        {
            try
            {
                int count = 0;
                GetPaymentReceiptWithPaginationModel model = new GetPaymentReceiptWithPaginationModel();
                string role = GetClaims();
                string _companyId = GetActorClaims();
                int CompanyId = Convert.ToInt32(_companyId);
                string _user = GetUserClaims();
                int _userId = _permissionRepository.GetUserIdForPermission(_user);
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _portalBalanceExpensesRepository.GetAllPaymentTransactionReceiptHistoryWithPagination(skip, pageSize, searchBy, searchedValue, sortColumn, sortColumnDirection, count);
                filterRecord = model.totalPaymentReceipts;
                totalRecord = model.totalPaymentReceipts;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getPaymentTransactionReceipts
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
    }
}
