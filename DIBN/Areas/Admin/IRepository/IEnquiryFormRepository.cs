using DIBN.Areas.Admin.Models;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.IRepository
{
    public interface IEnquiryFormRepository
    {
        List<EnquiryFormDetails> GetEnquiryFormDetails(string FromDate, string ToDate);
        List<int> GetAllAssignedUsers(int RequestId);
        int SaveAssignUserOfService(SaveAssignUserForEnquiry saveAssignUser);
        string GetStatusOfEnquiryForm(int RequestId);
        int SaveStatusOfEnquiryForm(ChangeEnquiryStatusModel model);
        int DeleteEnquiry(int RequestId);
    }
}
