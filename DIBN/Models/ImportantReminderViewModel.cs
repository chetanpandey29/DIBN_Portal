using System.Collections.Generic;

namespace DIBN.Models
{
    public class ImportantReminderViewModel
    {
        public List<GetCompanyDocumentReminder> getCompanyDocumentReminders { get; set; }
        public List<GetEmployeesDocumentReminder> getEmployeesDocumentReminders { get; set; }   
        public List<GetShareholderDocumentReminder> getShareholderDocumentReminders { get; set; }
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
        public string Message { get; set; }
        public int NotificationCount { get; set; }
        public string ExpiredNotification { get; set; }
    }

    public class GetCompanyDocumentReminder
    {
        public int DocumentId { get; set; }
        public string CompanyDocumentName { get; set; }
        public string CompanyDocumentExpiryDate { get; set; }
        public string CompanyDocumentExpireDaysLeft { get; set; }
    }

    public class GetEmployeesDocumentReminder
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string PassportExpiryDate { get; set; }
        public string EmployeePassportExpireDaysLeft { get; set; }
    }
    public class GetShareholderDocumentReminder
    {
        public int ID { get; set; }
        public string ShareholderName { get; set; }
        public string PassportExpiryDate { get; set; }
        public string ShareholderPassportExpireDaysLeft { get; set; }
    }
}
