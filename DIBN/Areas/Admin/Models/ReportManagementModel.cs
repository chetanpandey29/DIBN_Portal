using DocumentFormat.OpenXml.Office.CoverPageProps;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class ProfitLossCustomReportModel 
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class ProfitLossCustomReportExcelAndPdfModel
    {
        public ProfitLossCustomReportExcelAndPdfModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCustomReportModel>();
        }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
        public List<GetAccountHistoryForProfitLossCustomReportModel> accountList { get; set; }
    }
    public class ProfitLossCustomReportPaginationModel
    {
        public ProfitLossCustomReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCustomReportModel>();
        }
        public List<GetAccountHistoryForProfitLossCustomReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForProfitLossCustomReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
    }

    public class ProfitLossWeeklyReportModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class GetAccountHistoryForProfitLossWeeklyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
    }
    public class ProfitLossWeeklyReportPaginationModel
    {
        public ProfitLossWeeklyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossWeeklyReportModel>();
        }
        public List<GetAccountHistoryForProfitLossWeeklyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class ProfitLossWeeklyReportExcelAndPdfModel
    {
        public ProfitLossWeeklyReportExcelAndPdfModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossWeeklyReportModel>();
        }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
        public List<GetAccountHistoryForProfitLossWeeklyReportModel> accountList { get; set; }
    }

    public class ProfitLossMonthlyReportModel
    {
        public string MonthNumber { get; set; }
        public string MonthText { get; set; }
        public string YearNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class ProfitLossMonthlyReportExcelAndPdfModel
    {
        public ProfitLossMonthlyReportExcelAndPdfModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossMonthlyReportModel>();
        }
        public string monthNumber { get; set; }
        public string monthText { get; set; }
        public string yearNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
        public List<GetAccountHistoryForProfitLossMonthlyReportModel> accountList { get; set; }
    }
    public class ProfitLossMonthlyReportPaginationModel
    {
        public ProfitLossMonthlyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossMonthlyReportModel>();
        }
        public List<GetAccountHistoryForProfitLossMonthlyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForProfitLossMonthlyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
    }

    public class ProfitLossYearlyReportModel
    {
        public string YearNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }

    public class ProfitLossYearlyReportExcelAndPdfModel
    {
        public ProfitLossYearlyReportExcelAndPdfModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossYearlyReportModel>();
        }
        public string yearNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
        public List<GetAccountHistoryForProfitLossYearlyReportModel> accountList { get; set; }
    }
    public class ProfitLossYearlyReportPaginationModel
    {
        public ProfitLossYearlyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossYearlyReportModel>();
        }
        public List<GetAccountHistoryForProfitLossYearlyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForProfitLossYearlyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
    }

    public class GetAccountHistoryForProfitLossCustomReportGraphModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
    }
    public class GetAccountHistoryForProfitLossCustomReportGraphDataModel
    {
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; }
    }

    public class GetAccountHistoryForProfitLossWeeklyReportGraphModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
    }
    public class GetAccountHistoryForProfitLossWeeklyReportGraphDataModel
    {
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; }
    }

    public class GetAccountHistoryForProfitLossYearlyReportGraphModel
    {
        public string year { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
    }
    public class GetAccountHistoryForProfitLossYearlyReportGraphDataModel
    {
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionMonth { get; set; }
        public string Type { get; set; }
    }

    public class GetAccountHistoryForProfitLossMonthlyReportGraphModel
    {
        public string year { get; set; }
        public string month { get; set; }
        public string monthName { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
    }
    public class GetAccountHistoryForProfitLossMonthlyReportGraphDataModel
    {
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; }
    }

    public class GetCompanyListForProfitLossPagination
    {
        public List<GetCompanyListForProfitLossModel> companyListForProfitLossModels { get; set; }
        public int totalCompany { get; set; }
    }
    public class GetCompanyListForProfitLossModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAccountNumber { get; set; }
    }

    public class ProfitLossCompanyCustomReportModel
    {
        public int companyId { get; set; }
        public string CompanyName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class GetAccountHistoryForProfitLossCompanyCustomReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
    }
    public class ProfitLossCompanyCustomReportExcelAndPdfModel
    {
        public ProfitLossCompanyCustomReportExcelAndPdfModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCompanyCustomReportModel>();
        }
        public int companyId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
        public List<GetAccountHistoryForProfitLossCompanyCustomReportModel> accountList { get; set; }
    }
    public class ProfitLossCompanyCustomReportPaginationModel
    {
        public ProfitLossCompanyCustomReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCompanyCustomReportModel>();
        }
        public List<GetAccountHistoryForProfitLossCompanyCustomReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }

    public class ProfitLossCompanyWeeklyReportModel
    {
        public int companyId { get; set; }
        public string CompanyName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class GetAccountHistoryForProfitLossCompanyWeeklyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
    }
    public class ProfitLossCompanyWeeklyReportPaginationModel
    {
        public ProfitLossCompanyWeeklyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCompanyWeeklyReportModel>();
        }
        public List<GetAccountHistoryForProfitLossCompanyWeeklyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class ProfitLossCompanyWeeklyReportExcelAndPdfModel
    {
        public ProfitLossCompanyWeeklyReportExcelAndPdfModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCompanyWeeklyReportModel>();
        }
        public int companyId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
        public List<GetAccountHistoryForProfitLossCompanyWeeklyReportModel> accountList { get; set; }
    }

    public class ProfitLossCompanyMonthlyReportModel
    {
        public int companyId { get; set; }
        public string CompanyName { get; set; }
        public string MonthNumber { get; set; }
        public string MonthText { get; set; }
        public string YearNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class ProfitLossCompanyMonthlyReportExcelAndPdfModel
    {
        public ProfitLossCompanyMonthlyReportExcelAndPdfModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCompanyMonthlyReportModel>();
        }
        public int companyId { get; set; }
        public string monthNumber { get; set; }
        public string monthText { get; set; }
        public string yearNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
        public List<GetAccountHistoryForProfitLossCompanyMonthlyReportModel> accountList { get; set; }
    }
    public class ProfitLossCompanyMonthlyReportPaginationModel
    {
        public ProfitLossCompanyMonthlyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCompanyMonthlyReportModel>();
        }
        public List<GetAccountHistoryForProfitLossCompanyMonthlyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForProfitLossCompanyMonthlyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
    }

    public class ProfitLossCompanyYearlyReportModel
    {
        public int companyId { get; set; }
        public string CompanyName { get; set; }
        public string YearNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class ProfitLossCompanyYearlyReportExcelAndPdfModel
    {
        public ProfitLossCompanyYearlyReportExcelAndPdfModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCompanyYearlyReportModel>();
        }
        public int companyId { get; set; }
        public string yearNumber { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
        public List<GetAccountHistoryForProfitLossCompanyYearlyReportModel> accountList { get; set; }
    }
    public class ProfitLossCompanyYearlyReportPaginationModel
    {
        public ProfitLossCompanyYearlyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForProfitLossCompanyYearlyReportModel>();
        }
        public List<GetAccountHistoryForProfitLossCompanyYearlyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForProfitLossCompanyYearlyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
    }

    public class GetAccountHistoryForProfitLossCompanyCustomReportGraphModel
    {
        public int companyId { get; set; }
        public string CompanyName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
    }
    public class GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel
    {
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; }
    }

    public class GetAccountHistoryForProfitLossCompanyWeeklyReportGraphModel
    {
        public int companyId { get; set; }
        public string CompanyName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
    }
    public class GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel
    {
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; }
    }

    public class GetAccountHistoryForProfitLossCompanyYearlyReportGraphModel
    {
        public string year { get; set; }
        public int companyId { get; set; }
        public string CompanyName { get; set;}
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
    }
    public class GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel
    {
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionMonth { get; set; }
        public string Type { get; set; }
    }

    public class GetAccountHistoryForProfitLossCompanyMonthlyReportGraphModel
    {
        public int companyId { get; set; }
        public string CompanyName { get; set;}
        public string year { get; set; }
        public string month { get; set; }
        public string monthName { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public double totalProfitLoss { get; set; }
        public double profitLossPercentage { get; set; }
    }
    public class GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel
    {
        public string GrandTotal { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; }
    }

    public class GetMultipleSelectedCompanyProfitLossReportModel
    {
        public GetMultipleSelectedCompanyProfitLossReportModel()
        {
            companyIds = new List<int>();
        }
        public List<int> companyIds { get; set; }
    }


    public class BalanceSheetCustomReportModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class BalanceSheetCustomReportPaginationModel
    {
        public BalanceSheetCustomReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetCustomReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetCustomReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetCustomReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal DebitTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class BalanceSheetCustomReportExcelPdfModel
    {
        public BalanceSheetCustomReportExcelPdfModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetCustomExcelPdfReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetCustomExcelPdfReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string totalCredit { get; set; }
        public string totalDebit { get; set; }
        public string totalBalance { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetCustomExcelPdfReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    
    public class BalanceSheetWeeklyReportModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class BalanceSheetWeeklyReportPaginationModel
    {
        public BalanceSheetWeeklyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetWeeklyReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetWeeklyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetWeeklyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal DebitTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class BalanceSheetWeeklyReportExcelPdfModel
    {
        public BalanceSheetWeeklyReportExcelPdfModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string totalCredit { get; set; }
        public string totalDebit { get; set; }
        public string totalBalance { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class BalanceSheetMonthlyReportModel
    {
        public string monthName { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class BalanceSheetMonthlyReportPaginationModel
    {
        public BalanceSheetMonthlyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetMonthlyReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetMonthlyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetMonthlyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal DebitTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class BalanceSheetMonthlyReportExcelPdfModel
    {
        public BalanceSheetMonthlyReportExcelPdfModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
        public string monthName { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string totalCredit { get; set; }
        public string totalDebit { get; set; }
        public string totalBalance { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    
    public class BalanceSheetYearlyReportModel
    {
        public string year { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class BalanceSheetYearlyReportPaginationModel
    {
        public BalanceSheetYearlyReportPaginationModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetYearlyReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetYearlyReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetYearlyReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal DebitTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class BalanceSheetYearlyReportExcelPdfModel
    {
        public BalanceSheetYearlyReportExcelPdfModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetYearlyExcelPdfReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetYearlyExcelPdfReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
        public string monthName { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string totalCredit { get; set; }
        public string totalDebit { get; set; }
        public string totalBalance { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetYearlyExcelPdfReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class GetCompanyListForBalanceSheetPaginationModel
    {
        public GetCompanyListForBalanceSheetPaginationModel()
        {
            getCompanies = new List<GetCompanyListForBalanceSheetModel>();
        }
        public List<GetCompanyListForBalanceSheetModel> getCompanies { get; set; }
        public int totalCompany { get; set; }
    }

    public class GetCompanyListForBalanceSheetModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAccountNumber { get; set; }
    }


    public class BalanceSheetCustomReportByCompanyIdModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class BalanceSheetCustomReportPaginationByCompanyIdModel
    {
        public BalanceSheetCustomReportPaginationByCompanyIdModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetCustomByCompanyIdReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal DebitTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class BalanceSheetCustomReportExcelPdfByCompanyIdModel
    {
        public BalanceSheetCustomReportExcelPdfByCompanyIdModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetCustomExcelPdfByCompanyIdReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetCustomExcelPdfByCompanyIdReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string totalCredit { get; set; }
        public string totalDebit { get; set; }
        public string totalBalance { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetCustomExcelPdfByCompanyIdReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class BalanceSheetWeeklyReportByCompanyIdModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromDates { get; set; }
        public string ToDates { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class BalanceSheetWeeklyReportPaginationByCompanyIdModel
    {
        public BalanceSheetWeeklyReportPaginationByCompanyIdModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetWeeklyByCompanyIdReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal DebitTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class BalanceSheetWeeklyReportExcelPdfByCompanyIdModel
    {
        public BalanceSheetWeeklyReportExcelPdfByCompanyIdModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetWeeklyExcelPdfByCompanyIdReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetWeeklyExcelPdfByCompanyIdReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string totalCredit { get; set; }
        public string totalDebit { get; set; }
        public string totalBalance { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetWeeklyExcelPdfByCompanyIdReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class BalanceSheetMonthlyReportByCompanyIdModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string monthName { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class BalanceSheetMonthlyReportPaginationByCompanyIdModel
    {
        public BalanceSheetMonthlyReportPaginationByCompanyIdModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetMonthlyByCompanyIdReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal DebitTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class BalanceSheetMonthlyReportExcelPdfByCompanyIdModel
    {
        public BalanceSheetMonthlyReportExcelPdfByCompanyIdModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetMonthlyExcelPdfByCompanyIdReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetMonthlyExcelPdfByCompanyIdReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string monthName { get; set; }
        public string totalCredit { get; set; }
        public string totalDebit { get; set; }
        public string totalBalance { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetMonthlyExcelPdfByCompanyIdReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class BalanceSheetYearlyReportByCompanyIdModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string year { get; set; }
        public string TotalCredit { get; set; }
        public string TotalDebit { get; set; }
        public string TotalBalance { get; set; }
        public double totalCredit { get; set; }
        public double totalDebit { get; set; }
    }
    public class BalanceSheetYearlyReportPaginationByCompanyIdModel
    {
        public BalanceSheetYearlyReportPaginationByCompanyIdModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetYearlyByCompanyIdReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal DebitTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class BalanceSheetYearlyReportExcelPdfByCompanyIdModel
    {
        public BalanceSheetYearlyReportExcelPdfByCompanyIdModel()
        {
            accountList = new List<GetAccountHistoryForBalanceSheetYearlyExcelPdfByCompanyIdReportModel>();
        }
        public List<GetAccountHistoryForBalanceSheetYearlyExcelPdfByCompanyIdReportModel> accountList { get; set; }
        public int totalAccountEntry { get; set; }
        public string year { get; set; }
        public string totalCredit { get; set; }
        public string totalDebit { get; set; }
        public string totalBalance { get; set; }
    }
    public class GetAccountHistoryForBalanceSheetYearlyExcelPdfByCompanyIdReportModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int TransactionIdNo { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Date { get; set; }
        public string ReverseDate { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class GetClearedCompanyListModelForExcel
    {
        public string AccountNumber { get; set; }
        public string CompanyName { get; set;}
        public decimal CompanyPortalBalance { get;set; }
        public string CompanyOwner { get; set; }
        public string Email { get; set; }
    }

    public class GetOverdueCompanyListModelForExcel
    {
        public string AccountNumber { get; set; }
        public string CompanyName { get; set; }
        public decimal CompanyPortalBalance { get; set; }
        public string CompanyOwner { get; set; }
        public string Email { get; set; }
    }

    public class GetProfitLossGraphReportForDashboardModel
    {
        public decimal CreditedAmount { get;set; }
        public decimal DeditedAmount { get;set;}
    }
    public class SaveDownloadDocumentModel
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string EmailAddress { get; set; }
        public string ActionMethod { get; set; }
        public string Controller { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public int createdBy {  get; set; }
    }
    public class GetDownloadDocumentListModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string EmailAddress { get; set; }
        public string FromDate { get; set; }
        public string minDate { get; set; }
        public string ToDate { get; set; }
        public string maxDate { get; set; }
        public string CreatedOnUtc { get; set; }
        public string CreatedBy { get; set; }
        public string SendedStatus { get; set; }
    }
}

