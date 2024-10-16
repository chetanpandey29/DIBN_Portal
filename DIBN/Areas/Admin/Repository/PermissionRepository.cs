using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using static DIBN.Areas.Admin.Data.DataSetting;
using static DIBN.Areas.Admin.Models.PermissionViewModel;

namespace DIBN.Areas.Admin.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly Connection _dataSetting;
        public PermissionRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        public List<PermissionViewModel> GetPermissions()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<PermissionViewModel> permissions = new List<PermissionViewModel>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetPermission", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PermissionViewModel permission = new PermissionViewModel();
                    permission.PermissionId = Convert.ToInt32(reader["PermissionId"].ToString());
                    permission.PermissionName = reader["PermissionName"].ToString();
                    permission.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                    permission.IsDelete = Convert.ToBoolean(reader["IsDelete"].ToString());
                    permission.CreatedOn = reader["CreatedOn"].ToString();
                    permission.ModifyOn = reader["ModifyOn"].ToString();
                    permissions.Add(permission);
                }
                con.Close();
                return permissions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public int SaveRolePermission(SaveRolePermission saveRolePermission)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SaveRolePermission", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoleId", saveRolePermission.RoleID);
                command.Parameters.AddWithValue("@ModuleId", saveRolePermission.ModuleId);
                command.Parameters.AddWithValue("@PermissionId", saveRolePermission.PermissionId);
                command.Parameters.AddWithValue("@UserId", saveRolePermission.CreatedBy);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public int DeleteRolePermission(int roleId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveRolePermissions", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@roleId", roleId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public int DeleteUserPermission(int userId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveUserPermissions", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userId", userId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public int SaveUserPermission(SaveUserPermission saveUserPermission)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SaveUserPermission", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", saveUserPermission.UserId);
                command.Parameters.AddWithValue("@ModuleId", saveUserPermission.ModuleId);
                command.Parameters.AddWithValue("@PermissionId", saveUserPermission.PermissionId);
                command.Parameters.AddWithValue("@CreatedBy", saveUserPermission.CreatedBy);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int SaveCompanyPermission(SaveCompanyPermission saveCompanyPermission)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_SaveCompanyPermission", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", saveCompanyPermission.CompanyId);
                command.Parameters.AddWithValue("@ModuleId", saveCompanyPermission.ModuleId);
                command.Parameters.AddWithValue("@PermissionId", saveCompanyPermission.PermissionId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<GetRolePermissionByRoleId> GetRolePermissionByRoleId(int RoleId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetRolePermissionByRoleId> permissions = new List<GetRolePermissionByRoleId>();
                SqlCommand command = new SqlCommand("USP_Admin_GetRolePermissionByRoleId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoleId", RoleId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetRolePermissionByRoleId permission = new GetRolePermissionByRoleId();
                    permission.RolePermissionId = Convert.ToInt32(reader["RolePermissionId"].ToString());
                    permission.RoleID = Convert.ToInt32(reader["RoleID"].ToString());
                    permission.ModuleId = Convert.ToInt32(reader["ModuleId"].ToString());
                    permission.PermissionId = Convert.ToInt32(reader["PermissionId"].ToString());
                    permission.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                    permission.IsDelete = Convert.ToBoolean(reader["IsDelete"].ToString());
                    permission.CreatedOn = reader["CreatedOn"].ToString();
                    permission.ModifyOn = reader["ModifyOn"].ToString();
                    permissions.Add(permission);
                }
                con.Close();
                return permissions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<GetUserPermissionByUserId> GetUserPermissionByUserId(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetUserPermissionByUserId> permissions = new List<GetUserPermissionByUserId>();
                SqlCommand command = new SqlCommand("USP_Admin_GetUserPermissionByUserId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetUserPermissionByUserId permission = new GetUserPermissionByUserId();
                    permission.UserPermissionId = Convert.ToInt32(reader["UserPermissionId"].ToString());
                    permission.UserId = Convert.ToInt32(reader["UserId"].ToString());
                    permission.ModuleId = Convert.ToInt32(reader["ModuleId"].ToString());
                    permission.PermissionId = Convert.ToInt32(reader["PermissionId"].ToString());
                    permission.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                    permission.IsDelete = Convert.ToBoolean(reader["IsDelete"].ToString());
                    permission.CreatedOn = reader["CreatedOn"].ToString();
                    permission.ModifyOn = reader["ModifyOn"].ToString();
                    permissions.Add(permission);
                }
                con.Close();
                return permissions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<GetCompanyPermissionByCompanyId> GetCompanyPermissionByCompanyId(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetCompanyPermissionByCompanyId> permissions = new List<GetCompanyPermissionByCompanyId>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyPermissionByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetCompanyPermissionByCompanyId permission = new GetCompanyPermissionByCompanyId();
                    permission.CompanyPermissionId = Convert.ToInt32(reader["CompanyPermissionId"].ToString());
                    permission.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                    permission.ModuleId = Convert.ToInt32(reader["ModuleId"].ToString());
                    permission.PermissionId = Convert.ToInt32(reader["PermissionId"].ToString());
                    permission.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
                    permission.IsDelete = Convert.ToBoolean(reader["IsDelete"].ToString());
                    permission.CreatedOn = reader["CreatedOn"].ToString();
                    permission.ModifyOn = reader["ModifyOn"].ToString();
                    permissions.Add(permission);
                }
                con.Close();
                return permissions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public int RemoveRolePermission(SaveRolePermission permission)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveRolePermission", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RoleId", permission.RoleID);
                command.Parameters.AddWithValue("@ModuleId", permission.ModuleId);
                command.Parameters.AddWithValue("@PermissionId", permission.PermissionId);
                command.Parameters.AddWithValue("@UserId", permission.CreatedBy);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int CheckRolePermission(SaveRolePermission permission)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CheckRolePermissionExists", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@roleId", permission.RoleID);
                command.Parameters.AddWithValue("@moduleId", permission.ModuleId);
                command.Parameters.AddWithValue("@permissionId", permission.PermissionId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int RemoveUserPermission(SaveUserPermission permission)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveUserPermission", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", permission.UserId);
                command.Parameters.AddWithValue("@ModuleId", permission.ModuleId);
                command.Parameters.AddWithValue("@PermissionId", permission.PermissionId);
                command.Parameters.AddWithValue("@CreatedBy", permission.CreatedBy);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int RemoveCompanyPermission(SaveCompanyPermission permission)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_RemoveCompanyPermission", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", permission.CompanyId);
                command.Parameters.AddWithValue("@ModuleId", permission.ModuleId);
                command.Parameters.AddWithValue("@PermissionId", permission.PermissionId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> GetRolePermissionModuleByRoleId(string Role)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_GetRolePermissionModuleByRoleId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Role", Role);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["ModuleName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> GetUserPermissionModuleByUserId(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_GetUserPermissionModuleByUserId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["ModuleName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> GetCompanyPermissionModuleByCompanyId(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyPermissionModuleByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["ModuleName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int GetUserIdForPermission(string UserNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int Id = 0;
                Regex re = new Regex(@"(@)(.+)$");
                Match result = re.Match(UserNumber);

                SqlCommand command = new SqlCommand("USP_Admin_GetUserDetailsForPermiaaion", con);
                command.CommandType = CommandType.StoredProcedure;
                if (result.Success)
                {
                    command.Parameters.AddWithValue("@Email", UserNumber);
                }
                else
                {
                    command.Parameters.AddWithValue("@AccountNumber", UserNumber);
                }
                con.Open();
                Id = (int)command.ExecuteScalar();
                con.Close();
                return Id;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> GetRolePermissionName(string Role, string Module, int? UserId, int? rolePermission)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                if (UserId != null)
                {
                    List<string> ModulesName = new List<string>();
                    int userId = UserId ?? 0;
                    if (rolePermission == null || Convert.ToInt32(rolePermission) == 0)
                    {
                        ModulesName = GetUserPermissionModuleByUserId(userId);
                        if (ModulesName == null || ModulesName.Count == 0)
                        {
                            SqlCommand command = new SqlCommand("USP_Admin_GetRolePermissionNameByRoleId", con);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Role", Role);
                            command.Parameters.AddWithValue("@Module", Module);
                            con.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                ModuleName.Add(reader["PermissionName"].ToString());
                            }
                            con.Close();
                        }
                    }
                    else
                    {
                        SqlCommand command = new SqlCommand("USP_Admin_GetRolePermissionNameByRoleId", con);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Role", Role);
                        command.Parameters.AddWithValue("@Module", Module);
                        con.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ModuleName.Add(reader["PermissionName"].ToString());
                        }
                        con.Close();
                    }

                }
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> GetUserPermissionName(int UserId, string Module)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_GetUserPermissionNameByUserId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                command.Parameters.AddWithValue("@Module", Module);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["PermissionName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> GetCompanyPermissionName(int CompanyId, string Module)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyPermissionNameByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                command.Parameters.AddWithValue("@Module", Module);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["PermissionName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> CheckUserPermissionAllowedOrNot(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_CheckUserPermissionAllowedOrNot", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["ModuleName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> CheckCompanyPermissionAllowedOrNot(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_CheckCompnayPermissionAllowedOrNot", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["ModuleName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public List<string> CheckUserAndCompanyPermissionAllowedOrNot(string Name)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_CheckCompnayPermissionAllowedOrNotByRoleId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Role", Name);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["ModuleName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int GetUserRole(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int Role = 0;
                List<GetUserRoles> roles = new List<GetUserRoles>();
                SqlCommand command = new SqlCommand("USP_Admin_GetUserRole", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetUserRoles role = new GetUserRoles();
                    role.RoleID = Convert.ToInt32(reader["RoleID"]);
                    role.countOfPermisson = Convert.ToInt32(reader["CountOfPermission"]);
                    roles.Add(role);
                }
                int _permissionCount = 0;

                int _previousCount = 0;
                if (roles != null)
                {
                    if (roles.Count > 1)
                    {
                        for (int index = 0; index < roles.Count; index++)
                        {
                            if (_previousCount == 0)
                            {
                                _previousCount = roles[index].countOfPermisson;
                                Role = roles[index].RoleID;
                            }
                            else
                            {
                                if (roles[index].countOfPermisson > _previousCount)
                                {
                                    _previousCount = roles[index].countOfPermisson;
                                    Role = roles[index].RoleID;
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int index = 0; index < roles.Count; index++)
                        {
                            Role = roles[index].RoleID;
                        }
                    }
                }

                con.Close();
                return Role;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public string GetUserRoleName(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string Role = "";
                List<GetUserRoles> roles = new List<GetUserRoles>();
                SqlCommand command = new SqlCommand("USP_Admin_GetUserRoleName", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    GetUserRoles role = new GetUserRoles();
                    role.RoleID = Convert.ToInt32(reader["RoleID"]);
                    role.RoleName = Convert.ToString(reader["RoleName"]);
                    role.countOfPermisson = Convert.ToInt32(reader["CountOfPermission"]);
                    roles.Add(role);
                }
                int _permissionCount = 0;

                int _previousCount = 0;
                if (roles != null)
                {
                    if (roles.Count > 1)
                    {
                        for (int index = 0; index < roles.Count; index++)
                        {
                            if (_previousCount == 0)
                            {
                                _previousCount = roles[index].countOfPermisson;
                                Role = roles[index].RoleName;
                            }
                            else
                            {
                                if (roles[index].countOfPermisson > _previousCount)
                                {
                                    _previousCount = roles[index].countOfPermisson;
                                    Role = roles[index].RoleName;
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int index = 0; index < roles.Count; index++)
                        {
                            Role = roles[index].RoleName;
                        }
                    }
                }

                con.Close();
                return Role;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int GetUserPermissionCount(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_GetUserPermissionCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userId", UserId);
                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();


                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Get user permission module name                                                                                                             -- Yashasvi (14-10-2024)
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetUserPermissionedModuleName(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCurrentUserModuleByRoleId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["ModuleName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Get user permission module name                                                                                                             -- Yashasvi (14-10-2024)
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetRolePermissionedModuleName(string Role)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = new SqlCommand("USP_Admin_GetCurrentRoleModuleByRoleId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Role", Role);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["ModuleName"].ToString());
                }
                con.Close();
                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// Get current role permission                                                                                                                     -- Yashasvi (14-10-2024)
        /// </summary>
        /// <param name="Role"></param>
        /// <param name="Module"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetCurrentRolePermissionName(string Role, string Module)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();

                SqlCommand command = new SqlCommand("USP_Admin_GetCurrentRolePermissionNameByRoleId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Role", Role);
                command.Parameters.AddWithValue("@Module", Module);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ModuleName.Add(reader["PermissionName"].ToString());
                }
                con.Close();

                return ModuleName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
