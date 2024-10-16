using DIBN.IService;
using DIBN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN.Service
{
    public class ImportantReminderService : IImportantReminderService
    {
        private readonly Connection _dataSetting;
        public ImportantReminderService(Connection dataSetting)
        {
            _dataSetting = dataSetting;
        }
        public List<ImportantReminderViewModel> GetImportantReminderMessagesList(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                ImportantReminderViewModel model = new ImportantReminderViewModel();
                List<ImportantReminderViewModel> importantReminders = new List<ImportantReminderViewModel>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetNotificationsForCompanyWithMarkRead", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ImportantReminderViewModel importantReminder = new ImportantReminderViewModel();
                    importantReminder.ID = Convert.ToInt32(reader["ID"]);
                    importantReminder.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    importantReminder.CompanyName = reader["CompanyName"].ToString();
                    importantReminder.UserId = Convert.ToInt32(reader["UserId"]);
                    importantReminder.Username = reader["Username"].ToString();
                    importantReminder.Service = reader["Service"].ToString();
                    importantReminder.ExpiryDate = reader["ExpiryDate"].ToString();
                    importantReminder.LeftDayToExpire = Convert.ToInt32(reader["LeftDayToExpire"]);
                    importantReminder.SendNotificationAfter = Convert.ToInt32(reader["SendNotificationAfter"]);
                    importantReminder.MarkAsRead = Convert.ToBoolean(reader["MarkAsRead"]);
                    importantReminder.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    importantReminder.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    importantReminder.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    importantReminder.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    if (importantReminder.LeftDayToExpire < 0)
                    {
                        importantReminder.ExpiredNotification = "Expired";
                    }
                    importantReminders.Add(importantReminder);
                }

                con.Close();
                
                return importantReminders;
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
        public ImportantReminderViewModel GetImportantReminderMessages(int CompanyId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int index = 0;
                ImportantReminderViewModel model = new ImportantReminderViewModel();
                List<ImportantReminderViewModel> importantReminders = new List<ImportantReminderViewModel>();
                SqlCommand cmd = new SqlCommand("USP_Admin_GetReadedNotifications", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ImportantReminderViewModel importantReminder = new ImportantReminderViewModel();
                    importantReminder.ID = Convert.ToInt32(reader["ID"]);
                    importantReminder.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    if(Convert.ToInt32(reader["LeftDayToExpire"])<5 && Convert.ToBoolean(reader["MarkAsRead"]) == true)
                    {
                        UpdateStatusOfNotification(importantReminder.CompanyId, importantReminder.ID);
                    }
                }
                con.Close();
                cmd = new SqlCommand("USP_Admin_GetNotificationsForCompany", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ImportantReminderViewModel importantReminder = new ImportantReminderViewModel();
                    importantReminder.ID = Convert.ToInt32(reader["ID"]);
                    importantReminder.CompanyId = Convert.ToInt32(reader["CompanyId"]);
                    importantReminder.CompanyName = reader["CompanyName"].ToString();
                    importantReminder.UserId = Convert.ToInt32(reader["UserId"]);
                    importantReminder.Username = reader["Username"].ToString();
                    importantReminder.Service = reader["Service"].ToString();
                    importantReminder.ExpiryDate = reader["ExpiryDate"].ToString();
                    importantReminder.LeftDayToExpire = Convert.ToInt32(reader["LeftDayToExpire"]);
                    importantReminder.SendNotificationAfter = Convert.ToInt32(reader["SendNotificationAfter"]);
                    importantReminder.MarkAsRead = Convert.ToBoolean(reader["MarkAsRead"]);
                    importantReminder.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    importantReminder.IsDelete = Convert.ToBoolean(reader["IsDelete"]);
                    importantReminder.CreatedOnUtc = reader["CreatedOnUtc"].ToString();
                    importantReminder.ModifyOnUtc = reader["ModifyOnUtc"].ToString();
                    if (importantReminder.LeftDayToExpire < 0)
                    {
                        importantReminder.ExpiredNotification = "Expired";
                    }
                    importantReminders.Add(importantReminder);
                }
                con.Close();
                if (importantReminders != null)
                {
                    if (importantReminders.Count > 0)
                    {
                        model.NotificationCount += importantReminders.Count;
                        for (int i = 0; i < importantReminders.Count; i++)
                        {

                            index = index + 1;
                            if (importantReminders[i].Username == null || importantReminders[i].Username == "")
                            {
                                string message = null;
                                if (importantReminders[i].LeftDayToExpire < 0)
                                {
                                    message = index + ". <strong>" + importantReminders[i].Service + "</strong> of <strong>" + importantReminders[i].CompanyName + "</strong> Was Expired on " +
                                              importantReminders[i].ExpiryDate +".<br/><br/>";
                                }
                                else
                                {
                                    message = index + ". <strong>" + importantReminders[i].Service + "</strong> of <strong>" + importantReminders[i].CompanyName + "</strong> is going to Expire on " +
                                              importantReminders[i].ExpiryDate + ". Only " + importantReminders[i].LeftDayToExpire
                                              + " Days Left.<br/><br/>";
                                }
                               
                                model.Message += message;
                            }
                            else
                            {
                                string message = null;
                                if (importantReminders[i].LeftDayToExpire < 0)
                                {
                                    message = index + ". <strong>" + importantReminders[i].Service + "</strong> of <strong>" + importantReminders[i].Username + "</strong> Was Expired on " +
                                         importantReminders[i].ExpiryDate + ".<br/><br/>";
                                }
                                else { 
                                    message = index + ". <strong>" + importantReminders[i].Service + "</strong> of <strong>" + importantReminders[i].Username + "</strong> is going to Expire on " +
                                              importantReminders[i].ExpiryDate + ". Only " + importantReminders[i].LeftDayToExpire
                                              + " Days Left.<br/><br/>";
                                }
                                model.Message += message;
                            }

                        }
                    }
                }

                return model;
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
        public int UpdateStatusOfNotification(int CompanyId,int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 1;
                SqlCommand cmd = new SqlCommand("USP_Admin_UpdateNotification", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", 1);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                    cmd.ExecuteNonQuery();
                con.Close();
                return _returnId;
            }
            catch(Exception ex)
            {
                throw new Exception (ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public int MarkAsReadNotification(int CompanyId, int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 1;
                SqlCommand cmd = new SqlCommand("USP_Admin_UpdateNotification", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", 2);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
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
        public int MarkAsReadServiceNotification(int UserId,string SerialNumber)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 1;
                SqlCommand cmd = new SqlCommand("USP_Admin_MarkAsReadServiceNotification", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@SerialNumber", SerialNumber);
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

        public int GetAssignedServicesCount(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _countOfService = 0;
                SqlCommand command = new SqlCommand("USP_Admin_GetAssignedOpenServiceRequestCount", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["CompanyServiceRequests"] != DBNull.Value)
                        _countOfService = Convert.ToInt32(reader["CompanyServiceRequests"]);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader["EmployeeServiceRequest"] != DBNull.Value)
                        _countOfService += Convert.ToInt32(reader["EmployeeServiceRequest"]);
                }
                con.Close();
                return _countOfService;
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
        public int GetAssignedSupportTicketCount(int UserId)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand cmd = new SqlCommand("USP_Admin_GetCountOfAssignedSupportTicket", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    _returnId++;
                }
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

        public List<GetRequestNotificationModel> GetRequestNotifications(int UserId,string StartDate, string EndDate)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                List<GetRequestNotificationModel> listdata = new List<GetRequestNotificationModel>();
                List<GetRequestNotificationModel> testListData = new List<GetRequestNotificationModel>();

                SqlCommand command = new SqlCommand("USP_Admin_GetRequestNotificationByAssignId", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", UserId);
                if (StartDate != null && EndDate != null)
                {
                    command.Parameters.AddWithValue("@StartDate", StartDate);
                    command.Parameters.AddWithValue("@EndDate", EndDate);
                }

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if(Convert.ToInt32(reader["SupportTicketCreatedById"]) != UserId)
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
                        model.ResponseType = Convert.ToString(reader["ResponseType"]);
                        model.TicketCreatedBy = Convert.ToString(reader["SupportTicketCreatedBy"]);
                        model.SupportTicketCreatedBySalesPerson = Convert.ToString(reader["SupportTicketCreatedBySalesPerson"]);
                        model.CreatedOn = Convert.ToString(reader["CreatedOn"]);
                        model.CreatedOnUtc = DateTime.ParseExact(model.CreatedOn, "dd-MM-yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                        model.ResponseType = Convert.ToString(reader["ResponseType"]);
                        model.MarkAsRead = Convert.ToBoolean(reader["AssignedUserMarkAsRead"]);
                        testListData.Add(model);
                    }
                    
                }

                reader.NextResult();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["ServiceCreatedById"]) != UserId)
                    {
                        GetRequestNotificationModel model = new GetRequestNotificationModel();
                        model.Id = Convert.ToInt32(reader["Id"]);
                        model.SerialNumber = Convert.ToString(reader["SerialNumber"]);
                        model.RequestCreatedOn = Convert.ToString(reader["RequestCreatedOn"]);
                        model.Type = Convert.ToString(reader["Type"]);
                        model.AssignedUserId = Convert.ToInt32(reader["AssignedUserId"]);
                        model.AssignedById = Convert.ToInt32(reader["AssignedById"]);
                        model.AssignedOn = Convert.ToString(reader["AssignedOn"]);
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
                        model.MarkAsRead = Convert.ToBoolean(reader["AssignedUserMarkAsRead"]);
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
        public int ChangeRequestNotificationStatus(int Status, int Id)
        {
            SqlConnection con = new SqlConnection(_dataSetting.DefaultConnection);
            try
            {
                int _returnId = 0;
                SqlCommand command = new SqlCommand("USP_Admin_ChangeRequestNotificationStatus", con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@Id", Id);

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
