using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static DIBN.Areas.Admin.Models.PermissionViewModel;

namespace DIBN.Areas.Admin.Models
{
    public class MainRoleViewModel
    {
        public List<RoleViewModel> roles { get; set; }
        public List<string> allowedModule {  get; set; }
        public string Module {  get; set; }
    }
    public class RoleViewModel
    {
        public int RoleID { get; set; }
        [DisplayName("Role Name")]
        [Required(ErrorMessage = "Please enter Role Name.")]
        public string RoleName { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }
        public string Module { get; set; }
        public int CreatedBy { get; set; }

        #region Nested Class
        public class RolePermissionList
        {
            [DisplayName("Roles")]
            [Required(ErrorMessage = "Please Select Role from below list.")]
            public IList<SelectListItem> Roles { get; set; }
            [DisplayName("Roles")]
            [Required(ErrorMessage = "Please Select Role from below list.")]
            public int Id { get; set; }
            [DisplayName("Roles")]
            [Required(ErrorMessage = "Please Select Role from below list.")]
            public string Role { get; set; }
            public int RoleId { get; set; }
            [DisplayName("Modules")]
            public List<ModuleViewModel> Modules { get; set; }
            public List<PermissionViewModel> Permissions { get; set; }
            public int permissionCount { get; set; }
            public string Module { get; set; }
            public List<GetRolePermissionByRoleId> getRolePermissionByRoleIds { get; set; }
            public List<string> allowedModule {  get; set; }
        }

        public class SaveRolePermissionValues
        {
            public int[] InsertPermission { get; set; }
            public int[] UpdatePermission { get; set; } 
            public int[] ViewPermission { get; set; }
            public int[] DeletePermission { get; set; }
            public int RoleId { get;set; }
        }
        #endregion
    }
}
