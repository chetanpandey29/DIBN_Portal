using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface ILogRepository
    {
        List<LogsModel> GetLogs();
        int RemoveLogDetails(int Id);
        int RemoveAllLogDetails();
    }
}
