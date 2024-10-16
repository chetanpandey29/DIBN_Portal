using DIBN.Areas.Admin.Data;
using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class UserShareholderService : IUserShareholderService
    {
        private readonly Connection _dataSetting;
        public UserShareholderService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        public List<ShareholderViewModel> GetAllShareholders(int? Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ShareholderViewModel> shareholders = new List<ShareholderViewModel>();
                SqlCommand command = new SqlCommand("USP_Company_GetAllShareholderByCompanyId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyId", Id);
                
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
                    shareholder.Share = reader["SharePercentage"].ToString();
                    shareholder.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    shareholder.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    shareholder.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    shareholder.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    shareholder.CompanyId = (int)Id;
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
                    shareholder.Share = reader["SharePercentage"].ToString();
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
                command.Parameters.AddWithValue("@Passport_Number", shareholder.PassportNumber);
                command.Parameters.AddWithValue("@PassportExpiryDate", shareholder.PassportExpiryDate);
                command.Parameters.AddWithValue("@Nationality", shareholder.Nationality);
                command.Parameters.AddWithValue("@Designation", shareholder.Designation);
                command.Parameters.AddWithValue("@VisaExpiryDate", shareholder.VisaExpiryDate);
                command.Parameters.AddWithValue("@InsuranceCompany", shareholder.InsuranceCompany);
                command.Parameters.AddWithValue("@InsuranceExpiryDate", shareholder.InsuranceExpiryDate);
                command.Parameters.AddWithValue("@IsActive", shareholder.IsActive);
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
                command.Parameters.AddWithValue("@Passport_Number", shareholder.PassportNumber);
                command.Parameters.AddWithValue("@PassportExpiryDate", shareholder.PassportExpiryDate);
                command.Parameters.AddWithValue("@Nationality", shareholder.Nationality);
                command.Parameters.AddWithValue("@Designation", shareholder.Designation);
                command.Parameters.AddWithValue("@VisaExpiryDate", shareholder.VisaExpiryDate);
                command.Parameters.AddWithValue("@InsuranceCompany", shareholder.InsuranceCompany);
                command.Parameters.AddWithValue("@InsuranceExpiryDate", shareholder.InsuranceExpiryDate);
                command.Parameters.AddWithValue("@IsActive", shareholder.IsActive);
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
        public int RemoveShareholder(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ShareholderOrDirectorOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Delete);
                command.Parameters.AddWithValue("@ID", Id);
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
                if (Id != null)
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
        public DataSet GetShareholdersForExport(int ID)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetAllShareholdersDetailsForExport_Cmp", con);
                cmd.CommandType = CommandType.StoredProcedure;
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
    }
}
