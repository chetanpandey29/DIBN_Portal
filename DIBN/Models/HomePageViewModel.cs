using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace DIBN.Models
{
    public class HomePageViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int CountOfManager { get; set; }
        public int CountOfShareholders { get; set; }
        public int CountOfEmployee { get; set; }
        public int CountOfOpenService { get; set; }
        public int CountOfCloseService { get; set; }
        public int CountOfRejectedService { get; set; }
        public int CountOfOpenSupportTicket { get; set; }
        public int CountOfCloseSupportTicket { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string CompanyType { get; set; }
        public string Module { get; set; }
        public ImportantReminderViewModel importantReminderViewModel { get; set; }
        public List<string> allowedModule {  get; set; }
        public List<string> allowedPermission {  get; set; }
    }
    public class LoggedinStatus
    {
        public int IsDelete { get; set; }
        public int IsActive { get; set; }
        public int IsLoggedIn { get; set; }
    }
}
