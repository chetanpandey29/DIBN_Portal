using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;
using static System.Net.WebRequestMethods;

namespace DIBN.Areas.Admin.Repository
{
    public class ReportManagementRepository : IReportManagementRepository
    {
        private readonly Connection _dataSetting;
        public ReportManagementRepository(
            Connection dataSetting
        )
        {
            _dataSetting = dataSetting;
        }

        public async Task<ProfitLossCustomReportModel> GetAccountHistoryForProfitLossCustomReportTotalDetails(string fromDate,string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCustomReportModel model = new ProfitLossCustomReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCustomSelectedDateReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                    totalCredit = Convert.ToDouble(model.TotalCredit);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                    totalDebit = Convert.ToDouble(model.TotalDebit);
                }

                connection.Close();

                totalProfitLoss = totalCredit - totalDebit;

                if (totalProfitLoss > 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                else if (totalProfitLoss != 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                model.totalProfitLoss = totalProfitLoss;
                model.totalCredit = totalCredit;
                model.totalDebit = totalDebit;
                model.profitLossPercentage = profitLossPercentage;
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

        public async Task<ProfitLossCustomReportExcelAndPdfModel> GetAccountHistoryForProfitLossCustomReportExcelPdf(string fromDate, string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                double totalProfitLoss = 0, profitLossPercentage = 0;
                ProfitLossCustomReportExcelAndPdfModel profitLoss = new ProfitLossCustomReportExcelAndPdfModel();
                List<GetAccountHistoryForProfitLossCustomReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCustomReportModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCustomSelectedDateReport_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCustomReportModel model = new GetAccountHistoryForProfitLossCustomReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        if (reader["Type"].ToString() =="Debit")
                            model.GrandTotal = Convert.ToDecimal("-"+reader["GrandTotal"]);
                        else
                            model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    }
                        
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        profitLoss.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        profitLoss.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();
                
                if (accountHistory.Count > 0)
                {
                    var data = Convert.ToDouble(profitLoss.TotalCredit) + Convert.ToDouble(profitLoss.TotalDebit);
                    totalProfitLoss = Convert.ToDouble(profitLoss.TotalCredit) - Convert.ToDouble(profitLoss.TotalDebit);
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                profitLoss.totalProfitLoss = totalProfitLoss;
                profitLoss.totalCredit = Convert.ToDouble(profitLoss.TotalCredit);
                profitLoss.totalDebit = Convert.ToDouble(profitLoss.TotalDebit);
                profitLoss.profitLossPercentage = profitLossPercentage;
                profitLoss.accountList = accountHistory;
                return profitLoss;
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

        public async Task<ProfitLossCustomReportPaginationModel> GetAccountHistoryForProfitLossCustomReport(string fromDate,string toDate,int skipRows,int rowsOfPage,
            string? searchPrefix,string sortColumn,string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                //string minDate = "", maxDate = "";
                //DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                //minDate = FromDate.ToString("yyyy-MM-dd");
                //maxDate = ToDate.ToString("yyyy-MM-dd");
                ProfitLossCustomReportPaginationModel profitLoss = new ProfitLossCustomReportPaginationModel();
                List<GetAccountHistoryForProfitLossCustomReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCustomReportModel>();
                int totalAccountEntry = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCustomSelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if(searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while( reader.Read())
                {
                    GetAccountHistoryForProfitLossCustomReportModel model = new GetAccountHistoryForProfitLossCustomReportModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index= dateArray.Length-1; index>=0;index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                        
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalAccountEntry"] != DBNull.Value)
                        totalAccountEntry = Convert.ToInt32(reader["TotalAccountEntry"]);
                }

                connection.Close();

                profitLoss.accountList = accountHistory;
                profitLoss.totalAccountEntry = totalAccountEntry;

                return profitLoss;
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

        public async Task<ProfitLossWeeklyReportModel> GetAccountHistoryForProfitLossWeeklyReportTotalDetails(string fromDate, string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossWeeklyReportModel model = new ProfitLossWeeklyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossWeeklySelectedDateReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                    totalCredit = Convert.ToDouble(model.TotalCredit);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                    totalDebit = Convert.ToDouble(model.TotalDebit);
                }

                connection.Close();
                totalProfitLoss = totalCredit - totalDebit;

                if (totalProfitLoss > 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                else if(totalProfitLoss != 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                model.totalProfitLoss = totalProfitLoss;
                model.totalCredit = totalCredit;
                model.totalDebit = totalDebit;
                model.profitLossPercentage = profitLossPercentage;
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

        public async Task<ProfitLossWeeklyReportExcelAndPdfModel> GetAccountHistoryForProfitLossWeeklyReportExcelPdf(string fromDate, string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossWeeklyReportExcelAndPdfModel profitLoss = new ProfitLossWeeklyReportExcelAndPdfModel();
                List<GetAccountHistoryForProfitLossWeeklyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossWeeklyReportModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossWeeklySelectedDateReport_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossWeeklyReportModel model = new GetAccountHistoryForProfitLossWeeklyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        if (reader["Type"].ToString() == "Debit")
                            model.GrandTotal = Convert.ToDecimal("-" + reader["GrandTotal"]);
                        else
                            model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    }
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        profitLoss.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        profitLoss.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                if (accountHistory.Count > 0)
                {
                    var data = Convert.ToDouble(profitLoss.TotalCredit) + Convert.ToDouble(profitLoss.TotalDebit);
                    totalProfitLoss = Convert.ToDouble(profitLoss.TotalCredit) - Convert.ToDouble(profitLoss.TotalDebit);
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                profitLoss.totalProfitLoss = totalProfitLoss;
                profitLoss.totalCredit = Convert.ToDouble(profitLoss.TotalCredit);
                profitLoss.totalDebit = Convert.ToDouble(profitLoss.TotalDebit);
                profitLoss.profitLossPercentage = profitLossPercentage;
                profitLoss.accountList = accountHistory;
                return profitLoss;
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

        public async Task<ProfitLossWeeklyReportPaginationModel> GetAccountHistoryForProfitLossWeeklyReport(string fromDate, string toDate, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                ProfitLossWeeklyReportPaginationModel profitLoss = new ProfitLossWeeklyReportPaginationModel();
                List<GetAccountHistoryForProfitLossWeeklyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossWeeklyReportModel>();
                int totalAccountEntry = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossWeeklySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossWeeklyReportModel model = new GetAccountHistoryForProfitLossWeeklyReportModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalAccountEntry"] != DBNull.Value)
                        totalAccountEntry = Convert.ToInt32(reader["TotalAccountEntry"]);
                }

                connection.Close();

                profitLoss.accountList = accountHistory;
                profitLoss.totalAccountEntry = totalAccountEntry;

                return profitLoss;
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

        public async Task<ProfitLossMonthlyReportModel> GetAccountHistoryForProfitLossMonthlyReportTotalDetails(string monthNumber, string yearNumber)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossMonthlyReportModel model = new ProfitLossMonthlyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossMonthlySelectedReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MonthNumber", monthNumber);
                command.Parameters.AddWithValue("@Year", yearNumber);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                    totalCredit = Convert.ToDouble(model.TotalCredit);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                    totalDebit = Convert.ToDouble(model.TotalDebit);
                }

                connection.Close();

                totalProfitLoss = totalCredit - totalDebit;

                if (totalProfitLoss > 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                else if (totalProfitLoss != 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                model.totalProfitLoss = totalProfitLoss;
                model.totalCredit = totalCredit;
                model.totalDebit = totalDebit;
                model.profitLossPercentage = profitLossPercentage;

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

        public async Task<ProfitLossMonthlyReportExcelAndPdfModel> GetAccountHistoryForProfitLossMonthlyReportExcelPdf(string month,string year)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossMonthlyReportExcelAndPdfModel profitLoss = new ProfitLossMonthlyReportExcelAndPdfModel();
                List<GetAccountHistoryForProfitLossMonthlyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossMonthlyReportModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossMonthlySelectedReport_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MonthNumber", month);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossMonthlyReportModel model = new GetAccountHistoryForProfitLossMonthlyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        if (reader["Type"].ToString() == "Debit")
                            model.GrandTotal = Convert.ToDecimal("-" + reader["GrandTotal"]);
                        else
                            model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    }
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        profitLoss.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        profitLoss.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();
                if (accountHistory.Count > 0)
                {
                    var data = Convert.ToDouble(profitLoss.TotalCredit) + Convert.ToDouble(profitLoss.TotalDebit);
                    totalProfitLoss = Convert.ToDouble(profitLoss.TotalCredit) - Convert.ToDouble(profitLoss.TotalDebit);
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                profitLoss.totalProfitLoss = totalProfitLoss;
                profitLoss.totalCredit = Convert.ToDouble(profitLoss.TotalCredit);
                profitLoss.totalDebit = Convert.ToDouble(profitLoss.TotalDebit);
                profitLoss.profitLossPercentage = profitLossPercentage;
                profitLoss.accountList = accountHistory;
                return profitLoss;
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

        public async Task<ProfitLossMonthlyReportPaginationModel> GetAccountHistoryForProfitLossMonthlyReport(string month, string year, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ProfitLossMonthlyReportPaginationModel profitLoss = new ProfitLossMonthlyReportPaginationModel();
                List<GetAccountHistoryForProfitLossMonthlyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossMonthlyReportModel>();
                int totalAccountEntry = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossMonthlySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@monthNumber", month);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossMonthlyReportModel model = new GetAccountHistoryForProfitLossMonthlyReportModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalAccountEntry"] != DBNull.Value)
                        totalAccountEntry = Convert.ToInt32(reader["TotalAccountEntry"]);
                }

                connection.Close();

                profitLoss.accountList = accountHistory;
                profitLoss.totalAccountEntry = totalAccountEntry;

                return profitLoss;
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

        public async Task<ProfitLossYearlyReportModel> GetAccountHistoryForProfitLossYearlyReportTotalDetails(string yearNumber)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossYearlyReportModel model = new ProfitLossYearlyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossYearlySelectedReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Year", yearNumber);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                    totalCredit = Convert.ToDouble(model.TotalCredit);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                    totalDebit = Convert.ToDouble(model.TotalDebit);
                }

                connection.Close();

                totalProfitLoss = totalCredit - totalDebit;

                if (totalProfitLoss > 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                else if (totalProfitLoss != 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                model.totalProfitLoss = totalProfitLoss;
                model.totalCredit = totalCredit;
                model.totalDebit = totalDebit;
                model.profitLossPercentage = profitLossPercentage;

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

        public async Task<ProfitLossYearlyReportExcelAndPdfModel> GetAccountHistoryForProfitLossYearlyReportExcelPdf(string year)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossYearlyReportExcelAndPdfModel profitLoss = new ProfitLossYearlyReportExcelAndPdfModel();
                List<GetAccountHistoryForProfitLossYearlyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossYearlyReportModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossYearlySelectedReport_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossYearlyReportModel model = new GetAccountHistoryForProfitLossYearlyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        if (reader["Type"].ToString() == "Debit")
                            model.GrandTotal = Convert.ToDecimal("-" + reader["GrandTotal"]);
                        else
                            model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    }
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        profitLoss.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        profitLoss.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();
                if (accountHistory.Count > 0)
                {
                    var data = Convert.ToDouble(profitLoss.TotalCredit) + Convert.ToDouble(profitLoss.TotalDebit);
                    totalProfitLoss = Convert.ToDouble(profitLoss.TotalCredit) - Convert.ToDouble(profitLoss.TotalDebit);
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                profitLoss.totalProfitLoss = totalProfitLoss;
                profitLoss.totalCredit = Convert.ToDouble(profitLoss.TotalCredit);
                profitLoss.totalDebit = Convert.ToDouble(profitLoss.TotalDebit);
                profitLoss.profitLossPercentage = profitLossPercentage;
                profitLoss.accountList = accountHistory;
                return profitLoss;
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

        public async Task<ProfitLossYearlyReportPaginationModel> GetAccountHistoryForProfitLossYearlyReport(string year, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ProfitLossYearlyReportPaginationModel profitLoss = new ProfitLossYearlyReportPaginationModel();
                List<GetAccountHistoryForProfitLossYearlyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossYearlyReportModel>();
                int totalAccountEntry = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossYearlySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossYearlyReportModel model = new GetAccountHistoryForProfitLossYearlyReportModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalAccountEntry"] != DBNull.Value)
                        totalAccountEntry = Convert.ToInt32(reader["TotalAccountEntry"]);
                }

                connection.Close();

                profitLoss.accountList = accountHistory;
                profitLoss.totalAccountEntry = totalAccountEntry;

                return profitLoss;
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

        public async Task<List<GetAccountHistoryForProfitLossCustomReportGraphDataModel>> GetAccountHistoryForProfitLossCustomReportGraph(string fromDate,string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                List<GetAccountHistoryForProfitLossCustomReportGraphDataModel> getHistory = new List<GetAccountHistoryForProfitLossCustomReportGraphDataModel>();
                List<GetAccountHistoryForProfitLossCustomReportGraphDataModel> profitLoss = new List<GetAccountHistoryForProfitLossCustomReportGraphDataModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCustomSelectedDateReport_Graph", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCustomReportGraphDataModel model = new GetAccountHistoryForProfitLossCustomReportGraphDataModel();

                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]).ToString("0.00");
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    getHistory.Add(model);
                }

                connection.Close();

                var totaleDates = getHistory.GroupBy(item => item.TransactionDate).ToList();

                //totaleDates = totaleDates.OrderBy(item => item).ToList();

                foreach(var t in totaleDates)
                {
                    double credit = 0, debit = 0;
                    var creditData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Credit")).ToList();

                    var debitData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Debit")).ToList();

                    if(creditData != null)
                    {
                        foreach(var item in creditData)
                        {
                            credit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    if(debitData != null)
                    {
                        foreach(var item in debitData)
                        {
                            debit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    GetAccountHistoryForProfitLossCustomReportGraphDataModel creditModel = new GetAccountHistoryForProfitLossCustomReportGraphDataModel();
                    creditModel.Date = t.Key.ToString("dd-MM-yyyy");
                    creditModel.GrandTotal = credit.ToString("0.00");
                    creditModel.Type = "Credit";

                    GetAccountHistoryForProfitLossCustomReportGraphDataModel debitModel = new GetAccountHistoryForProfitLossCustomReportGraphDataModel();
                    debitModel.Date = t.Key.ToString("dd-MM-yyyy");
                    debitModel.GrandTotal = debit.ToString("0.00");
                    debitModel.Type = "Debit";

                    profitLoss.Add(creditModel);
                    profitLoss.Add(debitModel);
                }

                return profitLoss;
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

        public async Task<List<GetAccountHistoryForProfitLossWeeklyReportGraphDataModel>> GetAccountHistoryForProfitLossWeeklyReportGraph(string fromDate, string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                List<GetAccountHistoryForProfitLossWeeklyReportGraphDataModel> getHistory = new List<GetAccountHistoryForProfitLossWeeklyReportGraphDataModel>();
                List<GetAccountHistoryForProfitLossWeeklyReportGraphDataModel> profitLoss = new List<GetAccountHistoryForProfitLossWeeklyReportGraphDataModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossWeeklyDateReport_Graph", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossWeeklyReportGraphDataModel model = new GetAccountHistoryForProfitLossWeeklyReportGraphDataModel();

                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]).ToString("0.00");
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    getHistory.Add(model);
                }

                connection.Close();

                var totaleDates = getHistory.GroupBy(item => item.TransactionDate).ToList();

                //totaleDates = totaleDates.OrderBy(item => item).ToList();

                foreach (var t in totaleDates)
                {
                    double credit = 0, debit = 0;
                    var creditData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Credit")).ToList();

                    var debitData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Debit")).ToList();

                    if (creditData != null)
                    {
                        foreach (var item in creditData)
                        {
                            credit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    if (debitData != null)
                    {
                        foreach (var item in debitData)
                        {
                            debit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    GetAccountHistoryForProfitLossWeeklyReportGraphDataModel creditModel = new GetAccountHistoryForProfitLossWeeklyReportGraphDataModel();
                    creditModel.Date = t.Key.ToString("dd-MM-yyyy");
                    creditModel.GrandTotal = credit.ToString("0.00");
                    creditModel.Type = "Credit";

                    GetAccountHistoryForProfitLossWeeklyReportGraphDataModel debitModel = new GetAccountHistoryForProfitLossWeeklyReportGraphDataModel();
                    debitModel.Date = t.Key.ToString("dd-MM-yyyy");
                    debitModel.GrandTotal = debit.ToString("0.00");
                    debitModel.Type = "Debit";

                    profitLoss.Add(creditModel);
                    profitLoss.Add(debitModel);
                }

                return profitLoss;
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

        public async Task<List<GetAccountHistoryForProfitLossYearlyReportGraphDataModel>> GetAccountHistoryForProfitLossYealyReportGraph(string year)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAccountHistoryForProfitLossYearlyReportGraphDataModel> getHistory = new List<GetAccountHistoryForProfitLossYearlyReportGraphDataModel>();
                List<GetAccountHistoryForProfitLossYearlyReportGraphDataModel> profitLoss = new List<GetAccountHistoryForProfitLossYearlyReportGraphDataModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossYearlyDateReport_Graph", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossYearlyReportGraphDataModel model = new GetAccountHistoryForProfitLossYearlyReportGraphDataModel();

                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]).ToString("0.00");
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    if (reader["TransactionMonth"] != DBNull.Value)
                        model.TransactionMonth = reader["TransactionMonth"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    getHistory.Add(model);
                }

                connection.Close();

                var totaleDates = new List<string>();
                totaleDates.Add("1");
                totaleDates.Add("2");
                totaleDates.Add("3");
                totaleDates.Add("4");
                totaleDates.Add("5");
                totaleDates.Add("6");
                totaleDates.Add("7");
                totaleDates.Add("8");
                totaleDates.Add("9");
                totaleDates.Add("10");
                totaleDates.Add("11");
                totaleDates.Add("12");

                //totaleDates = totaleDates.OrderBy(item => item).ToList();

                foreach (var t in totaleDates)
                {
                    double credit = 0, debit = 0;
                    var creditData = getHistory.Where(item => item.TransactionMonth == t && item.Type.Contains("Credit")).ToList();

                    var debitData = getHistory.Where(item => item.TransactionMonth == t && item.Type.Contains("Debit")).ToList();

                    if (creditData != null)
                    {
                        foreach (var item in creditData)
                        {
                            credit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    if (debitData != null)
                    {
                        foreach (var item in debitData)
                        {
                            debit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    GetAccountHistoryForProfitLossYearlyReportGraphDataModel creditModel = new GetAccountHistoryForProfitLossYearlyReportGraphDataModel();
                    creditModel.Date = GetMonth(t);
                    creditModel.GrandTotal = credit.ToString("0.00");
                    creditModel.Type = "Credit";

                    GetAccountHistoryForProfitLossYearlyReportGraphDataModel debitModel = new GetAccountHistoryForProfitLossYearlyReportGraphDataModel();
                    debitModel.Date = GetMonth(t);
                    debitModel.GrandTotal = debit.ToString("0.00");
                    debitModel.Type = "Debit";

                    profitLoss.Add(creditModel);
                    profitLoss.Add(debitModel);
                }

                return profitLoss;
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

        public async Task<List<GetAccountHistoryForProfitLossMonthlyReportGraphDataModel>> GetAccountHistoryForProfitLossMonthlyReportGraph(string year,string month)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAccountHistoryForProfitLossMonthlyReportGraphDataModel> getHistory = new List<GetAccountHistoryForProfitLossMonthlyReportGraphDataModel>();
                List<GetAccountHistoryForProfitLossMonthlyReportGraphDataModel> profitLoss = new List<GetAccountHistoryForProfitLossMonthlyReportGraphDataModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossMonthlyDateReport_Graph", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossMonthlyReportGraphDataModel model = new GetAccountHistoryForProfitLossMonthlyReportGraphDataModel();

                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]).ToString("0.00");
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    getHistory.Add(model);
                }

                connection.Close();

                var totaleDates = getHistory.GroupBy(item => item.TransactionDate).ToList();

                //totaleDates = totaleDates.OrderBy(item => item).ToList();

                foreach (var t in totaleDates)
                {
                    double credit = 0, debit = 0;
                    var creditData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Credit")).ToList();

                    var debitData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Debit")).ToList();

                    if (creditData != null)
                    {
                        foreach (var item in creditData)
                        {
                            credit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    if (debitData != null)
                    {
                        foreach (var item in debitData)
                        {
                            debit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    GetAccountHistoryForProfitLossMonthlyReportGraphDataModel creditModel = new GetAccountHistoryForProfitLossMonthlyReportGraphDataModel();
                    creditModel.Date = t.Key.ToString("dd-MM-yyyy");
                    creditModel.GrandTotal = credit.ToString("0.00");
                    creditModel.Type = "Credit";

                    GetAccountHistoryForProfitLossMonthlyReportGraphDataModel debitModel = new GetAccountHistoryForProfitLossMonthlyReportGraphDataModel();
                    debitModel.Date = t.Key.ToString("dd-MM-yyyy");
                    debitModel.GrandTotal = debit.ToString("0.00");
                    debitModel.Type = "Debit";

                    profitLoss.Add(creditModel);
                    profitLoss.Add(debitModel);
                }

                return profitLoss;
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

        public async Task<GetCompanyListForProfitLossPagination> GetCompanyListForProfitLoss(int skipRows,int rowsOfPage,string searchString,string sortColumn,string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalCompany = 0;
                GetCompanyListForProfitLossPagination profitLoss = new GetCompanyListForProfitLossPagination();

                List<GetCompanyListForProfitLossModel> getCompanies = new List<GetCompanyListForProfitLossModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetProfitLossCompanyList",connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@rowsOfPage", rowsOfPage);
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchString", searchString);
                if (sortColumn != null && sortColumn != "")
                    command.Parameters.AddWithValue("@sortColumn", sortColumn);
                if (sortDirection != null && sortDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetCompanyListForProfitLossModel model = new GetCompanyListForProfitLossModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CompanyAccountNumber"] != DBNull.Value)
                        model.CompanyAccountNumber = reader["CompanyAccountNumber"].ToString();

                    getCompanies.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["CompanyCount"] != DBNull.Value)
                        totalCompany = Convert.ToInt32(reader["CompanyCount"]);
                }

                connection.Close();

                profitLoss.companyListForProfitLossModels = getCompanies;
                profitLoss.totalCompany = totalCompany;
                return profitLoss;
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
        public async Task<ProfitLossCompanyCustomReportModel> GetAccountHistoryForProfitLossCompanyCustomReportTotalDetails(string fromDate, string toDate,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCompanyCustomReportModel model = new ProfitLossCompanyCustomReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyCustomSelectedDateReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                    totalCredit = Convert.ToDouble(model.TotalCredit);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                    totalDebit = Convert.ToDouble(model.TotalDebit);
                }

                connection.Close();

                totalProfitLoss = totalCredit - totalDebit;

                if (totalProfitLoss > 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                else if (totalProfitLoss != 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                model.totalProfitLoss = totalProfitLoss;
                model.totalCredit = totalCredit;
                model.totalDebit = totalDebit;
                model.profitLossPercentage = profitLossPercentage;
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
        public async Task<ProfitLossCompanyCustomReportPaginationModel> GetAccountHistoryForProfitLossCompanyCustomReport(string fromDate, string toDate,int companyId, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ProfitLossCompanyCustomReportPaginationModel profitLoss = new ProfitLossCompanyCustomReportPaginationModel();
                List<GetAccountHistoryForProfitLossCompanyCustomReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCompanyCustomReportModel>();
                int totalAccountEntry = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyCustomSelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyCustomReportModel model = new GetAccountHistoryForProfitLossCompanyCustomReportModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalAccountEntry"] != DBNull.Value)
                        totalAccountEntry = Convert.ToInt32(reader["TotalAccountEntry"]);
                }

                connection.Close();

                profitLoss.accountList = accountHistory;
                profitLoss.totalAccountEntry = totalAccountEntry;

                return profitLoss;
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
        public async Task<ProfitLossCompanyCustomReportExcelAndPdfModel> GetAccountHistoryForProfitLossCompanyCustomReportExcelPdf(string fromDate, string toDate,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCompanyCustomReportExcelAndPdfModel profitLoss = new ProfitLossCompanyCustomReportExcelAndPdfModel();
                List<GetAccountHistoryForProfitLossCompanyCustomReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCompanyCustomReportModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyCustomSelectedDateReport_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyCustomReportModel model = new GetAccountHistoryForProfitLossCompanyCustomReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        if (reader["Type"].ToString() == "Debit")
                            model.GrandTotal = Convert.ToDecimal("-" + reader["GrandTotal"]);
                        else
                            model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    }
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        profitLoss.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        profitLoss.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();
                if (accountHistory.Count > 0)
                {
                    var data = Convert.ToDouble(profitLoss.TotalCredit) + Convert.ToDouble(profitLoss.TotalDebit);
                    totalProfitLoss = Convert.ToDouble(profitLoss.TotalCredit) - Convert.ToDouble(profitLoss.TotalDebit);
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                profitLoss.totalProfitLoss = totalProfitLoss;
                profitLoss.totalCredit = Convert.ToDouble(profitLoss.TotalCredit);
                profitLoss.totalDebit = Convert.ToDouble(profitLoss.TotalDebit);
                profitLoss.profitLossPercentage = profitLossPercentage;
                profitLoss.accountList = accountHistory;
                return profitLoss;
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
        public async Task<ProfitLossCompanyWeeklyReportModel> GetAccountHistoryForProfitLossCompanyWeeklyReportTotalDetails(string fromDate, string toDate,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCompanyWeeklyReportModel model = new ProfitLossCompanyWeeklyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyWeeklySelectedDateReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                    totalCredit = Convert.ToDouble(model.TotalCredit);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                    totalDebit = Convert.ToDouble(model.TotalDebit);
                }

                connection.Close();
                totalProfitLoss = totalCredit - totalDebit;

                if (totalProfitLoss > 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                else if (totalProfitLoss != 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                model.totalProfitLoss = totalProfitLoss;
                model.totalCredit = totalCredit;
                model.totalDebit = totalDebit;
                model.profitLossPercentage = profitLossPercentage;
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
        public async Task<ProfitLossCompanyWeeklyReportExcelAndPdfModel> GetAccountHistoryForProfitLossCompanyWeeklyReportExcelPdf(string fromDate, string toDate,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCompanyWeeklyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyWeeklyReportExcelAndPdfModel();
                List<GetAccountHistoryForProfitLossCompanyWeeklyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCompanyWeeklyReportModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyWeeklySelectedDateReport_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyWeeklyReportModel model = new GetAccountHistoryForProfitLossCompanyWeeklyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        if (reader["Type"].ToString() == "Debit")
                            model.GrandTotal = Convert.ToDecimal("-" + reader["GrandTotal"]);
                        else
                            model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    }
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        profitLoss.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        profitLoss.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();
                
                if(accountHistory.Count > 0)
                {
                    var data = Convert.ToDouble(profitLoss.TotalCredit) + Convert.ToDouble(profitLoss.TotalDebit);
                    totalProfitLoss = Convert.ToDouble(profitLoss.TotalCredit) - Convert.ToDouble(profitLoss.TotalDebit);
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                
                profitLoss.totalProfitLoss = totalProfitLoss;
                profitLoss.totalCredit = Convert.ToDouble(profitLoss.TotalCredit);
                profitLoss.totalDebit = Convert.ToDouble(profitLoss.TotalDebit);
                profitLoss.profitLossPercentage = profitLossPercentage;
                profitLoss.accountList = accountHistory;
                return profitLoss;
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
        public async Task<ProfitLossCompanyWeeklyReportPaginationModel> GetAccountHistoryForProfitLossCompanyWeeklyReport(string fromDate, string toDate,int companyId, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                ProfitLossCompanyWeeklyReportPaginationModel profitLoss = new ProfitLossCompanyWeeklyReportPaginationModel();
                List<GetAccountHistoryForProfitLossCompanyWeeklyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCompanyWeeklyReportModel>();
                int totalAccountEntry = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyWeeklySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyWeeklyReportModel model = new GetAccountHistoryForProfitLossCompanyWeeklyReportModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalAccountEntry"] != DBNull.Value)
                        totalAccountEntry = Convert.ToInt32(reader["TotalAccountEntry"]);
                }

                connection.Close();

                profitLoss.accountList = accountHistory;
                profitLoss.totalAccountEntry = totalAccountEntry;

                return profitLoss;
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

        public async Task<ProfitLossCompanyMonthlyReportModel> GetAccountHistoryForProfitLossCompanyMonthlyReportTotalDetails(string monthNumber, string yearNumber,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCompanyMonthlyReportModel model = new ProfitLossCompanyMonthlyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyMonthlySelectedReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@MonthNumber", monthNumber);
                command.Parameters.AddWithValue("@Year", yearNumber);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                    totalCredit = Convert.ToDouble(model.TotalCredit);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                    totalDebit = Convert.ToDouble(model.TotalDebit);
                }

                connection.Close();

                totalProfitLoss = totalCredit - totalDebit;

                if (totalProfitLoss > 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                else if (totalProfitLoss != 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                model.totalProfitLoss = totalProfitLoss;
                model.totalCredit = totalCredit;
                model.totalDebit = totalDebit;
                model.profitLossPercentage = profitLossPercentage;

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

        public async Task<ProfitLossCompanyMonthlyReportExcelAndPdfModel> GetAccountHistoryForProfitLossCompanyMonthlyReportExcelPdf(string month, string year,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCompanyMonthlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyMonthlyReportExcelAndPdfModel();
                List<GetAccountHistoryForProfitLossCompanyMonthlyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCompanyMonthlyReportModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyMonthlySelectedReport_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@MonthNumber", month);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyMonthlyReportModel model = new GetAccountHistoryForProfitLossCompanyMonthlyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        if (reader["Type"].ToString() == "Debit")
                            model.GrandTotal = Convert.ToDecimal("-" + reader["GrandTotal"]);
                        else
                            model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    }
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        profitLoss.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        profitLoss.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                if (accountHistory.Count > 0)
                {
                    var data = Convert.ToDouble(profitLoss.TotalCredit) + Convert.ToDouble(profitLoss.TotalDebit);
                    totalProfitLoss = Convert.ToDouble(profitLoss.TotalCredit) - Convert.ToDouble(profitLoss.TotalDebit);
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                profitLoss.totalProfitLoss = totalProfitLoss;
                profitLoss.totalCredit = Convert.ToDouble(profitLoss.TotalCredit);
                profitLoss.totalDebit = Convert.ToDouble(profitLoss.TotalDebit);
                profitLoss.profitLossPercentage = profitLossPercentage;
                profitLoss.accountList = accountHistory;
                return profitLoss;
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

        public async Task<ProfitLossCompanyMonthlyReportPaginationModel> GetAccountHistoryForProfitLossCompanyMonthlyReport(string month, string year,int companyId, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ProfitLossCompanyMonthlyReportPaginationModel profitLoss = new ProfitLossCompanyMonthlyReportPaginationModel();
                List<GetAccountHistoryForProfitLossCompanyMonthlyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCompanyMonthlyReportModel>();
                int totalAccountEntry = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyMonthlySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@monthNumber", month);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyMonthlyReportModel model = new GetAccountHistoryForProfitLossCompanyMonthlyReportModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalAccountEntry"] != DBNull.Value)
                        totalAccountEntry = Convert.ToInt32(reader["TotalAccountEntry"]);
                }

                connection.Close();

                profitLoss.accountList = accountHistory;
                profitLoss.totalAccountEntry = totalAccountEntry;

                return profitLoss;
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
        public async Task<ProfitLossCompanyYearlyReportModel> GetAccountHistoryForProfitLossCompanyYearlyReportTotalDetails(string yearNumber,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCompanyYearlyReportModel model = new ProfitLossCompanyYearlyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyYearlySelectedReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@Year", yearNumber);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                    totalCredit = Convert.ToDouble(model.TotalCredit);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                    totalDebit = Convert.ToDouble(model.TotalDebit);
                }

                connection.Close();

                totalProfitLoss = totalCredit - totalDebit;

                if (totalProfitLoss > 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }
                else if (totalProfitLoss != 0)
                {
                    var data = totalDebit + totalCredit;
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                model.totalProfitLoss = totalProfitLoss;
                model.totalCredit = totalCredit;
                model.totalDebit = totalDebit;
                model.profitLossPercentage = profitLossPercentage;

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

        public async Task<ProfitLossCompanyYearlyReportExcelAndPdfModel> GetAccountHistoryForProfitLossCompanyYearlyReportExcelPdf(string year,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                double totalProfitLoss = 0, profitLossPercentage = 0, totalCredit = 0, totalDebit = 0;
                ProfitLossCompanyYearlyReportExcelAndPdfModel profitLoss = new ProfitLossCompanyYearlyReportExcelAndPdfModel();
                List<GetAccountHistoryForProfitLossCompanyYearlyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCompanyYearlyReportModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyYearlySelectedReport_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyYearlyReportModel model = new GetAccountHistoryForProfitLossCompanyYearlyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                    {
                        if (reader["Type"].ToString() == "Debit")
                            model.GrandTotal = Convert.ToDecimal("-" + reader["GrandTotal"]);
                        else
                            model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    }
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        profitLoss.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        profitLoss.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                if (accountHistory.Count > 0)
                {
                    var data = Convert.ToDouble(profitLoss.TotalCredit) + Convert.ToDouble(profitLoss.TotalDebit);
                    totalProfitLoss = Convert.ToDouble(profitLoss.TotalCredit) - Convert.ToDouble(profitLoss.TotalDebit);
                    profitLossPercentage = (totalProfitLoss / data);
                    profitLossPercentage = profitLossPercentage * 100;
                }

                profitLoss.totalProfitLoss = totalProfitLoss;
                profitLoss.totalCredit = Convert.ToDouble(profitLoss.TotalCredit);
                profitLoss.totalDebit = Convert.ToDouble(profitLoss.TotalDebit);
                profitLoss.profitLossPercentage = profitLossPercentage;
                profitLoss.accountList = accountHistory;
                return profitLoss;
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

        public async Task<ProfitLossCompanyYearlyReportPaginationModel> GetAccountHistoryForProfitLossCompanyYearlyReport(string year,int companyId, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ProfitLossCompanyYearlyReportPaginationModel profitLoss = new ProfitLossCompanyYearlyReportPaginationModel();
                List<GetAccountHistoryForProfitLossCompanyYearlyReportModel> accountHistory = new List<GetAccountHistoryForProfitLossCompanyYearlyReportModel>();
                int totalAccountEntry = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyYearlySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@RowsOfPage", rowsOfPage);
                if (searchPrefix != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                command.Parameters.AddWithValue("@sortColumn", sortColumn);
                command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyYearlyReportModel model = new GetAccountHistoryForProfitLossCompanyYearlyReportModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();

                    accountHistory.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["TotalAccountEntry"] != DBNull.Value)
                        totalAccountEntry = Convert.ToInt32(reader["TotalAccountEntry"]);
                }

                connection.Close();

                profitLoss.accountList = accountHistory;
                profitLoss.totalAccountEntry = totalAccountEntry;

                return profitLoss;
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
        public async Task<List<GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel>> GetAccountHistoryForProfitLossCompanyCustomReportGraph(string fromDate, string toDate,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                List<GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel> getHistory = new List<GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel>();
                List<GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel> profitLoss = new List<GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyCustomSelectedDateReport_Graph", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel model = new GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel();

                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]).ToString("0.00");
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    getHistory.Add(model);
                }

                connection.Close();

                var totaleDates = getHistory.GroupBy(item => item.TransactionDate).ToList();

                //totaleDates = totaleDates.OrderBy(item => item).ToList();

                foreach (var t in totaleDates)
                {
                    double credit = 0, debit = 0;
                    var creditData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Credit")).ToList();

                    var debitData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Debit")).ToList();

                    if (creditData != null)
                    {
                        foreach (var item in creditData)
                        {
                            credit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    if (debitData != null)
                    {
                        foreach (var item in debitData)
                        {
                            debit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel creditModel = new GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel();
                    creditModel.Date = t.Key.ToString("dd-MM-yyyy");
                    creditModel.GrandTotal = credit.ToString("0.00");
                    creditModel.Type = "Credit";

                    GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel debitModel = new GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel();
                    debitModel.Date = t.Key.ToString("dd-MM-yyyy");
                    debitModel.GrandTotal = debit.ToString("0.00");
                    debitModel.Type = "Debit";

                    profitLoss.Add(creditModel);
                    profitLoss.Add(debitModel);
                }

                return profitLoss;
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
        public async Task<List<GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel>> GetAccountHistoryForProfitLossCompanyWeeklyReportGraph(string fromDate, string toDate,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                List<GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel> getHistory = new List<GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel>();
                List<GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel> profitLoss = new List<GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyWeeklyDateReport_Graph", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel model = new GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel();

                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]).ToString("0.00");
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    getHistory.Add(model);
                }

                connection.Close();

                var totaleDates = getHistory.GroupBy(item => item.TransactionDate).ToList();

                //totaleDates = totaleDates.OrderBy(item => item).ToList();

                foreach (var t in totaleDates)
                {
                    double credit = 0, debit = 0;
                    var creditData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Credit")).ToList();

                    var debitData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Debit")).ToList();

                    if (creditData != null)
                    {
                        foreach (var item in creditData)
                        {
                            credit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    if (debitData != null)
                    {
                        foreach (var item in debitData)
                        {
                            debit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel creditModel = new GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel();
                    creditModel.Date = t.Key.ToString("dd-MM-yyyy");
                    creditModel.GrandTotal = credit.ToString("0.00");
                    creditModel.Type = "Credit";

                    GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel debitModel = new GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel();
                    debitModel.Date = t.Key.ToString("dd-MM-yyyy");
                    debitModel.GrandTotal = debit.ToString("0.00");
                    debitModel.Type = "Debit";

                    profitLoss.Add(creditModel);
                    profitLoss.Add(debitModel);
                }

                return profitLoss;
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
        public async Task<List<GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel>> GetAccountHistoryForProfitLossCompanyYealyReportGraph(string year,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel> getHistory = new List<GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel>();
                List<GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel> profitLoss = new List<GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyYearlyDateReport_Graph", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel model = new GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel();

                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]).ToString("0.00");
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    if (reader["TransactionMonth"] != DBNull.Value)
                        model.TransactionMonth = reader["TransactionMonth"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    getHistory.Add(model);
                }

                connection.Close();

                var totaleDates = new List<string>();
                totaleDates.Add("1");
                totaleDates.Add("2");
                totaleDates.Add("3");
                totaleDates.Add("4");
                totaleDates.Add("5");
                totaleDates.Add("6");
                totaleDates.Add("7");
                totaleDates.Add("8");
                totaleDates.Add("9");
                totaleDates.Add("10");
                totaleDates.Add("11");
                totaleDates.Add("12");

                //totaleDates = totaleDates.OrderBy(item => item).ToList();

                foreach (var t in totaleDates)
                {
                    double credit = 0, debit = 0;
                    var creditData = getHistory.Where(item => item.TransactionMonth == t && item.Type.Contains("Credit")).ToList();

                    var debitData = getHistory.Where(item => item.TransactionMonth == t && item.Type.Contains("Debit")).ToList();

                    if (creditData != null)
                    {
                        foreach (var item in creditData)
                        {
                            credit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    if (debitData != null)
                    {
                        foreach (var item in debitData)
                        {
                            debit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel creditModel = new GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel();
                    creditModel.Date = GetMonth(t);
                    creditModel.GrandTotal = credit.ToString("0.00");
                    creditModel.Type = "Credit";

                    GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel debitModel = new GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel();
                    debitModel.Date = GetMonth(t);
                    debitModel.GrandTotal = debit.ToString("0.00");
                    debitModel.Type = "Debit";

                    profitLoss.Add(creditModel);
                    profitLoss.Add(debitModel);
                }

                return profitLoss;
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
        public async Task<List<GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel>> GetAccountHistoryForProfitLossCompanyMonthlyReportGraph(string year, string month,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel> getHistory = new List<GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel>();
                List<GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel> profitLoss = new List<GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ProfitLossCompanyMonthlyDateReport_Graph", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel model = new GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel();

                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDouble(reader["GrandTotal"]).ToString("0.00");
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Date"] != DBNull.Value)
                        model.Date = reader["Date"].ToString();
                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    getHistory.Add(model);
                }

                connection.Close();

                var totaleDates = getHistory.GroupBy(item => item.TransactionDate).ToList();

                //totaleDates = totaleDates.OrderBy(item => item).ToList();

                foreach (var t in totaleDates)
                {
                    double credit = 0, debit = 0;
                    var creditData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Credit")).ToList();

                    var debitData = getHistory.Where(item => item.TransactionDate == t.Key && item.Type.Contains("Debit")).ToList();

                    if (creditData != null)
                    {
                        foreach (var item in creditData)
                        {
                            credit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    if (debitData != null)
                    {
                        foreach (var item in debitData)
                        {
                            debit += Convert.ToDouble(item.GrandTotal);
                        }
                    }

                    GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel creditModel = new GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel();
                    creditModel.Date = t.Key.ToString("dd-MM-yyyy");
                    creditModel.GrandTotal = credit.ToString("0.00");
                    creditModel.Type = "Credit";

                    GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel debitModel = new GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel();
                    debitModel.Date = t.Key.ToString("dd-MM-yyyy");
                    debitModel.GrandTotal = debit.ToString("0.00");
                    debitModel.Type = "Debit";

                    profitLoss.Add(creditModel);
                    profitLoss.Add(debitModel);
                }

                return profitLoss;
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
        public async Task<BalanceSheetCustomReportModel> GetAccountHistoryForBalanceSheetCustomReportTotalDetails(string fromDate, string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                decimal totalBalance = 0;
                BalanceSheetCustomReportModel model = new BalanceSheetCustomReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetCustomSelectedDateReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                totalBalance = Convert.ToDecimal(model.TotalCredit) - Convert.ToDecimal(model.TotalDebit);
                model.TotalBalance = totalBalance.ToString("0.00");
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
        public async Task<BalanceSheetCustomReportPaginationModel> GetAccountHistoryForBalanceSheetCustomReport(string fromDate, string toDate, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                BalanceSheetCustomReportPaginationModel balanceSheetModel = new BalanceSheetCustomReportPaginationModel();

                List<GetAccountHistoryForBalanceSheetCustomReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetCustomReportModel>();
                List<GetAccountHistoryForBalanceSheetCustomReportModel> returnAccountHistory = new List<GetAccountHistoryForBalanceSheetCustomReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetCustomSelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetCustomReportModel model = new GetAccountHistoryForBalanceSheetCustomReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                    {
                        model.Type = reader["Type"].ToString();
                        if(model.Type == "Credit")
                        {
                            model.CreditTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.DebitTotal = 0;
                        }   
                        else
                        {
                            model.DebitTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.CreditTotal = 0;
                        }
                    }
                    getAccountHistory.Add(model);
                }

                connection.Close();
                if (searchPrefix != null && searchPrefix != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                      where data.TransactionIdNo == Convert.ToInt32(searchPrefix) ||
                                      data.GrandTotal == Convert.ToDecimal(searchPrefix)
                                      select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else if(searchPrefix.Contains("-")) {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.Date.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionId.Contains(searchPrefix) ||
                                                data.CompanyName.Contains(searchPrefix) ||
                                                data.Description.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                }
                if (sortColumn != "" && sortDirection != null)
                {
                    if (sortDirection != null && sortDirection == "desc")
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate descending
                                          select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                    else
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                }

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0, temp_debit = 0;
                for (int index = 0; index < returnAccountHistory.Count; index++)
                {
                    if (returnAccountHistory.Count > 0 && returnAccountHistory != null && returnAccountHistory[index].Description != null)
                    {
                        if (returnAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (returnAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                return balanceSheetModel;
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

        public async Task<BalanceSheetCustomReportExcelPdfModel> GetAccountHistoryForBalanceSheetCustomExcelPdfReport(string fromDate, string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                BalanceSheetCustomReportExcelPdfModel balanceSheetModel = new BalanceSheetCustomReportExcelPdfModel();

                List<GetAccountHistoryForBalanceSheetCustomExcelPdfReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetCustomExcelPdfReportModel>();
                
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetCustomSelectedDateReport_ExcelAndPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetCustomExcelPdfReportModel model = new GetAccountHistoryForBalanceSheetCustomExcelPdfReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    getAccountHistory.Add(model);
                }

                connection.Close();
                
                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < getAccountHistory.Count; index++)
                {
                    if (getAccountHistory.Count > 0 && getAccountHistory != null && getAccountHistory[index].Description != null)
                    {
                        if (getAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (getAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                balanceSheetModel.accountList = getAccountHistory;
                balanceSheetModel.totalBalance = totalBalance.ToString("0.00");
                balanceSheetModel.totalCredit = totalCredit.ToString("0.00");
                balanceSheetModel.totalDebit = totalDebit.ToString("0.00");
                return balanceSheetModel;
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

        public async Task<BalanceSheetWeeklyReportModel> GetAccountHistoryForBalanceSheetWeeklyReportTotalDetails(string fromDate, string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                decimal totalBalance = 0;
                BalanceSheetWeeklyReportModel model = new BalanceSheetWeeklyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetWeeklySelectedDateReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                totalBalance = Convert.ToDecimal(model.TotalCredit) - Convert.ToDecimal(model.TotalDebit);
                model.TotalBalance = totalBalance.ToString("0.00");
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
        public async Task<BalanceSheetWeeklyReportPaginationModel> GetAccountHistoryForBalanceSheetWeeklyReport(string fromDate, string toDate, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                BalanceSheetWeeklyReportPaginationModel balanceSheetModel = new BalanceSheetWeeklyReportPaginationModel();

                List<GetAccountHistoryForBalanceSheetWeeklyReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetWeeklyReportModel>();
                List<GetAccountHistoryForBalanceSheetWeeklyReportModel> returnAccountHistory = new List<GetAccountHistoryForBalanceSheetWeeklyReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetWeeklySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetWeeklyReportModel model = new GetAccountHistoryForBalanceSheetWeeklyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);
                    if (reader["Type"] != DBNull.Value)
                    {
                        model.Type = reader["Type"].ToString();
                        if (model.Type == "Credit")
                        {
                            model.CreditTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.DebitTotal = 0;
                        }
                        else
                        {
                            model.DebitTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.CreditTotal = 0;
                        }
                    }
                    getAccountHistory.Add(model);
                }

                connection.Close();
                if (searchPrefix != null && searchPrefix != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionIdNo == Convert.ToInt32(searchPrefix) ||
                                                data.GrandTotal == Convert.ToDecimal(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else if (searchPrefix.Contains("-"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.Date.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionId.Contains(searchPrefix) ||
                                                data.CompanyName.Contains(searchPrefix) ||
                                                data.Description.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                }
                if (sortColumn != "" && sortDirection != null)
                {
                    if (sortDirection != null && sortDirection == "desc")
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                    else
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                }

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < returnAccountHistory.Count; index++)
                {
                    if (returnAccountHistory.Count > 0 && returnAccountHistory != null && returnAccountHistory[index].Description != null)
                    {
                        if (returnAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (returnAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                return balanceSheetModel;
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
        public async Task<BalanceSheetWeeklyReportExcelPdfModel> GetAccountHistoryForBalanceSheetWeeklyExcelPdfReport(string fromDate, string toDate)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                BalanceSheetWeeklyReportExcelPdfModel balanceSheetModel = new BalanceSheetWeeklyReportExcelPdfModel();

                List<GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetCustomSelectedDateReport_ExcelAndPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportModel model = new GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    getAccountHistory.Add(model);
                }

                connection.Close();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < getAccountHistory.Count; index++)
                {
                    if (getAccountHistory.Count > 0 && getAccountHistory != null && getAccountHistory[index].Description != null)
                    {
                        if (getAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (getAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                balanceSheetModel.accountList = getAccountHistory;
                balanceSheetModel.totalBalance = totalBalance.ToString("0.00");
                balanceSheetModel.totalCredit = totalCredit.ToString("0.00");
                balanceSheetModel.totalDebit = totalDebit.ToString("0.00");
                return balanceSheetModel;
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

        public async Task<BalanceSheetMonthlyReportModel> GetAccountHistoryForBalanceSheetMonthlyReportTotalDetails(string month, string year)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                decimal totalBalance = 0;
                BalanceSheetMonthlyReportModel model = new BalanceSheetMonthlyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetMonthlySelectedDateReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                totalBalance = Convert.ToDecimal(model.TotalCredit) - Convert.ToDecimal(model.TotalDebit);
                model.TotalBalance = totalBalance.ToString("0.00");
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
        public async Task<BalanceSheetMonthlyReportPaginationModel> GetAccountHistoryForBalanceSheetMonthlyReport(string month, string year, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                BalanceSheetMonthlyReportPaginationModel balanceSheetModel = new BalanceSheetMonthlyReportPaginationModel();

                List<GetAccountHistoryForBalanceSheetMonthlyReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetMonthlyReportModel>();
                List<GetAccountHistoryForBalanceSheetMonthlyReportModel> returnAccountHistory = new List<GetAccountHistoryForBalanceSheetMonthlyReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetMonthlySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetMonthlyReportModel model = new GetAccountHistoryForBalanceSheetMonthlyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                    {
                        model.Type = reader["Type"].ToString();
                        if (model.Type == "Credit")
                        {
                            model.CreditTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.DebitTotal = 0;
                        }
                        else
                        {
                            model.DebitTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.CreditTotal = 0;
                        }
                    }
                    getAccountHistory.Add(model);
                }

                connection.Close();
                if (searchPrefix != null && searchPrefix != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionIdNo == Convert.ToInt32(searchPrefix) ||
                                                data.GrandTotal == Convert.ToDecimal(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else if (searchPrefix.Contains("-"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.Date.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionId.Contains(searchPrefix) ||
                                                data.CompanyName.Contains(searchPrefix) ||
                                                data.Description.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                }
                if (sortColumn != "" && sortDirection != null)
                {
                    if (sortDirection != null && sortDirection == "desc")
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                    else
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                }

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < returnAccountHistory.Count; index++)
                {
                    if (returnAccountHistory.Count > 0 && returnAccountHistory != null && returnAccountHistory[index].Description != null)
                    {
                        if (returnAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (returnAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                return balanceSheetModel;
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
        public async Task<BalanceSheetMonthlyReportExcelPdfModel> GetAccountHistoryForBalanceSheetMonthlyExcelPdfReport(string month, string year)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                BalanceSheetMonthlyReportExcelPdfModel balanceSheetModel = new BalanceSheetMonthlyReportExcelPdfModel();

                List<GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetMonthlySelectedDateReport_ExcelAndPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportModel model = new GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    getAccountHistory.Add(model);
                }

                connection.Close();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < getAccountHistory.Count; index++)
                {
                    if (getAccountHistory.Count > 0 && getAccountHistory != null && getAccountHistory[index].Description != null)
                    {
                        if (getAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (getAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                balanceSheetModel.accountList = getAccountHistory;
                balanceSheetModel.totalBalance = totalBalance.ToString("0.00");
                balanceSheetModel.totalCredit = totalCredit.ToString("0.00");
                balanceSheetModel.totalDebit = totalDebit.ToString("0.00");
                return balanceSheetModel;
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
        public async Task<BalanceSheetYearlyReportModel> GetAccountHistoryForBalanceSheetYearlyReportTotalDetails(string year)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                decimal totalBalance = 0;
                BalanceSheetYearlyReportModel model = new BalanceSheetYearlyReportModel();
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetYearlySelectedDateReport_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                totalBalance = Convert.ToDecimal(model.TotalCredit) - Convert.ToDecimal(model.TotalDebit);
                model.TotalBalance = totalBalance.ToString("0.00");
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
        public async Task<BalanceSheetYearlyReportPaginationModel> GetAccountHistoryForBalanceSheetYearlyReport(string year, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                BalanceSheetYearlyReportPaginationModel balanceSheetModel = new BalanceSheetYearlyReportPaginationModel();

                List<GetAccountHistoryForBalanceSheetYearlyReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetYearlyReportModel>();
                List<GetAccountHistoryForBalanceSheetYearlyReportModel> returnAccountHistory = new List<GetAccountHistoryForBalanceSheetYearlyReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetYearlySelectedDateReport_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetYearlyReportModel model = new GetAccountHistoryForBalanceSheetYearlyReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                    {
                        model.Type = reader["Type"].ToString();
                        if (model.Type == "Credit")
                        {
                            model.CreditTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.DebitTotal = 0;
                        }
                        else
                        {
                            model.DebitTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.CreditTotal = 0;
                        }
                    }
                    getAccountHistory.Add(model);
                }

                connection.Close();
                if (searchPrefix != null && searchPrefix != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionIdNo == Convert.ToInt32(searchPrefix) ||
                                                data.GrandTotal == Convert.ToDecimal(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else if (searchPrefix.Contains("-"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.Date.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionId.Contains(searchPrefix) ||
                                                data.CompanyName.Contains(searchPrefix) ||
                                                data.Description.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                }
                if (sortColumn != "" && sortDirection != null)
                {
                    if (sortDirection != null && sortDirection == "desc")
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                    else
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                }

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < returnAccountHistory.Count; index++)
                {
                    if (returnAccountHistory.Count > 0 && returnAccountHistory != null && returnAccountHistory[index].Description != null)
                    {
                        if (returnAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (returnAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                return balanceSheetModel;
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
        public async Task<BalanceSheetYearlyReportExcelPdfModel> GetAccountHistoryForBalanceSheetYearlyExcelPdfReport(string year)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                BalanceSheetYearlyReportExcelPdfModel balanceSheetModel = new BalanceSheetYearlyReportExcelPdfModel();

                List<GetAccountHistoryForBalanceSheetYearlyExcelPdfReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetYearlyExcelPdfReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetYearlySelectedDateReport_ExcelAndPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", year);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetYearlyExcelPdfReportModel model = new GetAccountHistoryForBalanceSheetYearlyExcelPdfReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    getAccountHistory.Add(model);
                }

                connection.Close();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < getAccountHistory.Count; index++)
                {
                    if (getAccountHistory.Count > 0 && getAccountHistory != null && getAccountHistory[index].Description != null)
                    {
                        if (getAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (getAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                balanceSheetModel.accountList = getAccountHistory;
                balanceSheetModel.totalBalance = totalBalance.ToString("0.00");
                balanceSheetModel.totalCredit = totalCredit.ToString("0.00");
                balanceSheetModel.totalDebit = totalDebit.ToString("0.00");
                return balanceSheetModel;
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

        public async Task<GetCompanyListForBalanceSheetPaginationModel> GetCompanyListForBalanceSheet(int skipRows, int rowsOfPage, string searchString, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalCompany = 0;
                GetCompanyListForBalanceSheetPaginationModel balanceSheet = new GetCompanyListForBalanceSheetPaginationModel();
                List<GetCompanyListForBalanceSheetModel> getCompanies = new List<GetCompanyListForBalanceSheetModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetBalanceSheetCompanyList", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", skipRows);
                command.Parameters.AddWithValue("@rowsOfPage", rowsOfPage);
                if (searchString != null && searchString != "")
                    command.Parameters.AddWithValue("@searchString", searchString);
                if (sortColumn != null && sortColumn != "")
                    command.Parameters.AddWithValue("@sortColumn", sortColumn);
                if (sortDirection != null && sortDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetCompanyListForBalanceSheetModel model = new GetCompanyListForBalanceSheetModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["CompanyAccountNumber"] != DBNull.Value)
                        model.CompanyAccountNumber = reader["CompanyAccountNumber"].ToString();

                    getCompanies.Add(model);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["CompanyCount"] != DBNull.Value)
                        totalCompany = Convert.ToInt32(reader["CompanyCount"]);
                }

                connection.Close();

                balanceSheet.totalCompany = totalCompany;
                balanceSheet.getCompanies = getCompanies;
                return balanceSheet;
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
        public async Task<BalanceSheetCustomReportByCompanyIdModel> GetBalanceSheetCustomReportTotalDetailsByCompanyId(string fromDate, string toDate,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                decimal totalBalance = 0;

                BalanceSheetCustomReportByCompanyIdModel model = new BalanceSheetCustomReportByCompanyIdModel();
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetCustomSelectedDateReportByCompanyId_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                totalBalance = Convert.ToDecimal(model.TotalCredit) - Convert.ToDecimal(model.TotalDebit);
                model.TotalBalance = totalBalance.ToString("0.00");
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
        public async Task<BalanceSheetCustomReportPaginationByCompanyIdModel> GetAccountHistoryForBalanceSheetCustomReportByCompanyId(string fromDate, string toDate,int companyId, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                BalanceSheetCustomReportPaginationByCompanyIdModel balanceSheetModel = new BalanceSheetCustomReportPaginationByCompanyIdModel();

                List<GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel>();
                List<GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel> returnAccountHistory = new List<GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetCustomSelectedDateReportByCompanyId_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel model = new GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                    {
                        model.Type = reader["Type"].ToString();
                        if (model.Type == "Credit")
                        {
                            model.CreditTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.DebitTotal = 0;
                        }
                        else
                        {
                            model.DebitTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.CreditTotal = 0;
                        }
                    }
                    getAccountHistory.Add(model);
                }

                connection.Close();
                if (searchPrefix != null && searchPrefix != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionIdNo == Convert.ToInt32(searchPrefix) ||
                                                data.GrandTotal == Convert.ToDecimal(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else if (searchPrefix.Contains("-"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.Date.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionId.Contains(searchPrefix) ||
                                                data.CompanyName.Contains(searchPrefix) ||
                                                data.Description.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                }
                if (sortColumn != "" && sortDirection != null)
                {
                    if (sortDirection != null && sortDirection == "desc")
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                    else
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                }

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < returnAccountHistory.Count; index++)
                {
                    if (returnAccountHistory.Count > 0 && returnAccountHistory != null && returnAccountHistory[index].Description != null)
                    {
                        if (returnAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (returnAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                return balanceSheetModel;
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
        public async Task<BalanceSheetCustomReportExcelPdfByCompanyIdModel> GetAccountHistoryForBalanceSheetCustomExcelPdfReportByCompanyId(string fromDate, string toDate,int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                BalanceSheetCustomReportExcelPdfByCompanyIdModel balanceSheetModel = new BalanceSheetCustomReportExcelPdfByCompanyIdModel();

                List<GetAccountHistoryForBalanceSheetCustomExcelPdfByCompanyIdReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetCustomExcelPdfByCompanyIdReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetCustomSelectedDateReportByCompanyId_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetCustomExcelPdfByCompanyIdReportModel model = new GetAccountHistoryForBalanceSheetCustomExcelPdfByCompanyIdReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    getAccountHistory.Add(model);
                }

                connection.Close();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < getAccountHistory.Count; index++)
                {
                    if (getAccountHistory.Count > 0 && getAccountHistory != null && getAccountHistory[index].Description != null)
                    {
                        if (getAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (getAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                balanceSheetModel.accountList = getAccountHistory;
                balanceSheetModel.totalBalance = totalBalance.ToString("0.00");
                balanceSheetModel.totalCredit = totalCredit.ToString("0.00");
                balanceSheetModel.totalDebit = totalDebit.ToString("0.00");
                return balanceSheetModel;
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

        public async Task<BalanceSheetWeeklyReportByCompanyIdModel> GetBalanceSheetWeeklyReportTotalDetailsByCompanyId(string fromDate, string toDate, int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string minDate = "", maxDate = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                minDate = FromDate.ToString("yyyy-MM-dd");
                maxDate = ToDate.ToString("yyyy-MM-dd");
                decimal totalBalance = 0;

                BalanceSheetWeeklyReportByCompanyIdModel model = new BalanceSheetWeeklyReportByCompanyIdModel();
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetWeeklySelectedDateReportByCompanyId_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", minDate);
                command.Parameters.AddWithValue("@ToDate", maxDate);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                totalBalance = Convert.ToDecimal(model.TotalCredit) - Convert.ToDecimal(model.TotalDebit);
                model.TotalBalance = totalBalance.ToString("0.00");
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

        public async Task<BalanceSheetWeeklyReportPaginationByCompanyIdModel> GetAccountHistoryForBalanceSheetWeeklyReportByCompanyId(string fromDate, string toDate, int companyId, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                BalanceSheetWeeklyReportPaginationByCompanyIdModel balanceSheetModel = new BalanceSheetWeeklyReportPaginationByCompanyIdModel();

                List<GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel>();
                List<GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel> returnAccountHistory = new List<GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetWeeklySelectedDateReportByCompanyId_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel model = new GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                    {
                        model.Type = reader["Type"].ToString();
                        if (model.Type == "Credit")
                        {
                            model.CreditTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.DebitTotal = 0;
                        }
                        else
                        {
                            model.DebitTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.CreditTotal = 0;
                        }
                    }
                    getAccountHistory.Add(model);
                }

                connection.Close();
                if (searchPrefix != null && searchPrefix != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionIdNo == Convert.ToInt32(searchPrefix) ||
                                                data.GrandTotal == Convert.ToDecimal(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else if (searchPrefix.Contains("-"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.Date.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionId.Contains(searchPrefix) ||
                                                data.CompanyName.Contains(searchPrefix) ||
                                                data.Description.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                }
                if (sortColumn != "" && sortDirection != null)
                {
                    if (sortDirection != null && sortDirection == "desc")
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                    else
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                }

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < returnAccountHistory.Count; index++)
                {
                    if (returnAccountHistory.Count > 0 && returnAccountHistory != null && returnAccountHistory[index].Description != null)
                    {
                        if (returnAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (returnAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                return balanceSheetModel;
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
        public async Task<BalanceSheetWeeklyReportExcelPdfByCompanyIdModel> GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportByCompanyId(string fromDate, string toDate, int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                BalanceSheetWeeklyReportExcelPdfByCompanyIdModel balanceSheetModel = new BalanceSheetWeeklyReportExcelPdfByCompanyIdModel();

                List<GetAccountHistoryForBalanceSheetWeeklyExcelPdfByCompanyIdReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetWeeklyExcelPdfByCompanyIdReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetCustomSelectedDateReportByCompanyId_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetWeeklyExcelPdfByCompanyIdReportModel model = new GetAccountHistoryForBalanceSheetWeeklyExcelPdfByCompanyIdReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    getAccountHistory.Add(model);
                }

                connection.Close();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < getAccountHistory.Count; index++)
                {
                    if (getAccountHistory.Count > 0 && getAccountHistory != null && getAccountHistory[index].Description != null)
                    {
                        if (getAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (getAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                balanceSheetModel.accountList = getAccountHistory;
                balanceSheetModel.totalBalance = totalBalance.ToString("0.00");
                balanceSheetModel.totalCredit = totalCredit.ToString("0.00");
                balanceSheetModel.totalDebit = totalDebit.ToString("0.00");
                return balanceSheetModel;
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

        public async Task<BalanceSheetMonthlyReportByCompanyIdModel> GetBalanceSheetMonthlyReportTotalDetailsByCompanyId(string month, string year, int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                decimal totalBalance = 0;

                BalanceSheetMonthlyReportByCompanyIdModel model = new BalanceSheetMonthlyReportByCompanyIdModel();
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetMonthlySelectedDateReportByCompanyId_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                totalBalance = Convert.ToDecimal(model.TotalCredit) - Convert.ToDecimal(model.TotalDebit);
                model.TotalBalance = totalBalance.ToString("0.00");
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
        public async Task<BalanceSheetMonthlyReportPaginationByCompanyIdModel> GetAccountHistoryForBalanceSheetMonthlyReportByCompanyId(string month, string year, int companyId, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                BalanceSheetMonthlyReportPaginationByCompanyIdModel balanceSheetModel = new BalanceSheetMonthlyReportPaginationByCompanyIdModel();

                List<GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel>();
                List<GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel> returnAccountHistory = new List<GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetMonthlySelectedDateReportByCompanyId_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel model = new GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                    {
                        model.Type = reader["Type"].ToString();
                        if (model.Type == "Credit")
                        {
                            model.CreditTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.DebitTotal = 0;
                        }
                        else
                        {
                            model.DebitTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.CreditTotal = 0;
                        }
                    }
                    getAccountHistory.Add(model);
                }

                connection.Close();
                if (searchPrefix != null && searchPrefix != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionIdNo == Convert.ToInt32(searchPrefix) ||
                                                data.GrandTotal == Convert.ToDecimal(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else if (searchPrefix.Contains("-"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.Date.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionId.Contains(searchPrefix) ||
                                                data.CompanyName.Contains(searchPrefix) ||
                                                data.Description.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                }
                if (sortColumn != "" && sortDirection != null)
                {
                    if (sortDirection != null && sortDirection == "desc")
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                    else
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                }

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < returnAccountHistory.Count; index++)
                {
                    if (returnAccountHistory.Count > 0 && returnAccountHistory != null && returnAccountHistory[index].Description != null)
                    {
                        if (returnAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (returnAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                return balanceSheetModel;
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
        public async Task<BalanceSheetMonthlyReportExcelPdfByCompanyIdModel> GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportByCompanyId(string month, string year, int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                BalanceSheetMonthlyReportExcelPdfByCompanyIdModel balanceSheetModel = new BalanceSheetMonthlyReportExcelPdfByCompanyIdModel();

                List<GetAccountHistoryForBalanceSheetMonthlyExcelPdfByCompanyIdReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetMonthlyExcelPdfByCompanyIdReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetMonthlySelectedDateReportByCompanyId_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetMonthlyExcelPdfByCompanyIdReportModel model = new GetAccountHistoryForBalanceSheetMonthlyExcelPdfByCompanyIdReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    getAccountHistory.Add(model);
                }

                connection.Close();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < getAccountHistory.Count; index++)
                {
                    if (getAccountHistory.Count > 0 && getAccountHistory != null && getAccountHistory[index].Description != null)
                    {
                        if (getAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (getAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                balanceSheetModel.accountList = getAccountHistory;
                balanceSheetModel.totalBalance = totalBalance.ToString("0.00");
                balanceSheetModel.totalCredit = totalCredit.ToString("0.00");
                balanceSheetModel.totalDebit = totalDebit.ToString("0.00");
                return balanceSheetModel;
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

        public async Task<BalanceSheetYearlyReportByCompanyIdModel> GetBalanceSheetYearlyReportTotalDetailsByCompanyId(string year, int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                decimal totalBalance = 0;

                BalanceSheetYearlyReportByCompanyIdModel model = new BalanceSheetYearlyReportByCompanyIdModel();
                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetYearlySelectedDateReportByCompanyId_TotalDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (reader["Credit"] != DBNull.Value)
                        model.TotalCredit = Convert.ToDecimal(reader["Credit"]).ToString("0.00");
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["Debit"] != DBNull.Value)
                        model.TotalDebit = Convert.ToDecimal(reader["Debit"]).ToString("0.00");
                }

                connection.Close();

                totalBalance = Convert.ToDecimal(model.TotalCredit) - Convert.ToDecimal(model.TotalDebit);
                model.TotalBalance = totalBalance.ToString("0.00");
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

        public async Task<BalanceSheetYearlyReportPaginationByCompanyIdModel> GetAccountHistoryForBalanceSheetYearlyReportByCompanyId(string year, int companyId, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int totalHistoryCount = 0;
                BalanceSheetYearlyReportPaginationByCompanyIdModel balanceSheetModel = new BalanceSheetYearlyReportPaginationByCompanyIdModel();

                List<GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel>();
                List<GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel> returnAccountHistory = new List<GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetYearlySelectedDateReportByCompanyId_Pagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel model = new GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                    {
                        model.Type = reader["Type"].ToString();
                        if (model.Type == "Credit")
                        {
                            model.CreditTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.DebitTotal = 0;
                        }
                        else
                        {
                            model.DebitTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            model.CreditTotal = 0;
                        }
                    }
                    getAccountHistory.Add(model);
                }

                connection.Close();
                if (searchPrefix != null && searchPrefix != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(searchPrefix, "^[0-9]*$"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionIdNo == Convert.ToInt32(searchPrefix) ||
                                                data.GrandTotal == Convert.ToDecimal(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else if (searchPrefix.Contains("-"))
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.Date.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                    else
                    {
                        returnAccountHistory = (from data in getAccountHistory
                                                where data.TransactionId.Contains(searchPrefix) ||
                                                data.CompanyName.Contains(searchPrefix) ||
                                                data.Description.Contains(searchPrefix)
                                                select data).ToList();
                        getAccountHistory = returnAccountHistory.ToList();
                        totalHistoryCount = getAccountHistory.Count();
                    }
                }
                if (sortColumn != "" && sortDirection != null)
                {
                    if (sortDirection != null && sortDirection == "desc")
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal descending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                    else
                    {
                        if (sortColumn == "Transaction Date")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionDate ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Transaction Id")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.TransactionIdNo ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Company")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Description")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CompanyName ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Credit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.CreditTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                        else if (sortColumn == "Debit(AED)")
                        {
                            returnAccountHistory = (from data in getAccountHistory
                                                    orderby data.DebitTotal ascending
                                                    select data).ToList();
                            var getHistoryOfCompanyExpense = returnAccountHistory.Skip(skipRows).Take(rowsOfPage).ToList();
                            balanceSheetModel.accountList = getHistoryOfCompanyExpense;
                            balanceSheetModel.totalAccountEntry = totalHistoryCount != 0 ? totalHistoryCount : returnAccountHistory.Count();
                        }
                    }
                }

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < returnAccountHistory.Count; index++)
                {
                    if (returnAccountHistory.Count > 0 && returnAccountHistory != null && returnAccountHistory[index].Description != null)
                    {
                        if (returnAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (returnAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(returnAccountHistory[index].GrandTotal);

                            returnAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                return balanceSheetModel;
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

        public async Task<BalanceSheetYearlyReportExcelPdfByCompanyIdModel> GetAccountHistoryForBalanceSheetYearlyExcelPdfReportByCompanyId(string year, int companyId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                BalanceSheetYearlyReportExcelPdfByCompanyIdModel balanceSheetModel = new BalanceSheetYearlyReportExcelPdfByCompanyIdModel();

                List<GetAccountHistoryForBalanceSheetYearlyExcelPdfByCompanyIdReportModel> getAccountHistory = new List<GetAccountHistoryForBalanceSheetYearlyExcelPdfByCompanyIdReportModel>();

                SqlCommand command = new SqlCommand("USP_Admin_BalanceSheetYearlySelectedDateReportByCompanyId_ExcelPdf", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@CompanyId", companyId);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAccountHistoryForBalanceSheetYearlyExcelPdfByCompanyIdReportModel model = new GetAccountHistoryForBalanceSheetYearlyExcelPdfByCompanyIdReportModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["TransactionId"] != DBNull.Value)
                        model.TransactionId = reader["TransactionId"].ToString();
                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();
                    if (reader["Description"] != DBNull.Value)
                        model.Description = reader["Description"].ToString();
                    if (reader["TransactionIdNo"] != DBNull.Value)
                        model.TransactionIdNo = Convert.ToInt32(reader["TransactionIdNo"]);
                    if (reader["GrandTotal"] != DBNull.Value)
                        model.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                    if (reader["Date"] != DBNull.Value)
                    {
                        model.Date = reader["Date"].ToString();
                        //var dateArray = model.Date.Split("-");
                        //string reverseDate = "";
                        //for (int index = dateArray.Length - 1; index >= 0; index--)
                        //{
                        //    reverseDate += dateArray[index];
                        //}
                        //model.ReverseDate = reverseDate;
                    }
                    if (reader["TransactionDate"] != DBNull.Value)
                        model.TransactionDate = Convert.ToDateTime(reader["TransactionDate"]);

                    if (reader["Type"] != DBNull.Value)
                        model.Type = reader["Type"].ToString();
                    getAccountHistory.Add(model);
                }

                connection.Close();

                decimal totalCredit = 0, totalDebit = 0, totalBalance = 0;
                for (int index = 0; index < getAccountHistory.Count; index++)
                {
                    if (getAccountHistory.Count > 0 && getAccountHistory != null && getAccountHistory[index].Description != null)
                    {
                        if (getAccountHistory[index].Type == "Credit")
                        {
                            totalCredit = totalCredit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                        if (getAccountHistory[index].Type == "Debit")
                        {
                            totalDebit = totalDebit + Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            totalBalance = totalBalance - Convert.ToDecimal(getAccountHistory[index].GrandTotal);

                            getAccountHistory[index].BalanceTotal = Convert.ToDecimal(totalBalance.ToString("0.00"));
                        }
                    }
                }

                balanceSheetModel.accountList = getAccountHistory;
                balanceSheetModel.totalBalance = totalBalance.ToString("0.00");
                balanceSheetModel.totalCredit = totalCredit.ToString("0.00");
                balanceSheetModel.totalDebit = totalDebit.ToString("0.00");
                return balanceSheetModel;
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

        public async Task<List<GetClearedCompanyListModelForExcel>> GetClearedCompanyListForExcel()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetClearedCompanyListModelForExcel> clearedCompany = new List<GetClearedCompanyListModelForExcel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetProfitCompanyListReport", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetClearedCompanyListModelForExcel model = new GetClearedCompanyListModelForExcel();

                    if (reader["AccountNumber"] != DBNull.Value)
                        model.AccountNumber = reader["AccountNumber"].ToString();

                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();

                    if (reader["PortalBalance"] != DBNull.Value)
                        model.CompanyPortalBalance = Convert.ToDecimal(reader["PortalBalance"]);

                    if (reader["CompanyOwner"] != DBNull.Value)
                        model.CompanyOwner = reader["CompanyOwner"].ToString();

                    if (reader["Email"] != DBNull.Value)
                        model.Email = reader["Email"].ToString();

                    clearedCompany.Add(model);
                }

                connection.Close();

                return clearedCompany;
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

        public async Task<List<GetOverdueCompanyListModelForExcel>> GetOverdueCompanyListForExcel()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetOverdueCompanyListModelForExcel> clearedCompany = new List<GetOverdueCompanyListModelForExcel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetLossCompanyListReport", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    GetOverdueCompanyListModelForExcel model = new GetOverdueCompanyListModelForExcel();

                    if (reader["AccountNumber"] != DBNull.Value)
                        model.AccountNumber = reader["AccountNumber"].ToString();

                    if (reader["CompanyName"] != DBNull.Value)
                        model.CompanyName = reader["CompanyName"].ToString();

                    if (reader["PortalBalance"] != DBNull.Value)
                        model.CompanyPortalBalance = Convert.ToDecimal(reader["PortalBalance"]);

                    if (reader["CompanyOwner"] != DBNull.Value)
                        model.CompanyOwner = reader["CompanyOwner"].ToString();

                    if (reader["Email"] != DBNull.Value)
                        model.Email = reader["Email"].ToString();

                    clearedCompany.Add(model);
                }

                connection.Close();

                return clearedCompany;
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
        public async Task<GetProfitLossGraphReportForDashboardModel> GetProfitLossGraphReportForDashboard()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetProfitLossGraphReportForDashboardModel model = new GetProfitLossGraphReportForDashboardModel();
                decimal creditAmount = 0, debitAmount = 0;

                SqlCommand command = new SqlCommand("USP_Admin_Dashboard_BalanceSheetReport", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["PortalBalance"] != DBNull.Value)
                        creditAmount += Convert.ToDecimal(reader["PortalBalance"]);
                }

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    if (reader["PortalBalance"] != DBNull.Value)
                        debitAmount += Convert.ToDecimal(reader["PortalBalance"]);
                }

                connection.Close();

                model.CreditedAmount = creditAmount;
                model.DeditedAmount = debitAmount;
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

        public async Task SaveDownloadDocumentData(SaveDownloadDocumentModel model)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = new SqlCommand("USP_Admin_Insert_DownloadDocument",connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FileName", model.FileName);
                command.Parameters.AddWithValue("@FileExtension", model.FileExtension);
                command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                command.Parameters.AddWithValue("@ActionMethod", model.ActionMethod);
                command.Parameters.AddWithValue("@Controller", model.Controller);
                command.Parameters.AddWithValue("@FromDate", model.FromDate);
                command.Parameters.AddWithValue("@ToDate", model.ToDate);
                command.Parameters.AddWithValue("@CreatedBy", model.createdBy);
                command.Parameters.AddWithValue("@SendedStatus", "Pending");
                if(model.Month !="" && model.Month !=null)
                    command.Parameters.AddWithValue("@Month", model.Month);
                if (model.Year != "" && model.Year != null)
                    command.Parameters.AddWithValue("@Year", model.Year);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
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
        public async Task<string> SendCompanyAccountFileOnEmail(string filePath, string fileName, string Email, string fromDate, string toDate, string? company,string? FileType,int createdBy,string username)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            SqlCommand command = new SqlCommand("USP_Admin_Insert_DownloadDocument", connection);
            command.CommandType = CommandType.StoredProcedure;
            try
            {
                string currentFileName = "";
                DateTime FromDate = Convert.ToDateTime(fromDate), ToDate = Convert.ToDateTime(toDate);
                string minDate = FromDate.ToString("dd-MM-yyyy");
                string maxDate = ToDate.ToString("dd-MM-yyyy");
                if (FileType == "Pdf")
                {
                    currentFileName = fileName.Replace(".pdf", ".zip");
                }
                else
                {
                    currentFileName = fileName.Replace(".xlsx", ".zip");
                }
                string responseText = "Something went wrong while sending email , please try again later.";
                var message = new MailMessage();    
                message.To.Add(new MailAddress(Email));
                message.From = new MailAddress("thebridgecode.yashasvi@gmail.com");
                message.Subject = "DIBN - Account Summary From " + minDate + " to " + maxDate;
                string _MessageBody = "Please find attachment of Account Summary "+ FileType + " file of DIBN from " + minDate + " to " + maxDate +" generated by <b>"+username+"</b>";
                StringBuilder builder = new StringBuilder();
                builder.Append("<p>" + _MessageBody + "</p>");
                FileStream fRead = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Net.Mail.Attachment attachment;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        
                            FileInfo file = new FileInfo(filePath);
                            ZipArchiveEntry zipItem = archive.CreateEntry(file.Name + file.Extension);
                            // Add file to zipItem.
                            using (MemoryStream msFile = new MemoryStream(System.IO.File.ReadAllBytes(filePath)))
                            {
                                using (Stream stream = zipItem.Open())
                                {
                                    msFile.CopyTo(stream);
                            }
                            }
                        attachment = new Attachment(new MemoryStream(memoryStream.ToArray()), currentFileName, MediaTypeNames.Application.Zip);
                        Debug.WriteLine(attachment.ContentStream.Length);
                    }
                    message.Attachments.Add(attachment);
                }
                //message.Attachments.Add(attachment);
                message.IsBodyHtml = true;
                message.Body = builder.ToString();
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential()
                    {
                        UserName = "thebridgecode.yashasvi@gmail.com",
                        Password = "rfqtstsochmmkwnj"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    responseText = "Email has been send successfully.";
                    fRead.Close();

                    //command.Parameters.AddWithValue("@FileName", fileName);
                    //command.Parameters.AddWithValue("@CreatedBy", createdBy);
                    //command.Parameters.AddWithValue("@SendedStatus", "Success");
                    //connection.Open();
                    //await command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return responseText;
            }
            catch(Exception ex)
            {
                //command.Parameters.AddWithValue("@FileName", fileName);
                //command.Parameters.AddWithValue("@CreatedBy", createdBy);
                //command.Parameters.AddWithValue("@SendedStatus", "Fail");
                //connection.Open();
                //await command.ExecuteNonQueryAsync();
                //connection.Close();
                using (MemoryStream stream = new MemoryStream())
                {
                    string path = "wwwroot/companyPdfExcel/" + fileName;
                    System.IO.File.Delete(path);
                }
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<List<GetDownloadDocumentListModel>> GetDownloadDocumentList()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetDownloadDocumentListModel> mainList = new List<GetDownloadDocumentListModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetDownloadDocumentList", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetDownloadDocumentListModel model = new GetDownloadDocumentListModel();
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["FileName"] != DBNull.Value)
                        model.FileName = reader["FileName"].ToString();
                    if (reader["EmailAddress"] != DBNull.Value)
                        model.EmailAddress = reader["EmailAddress"].ToString();
                    if (reader["FromDate"] != DBNull.Value)
                    {
                        model.FromDate = Convert.ToDateTime(reader["FromDate"]).ToString("yyyy-MM-dd");
                        model.minDate = Convert.ToDateTime(model.FromDate).ToString("dd-MM-yyyy");
                    }
                        
                    if (reader["ToDate"] != DBNull.Value)
                    {
                        model.ToDate = Convert.ToDateTime(reader["ToDate"]).ToString("yyyy-MM-dd");
                        model.maxDate = Convert.ToDateTime(model.ToDate).ToString("dd-MM-yyyy");
                    }
                    if (reader["CreatedOnUtc"] != DBNull.Value)
                        model.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    if (reader["CreatedBy"] != DBNull.Value)
                        model.CreatedBy = reader["CreatedBy"].ToString();
                    if (reader["SendedStatus"] != DBNull.Value)
                        model.SendedStatus = reader["SendedStatus"].ToString();
                    mainList.Add(model);
                }
                connection.Close();
                return mainList;
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
    }
}
