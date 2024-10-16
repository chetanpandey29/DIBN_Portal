using DIBN.Models;
using System.Collections.Generic;
using System.Data;

namespace DIBN.IService
{
    public interface IUserShareholderService
    {
        List<ShareholderViewModel> GetAllShareholders(int? Id);
        ShareholderViewModel GetDetailsOfShareholder(int Id);
        int AddNewShareholder(ShareholderViewModel shareholder);
        int UpdateShareholderDetails(ShareholderViewModel shareholder);
        int RemoveShareholder(int Id);
        List<ShareholderDocuments> GetShareholdersDocuments(int? Id, int? DocumentId);
        DataSet GetShareholdersForExport(int ID);
    }
}
