using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class AccountManagementService : IAccountManagementService
    {
        private readonly Connection _dataSetting;
        public AccountManagementService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        /// <summary>
        /// Get History of All Company Expenses                         -- Yashasvi TBC (25-11-2022)
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
                    getHistoryOfCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    getHistoryOfCompany.CompanyName = reader["CompanyName"].ToString();
                    getHistoryOfCompany.PaymentCredit = reader["PaymentCredit"].ToString();
                    getHistoryOfCompany.CreatedOnUtc = reader["Date"].ToString();
                    getHistoryOfCompany.Description = reader["Description"].ToString();
                    getHistoryOfCompany.ExpensesAmount = reader["Amount"].ToString();
                    getHistoryOfCompany.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    getHistoryOfCompany.TransactionId = reader["TransactionId"].ToString();
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
        public GetAccountHistoryData GetHistoryOfAllCompanyExpensesTest(int pageNumber, int fetchRows, string searchKey, string sortColumn, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                fetchRows = fetchRows / 2;
                int totalHistoryCount = 0;
                GetAccountHistoryData model = new GetAccountHistoryData();
                List<GetHistoryOfCompanyExpensesDatatable> getHistoryOfCompanyExpenses = new List<GetHistoryOfCompanyExpensesDatatable>();
                IList<GetHistoryOfCompanyExpensesDatatable> getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                List<GetHistoryOfCompanyExpensesDatatable> GetDebits = new List<GetHistoryOfCompanyExpensesDatatable>();
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
                    GetHistoryOfCompanyExpensesDatatable getHistoryOfCompany = new GetHistoryOfCompanyExpensesDatatable();
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
                    getHistoryOfCompany.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]);
                    getHistoryOfCompany.TransactionType = "Credit";

                    getdata.Add(getHistoryOfCompany);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    GetHistoryOfCompanyExpensesDatatable getHistoryOfCompany = new GetHistoryOfCompanyExpensesDatatable();
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
                    getHistoryOfCompany.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]);
                    getHistoryOfCompany.TransactionType = "Debit";


                    getdata.Add(getHistoryOfCompany);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    totalHistoryCount += Convert.ToInt32(reader["TransactionCount"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    totalHistoryCount += Convert.ToInt32(reader["ExpenseCount"]);
                }
                con.Close();

                if (sortDirection != null && sortDirection == "desc")
                {
                    if (sortColumn == "Created On")
                    {
                        var getalldata = from data in getdata
                                         orderby data.CreatedOn descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else
                    {
                        var getalldata = from data in getdata
                                         orderby data.TransactionIdNo descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                }
                else
                {
                    if (sortColumn == "Created On")
                    {
                        var getalldata = from data in getdata
                                         orderby data.CreatedOn
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else
                    {
                        var getalldata = from data in getdata
                                         orderby data.TransactionIdNo
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }

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


        public GetAccountHistoryData GetHistoryOfAllCompanyExpensesFilter(int skip, int take, string? searchBy, string? searchValue, string sortColumn, string sortDirection)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                GetAccountHistoryData model = new GetAccountHistoryData();
                List<GetHistoryOfCompanyExpensesDatatable> getHistoryOfCompanyExpenses = new List<GetHistoryOfCompanyExpensesDatatable>();
                IList<GetHistoryOfCompanyExpensesDatatable> getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                List<GetHistoryOfCompanyExpensesDatatable> GetDebits = new List<GetHistoryOfCompanyExpensesDatatable>();
                SqlCommand command = new SqlCommand("USP_Admin_GetHistoryofExpenses_Test_2", con);
                command.CommandType = CommandType.StoredProcedure;
                //command.Parameters.AddWithValue("@searchPrefix", searchValue);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetHistoryOfCompanyExpensesDatatable getHistoryOfCompany = new GetHistoryOfCompanyExpensesDatatable();
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
                    getHistoryOfCompany.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]);
                    getHistoryOfCompany.GrandTotal = getHistoryOfCompany.PaymentCredit;
                    getHistoryOfCompany.TransactionType = "Credit";

                    getdata.Add(getHistoryOfCompany);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    GetHistoryOfCompanyExpensesDatatable getHistoryOfCompany = new GetHistoryOfCompanyExpensesDatatable();
                    getHistoryOfCompany.Id = Convert.ToInt32(reader["Id"]);
                    getHistoryOfCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    getHistoryOfCompany.ExpenseReceiptId = Convert.ToInt32(reader["ExpenseReceiptId"]);
                    getHistoryOfCompany.CompanyName = reader["CompanyName"].ToString();
                    getHistoryOfCompany.Description = reader["ExpensesTitle"].ToString();
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
                    getHistoryOfCompany.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]);
                    getHistoryOfCompany.TransactionType = "Debit";


                    getdata.Add(getHistoryOfCompany);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    totalHistoryCount += Convert.ToInt32(reader["TransactionCount"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    totalHistoryCount += Convert.ToInt32(reader["ExpenseCount"]);
                }
                con.Close();

                //if (searchBy != null && searchBy != null)
                //{
                //    if (searchBy == "Transaction Id")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "^[0-9]*$"))
                //        {
                //            var getalldata = from data in getdata
                //                             where data.TransactionIdNo == Convert.ToInt32(searchValue)
                //                             select data;
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                //            Match result = re.Match(searchValue);
                //            string alphaPart = result.Groups[1].Value.ToUpper();
                //            var margeStr = alphaPart + result.Groups[2].Value;
                //            var getalldata = from data in getdata
                //                             where data.TransactionId == searchValue.ToString()
                //                             || data.TransactionId == margeStr
                //                             select data;
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Created On")
                //    {
                //        DateTime dt = DateTime.ParseExact(searchValue, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                //        var getalldata = from data in getdata
                //                         where data.CreatedOn == dt
                //                         select data;
                //        getdata = getalldata.ToList();
                //        totalHistoryCount = getdata.Count();
                //    }
                //    else if (searchBy == "Company")
                //    {
                //        var getalldata = from data in getdata
                //                         where data.CompanyName.Contains(searchValue.ToString()) ||
                //                               data.CompanyName.Contains(searchValue.ToLower()) ||
                //                               data.CompanyName.Contains(searchValue.ToUpper())
                //                         select data;
                //        getdata = getalldata.ToList();
                //        totalHistoryCount = getdata.Count();
                //    }
                //    else if (searchBy == "Description")
                //    {
                //        var getalldata = from data in getdata
                //                         where data.Description.Contains(searchValue.ToString())
                //                         || data.Description.Contains(searchValue.ToLower())
                //                         || data.Description.Contains(searchValue.ToUpper())
                //                         select data;
                //        getdata = getalldata.ToList();
                //        totalHistoryCount = getdata.Count();
                //    }
                //    else if (searchBy == "Type")
                //    {
                //        var getalldata = from data in getdata
                //                         where data.TransactionType == searchValue.ToString()
                //                         || data.TransactionType == searchValue.ToLower()
                //                         || data.TransactionType == searchValue.ToUpper()
                //                         select data;
                //        getdata = getalldata.ToList();
                //        totalHistoryCount = getdata.Count();
                //    }
                //    else if (searchBy == "Amount")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            var getalldata = from data in getdata
                //                             where Convert.ToDecimal(data.ExpensesAmount) == Convert.ToDecimal(searchValue)
                //                             select data;
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Quantity")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "^[0-9]*$"))
                //        {
                //            var getalldata = from data in getdata
                //                             where Convert.ToInt32(data.Quantity) == Convert.ToInt32(searchValue)
                //                             select data;
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Total Amount")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            var getalldata = from data in getdata
                //                             where Convert.ToDecimal(data.TotalAmount) == Convert.ToDecimal(searchValue) ||
                //                             Convert.ToDecimal(data.PaymentCredit) == Convert.ToDecimal(searchValue)
                //                             select data;
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Vat(%)")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            var getalldata = from data in getdata
                //                             where Convert.ToDecimal(data.Vat) == Convert.ToDecimal(searchValue)
                //                             select data;
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Vat Amount")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            var getalldata = from data in getdata
                //                             where Convert.ToDecimal(data.VatAmount) == Convert.ToDecimal(searchValue)
                //                             select data;
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //    else if (searchBy == "Grand Total")
                //    {
                //        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                //        {
                //            var getalldata = from data in getdata
                //                             where Convert.ToDecimal(data.GrandTotal) == Convert.ToDecimal(searchValue) ||
                //                             Convert.ToDecimal(data.PaymentCredit) == Convert.ToDecimal(searchValue)
                //                             select data;
                //            getdata = getalldata.ToList();
                //            totalHistoryCount = getdata.Count();
                //        }
                //        else
                //        {
                //            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                //            totalHistoryCount = getdata.Count();
                //        }
                //    }
                //}
                if (searchBy != null && searchValue != null)
                {
                    if (searchBy == "Transaction Id")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "^[0-9]*$"))
                        {
                            var getalldata = from data in getdata
                                             where data.TransactionIdNo == Convert.ToInt32(searchValue)
                                             select data;
                            getdata = getalldata.ToList();
                            totalHistoryCount = getdata.Count();
                        }
                        else
                        {
                            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                            Match result = re.Match(searchValue);
                            string alphaPart = result.Groups[1].Value.ToUpper();
                            var margeStr = alphaPart + result.Groups[2].Value;
                            var getalldata = from data in getdata
                                             where data.TransactionId == searchValue.ToString() ||
                                             data.TransactionId == margeStr
                                             select data;
                            getdata = getalldata.ToList();
                            totalHistoryCount = getdata.Count();
                        }
                    }
                    else if (searchBy == "Created On")
                    {
                        DateTime dt = DateTime.ParseExact(searchValue, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        var getalldata = from data in getdata
                                         where data.CreatedOn == dt
                                         select data;
                        getdata = getalldata.ToList();
                        totalHistoryCount = getdata.Count();
                    }
                    else if (searchBy == "Company")
                    {
                        var search = searchValue.Substring(0, 1).ToUpper();
                        var searchStr = searchValue.Substring(1, searchValue.Length - 1);
                        var searchStr1 = search + searchStr;

                        var getalldata = from data in getdata
                                         where data.CompanyName.Contains(searchValue.ToString()) ||
                                               data.CompanyName.Contains(searchValue.ToLower()) ||
                                               data.CompanyName.Contains(searchValue.ToUpper()) ||
                                               data.CompanyName.Contains(searchStr1)
                                         select data;
                        getdata = getalldata.ToList();
                        totalHistoryCount = getdata.Count();
                    }
                    else if (searchBy == "Description")
                    {
                        var search = searchValue.Substring(0, 1).ToUpper();
                        var searchStr = searchValue.Substring(1, searchValue.Length - 1);
                        var searchStr1 = search + searchStr;
                        var getalldata = from data in getdata
                                         where data.Description.Contains(searchValue.ToString())
                                         || data.Description.Contains(searchValue.ToLower())
                                         || data.Description.Contains(searchValue.ToUpper())
                                         || data.Description.Contains(searchStr1)
                                         select data;
                        getdata = getalldata.ToList();
                        totalHistoryCount = getdata.Count();
                    }
                    else if (searchBy == "Type")
                    {
                        var search = searchValue.Substring(0, 1).ToUpper();
                        var searchStr = searchValue.Substring(1, searchValue.Length - 1);
                        var searchStr1 = search + searchStr;

                        var getalldata = from data in getdata
                                         where data.TransactionType.Contains(searchValue.ToString())
                                         || data.TransactionType.Contains(searchValue.ToLower())
                                         || data.TransactionType.Contains(searchValue.ToUpper())
                                         || data.TransactionType.Contains(searchStr1)
                                         select data;
                        getdata = getalldata.ToList();
                        totalHistoryCount = getdata.Count();
                    }
                    else if (searchBy == "Amount")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                        {
                            var getalldata = from data in getdata
                                             where Convert.ToDecimal(data.ExpensesAmount) == Convert.ToDecimal(searchValue)
                                             select data;
                            getdata = getalldata.ToList();
                            totalHistoryCount = getdata.Count();
                        }
                        else
                        {
                            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                            totalHistoryCount = getdata.Count();
                        }
                    }
                    else if (searchBy == "Quantity")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "^[0-9]*$"))
                        {
                            var getalldata = from data in getdata
                                             where Convert.ToInt32(data.Quantity) == Convert.ToInt32(searchValue)
                                             select data;
                            getdata = getalldata.ToList();
                            totalHistoryCount = getdata.Count();
                        }
                        else
                        {
                            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                            totalHistoryCount = getdata.Count();
                        }
                    }
                    else if (searchBy == "Total Amount")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                        {
                            var getalldata = from data in getdata
                                             where Convert.ToDecimal(data.TotalAmount) == Convert.ToDecimal(searchValue) ||
                                             Convert.ToDecimal(data.PaymentCredit) == Convert.ToDecimal(searchValue)
                                             select data;
                            getdata = getalldata.ToList();
                            totalHistoryCount = getdata.Count();
                        }
                        else
                        {
                            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                            totalHistoryCount = getdata.Count();
                        }
                    }
                    else if (searchBy == "Vat(%)")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                        {
                            var getalldata = from data in getdata
                                             where Convert.ToDecimal(data.Vat) == Convert.ToDecimal(searchValue)
                                             select data;
                            getdata = getalldata.ToList();
                            totalHistoryCount = getdata.Count();
                        }
                        else
                        {
                            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                            totalHistoryCount = getdata.Count();
                        }
                    }
                    else if (searchBy == "Vat Amount")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                        {
                            var getalldata = from data in getdata
                                             where Convert.ToDecimal(data.VatAmount) == Convert.ToDecimal(searchValue)
                                             select data;
                            getdata = getalldata.ToList();
                            totalHistoryCount = getdata.Count();
                        }
                        else
                        {
                            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                            totalHistoryCount = getdata.Count();
                        }
                    }
                    else if (searchBy == "Grand Total")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(searchValue, "[+-]?([0-9]*[.])?[0-9]+"))
                        {
                            var getalldata = from data in getdata
                                             where Convert.ToDecimal(data.GrandTotal) == Convert.ToDecimal(searchValue) ||
                                             Convert.ToDecimal(data.PaymentCredit) == Convert.ToDecimal(searchValue)
                                             select data;
                            getdata = getalldata.ToList();
                            totalHistoryCount = getdata.Count();
                        }
                        else
                        {
                            getdata = new List<GetHistoryOfCompanyExpensesDatatable>();
                            totalHistoryCount = getdata.Count();
                        }
                    }
                }

                if (sortDirection != null && sortDirection == "desc")
                {
                    if (sortColumn == "Created On")
                    {
                        var getalldata = from data in getdata
                                         orderby data.CreatedOn descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else if (sortColumn == "Transaction Id")
                    {
                        var getalldata = from data in getdata
                                         orderby data.TransactionIdNo descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else if (sortColumn == "Company")
                    {
                        var getalldata = from data in getdata
                                         orderby data.CompanyName descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else if (sortColumn == "Type")
                    {
                        var getalldata = from data in getdata
                                         orderby data.TransactionType descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else if (sortColumn == "Grand Total")
                    {
                        var getalldata = from data in getdata
                                         orderby Convert.ToDecimal(data.GrandTotal) descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                }
                else
                {
                    if (sortColumn == "Created On")
                    {
                        var getalldata = from data in getdata
                                         orderby data.CreatedOn
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else if (sortColumn == "Transaction Id")
                    {
                        var getalldata = from data in getdata
                                         orderby data.TransactionIdNo
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else if (sortColumn == "Company")
                    {
                        var getalldata = from data in getdata
                                         orderby data.CompanyName
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else if (sortColumn == "Type")
                    {
                        var getalldata = from data in getdata
                                         orderby data.TransactionType
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
                    else if (sortColumn == "Grand Total")
                    {
                        var getalldata = from data in getdata
                                         orderby Convert.ToDecimal(data.GrandTotal) descending
                                         select data;
                        var getHistoryOfCompanyExpense = getalldata.Skip(skip).Take(take).ToList();
                        model.getHistoryOfCompanyExpenses = getHistoryOfCompanyExpense;
                        model.expenseCounts = totalHistoryCount;
                    }
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

        /// <summary>
        /// Add Company Expense / Portal Balance                    -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="expenses"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AddCompanyExpensesAccount(List<SaveCompanyExpenses> expenses)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = null;
                for (int index = 0; index < expenses.Count; index++)
                {
                    if (expenses[index].Type == "Credit")
                    {
                        command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", expenses[index].UserId);
                        command.Parameters.AddWithValue("@CompanyId", expenses[index].CompanyId);
                        command.Parameters.AddWithValue("@PortalAmount", expenses[index].Amount);
                        command.Parameters.AddWithValue("@CurrentAmount", expenses[index].Amount);
                        command.Parameters.AddWithValue("@PaymentMode", expenses[index].PaymentMode);
                        command.Parameters.AddWithValue("@Description", expenses[index].Task);
                        command.Parameters.AddWithValue("@TotalAmount", expenses[index].GrandTotal);
                        command.Parameters.AddWithValue("@Quantity", expenses[index].Quantity);
                        command.Parameters.AddWithValue("@PaymentStatus", "Success");
                        command.Parameters.AddWithValue("@CreatedOn", expenses[index].Date);
                        command.Parameters.AddWithValue("@Status", Operation.Insert);
                        con.Open();
                        int PaymentId = (int)command.ExecuteScalar();
                        con.Close();
                        command.Parameters.Clear();

                        if (PaymentId > 0)
                        {
                            command = new SqlCommand("USP_Admin_SavePaymentReceipt", con);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UserId", expenses[index].UserId);
                            command.Parameters.AddWithValue("@CompanyId", expenses[index].CompanyId);
                            command.Parameters.AddWithValue("@PaymentId", PaymentId);
                            command.Parameters.AddWithValue("@CreatedOn", expenses[index].Date);
                            command.Parameters.AddWithValue("@OnAccount", expenses[index].Task);
                            con.Open();
                            _returnId = (int)command.ExecuteScalar();
                            con.Close();
                            command.Parameters.Clear();

                            command = new SqlCommand("USP_Admin_PortalBalanceOperation", con);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UserId", expenses[index].UserId);
                            command.Parameters.AddWithValue("@CompanyId", expenses[index].CompanyId);
                            command.Parameters.AddWithValue("@PortalAmount", expenses[index].GrandTotal);
                            command.Parameters.AddWithValue("@IsCompletelyUsed", 0);
                            command.Parameters.AddWithValue("@Status", Operation.Insert);
                            con.Open();
                            _returnId = (int)command.ExecuteScalar();
                            con.Close();

                            command.Parameters.Clear();
                        }
                    }
                    else
                    {
                        command = new SqlCommand("USP_Admin_SaveAllCompanyExpenses", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CompanyId", expenses[index].CompanyId);
                        command.Parameters.AddWithValue("@ExpensesTitle", expenses[index].Task);
                        command.Parameters.AddWithValue("@ExpenseAmount", expenses[index].Amount);
                        if (expenses[index].TransactionId != "" && expenses[index].TransactionId != null)
                            command.Parameters.AddWithValue("@TransactionNumber", expenses[index].TransactionId);
                        command.Parameters.AddWithValue("@Quantity", expenses[index].Quantity);
                        command.Parameters.AddWithValue("@TotalAmount", expenses[index].TotalAmount);
                        command.Parameters.AddWithValue("@Vat", expenses[index].Vat);
                        command.Parameters.AddWithValue("@VatAmount", expenses[index].VatAmount);
                        command.Parameters.AddWithValue("@GrandTotal", expenses[index].GrandTotal);
                        command.Parameters.AddWithValue("@CreatedOnUtc", expenses[index].Date);
                        command.Parameters.AddWithValue("@UserId", expenses[index].UserId);

                        con.Open();
                        _returnId = (int)command.ExecuteScalar();
                        con.Close();
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
                con.Close();
            }
        }
        /// <summary>
        /// Update Company Expense Details                                      -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string UpdateExpenseDetail(UpdateCompanyExpenses model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _returnMessage = "";
                int _returnId = 0;
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
                    if (reader["TransactionId"] != DBNull.Value)
                        modelData.TransactionId = reader["TransactionId"].ToString();
                    if (reader["ResponseData"] != DBNull.Value)
                        modelData.ResponseData = Convert.ToInt32(reader["ResponseData"]);
                }
                con.Close();
                if (modelData.ResponseData > 0)
                {
                    _returnMessage = "Selected Company Expense Updated Successfully.";
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
                    _returnMessage = "Any operation does not perform on selected Company Expense (<b>"+model.TransactionId+"</b>) because it is already modified by " + modelData.Username + " at " + modelData.ModifyTime;
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
        /// Get Expense Details by Id                                               -- Yashasvi TBC (25-11-2022)
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
        /// <summary>
        /// Delete Expense Detail                               -- Yashasvi TBC (25-11-2022)
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
                command.Parameters.AddWithValue("@RemainingPortalBalance", Amount);
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Username"] != DBNull.Value)
                        model.Username = reader["Username"].ToString();
                    if (reader["ModifyTime"] != DBNull.Value)
                        model.ModifyTime = reader["ModifyTime"].ToString();
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["ModifyTime"].ToString();
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
                    _returnId = "Any operation does not perform on selected expense (<b>"+model.TransactionId+"</b>) because it is already modified by " + model.Username + " at " + model.ModifyTime;
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
        /// Get Details Of Payment                                  -- Yashasvi TBC (25-11-2022)
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
                    paymentTransaction.TransactionId = reader["TransactionId"].ToString();
                    paymentTransaction.Quantity = Convert.ToInt32(reader["Quantity"]);
                    paymentTransaction.TotalAmount = reader["TotalAmount"].ToString();
                    paymentTransaction.PaymentMode = reader["PaymentMode"].ToString();
                    paymentTransaction.Description = reader["Description"].ToString();
                    paymentTransaction.PaymentStatus = reader["PaymentStatus"].ToString();
                    paymentTransaction.TransectionDate = reader["TransectionDate"].ToString();
                    paymentTransaction.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    paymentTransaction.ModifyOnUtc = reader["ModifyOnUtc"].ToString();

                    var date = Convert.ToDateTime(paymentTransaction.TransectionDate);
                    var dateString = date.ToString("yyyy-MM-dd");
                    paymentTransaction.TransectionDate = dateString;
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
        /// Update Payment Details                              -- Yashasvi TBC (25-11-2022)
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public string UpdatePaymentDetails(PaymentTransaction paymentTransaction)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ResponseModel model = new ResponseModel();
                int _returnId = 0;
                string _returnMessage = "";
                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_PaymentTransectionOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", paymentTransaction.UserId);
                command.Parameters.AddWithValue("@CompanyId", paymentTransaction.CompanyId);
                command.Parameters.AddWithValue("@PortalAmount", paymentTransaction.Amount);
                command.Parameters.AddWithValue("@Quantity", paymentTransaction.Quantity);
                command.Parameters.AddWithValue("@TotalAmount", paymentTransaction.TotalAmount);
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
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["ResponseData"] != DBNull.Value)
                        model.ResponseData = Convert.ToInt32(reader["ResponseData"]);
                }
                con.Close();

                if (model.ResponseData > 0)
                {
                    _returnMessage = "Selected Payment Transaction Updated Successfully.";
                }
                else
                {
                    _returnMessage = "Any operation does not perform on selected Payment Transaction (<b>"+model.TransactionId+"</b>) because it is already modified by " + model.Username + " at " + model.ModifyTime;
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
        /// Delete Payment Transaction Details                                              -- Yashasvi TBC (25-11-2022)
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
                int _returnId = 0;
                ResponseModel model = new ResponseModel();
                SqlCommand command = null;
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
                    returnMessage = "Any operation does not perform on selected Payment Transaction (<b>"+model.TransactionId+"</b>) because it is already modified by " + model.Username + " at " + model.ModifyTime;
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
    }
}
