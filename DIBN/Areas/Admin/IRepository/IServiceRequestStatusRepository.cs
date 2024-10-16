using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IServiceRequestStatusRepository
    {
        List<ServiceRequestStatusModel> GetServiceRequestStatus();

        ServiceRequestStatusModel GetServiceRequestStatusById(int Id);

        int SaveServiceRequestStatus(ServiceRequestStatusModel model);

        int UpdateServiceRequestStatus(ServiceRequestStatusModel model);

        int DeleteServiceRequestStatus(int Id, int UserId);

        int GetlastDisplayId();
    }
}
