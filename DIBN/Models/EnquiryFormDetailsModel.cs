using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DIBN.Models
{
    public class EnquiryFormDetailsModel
    {
        public int Id { get; set; }
        public string EnquiryNumber { get; set; }
        public string Name { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public string EnquiryFor { get; set; }
        public string CreatedOn { get; set; }
        public string Status { get; set; }
        public string Module { get; set; }
        public string AssignedUser { get; set; }
        public string AssignedOn { get; set; }
    }
    public class ChangeEnquiryStatusModel
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public string EnquiryNumber { get; set; }
        public IList<SelectListItem> StatusItems { get; set; }
    }
}
