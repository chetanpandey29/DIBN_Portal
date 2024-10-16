using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class EnquiryFormService : IEnquiryFormService
    {
        private readonly Connection _dataSetting;

        public EnquiryFormService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        public List<EnquiryFormDetailsModel> GetEnquiryFormDetails(int UserId)
        {
            SqlConnection sqlConnection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<EnquiryFormDetailsModel> enquiryFormDetails = new List<EnquiryFormDetailsModel>();
                SqlCommand command = new SqlCommand("USP_GetAssignedEnquiryFormDetailOfWeb", sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                
                command.Parameters.AddWithValue("@UserId", UserId);
                
                sqlConnection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EnquiryFormDetailsModel formDetails = new EnquiryFormDetailsModel();
                    formDetails.Id = Convert.ToInt32(reader["Id"]);
                    formDetails.EnquiryNumber = Convert.ToString(reader["EnquiryNumber"]);
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
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
                lastStatus = (string)command.ExecuteScalar();
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
                    "Status = '" + model.Status + "' " +
                    "WHERE Id = " + model.RequestId + " " +
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
    }
}
