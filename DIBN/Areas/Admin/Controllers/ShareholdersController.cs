using ClosedXML.Excel;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nager.Country;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Authorize]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    #nullable enable
    public class ShareholdersController : Controller
    {
        private readonly IShareholderRepository _shareholderRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IPermissionRepository _permissionRepository;
        public ShareholdersController(IShareholderRepository shareholderRepository, ICompanyRepository companyRepository, IPermissionRepository permissionRepository)
        {
            _shareholderRepository = shareholderRepository;
            _companyRepository = companyRepository;
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(string? name, int? CompanyId, int? Message)
        {
            ShareholderWithFilter model = new ShareholderWithFilter();

            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "Shareholders");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role, "Shareholders");
                }
            }

            List<ShareholderViewModel> shareholders = new List<ShareholderViewModel>();
            if (CompanyId == null)
            {
                CompanyId = 0;
            }
            model.Module = name;
            List<CompanyViewModel> companys = new List<CompanyViewModel>();
            List<SelectListItem> companies = new List<SelectListItem>();
            companys = _companyRepository.GetCompanies();
            for (int i = 0; i < companys.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = companys[i].CompanyName,
                    Value = companys[i].Id.ToString()
                });
            }
            model.Companies = companies;
            model.CompanyId = (int)CompanyId;
            model.allowedModule = allowedModule;
            Log.Information("Show list of all Shareholders");
            return View(model);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Create(string? name)
        {
            ShareholderViewModel shareholder = new ShareholderViewModel();
            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();
            for (int i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i].CompanyName,
                    Value = company[i].Id.ToString()
                });
            }
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = ""
            });
            for (int i = 0; i < countryList.Count; i++)
            {
                _countries.Add(new SelectListItem
                {
                    Text = countryList[i].CommonName,
                    Value = countryList[i].CommonName.ToString()
                });
            }
            shareholder.Countries = _countries;
            shareholder.Module = name;
            shareholder.Companies = companies;
            return View(shareholder);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Creates(ShareholderViewModel shareholder)
        {
            string username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(username);
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = ""
            });
            if (ModelState.IsValid)
            {
                int addedShare = 100 - Convert.ToInt32(shareholder.RemainingSharePercentage);
                if (Convert.ToInt32(shareholder.SharePercentage) > Convert.ToInt32(addedShare))
                {
                    ModelState.AddModelError(shareholder.SharePercentage, "You can only left " + addedShare + " share to add");
                    Log.Error("Failed while Creating new Shareholder " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share as only " + addedShare + " left.");
                    List<SelectListItem> companies = new List<SelectListItem>();
                    var company = _companyRepository.GetCompanies();
                    for (int i = 0; i < company.Count; i++)
                    {
                        companies.Add(new SelectListItem
                        {
                            Text = company[i].CompanyName,
                            Value = company[i].Id.ToString()
                        });
                    }

                    for (int i = 0; i < countryList.Count; i++)
                    {
                        _countries.Add(new SelectListItem
                        {
                            Text = countryList[i].CommonName,
                            Value = countryList[i].CommonName.ToString()
                        });
                    }
                    shareholder.Countries = _countries;
                    shareholder.Companies = companies;
                    return View("Create", shareholder);
                }
                else if (addedShare < 0)
                {
                    ModelState.AddModelError(shareholder.SharePercentage, "You already use all 100% share.");
                    Log.Error("Failed while Creating new Shareholder " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share as 100% Share is already Used.");
                    List<SelectListItem> companies = new List<SelectListItem>();
                    var company = _companyRepository.GetCompanies();
                    for (int i = 0; i < company.Count; i++)
                    {
                        companies.Add(new SelectListItem
                        {
                            Text = company[i].CompanyName,
                            Value = company[i].Id.ToString()
                        });
                    }
                    for (int i = 0; i < countryList.Count; i++)
                    {
                        _countries.Add(new SelectListItem
                        {
                            Text = countryList[i].CommonName,
                            Value = countryList[i].CommonName.ToString()
                        });
                    }
                    shareholder.Countries = _countries;
                    shareholder.Companies = companies;
                    return View("Create", shareholder);
                }
                else if (Convert.ToInt32(shareholder.SharePercentage) == 0)
                {
                    ModelState.AddModelError(shareholder.SharePercentage, "Share Percentage should be greater than zero.");
                    Log.Error("Failed while Creating new Shareholder " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share as 100% Share is already Used.");
                    List<SelectListItem> companies = new List<SelectListItem>();
                    var company = _companyRepository.GetCompanies();
                    for (int i = 0; i < company.Count; i++)
                    {
                        companies.Add(new SelectListItem
                        {
                            Text = company[i].CompanyName,
                            Value = company[i].Id.ToString()
                        });
                    }
                    for (int i = 0; i < countryList.Count; i++)
                    {
                        _countries.Add(new SelectListItem
                        {
                            Text = countryList[i].CommonName,
                            Value = countryList[i].CommonName.ToString()
                        });
                    }
                    shareholder.Countries = _countries;
                    shareholder.Companies = companies;
                    return View("Create", shareholder);
                }
                int returnId = 0;
                shareholder.CreatedBy = UserId;
                returnId = _shareholderRepository.AddNewShareholder(shareholder);
                Log.Information("Create new Shareholder " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share");
                return RedirectToAction("Index", "Shareholders", new { name = shareholder.Module });
            }
            else
            {
                List<SelectListItem> companies = new List<SelectListItem>();
                var company = _companyRepository.GetCompanies();
                for (int i = 0; i < company.Count; i++)
                {
                    companies.Add(new SelectListItem
                    {
                        Text = company[i].CompanyName,
                        Value = company[i].Id.ToString()
                    });
                }
                for (int i = 0; i < countryList.Count; i++)
                {
                    _countries.Add(new SelectListItem
                    {
                        Text = countryList[i].CommonName,
                        Value = countryList[i].CommonName.ToString()
                    });
                }
                shareholder.Countries = _countries;
                shareholder.Companies = companies;
                return View("Create", shareholder);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(string? name, int Id)
        {
            ShareholderViewModel shareholder = new ShareholderViewModel();
            List<SelectListItem> companies = new List<SelectListItem>();
            var company = _companyRepository.GetCompanies();
            for (int i = 0; i < company.Count; i++)
            {
                companies.Add(new SelectListItem
                {
                    Text = company[i].CompanyName,
                    Value = company[i].Id.ToString()
                });
            }
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();

            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = ""
            });
            for (int i = 0; i < countryList.Count; i++)
            {
                _countries.Add(new SelectListItem
                {
                    Text = countryList[i].CommonName,
                    Value = countryList[i].CommonName.ToString()
                });
            }
            shareholder = _shareholderRepository.GetDetailsOfShareholder(Id);
            int remainingShare = 0;
            remainingShare = _shareholderRepository.GetRemainingSharePercentage(shareholder.CompanyId);
            shareholder.Module = name;
            shareholder.PreviousShare = shareholder.SharePercentage;
            shareholder.Companies = companies;
            shareholder.Countries = _countries;
            shareholder.RemainingSharePercentage = remainingShare.ToString();
            return View(shareholder);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(ShareholderViewModel shareholder)
        {
            string username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(username);
            ICountryProvider countryProvider = new CountryProvider();
            var countryList = countryProvider.GetCountries().ToList();
            int remainingShareCmp = 0;
            remainingShareCmp = _shareholderRepository.GetRemainingSharePercentage(shareholder.CompanyId);
            List<SelectListItem> _countries = new List<SelectListItem>();
            _countries.Add(new SelectListItem
            {
                Text = "Select Country",
                Value = ""
            });

            if (ModelState.IsValid)
            {
                int addedShare = 100 - (Convert.ToInt32(shareholder.RemainingSharePercentage) - Convert.ToInt32(shareholder.PreviousShare));
                int remainingShare = 100 - (Convert.ToInt32(shareholder.RemainingSharePercentage));
                if (Convert.ToInt32(shareholder.SharePercentage) > Convert.ToInt32(addedShare))
                {
                    ModelState.AddModelError(shareholder.SharePercentage, "You can only left " + addedShare + " share to add");
                    List<SelectListItem> companies = new List<SelectListItem>();
                    var company = _companyRepository.GetCompanies();
                    for (int i = 0; i < company.Count; i++)
                    {
                        companies.Add(new SelectListItem
                        {
                            Text = company[i].CompanyName,
                            Value = company[i].Id.ToString()
                        });
                    }
                    for (int i = 0; i < countryList.Count; i++)
                    {
                        _countries.Add(new SelectListItem
                        {
                            Text = countryList[i].CommonName,
                            Value = countryList[i].CommonName.ToString()
                        });
                    }
                    shareholder.Countries = _countries;
                    shareholder.Companies = companies;
                    shareholder.RemainingSharePercentage = remainingShareCmp.ToString();
                    Log.Error("Failed while updating details Shareholder " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share as 100% Share is already Used.");
                    return View("Edit", shareholder);
                }
                else if (addedShare < 0)
                {
                    ModelState.AddModelError(shareholder.SharePercentage, "You already use all 100% share.");
                    List<SelectListItem> companies = new List<SelectListItem>();
                    var company = _companyRepository.GetCompanies();
                    for (int i = 0; i < company.Count; i++)
                    {
                        companies.Add(new SelectListItem
                        {
                            Text = company[i].CompanyName,
                            Value = company[i].Id.ToString()
                        });
                    }
                    for (int i = 0; i < countryList.Count; i++)
                    {
                        _countries.Add(new SelectListItem
                        {
                            Text = countryList[i].CommonName,
                            Value = countryList[i].CommonName.ToString()
                        });
                    }
                    shareholder.Countries = _countries;
                    shareholder.Companies = companies;
                    shareholder.RemainingSharePercentage = remainingShareCmp.ToString();
                    Log.Error("Failed while updating Shareholder details " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share as 100% Share is already Used.");
                    return View("Edit", shareholder);
                }
                else if (Convert.ToInt32(shareholder.SharePercentage) == 0)
                {
                    ModelState.AddModelError(shareholder.SharePercentage, "Share Percentage should be greater than zero.");
                    List<SelectListItem> companies = new List<SelectListItem>();
                    var company = _companyRepository.GetCompanies();
                    for (int i = 0; i < company.Count; i++)
                    {
                        companies.Add(new SelectListItem
                        {
                            Text = company[i].CompanyName,
                            Value = company[i].Id.ToString()
                        });
                    }
                    for (int i = 0; i < countryList.Count; i++)
                    {
                        _countries.Add(new SelectListItem
                        {
                            Text = countryList[i].CommonName,
                            Value = countryList[i].CommonName.ToString()
                        });
                    }
                    shareholder.Countries = _countries;
                    shareholder.Companies = companies;
                    shareholder.RemainingSharePercentage = remainingShareCmp.ToString();
                    Log.Error("Failed while updating Shareholder details " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share as 100% Share is already Used.");
                    return View("Edit", shareholder);
                }
                int returnId = 0;
                Log.Information("Update Details of Shareholder " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share.");
                shareholder.CreatedBy = UserId;
                returnId = _shareholderRepository.UpdateShareholderDetails(shareholder);
                return RedirectToAction("Index", "Shareholders", new { name = shareholder.Module });
            }
            else
            {
                List<SelectListItem> companies = new List<SelectListItem>();
                var company = _companyRepository.GetCompanies();
                for (int i = 0; i < company.Count; i++)
                {
                    companies.Add(new SelectListItem
                    {
                        Text = company[i].CompanyName,
                        Value = company[i].Id.ToString()
                    });
                }
                for (int i = 0; i < countryList.Count; i++)
                {
                    _countries.Add(new SelectListItem
                    {
                        Text = countryList[i].CommonName,
                        Value = countryList[i].CommonName.ToString()
                    });
                }
                shareholder.Countries = _countries;
                shareholder.Companies = companies;
                shareholder.RemainingSharePercentage = remainingShareCmp.ToString();
                return View("Edit", shareholder);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id)
        {
            int returnId = 0;
            string username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(username);
            ShareholderViewModel shareholder = new ShareholderViewModel();
            shareholder = _shareholderRepository.GetDetailsOfShareholder(Id);
            Log.Information("Delete Shareholder " + shareholder.FirstName + " " + shareholder.LastName + " With " + shareholder.SharePercentage + "% of Share.");
            returnId = _shareholderRepository.RemoveShareholder(Id, UserId);

            return new JsonResult(returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUploadDocuments(string? name, int Id, int? DocumentId, int? Message)
        {
            List<string> allowedModule = new List<string>();

            string Role = GetClaims();
            string User = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(User);
            allowedModule = _permissionRepository.GetUserPermissionName(UserId, "Shareholders");
            List<string> allowedPermission = new List<string>();
            allowedPermission = _permissionRepository.GetUserPermissionedModuleName(UserId);
            if (allowedPermission.Count == 0 && allowedModule.Count == 0)
            {
                if (allowedModule.Count == 0)
                {
                    allowedModule = _permissionRepository.GetCurrentRolePermissionName(Role,"Shareholders");
                }
            }

            MainShareholderDocumentsModel mainModel = new MainShareholderDocumentsModel();
            List<ShareholderDocuments> documents = new List<ShareholderDocuments>();
            documents = _shareholderRepository.GetShareholdersDocuments(Id, DocumentId);
            ShareholderViewModel shareholder = new ShareholderViewModel();
            shareholder = _shareholderRepository.GetDetailsOfShareholder(Id);
            if (documents.Count > 0)
            {
                foreach (var item in documents)
                {
                    item.Module = name;
                    item.ShareholderId = Id;
                }
            }
            else
            {
                ShareholderDocuments document = new ShareholderDocuments();
                document.Module = name;
                document.ShareholderId = Id;
                documents.Add(document);
            }
            if (Message > 0)
            {
                Log.Information("Upload new Document for " + shareholder.FirstName + " " + shareholder.LastName + ".");
                ViewData["Message"] = "Document Uploaded Successfully..!!";
            }
            else if (Message < 0)
            {
                Log.Error("Failed while Uploading new Document for " + shareholder.FirstName + " " + shareholder.LastName + ".");
                ViewData["Message"] = "Sorry,Due to some problem currently we are unable to upload your Document. Please try again later..!!";
            }
            mainModel.shareholderDocuments = documents;
            mainModel.allowedModule = allowedModule;
            return View(mainModel);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadDocuments(ShareholderDocuments shareholder)
        {
            int _returnId = 0;
            string username = GetUserClaims();
            int userId = _permissionRepository.GetUserIdForPermission(username);
            shareholder.CreatedBy = userId;
            _returnId = _shareholderRepository.UploadShareholderDocuments(shareholder);
            return RedirectToAction("GetUploadDocuments", "Shareholders", new { name = shareholder.Module, Id = shareholder.ShareholderId, Message = 1 });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadSelectedDocument(int DocumentId, int Id)
        {
            List<ShareholderDocuments> document = new List<ShareholderDocuments>();
            document = _shareholderRepository.GetShareholdersDocuments(Id, DocumentId);
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
        [Route("action")]
        public IActionResult GetRemainingShare(int CompanyId)
        {
            try
            {
                int remainingShare = 0;
                remainingShare = _shareholderRepository.GetRemainingSharePercentage(CompanyId);
                return new JsonResult(remainingShare);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult RemoveShareholderDocuments(int ShareholderId, int DocumentId)
        {
            try
            {
                int returnId = 0;
                string username = GetUserClaims();
                int userId = _permissionRepository.GetUserIdForPermission(username);
                returnId = _shareholderRepository.RemoveDocument(ShareholderId, DocumentId, userId);
                return new JsonResult(returnId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult ExportShareholdersToExcel(int? Id)
        {
            try
            {
                Log.Information("Export Excel file of Shareholders");
                using (XLWorkbook wb = new XLWorkbook())
                {
                    DataTable dt = new DataTable();
                    dt = _shareholderRepository.GetShareholdersForExport(Id).Tables[0];
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
        public IActionResult ExportShareholdersToPdf(int? Id)
        {
            try
            {
                Log.Information("Export PDF file of Shareholders");
                DataTable dt = new DataTable();
                dt = _shareholderRepository.GetShareholdersForExport(Id).Tables[0];
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
            doc.SetMargins(0f,0f,5f,5f);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();

            //Adding paragraph for report generated by  
            Paragraph prgGeneratedBY = new Paragraph();
            BaseFont btnAuthor = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fntAuthor = new iTextSharp.text.Font(btnAuthor, 8, 2, iTextSharp.text.BaseColor.BLUE);
            prgGeneratedBY.Alignment = Element.ALIGN_RIGHT;
            //prgGeneratedBY.Add(new Chunk("Report Generated by : ASPArticles", fntAuthor));  
            //prgGeneratedBY.Add(new Chunk("\nGenerated Date : " + DateTime.Now.ToShortDateString(), fntAuthor));  
            doc.Add(prgGeneratedBY);

            ////Adding a line  
            //Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            //doc.Add(p);

            ////Adding line break  
            //doc.Add(new Chunk("\n", fntHead));

            //Adding  PdfPTable  
            PdfPTable table = new PdfPTable(dataTable.Columns.Count);
            table.SetWidths(new float[] { 1f,1f,0.5f,1f,1f,0.5f,1f,1f,2f,0.8f});
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
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAllActiveShareholderWithPagination()
        {
            try
            {
                string searchBy = "";
                GetAllActiveShareholdersWithPaginationModel model = new GetAllActiveShareholdersWithPaginationModel();
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
                model = await _shareholderRepository.GetAllActiveShareholdersWithPagination(skip, pageSize, searchBy,searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalActiveShareholder;
                totalRecord = model.totalActiveShareholder;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.shareholders
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
        public async Task<IActionResult> GetAllInActiveShareholderWithPagination()
        {
            try
            {
                string searchBy = "";
                GetAllInActiveShareholdersWithPaginationModel model = new GetAllInActiveShareholdersWithPaginationModel();
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
                model = await _shareholderRepository.GetAllInActiveShareholdersWithPagination(skip, pageSize, searchBy, searchValue, sortColumn, sortColumnDirection);
                filterRecord = model.totalInActiveShareholder;
                totalRecord = model.totalInActiveShareholder;
                var returnObj = new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                    recordsFiltered = filterRecord,
                    data = model.shareholders
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
    }

}
