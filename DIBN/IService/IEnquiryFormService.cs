using DIBN.Models;
using System.Collections.Generic;

namespace DIBN.IService
{
    public interface IEnquiryFormService
    {
        List<EnquiryFormDetailsModel> GetEnquiryFormDetails(int UserId);
        string GetStatusOfEnquiryForm(int RequestId);
        int SaveStatusOfEnquiryForm(ChangeEnquiryStatusModel model);
    }
}
