using Quartz;
using Serilog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DIBN.Areas.Admin.Models.ScheduledTasks
{
    public class GenerateNotifications : IJob
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
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Admin_GenerateNotifications", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                Log.Information("Task Generate Notification regarding Documents/Passport/Visa which are going to expire soon company wise.");
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
