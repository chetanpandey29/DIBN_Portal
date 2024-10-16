using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class MainCompanyDocumentTypeModel
    {
        public List<CompanyDocumentTypeModel> documentTypes { get; set; }
        public List<string> allowedModule {  get; set; }
        public string Module {  get; set; }
    }
    public class CompanyDocumentTypeModel
    {
        public int ID { get; set; }
        public string DocumentName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public string Module { get; set; }
        public int CreatedBy { get; set; }
    }
}
