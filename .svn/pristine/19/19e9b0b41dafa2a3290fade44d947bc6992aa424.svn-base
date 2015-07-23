using Crawler.Core.Code;
using Sitecrawler.Core.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.SingleMode
{
    class Program
    {
        static void Main(string[] args)
        {
            slrclubCrawler ilbe = new slrclubCrawler();
            var mgr = new TimePeriodManager();
            mgr.AddTimeperiod();

            ilbe.GetList(20, 4);
            ilbe.ParseArticles();

            Console.WriteLine(DateTime.Now + "Crawler finish the work");
        }
    }
}
