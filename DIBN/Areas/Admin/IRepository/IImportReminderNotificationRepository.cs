using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IImportReminderNotificationRepository
    {
        Task<List<ImportReminderNotification>> GetAllServiceNotifications(string service);
        List<ImportReminderNotification> GetAllNotifications();
        int UpdateReminderDays(int Id, int NotificationDays, int UserId);
        void UpdateImportantNotifications();
        void UpdateLeftDays();
        int RemoveNotification(int Id, int UserId);
        List<GetRequestNotificationModel> GetRequestNotifications(string StartDate, string EndDate, int UserId);
        int ChangeRequestNotificationStatus(int Status, int Id, int UserId);
        Task<List<GetNotificationServiceListModel>> GetNotificationServiceList();
    }
}
