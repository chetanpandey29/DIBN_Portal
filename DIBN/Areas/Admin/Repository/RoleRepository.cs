using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly Connection _dataSetting;
        public RoleRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        /// <summary>
        /// Create New Role                                                                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public int CreateNewRole(RoleViewModel role)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_RoleOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Insert);
                cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                cmd.Parameters.AddWithValue("@IsActive", role.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", role.CreatedBy);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
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
        /// Get All Roles                                                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<RoleViewModel> GetRoles()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<RoleViewModel> roles = new List<RoleViewModel>();
                SqlCommand cmd = new SqlCommand("USP_Admin_RoleOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    RoleViewModel role = new RoleViewModel();
                    role.RoleID = Convert.ToInt32(dr["RoleID"].ToString());
                    role.RoleName = dr["RoleName"].ToString();
                    role.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    role.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    role.CreatedOn = dr["CreatedOn"].ToString();
                    role.ModifyOn = dr["ModifyOn"].ToString();
                    roles.Add(role);
                }
                con.Close();
                return roles;
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
        /// Get Role Details By Id                                                                          -- Yashasvi TBC (26-11-2022)                   
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public RoleViewModel GetRoleDetail(int RoleId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                RoleViewModel role = new RoleViewModel();
                SqlCommand cmd = new SqlCommand("USP_Admin_RoleOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.GetById);
                cmd.Parameters.AddWithValue("@RoleID", RoleId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    role.RoleID = Convert.ToInt32(dr["RoleID"].ToString());
                    role.RoleName = dr["RoleName"].ToString();
                    role.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    role.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    role.CreatedOn = dr["CreatedOn"].ToString();
                    role.ModifyOn = dr["ModifyOn"].ToString();
                }
                con.Close();
                return role;
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
        /// Update Role Details                                                                             -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateRoleDetail(RoleViewModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_RoleOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Update);
                cmd.Parameters.AddWithValue("@RoleID", model.RoleID);
                cmd.Parameters.AddWithValue("@RoleName", model.RoleName);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
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
        /// Delete Role                                                                                     -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteRole(int RoleId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_RoleOperation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", Operation.Delete);
                cmd.Parameters.AddWithValue("@RoleID", RoleId);
                cmd.Parameters.AddWithValue("@CreatedBy", UserId);
                con.Open();
                returnId = (int)cmd.ExecuteScalar();
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
        /// Get Active Roles                                                                                -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<RoleViewModel> GetActiveRoles()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<RoleViewModel> roles = new List<RoleViewModel>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetActiveRoles", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    RoleViewModel role = new RoleViewModel();
                    role.RoleID = Convert.ToInt32(dr["RoleID"].ToString());
                    //if (dr["RoleName"].ToString() == "DIBN")
                    //{
                    //    role.RoleName = "DEVOTION";
                    //}
                    //else
                    //{
                        role.RoleName = dr["RoleName"].ToString();
                    //}
                    
                    role.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    role.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    role.CreatedOn = dr["CreatedOn"].ToString();
                    role.ModifyOn = dr["ModifyOn"].ToString();
                    roles.Add(role);
                }
                con.Close();
                return roles;
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
        /// Check Whether Role Already Exists or Not.                                                       -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CheckExistanceOfRole(string Name,int roleId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_CheckExistingRole", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", Name);
                if(roleId != 0) 
                    cmd.Parameters.AddWithValue("@id", roleId);
                connection.Open();
                returnId = (int)cmd.ExecuteScalar();
                connection.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get Company Owner Id                                                                            -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetCompanyOwnerId()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyOwnerRoleId", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                returnId = (int)cmd.ExecuteScalar();
                connection.Close();
                return returnId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
