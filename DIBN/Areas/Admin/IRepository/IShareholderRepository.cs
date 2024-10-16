using DIBN.Areas.Admin.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IShareholderRepository
    {
        List<ShareholderViewModel> GetAllShareholders(int CompanyId);
        ShareholderViewModel GetDetailsOfShareholder(int Id);
        int AddNewShareholder(ShareholderViewModel shareholder);
        int UpdateShareholderDetails(ShareholderViewModel shareholder);
        int RemoveShareholder(int Id, int UserId);
        int UploadShareholderDocuments(ShareholderDocuments documents);
        List<ShareholderDocuments> GetShareholdersDocuments(int? Id, int? DocumentId);
        int GetRemainingSharePercentage(int CompanyId);
        DataSet GetShareholdersForExport(int? ID);
        int RemoveDocument(int ShareholderId, int DocumentId, int UserId);
        Task<GetAllActiveShareholdersWithPaginationModel> GetAllActiveShareholdersWithPagination(int page, int pageSize, string searchBy, string searchString, string sortBy, string sortDirection);
        Task<GetAllInActiveShareholdersWithPaginationModel> GetAllInActiveShareholdersWithPagination(int page, int pageSize, string searchBy, string searchString, string sortBy, string sortDirection);
    }
}
