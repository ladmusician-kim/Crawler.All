using Crawler.Core.Jobs;
using Crawler.Data.DbContext;
using Crawler.DTO.RequestDTO;
using Crawler.DTO.ResponseDTO;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using ServiceStack.Plugins.ProtoBuf;
using ServiceStack.ServiceClient.Web;
using Sitecrawler.Core.Code;
using Sitecrawler.Core.Interface;
using System;
using System.Collections.Generic;

namespace Crawler.Core.Code
{
    public class TimePeriodManager
    {
        ProtoBufServiceClient client;
        protected SnapshotDTO Snapshot;
        
        IScheduler scheduler;
        TimePeriod currentPeriod;
        public TimePeriodManager()
        {
            try
            {

                
                CreateNewTimePeriod();

                var begintime = DateTime.Now;

                var crawler1 = new ruiwebCrawler();
                var crawler2 = new bobaedreamCrawler();
                var crawler3 = new cookCrawler();
                var crawler4 = new slrclubCrawler();
                var crawler5 = new nateCrawler();
                var crawler6 = new soccerlineCrawler();
                var crawler7 = new mparkCrawler();
                var crawler8 = new issueinCrawler();
                var crawler9 = new humorCrwaler();
                var crawler10 = new IlbeCrawler();
                var crawler11 = new dogdripCrawler();
                var crawler12 = new pomppuCrawler();

                crawler1.GetList(50);
                crawler1.ParseArticles();

                crawler2.GetList(50);
                crawler2.ParseArticles();

                crawler3.GetList(50);
                crawler3.ParseArticles();

                crawler4.GetList(50);
                crawler4.ParseArticles();

                crawler5.GetList(50);
                crawler5.ParseArticles();

                crawler6.GetList(50);
                crawler6.ParseArticles();

                crawler7.GetList(50);
                crawler7.ParseArticles();

                crawler8.GetList(50);
                crawler8.ParseArticles();

                crawler9.GetList(50);
                crawler9.ParseArticles();

                crawler10.GetList(50);
                crawler10.ParseArticles();

                crawler11.GetList(50);
                crawler11.ParseArticles();

                crawler12.GetList(50);
                crawler12.ParseArticles();

                var endtime = DateTime.Now.Subtract(begintime).TotalMilliseconds;

                System.Console.WriteLine(endtime + "ms");
                System.Console.ReadLine();
            }
            catch (Exception e)
            {

            }
        }
        public void InitTimer()
        {
            scheduler.Start();
        }

        public void AddCrawlers()
        {
            List<IJob> joblist = new List<IJob>();


            RunJob(joblist);
        }
        public void RunJob(List<IJob> joblist)
        {
            var starttime = DateTime.Now.Round10();

            foreach (var eachJob in joblist)
            {
                var type = eachJob.GetType();
                var jobdetail = JobBuilder.Create(type)
                                             .WithIdentity(type.Name, "DefaultJobGroup")
                                             .Build();

                var triggername = type.Name;
                var trigger = TriggerBuilder.Create()
                      .WithIdentity(type.Name + "Trigger", type.Name + "TriggerGroup")
                      .StartAt(starttime)
                      .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())
                      .Build();

                scheduler.ScheduleJob(jobdetail, trigger);
                Console.WriteLine("Added Crawler" + type.Name);
            }
        }

        public void AddJob(TimePeriodDTO timeperiod, IJob job)
        {
            var type = job.GetType();

            var jobdetail = scheduler.GetJobDetail(JobKey.Create(type.Name))
                            ?? JobBuilder.Create(type)
                                         .WithIdentity(type.Name, "DefaultJobGroup")
                                         .Build();

            var triggername = timeperiod.Label + type.Name;

            var trigger = TriggerBuilder.Create()
                  .WithIdentity(timeperiod.Label + type.Name, type.Name + "TriggerGroup")
                  .StartAt(timeperiod.Scheduled)
                  .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())
                  .Build();
        }
        public TimePeriodDTO CreateNewTimePeriod()
        {
            try
            {
                client = new ProtoBufServiceClient("http://localhost:61910");
                var dto = client.Send<EnvelopeDTO<TimePeriodDTO>>(new TimePeriodCreateRequestDTO
                {
                    Crawled = DateTime.Now,
                    Scheduled = DateTime.Now
                });
                return dto.SafeBody;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
    public static class TimeExtension
    {
        public static DateTime Round10(this DateTime value)
        {
            var ticksIn15Mins = TimeSpan.FromMinutes(15).Ticks;

            return (value.Ticks % ticksIn15Mins == 0) ? value : new DateTime((value.Ticks / ticksIn15Mins + 1) * ticksIn15Mins);
        }
    }
}
