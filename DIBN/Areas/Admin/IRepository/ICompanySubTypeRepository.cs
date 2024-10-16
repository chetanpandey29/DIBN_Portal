using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface ICompanySubTypeRepository
    {
        Task<List<GetAllCompanySubTypeModel>> GetAllCompanySubType();
        Task<int> SaveCompanyType(SaveCompanySubTypeModel model);
        Task<int> UpdateCompanyType(UpdateCompanySubTypeModel model);
        Task<UpdateCompanySubTypeModel> GetCompanySubTypeDetails(int Id);
        Task Delete(int Id, int UserId);
    }
}
