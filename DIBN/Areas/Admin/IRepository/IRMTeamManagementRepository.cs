using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IRMTeamManagementRepository
    {
        Task<GetRMTeamListWithPaginationModel> GetRMTeamListWithPagination(int fetchRows, int skipRows, string? searchPrefix, string? sortBy, string? sortingDirection);
        Task<int> SaveRMTeamDetails(SaveRMTeamModel model);
        Task<int> UpdateRMTeamDetails(UpdateRMTeamModel model);
        Task<UpdateRMTeamModel> GetRMTeamDetails(int Id);
        Task Delete(int Id, int UserId);
        Task<List<GetAllRMPersonModel>> GetAllRMPersonsForCompany();
    }
}
