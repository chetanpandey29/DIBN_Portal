using DIBN.Models;
using System.Collections.Generic;

namespace DIBN.IService
{
    public interface IFileUploaderService
    {
        List<UserDocumentsViewModel> GetAllDocuments(int CompanyId);
        int UploadSelectedFile(UserDocumentsViewModel document, int CompanyId);
        UserDocumentsViewModel DownloadDocument(int Id, int CompanyId);
        List<UserDocumentsViewModel> GetAllDocumentOfEmployee(int CompanyId, int UserId, int SelectedDocumentId);
        List<UserDocumentsViewModel> GetAllDocumentsByUserId(int CompanyId, int UserId); 
        UserDocumentsViewModel DownloadCompanyDocuments(int Id, int CompanyId);
    }
}
