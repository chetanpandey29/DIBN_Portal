using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class MainImportReminderNotification
    {
        public string Module {  get; set; }
        public List<ImportReminderNotification> notifications { get; set; }
        public List<string> allowedModule {  get; set; }
    }
    public class ImportReminderNotification
    {
        public int ID { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Service { get; set; }
        public string ExpiryDate { get; set; }
        public int LeftDayToExpire { get; set; }
        public int SendNotificationAfter { get; set; }
        public bool MarkAsRead { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string ExpiredNotification { get; set; }
    }
    public class MainNotificationModel
    {
        public MainNotificationModel()
        {
            services = new List<GetNotificationServiceListModel>();
        }
        public List<GetNotificationServiceListModel> services {  get; set; }
        public string? Module {  get; set; }
        public List<string> allowedModule {  get; set; }

    }
    public class GetNotificationServiceListModel
    {
        public int Index {  get; set; }
        public string Service { get; set; }
        public int totalCount { get; set; }
    }
}
