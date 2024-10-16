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
    public class CompanyServiceRepository : ICompanyServiceRepository
    {
        private readonly Connection _dataSetting;
        public CompanyServiceRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        /// <summary>
        /// Get All Company Service List                                                        -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CompanyServiceViewModel> GetAllCompanyServices()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<CompanyServiceViewModel> employeeServices = new List<CompanyServiceViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_CompanyServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CompanyServiceViewModel service = new CompanyServiceViewModel();

                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.CompanyTypeName = reader["CompanyType"].ToString();
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();

                    employeeServices.Add(service);
                }
                con.Close();
                return employeeServices;
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
        /// Get All Parent Company Service List                                                 -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CompanyServiceViewModel> GetAllParentCompanyServices()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<CompanyServiceViewModel> employeeServices = new List<CompanyServiceViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllParentCompanyServiceByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CompanyServiceViewModel service = new CompanyServiceViewModel();

                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.CompanyTypeName = reader["CompanyType"].ToString();
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();

                    employeeServices.Add(service);
                }
                con.Close();
                return employeeServices;
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
        /// Get Company Service Details By Id                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CompanyServiceViewModel GetCompanyServicesById(int ID)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CompanyServiceViewModel service = new CompanyServiceViewModel();
                SqlCommand command = new SqlCommand("USP_Admin_CompanyServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@ID", ID);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    service.ID = Convert.ToInt32(reader["ID"]);
                    service.ParentId = Convert.ToInt32(reader["ParentId"]);
                    service.CompanyTypeName = reader["CompanyType"].ToString();
                    service.ServiceName = reader["ServiceName"].ToString();
                    service.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    service.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    service.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    service.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                }
                con.Close();
                return service;
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
        /// Create New Company Service                                                          -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateNew(CompanyServiceViewModel service)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CompanyServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@CompanyType", service.CompanyTypeName);
                command.Parameters.AddWithValue("@ParentId", service.ParentId);
                command.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                command.Parameters.AddWithValue("@UserId", service.UserId);
                command.Parameters.AddWithValue("@IsActive", service.IsActive);
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
        /// Update Company Service Details                                                      -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateCompanyServices(CompanyServiceViewModel service)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CompanyServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@ID", service.ID);
                command.Parameters.AddWithValue("@ParentId", service.ParentId);
                command.Parameters.AddWithValue("@CompanyType", service.CompanyTypeName);
                command.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                command.Parameters.AddWithValue("@UserId", service.UserId);
                command.Parameters.AddWithValue("@IsActive", service.IsActive);
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
        /// Delete Company Service                                                              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteCompanyServices(int ID, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CompanyServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@UserId", UserId);
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
        /// Get Serial Number                                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetSerialNumber()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string returnId = null;
                SqlCommand command = new SqlCommand("USP_Admin_CompanyServiceOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 5);
                con.Open();
                returnId = (string)command.ExecuteScalar();
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
    }
}
