using Crawler.Core.Code;
using Quartz;
using Quartz.Impl;
using ServiceStack.ServiceClient.Web;
using Sitecrawler.Core.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Scheduler.Jobs
{
    public static class Variables
    {
        public static int count = 20;
    }
   
    public static class TimeExtension
    {
        public static DateTime Round10(this DateTime value)
        {
            var ticksIn10Mins = TimeSpan.FromMinutes(5).Ticks;

            return (value.Ticks % ticksIn10Mins == 0) ? value : new DateTime((value.Ticks / ticksIn10Mins + 1) * ticksIn10Mins);
        }
    }

    public class CrawlInstance
    {
        public CrawlInstance()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            intervalMins = 5;
        }

        IJobDetail Ilbejob;
        IJobDetail Ruiwebjob;
        IJobDetail Cookjob;
        IJobDetail Dogdripjob;
        IJobDetail Humorjob;
        IJobDetail Bobaejob;
        IJobDetail Issueinjob;
        IJobDetail Mparkjob;
        IJobDetail Natejob;
        IJobDetail pomppujob;
        IJobDetail slrjob;
        IJobDetail soccerLinejob;

        IJobDetail addtimeperiodjob;

        IScheduler scheduler;
        int intervalMins;

        public void AddJob()
        {

            Ruiwebjob = JobBuilder.Create<CrawlRuiweb>()
                            .WithIdentity("Ruiwebjob", "crawlgroup")
                            .Build();
            Bobaejob = JobBuilder.Create<CrawlBobae>()
                            .WithIdentity("Bobaejob", "crawlgroup")
                            .Build();
            Cookjob = JobBuilder.Create<CrawlCook>()
                           .WithIdentity("Cookjob", "crawlgroup")
                           .Build();

            slrjob = JobBuilder.Create<Crawlslr>()
                            .WithIdentity("slrjob", "crawlgroup")
                            .Build();
            
            Natejob = JobBuilder.Create<Crawlnate>()
                            .WithIdentity("Natejob", "crawlgroup")
                            .Build();

            soccerLinejob = JobBuilder.Create<CrawlsoccerLine>()
                            .WithIdentity("soccerLinejob", "crawlgroup")
                            .Build();
            
            Mparkjob = JobBuilder.Create<CrawlMpark>()
                           .WithIdentity("Mparkjob", "crawlgroup")
                           .Build();

            Issueinjob = JobBuilder.Create<CrawlIsuue>()
                            .WithIdentity("Issueinjob", "crawlgroup")
                           .Build();
            
            Humorjob = JobBuilder.Create<CrawlHumor>()
                           .WithIdentity("Humorjob", "crawlgroup")
                           .Build();
            
            Ilbejob = JobBuilder.Create<CrawlIlbe>()
                        .WithIdentity("Ilbejob", "crawlgroup")
                        .Build();
            
            Dogdripjob = JobBuilder.Create<CrawlDogdrip>()
                            .WithIdentity("Dogdripjob", "crawlgroup")
                            .Build();

            pomppujob = JobBuilder.Create<Crawlpomppu>()
                            .WithIdentity("pomppujob", "crawlgroup")
                            .Build();


            addtimeperiodjob = JobBuilder.Create<AddTimeperiodJob>()
                                .WithIdentity("timeperiodjob", "timeperiodgroup")
                                .Build();

            ITrigger timeperiodtrigger = TriggerBuilder.Create()
                .WithIdentity("timeperiodtrigger", "timeperiodtriggergroup")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(intervalMins).RepeatForever()).Build();

            var firetime = DateTime.Now.Round10();


            ITrigger ruiwebtrigger = TriggerBuilder.Create()
                   .WithIdentity("ruiwebtrigger", "crawltriggergroup")
                   .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                       .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

             ITrigger bobaetrigger = TriggerBuilder.Create()
                   .WithIdentity("bobaetrigger", "crawltriggergroup")
                    .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                       .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();
            ITrigger cooktrigger = TriggerBuilder.Create()
                   .WithIdentity("cooktrigger", "crawltriggergroup")
               .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                       .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

            ITrigger slrtrigger = TriggerBuilder.Create()
                   .WithIdentity("slrtrigger", "crawltriggergroup")
                   .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                       .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

            ITrigger natetrigger = TriggerBuilder.Create()
                   .WithIdentity("natetrigger", "crawltriggergroup")
                 .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                       .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

            ITrigger soccerlinetrigger = TriggerBuilder.Create()
                   .WithIdentity("soccerlinetrigger", "crawltriggergroup")
                    .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                       .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

            ITrigger mparktrigger = TriggerBuilder.Create()
                   .WithIdentity("mparktrigger", "crawltriggergroup")
                   .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                          .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

             ITrigger issueintrigger = TriggerBuilder.Create()
                   .WithIdentity("issueintrigger", "crawltriggergroup")
                  .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

            ITrigger humortrigger = TriggerBuilder.Create()
                   .WithIdentity("humortrigger", "crawltriggergroup")
                  .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                         .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

            ITrigger ilbetrigger = TriggerBuilder.Create()
                   .WithIdentity("ilbetrigger", "crawltriggergroup")
                    .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                         .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                    .Build();

            ITrigger dogdriptrigger = TriggerBuilder.Create()
                   .WithIdentity("dogdriptrigger", "crawltriggergroup")
                   .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                          .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();

             ITrigger pompputrigger = TriggerBuilder.Create()
                   .WithIdentity("pompputrigger", "crawltriggergroup")
                    .StartAt(firetime)
                   .WithSimpleSchedule(x => x
                         .WithIntervalInMinutes(intervalMins)
                       .RepeatForever())
                   .Build();


             scheduler.ScheduleJob(addtimeperiodjob, timeperiodtrigger);

             scheduler.ScheduleJob(Ruiwebjob, ruiwebtrigger);

            scheduler.ScheduleJob(Bobaejob, bobaetrigger);

            scheduler.ScheduleJob(Cookjob, cooktrigger);
            scheduler.ScheduleJob(slrjob, slrtrigger);
            scheduler.ScheduleJob(Natejob, natetrigger);
            scheduler.ScheduleJob(soccerLinejob, soccerlinetrigger);
            scheduler.ScheduleJob(Mparkjob, mparktrigger);
            scheduler.ScheduleJob(Issueinjob, issueintrigger);
            scheduler.ScheduleJob(Humorjob, humortrigger);
            scheduler.ScheduleJob(Ilbejob, ilbetrigger);
            scheduler.ScheduleJob(Dogdripjob, dogdriptrigger);
            scheduler.ScheduleJob(pomppujob, pompputrigger);


        }

        public void Start()
        {
            scheduler.Start();
        }

        public void End()
        {
            scheduler.Shutdown();
        }
    }
    public class AddTimeperiodJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var mgr = new TimePeriodManager();
            System.Console.WriteLine("Set New TimePeriod");
            mgr.AddTimeperiod();
        }
    }
    public class CrawlRuiweb : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ruiwebCrawler ruiweb = new ruiwebCrawler();
            System.Console.WriteLine("Ruiweb Crawler Start");
            ruiweb.GetList(Variables.count, 1);
            ruiweb.ParseArticles();
        }
    }
    public class CrawlBobae : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            bobaedreamCrawler bobae = new bobaedreamCrawler();
            System.Console.WriteLine("Bobae Crawler Start");
            bobae.GetList(Variables.count, 2);
            bobae.ParseArticles();
        }
    }

    public class CrawlCook : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            cookCrawler cook = new cookCrawler();
            System.Console.WriteLine("Cook Crawler Start");
            cook.GetList(Variables.count, 3);
            cook.ParseArticles();
        }
    }
    public class Crawlslr : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            slrclubCrawler slr = new slrclubCrawler();
            System.Console.WriteLine("Slr Crawler Start");
            slr.GetList(Variables.count, 4);
            slr.ParseArticles();
        }
    }
    public class Crawlnate : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            nateCrawler nate = new nateCrawler();
            System.Console.WriteLine("Nate Crawler Start");
            nate.GetList(Variables.count, 5);
            nate.ParseArticles();
        }
    }
    public class CrawlsoccerLine : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            soccerlineCrawler soccerLine = new soccerlineCrawler();
            System.Console.WriteLine("SoccerLine Crawler Start");
            soccerLine.GetList(Variables.count, 6);
            soccerLine.ParseArticles();
        }
    }
    public class CrawlMpark : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            mparkCrawler mpark = new mparkCrawler();
            System.Console.WriteLine("Mpark Crawler Start");
            mpark.GetList(Variables.count, 7);
            mpark.ParseArticles();
        }
    }
    public class CrawlIsuue : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            issueinCrawler issuein = new issueinCrawler();
            System.Console.WriteLine("Issuein Crawler Start");
            issuein.GetList(Variables.count, 8);
            issuein.ParseArticles();
        }
    }
    public class CrawlHumor : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            humorCrwaler humor = new humorCrwaler();
            System.Console.WriteLine("Humor Crawler Start");
            humor.GetList(Variables.count, 9);
            humor.ParseArticles();
        }
    }
    public class CrawlIlbe : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            IlbeCrawler ilbe = new IlbeCrawler();
            System.Console.WriteLine("Ilbe Crawler Start");
            ilbe.GetList(Variables.count, 10);
            ilbe.ParseArticles();
        }
    }
    public class CrawlDogdrip : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            dogdripCrawler dogdrip = new dogdripCrawler();
            System.Console.WriteLine("DogDrip Crawler Start");
            dogdrip.GetList(Variables.count, 11);
            dogdrip.ParseArticles();
        }
    }
    public class Crawlpomppu : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            pomppuCrawler pomppu = new pomppuCrawler();
            System.Console.WriteLine("Pomppu Crawler Start");
            pomppu.GetList(Variables.count, 12);
            pomppu.ParseArticles();
        }
    }
}
