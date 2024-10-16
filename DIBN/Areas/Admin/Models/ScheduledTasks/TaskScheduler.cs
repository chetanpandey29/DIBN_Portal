using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using Serilog;
using System;

namespace DIBN.Areas.Admin.Models.ScheduledTasks
{
    public class TaskSchedulerModel
    {
        private static readonly string ScheduleCronExpression = "0 0 0 * * ?"; // Every Day at 12 AM (Working)
        //"0 0 12 1/1 * ? *"; Every Day at 12 AM

        public static async System.Threading.Tasks.Task StartAsync()
        {
            try
            {
                LogProvider.IsDisabled = true;
                Log.Information("Task Scheduler Started.");
                var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                if (!scheduler.IsStarted)
                {
                    await scheduler.Start();
                }
                var job1 = JobBuilder.Create<GenerateNotifications>().WithIdentity("ExecuteTaskServiceCallJob1", "group1").Build();
                var trigger1 = TriggerBuilder.Create().WithIdentity("ExecuteTaskServiceCallTrigger1", "group1").WithCronSchedule(ScheduleCronExpression).Build();
                await scheduler.ScheduleJob(job1, trigger1);
                

                var job2 = JobBuilder.Create<UpdateExpiryDate>().WithIdentity("ExecuteTaskServiceCallJob2", "group2").Build();
                var trigger2 = TriggerBuilder.Create().WithIdentity("ExecuteTaskServiceCallTrigger2", "group2").WithCronSchedule(ScheduleCronExpression).Build();
                await scheduler.ScheduleJob(job2, trigger2);
            }
            catch (Exception ex) { 
                Log.Error("Task Scheduler throw exception :"+ex.Message);
            }
        }
    }
}
