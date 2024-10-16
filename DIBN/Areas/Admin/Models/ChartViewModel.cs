using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class GetEmployeeDetailsforChart
    {
        public int CountOfUsers { get; set; }
        public int UserYear { get; set; }
        public int SalesPersonYear { get; set; }
        public int CountOfSalesPersons { get; set; }
        public List<EmployeeDetailsforChartMonth> employeeDetailsforChartMonths { get; set; }
        public string Month { get; set; }
    }

    public class EmployeeDetailsforChartMonth
    {
        public int CountOfSalesPersons { get; set; }
        public string Month { get; set; }

    }

    public class OpenServiceChartYear
    {
        public int OpenServiceCount { get; set; }
        public int OpenServiceYear { get; set; }
    }

    public class CloseServiceChartYear
    {
        public int CloseServiceCount { get; set; }
        public int CloseServiceYear { get; set; }
    }
    public class CloseServiceChartMonth
    {
        public int CloseServiceCount { get; set; }
        public string CloseServiceMonth { get; set; }
    }
    public class OpenServiceChartMonth
    {
        public int OpenServiceCount { get; set; }
        public string OpenServiceMonth { get; set; }
    }

    public class GetServicesCountForChart
    {
        public List<OpenServiceChartYear> openServiceChartYears { get; set; }
        public List<CloseServiceChartYear> closeServiceChartYears { get; set; }
        public List<CloseServiceChartMonth> closeServiceChartMonths { get; set; }
        public List<OpenServiceChartMonth> openServiceChartMonths { get; set; }
    }
}
