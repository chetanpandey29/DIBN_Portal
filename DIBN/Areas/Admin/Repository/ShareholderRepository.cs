using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class ShareholderRepository : IShareholderRepository
    {
        private readonly Connection _dataSetting;
        public ShareholderRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        /// <summary>
        /// Get All Shareholder List                                                                                                        -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<ShareholderViewModel> GetAllShareholders(int companyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ShareholderViewModel> shareholders = new List<ShareholderViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_ShareholderOrDirectorOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                command.Parameters.AddWithValue("@CompanyId", companyId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ShareholderViewModel shareholder = new ShareholderViewModel();
                    shareholder.ID = Convert.ToInt32(reader["ID"]);
                    shareholder.FirstName = reader["FirstName"].ToString();
                    shareholder.LastName = reader["LastName"].ToString();
                    shareholder.PassportNumber = reader["PassportNumber"].ToString();
                    shareholder.Nationality = reader["Nationality"].ToString();
                    shareholder.Designation = reader["Designation"].ToString();
                    shareholder.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    shareholder.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    shareholder.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    shareholder.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    shareholder.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                    shareholder.Company = reader["CompanyName"].ToString();
                    shareholder.SharePercentage = reader["SharePercentage"].ToString();
                    shareholder.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    shareholder.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    shareholder.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    shareholder.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    shareholders.Add(shareholder);
                }
                con.Close();
                return shareholders;
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
        /// Get Details Of Shareholder By Id                                                                                                -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ShareholderViewModel GetDetailsOfShareholder(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ShareholderViewModel shareholder = new ShareholderViewModel();
                SqlCommand command = new SqlCommand("USP_Admin_ShareholderOrDirectorOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("ID", Id);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    shareholder.ID = Convert.ToInt32(reader["ID"]);
                    shareholder.FirstName = reader["FirstName"].ToString();
                    shareholder.LastName = reader["LastName"].ToString();
                    shareholder.PassportNumber = reader["PassportNumber"].ToString();
                    shareholder.Nationality = reader["Nationality"].ToString();
                    shareholder.Designation = reader["Designation"].ToString();
                    shareholder.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    shareholder.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    shareholder.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    shareholder.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    shareholder.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                    shareholder.Company = reader["CompanyName"].ToString();
                    shareholder.SharePercentage = reader["SharePercentage"].ToString();
                    shareholder.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    shareholder.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    shareholder.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    shareholder.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                }
                con.Close();
                return shareholder;
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
        ///  Create New Shareholder                                                                                                         -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="shareholder"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AddNewShareholder(ShareholderViewModel shareholder)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ShareholderOrDirectorOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@FirstName", shareholder.FirstName);
                command.Parameters.AddWithValue("@LastName", shareholder.LastName);
                if (shareholder.PassportNumber != "N/A" && shareholder.PassportNumber != "")
                    command.Parameters.AddWithValue("@Passport_Number", shareholder.PassportNumber);
                command.Parameters.AddWithValue("@Nationality", shareholder.Nationality);
                command.Parameters.AddWithValue("@Designation", shareholder.Designation);
                if (shareholder.VisaExpiryDate != "N/A" && shareholder.VisaExpiryDate != "")
                    command.Parameters.AddWithValue("@VisaExpiryDate", shareholder.VisaExpiryDate);
                if (shareholder.PassportExpiryDate != "N/A" && shareholder.PassportExpiryDate != "")
                    command.Parameters.AddWithValue("@PassportExpiryDate", shareholder.PassportExpiryDate);
                if (shareholder.InsuranceCompany != "N/A" && shareholder.InsuranceCompany != "")
                    command.Parameters.AddWithValue("@InsuranceCompany", shareholder.InsuranceCompany);
                if (shareholder.InsuranceExpiryDate != "N/A" && shareholder.InsuranceExpiryDate != "")
                    command.Parameters.AddWithValue("@InsuranceExpiryDate", shareholder.InsuranceExpiryDate);
                command.Parameters.AddWithValue("@CompanyId", shareholder.CompanyId);
                command.Parameters.AddWithValue("@Share", shareholder.SharePercentage);
                command.Parameters.AddWithValue("@IsActive", shareholder.IsActive);
                command.Parameters.AddWithValue("@CreatedBy", shareholder.CreatedBy);
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
        /// Update Shareholder Details                                                                                                      -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="shareholder"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateShareholderDetails(ShareholderViewModel shareholder)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ShareholderOrDirectorOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Update);
                command.Parameters.AddWithValue("@ID", shareholder.ID);
                command.Parameters.AddWithValue("@FirstName", shareholder.FirstName);
                command.Parameters.AddWithValue("@LastName", shareholder.LastName);
                if (shareholder.PassportNumber != "N/A" && shareholder.PassportNumber != "")
                    command.Parameters.AddWithValue("@Passport_Number", shareholder.PassportNumber);
                command.Parameters.AddWithValue("@Nationality", shareholder.Nationality);
                command.Parameters.AddWithValue("@Designation", shareholder.Designation);
                if (shareholder.VisaExpiryDate != "N/A" && shareholder.VisaExpiryDate != "")
                    command.Parameters.AddWithValue("@VisaExpiryDate", shareholder.VisaExpiryDate);
                if (shareholder.PassportExpiryDate != "N/A" && shareholder.PassportExpiryDate != "")
                    command.Parameters.AddWithValue("@PassportExpiryDate", shareholder.PassportExpiryDate);
                if (shareholder.InsuranceCompany != "N/A" && shareholder.InsuranceCompany != "")
                    command.Parameters.AddWithValue("@InsuranceCompany", shareholder.InsuranceCompany);
                if (shareholder.InsuranceExpiryDate != "N/A" && shareholder.InsuranceExpiryDate != "")
                    command.Parameters.AddWithValue("@InsuranceExpiryDate", shareholder.InsuranceExpiryDate);
                command.Parameters.AddWithValue("@CompanyId", shareholder.CompanyId);
                command.Parameters.AddWithValue("@Share", shareholder.SharePercentage);
                command.Parameters.AddWithValue("@IsActive", shareholder.IsActive);
                command.Parameters.AddWithValue("@CreatedBy", shareholder.CreatedBy);
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
        /// Remove Shareholder                                                                                                              -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemoveShareholder(int Id, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ShareholderOrDirectorOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@ID", Id);
                command.Parameters.AddWithValue("@CreatedBy", UserId);
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
        /// Upload Document For Shareholder                                                                                                 -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UploadShareholderDocuments(ShareholderDocuments documents)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                string _getName = documents.formFile.FileName;
                int lastIndex = documents.formFile.FileName.LastIndexOf(".");
                String Name = documents.formFile.FileName.Substring(0, lastIndex);
                string FileName = Name;
                var extn = Path.GetExtension(_getName);

                Byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    documents.formFile.OpenReadStream().CopyTo(ms);
                    bytes = ms.ToArray();
                }


                SqlCommand command = new SqlCommand("USP_Admin_ShareholderDocumentsOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@Title", documents.Title);
                command.Parameters.AddWithValue("@FileName", FileName);
                command.Parameters.AddWithValue("@Extension", extn);
                command.Parameters.AddWithValue("@DataBinary", bytes);
                command.Parameters.AddWithValue("@ShareholderId", documents.ShareholderId);
                command.Parameters.AddWithValue("@CreatedBy", documents.CreatedBy);
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
        /// Get List of Shareholder's Document                                                                                              -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<ShareholderDocuments> GetShareholdersDocuments(int? Id, int? DocumentId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ShareholderDocuments> documents = new List<ShareholderDocuments>();
                SqlCommand command = new SqlCommand("USP_Admin_ShareholderDocumentsOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);
                command.Parameters.AddWithValue("@ShareholderId", Id);
                if (DocumentId != null)
                {
                    command.Parameters.AddWithValue("@ID", DocumentId);
                }
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ShareholderDocuments document = new ShareholderDocuments();
                    document.ID = Convert.ToInt32(reader["ID"]);
                    document.Title = reader["Title"].ToString();
                    document.FileName = reader["FileName"].ToString();
                    document.Extension = reader["Extension"].ToString();
                    document.DataBinary = (Byte[])reader["DataBinary"];
                    document.ShareholderId = Convert.ToInt32(reader["ShareholderId"]);
                    document.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    document.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    document.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    document.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    documents.Add(document);
                }
                con.Close();
                return documents;

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
        /// Get Remaing Share Percentage from 100% which we can assign to Shareholder                                                       -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetRemainingSharePercentage(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand command = new SqlCommand("USP_Admin_GetRemainingSharePercentage", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@companyId", CompanyId);
                con.Open();
                int RemainingShare = (int)command.ExecuteScalar();
                con.Close();
                return RemainingShare;
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
        /// Get Shareholder Details for Export PDF / Excel                                                                                  -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetShareholdersForExport(int? ID)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllShareholdersDetailsForExport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (ID != null)
                    cmd.Parameters.AddWithValue("@CompanyId", ID);
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
                con.Close();
                return ds;
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
        /// Remove Documents                                                                                                                -- Yashasvi TBC (28-11-2022)
        /// </summary>
        /// <param name="ShareholderId"></param>
        /// <param name="DocumentId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemoveDocument(int ShareholderId, int DocumentId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = null;
                command = new SqlCommand("USP_Admin_RemoveShareholderDocument", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userId", UserId);
                command.Parameters.AddWithValue("@shareholderId", ShareholderId);
                command.Parameters.AddWithValue("@documentId", DocumentId);
                con.Open();
                command.ExecuteNonQuery();
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
        public async Task<GetAllActiveShareholdersWithPaginationModel> GetAllActiveShareholdersWithPagination(int page, int pageSize,string searchBy, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int index = 0;
                index = page + 1;
                GetAllActiveShareholdersWithPaginationModel model = new GetAllActiveShareholdersWithPaginationModel();
                int totalActiveShareholder = 0;
                List<ShareholderViewModel> shareholders = new List<ShareholderViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllActiveCompanyShareholders", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if(searchString != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                if(sortBy != null)
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                if(sortDirection != null)
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    ShareholderViewModel shareholder = new ShareholderViewModel();
                    shareholder.Index = index;
                    if(reader["ID"] != DBNull.Value)
                        shareholder.ID = Convert.ToInt32(reader["ID"]);
                    if(reader["FirstName"] != DBNull.Value)
                        shareholder.FirstName = reader["FirstName"].ToString();
                    if(reader["LastName"] != DBNull.Value)
                        shareholder.LastName = reader["LastName"].ToString();
                    if(reader["PassportNumber"] != DBNull.Value)
                        shareholder.PassportNumber = reader["PassportNumber"].ToString();
                    if(reader["Nationality"] != DBNull.Value)
                        shareholder.Nationality = reader["Nationality"].ToString();
                    if(reader["Designation"] != DBNull.Value)
                        shareholder.Designation = reader["Designation"].ToString();
                    if(reader["VisaExpiryDate"] != DBNull.Value)
                        shareholder.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    if(reader["InsuranceCompany"] != DBNull.Value)
                        shareholder.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    if(reader["InsuranceExpiryDate"] != DBNull.Value)
                        shareholder.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    if(reader["PassportExpiryDate"] != DBNull.Value)
                        shareholder.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    if(reader["CompanyId"] != DBNull.Value)
                        shareholder.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                    if(reader["CompanyName"] != DBNull.Value)
                        shareholder.Company = reader["CompanyName"].ToString();
                    if(reader["SharePercentage"] != DBNull.Value)
                        shareholder.SharePercentage = reader["SharePercentage"].ToString();
                    if(reader["IsActive"]!= DBNull.Value)
                        shareholder.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    shareholders.Add(shareholder);
                    index++;
                }
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllActiveCompanyShareholdersCount", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (searchString != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchString);

                connection.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["ID"] != DBNull.Value)
                        totalActiveShareholder++;
                }
                connection.Close();

                model.totalActiveShareholder = totalActiveShareholder;
                model.shareholders = shareholders;
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
        public async Task<GetAllInActiveShareholdersWithPaginationModel> GetAllInActiveShareholdersWithPagination(int page, int pageSize, string searchBy, string searchString, string sortBy, string sortDirection)
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int index = 0;
                index = page + 1;
                GetAllInActiveShareholdersWithPaginationModel model = new GetAllInActiveShareholdersWithPaginationModel();
                int totalInActiveShareholder = 0;
                List<ShareholderViewModel> shareholders = new List<ShareholderViewModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetAllInActiveCompanyShareholders", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@skipRows", page);
                command.Parameters.AddWithValue("@RowsOfPage", pageSize);
                if (searchString != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchString);
                if (sortBy != null)
                    command.Parameters.AddWithValue("@sortColumn", sortBy);
                if (sortDirection != null)
                    command.Parameters.AddWithValue("@sortDirection", sortDirection);

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    ShareholderViewModel shareholder = new ShareholderViewModel();
                    shareholder.Index = index;
                    if (reader["ID"] != DBNull.Value)
                        shareholder.ID = Convert.ToInt32(reader["ID"]);
                    if (reader["FirstName"] != DBNull.Value)
                        shareholder.FirstName = reader["FirstName"].ToString();
                    if (reader["LastName"] != DBNull.Value)
                        shareholder.LastName = reader["LastName"].ToString();
                    if (reader["PassportNumber"] != DBNull.Value)
                        shareholder.PassportNumber = reader["PassportNumber"].ToString();
                    if (reader["Nationality"] != DBNull.Value)
                        shareholder.Nationality = reader["Nationality"].ToString();
                    if (reader["Designation"] != DBNull.Value)
                        shareholder.Designation = reader["Designation"].ToString();
                    if (reader["VisaExpiryDate"] != DBNull.Value)
                        shareholder.VisaExpiryDate = reader["VisaExpiryDate"].ToString();
                    if (reader["InsuranceCompany"] != DBNull.Value)
                        shareholder.InsuranceCompany = reader["InsuranceCompany"].ToString();
                    if (reader["InsuranceExpiryDate"] != DBNull.Value)
                        shareholder.InsuranceExpiryDate = reader["InsuranceExpiryDate"].ToString();
                    if (reader["PassportExpiryDate"] != DBNull.Value)
                        shareholder.PassportExpiryDate = reader["PassportExpiryDate"].ToString();
                    if (reader["CompanyId"] != DBNull.Value)
                        shareholder.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                    if (reader["CompanyName"] != DBNull.Value)
                        shareholder.Company = reader["CompanyName"].ToString();
                    if (reader["SharePercentage"] != DBNull.Value)
                        shareholder.SharePercentage = reader["SharePercentage"].ToString();
                    if (reader["IsActive"] != DBNull.Value)
                        shareholder.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    shareholders.Add(shareholder);
                    index++;
                }
                connection.Close();

                command.Parameters.Clear();
                command = new SqlCommand("USP_Admin_GetAllInActiveCompanyShareholdersCount", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (searchString != null)
                    command.Parameters.AddWithValue("@searchPrefix", searchString);

                connection.Open();
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (reader["ID"] != DBNull.Value)
                        totalInActiveShareholder++;
                }
                connection.Close();

                model.totalInActiveShareholder = totalInActiveShareholder;
                model.shareholders = shareholders;
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
    }
}
