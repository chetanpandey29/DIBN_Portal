using DIBN.IService;
using DIBN.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
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
    public class AccountManagementController : Controller
    {
        private readonly IAccountManagementService _accountManagementService;
        private readonly IUserCompanyService _companyServiceList;
        private readonly IUserPermissionService _userPermissionService;

        public AccountManagementController(
                IAccountManagementService accountManagementService,
                IUserCompanyService companyServiceList,
                IUserPermissionService userPermissionService
            )
        {
            _accountManagementService = accountManagementService;
            _companyServiceList = companyServiceList;
            _userPermissionService = userPermissionService;
        }
        public IActionResult Index(string? name)
        {
            string Role = GetClaims();
            if(Role==null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            Log.Information("Account Management");
            AccountManagementModel model = new AccountManagementModel();
            List<UserCompanyViewModel> otherCompanies = new List<UserCompanyViewModel>();
            otherCompanies = _companyServiceList.GetCompanies();

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
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetHistory(string name, string? FromDate, string? ToDate,string? message)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            ConfirmationModel model = new ConfirmationModel();
            model.message = message;
            return View(model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetDataTableData(int page)
        {
            try
            {
                GetAccountHistoryData model = new GetAccountHistoryData();
                List<GetHistoryOfCompanyExpenses> getHistories = new List<GetHistoryOfCompanyExpenses>();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = _accountManagementService.GetHistoryOfAllCompanyExpensesTest(page, pageSize, searchValue, sortColumn, sortColumnDirection);
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
        public IActionResult DownloadExpenseReceipt(int ReceiptId)
        {
            ExpenseReceipt document = new ExpenseReceipt();
            document = _accountManagementService.GetCompanyExpenseReceipt(ReceiptId);
            string Files = document.FileName;
            //System.IO.File.WriteAllBytes(Files, document.DataBinary);
            MemoryStream ms = new MemoryStream(document.DataBinary);
            return File(document.DataBinary, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName + document.Extension);
        }
        [HttpGet]
        [Route("[action]")]
        public FileStreamResult DownloadPaymentReceipt(int Id)
        {
            GetPaymentTransactionReceipt model = new GetPaymentTransactionReceipt();
            model = _accountManagementService.GetPaymentTransactionReceiptDetails(Id);

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
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetFilterwiseDataTable(string sortColumn, string sortColumnDirection, string? searchBy, string? searchedValue)
        {
            try
            {
                GetAccountHistoryData model = new GetAccountHistoryData();
                List<GetHistoryOfCompanyExpenses> getHistories = new List<GetHistoryOfCompanyExpenses>();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                model = _accountManagementService.GetHistoryOfAllCompanyExpensesFilter(skip, pageSize, searchBy, searchedValue, sortColumn, sortColumnDirection);
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
        public IActionResult GetExpenseModification(int Id,string TransactionId)
        {
            string _returnMessages = "";
            var model = _accountManagementService.GetExpenseDetails(Id);
            if (model.TransactionId == null || model.TransactionId == "")
            {
                ResponseModel response = new ResponseModel();
                response = _accountManagementService.GetExpenseModificationDetails(Id);
                _returnMessages = "Selected Transaction ("+ TransactionId + ") is already deleted by " + response.Username + " at " + response.ModifyTime + ".";
            }
            return new JsonResult(_returnMessages);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTransactionModification(int Id, int CompanyId,string TransactionId)
        {
            string _returnMessages = "";
            var model = _accountManagementService.GetDetailsOfPayment(Id, CompanyId);
            if (model.TransactionId == null || model.TransactionId == "")
            {
                ResponseModel response = new ResponseModel();
                response = _accountManagementService.GetTransactionModificationDetails(Id);
                _returnMessages = "Selected Transaction ("+ TransactionId + ") is already deleted by " + response.Username + " at " + response.ModifyTime + ".";
            }
            return new JsonResult(_returnMessages);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult UpdateExpenses(int Id, string name, string ActionName, string Cnt)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            UpdateCompanyExpenses model = new UpdateCompanyExpenses();
            List<UserCompanyViewModel> otherCompanies = new List<UserCompanyViewModel>();
            otherCompanies = _companyServiceList.GetCompanies();
            List<SelectListItem> type = new List<SelectListItem>();
            List<SelectListItem> paymentMode = new List<SelectListItem>();

            List<SelectListItem> companies = new List<SelectListItem>();
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
            model = _accountManagementService.GetExpenseDetails(Id);
            model.Companies = companies;
            model.Module = name;
            model.ActionName = ActionName;
            model.Controller = Cnt;
            model.expenseType = type;
            model.Type = "Debit";
            model.PaymentModeType = paymentMode;
            return PartialView("_UpdateExpenses", model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateExpenseDetails(UpdateCompanyExpenses model)
        {
            string _returnMessages = "";
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            string Username = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            int _returnId = 0;
            if (model.Type == "Debit")
            {
                if (model.PreviousCompanyId != model.CompanyId)
                {
                    string _returnMessage = _accountManagementService.DeleteExpenses(model.Id, model.PreviousAmount, model.PreviousCompanyId,UserId);

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
                        _returnId = _accountManagementService.AddCompanyExpensesAccount(saveExpense);
                    }
                    else
                    {
                        if (model.ActionName == "GetCompanyExpenseDetails")
                        {
                            return RedirectToAction("GetCompanyExpenseDetails", "CompanyAccount", new { Id = model.PreviousCompanyId , message = _returnMessage });
                        }
                        return RedirectToAction("GetHistory", "AccountManagement", new { name = model.Module, message = _returnMessage });
                    }
                }
                else
                {
                    _returnMessages = _accountManagementService.UpdateExpenseDetail(model);
                }
            }
            else
            {
                string _returnMessage = _accountManagementService.DeleteExpenses(model.Id, model.PreviousAmount, model.PreviousCompanyId,UserId);
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
                    model1.TransactionId = model.TransactionId;
                    model1.Task = model.Task;
                    model1.PaymentMode = model.PaymentMode;
                    model1.UserId = UserId.ToString();
                    saveExpense.Add(model1);

                    _returnId = _accountManagementService.AddCompanyExpensesAccount(saveExpense);
                }
                else
                {
                    if (model.ActionName == "GetCompanyExpenseDetails")
                    {
                        return RedirectToAction("GetCompanyExpenseDetails", "CompanyAccount", new { Id = model.PreviousCompanyId, message = _returnMessage });
                    }
                    return RedirectToAction("GetHistory", "AccountManagement", new { name = model.Module, message = _returnMessage });
                }
            }

            if (_returnMessages.Contains("Selected Company Expense Updated Successfully."))
            {
                UserCompanyViewModel company = new UserCompanyViewModel();
                company = _companyServiceList.GetCompanyById(model.CompanyId);
                Log.Information("Changed Account Summary Details of " + company.CompanyName);
                Log.Information("Description : " + model.Task + " , Amount : " + model.Amount + " , Quantity :" + model.Quantity + " , Total Amount :" + model.TotalAmount + ".");
                if (model.ActionName == "GetCompanyExpenseDetails")
                {
                    return RedirectToAction("GetCompanyExpenseDetails", "CompanyAccount", new { Id = model.PreviousCompanyId });
                }
                return RedirectToAction("GetHistory", "AccountManagement", new { name = model.Module });
            }

            if (model.ActionName == "GetCompanyExpenseDetails")
            {
                return RedirectToAction("GetCompanyExpenseDetails", "CompanyAccount", new { Id = model.PreviousCompanyId, message = _returnMessages });
            }
            return RedirectToAction("GetHistory", "AccountManagement", new { name = model.Module, message = _returnMessages });
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddCompanyExpenses([FromBody] List<SaveCompanyExpenses> model)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            Log.Information("Added New Expenses of Companies");
            string username = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(username);
            foreach (var company in model)
            {
                company.UserId = UserId.ToString();
            }
            _returnId = _accountManagementService.AddCompanyExpensesAccount(model);
            return new JsonResult(_returnId);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id, int CompanyId, string Amount)
        {
            string _returnMessage = "";
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            string Username = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            _returnMessage = _accountManagementService.DeleteExpenses(Id, Amount, CompanyId, UserId);
            return new JsonResult(_returnMessage);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetPaymentDetails(int Id, int CompanyId, string ActionName, string Cnt)
        {
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            PaymentTransaction paymentTransaction = new PaymentTransaction();
            List<SelectListItem> type = new List<SelectListItem>();
            List<UserCompanyViewModel> otherCompanies = new List<UserCompanyViewModel>();
            otherCompanies = _companyServiceList.GetCompanies();
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
            List<SelectListItem> companies = new List<SelectListItem>();
            for (int i = 0; i < otherCompanies.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = otherCompanies[i].CompanyName,
                    Value = otherCompanies[i].Id.ToString()
                });
            }
            List<SelectListItem> paymentMode = new List<SelectListItem>();

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

            paymentTransaction = _accountManagementService.GetDetailsOfPayment(Id, CompanyId);
            paymentTransaction.Action = ActionName;
            paymentTransaction.Controller = Cnt;
            paymentTransaction.expenseType = type;
            paymentTransaction.Type = "Credit";
            paymentTransaction.CompanyId = CompanyId;
            paymentTransaction.Companies = companies;
            paymentTransaction.PaymentModes = paymentMode;
            return PartialView("_GetPaymentDetails", paymentTransaction);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdatePaymentDetails(PaymentTransaction payment)
        {
            string _returnMessages = "";
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            string Username = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            UserCompanyViewModel model = new UserCompanyViewModel();
            model = _companyServiceList.GetCompanyById(payment.CompanyId);
            payment.UserId = UserId;

            if (payment.Type == "Debit")
            {
                string _returnMessage = _accountManagementService.DeletePaymentTransaction(payment.Id, payment.PreviousCompanyId, payment.PreviousAmount,payment.UserId);
                if (_returnMessage.Contains("Selected Payment Transaction Deleted Successfully."))
                {
                    List<SaveCompanyExpenses> saveExpense = new List<SaveCompanyExpenses>();
                    SaveCompanyExpenses model1 = new SaveCompanyExpenses();
                    model1.CompanyId = payment.CompanyId.ToString();
                    model1.Quantity = payment.Quantity.ToString();
                    model1.Amount = payment.Amount;
                    model1.Vat = payment.Vat;
                    model1.Type = payment.Type;
                    model1.TotalAmount = payment.TotalAmount;
                    model1.GrandTotal = payment.GrandTotal;
                    model1.VatAmount = payment.VatAmount;
                    model1.Date = payment.CreatedOnUtc;
                    model1.Task = payment.Description;
                    model1.UserId = UserId.ToString();
                    model1.PaymentMode = payment.PaymentMode;
                    model1.TransactionId = payment.TransactionId;
                    saveExpense.Add(model1);
                    _returnId = _accountManagementService.AddCompanyExpensesAccount(saveExpense);
                }
                else
                {
                    if (payment.Action == "GetCompanyExpenseDetails")
                    {
                        return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, message = _returnMessages });
                    }
                    if (payment.Action == "Index")
                    {
                        return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = "Company", message = _returnMessages });
                    }
                    return RedirectToAction(payment.Action, payment.Controller, new { name = "MyAccount", message = _returnMessages });
                }
            }
            else
            {
                if (payment.PreviousCompanyId != payment.CompanyId)
                {
                    string _returnMessage = _accountManagementService.DeletePaymentTransaction(payment.Id, payment.PreviousCompanyId, payment.PreviousAmount,payment.UserId);
                    if (_returnMessage.Contains("Selected Payment Transaction Deleted Successfully."))
                    {
                        List<SaveCompanyExpenses> saveExpense = new List<SaveCompanyExpenses>();
                        SaveCompanyExpenses model1 = new SaveCompanyExpenses();
                        model1.CompanyId = payment.CompanyId.ToString();
                        model1.Quantity = payment.Quantity.ToString();
                        model1.Amount = payment.Amount;
                        model1.PaymentMode = payment.PaymentMode;
                        model1.Vat = payment.Vat;
                        model1.Type = payment.Type;
                        model1.TotalAmount = payment.TotalAmount;
                        model1.GrandTotal = payment.GrandTotal;
                        model1.VatAmount = payment.VatAmount;
                        model1.Date = payment.CreatedOnUtc;
                        model1.Task = payment.Description;
                        model1.GrandTotal = payment.GrandTotal;
                        model1.UserId = UserId.ToString();
                        model1.TransactionId = payment.TransactionId;
                        saveExpense.Add(model1);
                        _returnId = _accountManagementService.AddCompanyExpensesAccount(saveExpense);
                    }
                }
                else
                {
                    _returnMessages = _accountManagementService.UpdatePaymentDetails(payment);
                }
            }

            if (_returnMessages.Contains("Selected Payment Transaction Updated Successfully."))
            {
                Log.Information("Payment Transaction of " + model.CompanyName + " is updated by " + payment.Amount + " AED.");
                if (payment.Action == "GetCompanyExpenseDetails")
                {
                    return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId });
                }
                if (payment.Action == "Index")
                {
                    return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = "Company" });
                }
                return RedirectToAction(payment.Action, payment.Controller, new { name = "MyAccount" });
            }

            if (payment.Action == "GetCompanyExpenseDetails")
            {
                return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId ,message = _returnMessages});
            }
            if (payment.Action == "Index")
            {
                return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = "Company", message = _returnMessages });
            }
            return RedirectToAction(payment.Action, payment.Controller, new { name = "MyAccount", message = _returnMessages });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DeletePaymentDetails(int Id, int CompanyId, string Amount)
        {
            string _returnMessage = "";
            string Role = GetClaims();
            if (Role == null || Role == "")
            {
                return RedirectToAction("Logout", "Account");
            }
            int _returnId = 0;
            string Username = GetUserClaims();
            int UserId = _userPermissionService.GetUserIdForPermission(Username);
            _returnMessage = _accountManagementService.DeletePaymentTransaction(Id, CompanyId, Amount,UserId);
            return new JsonResult(_returnMessage);
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
