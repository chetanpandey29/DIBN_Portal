using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class FileUploaderService : IFileUploaderService
    {
        private readonly Connection _dataSetting;
        public FileUploaderService(Connection dataSetting)
        { 
            _dataSetting = dataSetting;
        }
        public List<UserDocumentsViewModel> GetAllDocuments(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserDocumentsViewModel> documents = new List<UserDocumentsViewModel>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyDocuments", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId",CompanyId);
                con.Open();
                SqlDataReader dr=cmd.ExecuteReader();
                while (dr.Read())
                {
                    UserDocumentsViewModel document = new UserDocumentsViewModel();
                    document.Id = Convert.ToInt32(dr["Id"].ToString());
                    document.FileName = dr["FileName"].ToString();
                    document.Extension = dr["Extension"].ToString();
                    document.Title = dr["Title"].ToString();
                    document.Description = dr["Description"].ToString();
                    document.IssueDate = dr["IssueDate"].ToString();
                    document.ExpiryDate = dr["ExpiryDate"].ToString();
                    document.AuthorityName = dr["AuthorityName"].ToString();
                    document.SelectedDocumentType = Convert.ToInt32(dr["DocumentTypeId"].ToString());
                    document.DocumentTypeName = dr["DocumentTypeName"].ToString();
                    documents.Add(document);    
                }
                con.Close();
                return documents;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally{
                con.Close();
            }
        }
        public List<UserDocumentsViewModel> GetAllDocumentOfEmployee(int CompanyId,int UserId,int SelectedDocumentId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserDocumentsViewModel> documents = new List<UserDocumentsViewModel>();
                SqlCommand cmd = new SqlCommand("USP_User_GetDocumentOfEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@SelectedDocumentType", SelectedDocumentId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UserDocumentsViewModel document = new UserDocumentsViewModel();
                    document.Id = Convert.ToInt32(dr["Id"].ToString());
                    document.FileName = dr["FileName"].ToString();
                    document.Extension = dr["Extension"].ToString();
                    document.Title = dr["Title"].ToString();
                    document.Description = dr["Description"].ToString();
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
        public int UploadSelectedFile(UserDocumentsViewModel document,int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                string _getName = document.formFile.FileName;
                var Name = document.formFile.FileName.Split(".");
                string FileName = Name[0];
                var extn = Path.GetExtension(_getName);
                
                Byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    document.formFile.OpenReadStream().CopyTo(ms);
                    bytes = ms.ToArray();
                }

                SqlCommand cmd = new SqlCommand("USP_User_SaveDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FileName", FileName);
                cmd.Parameters.AddWithValue("@Extension",extn);
                cmd.Parameters.AddWithValue("@DataBinary",bytes);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@Title", document.Title);
                cmd.Parameters.AddWithValue("@Description", document.Description);

                con.Open();
                returnId = (int)cmd.ExecuteScalar();
                con.Close();

                return returnId;
            }   
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public UserDocumentsViewModel DownloadDocument(int Id,int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                UserDocumentsViewModel document = new UserDocumentsViewModel();
                SqlCommand cmd = new SqlCommand("USP_User_GetDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@Id", Id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    document.Id = Convert.ToInt32(dr["Id"].ToString());
                    document.FileName = dr["FileName"].ToString();
                    document.Extension = dr["Extension"].ToString();
                    document.Data = (Byte[])dr["DataBinary"];
                    document.Title = dr["Title"].ToString();
                    document.Description = dr["Description"].ToString();
                }
                con.Close();
                return document;
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
        public UserDocumentsViewModel DownloadCompanyDocuments(int Id, int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                UserDocumentsViewModel document = new UserDocumentsViewModel();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCompanyDocuments", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    document.Id = Convert.ToInt32(dr["Id"].ToString());
                    document.FileName = dr["FileName"].ToString();
                    document.Extension = dr["Extension"].ToString();
                    document.Data = (Byte[])dr["DataBinary"];
                    document.Title = dr["Title"].ToString();
                    document.Description = dr["Description"].ToString();
                    document.IssueDate = dr["IssueDate"].ToString();
                    document.ExpiryDate = dr["ExpiryDate"].ToString();
                    document.AuthorityName = dr["AuthorityName"].ToString();
                    document.SelectedDocumentType = Convert.ToInt32(dr["DocumentTypeId"].ToString());
                }
                con.Close();
                return document;
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
        public List<UserDocumentsViewModel> GetAllDocumentsByUserId(int CompanyId, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<UserDocumentsViewModel> documents = new List<UserDocumentsViewModel>();
                SqlCommand cmd = new SqlCommand("USP_User_GetDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UserDocumentsViewModel document = new UserDocumentsViewModel();
                    document.Id = Convert.ToInt32(dr["Id"].ToString());
                    document.FileName = dr["FileName"].ToString();
                    document.Extension = dr["Extension"].ToString();
                    document.Title = dr["Title"].ToString();
                    document.Description = dr["Description"].ToString();
                    document.CompanyId = Convert.ToInt32(dr["CompanyId"].ToString());
                    document.UserId = Convert.ToInt32(dr["UserId"].ToString());
                    document.SelectedDocumentType = Convert.ToInt32(dr["DocumentTypeId"].ToString());
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
    }
}
