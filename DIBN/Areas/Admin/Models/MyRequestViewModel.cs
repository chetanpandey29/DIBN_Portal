using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class MyRequestViewModel
    {
        public List<EmployeeServiceRequest> employeeServiceRequests { get; set; }
        public List<CompanyServiceRequest> companyServiceRequests { get; set; }
        public int CompanyId { get; set; }
        public string Module { get; set; }
        public string Designation { get; set; }
    }
}
