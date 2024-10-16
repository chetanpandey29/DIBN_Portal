using ClosedXML.Excel;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class AccountManagementController : Controller
    {
        public readonly ICompanyRepository _companyRepository;
        private readonly IPortalBalanceExpensesRepository _portalBalanceExpensesRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IReportManagementRepository _reportManagementRepository;
        private readonly IUserRepository _userRepository;

        public AccountManagementController(ICompanyRepository companyRepository, IPortalBalanceExpensesRepository portalBalanceExpensesRepository,
            IPermissionRepository permissionRepository, IReportManagementRepository reportManagementRepository, IUserRepository userRepository)
        {
            _companyRepository = companyRepository;
            _portalBalanceExpensesRepository = portalBalanceExpensesRepository;
            _permissionRepository = permissionRepository;
            _reportManagementRepository = reportManagementRepository;
            _userRepository = userRepository;
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name)
        {
            Log.Information("Account Management");
            AccountManagementModel model = new AccountManagementModel();
            List<CompanyViewModel> otherCompanies = new List<CompanyViewModel>();
            otherCompanies = _companyRepository.GetCompanies();

            List<SelectListItem> companies = new List<SelectListItem>();
            companies.Add(new SelectListItem
            {
                Text = "Select Company",
                Value = "0"
            });
            for (int i = 0; i < otherCompanies.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = otherCompanies[i].CompanyName,
                    Value = otherCompanies[i].Id.ToString()
                });
            }

            model.Companies = companies;
            model.Module = name;
            return View(model);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddCompanyExpenses([FromBody] List<SaveCompanyExpenses> model)
        {
            try
            {
                int _returnId = 0;
                Log.Information("Added New Expenses of Companies");
                string username = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(username);
                foreach (var company in model)
                {
                    company.UserId = UserId.ToString();
                }
                _returnId = _portalBalanceExpensesRepository.AddCompanyExpensesAccount(model);
                return new JsonResult(_returnId);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SaveDirectCompanyExpensesAccount([FromBody] List<SaveCompanyExpenses> model)
        {
            try
            {
                string _returnId = "";
                Log.Information("Added New Expenses of Companies");
                string username = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(username);
                foreach (var company in model)
                {
                    company.UserId = UserId.ToString();
                }
                _returnId = _portalBalanceExpensesRepository.SaveDirectCompanyExpensesAccount(model);
                return new JsonResult(_returnId);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CheckAccountEntryType(string SerialCode)
        {
            try
            {
                GetAccountTypeModel accountType = new GetAccountTypeModel();
                accountType = await _portalBalanceExpensesRepository.CheckAccountEntryType(SerialCode);
                return new JsonResult(accountType);
            }
            catch(Exception ex)
            {
                GetAccountTypeModel accountType = new GetAccountTypeModel();
                return new JsonResult(accountType);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddCompanyExpensesToTemp([FromBody] SaveCompanyExpenses model)
        {
            try
            {
                int _returnId = 0;
                Log.Information("Added New Expenses of Companies Temp");
                string username = GetUserClaims();
                int UserId = _permissionRepository.GetUserIdForPermission(username);
                model.UserId = UserId.ToString();
                _returnId = _portalBalanceExpensesRepository.AddCompanyExpensesAccountToTemp(model);
                return new JsonResult(_returnId);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SaveCompanyExpensesFromTemp([FromBody]  List<int> ids)
        {
            try
            {
                int _returnId = 0;
                _returnId = _portalBalanceExpensesRepository.SaveCompanyExpensesAccountFromTemp(ids);
                return new JsonResult(_returnId);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateRepeatedAccountEntries()
        {
            try
            {
                int _returnId = 0;
                _returnId = _portalBalanceExpensesRepository.UpdateRepeatedAccountEntries();
                return new JsonResult(_returnId);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult DeleteCompanyExpensesFromTemp(int id)
        {
            try
            {
                int _returnId = 0;
                _returnId = _portalBalanceExpensesRepository.RemoveCompanyExpensesAccountFromTemp(id);
                return new JsonResult(_returnId);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ExportAsExcel(string FromDate, string ToDate)
        {
            #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string minDate = null, maxDate = null;
            List<GetHistoryOfCompanyExpenses> getHistories = new List<GetHistoryOfCompanyExpenses>();
            getHistories = _portalBalanceExpensesRepository.GetHistoryOfAllCompanyExpenses(FromDate, ToDate);

            if (getHistories.Count > 0 && getHistories != null)
            {
                int lastIndex = (getHistories.Count) - 1;
                minDate = getHistories[0].Date;
                maxDate = getHistories[lastIndex].Date;
                if (maxDate == null)
                {
                    maxDate = getHistories[0].Date;
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
            dt.Columns.AddRange(new DataColumn[12] { new DataColumn("No."),
                                        new DataColumn("TRN No."),
                                        new DataColumn("Company"),
                                        new DataColumn("Type"),
                                        new DataColumn("Expense Description"),
                                        new DataColumn("Amount"),
                                        new DataColumn("Quantity"),
                                        new DataColumn("Total Amount"),
                                        new DataColumn("Vat(%)"),
                                        new DataColumn("Vat Amount"),
                                        new DataColumn("Grand Total"),
                                        new DataColumn("Created On"),
                                        });

            foreach (var data in getHistories)
            {
                string totalAmount = data.Type == "Credit" ? data.PaymentCredit : data.TotalAmount.ToString();
                string description = data.Type == "Credit" ? data.Description : data.ExpensesTitle;
                string vat = data.Type == "Credit" ? "0 %" : data.Vat;
                string vatAmount = data.Type == "Credit" ? "0.0" : data.VatAmount;
                string grandTotal = data.Type == "Credit" ? data.PaymentCredit : data.GrandTotal;

                dt.Rows.Add(index,data.TransactionId,data.CompanyName, data.Type, description
                    , data.ExpensesAmount, data.Quantity,
                    totalAmount, vat, vatAmount,
                    grandTotal, data.CreatedOnUtc);
                index++;
            }


            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                string currentDate = DateTime.Now.ToString("ddMMyyyyhhmmss");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Account Management " + minDate + "-" + maxDate + "(" + currentDate + ").xlsx");
                }
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<JsonResult> SaveExcelDownloadDocumentList(string fromDate, string toDate, string emailAddress)
        {
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToEnd = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");

            maxDate = ToEnd.ToString("dd-MM-yyyy");

            string currentDate = DateTime.Now.ToString("ddMMyyyyhhmmss");

            SaveDownloadDocumentModel model = new SaveDownloadDocumentModel();
            model.FromDate = fromDate;
            model.ToDate = toDate;
            model.ActionMethod = "ExportAsExcelEmail";
            model.Controller = "AccountManagement";
            model.EmailAddress = emailAddress;
            model.FileExtension = ".xlsx";
            model.FileName = "Account Management " + minDate + "-" + maxDate + "(" + currentDate + ").xlsx";
            model.createdBy = userId;

            await _reportManagementRepository.SaveDownloadDocumentData(model);

            return new JsonResult(model.FileName);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task ExportAsExcelEmail(string FromDate, string ToDate,string emailAddress,string fileName)
        {
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            var userDetails = _userRepository.GetUserDetail(userId);

            string minDate = null, maxDate = null;
            List<GetHistoryOfCompanyExpenses> getHistories = new List<GetHistoryOfCompanyExpenses>();
            getHistories = _portalBalanceExpensesRepository.GetHistoryOfAllCompanyExpenses(FromDate, ToDate);

            if (getHistories.Count > 0 && getHistories != null)
            {
                int lastIndex = (getHistories.Count) - 1;
                minDate = getHistories[0].Date;
                maxDate = getHistories[lastIndex].Date;
                if (maxDate == null)
                {
                    maxDate = getHistories[0].Date;
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
            dt.Columns.AddRange(new DataColumn[12] { new DataColumn("No."),
                                        new DataColumn("TRN No."),
                                        new DataColumn("Company"),
                                        new DataColumn("Type"),
                                        new DataColumn("Expense Description"),
                                        new DataColumn("Amount"),
                                        new DataColumn("Quantity"),
                                        new DataColumn("Total Amount"),
                                        new DataColumn("Vat(%)"),
                                        new DataColumn("Vat Amount"),
                                        new DataColumn("Grand Total"),
                                        new DataColumn("Created On"),
                                        });

            foreach (var data in getHistories)
            {
                string totalAmount = data.Type == "Credit" ? data.PaymentCredit : data.TotalAmount.ToString();
                string description = data.Type == "Credit" ? data.Description : data.ExpensesTitle;
                string vat = data.Type == "Credit" ? "0 %" : data.Vat;
                string vatAmount = data.Type == "Credit" ? "0.0" : data.VatAmount;
                string grandTotal = data.Type == "Credit" ? data.PaymentCredit : data.GrandTotal;

                dt.Rows.Add(index, data.TransactionId, data.CompanyName, data.Type, description
                    , data.ExpensesAmount, data.Quantity,
                    totalAmount, vat, vatAmount,
                    grandTotal, data.CreatedOnUtc);
                index++;
            }


            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    string path = "wwwroot/companyPdfExcel/" + fileName;
                    wb.SaveAs(stream);
                    System.IO.File.WriteAllBytes(path, stream.ToArray());
                    string responseText = await _reportManagementRepository.SendCompanyAccountFileOnEmail(path, fileName, emailAddress, FromDate, ToDate
                        , "", "Excel", userId, (userDetails.FirstName + " " + userDetails.LastName + "(" + userDetails.EmailID + ")"));
                    System.IO.File.Delete(path);
                }
            }
        }
        /// <summary>
        /// To Get PDF of All Company Expenses Between Passed From Date and To Date
        /// Changes :- 24-11-2022 (Yashasvi TBC)
        /// (i) Display all expense details which can fit one Page and so on. (Remove Static Length of Data to Display on One Page.)
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [Route("[action]")]
        public IActionResult ExportAsPdf(string FromDate, string ToDate)
        {
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            var userDetails = _userRepository.GetUserDetail(userId);
            string minDate = null, maxDate = null;
            List<GetHistoryOfCompanyExpenses> getHistories = new List<GetHistoryOfCompanyExpenses>();
            getHistories = _portalBalanceExpensesRepository.GetHistoryOfAllCompanyExpenses(FromDate, ToDate);
            DateTime fromDate = Convert.ToDateTime(FromDate), toEnd = Convert.ToDateTime(ToDate);
            minDate = fromDate.ToString("dd-MM-yyyy");

            maxDate = toEnd.ToString("dd-MM-yyyy");

            int index = 1;

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();

            CompanyViewModel company = new CompanyViewModel();

            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                Phrase phrase = null;
                PdfPCell cell = null;
                PdfPTable table = null;
                pdfWriter.CloseStream = false;

                pdfWriter.PageEvent = new AccountManagementBottomLayout(minDate, maxDate);
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;
                document.SetMargins(5f, 5f, 40f, 28f);
                document.Open();
                Font titleFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font headerFont = FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font pdfTitleFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                //Header Table
                table = new PdfPTable(1);
                table.TotalWidth = 580f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 1f });
                table.DefaultCell.Padding = 0;
                table.HeaderRows = 1;

                PdfPTable account = new PdfPTable(9);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1.5f, 1.5f, 1.5f, 2.0f, 1f, 0.5f, 1.5f });
                account.PaddingTop = 130f;
                account.HeaderRows = 1;

                phrase = new Phrase();
                phrase.Add(new Chunk("\nNo.\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nTRN No.\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nDate\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nCompany\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nType\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nDescription\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nVat(%)\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nVat Amt.\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nAmount(AED)\n", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                if (getHistories.Count > 0)
                {
                    foreach (var item in getHistories)
                    {
                        string description = item.Type == "Credit" ? item.Description : item.ExpensesTitle;
                        string vat = item.Type == "Credit" ? "0" : item.Vat;
                        string vatAmount = item.Type == "Credit" ? "0.0" : item.VatAmount;
                        string grandTotal = item.Type == "Credit" ? item.PaymentCredit : item.ExpensesAmount;

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+index.ToString()+ "\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+item.TransactionId+ "\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+item.CreatedOnUtc+"\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+item.CompanyName+ "\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.BorderColor = BaseColor.BLACK;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+item.Type+ "\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+description+ "\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+vat+ "\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+vatAmount+ "\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk("\n"+grandTotal+ "\n", headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        index += 1;
                    }
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("\n\n---\n\n", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);
                }

                try
                {
                    account.Complete = true;
                    document.Add(account);            /// It take too much time to add table if data is too much.             Yashasvi (31-05-2023)
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                document.Close();
                stream.Flush();
                stream.Position = 0;
                string currentDate = DateTime.Now.ToString("ddMMyyyyhhmmss");
                Debug.WriteLine("final");
                string fileName = "Account Management " + minDate + "-" + maxDate + "(" + currentDate + ").pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<JsonResult> SavePDFDownloadDocumentList(string fromDate,string toDate,string emailAddress)
        {
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToEnd = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");

            maxDate = ToEnd.ToString("dd-MM-yyyy");

            string currentDate = DateTime.Now.ToString("ddMMyyyyhhmmss");

            SaveDownloadDocumentModel model = new SaveDownloadDocumentModel();
            model.FromDate = fromDate;
            model.ToDate = toDate;
            model.ActionMethod = "ExportAsPdfEmail";
            model.Controller = "AccountManagement";
            model.EmailAddress = emailAddress;
            model.FileExtension = ".pdf";
            model.FileName = "Account Management " + minDate + "-" + maxDate + "("+ currentDate + ").pdf";
            model.createdBy = userId;

            //await _reportManagementRepository.SaveDownloadDocumentData(model);

            return new JsonResult(model.FileName);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task ExportPdfEmail(string FromDate, string ToDate,string emailAddress,string fileName)
        {
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            var userDetails = _userRepository.GetUserDetail(userId);
            string minDate = null, maxDate = null;
            List<GetHistoryOfCompanyExpenses> getHistories = new List<GetHistoryOfCompanyExpenses>();
            getHistories = _portalBalanceExpensesRepository.GetHistoryOfAllCompanyExpenses(FromDate, ToDate);
            DateTime fromDate = Convert.ToDateTime(FromDate), toEnd = Convert.ToDateTime(ToDate);
            minDate = fromDate.ToString("dd-MM-yyyy");

            maxDate = toEnd.ToString("dd-MM-yyyy");

            int index = 1;

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();

            CompanyViewModel company = new CompanyViewModel();

            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                Phrase phrase = null;
                PdfPCell cell = null;
                PdfPTable table = null;
                pdfWriter.CloseStream = false;

                pdfWriter.PageEvent = new AccountManagementBottomLayout(minDate, maxDate);
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;
                document.SetMargins(5f, 5f, 40f, 28f);
                document.Open();
                Font titleFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font headerFont = FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font pdfTitleFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                //Header Table
                table = new PdfPTable(1);
                table.TotalWidth = 580f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 1f });
                table.DefaultCell.Padding = 0;
                table.HeaderRows = 1;

                PdfPTable account = new PdfPTable(9);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1.5f, 1.5f, 1.5f, 2.0f, 1f, 0.5f, 1.5f });
                account.PaddingTop = 130f;
                account.HeaderRows = 1;

                phrase = new Phrase();
                phrase.Add(new Chunk("\nNo.", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nTRN No.", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nDate", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nCompany", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nType", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nDescription", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nVat(%)", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nVat Amt.", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                phrase = new Phrase();
                phrase.Add(new Chunk("\nAmount(AED)", pdfTitleFont));
                cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                cell.Border = Rectangle.BOX;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 1;
                account.AddCell(cell);

                if (getHistories.Count > 0)
                {
                    foreach (var item in getHistories)
                    {
                        string description = item.Type == "Credit" ? item.Description : item.ExpensesTitle;
                        string vat = item.Type == "Credit" ? "0" : item.Vat;
                        string vatAmount = item.Type == "Credit" ? "0.0" : item.VatAmount;
                        string grandTotal = item.Type == "Credit" ? item.PaymentCredit : item.ExpensesAmount;

                        phrase = new Phrase();
                        phrase.Add(new Chunk(index.ToString(), headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(item.TransactionId, headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(item.CreatedOnUtc, headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(item.CompanyName, headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderWidth = 1;
                        cell.BorderColor = BaseColor.BLACK;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(item.Type, headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(description, headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(vat, headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(vatAmount, headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        phrase = new Phrase();
                        phrase.Add(new Chunk(grandTotal, headerFont));
                        cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.Border = Rectangle.BOX;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.BorderWidth = 1;
                        account.AddCell(cell);

                        index += 1;
                    }
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderWidth = 1;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("---", headerFont));
                    cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
                    cell.Border = Rectangle.BOX;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1;
                    account.AddCell(cell);
                }

                try
                {
                    account.Complete = true;
                    document.Add(account);            /// It take too much time to add table if data is too much.             Yashasvi (31-05-2023)
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                document.Close();
                stream.Flush();
                stream.Position = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    string path = "wwwroot/companyPdfExcel/" + fileName;
                    System.IO.File.WriteAllBytes(path, stream.ToArray());
                    string responseText = await _reportManagementRepository.SendCompanyAccountFileOnEmail(path, fileName, emailAddress, FromDate, ToDate, "", "Pdf", userId, (userDetails.FirstName + " " + userDetails.LastName + "(" + userDetails.EmailID + ")"));
                    System.IO.File.Delete(path);
                    //return File(stream, MediaTypeNames.Application.Pdf, fileName);
                }
                //TempData["fileSendedMessage"] = responseText;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> DownloadedDocuments()
        {
            List<GetDownloadDocumentListModel> mainList = new List<GetDownloadDocumentListModel>();
            mainList = await _reportManagementRepository.GetDownloadDocumentList();
            return View(mainList);
        } 
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetHistory(string? name, string? FromDate, string? ToDate,string? message)
        {
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            var userDetails = _userRepository.GetUserDetail(userId);

            ConfirmationModel model = new ConfirmationModel();
            model.message = message;
            model.emailAddress = userDetails.EmailID;
            return View(model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetDataTableData(int page)
        {
            try
            {
                GetAccountHistoryDataModel model = new GetAccountHistoryDataModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = _portalBalanceExpensesRepository.GetHistoryOfAllCompanyExpensesTest(skip, pageSize, searchValue,sortColumn,sortColumnDirection);
                filterRecord = model.expenseCounts;
                totalRecord = model.expenseCounts;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getHistoryOfCompanyExpenses
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
        public IActionResult GetFilterwiseDataTable(string sortColumn,string sortColumnDirection,string? searchBy,string? searchedValue)
        {
            try
            {
                GetAccountHistoryDataModel model = new GetAccountHistoryDataModel();
                List<GetHistoryOfCompanyExpenses> getHistories = new List<GetHistoryOfCompanyExpenses>();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                model = _portalBalanceExpensesRepository.GetHistoryOfAllCompanyExpensesFilter(skip,pageSize, searchBy, searchedValue, sortColumn, sortColumnDirection);
                filterRecord = model.expenseCounts;
                totalRecord = model.expenseCounts;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getHistoryOfCompanyExpenses
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
        public IActionResult UpdateExpenses(int Id, string name, string ActionName, string Cnt,int? companyId)
        {
            UpdateCompanyExpenses model = new UpdateCompanyExpenses();
            List<CompanyViewModel> otherCompanies = new List<CompanyViewModel>();
            otherCompanies = _companyRepository.GetCompanies();

            List<SelectListItem> companies = new List<SelectListItem>();
            List<SelectListItem> type = new List<SelectListItem>();
            List<SelectListItem> paymentMode = new List<SelectListItem>();
            for (int i = 0; i < otherCompanies.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = otherCompanies[i].CompanyName,
                    Value = otherCompanies[i].Id.ToString()
                });
            }
            type.Add(new SelectListItem
            {
                Text = "Credit",
                Value = "Credit"
            });
            type.Add(new SelectListItem
            {
                Text = "Debit",
                Value = "Debit"
            });
            paymentMode.Add(new SelectListItem
            {
                Text = "Cash",
                Value = "Cash"
            });
            paymentMode.Add(new SelectListItem
            {
                Text = "Online Transaction",
                Value = "Online Transaction"
            });
            paymentMode.Add(new SelectListItem
            {
                Text = "Cheque",
                Value = "Cheque"
            });
            model = _portalBalanceExpensesRepository.GetExpenseDetails(Id);
            model.Companies = companies;
            model.Module = name;
            model.ActionName = ActionName;
            model.Controller = Cnt;
            model.expenseType = type;
            model.Type = "Debit";
            model.PaymentModeType = paymentMode;
            return PartialView("_UpdateExpenses", model);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetExpenseModification(int Id,string TransactionId)
        {
            string _returnMessages = "";
            var model = _portalBalanceExpensesRepository.GetExpenseDetails(Id);
            if (model.TransactionId == null || model.TransactionId == "")
            {
                ResponseModel response = new ResponseModel();
                response = _portalBalanceExpensesRepository.GetExpenseModificationDetails(Id);
                _returnMessages = "Selected Transaction (<b>"+ TransactionId + "</b>) is already deleted by <b>" + response.Username + "</b> at " + response.ModifyTime +".";
            }
            return new JsonResult(_returnMessages);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTransactionModification(int Id,int CompanyId, string TransactionId)
        {
            string _returnMessages = "";
            var model = _portalBalanceExpensesRepository.GetDetailsOfPayment(Id, CompanyId);
            if (model.TransactionId == null || model.TransactionId == "")
            {
                ResponseModel response = new ResponseModel();
                response = _portalBalanceExpensesRepository.GetTransactionModificationDetails(Id);
                _returnMessages = "Selected Transaction (<b>"+ TransactionId + "</b>) is already deleted by <b>" + response.Username + "</b> at " + response.ModifyTime + ".";
            }
            return new JsonResult(_returnMessages);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateExpenseDetails(UpdateCompanyExpenses model)
        {
            string _returnMessages = "";
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            int _returnId = 0;
            if (model.Type == "Debit")
            {
                if (model.PreviousCompanyId != model.CompanyId)
                {
                    string _returnMessage = _portalBalanceExpensesRepository.DeleteExpenses(model.Id, model.PreviousAmount, model.PreviousCompanyId, UserId);

                    if (_returnMessage.Contains("Selected Expense Deleted Successfully."))
                    {
                        List<SaveCompanyExpenses> saveExpense = new List<SaveCompanyExpenses>();
                        SaveCompanyExpenses model1 = new SaveCompanyExpenses();
                        model1.CompanyId = model.CompanyId.ToString();
                        model1.Quantity = model.Quantity.ToString();
                        model1.Amount = model.Amount;
                        model1.Vat = model.Vat;
                        model1.Type = model.Type;
                        model1.TotalAmount = model.TotalAmount;
                        model1.GrandTotal = model.GrandTotal;
                        model1.VatAmount = model.VatAmount;
                        model1.Date = model.CreatedOnUtc;
                        model1.TransactionId = model.TransactionId;
                        model1.Task = model.Task;

                        model1.UserId = UserId.ToString();
                        saveExpense.Add(model1);
                        _returnId = _portalBalanceExpensesRepository.AddCompanyExpensesAccount(saveExpense);
                    }
                    else
                    {
                        if (model.ActionName == "GetCompanyExpenseDetails")
                        {
                            return RedirectToAction("GetCompanyExpenseDetails", "CompanyAccount", new { Id = model.PreviousCompanyId, name = model.Module, message = _returnMessage });
                        }
                        return RedirectToAction("GetHistory", "AccountManagement", new { name = model.Module, message = _returnMessage });
                    }
                }
                else
                {
                    model.UserId = UserId;
                    _returnMessages = _portalBalanceExpensesRepository.UpdateExpenseDetail(model);
                }
            }
            else
            {
                string _returnMessage = _portalBalanceExpensesRepository.DeleteExpenses(model.Id, model.PreviousAmount, model.PreviousCompanyId, UserId);
                if (_returnMessage.Contains("Selected Expense Deleted Successfully."))
                {
                    _returnId = _portalBalanceExpensesRepository.AddPortalBalance(model.TransactionId, model.Amount, model.TotalAmount, model.PaymentMode, model.Task, model.CompanyId, UserId, model.CreatedOnUtc, model.Quantity, model.TotalAmount);
                }
                else
                {
                    if (model.ActionName == "GetCompanyExpenseDetails")
                    {
                        return RedirectToAction("GetCompanyExpenseDetails", "CompanyAccount", new { Id = model.PreviousCompanyId, name = model.Module ,message = _returnMessage });
                    }
                    return RedirectToAction("GetHistory", "AccountManagement", new { name = model.Module, message = _returnMessage });
                }
            }
            string company = "";
            company = _companyRepository.GetCompanyName(model.CompanyId);
            Log.Information("Changed Account Summary Details of " + company);
            Log.Information("Description : " + model.Task + " , Amount : " + model.Amount + " , Quantity :" + model.Quantity + " , Total Amount :" + model.TotalAmount + ".");

            if (_returnMessages.Contains("Selected Company Expense Updated Successfully."))
            {
                if (model.ActionName == "GetCompanyExpenseDetails")
                {
                    return RedirectToAction("GetCompanyExpenseDetails", "CompanyAccount", new { Id = model.PreviousCompanyId, name = model.Module });
                }
                return RedirectToAction("GetHistory", "AccountManagement", new { name = model.Module });
            }

            if (model.ActionName == "GetCompanyExpenseDetails")
            {
                return RedirectToAction("GetCompanyExpenseDetails", "CompanyAccount", new { Id = model.PreviousCompanyId, name = model.Module, message = _returnMessages });
            }
            return RedirectToAction("GetHistory", "AccountManagement", new { name = model.Module, message = _returnMessages });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAccountHistory(string? name, int? CompanyId)
        {
            try
            {
                List<GetAccountHistory> getAccountHistories = new List<GetAccountHistory>();
                getAccountHistories = _portalBalanceExpensesRepository.GetAccountHistoryDetails(CompanyId);
                foreach (var item in getAccountHistories)
                {
                    item.Module = name;
                }
                return View(getAccountHistories);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult TemporaryAccountManagement(string? message)
        //{
        //    TemporaryAccountManagementModel model = new TemporaryAccountManagementModel();
        //    List<CompanyViewModel> otherCompanies = new List<CompanyViewModel>();
        //    otherCompanies = _companyRepository.GetCompanies();

        //    List<SelectListItem> companies = new List<SelectListItem>();
        //    companies.Add(new SelectListItem
        //    {
        //        Text = "Select Company",
        //        Value = "0"
        //    });
        //    for (int i = 0; i < otherCompanies.Count; i++)
        //    {
        //        companies.Add(new SelectListItem
        //        {
        //            Text = otherCompanies[i].CompanyName,
        //            Value = otherCompanies[i].Id.ToString()
        //        });
        //    }

        //    model.Companies = companies;
        //    model.message = message;
        //    return View(model);
        //}
        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> GetTemporaryAccountManagementDataWithPagination()
        //{
        //    try
        //    {
        //        GetTemporaryAccountManagementPaginationModel model = new GetTemporaryAccountManagementPaginationModel();
        //        int totalRecord = 0;
        //        int filterRecord = 0;
        //        var draw = Request.Form["draw"].FirstOrDefault();
        //        var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
        //        var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
        //        var searchValue = Request.Form["search[value]"].FirstOrDefault();
        //        int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
        //        int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
        //        model = await _portalBalanceExpensesRepository.GetTemporaryAccountManagementPagination(skip, pageSize, searchValue, sortColumn, sortColumnDirection);
        //        filterRecord = model.totalAccountCount;
        //        totalRecord = model.totalAccountCount;
        //        var returnObj = new
        //        {
        //            draw = draw,
        //            recordsTotal = totalRecord,
        //            recordsFiltered = filterRecord,
        //            data = model.getTemporaryAccounts
        //        };
        //        return new JsonResult(returnObj);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<JsonResult> AddTemporaryAccountExpenses([FromBody] List<SaveTemporaryAccountExpenseModel> model)
        //{
        //    int _returnId = 0;
        //    Log.Information("Added Temporary Account Expenses of Companies");
        //    string username = GetUserClaims();
        //    int UserId = _permissionRepository.GetUserIdForPermission(username);
        //    foreach (var company in model)
        //    {
        //        company.CreatedBy = UserId.ToString();
        //    }
        //    _returnId = await _portalBalanceExpensesRepository.AddTemporaryAccountExpenses(model);
        //    return Json(_returnId);
        //}

        //[HttpGet]
        //[Route("[action]")]
        //public async Task<IActionResult> GetTemporaryCreditAccount(int Id)
        //{
        //    GetTemporaryAccountDetailByIdModel model = new GetTemporaryAccountDetailByIdModel();
        //    List<CompanyViewModel> otherCompanies = new List<CompanyViewModel>();
        //    otherCompanies = _companyRepository.GetCompanies();

        //    List<SelectListItem> companies = new List<SelectListItem>();
        //    List<SelectListItem> type = new List<SelectListItem>();
        //    List<SelectListItem> paymentMode = new List<SelectListItem>();
        //    for (int i = 0; i < otherCompanies.Count; i++)
        //    {
        //        companies.Add(new SelectListItem
        //        {
        //            Text = otherCompanies[i].CompanyName,
        //            Value = otherCompanies[i].Id.ToString()
        //        });
        //    }
        //    type.Add(new SelectListItem
        //    {
        //        Text = "Credit",
        //        Value = "Credit"
        //    });
        //    type.Add(new SelectListItem
        //    {
        //        Text = "Debit",
        //        Value = "Debit"
        //    });
        //    paymentMode.Add(new SelectListItem
        //    {
        //        Text = "Cash",
        //        Value = "Cash"
        //    });
        //    paymentMode.Add(new SelectListItem
        //    {
        //        Text = "Online Transaction",
        //        Value = "Online Transaction"
        //    });
        //    paymentMode.Add(new SelectListItem
        //    {
        //        Text = "Cheque",
        //        Value = "Cheque"
        //    });
        //    model = await _portalBalanceExpensesRepository.GetTemporaryAccountDetailById(Id);
        //    model.Companies = companies;
        //    model.PaymentModeList = paymentMode;
        //    model.ExpenseType = type;
        //    return PartialView("_TemporaryCreditAccount",model);
        //}

        //[HttpGet]
        //[Route("[action]")]
        //public async Task<IActionResult> GetTemporaryDebitAccount(int Id)
        //{
        //    GetTemporaryAccountDetailByIdModel model = new GetTemporaryAccountDetailByIdModel();
        //    List<CompanyViewModel> otherCompanies = new List<CompanyViewModel>();
        //    otherCompanies = _companyRepository.GetCompanies();

        //    List<SelectListItem> companies = new List<SelectListItem>();
        //    List<SelectListItem> type = new List<SelectListItem>();
        //    List<SelectListItem> paymentMode = new List<SelectListItem>();
        //    for (int i = 0; i < otherCompanies.Count; i++)
        //    {
        //        companies.Add(new SelectListItem
        //        {
        //            Text = otherCompanies[i].CompanyName,
        //            Value = otherCompanies[i].Id.ToString()
        //        });
        //    }
        //    type.Add(new SelectListItem
        //    {
        //        Text = "Credit",
        //        Value = "Credit"
        //    });
        //    type.Add(new SelectListItem
        //    {
        //        Text = "Debit",
        //        Value = "Debit"
        //    });
        //    paymentMode.Add(new SelectListItem
        //    {
        //        Text = "Cash",
        //        Value = "Cash"
        //    });
        //    paymentMode.Add(new SelectListItem
        //    {
        //        Text = "Online Transaction",
        //        Value = "Online Transaction"
        //    });
        //    paymentMode.Add(new SelectListItem
        //    {
        //        Text = "Cheque",
        //        Value = "Cheque"
        //    });
        //    model = await _portalBalanceExpensesRepository.GetTemporaryAccountDetailById(Id);
        //    model.Companies = companies;
        //    model.PaymentModeList = paymentMode;
        //    model.ExpenseType = type;
        //    return PartialView("_TemporaryDebitAccount", model);
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> UpdateTemporaryAccountExpenses(GetTemporaryAccountDetailByIdModel model)
        //{
        //    int _returnId = 0;
        //    Log.Information("Update Temporary Account Expenses of Companies");
        //    string username = GetUserClaims();
        //    int UserId = _permissionRepository.GetUserIdForPermission(username);
        //    model.ModifyBy = UserId;
        //    _returnId = await _portalBalanceExpensesRepository.UpdateTemporaryAccountExpenses(model);
        //    if(_returnId > 0)
        //        return RedirectToAction("TemporaryAccountManagement", "AccountManagement", new { message = "Selected Account Details Updated Successfully!" });
        //    return RedirectToAction("TemporaryAccountManagement", "AccountManagement", new { message = "Something went wrong, Please try again." });
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<JsonResult> DeleteTemporaryAccountExpense(int Id)
        //{
        //    int _returnId = 0;
        //    string username = GetUserClaims();
        //    int UserId = _permissionRepository.GetUserIdForPermission(username);

        //    _returnId = await _portalBalanceExpensesRepository.DeleteTemporaryAccount(Id, UserId);
        //    return Json(_returnId);
        //}

        //[HttpGet]
        //[Route("[action]")]
        //public async Task<JsonResult> ApproveTemporaryAccount(int Id)
        //{
        //    int _returnId = 0;
        //    string username = GetUserClaims();
        //    int UserId = _permissionRepository.GetUserIdForPermission(username);

        //    _returnId = await _portalBalanceExpensesRepository.ApproveTemporaryAccount(Id, UserId);
        //    return Json(_returnId);
        //}

        //[HttpGet]
        //[Route("[action]")]
        //public async Task<JsonResult> RejectTemporaryAccount(int Id)
        //{
        //    int _returnId = 0;
        //    string username = GetUserClaims();
        //    int UserId = _permissionRepository.GetUserIdForPermission(username);

        //    _returnId = await _portalBalanceExpensesRepository.RejectTemporaryAccount(Id, UserId);
        //    return Json(_returnId);
        //}

        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult GetTemporaryAccountLog()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<JsonResult> GetTemporaryAccountManagementLogWithPagination()
        //{
        //    try
        //    {
        //        GetTemporaryAccountManagementLogPaginationModel model = new GetTemporaryAccountManagementLogPaginationModel();
        //        int totalRecord = 0;
        //        int filterRecord = 0;
        //        var draw = Request.Form["draw"].FirstOrDefault();
        //        var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
        //        var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
        //        var searchValue = Request.Form["search[value]"].FirstOrDefault();
        //        int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
        //        int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
        //        model = await _portalBalanceExpensesRepository.GetTemporaryAccountManagementLogPagination(skip, pageSize, searchValue, sortColumn, sortColumnDirection);
        //        filterRecord = model.totalLogs;
        //        totalRecord = model.totalLogs;
        //        var returnObj = new
        //        {
        //            draw = draw,
        //            recordsTotal = totalRecord,
        //            recordsFiltered = filterRecord,
        //            data = model.logs
        //        };
        //        return new JsonResult(returnObj);
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

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
