using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IMessageTemplateRepository
    {
        int SaveMessageTemplate(MessageTemplateViewModel model);
        List<MessageTemplateViewModel> GetAllMessageTemplate();
        MessageTemplateViewModel GetMessageTemplateDetails(int Id);
        int UpdateMessageTemplate(MessageTemplateViewModel model);
        int RemoveMessageTemplate(int Id);
        List<MailHistory> GetAllMailHistory();
        MailHistory GetMailById(int Id);
        Task<string> ReSendCompanyMail(MailHistory model);
        Task<string> SendCompanyMail(MailHistory model);
        EmailDocument GetEmailDocumentsById(int Id);
        int DeleteEmail(int Id);
    }
}
