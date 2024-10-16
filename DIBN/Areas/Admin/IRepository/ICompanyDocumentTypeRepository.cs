using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface ICompanyDocumentTypeRepository
    {
        List<CompanyDocumentTypeModel> GetCompanyDocuments();
        CompanyDocumentTypeModel GetCompanyDocumentById(int Id);
        int AddCompanyDocumentType(CompanyDocumentTypeModel documentType);
        int UpdateCompanyDocumentType(CompanyDocumentTypeModel documentType);
        int RemoveCompanyDocumentType(int Id, int UserId);
    }
}
