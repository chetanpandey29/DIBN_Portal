using DIBN.Areas.Admin.Data;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Models.ScheduledTasks
{
    public class UpdateExpiryDate : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() => Reminder());
            return task;
        }
        public void Reminder()
        {
            //SqlConnection con = new SqlConnection("Data Source=LAPTOP-EIVGA7UC;Initial Catalog=DIBN_DB_Staging;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=Admin@123;Max Pool Size=6000;MultipleActiveResultSets=true");
            //SqlConnection con = new SqlConnection("Data Source=VMI1193020;Initial Catalog=DIBN_Staging_DB;Integrated Security=False;Persist Security Info=False;User ID=developer;Password=Code.9636007458;Max Pool Size=6000;MultipleActiveResultSets=true");
            //SqlConnection con = new SqlConnection("Data Source=VMI1193020;Initial Catalog=DIBN_DB;Integrated Security=False;Persist Security Info=False;User ID=developer;Password=Code.9636007458;Max Pool Size=6000;MultipleActiveResultSets=true");
            SqlConnection con = new SqlConnection("Data Source=89.116.24.251;Initial Catalog=DIBN_Staging_DB;Integrated Security=False;Persist Security Info=False;User ID=developer;Password=Code.9636007458;Max Pool Size=6000;MultipleActiveResultSets=true");
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
            Log.Information("Update Left Days of Documents/Passport/Visa which are going to expire soon company wise.");
        }
        public List<ImportReminderNotification> GetAllNotifications()
        {
            //SqlConnection con = new SqlConnection("Data Source=LAPTOP-EIVGA7UC;Initial Catalog=DIBN_DB_Staging;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=Admin@123;Max Pool Size=6000;MultipleActiveResultSets=true");
            //SqlConnection con = new SqlConnection("Data Source=VMI1193020;Initial Catalog=DIBN_Staging_DB;Integrated Security=False;Persist Security Info=False;User ID=developer;Password=Code.9636007458;Max Pool Size=6000;MultipleActiveResultSets=true");
            //SqlConnection con = new SqlConnection("Data Source=VMI1193020;Initial Catalog=DIBN_DB;Integrated Security=False;Persist Security Info=False;User ID=developer;Password=Code.9636007458;Max Pool Size=6000;MultipleActiveResultSets=true");
            SqlConnection con = new SqlConnection("Data Source=89.116.24.251;Initial Catalog=DIBN_Staging_DB;Integrated Security=False;Persist Security Info=False;User ID=developer;Password=Code.9636007458;Max Pool Size=6000;MultipleActiveResultSets=true");
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
    }
}
