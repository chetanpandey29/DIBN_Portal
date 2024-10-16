namespace DIBN.Areas.Admin.Models
{
    public class PermissionViewModel
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyOn { get; set; }



        #region Nested Classes
            public class GetRolePermissionByRoleId
            {
                public int RolePermissionId { get; set; }
                public int RoleID { get; set; }
                public int PermissionId { get; set; }
                public int ModuleId { get; set; }
                public bool IsActive { get; set; }
                public bool IsDelete { get; set; }
                public string CreatedOn { get; set; }
                public string ModifyOn { get; set; }
            }

            public class GetUserPermissionByUserId
            {
                public int UserPermissionId { get; set;}
                public int UserId { get; set; }
                public int PermissionId { get; set;}
                public int ModuleId { get; set; }
                public bool IsActive { get; set; }
                public bool IsDelete { get; set; }
                public string CreatedOn { get; set; }
                public string ModifyOn { get; set; }
            }

            public class GetCompanyPermissionByCompanyId
            {
                public int CompanyPermissionId { get; set; }
                public int CompanyId { get; set; }
                public int PermissionId { get; set; }
                public int ModuleId { get; set; }
                public bool IsActive { get; set; }
                public bool IsDelete { get; set; }
                public string CreatedOn { get; set; }
                public string ModifyOn { get ; set; }
            }

            public class SaveRolePermission
            {
                public int RoleID { get; set; }
                public int ModuleId { get; set; }
                public int PermissionId { get; set;}
                public int CreatedBy { get; set; }
            }

            public class SaveUserPermission
            {
                public int UserId { get; set; }
                public int ModuleId { get; set; }
                public int PermissionId { get; set;}
                public int CreatedBy { get; set;}
            }
            public class SaveCompanyPermission
            {
                public int CompanyId { get; set; }
                public int ModuleId { get; set; }
                public int PermissionId { get;set;}
            }
        public class GetUserRoles
        {
            public int RoleID { get; set; }
            public string RoleName { get; set; }
            public int countOfPermisson { get; set; }
        }
        #endregion
    }
}
