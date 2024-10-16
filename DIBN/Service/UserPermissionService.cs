using DIBN.IService;
using DIBN.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly Connection _dataSetting;
        public UserPermissionService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
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
        public List<string> GetRolePermissionName(string Role, string Module)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = null;

                command = new SqlCommand("USP_Admin_GetRolePermissionCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Role", Role);
                con.Open();
                int _count = (int)command.ExecuteScalar();
                con.Close();
                command.Parameters.Clear();
                if (_count > 0)
                {
                    command = new SqlCommand("USP_Admin_GetRolePermissionNameByRoleId", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Role", Role);
                    command.Parameters.AddWithValue("@Module", Module);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ModuleName.Add(reader["PermissionName"].ToString());
                    }
                }
                if (ModuleName.Count == 0 && _count > 0)
                {
                    ModuleName.Add("PermissionCount");
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
        public List<string> GetUserPermissionName(int UserId, string Module)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = null;

                command = new SqlCommand("USP_Admin_GetUserPermissionCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userId", UserId);
                con.Open();
                int _count = (int)command.ExecuteScalar();
                con.Close();
                command.Parameters.Clear();
                if (_count > 0)
                {
                    command = new SqlCommand("USP_Admin_GetUserPermissionNameByUserId", con);
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
                }
                if (ModuleName.Count == 0 && _count > 0)
                {
                    ModuleName.Add("PermissionCount");
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
        public List<string> GetCompanyPermissionModuleByCompanyId(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_GetCompanyPermissionCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                int _count = (int)command.ExecuteScalar();
                con.Close();
                command.Parameters.Clear();
                if (_count > 0)
                {
                    command = new SqlCommand("USP_Admin_GetCompanyPermissionModuleByCompanyId", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ModuleName.Add(reader["ModuleName"].ToString());
                    }
                    con.Close();
                }
                if (ModuleName.Count == 0 && _count > 0)
                {
                    ModuleName.Add("PermissionCount");
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
        public List<string> GetCompanyPermissionName(int CompanyId, string Module)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> ModuleName = new List<string>();
                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_GetCompanyPermissionCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                int _count = (int)command.ExecuteScalar();
                con.Close();
                command.Parameters.Clear();
                if (_count > 0)
                {
                    command = new SqlCommand("USP_Admin_GetCompanyPermissionNameByCompanyId", con);
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
                }
                if (ModuleName.Count == 0 && _count > 0)
                {
                    ModuleName.Add("PermissionCount");
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
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                    {
                        Id = Convert.ToInt32(reader["Id"]);
                    }
                }
                con.Close();
                command.Parameters.Clear();
                if (Id == 0)
                {
                    command = new SqlCommand("USP_Admin_GetAssignedUserToCompanyForPermission", con);
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
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["Id"] != DBNull.Value)
                        {
                            Id = Convert.ToInt32(reader["Id"]);
                        }
                    }
                    con.Close();
                }
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

        public int GetSalesPersonIdForPermission(string UserNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int Id = 0;
                Regex re = new Regex(@"(@)(.+)$");
                Match result = re.Match(UserNumber);

                SqlCommand command = new SqlCommand("USP_Admin_GetSalesPersonDetailsForPermission", con);
                command.CommandType = CommandType.StoredProcedure;
                if (result.Success)
                {
                    command.Parameters.AddWithValue("@Email", UserNumber);
                }
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                    {
                        Id = Convert.ToInt32(reader["Id"]);
                    }
                }
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
        public int GetRMTeamIdForPermission(string UserNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int Id = 0;
                Regex re = new Regex(@"(@)(.+)$");
                Match result = re.Match(UserNumber);

                SqlCommand command = new SqlCommand("USP_Admin_GetRMTeamDetailsForPermission", con);
                command.CommandType = CommandType.StoredProcedure;
                if (result.Success)
                {
                    command.Parameters.AddWithValue("@Email", UserNumber);
                }
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                    {
                        Id = Convert.ToInt32(reader["Id"]);
                    }
                }
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

        /// <summary>
        /// To Get Company Id if passed Logged in Account Number Belongs to any Company             -- Yashasvi TBC (24-11-2022)
        /// </summary>
        /// <param name="UserNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetCompanyIdForPermission(string UserNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int Id = 0;
                Regex re = new Regex(@"(@)(.+)$");
                Match result = re.Match(UserNumber);

                SqlCommand command = new SqlCommand("USP_Admin_GetCompanyDetailsForPermission", con);
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
        public string GetSalesPersonName(string UserNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _username = null, firstName = null, lastName = null;

                SqlCommand command = new SqlCommand("USP_Admin_GetSalesPersonName", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@email", UserNumber);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    firstName = reader["FirstName"].ToString();
                    lastName = reader["LastName"].ToString();
                }
                con.Close();

                _username = firstName + " " + lastName;
                return _username;
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
        public string GetRMTeamName(string UserNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string _username = null, firstName = null, lastName = null;

                SqlCommand command = new SqlCommand("USP_Admin_GetRMTeamPersonName", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@email", UserNumber);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    firstName = reader["FirstName"].ToString();
                    lastName = reader["LastName"].ToString();
                }
                con.Close();

                _username = firstName + " " + lastName;
                return _username;
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

        public List<string> GetAllAssignedCompanies(int UserId, int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> Company = new List<string>();
             
                SqlCommand command = new SqlCommand("USP_Admin_GetAllSalesPersonAssignedCompany", con);
                command.CommandType = CommandType.StoredProcedure;
                if (CompanyId != 1)
                {
                    command.Parameters.AddWithValue("@UserId", UserId);
                }
                    con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Company.Add(reader["CompanyName"].ToString());
                }
                con.Close();
                return Company;
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
        
        public List<string> GetAllAssignedCompaniesToRMTeam(int UserId, int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<string> Company = new List<string>();
             
                SqlCommand command = new SqlCommand("USP_Admin_GetAllRMTeamAssignedCompany", con);
                command.CommandType = CommandType.StoredProcedure;
                if (CompanyId != 1)
                {
                    command.Parameters.AddWithValue("@UserId", UserId);
                }
                    con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Company.Add(reader["CompanyName"].ToString());
                }
                con.Close();
                return Company;
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

        public List<SalesPersonCompany> GetAllAssignedCompaniesSalesPerson(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<SalesPersonCompany> Company = new List<SalesPersonCompany>();

                SqlCommand command = new SqlCommand("USP_Admin_GetAllSalesPersonAssignedCompany", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SalesPersonCompany salesPersonCompany = new SalesPersonCompany();
                    salesPersonCompany.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    salesPersonCompany.CompanyName = reader["CompanyName"].ToString();
                    Company.Add(salesPersonCompany);
                }
                con.Close();
                return Company;
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
        public int GetEmployeesCount(int CompanyId, string Role)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                if (CompanyId != 0)
                {
                    SqlCommand command = null;

                    command = new SqlCommand("USP_Admin_GetRoleWiseCompanyEmployeeCount", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Role", Role);
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    con.Open();
                    _returnId = (int)command.ExecuteScalar();
                    con.Close();
                }

                return _returnId;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

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
        public int GetCompanyUsersCount(int companyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _usernameCount = 0;
                SqlCommand command = null;

                command = new SqlCommand("USP_Admin_GetRoleWiseCompanyOwnerCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", companyId);
                con.Open();
                _usernameCount = Convert.ToInt32(command.ExecuteScalar());
                con.Close();

                return _usernameCount;
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
        /// <summary>
        /// Get user permission module name                                                                                                             -- Yashasvi (15-10-2024)
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
        /// Get user permission module name                                                                                                             -- Yashasvi (15-10-2024)
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
        /// Get current role permission                                                                                                                     -- Yashasvi (15-10-2024)
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
