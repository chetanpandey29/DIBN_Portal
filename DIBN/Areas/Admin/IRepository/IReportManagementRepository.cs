using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IReportManagementRepository
    {
        Task<ProfitLossCustomReportModel> GetAccountHistoryForProfitLossCustomReportTotalDetails(string fromDate, string toDate);
        Task<ProfitLossCustomReportPaginationModel> GetAccountHistoryForProfitLossCustomReport(string fromDate, string toDate, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection);
        Task<ProfitLossCustomReportExcelAndPdfModel> GetAccountHistoryForProfitLossCustomReportExcelPdf(string fromDate, string toDate);
        Task<ProfitLossWeeklyReportModel> GetAccountHistoryForProfitLossWeeklyReportTotalDetails(string fromDate, string toDate);
        Task<ProfitLossWeeklyReportPaginationModel> GetAccountHistoryForProfitLossWeeklyReport(string fromDate, string toDate, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection);
        Task<ProfitLossWeeklyReportExcelAndPdfModel> GetAccountHistoryForProfitLossWeeklyReportExcelPdf(string fromDate, string toDate);
        Task<ProfitLossMonthlyReportModel> GetAccountHistoryForProfitLossMonthlyReportTotalDetails(string monthNumber, string yearNumber);
        Task<ProfitLossMonthlyReportExcelAndPdfModel> GetAccountHistoryForProfitLossMonthlyReportExcelPdf(string month, string year);
        Task<ProfitLossMonthlyReportPaginationModel> GetAccountHistoryForProfitLossMonthlyReport(string month, string year, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection);
        Task<ProfitLossYearlyReportModel> GetAccountHistoryForProfitLossYearlyReportTotalDetails(string yearNumber);
        Task<ProfitLossYearlyReportExcelAndPdfModel> GetAccountHistoryForProfitLossYearlyReportExcelPdf(string year);
        Task<ProfitLossYearlyReportPaginationModel> GetAccountHistoryForProfitLossYearlyReport(string year, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection);
        Task<List<GetAccountHistoryForProfitLossCustomReportGraphDataModel>> GetAccountHistoryForProfitLossCustomReportGraph(string fromDate, string toDate);
        Task<List<GetAccountHistoryForProfitLossWeeklyReportGraphDataModel>> GetAccountHistoryForProfitLossWeeklyReportGraph(string fromDate, string toDate);
        Task<List<GetAccountHistoryForProfitLossYearlyReportGraphDataModel>> GetAccountHistoryForProfitLossYealyReportGraph(string year);
        Task<List<GetAccountHistoryForProfitLossMonthlyReportGraphDataModel>> GetAccountHistoryForProfitLossMonthlyReportGraph(string year, string month);
        Task<GetCompanyListForProfitLossPagination> GetCompanyListForProfitLoss(int skipRows, int rowsOfPage, string searchString, string sortColumn, string sortDirection);
        Task<ProfitLossCompanyCustomReportModel> GetAccountHistoryForProfitLossCompanyCustomReportTotalDetails(string fromDate, string toDate, int companyId);
        Task<ProfitLossCompanyCustomReportPaginationModel> GetAccountHistoryForProfitLossCompanyCustomReport(string fromDate, string toDate, int companyId, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection);
        Task<ProfitLossCompanyCustomReportExcelAndPdfModel> GetAccountHistoryForProfitLossCompanyCustomReportExcelPdf(string fromDate, string toDate, int companyId);
        Task<ProfitLossCompanyWeeklyReportModel> GetAccountHistoryForProfitLossCompanyWeeklyReportTotalDetails(string fromDate, string toDate, int companyId);
        Task<ProfitLossCompanyWeeklyReportExcelAndPdfModel> GetAccountHistoryForProfitLossCompanyWeeklyReportExcelPdf(string fromDate, string toDate, int companyId);
        Task<ProfitLossCompanyWeeklyReportPaginationModel> GetAccountHistoryForProfitLossCompanyWeeklyReport(string fromDate, string toDate, int companyId, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection);
        Task<ProfitLossCompanyMonthlyReportModel> GetAccountHistoryForProfitLossCompanyMonthlyReportTotalDetails(string monthNumber, string yearNumber, int companyId);
        Task<ProfitLossCompanyMonthlyReportExcelAndPdfModel> GetAccountHistoryForProfitLossCompanyMonthlyReportExcelPdf(string month, string year, int companyId);
        Task<ProfitLossCompanyMonthlyReportPaginationModel> GetAccountHistoryForProfitLossCompanyMonthlyReport(string month, string year, int companyId, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection);
        Task<ProfitLossCompanyYearlyReportModel> GetAccountHistoryForProfitLossCompanyYearlyReportTotalDetails(string yearNumber, int companyId);
        Task<ProfitLossCompanyYearlyReportExcelAndPdfModel> GetAccountHistoryForProfitLossCompanyYearlyReportExcelPdf(string year, int companyId);
        Task<ProfitLossCompanyYearlyReportPaginationModel> GetAccountHistoryForProfitLossCompanyYearlyReport(string year, int companyId, int skipRows, int rowsOfPage,
            string? searchPrefix, string sortColumn, string sortDirection);
        Task<List<GetAccountHistoryForProfitLossCompanyCustomReportGraphDataModel>> GetAccountHistoryForProfitLossCompanyCustomReportGraph(string fromDate, string toDate, int companyId);
        Task<List<GetAccountHistoryForProfitLossCompanyWeeklyReportGraphDataModel>> GetAccountHistoryForProfitLossCompanyWeeklyReportGraph(string fromDate, string toDate, int companyId);
        Task<List<GetAccountHistoryForProfitLossCompanyYearlyReportGraphDataModel>> GetAccountHistoryForProfitLossCompanyYealyReportGraph(string year, int companyId);
        Task<List<GetAccountHistoryForProfitLossCompanyMonthlyReportGraphDataModel>> GetAccountHistoryForProfitLossCompanyMonthlyReportGraph(string year, string month, int companyId);
        Task<BalanceSheetCustomReportModel> GetAccountHistoryForBalanceSheetCustomReportTotalDetails(string fromDate, string toDate);
        Task<BalanceSheetCustomReportPaginationModel> GetAccountHistoryForBalanceSheetCustomReport(string fromDate, string toDate, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection);
        Task<BalanceSheetCustomReportExcelPdfModel> GetAccountHistoryForBalanceSheetCustomExcelPdfReport(string fromDate, string toDate);
        Task<BalanceSheetWeeklyReportModel> GetAccountHistoryForBalanceSheetWeeklyReportTotalDetails(string fromDate, string toDate);
        Task<BalanceSheetWeeklyReportPaginationModel> GetAccountHistoryForBalanceSheetWeeklyReport(string fromDate, string toDate, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection);
        Task<BalanceSheetWeeklyReportExcelPdfModel> GetAccountHistoryForBalanceSheetWeeklyExcelPdfReport(string fromDate, string toDate);
        Task<BalanceSheetMonthlyReportModel> GetAccountHistoryForBalanceSheetMonthlyReportTotalDetails(string month, string year);
        Task<BalanceSheetMonthlyReportPaginationModel> GetAccountHistoryForBalanceSheetMonthlyReport(string month, string year, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection);
        Task<BalanceSheetMonthlyReportExcelPdfModel> GetAccountHistoryForBalanceSheetMonthlyExcelPdfReport(string month, string year);
        Task<BalanceSheetYearlyReportModel> GetAccountHistoryForBalanceSheetYearlyReportTotalDetails(string year);
        Task<BalanceSheetYearlyReportPaginationModel> GetAccountHistoryForBalanceSheetYearlyReport(string year, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection);
        Task<BalanceSheetYearlyReportExcelPdfModel> GetAccountHistoryForBalanceSheetYearlyExcelPdfReport(string year);
        Task<GetCompanyListForBalanceSheetPaginationModel> GetCompanyListForBalanceSheet(int skipRows, int rowsOfPage, string searchString, string sortColumn, string sortDirection);
        Task<BalanceSheetCustomReportByCompanyIdModel> GetBalanceSheetCustomReportTotalDetailsByCompanyId(string fromDate, string toDate, int companyId);
        Task<BalanceSheetCustomReportPaginationByCompanyIdModel> GetAccountHistoryForBalanceSheetCustomReportByCompanyId(string fromDate, string toDate, int companyId, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection);
        Task<BalanceSheetCustomReportExcelPdfByCompanyIdModel> GetAccountHistoryForBalanceSheetCustomExcelPdfReportByCompanyId(string fromDate, string toDate, int companyId);
        Task<BalanceSheetWeeklyReportByCompanyIdModel> GetBalanceSheetWeeklyReportTotalDetailsByCompanyId(string fromDate, string toDate, int companyId);
        Task<BalanceSheetWeeklyReportPaginationByCompanyIdModel> GetAccountHistoryForBalanceSheetWeeklyReportByCompanyId(string fromDate, string toDate, int companyId, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection);
        Task<BalanceSheetWeeklyReportExcelPdfByCompanyIdModel> GetAccountHistoryForBalanceSheetWeeklyExcelPdfReportByCompanyId(string fromDate, string toDate, int companyId);
        Task<BalanceSheetMonthlyReportByCompanyIdModel> GetBalanceSheetMonthlyReportTotalDetailsByCompanyId(string month, string year, int companyId);
        Task<BalanceSheetMonthlyReportPaginationByCompanyIdModel> GetAccountHistoryForBalanceSheetMonthlyReportByCompanyId(string month, string year, int companyId, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection);
        Task<BalanceSheetMonthlyReportExcelPdfByCompanyIdModel> GetAccountHistoryForBalanceSheetMonthlyExcelPdfReportByCompanyId(string month, string year, int companyId);
        Task<BalanceSheetYearlyReportByCompanyIdModel> GetBalanceSheetYearlyReportTotalDetailsByCompanyId(string year, int companyId);
        Task<BalanceSheetYearlyReportPaginationByCompanyIdModel> GetAccountHistoryForBalanceSheetYearlyReportByCompanyId(string year, int companyId, int skipRows, int rowsOfPage,
           string? searchPrefix, string sortColumn, string sortDirection);
        Task<BalanceSheetYearlyReportExcelPdfByCompanyIdModel> GetAccountHistoryForBalanceSheetYearlyExcelPdfReportByCompanyId(string year, int companyId);
        Task<List<GetClearedCompanyListModelForExcel>> GetClearedCompanyListForExcel();
        Task<List<GetOverdueCompanyListModelForExcel>> GetOverdueCompanyListForExcel();
        Task<GetProfitLossGraphReportForDashboardModel> GetProfitLossGraphReportForDashboard();
        Task<string> SendCompanyAccountFileOnEmail(string filePath, string fileName, string Email, string fromDate, string toDate, string? company, string? FileType, int createdBy, string username);
        Task SaveDownloadDocumentData(SaveDownloadDocumentModel model);
        Task<List<GetDownloadDocumentListModel>> GetDownloadDocumentList();
    }
}
