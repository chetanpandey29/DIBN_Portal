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
    public class CompanyDocumentTypeRepository : ICompanyDocumentTypeRepository
    {
        private readonly Connection _dataSetting;
        public CompanyDocumentTypeRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        /// <summary>
        /// Get All Company Documents Type List                                                                                         -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CompanyDocumentTypeModel> GetCompanyDocuments()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<CompanyDocumentTypeModel> models = new List<CompanyDocumentTypeModel>();
                SqlCommand command = new SqlCommand("USP_Admin_CompayDocumentType", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CompanyDocumentTypeModel documentTypeModel = new CompanyDocumentTypeModel();
                    documentTypeModel.ID = Convert.ToInt32(reader["ID"].ToString());
                    documentTypeModel.DocumentName = reader["Name"].ToString();
                    documentTypeModel.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    documentTypeModel.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    documentTypeModel.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    documentTypeModel.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    models.Add(documentTypeModel);
                }
                con.Close();
                return models;
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
        /// Get Company Document Type Details By Id                                                                                     -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CompanyDocumentTypeModel GetCompanyDocumentById(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                CompanyDocumentTypeModel documentTypeModel = new CompanyDocumentTypeModel();
                SqlCommand command = new SqlCommand("USP_Admin_CompayDocumentType", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@Id", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    documentTypeModel.ID = Convert.ToInt32(reader["ID"].ToString());
                    documentTypeModel.DocumentName = reader["Name"].ToString();
                    documentTypeModel.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    documentTypeModel.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    documentTypeModel.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    documentTypeModel.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                }
                con.Close();
                return documentTypeModel;
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
        /// Create New Company Document Type                                                                                            -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AddCompanyDocumentType(CompanyDocumentTypeModel documentType)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CompayDocumentType", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@Name", documentType.DocumentName);
                command.Parameters.AddWithValue("@UserId", documentType.CreatedBy);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                return _returnId;
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
        /// Update Company Document Type Details                                                                                        -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateCompanyDocumentType(CompanyDocumentTypeModel documentType)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CompayDocumentType", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Update);
                command.Parameters.AddWithValue("@Id", documentType.ID);
                command.Parameters.AddWithValue("@Name", documentType.DocumentName);
                command.Parameters.AddWithValue("@UserId", documentType.CreatedBy);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                return _returnId;
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
        /// Remove Company Document Type                                                                                                -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemoveCompanyDocumentType(int Id, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_CompayDocumentType", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();
                return _returnId;
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
