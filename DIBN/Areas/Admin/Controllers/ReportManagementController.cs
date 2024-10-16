using ClosedXML.Excel;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing.Imaging;
using DIBN.Areas.Admin.Repository;
using Quartz.Util;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.VariantTypes;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Pkcs;
using JetBrains.Annotations;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ReportManagementController : Controller
    {
        private readonly IReportManagementRepository _reportManagementRepository;
        private readonly ICompanyRepository _companyRepository;

        public ReportManagementController(IReportManagementRepository reportManagementRepository,
            ICompanyRepository companyRepository)
        {
            _reportManagementRepository = reportManagementRepository;
            _companyRepository = companyRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult MainProfitLossReport()
        {
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCustomReport(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCustomReportModel profitLoss = new ProfitLossCustomReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCustomReportTotalDetails(fromDate, toDate);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;
            profitLoss.FromDates = fromDate;
            profitLoss.ToDates = toDate;
            return View(profitLoss);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCustomPaginationReport(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");
                ProfitLossCustomReportPaginationModel model = new ProfitLossCustomReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForProfitLossCustomReport(fromDate, toDate, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> ProfitLossCustomReportExcel(string fromDate, string toDate)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCustomReportExcelAndPdfModel profitLoss = new ProfitLossCustomReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCustomReportExcelPdf(fromDate, toDate);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });
            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Custom Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Custom Report of " + minDate + " - " + maxDate;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.profitLossPercentage > 0)
                {

                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["ProfitLossCustomReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCustomReportExcel(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCustomReportExcel"].ToString());
                string fileName = "Profit & Loss Custom Report " + minDate + "-" + maxDate + ".xlsx";

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ProfitLossCustomReportPdf(string fromDate, string toDate)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCustomReportExcelAndPdfModel profitLoss = new ProfitLossCustomReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCustomReportExcelPdf(fromDate, toDate);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;
                pdfWriter.PageEvent = new ProfitLossReportCustomPdfLayout(minDate, maxDate, profitLoss.totalCredit.ToString("0.00"), profitLoss.totalDebit.ToString("0.00"), profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 80f, 15f);
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }
                document.Open();

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                TempData["ProfitLossCustomReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCustomReportPdf(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCustomReportPdf"].ToString());
                string fileName = "Profit & Loss Custom Report " + minDate + "-" + maxDate + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossWeeklyReport(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossWeeklyReportModel profitLoss = new ProfitLossWeeklyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossWeeklyReportTotalDetails(fromDate, toDate);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;
            profitLoss.FromDates = fromDate;
            profitLoss.ToDates = toDate;
            return View(profitLoss);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossWeeklyPaginationReport(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                ProfitLossWeeklyReportPaginationModel model = new ProfitLossWeeklyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForProfitLossWeeklyReport(fromDate, toDate, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> ProfitLossWeeklyReportExcel(string fromDate, string toDate)
        {
            int index = 1;
            double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossWeeklyReportExcelAndPdfModel profitLoss = new ProfitLossWeeklyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossWeeklyReportExcelPdf(fromDate, toDate);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });

            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Weekly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Weekly Report of " + minDate + " - " + maxDate;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {                                                                                                          //-- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["ProfitLossWeeklyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossWeeklyReportExcel(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossWeeklyReportExcel"].ToString());
                string fileName = "Profit & Loss Weekly Report " + minDate + "-" + maxDate + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ProfitLossWeeklyReportPdf(string fromDate, string toDate)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossWeeklyReportExcelAndPdfModel profitLoss = new ProfitLossWeeklyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossWeeklyReportExcelPdf(fromDate, toDate);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;
                pdfWriter.PageEvent = new ProfitLossReportWeeklyPdfLayout(minDate, maxDate, profitLoss.TotalCredit, profitLoss.TotalDebit, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["ProfitLossWeeklyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossWeeklyReportPdf(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossWeeklyReportPdf"].ToString());
                string fileName = "Profit & Loss Weekly Report " + minDate + "-" + maxDate + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMonthlyReport(string month, string year)
        {
            string monthName = GetMonth(month);
            ProfitLossMonthlyReportModel profitLoss = new ProfitLossMonthlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossMonthlyReportTotalDetails(month, year);
            profitLoss.MonthNumber = month;
            profitLoss.YearNumber = year;
            profitLoss.MonthText = monthName;
            return View(profitLoss);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMonthlyPaginationReport(string month, string year)
        {
            try
            {
                ProfitLossMonthlyReportPaginationModel model = new ProfitLossMonthlyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForProfitLossMonthlyReport(month, year, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> ProfitLossMonthlyReportExcel(string month, string year)
        {
            int index = 1;
            string monthName = GetMonth(month);
            double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
            ProfitLossMonthlyReportExcelAndPdfModel profitLoss = new ProfitLossMonthlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossMonthlyReportExcelPdf(month, year);
            profitLoss.monthNumber = month;
            profitLoss.yearNumber = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });

            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            //profitLossPercentage = totalProfitLoss/ 100;

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Weekly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Monthly Report of " + monthName + " , " + year;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["ProfitLossMonthlyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossMonthlyReportExcel(string month, string year)
        {
            try
            {
                string monthName = "";
                monthName = GetMonth(month);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossMonthlyReportExcel"].ToString());
                string fileName = "Profit & Loss Monthly Report (" + monthName + "-" + year + ").xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ProfitLossMonthlyReportPdf(string month, string year)
        {
            int index = 1;
            string Month = GetMonth(month);
            ProfitLossMonthlyReportExcelAndPdfModel profitLoss = new ProfitLossMonthlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossMonthlyReportExcelPdf(month, year);

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportMonthlyPdfLayout(Month, year, profitLoss.TotalCredit, profitLoss.TotalDebit, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["ProfitLossMonthlyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossMonthlyReportPdf(string month, string year)
        {
            try
            {
                string monthName = "";
                monthName = GetMonth(month);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossMonthlyReportPdf"].ToString());
                string fileName = "Profit & Loss Monthly Report (" + monthName + "-" + year + ").pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossYearlyReport(string year)
        {
            ProfitLossYearlyReportModel profitLoss = new ProfitLossYearlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossYearlyReportTotalDetails(year);
            profitLoss.YearNumber = year;
            return View(profitLoss);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossYearlyPaginationReport(string year)
        {
            try
            {
                ProfitLossYearlyReportPaginationModel model = new ProfitLossYearlyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForProfitLossYearlyReport(year, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> ProfitLossYearlyReportExcel(string year)
        {
            int index = 1;
            ProfitLossYearlyReportExcelAndPdfModel profitLoss = new ProfitLossYearlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossYearlyReportExcelPdf(year);
            profitLoss.yearNumber = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });

            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Weekly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Yearly Report of " + year;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["ProfitLossYearlyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossYearlyReportExcel(string year)
        {
            try
            {
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossYearlyReportExcel"].ToString());
                string fileName = "Profit & Loss Yearly Report (" + year + ").xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ProfitLossYearlyReportPdf(string year)
        {
            int index = 1;
            ProfitLossYearlyReportExcelAndPdfModel profitLoss = new ProfitLossYearlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossYearlyReportExcelPdf(year);

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportYearlyPdfLayout(year, profitLoss.TotalCredit, profitLoss.TotalDebit, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 575f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["ProfitLossYearlyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossYearlyReportPdf(string year)
        {
            try
            {
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossYearlyReportPdf"].ToString());
                string fileName = "Profit & Loss Yearly Report (" + year + ").pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCustomReportGraph(string fromDate, string toDate)
        {
            GetAccountHistoryForProfitLossCustomReportGraphModel model = new GetAccountHistoryForProfitLossCustomReportGraphModel();
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");

            ProfitLossCustomReportModel profitLoss = new ProfitLossCustomReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCustomReportTotalDetails(fromDate, toDate);

            model.FromDate = minDate;
            model.ToDate = maxDate;
            model.FromDates = fromDate;
            model.ToDates = toDate;
            model.TotalCredit = profitLoss.TotalCredit;
            model.TotalDebit = profitLoss.TotalDebit;
            model.totalProfitLoss = profitLoss.totalProfitLoss;
            model.profitLossPercentage = profitLoss.profitLossPercentage;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCustomReportBarGraphData(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("yyyy-MM-dd");
            maxDate = ToDate.ToString("yyyy-MM-dd");
            var data = await _reportManagementRepository.GetAccountHistoryForProfitLossCustomReportGraph(fromDate, toDate);
            StringBuilder stringBuilder = new StringBuilder();
            if (data != null && data.Count != 0)
            {
                stringBuilder.Append("[[");
                for (int i = 0; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#00FF00"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("],[");

                for (int i = 1; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#FF0000"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]]");
            }
            else
            {
                stringBuilder.Append("[[{},],[{},]]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCustomReportPieGraphData(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("yyyy-MM-dd");
            maxDate = ToDate.ToString("yyyy-MM-dd");
            ProfitLossCustomReportModel profitLoss = new ProfitLossCustomReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCustomReportTotalDetails(fromDate, toDate);

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }
            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossWeeklyReportGraph(string fromDate, string toDate)
        {
            GetAccountHistoryForProfitLossWeeklyReportGraphModel model = new GetAccountHistoryForProfitLossWeeklyReportGraphModel();
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");

            ProfitLossWeeklyReportModel profitLoss = new ProfitLossWeeklyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossWeeklyReportTotalDetails(fromDate, toDate);

            model.FromDate = minDate;
            model.ToDate = maxDate;
            model.FromDates = fromDate;
            model.ToDates = toDate;
            model.TotalCredit = profitLoss.TotalCredit;
            model.TotalDebit = profitLoss.TotalDebit;
            model.totalProfitLoss = profitLoss.totalProfitLoss;
            model.profitLossPercentage = profitLoss.profitLossPercentage;

            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossWeeklyReportBarGraphData(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("yyyy-MM-dd");
            maxDate = ToDate.ToString("yyyy-MM-dd");
            var data = await _reportManagementRepository.GetAccountHistoryForProfitLossWeeklyReportGraph(fromDate, toDate);
            StringBuilder stringBuilder = new StringBuilder();

            if (data != null && data.Count != 0)
            {
                stringBuilder.Append("[[");
                for (int i = 0; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#00FF00"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("],[");

                for (int i = 1; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#FF0000"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]]");
            }
            else
            {
                stringBuilder.Append("[[{},],[{},]]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossWeeklyReportPieGraphData(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("yyyy-MM-dd");
            maxDate = ToDate.ToString("yyyy-MM-dd");
            ProfitLossWeeklyReportModel profitLoss = new ProfitLossWeeklyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossWeeklyReportTotalDetails(fromDate, toDate);

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossYealyReportGraph(string year)
        {
            GetAccountHistoryForProfitLossYearlyReportGraphModel model = new GetAccountHistoryForProfitLossYearlyReportGraphModel();
            model.year = year;

            ProfitLossYearlyReportModel profitLoss = new ProfitLossYearlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossYearlyReportTotalDetails(year);

            model.TotalCredit = profitLoss.TotalCredit;
            model.TotalDebit = profitLoss.TotalDebit;
            model.totalProfitLoss = profitLoss.totalProfitLoss;
            model.profitLossPercentage = profitLoss.profitLossPercentage;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossYearlyReportBarGraphData(string year)
        {
            var data = await _reportManagementRepository.GetAccountHistoryForProfitLossYealyReportGraph(year);
            StringBuilder stringBuilder = new StringBuilder();

            if (data != null && data.Count != 0)
            {
                stringBuilder.Append("[[");
                for (int i = 0; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#00FF00"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("],[");

                for (int i = 1; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#FF0000"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]]");
            }
            else
            {
                stringBuilder.Append("[[{},],[{},]]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossYearlyReportPieGraphData(string year)
        {
            ProfitLossYearlyReportModel profitLoss = new ProfitLossYearlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossYearlyReportTotalDetails(year);

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMonthlyReportGraph(string month, string year)
        {
            string monthName = GetMonth(month);
            GetAccountHistoryForProfitLossMonthlyReportGraphModel model = new GetAccountHistoryForProfitLossMonthlyReportGraphModel();
            model.year = year;
            model.month = month;
            model.monthName = monthName;

            ProfitLossMonthlyReportModel profitLoss = new ProfitLossMonthlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossMonthlyReportTotalDetails(month, year);

            model.TotalCredit = profitLoss.TotalCredit;
            model.TotalDebit = profitLoss.TotalDebit;
            model.totalProfitLoss = profitLoss.totalProfitLoss;
            model.profitLossPercentage = profitLoss.profitLossPercentage;

            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMonthlyReportBarGraphData(string month, string year)
        {
            var data = await _reportManagementRepository.GetAccountHistoryForProfitLossMonthlyReportGraph(year, month);
            StringBuilder stringBuilder = new StringBuilder();
            if (data != null && data.Count != 0)
            {
                stringBuilder.Append("[[");
                for (int i = 0; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#00FF00"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("],[");

                for (int i = 1; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#FF0000"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]]");
            }
            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMonthlyReportPieGraphData(string month, string year)
        {
            ProfitLossMonthlyReportModel profitLoss = new ProfitLossMonthlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossMonthlyReportTotalDetails(month, year);

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CompanyWiseProfitLossReport()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CompanyWiseProfitLossPaginationReport()
        {
            try
            {
                GetCompanyListForProfitLossPagination model = new GetCompanyListForProfitLossPagination();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetCompanyListForProfitLoss(skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalCompany;
                totalRecord = model.totalCompany;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.companyListForProfitLossModels
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
        public IActionResult CompanyProfitLossReport(int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            GetCompanyListForProfitLossModel model = new GetCompanyListForProfitLossModel();
            model.Id = companyId;
            model.CompanyName = company.CompanyName;
            model.CompanyAccountNumber = company.AccountNumber;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyCustomReport(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCompanyCustomReportModel profitLoss = new ProfitLossCompanyCustomReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReportTotalDetails(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;
            profitLoss.FromDates = fromDate;
            profitLoss.ToDates = toDate;
            profitLoss.companyId = companyId;
            profitLoss.CompanyName = company.CompanyName;
            return View(profitLoss);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyCustomPaginationReport(string fromDate, string toDate, int companyId)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");
                ProfitLossCompanyCustomReportPaginationModel model = new ProfitLossCompanyCustomReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReport(fromDate, toDate, companyId, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> ProfitLossCompanyCustomReportExcel(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyCustomReportExcelAndPdfModel profitLoss = new ProfitLossCompanyCustomReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReportExcelPdf(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });
            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Custom Report ");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Custom Report from " + minDate + " till " + maxDate + " of " + company.CompanyName;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["ProfitLossCompanyCustomReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMultipleCompanyCustomReportExcel(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyCustomReportExcelAndPdfModel profitLoss = new ProfitLossCompanyCustomReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReportExcelPdf(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });
            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Custom Report ");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Custom Report from " + minDate + " till " + maxDate + " of " + company.CompanyName;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    string fileName = "Profit & Loss Custom Report (" + minDate + "-" + maxDate + ") of " + company.CompanyName + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCompanyCustomReportExcel(string fromDate, string toDate, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCompanyCustomReportExcel"].ToString());
                string fileName = "Profit & Loss Custom Report (" + minDate + "-" + maxDate + ") of " + company.CompanyName + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ProfitLossCompanyCustomReportPdf(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            var company = _companyRepository.GetCompanyById(companyId);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCompanyCustomReportExcelAndPdfModel profitLoss = new ProfitLossCompanyCustomReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReportExcelPdf(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportCompanyCustomPdfLayout(minDate, maxDate, profitLoss.TotalCredit, profitLoss.TotalDebit, company.CompanyName, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["ProfitLossCompanyCustomReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMultipleCompanyCustomReportPdf(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            var company = _companyRepository.GetCompanyById(companyId);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCompanyCustomReportExcelAndPdfModel profitLoss = new ProfitLossCompanyCustomReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReportExcelPdf(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportCompanyCustomPdfLayout(minDate, maxDate, profitLoss.TotalCredit, profitLoss.TotalDebit, company.CompanyName, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                string fileName = "Profit & Loss Custom Report (" + minDate + "-" + maxDate + ") of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCompanyCustomReportPdf(string fromDate, string toDate, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCompanyCustomReportPdf"].ToString());
                string fileName = "Profit & Loss Custom Report (" + minDate + "-" + maxDate + ") of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyWeeklyReport(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCompanyWeeklyReportModel profitLoss = new ProfitLossCompanyWeeklyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReportTotalDetails(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;
            profitLoss.FromDates = fromDate;
            profitLoss.ToDates = toDate;
            profitLoss.companyId = companyId;
            profitLoss.CompanyName = company.CompanyName;
            return View(profitLoss);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyWeeklyPaginationReport(string fromDate, string toDate, int companyId)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                ProfitLossCompanyWeeklyReportPaginationModel model = new ProfitLossCompanyWeeklyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReport(fromDate, toDate, companyId, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> ProfitLossCompanyWeeklyReportExcel(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyWeeklyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyWeeklyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReportExcelPdf(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });
            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Weekly Report ");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Weekly Report from " + minDate + " till " + maxDate + " of " + company.CompanyName;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["ProfitLossCompanyWeeklyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCompanyWeeklyReportExcel(string fromDate, string toDate, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCompanyWeeklyReportExcel"].ToString());
                string fileName = "Profit & Loss Weekly Report (" + minDate + "-" + maxDate + ") of " + company.CompanyName + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMultipleCompanyWeeklyReportExcel(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyWeeklyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyWeeklyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReportExcelPdf(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });
            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Weekly Report ");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Weekly Report from " + minDate + " till " + maxDate + " of " + company.CompanyName;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    string fileName = "Profit Loss Weekly Report - " + company.CompanyName + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ProfitLossCompanyWeeklyReportPdf(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCompanyWeeklyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyWeeklyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReportExcelPdf(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportCompanyWeeklyPdfLayout(minDate, maxDate, profitLoss.TotalCredit, profitLoss.TotalDebit, company.CompanyName, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                TempData["ProfitLossCompanyWeeklyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCompanyWeeklyReportPdf(string fromDate, string toDate, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCompanyWeeklyReportPdf"].ToString());
                string fileName = "Profit & Loss Weekly Report (" + minDate + "-" + maxDate + ") of" + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMultipleCompanyWeeklyReportPdf(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            ProfitLossCompanyWeeklyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyWeeklyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReportExcelPdf(fromDate, toDate, companyId);
            profitLoss.FromDate = minDate;
            profitLoss.ToDate = maxDate;

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportCompanyWeeklyPdfLayout(minDate, maxDate, profitLoss.TotalCredit, profitLoss.TotalDebit, company.CompanyName, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                string fileName = "Profit Loss Weekly Report " + fromDate + "-" + toDate + " of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyMonthlyReport(string month, string year, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyMonthlyReportModel profitLoss = new ProfitLossCompanyMonthlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReportTotalDetails(month, year, companyId);
            profitLoss.MonthNumber = month;
            profitLoss.YearNumber = year;
            profitLoss.MonthText = GetMonth(month);
            profitLoss.companyId = companyId;
            profitLoss.CompanyName = company.CompanyName;
            return View(profitLoss);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyMonthlyPaginationReport(string month, string year, int companyId)
        {
            try
            {
                ProfitLossCompanyMonthlyReportPaginationModel model = new ProfitLossCompanyMonthlyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReport(month, year, companyId, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> ProfitLossCompanyMonthlyReportExcel(string month, string year, int companyId)
        {
            int index = 1;
            string monthName = GetMonth(month);
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyMonthlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyMonthlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReportExcelPdf(month, year, companyId);
            profitLoss.monthNumber = month;
            profitLoss.yearNumber = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });

            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            //profitLossPercentage = totalProfitLoss/ 100;

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Monthly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Monthly Report of " + monthName + " , " + year + " of " + company.CompanyName;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["ProfitLossCompanyMothlyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCompanyMonthlyReportExcel(string month, string year, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);
                string monthName = "";
                monthName = GetMonth(month);

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCompanyMothlyReportExcel"].ToString());
                string fileName = "Profit & Loss Monthly Report (" + monthName + "-" + year + ") of " + company.CompanyName + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMultipleCompanyMonthlyReportExcel(string month, string year, int companyId)
        {
            int index = 1;
            string monthName = GetMonth(month);
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyMonthlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyMonthlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReportExcelPdf(month, year, companyId);
            profitLoss.monthNumber = month;
            profitLoss.yearNumber = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });

            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Monthly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Monthly Report of " + monthName + " , " + year + " of " + company.CompanyName;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileName = "Profit Loss Monthly Report - " + company.CompanyName + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ProfitLossCompanyMonthlyReportPdf(string month, string year, int companyId)
        {
            int index = 1;
            string Month = GetMonth(month);
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyMonthlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyMonthlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReportExcelPdf(month, year, companyId);

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportCompanyMonthlyPdfLayout(Month, year, company.CompanyName, profitLoss.TotalCredit, profitLoss.TotalDebit, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                TempData["ProfitLossCompanyMonthlyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCompanyMonthlyReportPdf(string month, string year, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);
                string monthName = "";
                monthName = GetMonth(month);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCompanyMonthlyReportPdf"].ToString());
                string fileName = "Profit & Loss Monthly Report (" + monthName + "-" + year + ") of" + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMultipleCompanyMonthlyReportPdf(string month, string year, int companyId)
        {
            int index = 1;
            string Month = GetMonth(month);
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyMonthlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyMonthlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReportExcelPdf(month, year, companyId);

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportCompanyMonthlyPdfLayout(Month, year, company.CompanyName, profitLoss.TotalCredit, profitLoss.TotalDebit, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                string fileName = "Profit Loss " + Month + "-" + year + " of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyYearlyReport(string year, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyYearlyReportModel profitLoss = new ProfitLossCompanyYearlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYearlyReportTotalDetails(year, companyId);
            profitLoss.YearNumber = year;
            profitLoss.companyId = companyId;
            profitLoss.CompanyName = company.CompanyName;
            return View(profitLoss);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyYearlyPaginationReport(string year, int companyId)
        {
            try
            {
                ProfitLossCompanyYearlyReportPaginationModel model = new ProfitLossCompanyYearlyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYearlyReport(year, companyId, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> ProfitLossCompanyYearlyReportExcel(string year, int companyId)
        {
            int index = 1;
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyYearlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyYearlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYearlyReportExcelPdf(year, companyId);
            profitLoss.yearNumber = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });

            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Yearly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Yearly Report of " + year + " - " + company.CompanyName;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["ProfitLossCompanyYearlyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCompanyYearlyReportExcel(string year, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCompanyYearlyReportExcel"].ToString());
                string fileName = "Profit & Loss Yearly Report (" + year + ") of " + company.CompanyName + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMultipleCompanyYearlyReportExcel(string year, int companyId)
        {
            int index = 1;
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyYearlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyYearlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYearlyReportExcelPdf(year, companyId);
            profitLoss.yearNumber = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Transaction Amount (AED)\r\n"),
                               });

            if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in profitLoss.accountList)
                {
                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add("Profit Loss Yearly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitles = ws.Range("F8:G" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                // Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(09-06-2023)
                rngMainTitles.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(09-06-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(09-06-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(09-06-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                ws.Cell("D2").Value = "Profit & Loss Yearly Report of " + year + " - " + company.CompanyName;

                var rngMainTitle1 = ws.Range("D2:E3");
                rngMainTitle1.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle1.FirstRow().Merge();

                if (profitLoss.totalProfitLoss > 0)
                {
                    ws.Cell("D3").Value = "Profit : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(09-06-2023)
                    ws.Cell("D3").Value = "Loss : " + profitLoss.totalProfitLoss.ToString("0.00") + " AED";
                    ws.Cell("D3").Value += " (" + profitLoss.profitLossPercentage.ToString("0.00") + "%)";
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;
                }
                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += profitLoss.TotalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle2 = ws.Range("D4:D5");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle2.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += profitLoss.TotalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle3 = ws.Range("E4:E5");
                rngMainTitle3.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle3.FirstRow().Merge();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileName = "Profit Loss Yearly Report - " + company.CompanyName + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ProfitLossCompanyYearlyReportPdf(string year, int companyId)
        {
            int index = 1;
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyYearlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyYearlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYearlyReportExcelPdf(year, companyId);

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportCompanyYearlyPdfLayout(year, company.CompanyName, profitLoss.TotalCredit, profitLoss.TotalDebit, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                TempData["ProfitLossCompanyYearlyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadProfitLossCompanyYearlyReportPdf(string year, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["ProfitLossCompanyYearlyReportPdf"].ToString());
                string fileName = "Profit & Loss Yearly Report (" + year + ") of" + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossMultipleCompanyYearlyReportPdf(string year, int companyId)
        {
            int index = 1;
            var company = _companyRepository.GetCompanyById(companyId);
            ProfitLossCompanyYearlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyYearlyReportExcelAndPdfModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYearlyReportExcelPdf(year, companyId);

            Document document = new Document(PageSize.A4);
            document.Dispose();
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new ProfitLossReportCompanyYearlyPdfLayout(year, company.CompanyName, profitLoss.TotalCredit, profitLoss.TotalDebit, profitLoss.totalProfitLoss,
                    profitLoss.profitLossPercentage, profitLoss.totalCredit, profitLoss.totalDebit);
                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(6);
                account.TotalWidth = 575f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 1f, 1.5f, 1.5f, 4.5f, 4.5f, 2.0f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Index.\r\n"),
                        new DataColumn("Transaction No.\r\n"),
                        new DataColumn("Transaction Date\r\n"),
                        new DataColumn("Company\r\n"),
                        new DataColumn("Description\r\n"),
                        new DataColumn("Transaction Amount (AED)\r\n"),
               });

                if (profitLoss.accountList == null || profitLoss.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in profitLoss.accountList)
                    {
                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, data.GrandTotal);
                        index++;
                    }
                }
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Transaction Amount (AED)"))
                            flag = true;
                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), dt.Rows[rows][column].ToString().StartsWith("-") ? debitFont : creditFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                string fileName = "Profit Loss -" + year + " of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyCustomReportGraph(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            GetAccountHistoryForProfitLossCompanyCustomReportGraphModel model = new GetAccountHistoryForProfitLossCompanyCustomReportGraphModel();
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");

            ProfitLossCompanyCustomReportModel profitLoss = new ProfitLossCompanyCustomReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReportTotalDetails(fromDate, toDate, companyId);

            model.companyId = companyId;
            model.CompanyName = company.CompanyName;
            model.FromDate = minDate;
            model.ToDate = maxDate;
            model.FromDates = fromDate;
            model.ToDates = toDate;

            model.TotalCredit = profitLoss.TotalCredit;
            model.TotalDebit = profitLoss.TotalDebit;
            model.totalProfitLoss = profitLoss.totalProfitLoss;
            model.profitLossPercentage = profitLoss.profitLossPercentage;

            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyCustomReportBarGraphData(string fromDate, string toDate, int companyId)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("yyyy-MM-dd");
            maxDate = ToDate.ToString("yyyy-MM-dd");
            var data = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReportGraph(fromDate, toDate, companyId);
            StringBuilder stringBuilder = new StringBuilder();

            if (data != null && data.Count != 0)
            {
                stringBuilder.Append("[[");
                for (int i = 0; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#00FF00"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("],[");

                for (int i = 1; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#FF0000"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]]");
            }
            else
            {
                stringBuilder.Append("[[{},],[{},]]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyCustomReportPieGraphData(string fromDate, string toDate, int companyId)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("yyyy-MM-dd");
            maxDate = ToDate.ToString("yyyy-MM-dd");
            ProfitLossCompanyCustomReportModel profitLoss = new ProfitLossCompanyCustomReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyCustomReportTotalDetails(fromDate, toDate, companyId);

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyWeeklyReportGraph(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            GetAccountHistoryForProfitLossCompanyWeeklyReportGraphModel model = new GetAccountHistoryForProfitLossCompanyWeeklyReportGraphModel();
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");

            ProfitLossCompanyWeeklyReportModel profitLoss = new ProfitLossCompanyWeeklyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReportTotalDetails(fromDate, toDate, companyId);

            model.companyId = companyId;
            model.CompanyName = company.CompanyName;
            model.FromDate = minDate;
            model.ToDate = maxDate;
            model.FromDates = fromDate;
            model.ToDates = toDate;

            model.TotalCredit = profitLoss.TotalCredit;
            model.TotalDebit = profitLoss.TotalDebit;
            model.totalProfitLoss = profitLoss.totalProfitLoss;
            model.profitLossPercentage = profitLoss.profitLossPercentage;

            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossWeeklyReportCompanyBarGraphData(string fromDate, string toDate, int companyId)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("yyyy-MM-dd");
            maxDate = ToDate.ToString("yyyy-MM-dd");
            var data = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReportGraph(fromDate, toDate, companyId);
            StringBuilder stringBuilder = new StringBuilder();

            if (data != null && data.Count != 0)
            {
                stringBuilder.Append("[[");
                for (int i = 0; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#00FF00"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("],[");

                for (int i = 1; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#FF0000"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]]");
            }
            else
            {
                stringBuilder.Append("[[{},],[{},]]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossWeeklyReportCompanyPieGraphData(string fromDate, string toDate, int companyId)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("yyyy-MM-dd");
            maxDate = ToDate.ToString("yyyy-MM-dd");
            ProfitLossCompanyWeeklyReportModel profitLoss = new ProfitLossCompanyWeeklyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyWeeklyReportTotalDetails(fromDate, toDate, companyId);

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyYealyReportGraph(string year, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            GetAccountHistoryForProfitLossCompanyYearlyReportGraphModel model = new GetAccountHistoryForProfitLossCompanyYearlyReportGraphModel();
            model.year = year;
            model.companyId = companyId;
            model.CompanyName = company.CompanyName;

            ProfitLossCompanyYearlyReportModel profitLoss = new ProfitLossCompanyYearlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYearlyReportTotalDetails(year, companyId);

            model.TotalCredit = profitLoss.TotalCredit;
            model.TotalDebit = profitLoss.TotalDebit;
            model.totalProfitLoss = profitLoss.totalProfitLoss;
            model.profitLossPercentage = profitLoss.profitLossPercentage;

            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossYearlyReportCompanyBarGraphData(string year, int companyId)
        {
            var data = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYealyReportGraph(year, companyId);
            StringBuilder stringBuilder = new StringBuilder();

            if (data != null && data.Count != 0)
            {
                stringBuilder.Append("[[");
                for (int i = 0; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#00FF00"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("],[");

                for (int i = 1; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#FF0000"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]]");
            }
            else
            {
                stringBuilder.Append("[[{},],[{},]]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyYearlyReportPieGraphData(string year, int companyId)
        {
            ProfitLossCompanyYearlyReportModel profitLoss = new ProfitLossCompanyYearlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyYearlyReportTotalDetails(year, companyId);

            StringBuilder stringBuilder = new StringBuilder();

            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyMonthlyReportGraph(string month, string year, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            string monthName = GetMonth(month);
            GetAccountHistoryForProfitLossCompanyMonthlyReportGraphModel model = new GetAccountHistoryForProfitLossCompanyMonthlyReportGraphModel();
            model.year = year;
            model.month = month;
            model.monthName = monthName;
            model.CompanyName = company.CompanyName;
            model.companyId = companyId;

            ProfitLossCompanyMonthlyReportModel profitLoss = new ProfitLossCompanyMonthlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReportTotalDetails(month, year, companyId);

            model.TotalCredit = profitLoss.TotalCredit;
            model.TotalDebit = profitLoss.TotalDebit;
            model.totalProfitLoss = profitLoss.totalProfitLoss;
            model.profitLossPercentage = profitLoss.profitLossPercentage;

            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyMonthlyReportBarGraphData(string month, string year, int companyId)
        {
            var data = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReportGraph(year, month, companyId);
            StringBuilder stringBuilder = new StringBuilder();
            if (data != null && data.Count != 0)
            {
                stringBuilder.Append("[[");
                for (int i = 0; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#00FF00"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("],[");

                for (int i = 1; i < data.Count; i++)
                {
                    stringBuilder.Append("{");
                    System.Threading.Thread.Sleep(50);
                    stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", data[i].Date, Convert.ToDouble(data[i].GrandTotal), "#FF0000"));
                    stringBuilder.Append("},");
                    i++;
                }
                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]]");
            }
            else
            {
                stringBuilder.Append("[[{},],[{},]]");
            }
            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ProfitLossCompanyMonthlyReportPieGraphData(string month, string year, int companyId)
        {
            ProfitLossCompanyMonthlyReportModel profitLoss = new ProfitLossCompanyMonthlyReportModel();
            profitLoss = await _reportManagementRepository.GetAccountHistoryForProfitLossCompanyMonthlyReportTotalDetails(month, year, companyId);

            StringBuilder stringBuilder = new StringBuilder();
            if (profitLoss != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], profitLoss.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], profitLoss.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }
            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult MainBalanceSheetReport()
        {
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCustomReport(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetCustomReportModel balanceSheet = new BalanceSheetCustomReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomReportTotalDetails(fromDate, toDate);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;
            balanceSheet.FromDates = fromDate;
            balanceSheet.ToDates = toDate;
            return View(balanceSheet);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCustomPaginationReport(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");
                BalanceSheetCustomReportPaginationModel model = new BalanceSheetCustomReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomReport(fromDate, toDate, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> BalanceSheetCustomReportExcel(string fromDate, string toDate)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetCustomReportExcelPdfModel balanceSheet = new BalanceSheetCustomReportExcelPdfModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomExcelPdfReport(fromDate, toDate);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(08-07-2023)
                var ws = wb.Worksheets.Add("Balance Sheet Custom Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(08-07-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(08-07-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(08-07-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Custom Report of " + minDate + " - " + maxDate;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();


                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["BalanceSheetCustomReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCustomReportExcel(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCustomReportExcel"].ToString());
                string fileName = "Balance Sheet Custom Report " + minDate + "-" + maxDate + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> BalanceSheetCustomReportPdf(string fromDate, string toDate)
        {
            try
            {
                int index = 1;
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                BalanceSheetCustomReportExcelPdfModel balanceSheet = new BalanceSheetCustomReportExcelPdfModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomExcelPdfReport(fromDate, toDate);
                balanceSheet.FromDate = minDate;
                balanceSheet.ToDate = maxDate;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetReportCustomPdfLayout(minDate, maxDate, balanceSheet.totalCredit, balanceSheet.totalDebit, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                            new DataColumn("Transaction No.\r\n"),
                            new DataColumn("Transaction Date\r\n"),
                            new DataColumn("Company\r\n"),
                            new DataColumn("Description\r\n"),
                            new DataColumn("Credit (AED)\r\n"),
                            new DataColumn("Debit (AED)\r\n"),
                            new DataColumn("Balance (AED)\r\n"),
                   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                TempData["BalanceSheetCustomReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCustomReportPdf(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCustomReportPdf"].ToString());
                string fileName = "Balance Sheet Custom Report " + minDate + "-" + maxDate + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetWeeklyReport(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetWeeklyReportModel balanceSheet = new BalanceSheetWeeklyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyReportTotalDetails(fromDate, toDate);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;
            balanceSheet.FromDates = fromDate;
            balanceSheet.ToDates = toDate;
            return View(balanceSheet);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetWeeklyPaginationReport(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");
                BalanceSheetWeeklyReportPaginationModel model = new BalanceSheetWeeklyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyReport(fromDate, toDate, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> BalanceSheetWeeklyReportExcel(string fromDate, string toDate)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetWeeklyReportExcelPdfModel balanceSheet = new BalanceSheetWeeklyReportExcelPdfModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyExcelPdfReport(fromDate, toDate);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(08-07-2023)
                var ws = wb.Worksheets.Add("Balance Sheet Weekly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(08-07-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(08-07-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(08-07-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Weekly Report of " + minDate + " - " + maxDate;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();


                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["BalanceSheetWeeklyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetWeeklyReportExcel(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetWeeklyReportExcel"].ToString());
                string fileName = "Balance Sheet Weekly Report " + minDate + "-" + maxDate + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> BalanceSheetWeeklyReportPdf(string fromDate, string toDate)
        {
            try
            {
                int index = 1;
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                BalanceSheetWeeklyReportExcelPdfModel balanceSheet = new BalanceSheetWeeklyReportExcelPdfModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyExcelPdfReport(fromDate, toDate);
                balanceSheet.FromDate = minDate;
                balanceSheet.ToDate = maxDate;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetReportWeeklyPdfLayout(minDate, maxDate, balanceSheet.totalCredit, balanceSheet.totalDebit, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                //new iTextSharp.text.BaseColor(9, 121, 105)
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["BalanceSheetWeeklyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetWeeklyReportPdf(string fromDate, string toDate)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetWeeklyReportPdf"].ToString());
                string fileName = "Balance Sheet Weekly Report " + minDate + "-" + maxDate + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMonthlyReport(string month, string year)
        {
            string monthName = GetMonth(month);
            BalanceSheetMonthlyReportModel balanceSheet = new BalanceSheetMonthlyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyReportTotalDetails(month, year);
            balanceSheet.month = month;
            balanceSheet.monthName = monthName;
            balanceSheet.year = year;
            return View(balanceSheet);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMonthlyPaginationReport(string month, string year)
        {
            try
            {
                BalanceSheetMonthlyReportPaginationModel model = new BalanceSheetMonthlyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyReport(month, year, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> BalanceSheetMonthlyReportExcel(string month, string year)
        {
            int index = 1;
            string monthName = GetMonth(month);
            BalanceSheetMonthlyReportExcelPdfModel balanceSheet = new BalanceSheetMonthlyReportExcelPdfModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyExcelPdfReport(month, year);
            balanceSheet.monthName = GetMonth(month);
            balanceSheet.year = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(08-07-2023)
                var ws = wb.Worksheets.Add("Balance Sheet Monthly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(08-07-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(08-07-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(08-07-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Monthly Report of " + monthName + " , " + balanceSheet.year;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();


                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["BalanceSheetMonthlyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetMonthlyReportExcel(string month, string year)
        {
            try
            {
                string monthName = "";
                monthName = GetMonth(month);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetMonthlyReportExcel"].ToString());
                string fileName = "Balance Sheet Monthly Report (" + monthName + "-" + year + ").xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> BalanceSheetMonthlyReportPdf(string month, string year)
        {
            try
            {
                int index = 1;

                BalanceSheetMonthlyReportExcelPdfModel balanceSheet = new BalanceSheetMonthlyReportExcelPdfModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyExcelPdfReport(month, year);
                balanceSheet.monthName = GetMonth(month);
                balanceSheet.year = year;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetReportMonthlyPdfLayout(balanceSheet.monthName, year, balanceSheet.totalCredit, balanceSheet.totalDebit, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["BalanceSheetMonthlyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetMonthlyReportPdf(string month, string year)
        {
            try
            {
                string monthName = "";
                monthName = GetMonth(month);

                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetMonthlyReportPdf"].ToString());
                string fileName = "Balance Sheet Monthly Report (" + monthName + "-" + year + ").pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetYearlyReport(string year)
        {
            BalanceSheetYearlyReportModel balanceSheet = new BalanceSheetYearlyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyReportTotalDetails(year);
            balanceSheet.year = year;
            return View(balanceSheet);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetYearlyPaginationReport(string year)
        {
            try
            {
                BalanceSheetYearlyReportPaginationModel model = new BalanceSheetYearlyReportPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyReport(year, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> BalanceSheetYearlyReportExcel(string year)
        {
            int index = 1;
            BalanceSheetYearlyReportExcelPdfModel balanceSheet = new BalanceSheetYearlyReportExcelPdfModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyExcelPdfReport(year);
            balanceSheet.year = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(08-07-2023)
                var ws = wb.Worksheets.Add("Balance Sheet Yearly Report");

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(08-07-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(08-07-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(08-07-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = " DIBN";

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["BalanceSheetYearlyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetYearlyReportExcel(string year)
        {
            try
            {
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetYearlyReportExcel"].ToString());
                string fileName = "Balance Sheet Yearly Report " + year + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> BalanceSheetYearlyReportPdf(string year)
        {
            try
            {
                int index = 1;

                BalanceSheetYearlyReportExcelPdfModel balanceSheet = new BalanceSheetYearlyReportExcelPdfModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyExcelPdfReport(year);
                balanceSheet.year = year;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetReportYearlyPdfLayout(year, balanceSheet.totalCredit, balanceSheet.totalDebit, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 80f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["BalanceSheetYearlyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());
                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetYearlyReportPdf(string year)
        {
            try
            {
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetYearlyReportPdf"].ToString());
                string fileName = "Balance Sheet Yearly Report (" + year + ").pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetYearlyGraphReport(string year)
        {
            BalanceSheetYearlyReportModel balanceSheet = new BalanceSheetYearlyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyReportTotalDetails(year);
            balanceSheet.year = year;
            return View(balanceSheet);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetYearlyReportPieGraphData(string year)
        {
            BalanceSheetYearlyReportModel balanceSheet = new BalanceSheetYearlyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyReportTotalDetails(year);

            StringBuilder stringBuilder = new StringBuilder();

            if (balanceSheet != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], balanceSheet.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], balanceSheet.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMonthlyGraphReport(string month, string year)
        {
            string monthName = GetMonth(month);
            BalanceSheetMonthlyReportModel balanceSheet = new BalanceSheetMonthlyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyReportTotalDetails(month, year);
            balanceSheet.month = month;
            balanceSheet.monthName = monthName;
            balanceSheet.year = year;
            return View(balanceSheet);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMonthlyReportPieGraphData(string month, string year)
        {
            BalanceSheetMonthlyReportModel balanceSheet = new BalanceSheetMonthlyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyReportTotalDetails(month, year);

            StringBuilder stringBuilder = new StringBuilder();

            if (balanceSheet != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], balanceSheet.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], balanceSheet.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetWeeklyGraphReport(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetWeeklyReportModel balanceSheet = new BalanceSheetWeeklyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyReportTotalDetails(fromDate, toDate);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;
            balanceSheet.FromDates = fromDate;
            balanceSheet.ToDates = toDate;
            return View(balanceSheet);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetWeeklyReportPieGraphData(string fromDate, string toDate)
        {
            BalanceSheetWeeklyReportModel balanceSheet = new BalanceSheetWeeklyReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyReportTotalDetails(fromDate, toDate);

            StringBuilder stringBuilder = new StringBuilder();

            if (balanceSheet != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], balanceSheet.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], balanceSheet.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCustomGraphReport(string fromDate, string toDate)
        {
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetCustomReportModel balanceSheet = new BalanceSheetCustomReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomReportTotalDetails(fromDate, toDate);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;
            balanceSheet.FromDates = fromDate;
            balanceSheet.ToDates = toDate;
            return View(balanceSheet);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCustomReportPieGraphData(string fromDate, string toDate)
        {
            BalanceSheetCustomReportModel balanceSheet = new BalanceSheetCustomReportModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomReportTotalDetails(fromDate, toDate);

            StringBuilder stringBuilder = new StringBuilder();

            if (balanceSheet != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], balanceSheet.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], balanceSheet.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CompanyWiseBalanceSheetReport()
        {
            return View();
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CompanyWiseBalanceSheetPaginationReport()
        {
            try
            {
                GetCompanyListForBalanceSheetPaginationModel model = new GetCompanyListForBalanceSheetPaginationModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetCompanyListForBalanceSheet(skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalCompany;
                totalRecord = model.totalCompany;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.getCompanies
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
        public IActionResult CompanyBalanceSheetReport(int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            GetCompanyListForBalanceSheetModel model = new GetCompanyListForBalanceSheetModel();
            model.Id = companyId;
            model.CompanyName = company.CompanyName;
            model.CompanyAccountNumber = company.AccountNumber;
            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyCustomReport(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetCustomReportByCompanyIdModel balanceSheet = new BalanceSheetCustomReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetCustomReportTotalDetailsByCompanyId(fromDate, toDate, companyId);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;
            balanceSheet.FromDates = fromDate;
            balanceSheet.ToDates = toDate;
            balanceSheet.CompanyId = companyId;
            balanceSheet.CompanyName = company.CompanyName;
            return View(balanceSheet);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyCustomPaginationReport(string fromDate, string toDate, int companyId)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");
                BalanceSheetCustomReportPaginationByCompanyIdModel model = new BalanceSheetCustomReportPaginationByCompanyIdModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomReportByCompanyId(fromDate, toDate, companyId, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> BalanceSheetCompanyCustomReportExcel(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetCustomReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetCustomReportExcelPdfByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomExcelPdfReportByCompanyId(fromDate, toDate, companyId);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Balance Sheet Custom Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Custom Report from " + minDate + " - " + maxDate + " of " + company.CompanyName;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["BalanceSheetCompanyCustomReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCompanyCustomReportExcel(string fromDate, string toDate, int companyId)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var company = _companyRepository.GetCompanyById(companyId);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCompanyCustomReportExcel"].ToString());
                string fileName = "Balance Sheet Custom Report " + minDate + "-" + maxDate + " of " + company.CompanyName + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> BalanceSheetCompanyCustomReportPdf(string fromDate, string toDate, int companyId)
        {
            try
            {
                int index = 1;
                var company = _companyRepository.GetCompanyById(companyId);
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                BalanceSheetCustomReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetCustomReportExcelPdfByCompanyIdModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomExcelPdfReportByCompanyId(fromDate, toDate, companyId);
                balanceSheet.FromDate = minDate;
                balanceSheet.ToDate = maxDate;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetCompanyCustomPdfLayout(minDate, maxDate, balanceSheet.totalCredit, balanceSheet.totalDebit, company.CompanyName, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }

                document.Add(account);

                document.Close();
                stream.Flush();
                stream.Position = 0;

                TempData["BalanceSheetCompanyCustomReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());

                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCompanyCustomReportPdf(string fromDate, string toDate, int companyId)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var company = _companyRepository.GetCompanyById(companyId);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCompanyCustomReportPdf"].ToString());
                string fileName = "Balance Sheet Custom Report " + minDate + "-" + maxDate + " of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMultipleCompanyCustomReportExcel(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetCustomReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetCustomReportExcelPdfByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomExcelPdfReportByCompanyId(fromDate, toDate, companyId);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Balance Sheet Custom Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Custom Report from " + minDate + " - " + maxDate + " of " + company.CompanyName;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    string fileName = "Balance Sheet Custom Report " + minDate + "-" + maxDate + " of " + company.CompanyName + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMultipleCompanyCustomReportPdf(string fromDate, string toDate, int companyId)
        {
            try
            {
                int index = 1;
                var company = _companyRepository.GetCompanyById(companyId);
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                BalanceSheetCustomReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetCustomReportExcelPdfByCompanyIdModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetCustomExcelPdfReportByCompanyId(fromDate, toDate, companyId);
                balanceSheet.FromDate = minDate;
                balanceSheet.ToDate = maxDate;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetCompanyCustomPdfLayout(minDate, maxDate, balanceSheet.totalCredit, balanceSheet.totalDebit, company.CompanyName, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }

                document.Add(account);

                document.Close();
                stream.Flush();
                stream.Position = 0;

                string fileName = "Balance Sheet Custom Report " + minDate + "-" + maxDate + " of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyWeeklyReport(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetWeeklyReportByCompanyIdModel balanceSheet = new BalanceSheetWeeklyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetWeeklyReportTotalDetailsByCompanyId(fromDate, toDate, companyId);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;
            balanceSheet.FromDates = fromDate;
            balanceSheet.ToDates = toDate;
            balanceSheet.CompanyId = companyId;
            balanceSheet.CompanyName = company.CompanyName;
            return View(balanceSheet);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyWeeklyPaginationReport(string fromDate, string toDate, int companyId)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");
                BalanceSheetWeeklyReportPaginationByCompanyIdModel model = new BalanceSheetWeeklyReportPaginationByCompanyIdModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyReportByCompanyId(fromDate, toDate, companyId, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> BalanceSheetCompanyWeeklyReportExcel(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetWeeklyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetWeeklyReportExcelPdfByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportByCompanyId(fromDate, toDate, companyId);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Balance Sheet Weekly Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Weekly Report from " + minDate + " - " + maxDate + " of " + company.CompanyName;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    TempData["BalanceSheetCompanyWeeklyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());
                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCompanyWeeklyReportExcel(string fromDate, string toDate, int companyId)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var company = _companyRepository.GetCompanyById(companyId);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCompanyWeeklyReportExcel"].ToString());
                string fileName = "Balance Sheet Weekly Report " + minDate + "-" + maxDate + " of " + company.CompanyName + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> BalanceSheetCompanyWeeklyReportPdf(string fromDate, string toDate, int companyId)
        {
            try
            {
                int index = 1;
                var company = _companyRepository.GetCompanyById(companyId);
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                BalanceSheetWeeklyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetWeeklyReportExcelPdfByCompanyIdModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportByCompanyId(fromDate, toDate, companyId);
                balanceSheet.FromDate = minDate;
                balanceSheet.ToDate = maxDate;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetCompanyWeeklyPdfLayout(minDate, maxDate, balanceSheet.totalCredit, balanceSheet.totalDebit, company.CompanyName, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                //new iTextSharp.text.BaseColor(9, 121, 105)
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;

                TempData["BalanceSheetCompanyWeeklyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());

                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCompanyWeeklyReportPdf(string fromDate, string toDate, int companyId)
        {
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                var company = _companyRepository.GetCompanyById(companyId);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCompanyWeeklyReportPdf"].ToString());
                string fileName = "Balance Sheet Weekly Report " + minDate + "-" + maxDate + " of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMultipleCompanyWeeklyReportExcel(string fromDate, string toDate, int companyId)
        {
            int index = 1;
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetWeeklyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetWeeklyReportExcelPdfByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportByCompanyId(fromDate, toDate, companyId);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Balance Sheet Weekly Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Weekly Report from " + minDate + " - " + maxDate + " of " + company.CompanyName;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    string fileName = "Balance Sheet Weekly Report " + minDate + "-" + maxDate + " of " + company.CompanyName + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMultipleCompanyWeeklyReportPdf(string fromDate, string toDate, int companyId)
        {
            try
            {
                int index = 1;
                var company = _companyRepository.GetCompanyById(companyId);
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("dd-MM-yyyy");
                maxDate = ToDate.ToString("dd-MM-yyyy");

                BalanceSheetWeeklyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetWeeklyReportExcelPdfByCompanyIdModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportByCompanyId(fromDate, toDate, companyId);
                balanceSheet.FromDate = minDate;
                balanceSheet.ToDate = maxDate;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetCompanyWeeklyPdfLayout(minDate, maxDate, balanceSheet.totalCredit, balanceSheet.totalDebit, company.CompanyName, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                string fileName = "Balance Sheet Weekly Report " + minDate + "-" + maxDate + " of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyMonthlyReport(string month, string year, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetMonthlyReportByCompanyIdModel balanceSheet = new BalanceSheetMonthlyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetMonthlyReportTotalDetailsByCompanyId(month, year, companyId);
            balanceSheet.month = month;
            balanceSheet.year = year;
            balanceSheet.monthName = GetMonth(month);
            balanceSheet.CompanyId = companyId;
            balanceSheet.CompanyName = company.CompanyName;
            return View(balanceSheet);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyMonthlyPaginationReport(string month, string year, int companyId)
        {
            try
            {
                BalanceSheetMonthlyReportPaginationByCompanyIdModel model = new BalanceSheetMonthlyReportPaginationByCompanyIdModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyReportByCompanyId(month, year, companyId, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> BalanceSheetCompanyMonthlyReportExcel(string month, string year, int companyId)
        {
            int index = 1;
            string monthName = GetMonth(month);
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetMonthlyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetMonthlyReportExcelPdfByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportByCompanyId(month, year, companyId);
            balanceSheet.monthName = monthName;
            balanceSheet.month = month;
            balanceSheet.year = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Balance Sheet Month Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Monthly Report ( " + monthName + " , " + year + " ) of " + company.CompanyName;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    TempData["BalanceSheetCompanyMonthlyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());

                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCompanyMonthlyReportExcel(string month, string year, int companyId)
        {
            try
            {
                string monthName = "";
                monthName = GetMonth(month);

                var company = _companyRepository.GetCompanyById(companyId);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCompanyMonthlyReportExcel"].ToString());
                string fileName = "Balance Sheet Monthly Report (" + monthName + "-" + year + ") of " + company.CompanyName + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> BalanceSheetCompanyMonthlyReportPdf(string month, string year, int companyId)
        {
            try
            {
                int index = 1;
                var company = _companyRepository.GetCompanyById(companyId);
                string monthName = GetMonth(month);
                BalanceSheetMonthlyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetMonthlyReportExcelPdfByCompanyIdModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportByCompanyId(month, year, companyId);
                balanceSheet.month = month;
                balanceSheet.monthName = monthName;
                balanceSheet.year = year;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetCompanyMonthlyPdfLayout(monthName, year, balanceSheet.totalCredit, balanceSheet.totalDebit, company.CompanyName, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["BalanceSheetCompanyMonthlyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());

                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCompanyMonthlyReportPdf(string month, string year, int companyId)
        {
            try
            {
                string monthName = "";
                monthName = GetMonth(month);

                var company = _companyRepository.GetCompanyById(companyId);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCompanyMonthlyReportPdf"].ToString());
                string fileName = "Balance Sheet Monthly Report (" + monthName + "-" + year + ") of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMultipleCompanyMonthlyReportExcel(string month, string year, int companyId)
        {
            int index = 1;
            string monthName = GetMonth(month);
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetMonthlyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetMonthlyReportExcelPdfByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportByCompanyId(month, year, companyId);
            balanceSheet.monthName = monthName;
            balanceSheet.month = month;
            balanceSheet.year = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Balance Sheet Month Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Monthly Report ( " + monthName + " , " + year + " ) of " + company.CompanyName;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    string fileName = "Balance Sheet Monthly Report (" + monthName + "-" + year + ") of " + company.CompanyName + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMultipleCompanyMonthlyReportPdf(string month, string year, int companyId)
        {
            try
            {
                int index = 1;
                var company = _companyRepository.GetCompanyById(companyId);
                string monthName = GetMonth(month);
                BalanceSheetMonthlyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetMonthlyReportExcelPdfByCompanyIdModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportByCompanyId(month, year, companyId);
                balanceSheet.month = month;
                balanceSheet.monthName = monthName;
                balanceSheet.year = year;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetCompanyMonthlyPdfLayout(monthName, year, balanceSheet.totalCredit, balanceSheet.totalDebit, company.CompanyName, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }

                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                string fileName = "Balance Sheet Monthly Report (" + monthName + "-" + year + ") of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyYearlyReport(string year, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetYearlyReportByCompanyIdModel balanceSheet = new BalanceSheetYearlyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetYearlyReportTotalDetailsByCompanyId(year, companyId);
            balanceSheet.year = year;
            balanceSheet.CompanyId = companyId;
            balanceSheet.CompanyName = company.CompanyName;
            return View(balanceSheet);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyYearlyPaginationReport(string year, int companyId)
        {
            try
            {
                BalanceSheetYearlyReportPaginationByCompanyIdModel model = new BalanceSheetYearlyReportPaginationByCompanyIdModel();
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + sortColumnIndex + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                model = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyReportByCompanyId(year, companyId, skip, pageSize, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalAccountEntry;
                totalRecord = model.totalAccountEntry;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.accountList
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
        public async Task<JsonResult> BalanceSheetCompanyYearlyReportExcel(string year, int companyId)
        {
            int index = 1;
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetYearlyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetYearlyReportExcelPdfByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyExcelPdfReportByCompanyId(year, companyId);
            balanceSheet.year = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Balance Sheet Yearly Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = " DIBN ";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Yearly Report ( " + year + " ) of " + company.CompanyName;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    TempData["BalanceSheetCompanyYearlyReportExcel"] = JsonConvert.SerializeObject(stream.ToArray());

                    return new JsonResult("Success");
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCompanyYearlyReportExcel(string year, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCompanyYearlyReportExcel"].ToString());
                string fileName = "Balance Sheet Yearly Report (" + year + ") of " + company.CompanyName + ".xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> BalanceSheetCompanyYearlyReportPdf(string year, int companyId)
        {
            try
            {
                int index = 1;
                var company = _companyRepository.GetCompanyById(companyId);

                BalanceSheetYearlyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetYearlyReportExcelPdfByCompanyIdModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyExcelPdfReportByCompanyId(year, companyId);
                balanceSheet.year = year;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetCompanyYearlyPdfLayout(year, balanceSheet.totalCredit, balanceSheet.totalDebit, company.CompanyName, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
            new DataColumn("Transaction No.\r\n"),
            new DataColumn("Transaction Date\r\n"),
            new DataColumn("Company\r\n"),
            new DataColumn("Description\r\n"),
            new DataColumn("Credit (AED)\r\n"),
            new DataColumn("Debit (AED)\r\n"),
            new DataColumn("Balance (AED)\r\n"),
   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                TempData["BalanceSheetCompanyYearlyReportPdf"] = JsonConvert.SerializeObject(stream.ToArray());

                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadBalanceSheetCompanyYearlyReportPdf(string year, int companyId)
        {
            try
            {
                var company = _companyRepository.GetCompanyById(companyId);
                var stream = JsonConvert.DeserializeObject<byte[]>(TempData["BalanceSheetCompanyYearlyReportPdf"].ToString());
                string fileName = "Balance Sheet Yearly Report (" + year + ") of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMultipleCompanyYearlyReportExcel(string year, int companyId)
        {
            int index = 1;
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetYearlyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetYearlyReportExcelPdfByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyExcelPdfReportByCompanyId(year, companyId);
            balanceSheet.year = year;

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                                        new DataColumn("Transaction No.\r\n"),
                                        new DataColumn("Transaction Date\r\n"),
                                        new DataColumn("Company\r\n"),
                                        new DataColumn("Description\r\n"),
                                        new DataColumn("Credit (AED)\r\n"),
                                        new DataColumn("Debit (AED)\r\n"),
                                        new DataColumn("Balance (AED)\r\n"),
                               });

            if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in balanceSheet.accountList)
                {
                    string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                    string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                    string balance = data.BalanceTotal.ToString("0.00");

                    dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                    index++;
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Balance Sheet Yearly Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);

                // Insert Datatable contains all data and set it to B7                                                                                                                      -- Yashasvi(09-06-2023)                
                var ws1 = wb.Worksheet(1).Cell(7, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                // Get count of total rows added to excel file ,as it will also take first 6 lines where we show details of Excel file i have increased it with extra 6 lines.              -- Yashasvi(09-06-2023)
                var count = ws1.RowCount() + 6;

                // It will adjust column width based on contain inside each column.                                                                                                         -- Yashasvi(09-06-2023)
                ws1.Cell("A7").WorksheetColumn().AdjustToContents();
                ws1.Cell("B7").WorksheetColumn().AdjustToContents();
                ws1.Cell("C7").WorksheetColumn().AdjustToContents();
                ws1.Cell("D7").WorksheetColumn().Width = 50;
                ws1.Cell("E7").WorksheetColumn().Width = 50;
                ws1.Cell("F7").WorksheetColumn().AdjustToContents();
                ws1.Cell("G7").WorksheetColumn().AdjustToContents();
                ws1.Cell("H7").WorksheetColumn().AdjustToContents();
                ws1.Cell("I7").WorksheetColumn().AdjustToContents();

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitles = ws.Range("F8:F" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitles.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle1 = ws.Range("G8:G" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle1.Cells().Where(x => !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                //rngMainTitles.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle1.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitle3 = ws.Range("H8:H" + count);

                // If value does not contain (minus) sign it will change text color to Green.                                                                                               -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Green);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.Style.Font.FontColor = XLColor.Red);

                //// Convert Text to Number and Adjust text Align and make it bold                                                                                                            -- Yashasvi(08-07-2023)
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                rngMainTitle3.Cells().Style
                    .Font.SetBold()
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);



                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitless = ws.Range("E8:F" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitless.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitless.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // To show debit values in red color and credit value in green color. Also change type of each data from Number to Text.                                                    -- Yashasvi(08-07-2023)
                var rngMainTitlee = ws.Range("D8:E" + count);

                // If value contain (minus) sign it will change text color to Red.                                                                                                          -- Yashasvi(08-07-2023)
                rngMainTitlee.Cells().Where(x => !x.Value.ToString().IsNullOrWhiteSpace()).ToList().ForEach(x => x.Style.Alignment.WrapText = true);

                rngMainTitlee.Cells().Style
                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //  Adding text to Cell B2 and Merge Cells                                                                                                               -- Yashasvi(08-07-2023)
                ws.Cell("D1").Value = "DIBN";

                var rngMainTitle = ws.Range("D1:E2");
                rngMainTitle.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle.FirstCell().Style
                    .Font.FontSize = 22;
                //Merge title cells
                rngMainTitle.FirstRow().Merge();


                //  Adding text to Cell B3 and Merge Cells                                                                                                              -- Yashasvi(08-07-2023)
                ws.Cell("D2").Value = "Balance Sheet Yearly Report ( " + year + " ) of " + company.CompanyName;

                var rngMainTitle2 = ws.Range("D2:E3");
                rngMainTitle2.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle2.FirstRow().Merge();

                ws.Cell("D3").Value = "Total Balance : " + balanceSheet.totalBalance + " AED ";
                if (Convert.ToDouble(balanceSheet.totalBalance) > 0)
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Green;
                else
                    ws.Cell("D3").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle4 = ws.Range("D3:E4");
                rngMainTitle4.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                rngMainTitle4.FirstRow().Merge();

                //  Adding text to Cell B4 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("D4").Value = "Total Credited Amount : ";
                ws.Cell("D4").Value += balanceSheet.totalCredit + " AED ";
                ws.Cell("D4").Style.Font.FontColor = XLColor.Green;

                var rngMainTitle5 = ws.Range("D4:D5");
                rngMainTitle5.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle5.FirstRow().Merge();

                //  Adding text to Cell B5 and Merge Cells                                                                                                             -- Yashasvi(09-06-2023)
                ws.Cell("E4").Value += "Total Debited Amount : ";
                ws.Cell("E4").Value += balanceSheet.totalDebit + " AED ";
                ws.Cell("E4").Style.Font.FontColor = XLColor.Red;

                var rngMainTitle6 = ws.Range("E4:E5");
                rngMainTitle6.FirstCell().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Merge title cells
                rngMainTitle6.FirstRow().Merge();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    string fileName = "Balance Sheet Yearly Report (" + year + ") of " + company.CompanyName + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetMultipleCompanyYearlyReportPdf(string year, int companyId)
        {
            try
            {
                int index = 1;
                var company = _companyRepository.GetCompanyById(companyId);

                BalanceSheetYearlyReportExcelPdfByCompanyIdModel balanceSheet = new BalanceSheetYearlyReportExcelPdfByCompanyIdModel();
                balanceSheet = await _reportManagementRepository.GetAccountHistoryForBalanceSheetYearlyExcelPdfReportByCompanyId(year, companyId);
                balanceSheet.year = year;

                Document document = new Document(PageSize.A4);
                document.Dispose();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                
                PdfPCell cell = null;
                pdfWriter.CloseStream = false;
                pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;

                pdfWriter.PageEvent = new BalanceSheetCompanyYearlyPdfLayout(year, balanceSheet.totalCredit, balanceSheet.totalDebit, company.CompanyName, balanceSheet.totalBalance);

                document.SetMargins(5f, 5f, 90f, 15f);
                document.Open();
                iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                iTextSharp.text.Font headerFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font debitFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Font creditFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 128, 0));
                iTextSharp.text.Font pdfTitleFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPTable account = new PdfPTable(8);
                account.TotalWidth = 580f;
                account.LockedWidth = true;
                account.SetWidths(new float[] { 0.5f, 1f, 1f, 1.5f, 3f, 1f, 1f, 1f });
                account.HeaderRows = 1;

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Index.\r\n"),
                            new DataColumn("Transaction No.\r\n"),
                            new DataColumn("Transaction Date\r\n"),
                            new DataColumn("Company\r\n"),
                            new DataColumn("Description\r\n"),
                            new DataColumn("Credit (AED)\r\n"),
                            new DataColumn("Debit (AED)\r\n"),
                            new DataColumn("Balance (AED)\r\n"),
                   });

                if (balanceSheet.accountList == null || balanceSheet.accountList.Count <= 0)
                {
                    dt.Rows.Add("---", "---", "---", "---", "---", "---", "---", "---");
                }
                else
                {
                    foreach (var data in balanceSheet.accountList)
                    {
                        string credit = data.Type == "Credit" ? data.GrandTotal.ToString() : "--";
                        string debit = data.Type == "Credit" ? "--" : data.GrandTotal.ToString();
                        string balance = data.BalanceTotal.ToString("0.00");

                        dt.Rows.Add(index, data.TransactionId, data.Date, data.CompanyName, data.Description, credit, debit, balance);
                        index++;
                    }
                }

                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), titleFont)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(36, 60, 124);
                    cell.Border = Rectangle.BOX;
                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    account.AddCell(cell);
                }

                for (int rows = 0; rows < dt.Rows.Count; rows++)
                {
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        bool flag = false, creditFlag = false, debitFlag = false;
                        if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)") || dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                        {
                            flag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Credit (AED)"))
                                creditFlag = true;
                            if (dt.Columns[column].ToString().StartsWith("﻿﻿Debit (AED)"))
                                debitFlag = true;
                        }

                        if (!flag)
                        {
                            Console.WriteLine(dt.Columns[column].ToString());
                            cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), headerFont)));
                            cell.Border = Rectangle.BOX;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            account.AddCell(cell);
                        }
                        else
                        {
                            if (creditFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), creditFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                            if (debitFlag)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), debitFont)));
                                cell.Border = Rectangle.BOX;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                                account.AddCell(cell);
                            }
                        }
                    }
                }
                document.Add(account);

                document.Close();

                stream.Flush();

                stream.Position = 0;


                string fileName = "Balance Sheet Yearly Report (" + year + ") of " + company.CompanyName + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyYearlyGraphReport(string year, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetYearlyReportByCompanyIdModel balanceSheet = new BalanceSheetYearlyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetYearlyReportTotalDetailsByCompanyId(year, companyId);
            balanceSheet.year = year;
            balanceSheet.CompanyId = companyId;
            balanceSheet.CompanyName = company.CompanyName;
            return View(balanceSheet);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyYearlyReportPieGraphData(string year, int companyId)
        {
            BalanceSheetYearlyReportByCompanyIdModel balanceSheet = new BalanceSheetYearlyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetYearlyReportTotalDetailsByCompanyId(year, companyId);

            StringBuilder stringBuilder = new StringBuilder();

            if (balanceSheet != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], balanceSheet.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], balanceSheet.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyMonthlyGraphReport(string month, string year, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            BalanceSheetMonthlyReportByCompanyIdModel balanceSheet = new BalanceSheetMonthlyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetMonthlyReportTotalDetailsByCompanyId(month, year, companyId);
            balanceSheet.month = month;
            balanceSheet.year = year;
            balanceSheet.monthName = GetMonth(month);
            balanceSheet.CompanyId = companyId;
            balanceSheet.CompanyName = company.CompanyName;
            return View(balanceSheet);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyMonthlyReportPieGraphData(string month, string year, int companyId)
        {
            BalanceSheetMonthlyReportByCompanyIdModel balanceSheet = new BalanceSheetMonthlyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetMonthlyReportTotalDetailsByCompanyId(month, year, companyId);

            StringBuilder stringBuilder = new StringBuilder();

            if (balanceSheet != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], balanceSheet.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], balanceSheet.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyWeeklyGraphReport(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetWeeklyReportByCompanyIdModel balanceSheet = new BalanceSheetWeeklyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetWeeklyReportTotalDetailsByCompanyId(fromDate, toDate, companyId);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;
            balanceSheet.FromDates = fromDate;
            balanceSheet.ToDates = toDate;
            balanceSheet.CompanyId = companyId;
            balanceSheet.CompanyName = company.CompanyName;
            return View(balanceSheet);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyWeeklyReportPieGraphData(string fromDate, string toDate, int companyId)
        {
            BalanceSheetWeeklyReportByCompanyIdModel balanceSheet = new BalanceSheetWeeklyReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetWeeklyReportTotalDetailsByCompanyId(fromDate, toDate, companyId);

            StringBuilder stringBuilder = new StringBuilder();

            if (balanceSheet != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], balanceSheet.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], balanceSheet.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyCustomGraphReport(string fromDate, string toDate, int companyId)
        {
            var company = _companyRepository.GetCompanyById(companyId);
            string minDate = "", maxDate = "";
            DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
            minDate = FromDate.ToString("dd-MM-yyyy");
            maxDate = ToDate.ToString("dd-MM-yyyy");
            BalanceSheetCustomReportByCompanyIdModel balanceSheet = new BalanceSheetCustomReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetCustomReportTotalDetailsByCompanyId(fromDate, toDate, companyId);
            balanceSheet.FromDate = minDate;
            balanceSheet.ToDate = maxDate;
            balanceSheet.FromDates = fromDate;
            balanceSheet.ToDates = toDate;
            balanceSheet.CompanyId = companyId;
            balanceSheet.CompanyName = company.CompanyName;
            return View(balanceSheet);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> BalanceSheetCompanyCustomReportPieGraphData(string fromDate, string toDate, int companyId)
        {
            BalanceSheetCustomReportByCompanyIdModel balanceSheet = new BalanceSheetCustomReportByCompanyIdModel();
            balanceSheet = await _reportManagementRepository.GetBalanceSheetCustomReportTotalDetailsByCompanyId(fromDate, toDate, companyId);

            StringBuilder stringBuilder = new StringBuilder();

            if (balanceSheet != null)
            {
                stringBuilder.Append("[");

                List<string> _text = new List<string>();
                List<string> _color = new List<string>();
                _text.Add("Total Credits");
                _text.Add("Total Debits");
                _color.Add("#7FFF00");
                _color.Add("#D60000");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[0], balanceSheet.TotalCredit, _color[0]));
                stringBuilder.Append("},");

                stringBuilder.Append("{");
                System.Threading.Thread.Sleep(50);
                stringBuilder.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", _text[1], balanceSheet.TotalDebit, _color[1]));
                stringBuilder.Append("},");

                stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append("]");
            }
            else
            {
                stringBuilder.Append("[{},{},]");
            }

            return Content(stringBuilder.ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CompanyProfitLoss()
        {
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ClearedCompanyListForExcel()
        {
            int count = 0;
            decimal totalCredit = 0;
            List<GetClearedCompanyListModelForExcel> clearedCompany = new List<GetClearedCompanyListModelForExcel>();
            clearedCompany = await _reportManagementRepository.GetClearedCompanyListForExcel();

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Account Number.\r\n"),
                                        new DataColumn("Company.\r\n"),
                                        new DataColumn("Company Owner.\r\n"),
                                        new DataColumn("Email\r\n"),
                                        new DataColumn("Total Credit\r\n")
                               });

            if (clearedCompany == null && clearedCompany.Count > 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in clearedCompany)
                {
                    dt.Rows.Add(data.AccountNumber, data.CompanyName, data.CompanyOwner, data.Email, data.CompanyPortalBalance);
                    totalCredit += data.CompanyPortalBalance;
                }
            }

            count = clearedCompany.Count;

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Total Credit Company Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);
                var ws1 = wb.Worksheet(1).Cell(3, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                ws1.Cell("A3").WorksheetColumn().AdjustToContents();
                ws1.Cell("B3").WorksheetColumn().AdjustToContents();
                ws1.Cell("C3").WorksheetColumn().AdjustToContents();
                ws1.Cell("D3").WorksheetColumn().AdjustToContents();
                ws1.Cell("E3").WorksheetColumn().AdjustToContents();

                var rngMainTitles = ws.Range("A1:E3");
                rngMainTitles.Cells().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                count = count + 3;
                var rngMainTitle3 = ws.Range("E4:E" + count);
                rngMainTitle3.Cells().Where(x => !x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                count = count + 1;
                ws.Cell("A" + count).Value = "Total";
                var rngMainTitle = ws.Range("B" + count + ":D" + count);
                rngMainTitle.Merge();

                ws.Cell("B" + count).Value = "Credit Balance as on current date";
                ws.Cell("B" + count).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                var tillData = count - 1;

                ws.Cell("E" + count).Value = totalCredit.ToString("0.00");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    string fileName = "Total Credit Company.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> OverdueCompanyListForExcel()
        {
            int count = 0;
            decimal totalDebit = 0;
            List<GetOverdueCompanyListModelForExcel> clearedCompany = new List<GetOverdueCompanyListModelForExcel>();
            clearedCompany = await _reportManagementRepository.GetOverdueCompanyListForExcel();

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Account Number.\r\n"),
                                        new DataColumn("Company.\r\n"),
                                        new DataColumn("Company Owner.\r\n"),
                                        new DataColumn("Email\r\n"),
                                        new DataColumn("Total Debit\r\n")
                               });

            if (clearedCompany == null && clearedCompany.Count > 0)
            {
                dt.Rows.Add("---", "---", "---", "---", "---");
            }
            else
            {
                foreach (var data in clearedCompany)
                {
                    dt.Rows.Add(data.AccountNumber, data.CompanyName, data.CompanyOwner, data.Email, data.CompanyPortalBalance);
                    totalDebit += data.CompanyPortalBalance;
                }
            }

            count = clearedCompany.Count;

            using (XLWorkbook wb = new XLWorkbook())
            {
                string balanceSheetName = "Total Debit Company Report";
                // Change Sheet Name                                                                                                                                                        -- Yashasvi(09-06-2023)
                var ws = wb.Worksheets.Add(balanceSheetName);
                var ws1 = wb.Worksheet(1).Cell(3, 1).InsertTable(dt);
                ws1.ShowAutoFilter = false;

                ws1.Cell("A3").WorksheetColumn().AdjustToContents();
                ws1.Cell("B3").WorksheetColumn().AdjustToContents();
                ws1.Cell("C3").WorksheetColumn().AdjustToContents();
                ws1.Cell("D3").WorksheetColumn().AdjustToContents();
                ws1.Cell("E3").WorksheetColumn().AdjustToContents();

                var rngMainTitles = ws.Range("A1:E3");
                rngMainTitles.Cells().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                count = count + 3;
                var rngMainTitle3 = ws.Range("E4:E" + count);
                rngMainTitle3.Cells().Where(x => x.Value.ToString().Contains("-") && !x.Value.ToString().Contains("--")).ToList().ForEach(x => x.DataType = XLDataType.Number);

                count = count + 1;
                var rngMainTitle = ws.Range("B" + count + ":D" + count);
                rngMainTitle.Merge();
                ws.Cell("A" + count).Value = "Total";
                ws.Cell("B" + count).Value = "Out standing balance as on current date";
                ws.Cell("B" + count).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                var tillData = count - 1;

                ws.Cell("E" + count).Value = totalDebit.ToString("0.00");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    string fileName = "Total Debit Company.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        public string GetMonth(string month)
        {
            string monthName = "";
            if (month == "01" || month == "1")
                monthName = "January";
            if (month == "02" || month == "2")
                monthName = "February";
            if (month == "03" || month == "3")
                monthName = "March";
            if (month == "04" || month == "4")
                monthName = "April";
            if (month == "05" || month == "5")
                monthName = "May";
            if (month == "06" || month == "6")
                monthName = "June";
            if (month == "07" || month == "7")
                monthName = "July";
            if (month == "08" || month == "8")
                monthName = "August";
            if (month == "09" || month == "9")
                monthName = "September";
            if (month == "10")
                monthName = "October";
            if (month == "11")
                monthName = "November";
            if (month == "12")
                monthName = "December";
            return monthName;
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
