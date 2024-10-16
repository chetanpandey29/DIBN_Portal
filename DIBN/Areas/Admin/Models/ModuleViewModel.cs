using System.Collections.Generic;

namespace DIBN.Areas.Admin.Models
{
    public class ModuleViewModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleKeyword { get; set; }   
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
    }
}
