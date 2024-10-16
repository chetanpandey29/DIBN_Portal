using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class PortalBalanceService : IPortalBalanceService
    {
        private readonly Connection _dataSetting;
        public PortalBalanceService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        public int AddPortalBalance(string BalanceAmount,int CompanyId,int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_PortalBalanceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@PortalAmount", BalanceAmount);
                command.Parameters.AddWithValue("@IsCompletelyUsed", 0);
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                con.Open();
                _returnId = (int) command.ExecuteScalar();
                con.Close();

                command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@PortalAmount", BalanceAmount);
                command.Parameters.AddWithValue("@PaymentStatus", "Success");
                command.Parameters.AddWithValue("@Status", Operation.Insert);
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

        public PortalBalanceViewModel GetPortalBalance(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                PortalBalanceViewModel model = new PortalBalanceViewModel();
                SqlCommand command = new SqlCommand("USP_Admin_PortalBalanceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.UserId = Convert.ToInt32(reader["UserId"]);
                    model.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    model.BalanceAmount = reader["PortalAmount"].ToString();
                    model.RemainingPortalBalance = reader["RemainingPortalBalance"].ToString();
                    model.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    model.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    model.IsCompletelyUsed = Convert.ToBoolean(reader["IsCompletelyUsed"].ToString());
                }
                con.Close();
                return model;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { 
                con.Close(); 
            }
        }
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
                    expenses.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    expenses.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    expenses.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    expenses.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"].ToString());
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
        public List<PaymentTransection> GetTransectionsDetails(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<PaymentTransection> transections = new List<PaymentTransection>();
                SqlCommand command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PaymentTransection transection = new PaymentTransection();
                    transection.Id = Convert.ToInt32(reader["Id"]);
                    transection.UserId = Convert.ToInt32(reader["UserId"]);
                    transection.Username = reader["Username"].ToString();
                    transection.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    transection.CompanyName = reader["CompanyName"].ToString();
                    transection.Amount = reader["Amount"].ToString();
                    transection.PaymentStatus = reader["PaymentStatus"].ToString();
                    transection.TransectionDate = reader["TransectionDate"].ToString();
                    transection.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    transection.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    transections.Add(transection);
                }
                con.Close();
                return transections;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
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
        public CompanyAccountDetails GetCompanyAccount(int CompanyId,string FromDate,string ToDate)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CompanyAccountDetails model = new CompanyAccountDetails();
                model.getexpensesofCompanies = new List<GetexpensesofCompany>();
                List<GetexpensesofCompany> getdata = new List<GetexpensesofCompany>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyAccountDetails", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                if(FromDate != null && ToDate != null)
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
                    if (reader["TransactionId"] != DBNull.Value)
                        getexpensesofCompany.TransactionId = reader["TransactionId"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        getexpensesofCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["PaymentTransactionId"] != DBNull.Value)
                        getexpensesofCompany.PaymentTransactionId = Convert.ToInt32(reader["PaymentTransactionId"]);
                    if (reader["PaymentCredit"] != DBNull.Value)
                        getexpensesofCompany.PaymentCredit = reader["PaymentCredit"].ToString();
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
                    if (reader["TransactionId"] != DBNull.Value)
                        getexpensesofCompany.TransactionId = reader["TransactionId"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        getexpensesofCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value && reader["GrandTotal"].ToString() != "-1")
                    {
                        getexpensesofCompany.Balance += Convert.ToDecimal(reader["GrandTotal"].ToString());
                        getexpensesofCompany.Debit = reader["GrandTotal"].ToString();
                        getexpensesofCompany.Credit = "---";
                    }
                    if (reader["Vat"] != DBNull.Value)
                        getexpensesofCompany.Vat = reader["Vat"].ToString();
                    if (reader["VatAmount"] != DBNull.Value)
                        getexpensesofCompany.VatAmount = reader["VatAmount"].ToString();
                    if (reader["Id"] != DBNull.Value)
                        getexpensesofCompany.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyId"] != DBNull.Value)
                        getexpensesofCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    getexpensesofCompany.Type = "Debit";
                    if (reader["ExpenseReceiptId"] != DBNull.Value)
                        getexpensesofCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
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
        public int AddCompanyExpensesAccount(List<SaveCompanyExpenses> expenses)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = null;

                for (int index = 0; index < expenses.Count; index++)
                {
                    command = new SqlCommand("USP_Admin_SaveAllCompanyExpenses", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CompanyId", expenses[index].CompanyId);
                    command.Parameters.AddWithValue("@ExpensesTitle", expenses[index].Task);
                    command.Parameters.AddWithValue("@ExpenseAmount", expenses[index].Amount);
                    command.Parameters.AddWithValue("@Quantity", expenses[index].Quantity);
                    command.Parameters.AddWithValue("@TotalAmount", expenses[index].TotalAmount);

                    con.Open();
                    _returnId = (int)command.ExecuteScalar();
                    con.Close();
                    command.Parameters.Clear();
                    //if (expenses[index].formFile != null)
                    //{
                    //    string _getName = expenses[index].formFile.FileName;
                    //    var Name = expenses[index].formFile.FileName.Split(".");
                    //    string FileName = Name[0];
                    //    var extn = Path.GetExtension(_getName);
                    //    Byte[] bytes = null;
                    //    using (MemoryStream ms = new MemoryStream())
                    //    {
                    //        expenses[index].formFile.OpenReadStream().CopyTo(ms);
                    //        bytes = ms.ToArray();
                    //    }

                    //    command = new SqlCommand("USP_Admin_ExpesesRecieptOperation", con);
                    //    command.CommandType = CommandType.StoredProcedure;
                    //    command.Parameters.AddWithValue("@Status", Operation.Insert);
                    //    command.Parameters.AddWithValue("@CompanyId", expenses[index].CompanyId);
                    //    command.Parameters.AddWithValue("@DataBinary", bytes);
                    //    command.Parameters.AddWithValue("@FileName", FileName);
                    //    command.Parameters.AddWithValue("@Extension", extn);
                    //    command.Parameters.AddWithValue("@ReceiptId", _returnId);
                    //    con.Open();
                    //    _returnId = (int)command.ExecuteScalar();
                    //    con.Close();
                    //    command.Parameters.Clear();
                    //}
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
        public List<GetPaymentTransactionReceipt> GetAllPaymentTransactionReceipt(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetPaymentTransactionReceipt> transactionReceipts = new List<GetPaymentTransactionReceipt>();
                SqlCommand command = new SqlCommand("USP_Admin_GetPaymentReceipt", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
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

        public async Task<GetPaymentReceiptsByCompanyIdWithPaginationModel> GetPaymentReceiptsByCompanyIdWithPagination(int CompanyId,int skipRows,int takeRows,string searchBy,string searchValue,string sortBy,string sortingDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetPaymentReceiptsByCompanyIdWithPaginationModel model = new GetPaymentReceiptsByCompanyIdWithPaginationModel();
                List<GetPaymentTransactionReceipt> transactionReceipts = new List<GetPaymentTransactionReceipt>();
                int totalPaymentReceipts = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetPaymentReceiptsByCompanyIdWithPagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@rowsOfPage", takeRows);
                if(searchValue!=null)
                    command.Parameters.AddWithValue("@searchPrefix", searchValue);
                if(sortBy != null)
                    command.Parameters.AddWithValue("@sortBy", sortBy);
                if(sortingDirection != null)
                    command.Parameters.AddWithValue("@sortingDirection", sortingDirection);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while(reader.Read())
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
                connection.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_GetPaymentReceiptsByCompanyIdWithPaginationCount", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                if (searchValue != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchValue);

                connection.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        totalPaymentReceipts++;
                }
                connection.Close();

                model.getPaymentTransactions = transactionReceipts;
                model.totalPaymentReceipts = totalPaymentReceipts;
                return model;
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
    }
}
