using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class EnquiryFormRepository : IEnquiryFormRepository
    {
        private readonly Connection _dataSetting;

        public EnquiryFormRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        public List<EnquiryFormDetails> GetEnquiryFormDetails(string FromDate,string ToDate)
        {
            SqlConnection sqlConnection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EnquiryFormDetails> enquiryFormDetails = new List<EnquiryFormDetails>();
                SqlCommand command = new SqlCommand("USP_GetEnquiryFormDetailOfWeb",sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                if(FromDate!=null && ToDate != null)
                {
                    command.Parameters.AddWithValue("@StartDate", FromDate);
                    command.Parameters.AddWithValue("@EndDate", ToDate);
                }
                sqlConnection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EnquiryFormDetails formDetails = new EnquiryFormDetails();
                    formDetails.Id = Convert.ToInt32(reader["Id"]);
                    formDetails.EnquiryNumber = reader["EnquiryNumber"].ToString();
                    formDetails.ClientEnquiryNumber = reader["ClientEnquiryNumber"].ToString();
                    formDetails.EmailFrom = reader["FromEmail"].ToString();
                    formDetails.EmailTo = reader["ToEmail"].ToString();
                    formDetails.Name = reader["Name"].ToString();
                    formDetails.ContactNumber = reader["ContactNumber"].ToString();
                    formDetails.Description = reader["Description"].ToString();
                    formDetails.EnquiryFor = reader["EnquiryFor"].ToString();
                    formDetails.CreatedOn = reader["CreatedOn"].ToString();
                    formDetails.Status = reader["Status"].ToString();
                    formDetails.AssignedUser = reader["AssignedUser"].ToString();
                    formDetails.AssignedOn = reader["AssignOn"].ToString();
                    enquiryFormDetails.Add(formDetails);
                }
                sqlConnection.Close();
                return enquiryFormDetails;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        public List<int> GetAllAssignedUsers(int RequestId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<int> _returnList = new List<int>();
                string query = "SELECT UserId AS UserId FROM [dbo].[Tbl_AssignEnquiryRequests] tbl_assignEnquiryRequests " +
                    "WHERE RequestId = " + RequestId;

                SqlCommand command = new SqlCommand(query, con);
                command.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    _returnList.Add(Convert.ToInt32(reader["UserId"]));
                }
                con.Close();

                return _returnList;
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
        public int SaveAssignUserOfService(SaveAssignUserForEnquiry saveAssignUser)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                string _query = "", _assignedUsers = "";
                SqlCommand command = null;

                _query = "IF EXISTS(SELECT * FROM [dbo].[Tbl_AssignEnquiryRequests] WHERE RequestId = " + saveAssignUser.RequestId + ") " +
                    "BEGIN " +
                    " DELETE FROM [dbo].[Tbl_AssignEnquiryRequests] WHERE RequestId = " + saveAssignUser.RequestId + "" +
                    " END";

                command = new SqlCommand(_query, con);
                command.CommandType = CommandType.Text;

                con.Open();
                command.ExecuteNonQuery();
                con.Close();

                command.Parameters.Clear();
                for (int index = 0; index < saveAssignUser.UserId.Length; index++)
                {
                    _query = "SELECT FirstName +' '+ LastName FROM [dbo].[Tbl_User] WHERE Id=" + saveAssignUser.UserId[index];

                    command.Parameters.Clear();
                    command = new SqlCommand(_query, con);
                    command.CommandType = CommandType.Text;
                    con.Open();
                    if (_assignedUsers == "")
                    {
                        _assignedUsers = (string)command.ExecuteScalar();
                    }
                    else
                    {
                        _assignedUsers = _assignedUsers + " , " + (string)command.ExecuteScalar();
                    }
                    con.Close();
                }

                if (saveAssignUser.UserId != null)
                {
                    if (saveAssignUser.UserId.Length > 0)
                    {
                        command = new SqlCommand("USP_Admin_SaveAssignEnquiryFormRequestUser", con);
                        command.CommandType = CommandType.StoredProcedure;
                        for (int index = 0; index < saveAssignUser.UserId.Length; index++)
                        {
                            command.Parameters.AddWithValue("@UserId", saveAssignUser.UserId[index]);
                            command.Parameters.AddWithValue("@RequestId", saveAssignUser.RequestId);

                            con.Open();
                            _returnId = (int)command.ExecuteScalar();
                            con.Close();
                            command.Parameters.Clear();
                        }
                    }
                }
                command.Parameters.Clear();
                string createdDate = ""; string Title = "";
                
                command = new SqlCommand("USP_Admin_GetArabianCurrentDateTime", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;

                con.Open();
                createdDate = (string)command.ExecuteScalar();
                con.Close();

                command.Parameters.Clear();


                command = new SqlCommand("USP_Admin_SaveRequestNotifications", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                for (int index = 0; index < saveAssignUser.UserId.Length; index++)
                {
                    command.Parameters.AddWithValue("@Type", "Enquiry Request");
                    command.Parameters.AddWithValue("@RequestCreatedOn", createdDate);
                    command.Parameters.AddWithValue("@AssignedOn", createdDate);
                    command.Parameters.AddWithValue("@EnquiryNumber", saveAssignUser.EnquiryNumber);
                    command.Parameters.AddWithValue("@AssignedUserId", saveAssignUser.UserId[index]);
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@Description", "Your request is now assigned to " + _assignedUsers + ".");
                    command.Parameters.AddWithValue("@ResponseType", "Response");
                    command.Parameters.AddWithValue("@ResponseCreatedBy", saveAssignUser.CreatedBy);
                    command.Parameters.AddWithValue("@AssignedBy", saveAssignUser.CreatedBy);

                    con.Open();
                    int _notification = (int)command.ExecuteScalar();
                    con.Close();

                    command.Parameters.Clear();
                }

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
        public string GetStatusOfEnquiryForm(int RequestId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                string lastStatus = "";
                string query = "SELECT ISNULL(enquiryForm.Status,'N/A') AS [Status] FROM [dbo].[Tbl_EnquiryFormDetails] enquiryForm " +
                    "WHERE enquiryForm.Id = " + RequestId + " AND (enquiryForm.IsDelete <> 1 OR enquiryForm.IsDelete IS NULL)";

                SqlCommand command = new SqlCommand(query, con);
                command.CommandType = CommandType.Text;

                con.Open();
                lastStatus = (string) command.ExecuteScalar();
                con.Close();

                return lastStatus;
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
        public int SaveStatusOfEnquiryForm(ChangeEnquiryStatusModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                string query = "UPDATE [dbo].[Tbl_EnquiryFormDetails] SET " +
                    "Status = '"+model.Status+"' " +
                    "WHERE Id = "+model.RequestId +" " +
                    "SELECT 1";
                SqlCommand command = new SqlCommand(query, con);
                command.CommandType = CommandType.Text;

                con.Open();
                returnId = (int)command.ExecuteScalar();
                con.Close();

                command.Parameters.Clear();
                string createdDate = ""; string Title = "";
                command = new SqlCommand("USP_Admin_GetArabianCurrentDateTime", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;

                con.Open();
                createdDate = (string)command.ExecuteScalar();
                con.Close();

                command.Parameters.Clear();

                command = new SqlCommand("USP_Admin_SaveRequestNotifications", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;

                command.Parameters.AddWithValue("@Type", "Enquiry Request");
                command.Parameters.AddWithValue("@RequestCreatedOn", createdDate);
                command.Parameters.AddWithValue("@AssignedOn", createdDate);
                command.Parameters.AddWithValue("@EnquiryNumber", model.EnquiryNumber);
                command.Parameters.AddWithValue("@Title", Title);
                command.Parameters.AddWithValue("@Description", "Status Changed.");
                command.Parameters.AddWithValue("@ResponseType", "Response");
                command.Parameters.AddWithValue("@ResponseCreatedBy", model.UserId);

                con.Open();
                int _notification = (int)command.ExecuteScalar();
                con.Close();

                command.Parameters.Clear();


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
        public int DeleteEnquiry(int RequestId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int returnId = 0;
                string query = "UPDATE [dbo].[Tbl_EnquiryFormDetails] SET " +
                    "IsDelete = 1 " +
                    "WHERE Id = " + RequestId + " " +
                    "SELECT 1";
                SqlCommand command = new SqlCommand(query, con);
                command.CommandType = CommandType.Text;

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
    }
}
