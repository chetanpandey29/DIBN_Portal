using DIBN.Models;
using System.Collections.Generic;

namespace DIBN.IService
{
    public interface IImportantReminderService
    {
        ImportantReminderViewModel GetImportantReminderMessages(int CompanyId);
        List<ImportantReminderViewModel> GetImportantReminderMessagesList(int CompanyId);
        int MarkAsReadNotification(int CompanyId, int Id);
        int UpdateStatusOfNotification(int CompanyId, int Id);
        int GetAssignedServicesCount(int UserId);
        int GetAssignedSupportTicketCount(int UserId);
        int MarkAsReadServiceNotification(int UserId, string SerialNumber);
        List<GetRequestNotificationModel> GetRequestNotifications(int UserId, string StartDate, string EndDate);
        int ChangeRequestNotificationStatus(int Status, int Id);
    }
}
