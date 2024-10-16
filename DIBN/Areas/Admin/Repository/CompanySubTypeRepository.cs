using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class CompanySubTypeRepository : ICompanySubTypeRepository
    {
        private readonly Connection _connection;
        public CompanySubTypeRepository(Connection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Get all company sub types                                                                                                                    -- Yashasvi (16-10-2024)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<GetAllCompanySubTypeModel>> GetAllCompanySubType()
        {
            SqlConnection connection = new SqlConnection(_connection.DefaultConnection);
            try
            {
                int index = 1;
                List<GetAllCompanySubTypeModel> subTypes = new List<GetAllCompanySubTypeModel>();

                SqlCommand command = new SqlCommand("USP_Admin_Select_GetAllCompanySubTypes", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetAllCompanySubTypeModel subType = new GetAllCompanySubTypeModel();

                    if (reader["CompanyType"] != DBNull.Value)
                        subType.CompanyType = reader["CompanyType"].ToString();
                    if (reader["CompanySubType"] != DBNull.Value)
                        subType.SubType = reader["CompanySubType"].ToString();
                    if (reader["Id"] != DBNull.Value)
                        subType.Id = Convert.ToInt32(reader["Id"]);
                    subType.Index = index;
                    subTypes.Add(subType);
                    index++;
                }
                connection.Close();

                return subTypes;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Save new company type                                                                                                                           -- Yashasvi (16-10-2024)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> SaveCompanyType(SaveCompanySubTypeModel model)
        {
            SqlConnection connection = new SqlConnection(_connection.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_Insert_Update_CompanySubType", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MainType", model.MainType);
                command.Parameters.AddWithValue("@SubType", model.SubType);
                command.Parameters.AddWithValue("@UserId", model.UserId);

                connection.Open();
                _returnId = Convert.ToInt32(await command.ExecuteScalarAsync());
                connection.Close();

                return _returnId;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// update company type                                                                                                                           -- Yashasvi (16-10-2024)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> UpdateCompanyType(UpdateCompanySubTypeModel model)
        {
            SqlConnection connection = new SqlConnection(_connection.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_Insert_Update_CompanySubType", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@MainType", model.MainType);
                command.Parameters.AddWithValue("@SubType", model.SubType);
                command.Parameters.AddWithValue("@UserId", model.UserId);

                connection.Open();
                _returnId = Convert.ToInt32(await command.ExecuteScalarAsync());
                connection.Close();

                return _returnId;
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
        /// Update company sub type                                                                                                                         -- Yashasvi (16-10-2024)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UpdateCompanySubTypeModel> GetCompanySubTypeDetails(int Id)
        {
            SqlConnection connection = new SqlConnection(_connection.DefaultConnection);
            try
            {
                UpdateCompanySubTypeModel model = new UpdateCompanySubTypeModel();

                SqlCommand command = new SqlCommand("USP_Admin_Select_GetCompanyTypeDetailById", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["CompanyType"] != DBNull.Value)
                        model.MainType = reader["CompanyType"].ToString();
                    if (reader["CompanySubType"] != DBNull.Value)
                        model.SubType = reader["CompanySubType"].ToString();
                }
                connection.Close();

                return model;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Delete Company Sub Type                                                                                                                          -- Yashasvi (16-10-2024)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Delete(int Id,int UserId)
        {
            SqlConnection connection = new SqlConnection(_connection.DefaultConnection);
            try
            {
                SqlCommand command = new SqlCommand("USP_Admin_Delete_CompanyType", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@UserId", UserId);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
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
