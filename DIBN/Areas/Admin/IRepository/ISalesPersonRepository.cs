using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface ISalesPersonRepository
    {
        List<SalesPersonViewModel> GetAllSalesPersons();
        int CreateNew(SalesPersonViewModel salesPerson);
        SalesPersonViewModel GetSalesPersonDetail(int Id);
        Task<GetSalesPersonDetailsForLoginModel> GetSalesPersonDetailsForLogin(int Id);
        int Update(SalesPersonViewModel salesPerson);
        int Delete(int Id, int UserId);
        GetAllSalesPersonsWithPaginationModel GetAllSalesPersonsWithPagination(int page, int pageSize, string searchValue, string sortBy, string sortDirection);
    }
}
