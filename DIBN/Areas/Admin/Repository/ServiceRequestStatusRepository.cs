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
    public class ServiceRequestStatusRepository : IServiceRequestStatusRepository
    {
        private readonly Connection _dataSetting;
        public ServiceRequestStatusRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        public List<ServiceRequestStatusModel> GetServiceRequestStatus()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ServiceRequestStatusModel> model = new List<ServiceRequestStatusModel>();

                SqlCommand command = new SqlCommand("USP_Admin_ServiceRequestStatusOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Show);

                con.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ServiceRequestStatusModel data = new ServiceRequestStatusModel();
                    data.Id = Convert.ToInt32(reader["Id"]);
                    data.DisplayId = Convert.ToInt32(reader["DisplayId"]);
                    data.DisplayName = Convert.ToString(reader["Status"]);
                    data.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    data.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    data.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    data.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
                    data.CreatedOn = Convert.ToString(reader["CreatedOn"]);
                    data.ModifyOn = Convert.ToString(reader["ModifyOn"]);
                    model.Add(data);
                }

                con.Close();

                return model;
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
        public ServiceRequestStatusModel GetServiceRequestStatusById(int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ServiceRequestStatusModel data = new ServiceRequestStatusModel();

                SqlCommand command = new SqlCommand("USP_Admin_ServiceRequestStatusOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.GetById);
                command.Parameters.AddWithValue("@Id", Id);

                con.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    data.Id = Convert.ToInt32(reader["Id"]);
                    data.DisplayId = Convert.ToInt32(reader["DisplayId"]);
                    data.DisplayName = Convert.ToString(reader["Status"]);
                    data.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    data.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    data.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    data.ModifyBy = Convert.ToInt32(reader["ModifyBy"]);
                    data.CreatedOn = Convert.ToString(reader["CreatedOn"]);
                    data.ModifyOn = Convert.ToString(reader["ModifyOn"]);
                }

                con.Close();

                return data;
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

        public int SaveServiceRequestStatus(ServiceRequestStatusModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_ServiceRequestStatusOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@Name", model.DisplayName);
                command.Parameters.AddWithValue("@DisplayId", model.DisplayId);
                command.Parameters.AddWithValue("@UserId", model.UserId);

                con.Open();
                _returnId = (int)command.ExecuteScalar();
                con.Close();

                return _returnId;
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
        public int UpdateServiceRequestStatus(ServiceRequestStatusModel model)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_ServiceRequestStatusOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Operation.Insert);
                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@DisplayId", model.DisplayId);
                command.Parameters.AddWithValue("@Name", model.DisplayName);
                command.Parameters.AddWithValue("@UserId", model.UserId);

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
        public int DeleteServiceRequestStatus(int Id,int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_ServiceRequestStatusOperation", con);
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
        public int GetlastDisplayId()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;

                SqlCommand command = new SqlCommand("USP_Admin_ServiceRequestStatusOperation", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", 5);

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
