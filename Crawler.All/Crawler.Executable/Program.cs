using Crawler.Core.Code;
using Crawler.DTO.RequestDTO;
using Crawler.DTO.ResponseDTO;
using Crawler.Scheduler.Jobs;
using Sitecrawler.Core.Code;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
    
namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            CrawlInstance instance = new CrawlInstance();
         
            instance.Start();
            instance.AddJob();
        }
    }
}   