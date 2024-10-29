using System.Collections.Generic;
using static DIBN.Areas.Admin.Models.UserViewModel;

namespace DIBN.Areas.Admin.Models
{
    public class HomePageViewModel
    {
        public string Module { get; set; }
        public GetCompaniesWithMainCompany getCompaniesWithMainCompany { get; set; }
        public GetCompanyEmployeesModel getCompanyEmployeesModel { get; set; }
        public int CountofCompany { get; set; }
        public int CountofEmployees { get; set; }
        public int CountofMainCompanyEmp { get; set; }
        public int CountofOtherCompanyEmp { get; set; }
        public int CountofOpenServie { get; set; }
        public int CountofCloseService { get; set; }
        public int CountofRejectedService { get; set; }
        public int CountOfOpenSupportTicket { get; set; }
        public int CountOfCloseSupportTicket { get; set; }
        public int CountOfMainLandCmp { get; set; }
        public int CountOfCFreezoneCmp { get; set; }
        public List<GetEmployeeDetailsforChart> YearlyUser { get; set; }
        public List<GetEmployeeDetailsforChart> MonthlyUser { get; set; }
        public GetServicesCountForChart getServicesCountForYearlyChart { get; set; }
        public GetServicesCountForChart getServicesCountForMonthlyChart { get; set; }
        public List<decimal> getPortalBalance { get; set; }
        public List<string> allowedPermission {  get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class LoggedinStatus
    {
        public int IsDelete { get; set; }
        public int IsActive { get; set; }
        public int IsLoggedIn { get; set; }
    }
    public class GetWeeklyCompanySubTypeReportModel
    {
        public int TotalMainlandCompanies { get; set; }
        public int TotalFreezoneCompanies { get; set; }
        public string CompanySubType {  get; set; }
        public string color {  get; set; }
    }
    public class GetMonthlyCompanySubTypeReportModel
    {
        public int TotalMainlandCompanies { get; set; }
        public int TotalFreezoneCompanies { get; set; }
        public string CompanySubType { get; set; }
        public string color { get; set; }
    }
    public class GetYearlyCompanySubTypeReportModel
    {
        public int TotalMainlandCompanies { get; set; }
        public int TotalFreezoneCompanies { get; set; }
        public string CompanySubType { get; set; }
        public string color { get; set; }
    }
}
