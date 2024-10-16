using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly Connection _dataSetting;
        public LogRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        public List<LogsModel> GetLogs()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<LogsModel> logs = new List<LogsModel>();
                SqlCommand command = new SqlCommand("USP_Admin_GetLogs", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    LogsModel model = new LogsModel();
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.Message = reader["Message"].ToString();
                    model.MessageTemplate = reader["MessageTemplate"].ToString();
                    model.Exception = reader["Exception"].ToString();
                    model.Ip = reader["IP"].ToString();
                    model.Level = reader["Level"].ToString();
                    model.TimeStamp = reader["TimeStamp"].ToString();
                    model.LogEvent = reader["LogEvent"].ToString();
                    model.Username = reader["Username"].ToString();
                    logs.Add(model);
                }
                connection.Close();

                return logs;
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
        public int RemoveLogDetails(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 1;
                
                SqlCommand cmd = new SqlCommand("USP_Admin_RemoveSelectedLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", Id);
                con.Open();
                cmd.ExecuteNonQuery();
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

        public int RemoveAllLogDetails()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 1;
                SqlCommand cmd = new SqlCommand("USP_Admin_RemoveSelectedLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();
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
