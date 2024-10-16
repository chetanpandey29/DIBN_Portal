using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Areas.Admin.Repository
{
    public class ImportReminderNotificationRepository : IImportReminderNotificationRepository
    {
        private readonly Connection _dataSetting;

        public ImportReminderNotificationRepository(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }

        /// <summary>
        /// Get All Notifications                                                                                       -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<ImportReminderNotification>> GetAllServiceNotifications(string service)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ImportReminderNotification> importReminderNotifications = new List<ImportReminderNotification>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_GetAllServiceNotifications", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@service", service);
                con.Open();
                SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
                while (reader.Read())
                {
                    ImportReminderNotification notification = new ImportReminderNotification();

                    notification.ID = Convert.ToInt32(reader["ID"]);
                    notification.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    notification.CompanyName = reader["CompanyName"].ToString();
                    notification.UserId = Convert.ToInt32(reader["UserId"]);
                    notification.Username = reader["Username"].ToString();
                    notification.Service = reader["Service"].ToString();
                    notification.ExpiryDate = reader["ExpiryDate"].ToString();
                    notification.LeftDayToExpire = Convert.ToInt32(reader["LeftDayToExpire"]);
                    notification.SendNotificationAfter = Convert.ToInt32(reader["SendNotificationAfter"]);
                    notification.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    notification.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    notification.MarkAsRead = Convert.ToBoolean(reader["MarkAsRead"]);
                    notification.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    notification.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    if (notification.LeftDayToExpire < 0)
                    {
                        notification.ExpiredNotification = "Expired";
                    }

                    importReminderNotifications.Add(notification);
                }
                con.Close();
                return importReminderNotifications;
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

        public List<ImportReminderNotification> GetAllNotifications()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<ImportReminderNotification> importReminderNotifications = new List<ImportReminderNotification>();
                SqlCommand sqlCommand = new SqlCommand("USP_Admin_GetAllNotifications", con);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    ImportReminderNotification notification = new ImportReminderNotification();

                    notification.ID = Convert.ToInt32(reader["ID"]);
                    notification.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    notification.CompanyName = reader["CompanyName"].ToString();
                    notification.UserId = Convert.ToInt32(reader["UserId"]);
                    notification.Username = reader["Username"].ToString();
                    notification.Service = reader["Service"].ToString();
                    notification.ExpiryDate = reader["ExpiryDate"].ToString();
                    notification.LeftDayToExpire = Convert.ToInt32(reader["LeftDayToExpire"]);
                    notification.SendNotificationAfter = Convert.ToInt32(reader["SendNotificationAfter"]);
                    notification.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    notification.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    notification.MarkAsRead = Convert.ToBoolean(reader["MarkAsRead"]);
                    notification.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    notification.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    if (notification.LeftDayToExpire < 0)
                    {
                        notification.ExpiredNotification = "Expired";
                    }

                    importReminderNotifications.Add(notification);
                }
                con.Close();
                return importReminderNotifications;
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
        /// Update Reminder Days (Everyday it will update automatically at 12:00 AM using Task Schedular)              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="NotificationDays"></param>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateReminderDays(int NotificationDays, int Id, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 1;
                SqlCommand cmd = new SqlCommand("USP_Admin_UpdateNotification", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", 3);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@NotificationDays", NotificationDays);
                cmd.Parameters.AddWithValue("@ID", Id);
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

        /// <summary>
        /// Update Reminder Days (Everyday it will update automatically at 12:00 AM using Task Schedular)              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void UpdateLeftDays()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Admin_UpdateNotification", con);
                cmd.CommandType = CommandType.StoredProcedure;

                List<ImportReminderNotification> importReminderNotifications = new List<ImportReminderNotification>();
                importReminderNotifications = GetAllNotifications();
                if (importReminderNotifications != null)
                {
                    if (importReminderNotifications.Count > 0)
                    {
                        for (int index = 0; index < importReminderNotifications.Count; index++)
                        {
                            int LeftDays = importReminderNotifications[index].LeftDayToExpire;
                            LeftDays = LeftDays - 1;
                            cmd.Parameters.AddWithValue("@Status", 4);
                            cmd.Parameters.AddWithValue("@NotificationDays", LeftDays);
                            cmd.Parameters.AddWithValue("@ID", importReminderNotifications[index].ID);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            cmd.Parameters.Clear();
                        }
                    }
                }
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
        /// Generate Notification (Everyday it will update automatically at 12:00 AM using Task Schedular)              -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void UpdateImportantNotifications()
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Admin_GenerateNotifications", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
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
        /// Remove Notification                                                                                         -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RemoveNotification(int Id, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 1;
                SqlCommand cmd = null;
                cmd = new SqlCommand("USP_Admin_RemoveExpiryNotification", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@userId", UserId);
                con.Open();
                _returnId = Convert.ToInt32(cmd.ExecuteScalar());
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
        /// Get Request Notifications                                                                                   -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetRequestNotificationModel> GetRequestNotifications(string StartDate, string EndDate, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetRequestNotificationModel> listdata = new List<GetRequestNotificationModel>();
                List<GetRequestNotificationModel> testListData = new List<GetRequestNotificationModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetAllRequestNotificaion", con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 10000;

                if (StartDate != null && EndDate != null)
                {
                    command.Parameters.AddWithValue("@StartDate", StartDate);
                    command.Parameters.AddWithValue("@EndDate", EndDate);
                }

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["ServiceCreatedById"]) != UserId && Convert.ToInt32(reader["AssignedById"]) != UserId)
                    {
                        GetRequestNotificationModel model = new GetRequestNotificationModel();
                        model.Id = Convert.ToInt32(reader["Id"]);
                        model.SerialNumber = Convert.ToString(reader["SerialNumber"]);
                        model.RequestCreatedOn = Convert.ToString(reader["RequestCreatedOn"]);
                        model.Type = Convert.ToString(reader["Type"]);
                        model.AssignedUserId = Convert.ToInt32(reader["AssignedUserId"]);
                        model.AssignedById = Convert.ToInt32(reader["AssignedById"]);
                        model.AssignedOn = Convert.ToString(reader["AssignedOn"]);
                        model.FormName = Convert.ToString(reader["FormName"]);
                        model.Title = Convert.ToString(reader["Title"]);
                        model.Description = Convert.ToString(reader["Description"]);
                        model.AssignedUser = Convert.ToString(reader["AssignedUser"]);
                        model.AssignedBy = Convert.ToString(reader["AssignedBy"]);
                        model.ServiceCreatedBy = Convert.ToString(reader["ServiceCreatedBy"]);
                        model.ServiceCreatedBySalesPerson = Convert.ToString(reader["ServiceCreatedBySalesPerson"]);
                        model.CreatedOn = Convert.ToString(reader["CreatedOn"]);
                        model.CreatedOnUtc = DateTime.ParseExact(model.CreatedOn, "dd-MM-yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                        model.ResponseType = Convert.ToString(reader["ResponseType"]);
                        model.MarkAsRead = Convert.ToBoolean(reader["MarkAsRead"]);
                        model.ServiceCreatedByRMTeam = reader["ServiceCreatedByRMTeam"].ToString();
                        testListData.Add(model);
                    }
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["SupportTicketCreatedById"]) != UserId && Convert.ToInt32(reader["AssignedById"]) != UserId)
                    {
                        GetRequestNotificationModel model = new GetRequestNotificationModel();
                        model.Id = Convert.ToInt32(reader["Id"]);
                        model.TrackingNumber = Convert.ToString(reader["TrackingNumber"]);
                        model.RequestCreatedOn = Convert.ToString(reader["RequestCreatedOn"]);
                        model.Type = Convert.ToString(reader["Type"]);
                        model.AssignedUserId = Convert.ToInt32(reader["AssignedUserId"]);
                        model.AssignedById = Convert.ToInt32(reader["AssignedById"]);
                        model.AssignedOn = Convert.ToString(reader["AssignedOn"]);
                        model.Title = Convert.ToString(reader["Title"]);
                        model.Description = Convert.ToString(reader["Description"]);
                        model.AssignedUser = Convert.ToString(reader["AssignedUser"]);
                        model.AssignedBy = Convert.ToString(reader["AssignedBy"]);
                        model.TicketCreatedBy = Convert.ToString(reader["SupportTicketCreatedBy"]);
                        model.SupportTicketCreatedBySalesPerson = Convert.ToString(reader["SupportTicketCreatedBySalesPerson"]);
                        model.CreatedOn = Convert.ToString(reader["CreatedOn"]);
                        model.CreatedOnUtc = DateTime.ParseExact(model.CreatedOn, "dd-MM-yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                        model.ResponseType = Convert.ToString(reader["ResponseType"]);
                        model.MarkAsRead = Convert.ToBoolean(reader["MarkAsRead"]);
                        testListData.Add(model);
                    }
                }

                con.Close();

                var getalldata = from data in testListData
                                 orderby data.CreatedOnUtc descending
                                 select data;

                listdata = getalldata.ToList();

                return listdata;
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
        /// Change Request Notification Status                                                                           -- Yashasvi TBC (26-11-2022)
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="Id"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ChangeRequestNotificationStatus(int Status, int Id, int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ChangeRequestNotificationStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Status);
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
        public async Task<List<GetNotificationServiceListModel>> GetNotificationServiceList()
        {
            SqlConnection connection = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int index = 1;
                List<GetNotificationServiceListModel> services = new List<GetNotificationServiceListModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetNotificationServices", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    GetNotificationServiceListModel service = new GetNotificationServiceListModel();
                    if (reader["Service"] != DBNull.Value)
                        service.Service = reader["Service"].ToString();
                    if (reader["TotalCount"] != DBNull.Value)
                        service.totalCount = Convert.ToInt32(reader["TotalCount"]);
                    service.Index = index;
                    index++;
                    services.Add(service);
                }
                connection.Close();

                return services;
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
