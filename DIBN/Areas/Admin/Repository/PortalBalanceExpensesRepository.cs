using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using X.PagedList;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class PortalBalanceExpensesRepository : IPortalBalanceExpensesRepository
    {
        private readonly Connection _dataSetting;
        private readonly ILogger<PortalBalanceExpensesRepository> _logger;
        public PortalBalanceExpensesRepository(Connection dataSetting, ILogger<PortalBalanceExpensesRepository> logger)
        {
            _dataSetting = dataSetting;
            _logger = logger;
        }
        public GetPortalBalanceDetails GetPortalBalanceDetailsForCompany(int CompanyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetPortalBalanceDetails model = new GetPortalBalanceDetails();
                SqlCommand command = new SqlCommand("USP_Admin_GetPortalBalanceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    model.Id = Convert.ToInt32(dr["Id"]);
                    model.PortalBalance = dr["PortalBalance"].ToString();
                    model.CompanyId = Convert.ToInt32(dr["CompanyId"]);
                    model.CompanyName = dr["CompanyName"].ToString();
                    model.UserId = Convert.ToInt32(dr["UserId"]);
                    model.Username = dr["Username"].ToString();
                    model.UserEmailId = dr["UserEmailId"].ToString();
                    model.CreatedOnUtc = dr["CreatedOnUtc"].ToString();
                    model.ModifyOnUtc = dr["ModifyOnUtc"].ToString();
                }
                connection.Close();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Get All Expenses of Company                                 -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public List<PortalBalanceExpenses> GetAllExpensesForCompany(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<PortalBalanceExpenses> portalBalanceExpenses = new List<PortalBalanceExpenses>();
                SqlCommand command = new SqlCommand("USP_Admin_PortalBalanceExpensesOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PortalBalanceExpenses expenses = new PortalBalanceExpenses();
                    expenses.Id = Convert.ToInt32(reader["Id"]);
                    expenses.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    expenses.Title = reader["ExpensesTitle"].ToString();
                    expenses.Amount = reader["ExpensesAmount"].ToString();
                    expenses.Quantity = Convert.ToInt32(reader["Quantity"]);
                    expenses.TotalAmount = reader["TotalAmount"].ToString();
                    expenses.Vat = reader["Vat"].ToString();
                    expenses.VatAmount = reader["VatAmount"].ToString();
                    expenses.GrandAmount = reader["GrandTotal"].ToString();
                    expenses.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    expenses.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    expenses.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    expenses.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    portalBalanceExpenses.Add(expenses);
                }
                con.Close();
                return portalBalanceExpenses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Store Company Expenses                                                  -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="expenses"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AddExpensesOfCompany(List<PortalBalanceExpenses> expenses)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = null;

                int LastIndex = expenses.Count - 1;
                string RemainingPortalBalance = expenses[LastIndex].RemainingPortalBalance;


                for (int i = 0; i < expenses.Count; i++)
                {
                    command = new SqlCommand("USP_Admin_PortalBalanceExpensesOperation", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Status", Operation.Insert);
                    command.Parameters.AddWithValue("@CompanyId", expenses[i].CompanyId);
                    command.Parameters.AddWithValue("@ExpensesTitle", expenses[i].Title);
                    command.Parameters.AddWithValue("@ExpenseAmount", expenses[i].Amount);
                    command.Parameters.AddWithValue("@Quantity", expenses[i].Quantity);
                    command.Parameters.AddWithValue("@TotalAmount", expenses[i].TotalAmount);
                    command.Parameters.AddWithValue("@UserId", expenses[i].UserId);
                    command.Parameters.AddWithValue("@RemainingPortalBalance", RemainingPortalBalance);
                    command.Parameters.AddWithValue("@CreatedOnUtc", expenses[i].CreatedOnUtc);
                    command.Parameters.AddWithValue("@Vat", expenses[i].Vat);
                    command.Parameters.AddWithValue("@VatAmount", expenses[i].VatAmount);
                    command.Parameters.AddWithValue("@GrandTotal", expenses[i].GrandAmount);
                    con.Open();
                    _returnId = (int)command.ExecuteScalar();
                    con.Close();
                    command.Parameters.Clear();
                }

                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Get Particular Expense Detail of Company                                                        -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="ExpenseId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public PortalBalanceExpenses GetPortalBalanceExpenseDetail(int CompanyId, int ExpenseId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                PortalBalanceExpenses expenses = new PortalBalanceExpenses();
                SqlCommand command = new SqlCommand("USP_Admin_PortalBalanceExpensesOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Id", ExpenseId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    expenses.Id = Convert.ToInt32(reader["Id"]);
                    expenses.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    expenses.Title = reader["ExpensesTitle"].ToString();
                    expenses.Amount = reader["ExpensesAmount"].ToString();
                    expenses.Quantity = Convert.ToInt32(reader["Quantity"]);
                    expenses.TotalAmount = reader["TotalAmount"].ToString();
                    expenses.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    expenses.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    expenses.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    expenses.Vat = reader["Vat"].ToString();
                    expenses.VatAmount = reader["VatAmount"].ToString();
                    expenses.GrandAmount = reader["GrandTotal"].ToString();
                    expenses.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                }
                con.Close();
                return expenses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Update Expense Details                  -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="expenses"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateExpenseDetail(PortalBalanceExpenses expenses)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_PortalBalanceExpensesOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Update);
                command.Parameters.AddWithValue("@CompanyId", expenses.CompanyId);
                command.Parameters.AddWithValue("@Id", expenses.Id);
                command.Parameters.AddWithValue("@ExpensesTitle", expenses.Title);
                command.Parameters.AddWithValue("@ExpenseAmount", expenses.Amount);
                command.Parameters.AddWithValue("@Quantity", expenses.Quantity);
                command.Parameters.AddWithValue("@TotalAmount", expenses.TotalAmount);
                command.Parameters.AddWithValue("@Vat", expenses.Vat);
                command.Parameters.AddWithValue("@VatAmount", expenses.VatAmount);
                command.Parameters.AddWithValue("@GrandTotal", expenses.GrandAmount);
                command.Parameters.AddWithValue("@CreatedOnDate", expenses.CreatedOnUtc);
                command.Parameters.AddWithValue("@UserId", expenses.UserId);

                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                if (expenses.FormFile != null)
                {
                    string _getName = expenses.FormFile.FileName;
                    var Name = expenses.FormFile.FileName.Split(".");
                    string FileName = Name[0];
                    var extn = Path.GetExtension(_getName);
                    Byte[] bytes = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        expenses.FormFile.OpenReadStream().CopyTo(ms);
                        bytes = ms.ToArray();
                    }

                    command = new SqlCommand("USP_Admin_ExpesesRecieptOperation", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Status", Operation.Insert);
                    command.Parameters.AddWithValue("@CompanyId", expenses.CompanyId);
                    command.Parameters.AddWithValue("@DataBinary", bytes);
                    command.Parameters.AddWithValue("@FileName", FileName);
                    command.Parameters.AddWithValue("@Extension", extn);
                    command.Parameters.AddWithValue("@ReceiptId", _returnId);
                    command.Parameters.AddWithValue("@UserId", expenses.UserId);
                    con.Open();
                    _returnId = (int)command.ExecuteScalar();
                    con.Close();
                }
                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Delete Company Expense                      -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Amount"></param>
        /// <param name="CompanyId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string DeleteExpenses(int Id, string Amount, int CompanyId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _returnId = "";
                ResponseModel model = new ResponseModel();
                SqlCommand command = new SqlCommand("USP_Admin_PortalBalanceExpensesOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@RemainingPortalBalance", Amount);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Username"] != DBNull.Value)
                        model.Username = reader["Username"].ToString();
                    if (reader["ModifyTime"] != DBNull.Value)
                        model.ModifyTime = reader["ModifyTime"].ToString();
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["ResponseData"] != DBNull.Value)
                        model.ResponseData = Convert.ToInt32(reader["ResponseData"]);
                }
                con.Close();
                if (model.ResponseData > 0)
                {
                    _returnId = "Selected Expense Deleted Successfully.";
                }
                else
                {
                    _returnId = "Any operation does not perform on selected expense (<b>" + model.TransactionId + "</b>) because it is already modified by <b>" + model.Username + "</b> at " + model.ModifyTime;
                }
                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Add Company Portal Balance                                  -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="TransactionId"></param>
        /// <param name="Amount"></param>
        /// <param name="BalanceAmount"></param>
        /// <param name="PaymentMode"></param>
        /// <param name="Description"></param>
        /// <param name="CompanyId"></param>
        /// <param name="UserId"></param>
        /// <param name="Date"></param>
        /// <param name="Quantity"></param>
        /// <param name="TotalAmount"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AddPortalBalance(string TransactionId, string Amount, string BalanceAmount, string PaymentMode, string Description, int CompanyId, int UserId, string Date, int Quantity, string TotalAmount)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_PortalBalanceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@PortalAmount", TotalAmount);
                command.Parameters.AddWithValue("@IsCompletelyUsed", 0);
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();

                command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@PortalAmount", BalanceAmount);
                command.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                command.Parameters.AddWithValue("@Quantity", Quantity);
                command.Parameters.AddWithValue("@PaymentMode", PaymentMode);
                if (TransactionId != "" && TransactionId != null)
                    command.Parameters.AddWithValue("@TransactionNumber", TransactionId);
                command.Parameters.AddWithValue("@CurrentAmount", Amount != "0" ? Amount : BalanceAmount);
                command.Parameters.AddWithValue("@Description", Description);
                command.Parameters.AddWithValue("@PaymentStatus", "Success");
                command.Parameters.AddWithValue("@CreatedOn", Date);
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                con.Open();
                int PaymentId = (int)command.ExecuteScalar();
                con.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_SavePaymentReceipt", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@PaymentId", PaymentId);
                command.Parameters.AddWithValue("@OnAccount", Description);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();

                command = new SqlCommand("USP_Admin_GetTransactionNumber", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                string transactionId = (string)command.ExecuteScalar();
                con.Close();

                command = new SqlCommand("USP_Admin_SaveAccountHistory", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TransactionId", TransactionId != null && TransactionId != "" ? TransactionId : transactionId);
                command.Parameters.AddWithValue("@Description", Description);
                command.Parameters.AddWithValue("@Amount", Amount != "0" ? Amount : BalanceAmount);
                command.Parameters.AddWithValue("@Quantity", Quantity);
                command.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                command.Parameters.AddWithValue("@Vat", 0);
                command.Parameters.AddWithValue("@VatAmount", 0);
                command.Parameters.AddWithValue("@GrandTotal", TotalAmount);
                command.Parameters.AddWithValue("@ExpenseType", "Credit");
                command.Parameters.AddWithValue("@TransactionDate", Date);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@CreatedBy", UserId);
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
                command.Parameters.Clear();

                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Get Company Expense Receipt By Id and Receipt Id                            -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="ReceiptId"></param>
        /// <param name="ExpenseReceiptId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ExpenseReceipt GetCompanyExpenseReceipt(int ReceiptId, int ExpenseReceiptId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ExpenseReceipt expenseReceipt = new ExpenseReceipt();
                SqlCommand command = new SqlCommand("USP_Admin_ExpesesRecieptOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", ExpenseReceiptId);
                command.Parameters.AddWithValue("@ReceiptId", ReceiptId);
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    expenseReceipt.Id = Convert.ToInt32(reader["Id"]);
                    expenseReceipt.FileName = reader["FileName"].ToString();
                    expenseReceipt.Extension = reader["Extension"].ToString();
                    expenseReceipt.DataBinary = (Byte[])reader["DataBinary"];
                    expenseReceipt.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    expenseReceipt.ReceiptId = Convert.ToInt32(reader["ReceiptId"]);
                    expenseReceipt.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    expenseReceipt.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    expenseReceipt.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    expenseReceipt.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                }
                con.Close();
                return expenseReceipt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Get Company Expense By Id                               -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="ReceiptId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ExpenseReceipt GetCompanyExpenseReceipt(int ReceiptId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ExpenseReceipt expenseReceipt = new ExpenseReceipt();
                SqlCommand command = new SqlCommand("USP_Admin_ExpesesRecieptOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", ReceiptId);
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    expenseReceipt.Id = Convert.ToInt32(reader["Id"]);
                    expenseReceipt.FileName = reader["FileName"].ToString();
                    expenseReceipt.Extension = reader["Extension"].ToString();
                    expenseReceipt.DataBinary = (Byte[])reader["DataBinary"];
                    expenseReceipt.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    expenseReceipt.ReceiptId = Convert.ToInt32(reader["ReceiptId"]);
                    expenseReceipt.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    expenseReceipt.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    expenseReceipt.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    expenseReceipt.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                }
                con.Close();
                return expenseReceipt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Get All Transaction of Company                  -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<PaymentTransaction> GetTransectionsDetails(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<PaymentTransaction> transections = new List<PaymentTransaction>();
                SqlCommand command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PaymentTransaction transection = new PaymentTransaction();
                    transection.Id = Convert.ToInt32(reader["Id"]);
                    transection.UserId = Convert.ToInt32(reader["UserId"]);
                    transection.Username = reader["Username"].ToString();
                    transection.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    transection.CompanyName = reader["CompanyName"].ToString();
                    transection.Amount = reader["Amount"].ToString();
                    transection.Quantity = Convert.ToInt32(reader["Quantity"]);
                    transection.TotalAmount = reader["TotalAmount"].ToString();
                    transection.PaymentMode = reader["PaymentMode"].ToString();
                    transection.Description = reader["Description"].ToString();
                    transection.PaymentStatus = reader["PaymentStatus"].ToString();
                    transection.TransectionDate = reader["TransectionDate"].ToString();
                    transection.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    transection.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    transections.Add(transection);
                }
                con.Close();
                return transections;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Update Portal Balance Details                                               -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdatePortalBalance(int CompanyId, string Amount)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_PortalBalanceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Status", 5);
                command.Parameters.AddWithValue("@PortalAmount", Amount);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get Company Account Details from Passed From date to Passed To Date                             -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetCompanyAccountDetailModel GetCompanyAccount(int CompanyId, string FromDate, string ToDate)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetCompanyAccountDetailModel model = new GetCompanyAccountDetailModel();
                model.getexpensesofCompanies = new List<GetexpensesofCompany>();

                List<GetexpensesofCompany> getdata = new List<GetexpensesofCompany>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyAccountDetails", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                if (FromDate != null && ToDate != null)
                {
                    command.Parameters.AddWithValue("@FromDate", FromDate);
                    command.Parameters.AddWithValue("@ToDate", ToDate);
                }

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetexpensesofCompany getexpensesofCompany = new GetexpensesofCompany();
                    if (reader["Date"] != DBNull.Value && reader["Date"].ToString() != "-1")
                        getexpensesofCompany.Date = reader["Date"].ToString();
                    if (reader["PaymentCredit"] != DBNull.Value && reader["PaymentCredit"].ToString() != "-1")
                    {
                        getexpensesofCompany.Description = "Credit: " + reader["PaymentMode"].ToString() + " Received - # " + reader["PaymentCredit"].ToString() + " ( Description : " + reader["Description"].ToString() + ")";
                        getexpensesofCompany.Balance += Convert.ToDecimal(reader["PaymentCredit"].ToString());
                        getexpensesofCompany.Credit = reader["PaymentCredit"].ToString();
                        getexpensesofCompany.Debit = "---";
                    }
                    if (reader["PaymentTransactionId"] != DBNull.Value)
                        getexpensesofCompany.PaymentTransactionId = Convert.ToInt32(reader["PaymentTransactionId"]);
                    if (reader["PaymentCredit"] != DBNull.Value)
                        getexpensesofCompany.PaymentCredit = reader["PaymentCredit"].ToString();
                    if (reader["TransactionId"] != DBNull.Value)
                        getexpensesofCompany.TransactionId = reader["TransactionId"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        getexpensesofCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        getexpensesofCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["ExpenseReceiptId"] != DBNull.Value)
                        getexpensesofCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    getexpensesofCompany.Type = "Credit";
                    if (getexpensesofCompany.Description != null)
                        getdata.Add(getexpensesofCompany);

                }

                reader.NextResult();
                while (reader.Read())
                {
                    GetexpensesofCompany getexpensesofCompany = new GetexpensesofCompany();
                    if (reader["Date"] != DBNull.Value && reader["Date"].ToString() != "-1")
                        getexpensesofCompany.Date = reader["Date"].ToString();
                    if (reader["ExpensesTitle"] != DBNull.Value && reader["ExpensesTitle"].ToString() != "-1")
                    {
                        getexpensesofCompany.Description = "Debit: " + reader["ExpensesTitle"].ToString() + "#";
                        if (reader["Vat"] != DBNull.Value && reader["VatAmount"] != DBNull.Value && reader["Vat"].ToString() != "0")
                        {
                            getexpensesofCompany.Description += " Incl. VAT@" + reader["Vat"].ToString() + "%" + " (" + Convert.ToDecimal(reader["GrandTotal"].ToString()) + " AED)";
                        }
                        else
                        {
                            getexpensesofCompany.Description += " (" + Convert.ToDecimal(reader["GrandTotal"].ToString()) + " AED)";
                        }
                    }
                    if (reader["GrandTotal"] != DBNull.Value && reader["GrandTotal"].ToString() != "-1")
                    {
                        getexpensesofCompany.Balance += Convert.ToDecimal(reader["GrandTotal"].ToString());
                        getexpensesofCompany.Debit = reader["GrandTotal"].ToString();
                        getexpensesofCompany.Credit = "---";
                    }
                    if (reader["TransactionId"] != DBNull.Value)
                        getexpensesofCompany.TransactionId = reader["TransactionId"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        getexpensesofCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["Vat"] != DBNull.Value)
                        getexpensesofCompany.Vat = reader["Vat"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        getexpensesofCompany.VatAmount = reader["VatAmount"].ToString();
                    if (reader["Id"] != DBNull.Value)
                        getexpensesofCompany.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        getexpensesofCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["ExpenseReceiptId"] != DBNull.Value)
                        getexpensesofCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    getexpensesofCompany.Type = "Debit";
                    if (getexpensesofCompany.Description != null)
                        getdata.Add(getexpensesofCompany);
                }
                con.Close();
                var getalldata = from data in getdata
                                 orderby data.TransactionIdNo
                                 select data;
                model.getexpensesofCompanies = getalldata.ToList();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0, temp_debit = 0;
                for (int index = 0; index < model.getexpensesofCompanies.Count; index++)
                {
                    if (model.getexpensesofCompanies.Count > 0 && model.getexpensesofCompanies != null && model.getexpensesofCompanies[index].Description != null)
                    {
                        if (model.getexpensesofCompanies[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(model.getexpensesofCompanies[index].Credit);
                            model.TotalCredit = totalCredit.ToString();

                            totalBalance = totalBalance + Convert.ToDecimal(model.getexpensesofCompanies[index].Credit);
                            model.TotalBalance = totalBalance.ToString();

                            model.getexpensesofCompanies[index].Balance = totalBalance;
                        }
                        if (model.getexpensesofCompanies[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(model.getexpensesofCompanies[index].Debit);
                            model.TotalDebit = "-" + totalDebit.ToString();
                            totalBalance = totalBalance - Convert.ToDecimal(model.getexpensesofCompanies[index].Debit);
                            model.getexpensesofCompanies[index].Balance = totalBalance;
                            model.TotalBalance = totalBalance.ToString();
                            model.getexpensesofCompanies[index].Debit = "-" + model.getexpensesofCompanies[index].Debit;
                        }
                    }
                }
                if (totalDebit == 0)
                {
                    model.TotalDebit = "0.0";
                }
                if (totalCredit == 0)
                {
                    model.TotalCredit = "0.0";
                }
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public async Task<GetCompanyAccountDetailModel> GetCompanyTotalAccount(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetCompanyAccountDetailModel model = new GetCompanyAccountDetailModel();
                model.getexpensesofCompanies = new List<GetexpensesofCompany>();

                List<GetexpensesofCompany> getdata = new List<GetexpensesofCompany>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyAccountDetails", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);

                con.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetexpensesofCompany getexpensesofCompany = new GetexpensesofCompany();
                    if (reader["PaymentCredit"] != DBNull.Value && reader["PaymentCredit"].ToString() != "-1")
                    {
                        getexpensesofCompany.Description = "Credit: " + reader["PaymentMode"].ToString() + " Received - # " + reader["PaymentCredit"].ToString() + " ( Description : " + reader["Description"].ToString() + ")";
                        getexpensesofCompany.Balance += Convert.ToDecimal(reader["PaymentCredit"].ToString());
                        getexpensesofCompany.Credit = reader["PaymentCredit"].ToString();
                        getexpensesofCompany.Debit = "---";
                    }
                    if (reader["PaymentTransactionId"] != DBNull.Value)
                        getexpensesofCompany.PaymentTransactionId = Convert.ToInt32(reader["PaymentTransactionId"]);
                    getexpensesofCompany.Type = "Credit";
                    if (getexpensesofCompany.Description != null)
                        getdata.Add(getexpensesofCompany);

                }

                await reader.NextResultAsync();
                while (reader.Read())
                {
                    GetexpensesofCompany getexpensesofCompany = new GetexpensesofCompany();
                    if (reader["ExpensesTitle"] != DBNull.Value && reader["ExpensesTitle"].ToString() != "-1")
                    {
                        getexpensesofCompany.Description = "Debit: " + reader["ExpensesTitle"].ToString() + "#";
                        if (reader["Vat"] != DBNull.Value && reader["VatAmount"] != DBNull.Value && reader["Vat"].ToString() != "0")
                        {
                            getexpensesofCompany.Description += " Incl. VAT@" + reader["Vat"].ToString() + "%" + " (" + Convert.ToDecimal(reader["GrandTotal"].ToString()) + " AED)";
                        }
                        else
                        {
                            getexpensesofCompany.Description += " (" + Convert.ToDecimal(reader["GrandTotal"].ToString()) + " AED)";
                        }
                    }
                    if (reader["GrandTotal"] != DBNull.Value && reader["GrandTotal"].ToString() != "-1")
                    {
                        getexpensesofCompany.Balance += Convert.ToDecimal(reader["GrandTotal"].ToString());
                        getexpensesofCompany.Debit = reader["GrandTotal"].ToString();
                        getexpensesofCompany.Credit = "---";
                    }
                    if (reader["Id"] != DBNull.Value)
                        getexpensesofCompany.Id = Convert.ToInt32(reader["Id"]);
                    getexpensesofCompany.Type = "Debit";
                    if (getexpensesofCompany.Description != null)
                        getdata.Add(getexpensesofCompany);
                }
                con.Close();
                var getalldata = from data in getdata
                                 orderby data.TransactionIdNo
                                 select data;
                model.getexpensesofCompanies = getalldata.ToList();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0, temp_debit = 0;
                for (int index = 0; index < model.getexpensesofCompanies.Count; index++)
                {
                    if (model.getexpensesofCompanies.Count > 0 && model.getexpensesofCompanies != null && model.getexpensesofCompanies[index].Description != null)
                    {
                        if (model.getexpensesofCompanies[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(model.getexpensesofCompanies[index].Credit);
                            model.TotalCredit = totalCredit.ToString();

                            totalBalance = totalBalance + Convert.ToDecimal(model.getexpensesofCompanies[index].Credit);
                            model.TotalBalance = totalBalance.ToString();

                            model.getexpensesofCompanies[index].Balance = totalBalance;
                        }
                        if (model.getexpensesofCompanies[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(model.getexpensesofCompanies[index].Debit);
                            model.TotalDebit = "-" + totalDebit.ToString();
                            totalBalance = totalBalance - Convert.ToDecimal(model.getexpensesofCompanies[index].Debit);
                            model.getexpensesofCompanies[index].Balance = totalBalance;
                            model.TotalBalance = totalBalance.ToString();
                            model.getexpensesofCompanies[index].Debit = "-" + model.getexpensesofCompanies[index].Debit;
                        }
                    }
                }
                if (totalDebit == 0)
                {
                    model.TotalDebit = "0.0";
                }
                if (totalCredit == 0)
                {
                    model.TotalCredit = "0.0";
                }
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public async Task<GetCompanyAccountDetailPaginationModel> GetCompanyAccountWithPagination(int CompanyId, int skipRows, int takeRows, string sortBy, string sortingDirection, string searchBy, string searchPrefix)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                var getalldata = new List<GetexpensesofCompany>();
                decimal balance = 0;
                GetCompanyAccountDetailPaginationModel model = new GetCompanyAccountDetailPaginationModel();
                model.getexpensesofCompanies = new List<GetCompanyWiseExpensesWithPagination>();

                List<GetCompanyWiseExpensesWithPagination> getdata = new List<GetCompanyWiseExpensesWithPagination>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyAccountDetailBalanceForPagination", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                if (sortBy != null)
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                else
                    command.Parameters.AddWithValue("@sortColumn", "Transaction Id");

                if (sortingDirection != null)
                    command.Parameters.AddWithValue("@sortDirection", sortingDirection);
                else
                    command.Parameters.AddWithValue("@sortDirection", "asc");

                if (searchBy == "Transaction Id")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        command.Parameters.AddWithValue("@searchBy", "Transaction Id No");
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@searchBy", searchBy);
                    }
                }
                else
                {
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                }

                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);

                con.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        balance = balance + Convert.ToDecimal(reader["GrandTotal"]);
                    }
                }
                con.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_GetCompanyAccountDetailWithPagination", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", takeRows);
                if (sortBy != null)
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                else
                    command.Parameters.AddWithValue("@sortColumn", "Transaction Id");

                if (sortingDirection != null)
                    command.Parameters.AddWithValue("@sortDirection", sortingDirection);
                else
                    command.Parameters.AddWithValue("@sortDirection", "asc");

                if (searchBy == "Transaction Id")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        command.Parameters.AddWithValue("@searchBy", "Transaction Id No");
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@searchBy", searchBy);
                    }
                }
                else
                {
                    command.Parameters.AddWithValue("@searchBy", searchBy);
                }

                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetCompanyWiseExpensesWithPagination getexpensesofCompany = new GetCompanyWiseExpensesWithPagination();
                    if (reader["Date"] != DBNull.Value)
                    {
                        getexpensesofCompany.Date = reader["Date"].ToString();
                    }

                    if (reader["TransactionType"] != DBNull.Value && Convert.ToString(reader["TransactionType"]) == "Credit")
                    {
                        getexpensesofCompany.Description = "Credit: " + reader["PaymentMode"].ToString() + " Received - # " + reader["GrandTotal"].ToString() + " ( Description : " + reader["Description"].ToString() + ")";
                        balance = balance + Convert.ToDecimal(reader["GrandTotal"]);
                        getexpensesofCompany.Balance = balance;
                        getexpensesofCompany.Credit = reader["GrandTotal"].ToString();
                        getexpensesofCompany.Debit = "---";
                    }

                    if (reader["TransactionType"] != DBNull.Value && Convert.ToString(reader["TransactionType"]) == "Debit")
                    {
                        getexpensesofCompany.Description = "Debit: " + reader["Description"].ToString() + "#";
                        if (reader["Vat"] != DBNull.Value && reader["VatAmount"] != DBNull.Value && reader["Vat"].ToString() != "0")
                        {
                            getexpensesofCompany.Description += " Incl. VAT@" + reader["Vat"].ToString() + "%" + " (" + Convert.ToDecimal(reader["GrandTotal"].ToString()) + " AED)";
                        }
                        else
                        {
                            getexpensesofCompany.Description += " (" + Convert.ToDecimal(reader["GrandTotal"].ToString()) + " AED)";
                        }
                        balance = balance - Convert.ToDecimal(reader["GrandTotal"]);
                        getexpensesofCompany.Balance = balance;
                        getexpensesofCompany.Debit = reader["GrandTotal"].ToString();
                        getexpensesofCompany.Credit = "---";
                    }

                    if (reader["Id"] != DBNull.Value)
                        getexpensesofCompany.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        getexpensesofCompany.GrandTotal = reader["GrandTotal"].ToString();
                    if (reader["TransactionId"] != DBNull.Value)
                        getexpensesofCompany.TransactionId = reader["TransactionId"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        getexpensesofCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        getexpensesofCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["ExpenseReceiptId"] != DBNull.Value)
                        getexpensesofCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    if (reader["Vat"] != DBNull.Value)
                        getexpensesofCompany.Vat = reader["Vat"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        getexpensesofCompany.VatAmount = reader["VatAmount"].ToString();
                    getexpensesofCompany.TransactionType = reader["TransactionType"].ToString();
                    if (getexpensesofCompany.Description != null)
                        getdata.Add(getexpensesofCompany);

                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TransactionCount"] != DBNull.Value)
                        model.totalAccounts = Convert.ToInt32(reader["TransactionCount"]);
                }

                con.Close();

                model.getexpensesofCompanies = getdata;
                //if (searchPrefix != null && searchBy != null)
                //{
                //    if (searchBy == "Transaction Id")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                //        {
                //            getalldata = (from data in getdata
                //                             where data.TransactionIdNo == Convert.ToInt32(searchPrefix)
                //                             select data).ToList();
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                //            Match result = re.Match(searchPrefix);
                //            string alphaPart = result.Groups[1].Value.ToUpper();
                //            var margeStr = alphaPart + result.Groups[2].Value;
                //            getalldata = (from data in getdata
                //                             where data.TransactionId == searchPrefix.ToString() ||
                //                             data.TransactionId == margeStr
                //                             select data).ToList();
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Date")
                //    {
                //        DateTime dt = DateTime.ParseExact(searchPrefix, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                //        getalldata = (from data in getdata
                //                         where data.CreatedOn == dt
                //                         select data).ToList();
                //        getdata = getalldata.ToList();
                //        totalHistoryCount = getdata.Count();
                //    }
                //    else if (searchBy == "Description")
                //    {
                //        var search = searchPrefix.Substring(0, 1).ToUpper();
                //        var searchStr = searchPrefix.Substring(1, searchPrefix.Length - 1);
                //        var searchStr1 = search + searchStr;
                //        getalldata = (from data in getdata
                //                      where data.Description.Contains(searchPrefix, StringComparison.CurrentCultureIgnoreCase)
                //                      select data).ToList();
                //        getdata = getalldata.ToList();
                //        totalHistoryCount = getdata.Count();
                //    }
                //    else if (searchBy == "Credit(AED)")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            getalldata = (from data in getdata
                //                          where data.Credit != "---" && Convert.ToDecimal(data.Credit) == Convert.ToDecimal(searchPrefix)
                //                          select data).ToList();
                //            if (getalldata.Any())
                //            {
                //                getdata = getalldata.ToList();
                //                totalHistoryCount = getdata.Count();
                //            }
                //            else
                //            {
                //                getdata = new List<GetexpensesofCompany>();
                //                totalHistoryCount = getdata.Count();
                //            }
                //        }
                //        else
                //        {
                //            getdata = new List<GetexpensesofCompany>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Debit(AED)")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            getalldata = (from data in getdata
                //                          where data.Debit != "---" && Convert.ToDecimal(data.Debit) == Convert.ToDecimal(searchPrefix)
                //                          select data).ToList();
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetexpensesofCompany>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Vat(%)")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            getalldata = (from data in getdata
                //                             where Convert.ToDecimal(data.Vat) == Convert.ToDecimal(searchPrefix)
                //                             && data.Vat != "---"
                //                             select data).ToList();
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetexpensesofCompany>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Vat Amount")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            getalldata = (from data in getdata
                //                             where Convert.ToDecimal(data.VatAmount) == Convert.ToDecimal(searchPrefix) 
                //                             && data.VatAmount != "---"
                //                             select data).ToList();
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetexpensesofCompany>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //}

                //if(sortBy!="" && sortBy != null)
                //{
                //    if (sortingDirection != null && sortingDirection == "desc")
                //    {
                //        if (sortBy == "Date")
                //        {
                //            getalldata = (from data in getdata
                //                             orderby data.CreatedOn descending
                //                             select data).ToList();
                //            var getHistoryOfCompanyExpense = getalldata.Skip(skipRows).Take(takeRows).ToList();
                //            model.getexpensesofCompanies = getHistoryOfCompanyExpense;
                //            model.totalAccounts = totalHistoryCount!=0?totalHistoryCount:getalldata.Count();
                //        }
                //        else if (sortBy == "Transaction Id")
                //        {
                //            getalldata = (from data in getdata
                //                             orderby data.TransactionIdNo descending
                //                             select data).ToList();
                //            var getHistoryOfCompanyExpense = getalldata.Skip(skipRows).Take(takeRows).ToList();
                //            model.getexpensesofCompanies = getHistoryOfCompanyExpense;
                //            model.totalAccounts = totalHistoryCount != 0 ? totalHistoryCount : getalldata.Count();
                //        }
                //        else if (sortBy == "Description")
                //        {
                //            getalldata = (from data in getdata
                //                             orderby data.Description descending
                //                             select data).ToList();
                //            var getHistoryOfCompanyExpense = getalldata.Skip(skipRows).Take(takeRows).ToList();
                //            model.getexpensesofCompanies = getHistoryOfCompanyExpense;
                //            model.totalAccounts = totalHistoryCount != 0 ? totalHistoryCount : getalldata.Count();
                //        }
                //    }
                //    else
                //    {
                //        if (sortBy == "Date")
                //        {
                //            getalldata = (from data in getdata
                //                             orderby data.CreatedOn
                //                             select data).ToList();
                //            var getHistoryOfCompanyExpense = getalldata.Skip(skipRows).Take(takeRows).ToList();
                //            model.getexpensesofCompanies = getHistoryOfCompanyExpense;
                //            model.totalAccounts = totalHistoryCount != 0 ? totalHistoryCount : getalldata.Count();
                //        }
                //        else if (sortBy == "Transaction Id")
                //        {
                //            getalldata = (from data in getdata
                //                             orderby data.TransactionIdNo
                //                             select data).ToList();
                //            var getHistoryOfCompanyExpense = getalldata.Skip(skipRows).Take(takeRows).ToList();
                //            model.getexpensesofCompanies = getHistoryOfCompanyExpense;
                //            model.totalAccounts = totalHistoryCount != 0 ? totalHistoryCount : getalldata.Count();
                //        }
                //        else if (sortBy == "Description")
                //        {
                //            getalldata = (from data in getdata
                //                             orderby data.Description
                //                             select data).ToList();
                //            var getHistoryOfCompanyExpense = getalldata.Skip(skipRows).Take(takeRows).ToList();
                //            model.getexpensesofCompanies = getHistoryOfCompanyExpense;
                //            model.totalAccounts = totalHistoryCount != 0 ? totalHistoryCount : getalldata.Count();
                //        }
                //    }
                //}
                //else
                //{
                //    getalldata = (from data in getdata
                //                     orderby data.TransactionIdNo
                //                     select data).ToList();
                //    var getHistoryOfCompanyExpense = getalldata.Skip(skipRows).Take(takeRows).ToList();
                //    model.getexpensesofCompanies = getHistoryOfCompanyExpense;
                //    model.totalAccounts = getalldata.Count();
                //}

                //decimal totalCredit = 0, totalDebit = 0, totalBalance = 0, temp_debit = 0;
                //for (int index = 0; index < getalldata.Count; index++)
                //{
                //    if (getalldata.Count > 0 && getalldata != null && getalldata[index].Description != null)
                //    {
                //        if (getalldata[index].Type == "Credit")
                //        {
                //            totalCredit = totalCredit + Convert.ToDecimal(getalldata[index].Credit);
                //            model.TotalCredit = totalCredit.ToString();

                //            totalBalance = totalBalance + Convert.ToDecimal(getalldata[index].Credit);
                //            model.TotalBalance = totalBalance.ToString();

                //            getalldata[index].Balance = totalBalance;
                //        }
                //        if (getalldata[index].Type == "Debit")
                //        {
                //            totalDebit = totalDebit + Convert.ToDecimal(getalldata[index].Debit);
                //            model.TotalDebit = "-" + totalDebit.ToString();
                //            totalBalance = totalBalance - Convert.ToDecimal(getalldata[index].Debit);
                //            getalldata[index].Balance = totalBalance;
                //            model.TotalBalance = totalBalance.ToString();
                //            getalldata[index].Debit = "-" + getalldata[index].Debit;
                //        }
                //    }
                //}
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int AddCompanyExpensesAccountToTemp(SaveCompanyExpenses expense)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _accountId = 0;
                SqlCommand command = null;

                command = new SqlCommand("USP_Admin_SaveAccountManagementTransactionTemp", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", expense.UserId);
                command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                command.Parameters.AddWithValue("@TransactionType", expense.Type);
                command.Parameters.AddWithValue("@Description", expense.Task);
                command.Parameters.AddWithValue("@PaymentMode", "Cash");
                command.Parameters.AddWithValue("@Amount", expense.Amount);
                command.Parameters.AddWithValue("@Quantity", expense.Quantity);
                command.Parameters.AddWithValue("@TotalAmount", expense.TotalAmount);
                command.Parameters.AddWithValue("@Vat", expense.Vat);
                command.Parameters.AddWithValue("@VatAmount", expense.VatAmount);
                command.Parameters.AddWithValue("@GrandTotal", expense.Type == "Credit" ? expense.TotalAmount : expense.GrandTotal);
                command.Parameters.AddWithValue("@CreatedOn", expense.Date);
                command.Parameters.AddWithValue("@PaymentStatus", "Success");

                connection.Open();
                _accountId = (int)command.ExecuteScalar();
                connection.Close();

                return _accountId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public int SaveCompanyExpensesAccountFromTemp(List<int> ids)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _accountId = 0;
                SqlCommand command = null;
                if (ids.Count > 0)
                {
                    foreach (int id in ids)
                    {
                        command = new SqlCommand("USP_Admin_SaveAccountManagementTransactions", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        _accountId = (int)command.ExecuteScalar();
                        connection.Close();

                        command.Parameters.Clear();
                    }
                }
                return _accountId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public int RemoveCompanyExpensesAccountFromTemp(int id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _accountId = 0;
                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_RemoveAccountManagementStaging", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                    
                return _accountId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Add Company Expense Details                             -- Yashasvi TBC (25-11-2022)
        /// Use staging table to store all data we want to store into company account and from table store all transactions to main tables.              -- Yashasvi TBC (22-03-2024)
        /// </summary>
        /// <param name="expenses"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AddCompanyExpensesAccount(List<SaveCompanyExpenses> expenses)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            string dbName = connection.Database;
            try
            {
                int _returnId = 0;
                List<int> _accountIds = new List<int>();
                List<string> _runningStoredProcedure = new List<string>();
                _logger.LogInformation("STARTING TIME : "+DateTime.Now);
                SqlCommand command = null;

                string query = "USE[" + dbName + "] SELECT object_name(st.objectid) as ProcName, DB_NAME(st.dbid) AS [database] FROM " +
                    "sys.dm_exec_connections as qs CROSS APPLY sys.dm_exec_sql_text(qs.most_recent_sql_handle) st CROSS APPLY  sys.dm_exec_requests a " +
                    "WHERE object_name(st.objectid) is not null AND a.status = 'running'";
                command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["ProcName"] != DBNull.Value)
                        _runningStoredProcedure.Add(reader["ProcName"].ToString());
                }
                connection.Close();

                command.Parameters.Clear();

                foreach (var expense in expenses)
                {
                    command = new SqlCommand("USP_Admin_SaveAccountManagementTransactionTemp", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", expense.UserId);
                    command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                    command.Parameters.AddWithValue("@TransactionType", expense.Type);
                    command.Parameters.AddWithValue("@Description", expense.Task);
                    command.Parameters.AddWithValue("@PaymentMode", "Cash");
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Quantity", expense.Quantity);
                    command.Parameters.AddWithValue("@TotalAmount", expense.TotalAmount);
                    command.Parameters.AddWithValue("@Vat", expense.Vat);
                    command.Parameters.AddWithValue("@VatAmount", expense.VatAmount);
                    command.Parameters.AddWithValue("@GrandTotal", expense.Type == "Credit" ? expense.TotalAmount : expense.GrandTotal);
                    command.Parameters.AddWithValue("@CreatedOn", expense.Date);
                    command.Parameters.AddWithValue("@PaymentStatus", "Success");

                    connection.Open();
                    int _accountId = (int)command.ExecuteScalar();
                    _accountIds.Add(_accountId);
                    connection.Close();

                    command.Parameters.Clear();
                }

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveAccountManagementTransactions", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                foreach(int id in _accountIds)
                {
                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_CheckWhetherAccountManagementAdded", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                _logger.LogInformation("PROCESS ENDING TIME : "+DateTime.Now);
                //if (expense.Type == "Credit")
                //{
                //    command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                //    command.CommandType = CommandType.StoredProcedure;
                //    command.Parameters.AddWithValue("@UserId", expense.UserId);
                //    command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                //    command.Parameters.AddWithValue("@PortalAmount", expense.Amount);
                //    command.Parameters.AddWithValue("@CurrentAmount", expense.Amount);
                //    command.Parameters.AddWithValue("@PaymentMode", "Cash");
                //    command.Parameters.AddWithValue("@Description", expense.Task);
                //    command.Parameters.AddWithValue("@TotalAmount", expense.GrandTotal);
                //    command.Parameters.AddWithValue("@Quantity", expense.Quantity);
                //    command.Parameters.AddWithValue("@PaymentStatus", "Success");
                //    command.Parameters.AddWithValue("@CreatedOn", expense.Date);
                //    command.Parameters.AddWithValue("@Status", Operation.Insert);
                //    con.Open();
                //    int PaymentId = (int)command.ExecuteScalar();
                //    con.Close();
                //    command.Parameters.Clear();

                //    if(PaymentId > 0)
                //    {
                //        command = new SqlCommand("USP_Admin_SavePaymentReceipt", con);
                //        command.CommandType = CommandType.StoredProcedure;
                //        command.Parameters.AddWithValue("@UserId", expense.UserId);
                //        command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                //        command.Parameters.AddWithValue("@PaymentId", PaymentId);
                //        command.Parameters.AddWithValue("@CreatedOn", expense.Date);
                //        command.Parameters.AddWithValue("@OnAccount", expense.Task);
                //        con.Open();
                //        _returnId = (int)command.ExecuteScalar();
                //        con.Close();
                //        command.Parameters.Clear();

                //        command = new SqlCommand("USP_Admin_PortalBalanceOperation", con);
                //        command.CommandType = CommandType.StoredProcedure;
                //        command.Parameters.AddWithValue("@UserId", expense.UserId);
                //        command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                //        command.Parameters.AddWithValue("@PortalAmount", expense.GrandTotal);
                //        command.Parameters.AddWithValue("@IsCompletelyUsed", 0);
                //        command.Parameters.AddWithValue("@Status", Operation.Insert);
                //        con.Open();
                //        _returnId = (int)command.ExecuteScalar();
                //        con.Close();

                //        command.Parameters.Clear();

                //        command = new SqlCommand("USP_Admin_GetTransactionNumber", con);
                //        command.CommandType = CommandType.StoredProcedure;
                //        con.Open();
                //        string transactionId = (string)command.ExecuteScalar();
                //        con.Close();

                //        command.Parameters.Clear();
                //        command = new SqlCommand("USP_Admin_SaveAccountHistory", con);
                //        command.CommandType = CommandType.StoredProcedure;
                //        command.Parameters.AddWithValue("@TransactionId", expense.TransactionId != null && expense.TransactionId != "" ? expense.TransactionId : transactionId);
                //        command.Parameters.AddWithValue("@Description", expense.Task);
                //        command.Parameters.AddWithValue("@Amount", expense.Amount);
                //        command.Parameters.AddWithValue("@Quantity", expense.Quantity);
                //        command.Parameters.AddWithValue("@TotalAmount", expense.TotalAmount);
                //        command.Parameters.AddWithValue("@Vat", 0);
                //        command.Parameters.AddWithValue("@VatAmount", 0);
                //        command.Parameters.AddWithValue("@GrandTotal", expense.TotalAmount);
                //        command.Parameters.AddWithValue("@ExpenseType", "Credit");
                //        command.Parameters.AddWithValue("@PaymentMethod", "Cash");
                //        command.Parameters.AddWithValue("@TransactionDate", expense.Date);
                //        command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                //        command.Parameters.AddWithValue("@CreatedBy", expense.UserId);
                //        con.Open();
                //        command.ExecuteNonQuery();
                //        con.Close();
                //        command.Parameters.Clear();
                //    }
                //}
                //else
                //{
                //    command = new SqlCommand("USP_Admin_SaveAllCompanyExpenses", con);
                //    command.CommandType = CommandType.StoredProcedure;
                //    command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                //    command.Parameters.AddWithValue("@ExpensesTitle", expense.Task);
                //    command.Parameters.AddWithValue("@ExpenseAmount", expense.Amount);
                //    if (expense.TransactionId != "" && expense.TransactionId != null)
                //        command.Parameters.AddWithValue("@TransactionNumber", expense.TransactionId);
                //    command.Parameters.AddWithValue("@Quantity", expense.Quantity);
                //    command.Parameters.AddWithValue("@TotalAmount", expense.TotalAmount);
                //    command.Parameters.AddWithValue("@Vat", expense.Vat);
                //    command.Parameters.AddWithValue("@VatAmount", expense.VatAmount);
                //    command.Parameters.AddWithValue("@GrandTotal", expense.GrandTotal);
                //    command.Parameters.AddWithValue("@CreatedOnUtc", expense.Date);
                //    command.Parameters.AddWithValue("@UserId", expense.UserId);

                //    con.Open();
                //    _returnId = (int)command.ExecuteScalar();
                //    con.Close();

                //    command.Parameters.Clear();

                //    command = new SqlCommand("USP_Admin_GetTransactionNumber", con);
                //    command.CommandType = CommandType.StoredProcedure;
                //    con.Open();
                //    string transactionId = (string)command.ExecuteScalar();
                //    con.Close();

                //    command.Parameters.Clear();
                //    command = new SqlCommand("USP_Admin_SaveAccountHistory", con);
                //    command.CommandType = CommandType.StoredProcedure;
                //    command.Parameters.AddWithValue("@TransactionId", expense.TransactionId != null && expense.TransactionId != "" ? expense.TransactionId : transactionId);
                //    command.Parameters.AddWithValue("@Description", expense.Task);
                //    command.Parameters.AddWithValue("@Amount", expense.Amount);
                //    command.Parameters.AddWithValue("@Quantity", expense.Quantity);
                //    command.Parameters.AddWithValue("@TotalAmount", expense.TotalAmount);
                //    command.Parameters.AddWithValue("@Vat", expense.Vat);
                //    command.Parameters.AddWithValue("@VatAmount", expense.VatAmount);
                //    command.Parameters.AddWithValue("@GrandTotal", expense.GrandTotal);
                //    command.Parameters.AddWithValue("@ExpenseType", "Debit");
                //    command.Parameters.AddWithValue("@TransactionDate", expense.Date);
                //    command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                //    command.Parameters.AddWithValue("@CreatedBy", expense.UserId);
                //    con.Open();
                //    command.ExecuteNonQuery();
                //    con.Close();
                //    command.Parameters.Clear();
                //}
                //}

                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public string SaveDirectCompanyExpensesAccount(List<SaveCompanyExpenses> expenses)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            string dbName = connection.Database;
            try
            {
                int _returnId = 0;
                string expenseType = "";
                List<int> _accountIds = new List<int>();
                List<string> _runningStoredProcedure = new List<string>();
                _logger.LogInformation("STARTING TIME : " + DateTime.Now);
                SqlCommand command = null;

                string query = "USE[" + dbName + "] SELECT object_name(st.objectid) as ProcName, DB_NAME(st.dbid) AS [database] FROM " +
                    "sys.dm_exec_connections as qs CROSS APPLY sys.dm_exec_sql_text(qs.most_recent_sql_handle) st CROSS APPLY  sys.dm_exec_requests a " +
                    "WHERE object_name(st.objectid) is not null AND a.status = 'running'";
                command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["ProcName"] != DBNull.Value)
                        _runningStoredProcedure.Add(reader["ProcName"].ToString());
                }
                connection.Close();

                command.Parameters.Clear();

                foreach (var expense in expenses)
                {
                    expenseType = expense.Type;

                    command = new SqlCommand("USP_Admin_SaveAccountManagementTransactionTemp", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", expense.UserId);
                    command.Parameters.AddWithValue("@CompanyId", expense.CompanyId);
                    command.Parameters.AddWithValue("@TransactionType", expense.Type);
                    command.Parameters.AddWithValue("@Description", expense.Task);
                    command.Parameters.AddWithValue("@PaymentMode", "Cash");
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Quantity", expense.Quantity);
                    command.Parameters.AddWithValue("@TotalAmount", expense.TotalAmount);
                    command.Parameters.AddWithValue("@Vat", expense.Vat);
                    command.Parameters.AddWithValue("@VatAmount", expense.VatAmount);
                    command.Parameters.AddWithValue("@GrandTotal", expense.Type == "Credit" ? expense.TotalAmount : expense.GrandTotal);
                    command.Parameters.AddWithValue("@CreatedOn", expense.Date);
                    command.Parameters.AddWithValue("@PaymentStatus", "Success");

                    connection.Open();
                    int _accountId = (int)command.ExecuteScalar();
                    _accountIds.Add(_accountId);
                    connection.Close();

                    command.Parameters.Clear();
                }
                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveAccountManagementTransactions", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                foreach (int id in _accountIds)
                {
                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_CheckWhetherAccountManagementAdded", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                _logger.LogInformation("PROCESS ENDING TIME : " + DateTime.Now);

                string TransactionId = "";

                command = new SqlCommand("USP_Admin_GetLastAddedTransactionCode", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", _accountIds[0]);

                connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["TransactionId"] != DBNull.Value)
                        TransactionId = reader["TransactionId"].ToString();
                }
                connection.Close();

                return TransactionId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateRepeatedAccountEntries()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<RepeatedTransactionIdModel> reapatedTransactions = new List<RepeatedTransactionIdModel>();
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_GetRepeatedTransactionNumbers", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    RepeatedTransactionIdModel model = new RepeatedTransactionIdModel();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionType"] != DBNull.Value)
                        model.TransactionType = reader["TransactionType"].ToString();

                    reapatedTransactions.Add(model);
                }
                connection.Close();

                if (reapatedTransactions.Count > 0)
                {
                    foreach (var transaction in reapatedTransactions)
                    {
                        command.Parameters.Clear();
                        command = new SqlCommand("USP_Admin_UpdateReapatedTransactionIdNumber", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", transaction.Id);
                        command.Parameters.AddWithValue("@TransactionId", transaction.TransactionIdNo);
                        command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                return _returnId;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get History of All Company's Expenses from Passed From Date to Passed To Date                   -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetHistoryOfCompanyExpenses> GetHistoryOfAllCompanyExpenses(string FromDate, string ToDate)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetHistoryOfCompanyExpenses> getHistoryOfCompanyExpenses = new List<GetHistoryOfCompanyExpenses>();
                IList<GetHistoryOfCompanyExpenses> getdata = new List<GetHistoryOfCompanyExpenses>();
                List<GetHistoryOfCompanyExpenses> GetDebits = new List<GetHistoryOfCompanyExpenses>();
                SqlCommand command = new SqlCommand("USP_Admin_GetHistoryofExpenses", con);
                command.CommandType = CommandType.StoredProcedure;
                if (FromDate != null && ToDate != null)
                {
                    command.Parameters.AddWithValue("@FromDate", FromDate);
                    command.Parameters.AddWithValue("@ToDate", ToDate);
                }
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetHistoryOfCompanyExpenses getHistoryOfCompany = new GetHistoryOfCompanyExpenses();
                    getHistoryOfCompany.PaymentTransactionId = Convert.ToInt32(reader["PaymentTransactionId"]);
                    getHistoryOfCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    getHistoryOfCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    getHistoryOfCompany.CompanyName = reader["CompanyName"].ToString();
                    getHistoryOfCompany.PaymentCredit = reader["PaymentCredit"].ToString();
                    getHistoryOfCompany.CreatedOnUtc = reader["Date"].ToString();
                    getHistoryOfCompany.Description = reader["Description"].ToString();
                    getHistoryOfCompany.ExpensesAmount = reader["Amount"].ToString();
                    getHistoryOfCompany.TransactionId = reader["TransactionId"].ToString();
                    getHistoryOfCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    getHistoryOfCompany.Quantity = Convert.ToInt32(reader["Quantity"].ToString());
                    getHistoryOfCompany.Type = "Credit";

                    getdata.Add(getHistoryOfCompany);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    GetHistoryOfCompanyExpenses getHistoryOfCompany = new GetHistoryOfCompanyExpenses();

                    getHistoryOfCompany.Id = Convert.ToInt32(reader["Id"]);
                    getHistoryOfCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    getHistoryOfCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    getHistoryOfCompany.CompanyName = reader["CompanyName"].ToString();
                    getHistoryOfCompany.ExpensesTitle = reader["ExpensesTitle"].ToString();
                    getHistoryOfCompany.ExpensesAmount = reader["ExpensesAmount"].ToString();
                    getHistoryOfCompany.Quantity = Convert.ToInt32(reader["Quantity"]);
                    getHistoryOfCompany.TotalAmount = Convert.ToDouble(reader["TotalAmount"]);
                    getHistoryOfCompany.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    getHistoryOfCompany.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    getHistoryOfCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    getHistoryOfCompany.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    getHistoryOfCompany.Vat = reader["Vat"].ToString();
                    getHistoryOfCompany.TransactionId = reader["TransactionId"].ToString();
                    getHistoryOfCompany.VatAmount = reader["VatAmount"].ToString();
                    getHistoryOfCompany.GrandTotal = reader["GrandTotal"].ToString();
                    getHistoryOfCompany.Type = "Debit";

                    getdata.Add(getHistoryOfCompany);
                }
                con.Close();

                var getalldata = from data in getdata
                                 orderby data.TransactionIdNo
                                 select data;

                getHistoryOfCompanyExpenses = getalldata.ToList();
                return getHistoryOfCompanyExpenses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public GetAccountHistoryDataModel GetHistoryOfAllCompanyExpensesTest(int pageNumber, int fetchRows, string searchKey, string sortColumn, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                GetAccountHistoryDataModel model = new GetAccountHistoryDataModel();
                List<GetAccountHistoryModel> getHistoryOfCompanyExpenses = new List<GetAccountHistoryModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetHistoryofExpenses_Test", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", pageNumber);
                command.Parameters.AddWithValue("@RowsOfPage", fetchRows);
                command.Parameters.AddWithValue("@searchPrefix", searchKey);
                if (sortColumn != null)
                    command.Parameters.AddWithValue("@sortColumn", sortColumn);
                if (sortDirection != null)
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetAccountHistoryModel getHistoryOfCompany = new GetAccountHistoryModel();
                    getHistoryOfCompany.Id = Convert.ToInt32(reader["Id"]);
                    getHistoryOfCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    getHistoryOfCompany.TransactionId = Convert.ToString(reader["TransactionId"]);
                    getHistoryOfCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    getHistoryOfCompany.CompanyName = reader["CompanyName"].ToString();
                    getHistoryOfCompany.Description = reader["Description"].ToString();
                    getHistoryOfCompany.Amount = reader["Amount"].ToString();
                    getHistoryOfCompany.Quantity = Convert.ToInt32(reader["Quantity"]);
                    getHistoryOfCompany.TotalAmount = reader["TotalAmount"].ToString();
                    getHistoryOfCompany.Vat = reader["Vat"].ToString();
                    getHistoryOfCompany.VatAmount = reader["VatAmount"].ToString();
                    getHistoryOfCompany.GrandTotal = reader["GrandTotal"].ToString();
                    getHistoryOfCompany.Date = reader["Date"].ToString();
                    getHistoryOfCompany.CreatedOn = reader["CreatedOn"].ToString();
                    getHistoryOfCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    getHistoryOfCompany.TransactionType = reader["TransactionType"].ToString();

                    getHistoryOfCompanyExpenses.Add(getHistoryOfCompany);
                }
                reader.NextResult();

                while (reader.Read())
                {
                    totalHistoryCount = Convert.ToInt32(reader["TransactionCount"]);
                }
                con.Close();
                model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpenses;
                model.expenseCounts = totalHistoryCount;
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public GetAccountHistoryDataModel GetHistoryOfAllCompanyExpensesFilter(int skip, int take, string? searchBy, string? searchValue, string sortColumn, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                GetAccountHistoryDataModel model = new GetAccountHistoryDataModel();
                List<GetAccountHistoryModel> getHistoryOfCompanyExpenses = new List<GetAccountHistoryModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetHistoryofExpenses_Test_2", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", skip);
                command.Parameters.AddWithValue("@RowsOfPage", take);
                if (searchBy != null)
                {
                    if (searchBy == "Transaction Id")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "^[0-9]*$"))
                        {
                            command.Parameters.AddWithValue("@searchBy", "Transaction Id No");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@searchBy", searchBy);
                        }
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@searchBy", searchBy);
                    }
                }

                if (searchValue != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchValue);
                if (sortColumn != null)
                    command.Parameters.AddWithValue("@sortColumn", sortColumn);
                if (sortDirection != null)
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetAccountHistoryModel getHistoryOfCompany = new GetAccountHistoryModel();
                    getHistoryOfCompany.Id = Convert.ToInt32(reader["Id"]);
                    getHistoryOfCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    getHistoryOfCompany.TransactionId = Convert.ToString(reader["TransactionId"]);
                    getHistoryOfCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    getHistoryOfCompany.CompanyName = reader["CompanyName"].ToString();
                    getHistoryOfCompany.Description = reader["Description"].ToString();
                    getHistoryOfCompany.Amount = reader["Amount"].ToString();
                    getHistoryOfCompany.Quantity = Convert.ToInt32(reader["Quantity"]);
                    getHistoryOfCompany.TotalAmount = reader["TotalAmount"].ToString();
                    getHistoryOfCompany.Vat = reader["Vat"].ToString();
                    getHistoryOfCompany.VatAmount = reader["VatAmount"].ToString();
                    getHistoryOfCompany.GrandTotal = reader["GrandTotal"].ToString();
                    getHistoryOfCompany.Date = reader["Date"].ToString();
                    getHistoryOfCompany.CreatedOn = reader["CreatedOn"].ToString();
                    getHistoryOfCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    getHistoryOfCompany.TransactionType = reader["TransactionType"].ToString();

                    getHistoryOfCompanyExpenses.Add(getHistoryOfCompany);
                }
                reader.NextResult();

                while (reader.Read())
                {
                    totalHistoryCount = Convert.ToInt32(reader["TransactionCount"]);
                }

                con.Close();

                model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpenses;
                model.expenseCounts = totalHistoryCount;
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Get Suggestions for Company Expense Title / Description                     -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetTaskList(string prefix)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> _returnId = new List<string>();
                if (prefix != null && prefix != "")
                {
                    SqlCommand command = new SqlCommand("USP_Admin_GetSuggestionForPortalExpense", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@prefix", prefix);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        _returnId.Add(reader["ExpensesTitle"].ToString());
                    }
                    con.Close();
                }
                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get Complete Expense Details By Id                              -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public UpdateCompanyExpenses GetExpenseDetails(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                UpdateCompanyExpenses model = new UpdateCompanyExpenses();

                SqlCommand command = new SqlCommand("USP_Admin_GetExpenseDetailsById", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    model.Task = reader["ExpensesTitle"].ToString();
                    model.Amount = reader["ExpensesAmount"].ToString();
                    model.Quantity = Convert.ToInt32(reader["Quantity"]);
                    model.TotalAmount = reader["TotalAmount"].ToString();
                    model.Vat = reader["Vat"].ToString();
                    model.TransactionId = reader["TransactionId"].ToString();
                    model.VatAmount = reader["VatAmount"].ToString();
                    model.GrandTotal = reader["GrandTotal"].ToString();
                    model.CreatedOnUtc = reader["CreatedOnUtc"].ToString();


                    var date = Convert.ToDateTime(model.CreatedOnUtc);
                    var dateString = date.ToString("yyyy-MM-dd");
                    model.CreatedOnUtc = dateString;

                }
                con.Close();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public ResponseModel GetExpenseModificationDetails(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ResponseModel model = new ResponseModel();

                SqlCommand command = new SqlCommand("USP_Admin_GetExpenseModificationDetailsById", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Username"] != DBNull.Value)
                        model.Username = reader["Username"].ToString();
                    if (reader["ModifyTime"] != DBNull.Value)
                        model.ModifyTime = reader["ModifyTime"].ToString();
                    if (reader["ResponseData"] != DBNull.Value)
                        model.ResponseData = Convert.ToInt32(reader["ResponseData"]);
                }
                connection.Close();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public ResponseModel GetTransactionModificationDetails(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ResponseModel model = new ResponseModel();

                SqlCommand command = new SqlCommand("USP_Admin_GetTransactionModificationDetailsById", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Username"] != DBNull.Value)
                        model.Username = reader["Username"].ToString();
                    if (reader["ModifyTime"] != DBNull.Value)
                        model.ModifyTime = reader["ModifyTime"].ToString();
                    if (reader["ResponseData"] != DBNull.Value)
                        model.ResponseData = Convert.ToInt32(reader["ResponseData"]);
                }
                connection.Close();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        ///  Update Expense Detail                                          -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string UpdateExpenseDetail(UpdateCompanyExpenses model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                string _returnMessage = "";
                ResponseModel modelData = new ResponseModel();
                SqlCommand command = new SqlCommand("USP_Admin_UpdateCompanyExpense", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                command.Parameters.AddWithValue("@ExpenseTitle", model.Task);
                command.Parameters.AddWithValue("@ExpenseAmount", model.Amount);
                command.Parameters.AddWithValue("@TotalAmount", model.TotalAmount);
                command.Parameters.AddWithValue("@Quantity", model.Quantity);
                command.Parameters.AddWithValue("@Vat", model.Vat);
                command.Parameters.AddWithValue("@VatAmount", model.VatAmount);
                command.Parameters.AddWithValue("@GrandTotal", model.GrandTotal);
                command.Parameters.AddWithValue("@CreatedOn", model.CreatedOnUtc);
                command.Parameters.AddWithValue("@UserId", model.UserId);
                con.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Username"] != DBNull.Value)
                        modelData.Username = reader["Username"].ToString();
                    if (reader["ModifyTime"] != DBNull.Value)
                        modelData.ModifyTime = reader["ModifyTime"].ToString();
                    if (reader["ResponseData"] != DBNull.Value)
                        modelData.ResponseData = Convert.ToInt32(reader["ResponseData"]);
                }
                con.Close();
                if (modelData.ResponseData > 0)
                {
                    _returnMessage = "Selected Company Expense Updated Successfully.";

                    command.Parameters.Clear();
                    command = new SqlCommand("USP_Admin_SaveAccountHistory", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TransactionId", model.TransactionId);
                    command.Parameters.AddWithValue("@Description", model.Task);
                    command.Parameters.AddWithValue("@Amount", model.Amount);
                    command.Parameters.AddWithValue("@Quantity", model.Quantity);
                    command.Parameters.AddWithValue("@TotalAmount", model.TotalAmount);
                    command.Parameters.AddWithValue("@Vat", model.Vat);
                    command.Parameters.AddWithValue("@VatAmount", model.VatAmount);
                    command.Parameters.AddWithValue("@GrandTotal", model.GrandTotal);
                    command.Parameters.AddWithValue("@ExpenseType", "Debit");
                    command.Parameters.AddWithValue("@TransactionDate", model.CreatedOnUtc);
                    command.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                    command.Parameters.AddWithValue("@CreatedBy", model.UserId);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                    command.Parameters.Clear();
                    if (model.formFile != null)
                    {
                        string _getName = model.formFile.FileName;
                        var Name = model.formFile.FileName.Split(".");
                        string FileName = Name[0];
                        var extn = Path.GetExtension(_getName);
                        Byte[] bytes = null;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            model.formFile.OpenReadStream().CopyTo(ms);
                            bytes = ms.ToArray();
                        }

                        command = new SqlCommand("USP_Admin_ExpesesRecieptOperation", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Status", Operation.Insert);
                        command.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                        command.Parameters.AddWithValue("@DataBinary", bytes);
                        command.Parameters.AddWithValue("@FileName", FileName);
                        command.Parameters.AddWithValue("@Extension", extn);
                        command.Parameters.AddWithValue("@ReceiptId", model.Id);
                        command.Parameters.AddWithValue("@UserId", model.UserId);
                        con.Open();
                        _returnId = (int)command.ExecuteScalar();
                        con.Close();
                    }
                }
                else
                {
                    _returnMessage = "Any operation does not perform on selected Company Expense (<b>" + model.TransactionId + "</b>) because it is already modified by <b>" + modelData.Username + "</b> at " + modelData.ModifyTime;
                }
                return _returnMessage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get Detail of Company Payment Details                           -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public PaymentTransaction GetDetailsOfPayment(int Id, int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                PaymentTransaction paymentTransaction = new PaymentTransaction();
                SqlCommand command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    paymentTransaction.Id = Convert.ToInt32(reader["Id"]);
                    paymentTransaction.UserId = Convert.ToInt32(reader["UserId"]);
                    paymentTransaction.Username = reader["Username"].ToString();
                    paymentTransaction.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    paymentTransaction.CompanyName = reader["CompanyName"].ToString();
                    paymentTransaction.Amount = reader["Amount"].ToString();
                    paymentTransaction.Quantity = Convert.ToInt32(reader["Quantity"]);
                    paymentTransaction.TotalAmount = reader["TotalAmount"].ToString();
                    paymentTransaction.TransactionId = reader["TransactionId"].ToString();
                    paymentTransaction.PaymentMode = reader["PaymentMode"].ToString();
                    paymentTransaction.Description = reader["Description"].ToString();
                    paymentTransaction.PaymentStatus = reader["PaymentStatus"].ToString();
                    paymentTransaction.TransectionDate = reader["TransectionDate"].ToString();
                    paymentTransaction.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    paymentTransaction.ModifyOnUtc = reader["ModifyOnUtc"].ToString();

                    var date = Convert.ToDateTime(paymentTransaction.TransectionDate);
                    var dateString = date.ToString("yyyy-MM-dd");
                    paymentTransaction.TransectionDate = dateString;
                    //paymentTransaction.TransectionDate = date;

                }
                con.Close();

                return paymentTransaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Update Payment Details of Any Company                               -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string UpdatePaymentDetails(PaymentTransaction paymentTransaction)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                string _returnMessage = "";
                ResponseModel model = new ResponseModel();

                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", paymentTransaction.UserId);
                command.Parameters.AddWithValue("@CompanyId", paymentTransaction.CompanyId);
                command.Parameters.AddWithValue("@PortalAmount", paymentTransaction.Amount);
                command.Parameters.AddWithValue("@Quantity", paymentTransaction.Quantity);
                command.Parameters.AddWithValue("@TotalAmount", paymentTransaction.TotalAmount == null ? paymentTransaction.Amount : paymentTransaction.TotalAmount);
                if (paymentTransaction.PaymentMode != null)
                    command.Parameters.AddWithValue("@PaymentMode", paymentTransaction.PaymentMode);
                command.Parameters.AddWithValue("@Description", paymentTransaction.Description);
                command.Parameters.AddWithValue("@PaymentStatus", "Success");
                command.Parameters.AddWithValue("@CreatedOn", paymentTransaction.CreatedOnUtc);
                command.Parameters.AddWithValue("@Id", paymentTransaction.Id);
                command.Parameters.AddWithValue("@Through", paymentTransaction.Through);
                command.Parameters.AddWithValue("@PaymentReceiptId", paymentTransaction.PaymentReceiptId);
                command.Parameters.AddWithValue("@CreatedOnUTC", paymentTransaction.TransectionDate != null ? paymentTransaction.TransectionDate : paymentTransaction.CreatedOnUtc);
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Username"] != DBNull.Value)
                        model.Username = reader["Username"].ToString();
                    if (reader["ModifyTime"] != DBNull.Value)
                        model.ModifyTime = reader["ModifyTime"].ToString();
                    if (reader["ResponseData"] != DBNull.Value)
                        model.ResponseData = Convert.ToInt32(reader["ResponseData"]);
                }
                con.Close();

                if (model.ResponseData > 0)
                {
                    _returnMessage = "Selected Payment Transaction Updated Successfully.";

                    command = new SqlCommand("USP_Admin_SaveAccountHistory", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TransactionId", paymentTransaction.TransactionId);
                    command.Parameters.AddWithValue("@Description", paymentTransaction.Description);
                    command.Parameters.AddWithValue("@Amount", paymentTransaction.Amount);
                    command.Parameters.AddWithValue("@Quantity", paymentTransaction.Quantity);
                    command.Parameters.AddWithValue("@TotalAmount", paymentTransaction.TotalAmount);
                    command.Parameters.AddWithValue("@Vat", 0);
                    command.Parameters.AddWithValue("@VatAmount", 0);
                    command.Parameters.AddWithValue("@GrandTotal", paymentTransaction.TotalAmount);
                    command.Parameters.AddWithValue("@ExpenseType", "Credit");
                    command.Parameters.AddWithValue("@TransactionDate", paymentTransaction.TransectionDate != null ? paymentTransaction.TransectionDate : paymentTransaction.CreatedOnUtc);
                    command.Parameters.AddWithValue("@CompanyId", paymentTransaction.CompanyId);
                    command.Parameters.AddWithValue("@CreatedBy", paymentTransaction.UserId);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                    command.Parameters.Clear();
                }
                else
                {
                    _returnMessage = "Any operation does not perform on selected Payment Transaction (<b>" + paymentTransaction.TransactionId + "</b>) because it is already modified by <b>" + model.Username + "</b> at " + model.ModifyTime;
                }

                return _returnMessage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete Payment Transaction of Any Company                       -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CompanyId"></param>
        /// <param name="Amount"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string DeletePaymentTransaction(int Id, int CompanyId, string Amount, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnMessage = "";
                SqlCommand command = null;
                ResponseModel model = new ResponseModel();
                command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@TotalAmount", Amount);
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Username"] != DBNull.Value)
                        model.Username = reader["Username"].ToString();
                    if (reader["ModifyTime"] != DBNull.Value)
                        model.ModifyTime = reader["ModifyTime"].ToString();
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["ResponseData"] != DBNull.Value)
                        model.ResponseData = Convert.ToInt32(reader["ResponseData"]);
                }
                con.Close();
                if (model.ResponseData > 0)
                {
                    returnMessage = "Selected Payment Transaction Deleted Successfully.";
                }
                else
                {
                    returnMessage = "Any operation does not perform on selected Payment Transaction (<b>" + model.TransactionId + "</b>) because it is already modified by <b>" + model.Username + "</b> at " + model.ModifyTime;
                }
                return returnMessage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get All Company's Payment Transaction Receipt                   -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetPaymentTransactionReceipt> GetAllPaymentTransactionReceipt()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetPaymentTransactionReceipt> transactionReceipts = new List<GetPaymentTransactionReceipt>();
                SqlCommand command = new SqlCommand("USP_Admin_GetPaymentReceipt", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
                    if (reader["Id"] != DBNull.Value)
                        transactionReceipt.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        transactionReceipt.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["PaymentId"] != DBNull.Value)
                        transactionReceipt.PaymentId = Convert.ToInt32(reader["PaymentId"]);
                    if (reader["CreatedBy"] != DBNull.Value)
                        transactionReceipt.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    if (reader["Amount"] != DBNull.Value)
                        transactionReceipt.Amount = reader["Amount"].ToString();
                    if (reader["Through"] != DBNull.Value)
                        transactionReceipt.Through = reader["Through"].ToString();
                    if (reader["OnAccount"] != DBNull.Value)
                        transactionReceipt.OnAccount = reader["OnAccount"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        transactionReceipt.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        transactionReceipt.CreatedOn = reader["CreatedOn"].ToString();
                    if (reader["SerialNumber"] != DBNull.Value)
                        transactionReceipt.SerialNumber = reader["SerialNumber"].ToString();
                    transactionReceipts.Add(transactionReceipt);
                }
                con.Close();

                return transactionReceipts;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get Payment Transaction Receipt History of All Companies                        -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetPaymentTransactionReceipt> GetPaymentTransactionReceiptHistory()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetPaymentTransactionReceipt> transactionReceipts = new List<GetPaymentTransactionReceipt>();
                SqlCommand command = new SqlCommand("USP_Admin_GetPaymentReceipt", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 5);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
                    if (reader["Id"] != DBNull.Value)
                        transactionReceipt.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        transactionReceipt.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["PaymentId"] != DBNull.Value)
                        transactionReceipt.PaymentId = Convert.ToInt32(reader["PaymentId"]);
                    if (reader["CreatedBy"] != DBNull.Value)
                        transactionReceipt.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    if (reader["Amount"] != DBNull.Value)
                        transactionReceipt.Amount = reader["Amount"].ToString();
                    if (reader["Through"] != DBNull.Value)
                        transactionReceipt.Through = reader["Through"].ToString();
                    if (reader["OnAccount"] != DBNull.Value)
                        transactionReceipt.OnAccount = reader["OnAccount"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        transactionReceipt.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        transactionReceipt.CreatedOn = reader["CreatedOn"].ToString();
                    if (reader["SerialNumber"] != DBNull.Value)
                        transactionReceipt.SerialNumber = reader["SerialNumber"].ToString();
                    transactionReceipts.Add(transactionReceipt);
                }
                con.Close();

                return transactionReceipts;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get Payment Transaction Receipt's Complete Details for Download                         -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetPaymentTransactionReceipt GetPaymentTransactionReceiptDetails(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
                SqlCommand command = new SqlCommand("USP_Admin_GetPaymentReceipt", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@Id", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        transactionReceipt.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        transactionReceipt.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["PaymentId"] != DBNull.Value)
                        transactionReceipt.PaymentId = Convert.ToInt32(reader["PaymentId"]);
                    if (reader["CreatedBy"] != DBNull.Value)
                        transactionReceipt.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    if (reader["Amount"] != DBNull.Value)
                        transactionReceipt.Amount = reader["Amount"].ToString();
                    if (reader["Through"] != DBNull.Value)
                        transactionReceipt.Through = reader["Through"].ToString();
                    if (reader["OnAccount"] != DBNull.Value)
                        transactionReceipt.OnAccount = reader["OnAccount"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        transactionReceipt.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        transactionReceipt.CreatedOn = reader["CreatedOn"].ToString();
                    if (reader["SerialNumber"] != DBNull.Value)
                        transactionReceipt.SerialNumber = reader["SerialNumber"].ToString();
                }
                con.Close();

                return transactionReceipt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get Payment Transaction Complete Detail for Edit                        -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetPaymentTransactionReceipt GetPaymentTransactionReceiptDetailsForEdit(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
                SqlCommand command = new SqlCommand("USP_Admin_GetPaymentReceipt", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@Id", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        transactionReceipt.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        transactionReceipt.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["PaymentId"] != DBNull.Value)
                        transactionReceipt.PaymentId = Convert.ToInt32(reader["PaymentId"]);
                    if (reader["CreatedBy"] != DBNull.Value)
                        transactionReceipt.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    if (reader["Amount"] != DBNull.Value)
                        transactionReceipt.Amount = reader["Amount"].ToString();
                    if (reader["Quantity"] != DBNull.Value)
                        transactionReceipt.Quantity = Convert.ToInt32(reader["Quantity"].ToString());
                    if (reader["TotalAmount"] != DBNull.Value)
                        transactionReceipt.TotalAmount = reader["TotalAmount"].ToString();
                    if (reader["Through"] != DBNull.Value)
                        transactionReceipt.Through = reader["Through"].ToString();
                    if (reader["OnAccount"] != DBNull.Value)
                        transactionReceipt.OnAccount = reader["OnAccount"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        transactionReceipt.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        transactionReceipt.CreatedOn = reader["CreatedOn"].ToString();
                    if (reader["SerialNumber"] != DBNull.Value)
                        transactionReceipt.SerialNumber = reader["SerialNumber"].ToString();
                }
                con.Close();

                return transactionReceipt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get Company's Account History                       -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetAccountHistory> GetAccountHistoryDetails(int? CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAccountHistory> transaction = new List<GetAccountHistory>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAccountHistoryByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                if (CompanyId != null)
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetAccountHistory transactionReceipt = new GetAccountHistory();
                    if (reader["Id"] != DBNull.Value)
                        transactionReceipt.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        transactionReceipt.TransactionId = reader["TransactionId"].ToString();
                    if (reader["ExpenseType"] != DBNull.Value)
                    {
                        if (reader["Description"] != DBNull.Value)
                        {
                            if (reader["ExpenseType"].ToString() == "Credit")
                            {
                                transactionReceipt.Description = "Credit: " + " Received - # " + reader["TotalAmount"].ToString() + " ( Description : " + reader["Description"].ToString() + ")";
                            }
                            else
                            {
                                transactionReceipt.Description = "Debit: " + reader["Description"].ToString() + "#";
                                if (reader["Vat"] != DBNull.Value && reader["VatAmount"] != DBNull.Value && reader["Vat"].ToString() != "0")
                                {
                                    transactionReceipt.Description += " Incl. VAT@" + reader["Vat"].ToString() + "%" + " (" + Convert.ToDecimal(reader["GrandTotal"].ToString()) + " AED)";
                                }
                                else
                                {
                                    transactionReceipt.Description += " (" + Convert.ToDecimal(reader["GrandTotal"].ToString()) + " AED)";
                                }
                            }
                        }
                    }

                    if (reader["Amount"] != DBNull.Value)
                        transactionReceipt.Amount = reader["Amount"].ToString();
                    if (reader["Quantity"] != DBNull.Value)
                        transactionReceipt.Quantity = Convert.ToInt32(reader["Quantity"]);
                    if (reader["Vat"] != DBNull.Value)
                        transactionReceipt.Vat = reader["Vat"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        transactionReceipt.VatAmount = reader["VatAmount"].ToString();
                    if (reader["TotalAmount"] != DBNull.Value)
                        transactionReceipt.TotalAmount = reader["TotalAmount"].ToString();
                    if (reader["GrandTotal"] != DBNull.Value)
                        transactionReceipt.GrandTotal = reader["GrandTotal"].ToString();
                    if (reader["ExpenseType"] != DBNull.Value)
                        transactionReceipt.ExpenseType = reader["ExpenseType"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        transactionReceipt.TransactionDate = reader["TransactionDate"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        transactionReceipt.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Username"] != DBNull.Value)
                        transactionReceipt.Username = reader["Username"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        transactionReceipt.CreatedOn = reader["CreatedOn"].ToString();
                    transaction.Add(transactionReceipt);
                }
                con.Close();

                return transaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get All Company's Payment Transaction Receipt                   -- Yashasvi TBC (24-12-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GetPaymentReceiptWithPaginationModel> GetAllPaymentTransactionReceiptWithPagination(int page, int pageSize, string searchBy, string searchString, string sortBy, string sortDirection, int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetPaymentReceiptWithPaginationModel model = new GetPaymentReceiptWithPaginationModel();
                int totalPaymentReceipts = 0;
                List<GetPaymentTransactionReceipt> transactionReceipts = new List<GetPaymentTransactionReceipt>();

                SqlCommand command = new SqlCommand("USP_Admin_GetPaymentReceiptWithPagination", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
                    if (reader["Id"] != DBNull.Value)
                        transactionReceipt.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        transactionReceipt.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["PaymentId"] != DBNull.Value)
                        transactionReceipt.PaymentId = Convert.ToInt32(reader["PaymentId"]);
                    if (reader["CreatedBy"] != DBNull.Value)
                        transactionReceipt.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    if (reader["Amount"] != DBNull.Value)
                        transactionReceipt.Amount = reader["Amount"].ToString();
                    if (reader["Through"] != DBNull.Value)
                        transactionReceipt.Through = reader["Through"].ToString();
                    if (reader["OnAccount"] != DBNull.Value)
                        transactionReceipt.OnAccount = reader["OnAccount"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        transactionReceipt.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        transactionReceipt.CreatedOn = reader["CreatedOn"].ToString();
                    if (reader["SerialNumber"] != DBNull.Value)
                        transactionReceipt.SerialNumber = reader["SerialNumber"].ToString();
                    transactionReceipts.Add(transactionReceipt);
                }
                con.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetPaymentReceiptWithPaginationCount", con);
                command.CommandType = CommandType.StoredProcedure;
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);

                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalPaymentReceipts++;
                }
                con.Close();
                model.totalPaymentReceipts = totalPaymentReceipts;
                model.getPaymentTransactionReceipts = transactionReceipts;
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Get All Company's Payment Transaction Receipt                   -- Yashasvi TBC (24-12-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GetPaymentReceiptWithPaginationModel> GetAllPaymentTransactionReceiptHistoryWithPagination(int page, int pageSize, string searchBy, string searchString, string sortBy, string sortDirection, int count)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetPaymentReceiptWithPaginationModel model = new GetPaymentReceiptWithPaginationModel();
                int totalPaymentReceipts = 0;
                List<GetPaymentTransactionReceipt> transactionReceipts = new List<GetPaymentTransactionReceipt>();
                SqlCommand command = new SqlCommand("USP_Admin_GetPaymentReceiptHistoryWithPagination", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetPaymentTransactionReceipt transactionReceipt = new GetPaymentTransactionReceipt();
                    if (reader["Id"] != DBNull.Value)
                        transactionReceipt.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        transactionReceipt.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["PaymentId"] != DBNull.Value)
                        transactionReceipt.PaymentId = Convert.ToInt32(reader["PaymentId"]);
                    if (reader["CreatedBy"] != DBNull.Value)
                        transactionReceipt.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    if (reader["Amount"] != DBNull.Value)
                        transactionReceipt.Amount = reader["Amount"].ToString();
                    if (reader["Through"] != DBNull.Value)
                        transactionReceipt.Through = reader["Through"].ToString();
                    if (reader["OnAccount"] != DBNull.Value)
                        transactionReceipt.OnAccount = reader["OnAccount"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        transactionReceipt.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CreatedOn"] != DBNull.Value)
                        transactionReceipt.CreatedOn = reader["CreatedOn"].ToString();
                    if (reader["SerialNumber"] != DBNull.Value)
                        transactionReceipt.SerialNumber = reader["SerialNumber"].ToString();
                    transactionReceipts.Add(transactionReceipt);
                }
                con.Close();


                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetPaymentReceiptHistoryCount", con);
                command.CommandType = CommandType.StoredProcedure;
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                con.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalPaymentReceipts++;
                }
                con.Close();

                model.totalPaymentReceipts = totalPaymentReceipts;
                model.getPaymentTransactionReceipts = transactionReceipts;
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public async Task<GetTemporaryAccountManagementPaginationModel> GetTemporaryAccountManagementPagination(int page, int pageSize, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalAccountCount = 0;
                GetTemporaryAccountManagementPaginationModel getTemporaryAccountManagement = new GetTemporaryAccountManagementPaginationModel();
                List<GetTemporaryAccountManagementListModel> getTemporaryAccountManagementList = new List<GetTemporaryAccountManagementListModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetTemporaryAccountManagement_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                command.Parameters.AddWithValue("@searchPrefix", searchString);
                command.Parameters.AddWithValue("@sortColumn", sortBy);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetTemporaryAccountManagementListModel model = new GetTemporaryAccountManagementListModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["Amount"] != DBNull.Value)
                        model.Amount = Convert.ToDouble(reader["Amount"]);
                    if (reader["Quantity"] != DBNull.Value)
                        model.Quantity = Convert.ToInt32(reader["Quantity"]);
                    if (reader["TotalAmount"] != DBNull.Value)
                        model.TotalAmount = Convert.ToDouble(reader["TotalAmount"]);
                    if (reader["Vat"] != DBNull.Value)
                        model.Vat = Convert.ToDouble(reader["Vat"]);
                    if (reader["VatAmount"] != DBNull.Value)
                        model.VatAmount = Convert.ToDouble(reader["VatAmount"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]);
                    if (reader["EntryType"] != DBNull.Value)
                        model.EntryType = reader["EntryType"].ToString();
                    if (reader["PaymentMode"] != DBNull.Value)
                        model.PaymentMode = reader["PaymentMode"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = reader["TransactionDate"].ToString();
                    if (reader["CreatedBy"] != DBNull.Value)
                        model.CreatedBy = reader["CreatedBy"].ToString();
                    if (reader["CreatedOnUtc"] != DBNull.Value)
                        model.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    if (reader["IsApproved"] != DBNull.Value)
                        model.IsApproved = Convert.ToBoolean(reader["IsApproved"]);
                    if (reader["ApprovedBy"] != DBNull.Value)
                        model.ApprovedBy = reader["ApprovedBy"].ToString();
                    if (reader["ApprovedOnUtc"] != DBNull.Value)
                        model.ApprovedOnUtc = reader["ApprovedOnUtc"].ToString();
                    if (reader["IsRejected"] != DBNull.Value)
                        model.IsRejected = Convert.ToBoolean(reader["IsRejected"]);
                    if (reader["RejectedBy"] != DBNull.Value)
                        model.RejectedBy = reader["RejectedBy"].ToString();
                    if (reader["RejectedOnUtc"] != DBNull.Value)
                        model.RejectedOnUtc = reader["RejectedOnUtc"].ToString();

                    getTemporaryAccountManagementList.Add(model);
                }
                connection.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_GetTemporaryAccountManagement_Pagination_Count", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@searchPrefix", searchString);

                connection.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["TotalAccountManagement"] != DBNull.Value)
                        totalAccountCount += Convert.ToInt32(reader["TotalAccountManagement"]);
                }
                connection.Close();

                getTemporaryAccountManagement.totalAccountCount = totalAccountCount;
                getTemporaryAccountManagement.getTemporaryAccounts = getTemporaryAccountManagementList;
                return getTemporaryAccountManagement;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<int> AddTemporaryAccountExpenses(List<SaveTemporaryAccountExpenseModel> expenses)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = null;
                if (expenses.Count > 0)
                {
                    for (int index = 0; index < expenses.Count; index++)
                    {
                        command = new SqlCommand("USP_Admin_Insert_Update_Temp_AccountManagement", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CompanyId", expenses[index].CompanyId);
                        command.Parameters.AddWithValue("@Description", expenses[index].Task);
                        command.Parameters.AddWithValue("@Amount", expenses[index].Amount);
                        command.Parameters.AddWithValue("@Quantity", expenses[index].Quantity);
                        command.Parameters.AddWithValue("@TotalAmount", expenses[index].TotalAmount);
                        command.Parameters.AddWithValue("@Vat", expenses[index].Vat);
                        command.Parameters.AddWithValue("@VatAmount", expenses[index].VatAmount);
                        command.Parameters.AddWithValue("@GrandTotal", expenses[index].GrandTotal);
                        command.Parameters.AddWithValue("@EntryType", expenses[index].Type);
                        command.Parameters.AddWithValue("@PaymentMode", expenses[index].PaymentMode);
                        command.Parameters.AddWithValue("@TransactionDate", expenses[index].Date);
                        command.Parameters.AddWithValue("@CreatedBy", expenses[index].CreatedBy);

                        connection.Open();
                        _returnId = (int)await command.ExecuteScalarAsync();
                        connection.Close();

                        command.Parameters.Clear();

                        command = new SqlCommand("USP_Admin_SaveTemporaryAccountManagementLog", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", _returnId);
                        command.Parameters.AddWithValue("@CreatedBy", expenses[index].CreatedBy);
                        connection.Open();
                        await command.ExecuteNonQueryAsync();
                        connection.Close();

                        command.Parameters.Clear();
                    }
                }
                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<GetTemporaryAccountDetailByIdModel> GetTemporaryAccountDetailById(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetTemporaryAccountDetailByIdModel model = new GetTemporaryAccountDetailByIdModel();

                SqlCommand command = new SqlCommand("USP_Admin_GetTemporaryAccountDetailsById", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["Amount"] != DBNull.Value)
                        model.Amount = Convert.ToDouble(reader["Amount"]);
                    if (reader["Quantity"] != DBNull.Value)
                        model.Quantity = Convert.ToInt32(reader["Quantity"]);
                    if (reader["TotalAmount"] != DBNull.Value)
                        model.TotalAmount = Convert.ToDouble(reader["TotalAmount"]);
                    if (reader["Vat"] != DBNull.Value)
                        model.Vat = Convert.ToDouble(reader["Vat"]);
                    if (reader["VatAmount"] != DBNull.Value)
                        model.VatAmount = Convert.ToDouble(reader["VatAmount"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]);
                    if (reader["EntryType"] != DBNull.Value)
                        model.EntryType = reader["EntryType"].ToString();
                    if (reader["PaymentMode"] != DBNull.Value)
                        model.PaymentMode = reader["PaymentMode"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = reader["TransactionDate"].ToString();
                }

                connection.Close();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<int> UpdateTemporaryAccountExpenses(GetTemporaryAccountDetailByIdModel expenses)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_Insert_Update_Temp_AccountManagement", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", expenses.Id);
                command.Parameters.AddWithValue("@CompanyId", expenses.CompanyId);
                command.Parameters.AddWithValue("@Description", expenses.Description);
                command.Parameters.AddWithValue("@Amount", expenses.Amount);
                command.Parameters.AddWithValue("@Quantity", expenses.Quantity);
                command.Parameters.AddWithValue("@TotalAmount", expenses.TotalAmount);
                command.Parameters.AddWithValue("@Vat", expenses.Vat);
                command.Parameters.AddWithValue("@VatAmount", expenses.VatAmount);
                command.Parameters.AddWithValue("@GrandTotal", expenses.GrandTotal);
                command.Parameters.AddWithValue("@EntryType", expenses.EntryType);
                command.Parameters.AddWithValue("@PaymentMode", expenses.PaymentMode);
                command.Parameters.AddWithValue("@TransactionDate", expenses.TransactionDate);
                command.Parameters.AddWithValue("@CreatedBy", expenses.ModifyBy);

                connection.Open();
                _returnId = (int)await command.ExecuteScalarAsync();
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveTemporaryAccountManagementLog", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", _returnId);
                command.Parameters.AddWithValue("@ModifyBy", expenses.ModifyBy);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<int> DeleteTemporaryAccount(int Id, int UserId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_DeleteTemporaryAccount", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@UserId", UserId);

                connection.Open();
                _returnId = (int)await command.ExecuteScalarAsync();
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveTemporaryAccountManagementLog", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", _returnId);
                command.Parameters.AddWithValue("@ModifyBy", UserId);
                command.Parameters.AddWithValue("@IsDelete", 1);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<int> ApproveTemporaryAccount(int Id, int UserId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ApproveTemporaryAccountManagement", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@ApprovedBy", UserId);

                connection.Open();
                _returnId = (int)await command.ExecuteScalarAsync();
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveTemporaryAccountManagementLog", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@ApprovedBy", UserId);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<int> RejectTemporaryAccount(int Id, int UserId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RejectTemporaryAccountManagement", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@RejectBy", UserId);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_SaveTemporaryAccountManagementLog", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@RejectedBy", UserId);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return _returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<GetTemporaryAccountManagementLogPaginationModel> GetTemporaryAccountManagementLogPagination(int page, int pageSize, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int index = 0;
                if (page == 0)
                    index = 1;
                else
                    index = page + index;
                GetTemporaryAccountManagementLogPaginationModel model = new GetTemporaryAccountManagementLogPaginationModel();
                List<GetTemporaryAccountManagementLogModel> TempLogs = new List<GetTemporaryAccountManagementLogModel>();
                List<GetTemporaryAccountManagementLogModel> logs = new List<GetTemporaryAccountManagementLogModel>();
                int totalLogs = 0;

                SqlCommand command = new SqlCommand("USP_Admin_GetTemporaryAccountManagementLog", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    string message = "";
                    GetTemporaryAccountManagementLogModel log = new GetTemporaryAccountManagementLogModel();
                    if (reader["CreatedBy"] != DBNull.Value && Convert.ToInt32(reader["CreatedBy"]) != 0)
                    {
                        DateTime createdOn = Convert.ToDateTime(reader["CreatedOnUtc"]);
                        string createdTime = createdOn.ToString("hh:mm:ss tt");
                        message = "New Transaction with " + reader["TransactionId"].ToString() + " is Created by " + reader["CreatedByUser"].ToString() + " on " + reader["CreatedOn"].ToString()
                            + ", Where Transaction Date equals to " + reader["TransactionDate"].ToString() + " , Amount equals to " + reader["Amount"].ToString() + " , Quantity equals to " +
                            reader["Quantity"].ToString() + " , Total Amount equals to " + reader["TotalAmount"].ToString() + " , Vat equals to " + reader["Vat"].ToString() +
                            ", Vat Amount equals to " + reader["VatAmount"].ToString() + " , Grand Total equals to " + reader["GrandTotal"].ToString() + " and Entry type is selected as " +
                            reader["EntryType"].ToString() + ".";

                        log.message = message;
                        log.CreatedOn = reader["CreatedOn"].ToString() + " " + createdTime;
                        log.CreatedBy = reader["CreatedByUser"].ToString();
                        log.CreatedOnUtc = Convert.ToDateTime(reader["CreatedOnUtc"]);
                        log.ApprovedOn = "--";
                        log.ApprovedBy = "--";
                        log.RejectedBy = "--";
                        log.RejectedOn = "--";
                        log.ModifyOn = "--";
                        log.ModifyBy = "--";
                    }

                    if (reader["ModifyBy"] != DBNull.Value && Convert.ToInt32(reader["ModifyBy"]) != 0)
                    {
                        DateTime modifyOn = Convert.ToDateTime(reader["ModifyOnUtc"]);
                        string modifyTime = modifyOn.ToString("hh:mm:ss tt");
                        if (reader["IsDelete"] != DBNull.Value && Convert.ToBoolean(reader["IsDelete"]) == true)
                        {
                            message = "Transaction with " + reader["TransactionId"].ToString() + " is Deleted by " + reader["ModifyByUser"].ToString() + " on " + reader["ModifyOn"].ToString()
                            + ", Where Transaction Date equals to " + reader["TransactionDate"].ToString() + " , Amount equals to " + reader["Amount"].ToString() + " , Quantity equals to " +
                            reader["Quantity"].ToString() + " , Total Amount equals to " + reader["TotalAmount"].ToString() + " , Vat equals to " + reader["Vat"].ToString() +
                            ", Vat Amount equals to " + reader["VatAmount"].ToString() + " , Grand Total equals to " + reader["GrandTotal"].ToString() + " and Entry type is selected as " +
                            reader["EntryType"].ToString() + ".";
                        }
                        else
                        {
                            message = "Transaction with " + reader["TransactionId"].ToString() + " is Modify by " + reader["ModifyByUser"].ToString() + " on " + reader["ModifyOn"].ToString()
                            + ", Where Transaction Date equals to " + reader["TransactionDate"].ToString() + " , Amount equals to " + reader["Amount"].ToString() + " , Quantity equals to " +
                            reader["Quantity"].ToString() + " , Total Amount equals to " + reader["TotalAmount"].ToString() + " , Vat equals to " + reader["Vat"].ToString() +
                            ", Vat Amount equals to " + reader["VatAmount"].ToString() + " , Grand Total equals to " + reader["GrandTotal"].ToString() + " and Entry type is selected as " +
                            reader["EntryType"].ToString() + ".";
                        }

                        log.message = message;
                        log.ModifyOn = reader["ModifyOn"].ToString() + " " + modifyTime;
                        log.ModifyBy = reader["ModifyByUser"].ToString();
                        log.ModifyOnUtc = Convert.ToDateTime(reader["ModifyOnUtc"]);
                        log.ApprovedOn = "--";
                        log.ApprovedBy = "--";
                        log.RejectedBy = "--";
                        log.RejectedOn = "--";
                        log.CreatedOn = "--";
                        log.CreatedBy = "--";
                    }

                    if (reader["ApprovedBy"] != DBNull.Value && Convert.ToInt32(reader["ApprovedBy"]) != 0)
                    {
                        DateTime approvedOn = Convert.ToDateTime(reader["ApprovedOnUtc"]);
                        string approvedTime = approvedOn.ToString("hh:mm:ss tt");
                        message = "Transaction with " + reader["TransactionId"].ToString() + " is Approved by " + reader["ApprovedByUser"].ToString() + " on " + reader["ApprovedOn"].ToString() + ".";

                        log.message = message;
                        log.ApprovedOn = reader["ApprovedOn"].ToString() + " " + approvedTime;
                        log.ApprovedBy = reader["ApprovedByUser"].ToString();
                        log.ApprovedOnUtc = Convert.ToDateTime(reader["ApprovedOnUtc"]);
                        log.ModifyBy = "--";
                        log.ModifyOn = "--";
                        log.RejectedBy = "--";
                        log.RejectedOn = "--";
                        log.CreatedOn = "--";
                        log.CreatedBy = "--";
                    }

                    if (reader["RejectedBy"] != DBNull.Value && Convert.ToInt32(reader["RejectedBy"]) != 0)
                    {
                        DateTime rejectedOn = Convert.ToDateTime(reader["RejectedOnUtc"]);
                        string rejectedTime = rejectedOn.ToString("hh:mm:ss tt");
                        message = "Transaction with " + reader["TransactionId"].ToString() + " is Rejected by " + reader["RejectedByUser"].ToString() + " on " + reader["RejectedOn"].ToString() + ".";

                        log.message = message;
                        log.RejectedOn = reader["RejectedOn"].ToString() + " " + rejectedTime;
                        log.RejectedBy = reader["RejectedByUser"].ToString();
                        log.RejectedOnUtc = Convert.ToDateTime(reader["RejectedOnUtc"]);
                        log.ModifyBy = "--";
                        log.ModifyOn = "--";
                        log.ApprovedBy = "--";
                        log.ApprovedOn = "--";
                        log.CreatedOn = "--";
                        log.CreatedBy = "--";
                    }

                    totalLogs++;
                    TempLogs.Add(log);
                }

                connection.Close();

                if (searchString != null && searchString != "")
                {
                    var data = (from log in TempLogs
                                where log.message.Contains(searchString)
                                || log.CreatedOn.Contains(searchString)
                                || log.ApprovedOn.Contains(searchString)
                                || log.RejectedOn.Contains(searchString)
                                || log.ModifyOn.Contains(searchString)
                                || log.ApprovedBy.Contains(searchString)
                                || log.RejectedBy.Contains(searchString)
                                || log.CreatedBy.Contains(searchString)
                                || log.ModifyBy.Contains(searchString)
                                select log).ToList();
                    totalLogs = data.Count;
                    logs = data;
                }
                else
                {
                    logs = TempLogs;
                }

                if (sortBy != "" && sortBy != null)
                {
                    if (sortDirection == "desc")
                    {
                        if (sortBy == "Log Message")
                        {
                            var data = (from log in logs
                                        orderby log.message descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Created By")
                        {
                            var data = (from log in logs
                                        orderby log.CreatedBy descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Created On")
                        {
                            var data = (from log in logs
                                        orderby log.CreatedOnUtc descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Modify By")
                        {
                            var data = (from log in logs
                                        orderby log.ModifyBy descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Modify On")
                        {
                            var data = (from log in logs
                                        orderby log.ModifyOnUtc descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Approved By")
                        {
                            var data = (from log in logs
                                        orderby log.ApprovedBy descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Approved On")
                        {
                            var data = (from log in logs
                                        orderby log.ApprovedOnUtc descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Rejected By")
                        {
                            var data = (from log in logs
                                        orderby log.RejectedBy descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Rejected On")
                        {
                            var data = (from log in logs
                                        orderby log.RejectedOnUtc descending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                    }
                    else
                    {
                        if (sortBy == "Log Message")
                        {
                            var data = (from log in logs
                                        orderby log.message ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Created By")
                        {
                            var data = (from log in logs
                                        orderby log.CreatedBy ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Created On")
                        {
                            var data = (from log in logs
                                        orderby log.CreatedOnUtc ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Modify By")
                        {
                            var data = (from log in logs
                                        orderby log.ModifyBy ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Modify On")
                        {
                            var data = (from log in logs
                                        orderby log.ModifyOnUtc ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Approved By")
                        {
                            var data = (from log in logs
                                        orderby log.ApprovedBy ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Approved On")
                        {
                            var data = (from log in logs
                                        orderby log.ApprovedOnUtc ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Rejected By")
                        {
                            var data = (from log in logs
                                        orderby log.RejectedBy ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                        else if (sortBy == "Rejected On")
                        {
                            var data = (from log in logs
                                        orderby log.RejectedOnUtc ascending
                                        select log).ToList();
                            var expense = data.Skip(page).Take(pageSize).ToList();
                            logs = expense;
                            totalLogs = data.Count;
                        }
                    }
                }
                else
                {
                    var expense = logs.Skip(page).Take(pageSize).ToList();
                    logs = expense;
                }

                foreach (var log in logs)
                {
                    log.Index = index;
                    index++;
                }

                model.logs = logs;
                model.totalLogs = totalLogs;
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<GetAccountTypeModel> CheckAccountEntryType(string serialCode)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetAccountTypeModel accountType = new GetAccountTypeModel();

                SqlCommand command = new SqlCommand("USP_Admin_CheckAccountEntryType", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@transactionCode", serialCode);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["AccountType"] != DBNull.Value)
                        accountType.accountType = reader["AccountType"].ToString();
                    if (reader["Id"] != DBNull.Value)
                        accountType.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        accountType.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if (reader["TotalAmount"] != DBNull.Value)
                        accountType.TotalAmount = reader["TotalAmount"].ToString();
                }
                connection.Close();

                return accountType;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
