using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class RMTeamManagementRepository : IRMTeamManagementRepository
    {
        private readonly Connection _dataSetting;
        private readonly EncryptionService _encryptionService;

        public RMTeamManagementRepository(Connection dataSetting, EncryptionService encryptionService)
        {
            _dataSetting = dataSetting;
            _encryptionService = encryptionService;
        }
        /// <summary>
        /// Get RM Team with pagination                                                                                                                     -- Yashasvi (10-10-2024)
        /// </summary>
        /// <param name="fetchRows"></param>
        /// <param name="skipRows"></param>
        /// <param name="searchPrefix"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortingDirection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GetRMTeamListWithPaginationModel> GetRMTeamListWithPagination(int fetchRows,int skipRows,string? searchPrefix,string? sortBy,string? sortingDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                GetRMTeamListWithPaginationModel model = new GetRMTeamListWithPaginationModel();
                List<GetRMTeamListModel> rmTeamList = new List<GetRMTeamListModel>();
                int index = 0,totalCount =0;
                if (skipRows > 0)
                    index = skipRows + 1;
                else
                    index = 1;

                SqlCommand command = new SqlCommand("USP_Admin_Select_GetRMTeamListWithPagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@fetchRows", fetchRows);
                command.Parameters.AddWithValue("@skipRows", skipRows);
                if (searchPrefix != null && searchPrefix != "")
                    command.Parameters.AddWithValue("@searchPrefix", searchPrefix);
                if (sortBy != null && sortBy != "")
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                if (sortingDirection != null && sortingDirection != "")
                    command.Parameters.AddWithValue("@sortDirection", sortingDirection);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while(reader.Read())
                {
                    GetRMTeamListModel rmTeam = new GetRMTeamListModel();
                    if (reader["Id"] != DBNull.Value)
                        rmTeam.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["Name"] != DBNull.Value && reader["Name"].ToString() != "N/A")
                        rmTeam.Name = reader["Name"].ToString();
                    if (reader["EmailAddress"] != DBNull.Value && reader["EmailAddress"].ToString() != "N/A")
                        rmTeam.EmailAddress = reader["EmailAddress"].ToString();
                    if (reader["MobileNumber"] != DBNull.Value && reader["MobileNumber"].ToString() != "N/A")
                        rmTeam.MobileNumber = reader["MobileNumber"].ToString();
                    if (reader["Company"] != DBNull.Value && reader["Company"].ToString() != "N/A")
                        rmTeam.Company = reader["Company"].ToString();
                    if (reader["IsLoginAllowed"] != DBNull.Value)
                        rmTeam.IsLoginAllowed = Convert.ToBoolean(reader["IsLoginAllowed"]);
                    if (reader["IsActive"] != DBNull.Value)
                        rmTeam.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    rmTeam.Index = index;
                    index++;
                    rmTeamList.Add(rmTeam);
                }

                await reader.NextResultAsync();
                
                while (reader.Read())
                {
                    if (reader["TotalCount"] != DBNull.Value)
                        totalCount = Convert.ToInt32(reader["TotalCount"]);
                }
                connection.Close();

                model.getRMTeamsList = rmTeamList;
                model.totalCount = totalCount;
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
        /// Save RM Team Details                                                                                                                       -- Yashasvi (10-10-2024)                             
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> SaveRMTeamDetails(SaveRMTeamModel model)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_Insert_Update_RmTeam", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Firstname", model.Firstname);
                if (model.Lastname != null && model.Lastname != "")
                    command.Parameters.AddWithValue("@Lastname", model.Lastname);
                command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                command.Parameters.AddWithValue("@MobileCode", model.MobileCode);
                command.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                command.Parameters.AddWithValue("@CountryOfResidence", model.CountryofResidence);
                if (model.PassportNumber != null && model.PassportNumber != "")
                    command.Parameters.AddWithValue("@PassportNo", model.PassportNumber);
                if (model.PassportExpiryDate != null && model.PassportExpiryDate != "")
                    command.Parameters.AddWithValue("@PassportExpiryDate", model.PassportExpiryDate);
                if (model.VisaExpiryDate != null && model.VisaExpiryDate != "")
                    command.Parameters.AddWithValue("@VisaExpiryDate", model.VisaExpiryDate);
                if (model.Designation != null && model.Designation != "")
                    command.Parameters.AddWithValue("@Designation", model.Designation);
                if (model.InsuranceCompany != null && model.InsuranceCompany != "")
                    command.Parameters.AddWithValue("@InsuranceCompany", model.InsuranceCompany);
                if (model.InsuranceExpiryDate != null && model.InsuranceExpiryDate != "")
                    command.Parameters.AddWithValue("@InsuranceExpiryDate", model.InsuranceExpiryDate);
                command.Parameters.AddWithValue("@IsLoginAllowed", model.IsLoginAllowed);
                command.Parameters.AddWithValue("@IsActive", model.IsActive);
                command.Parameters.AddWithValue("@UserId", model.UserId);
                if(model.Password != null && model.Password != "")
                {
                    string password = _encryptionService.EncryptText(model.Password);
                    command.Parameters.AddWithValue("@Password", password);
                }

                connection.Open();
                _returnId = Convert.ToInt32(await command.ExecuteScalarAsync());    
                connection.Close();

                if(_returnId > 0)
                {
                    if (model.CompanyId != null)
                    {
                        if (model.CompanyId.Count > 0)
                        {
                            for (int index = 0; index < model.CompanyId.Count; index++)
                            {
                                command.Parameters.Clear();
                                command = new SqlCommand("USP_Admin_Insert_AssignedCompanyRMTeam", connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Id", _returnId);
                                command.Parameters.AddWithValue("@CompanyId", model.CompanyId[index]);
                                command.Parameters.AddWithValue("@UserId", model.UserId);

                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                                connection.Close();
                            }
                        }
                    }
                }
                
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
        /// Update RM Team Details                                                                                                                    -- Yashasvi (10-10-2024)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> UpdateRMTeamDetails(UpdateRMTeamModel model)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_Insert_Update_RmTeam", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@Firstname", model.Firstname);
                if (model.Lastname != null && model.Lastname != "")
                    command.Parameters.AddWithValue("@Lastname", model.Lastname);
                command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                command.Parameters.AddWithValue("@MobileCode", model.MobileCode);
                command.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                command.Parameters.AddWithValue("@CountryOfResidence", model.CountryofResidence);
                if (model.PassportNumber != null && model.PassportNumber != "")
                    command.Parameters.AddWithValue("@PassportNo", model.PassportNumber);
                if (model.PassportExpiryDate != null && model.PassportExpiryDate != "")
                    command.Parameters.AddWithValue("@PassportExpiryDate", model.PassportExpiryDate);
                if (model.VisaExpiryDate != null && model.VisaExpiryDate != "")
                    command.Parameters.AddWithValue("@VisaExpiryDate", model.VisaExpiryDate);
                if (model.Designation != null && model.Designation != "")
                    command.Parameters.AddWithValue("@Designation", model.Designation);
                if (model.InsuranceCompany != null && model.InsuranceCompany != "")
                    command.Parameters.AddWithValue("@InsuranceCompany", model.InsuranceCompany);
                if (model.InsuranceExpiryDate != null && model.InsuranceExpiryDate != "")
                    command.Parameters.AddWithValue("@InsuranceExpiryDate", model.InsuranceExpiryDate);
                command.Parameters.AddWithValue("@IsLoginAllowed", model.IsLoginAllowed);
                command.Parameters.AddWithValue("@IsActive", model.IsActive);
                command.Parameters.AddWithValue("@UserId", model.UserId);
                if (model.Password != null && model.Password != "")
                {
                    string password = _encryptionService.EncryptText(model.Password);
                    command.Parameters.AddWithValue("@Password", password);
                }

                connection.Open();
                _returnId = Convert.ToInt32(await command.ExecuteScalarAsync());
                connection.Close();

                if (_returnId > 0)
                {
                    if (model.CompanyId != null)
                    {
                        if (model.CompanyId.Count > 0)
                        {
                            for (int index = 0; index < model.CompanyId.Count; index++)
                            {
                                command.Parameters.Clear();
                                command = new SqlCommand("USP_Admin_Insert_AssignedCompanyRMTeam", connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@Id", model.Id);
                                command.Parameters.AddWithValue("@CompanyId", model.CompanyId[index]);
                                command.Parameters.AddWithValue("@UserId", model.UserId);

                                connection.Open();
                                await command.ExecuteNonQueryAsync();
                                connection.Close();
                            }
                        }
                    }
                }
                
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
        /// Get RM Team Details For Update                                                                                                              -- Yashavi (10-10-2024)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UpdateRMTeamModel> GetRMTeamDetails(int Id)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                UpdateRMTeamModel model = new UpdateRMTeamModel();
                List<int> companyId = new List<int>();
                SqlCommand command = new SqlCommand("USP_Admin_Select_GetRMTeamDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["Firstname"] != DBNull.Value && reader["Firstname"].ToString() != "N/A")
                        model.Firstname = reader["Firstname"].ToString();
                    if (reader["Lastname"] != DBNull.Value && reader["Lastname"].ToString() != "N/A")
                        model.Lastname = reader["Lastname"].ToString();
                    if (reader["Password"] != DBNull.Value)
                    {
                        string password = reader["Password"].ToString();
                        password = _encryptionService.DecryptText(password);
                        model.Password = password;
                    }
                    if (reader["EmailAddress"] != DBNull.Value && reader["EmailAddress"].ToString() != "N/A")
                        model.EmailAddress = reader["EmailAddress"].ToString();
                    if (reader["MobileCode"] != DBNull.Value && reader["MobileCode"].ToString() != "N/A")
                        model.MobileCode = reader["MobileCode"].ToString();
                    if (reader["MobileNumber"] != DBNull.Value && reader["MobileNumber"].ToString() != "N/A")
                        model.MobileNumber = reader["MobileNumber"].ToString();
                    if (reader["CountryOfResidence"] != DBNull.Value && reader["CountryOfResidence"].ToString() != "N/A")
                        model.CountryofResidence = reader["CountryOfResidence"].ToString();
                    if (reader["PassportNumber"] != DBNull.Value && reader["PassportNumber"].ToString() != "N/A")
                        model.PassportNumber = reader["PassportNumber"].ToString();
                    if (reader["PassportExpiryDate"] != DBNull.Value && reader["PassportExpiryDate"].ToString() != "N/A")
                        model.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    if (reader["VisaExpiryDate"] != DBNull.Value && reader["VisaExpiryDate"].ToString() != "N/A")
                        model.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    if (reader["Designation"] != DBNull.Value && reader["Designation"].ToString() != "N/A")
                        model.Designation = reader["Designation"].ToString();
                    if (reader["InsuranceCompany"] != DBNull.Value && reader["InsuranceCompany"].ToString() != "N/A")
                        model.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    if (reader["InsuranceExpiryDate"] != DBNull.Value && reader["InsuranceExpiryDate"].ToString() != "N/A")
                        model.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    if (reader["IsLoginAllowed"] != DBNull.Value)
                        model.IsLoginAllowed = Convert.ToBoolean(reader["IsLoginAllowed"]);
                    if (reader["IsActive"] != DBNull.Value)
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                }
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_Select_GetAssignedCompanyToRMTeam", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RmTeamId", model.Id);

                connection.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["CompanyId"] != DBNull.Value)
                        if (!companyId.Contains(Convert.ToInt32(reader["CompanyId"])))
                            companyId.Add(Convert.ToInt32(reader["CompanyId"]));
                }
                connection.Close();
                model.CompanyId = companyId;
                return model;
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
        /// Delete                                                                                                                                              -- Yashasvi (10-10-2024)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Delete(int Id,int UserId)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = new SqlCommand("USP_Admin_Delete_RMTeam", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@UserId", UserId);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
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
        /// Get RM Team for Company creation                                                                                                                                            -- Yashasvi (29-10-2024)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<GetAllRMPersonModel>> GetAllRMPersonsForCompany()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetAllRMPersonModel> rmPersonList = new List<GetAllRMPersonModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetAllRMTeamPersonForCompany", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read()) 
                {
                    GetAllRMPersonModel model =new GetAllRMPersonModel();

                    if (reader["Id"] != DBNull.Value)
                        model.Id = Convert.ToInt32(reader["Id"]);
                    if (reader["RMPerson"] != DBNull.Value)
                        model.RmTeamName = reader["RMPerson"].ToString();

                    rmPersonList.Add(model);
                }
                connection.Close();

                return rmPersonList;
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
    }
}
