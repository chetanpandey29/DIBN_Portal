using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace DIBN.Areas.Admin.Controllers
{
    [Authorize(Policy = "AllowView")]
    [Area("Admin")]
    [Route("admin/[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class PortalBalanceExpenseController : Controller
    {
        private readonly IPortalBalanceExpensesRepository _portalBalanceExpensesRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ICompanyRepository _companyRepository;
        [Obsolete]
        private IHostingEnvironment _Environment;

        [Obsolete]
        public PortalBalanceExpenseController(IPortalBalanceExpensesRepository portalBalanceExpensesRepository,
            IPermissionRepository permissionRepository,
            ICompanyRepository companyRepository,
            IHostingEnvironment Environment)
        {
            _portalBalanceExpensesRepository = portalBalanceExpensesRepository;
            _permissionRepository = permissionRepository;
            _companyRepository = companyRepository;
            _Environment = Environment;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index(int Id, string? name,string? message)
        {
            GetAllExpenses model = new GetAllExpenses();

            CompanyViewModel companyViewModel = new CompanyViewModel();
            companyViewModel = _companyRepository.GetCompanyById(Id);
            model.getPortalBalanceDetails = _portalBalanceExpensesRepository.GetPortalBalanceDetailsForCompany(Id);
            model.portalBalanceExpenses = _portalBalanceExpensesRepository.GetAllExpensesForCompany(Id);
            model.paymentTransactions = _portalBalanceExpensesRepository.GetTransectionsDetails(Id);
            model.CompanyId = Id;
            model.Module = name;
            model.message = message;
            model.Company = companyViewModel.CompanyName;

            return View(model);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddExpenses([FromBody] List<AddNewCompanyExpenses> model)
        {
            string _user = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(_user);

            List<PortalBalanceExpenses> data = new List<PortalBalanceExpenses>();
            foreach (var item in model)
            {
                PortalBalanceExpenses expenses = new PortalBalanceExpenses();
                expenses.CompanyId = Convert.ToInt32(item.CompanyId);
                expenses.Title = item.Task;
                expenses.Quantity = Convert.ToInt32(item.Quantity);
                expenses.Amount = item.Amount;
                expenses.TotalAmount = item.TotalAmount;
                expenses.UserId = UserId;
                expenses.RemainingPortalBalance = item.RemainingBalance;
                expenses.Vat = item.Vat;
                expenses.VatAmount = item.VatAmount;
                expenses.GrandAmount = item.GrandTotal;
                expenses.CreatedOnUtc = item.Date;
                data.Add(expenses);
            }
            int _returnId = _portalBalanceExpensesRepository.AddExpensesOfCompany(data);
            return new JsonResult(1);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(int Id, int CompanyId, string? name)
        {
            PortalBalanceExpenses expenses = new PortalBalanceExpenses();
            expenses = _portalBalanceExpensesRepository.GetPortalBalanceExpenseDetail(CompanyId, Id);
            expenses.Module = name;
            return PartialView("_EditExpense", expenses);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Edits(PortalBalanceExpenses expenses)
        {
            int _returnId = 0;
            _returnId = _portalBalanceExpensesRepository.UpdateExpenseDetail(expenses);
            return RedirectToAction("Index", "PortalBalanceExpense", new { Id = expenses.CompanyId, name = expenses.Module });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int Id, int CompanyId, string Amount)
        {
            string _returnMessage = "";
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            _returnMessage = _portalBalanceExpensesRepository.DeleteExpenses(Id, Amount, CompanyId, UserId);
            return new JsonResult(_returnMessage);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddPortalBalance(string Amount, string PaymentMode, string Description, int CompanyId,
            string Actionname, string cnt, string Date, int Quantity, string TotalAmount)
        {
            int _returnId = 0;
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            string model = "";
            model = _companyRepository.GetCompanyName(CompanyId);
            _returnId = _portalBalanceExpensesRepository.AddPortalBalance("", "0", Amount, PaymentMode, Description, CompanyId, UserId, Date, Quantity, TotalAmount);
            Log.Information("Portal Balance of " + model + " is updated by " + Amount + " AED.");
            if (Actionname == "GetCompanyExpenseDetails")
            {
                return RedirectToAction(Actionname, cnt, new { Id = CompanyId, name = "MyAccount" });
            }
            if (Actionname == "Index")
            {
                return RedirectToAction(Actionname, cnt, new { Id = CompanyId, name = "Company" });
            }
            return RedirectToAction(Actionname, cnt, new { name = "MyAccount" });
            //return RedirectToAction(Actionname, cnt, Actionname == "Index" ? new { Id = CompanyId, name = "Company" } : new { name = "MyAccount" });
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadReceipt(int ReceiptId, int ExpenseReceiptId)
        {
            ExpenseReceipt document = new ExpenseReceipt();
            document = _portalBalanceExpensesRepository.GetCompanyExpenseReceipt(ReceiptId, ExpenseReceiptId);
            MemoryStream ms = new MemoryStream(document.DataBinary);
            return File(document.DataBinary, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName + document.Extension);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult DownloadExpenseReceipt(int ReceiptId)
        {
            ExpenseReceipt document = new ExpenseReceipt();
            document = _portalBalanceExpensesRepository.GetCompanyExpenseReceipt(ReceiptId);
            string Files = document.FileName;
            //string path = _Environment.WebRootPath;
            //System.IO.File.WriteAllBytes(Files, document.DataBinary);
            MemoryStream ms = new MemoryStream(document.DataBinary);
            return File(document.DataBinary, System.Net.Mime.MediaTypeNames.Application.Octet, document.FileName + document.Extension);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult UpdatePortalBalance(int CompanyId, string Amount)
        {
            int returnId = 0;
            returnId = _portalBalanceExpensesRepository.UpdatePortalBalance(CompanyId, Amount);
            return new JsonResult(returnId);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTaskDetails(string prefix)
        {
            List<string> _tasks = new List<string>();
            _tasks = _portalBalanceExpensesRepository.GetTaskList(prefix);
            return new JsonResult(_tasks);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetPaymentDetails(int Id, int CompanyId, string ActionName, string Cnt, string? name)
        {
            PaymentTransaction paymentTransaction = new PaymentTransaction();
            List<CompanyViewModel> otherCompanies = new List<CompanyViewModel>();
            otherCompanies = _companyRepository.GetCompanies();
            List<SelectListItem> companies = new List<SelectListItem>();
            List<SelectListItem> type = new List<SelectListItem>();
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
            paymentTransaction = _portalBalanceExpensesRepository.GetDetailsOfPayment(Id, CompanyId);
            paymentTransaction.Action = ActionName;
            paymentTransaction.Controller = Cnt;
            paymentTransaction.expenseType = type;
            paymentTransaction.Module = name;
            paymentTransaction.Type = "Credit";
            paymentTransaction.Companies = companies;
            paymentTransaction.PaymentModes = paymentMode;
            paymentTransaction.CompanyId = CompanyId;

            return PartialView("_GetPaymentDetails", paymentTransaction);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdatePaymentDetails(PaymentTransaction payment)
        {
            string _returnMessages = "";
            int _returnId = 0;
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            string model = "";
            model = _companyRepository.GetCompanyName(payment.CompanyId);
            payment.UserId = UserId;
            DateTime Date = Convert.ToDateTime(payment.CreatedOnUtc);
            string createdDate = Date.ToString("yyyy-MM-dd");
            payment.CreatedOnUtc = createdDate;
            if (payment.Type == "Debit")
            {
                string _returnMessage = _portalBalanceExpensesRepository.DeletePaymentTransaction(payment.Id, payment.PreviousCompanyId, payment.PreviousAmount, UserId);
                
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
                    model1.Date = payment.TransectionDate;
                    model1.TransactionId = payment.TransactionId;
                    model1.Task = payment.Description;

                    model1.UserId = UserId.ToString();
                    saveExpense.Add(model1);
                    _returnId = _portalBalanceExpensesRepository.AddCompanyExpensesAccount(saveExpense);
                }
                else
                {
                    if (payment.Action == "GetCompanyExpenseDetails")
                    {
                        return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = payment.Module, message = _returnMessage });
                    }
                    if (payment.Action == "Index")
                    {
                        return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = payment.Module, message = _returnMessage });
                    }
                    return RedirectToAction(payment.Action, payment.Controller, new { name = payment.Module, message = _returnMessage });
                }
            }
            else
            {
                if (payment.PreviousCompanyId != payment.CompanyId)
                {
                    string _returnMessage = _portalBalanceExpensesRepository.DeletePaymentTransaction(payment.Id, payment.PreviousCompanyId, payment.PreviousAmount, UserId);
                    if (_returnMessage.Contains("Selected Payment Transaction Deleted Successfully."))
                    {
                        _returnId = _portalBalanceExpensesRepository.AddPortalBalance(payment.TransactionId, "0", payment.Amount, payment.PaymentMode, payment.Description, payment.CompanyId, UserId, payment.TransectionDate, payment.Quantity, payment.TotalAmount);
                    }
                }
                else
                {
                    _returnMessages = _portalBalanceExpensesRepository.UpdatePaymentDetails(payment);
                }
            }

            Log.Information("Payment Transaction of " + model + " is updated by " + payment.Amount + " AED.");
            if(_returnMessages.Contains("Selected Payment Transaction Updated Successfully."))
            {
                if (payment.Action == "GetCompanyExpenseDetails")
                {
                    return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = payment.Module });
                }
                if (payment.Action == "Index")
                {
                    return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = payment.Module });
                }
                return RedirectToAction(payment.Action, payment.Controller, new { name = payment.Module });
            }

            if (payment.Action == "GetCompanyExpenseDetails")
            {
                return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = payment.Module,message = _returnMessages });
            }
            if (payment.Action == "Index")
            {
                return RedirectToAction(payment.Action, payment.Controller, new { Id = payment.PreviousCompanyId, name = payment.Module, message = _returnMessages });
            }
            return RedirectToAction(payment.Action, payment.Controller, new { name = payment.Module, message = _returnMessages });
            //return RedirectToAction(payment.Action, payment.Controller, payment.Action=="Index"? new { Id = payment.CompanyId, name = "Company" } : new { name = "MyAccount" });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult DeletePaymentDetails(int Id, int CompanyId, string Amount)
        {
            string _returnMessage = "";
            string Username = GetUserClaims();
            int UserId = _permissionRepository.GetUserIdForPermission(Username);
            _returnMessage = _portalBalanceExpensesRepository.DeletePaymentTransaction(Id, CompanyId, Amount, UserId);
            return new JsonResult(_returnMessage);
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
