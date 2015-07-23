using Sitecrawler.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Crawler.Data.DbContext;
using System.Text.RegularExpressions;
using Crawler.DTO.ResponseDTO;
using Crawler.API.Helper;

namespace Sitecrawler.Core.Code
{
    public class dogdripCrawler : commonCrawler
    {
        private readonly int _perpage = 20;
        protected override int perpage { get { return _perpage; } }
        protected override List<ContentRevisionDTO> GetContentsPerPage(int page)
        {
            var contentrevisionList = new List<ContentRevisionDTO>();
            try
            {
                //개드립
                var dogdrip = webGetutf.Load("http://www.dogdrip.net/index.php?mid=dogdrip&page="+ page);

                var dogdrip_ul = dogdrip.DocumentNode.SelectNodes("//ul[@class= 'lt']").FirstOrDefault();
                var articles = dogdrip_ul.Descendants("li");

                foreach (var dogdrip_li in articles)
                {
                    var anchortag = dogdrip_li.ChildNodes.SingleOrDefault(c => c.Name.Equals("a"));
                    var titlenode = anchortag.SelectSingleNode("./span[@class='title']");
                    //var authnode = anchortag.SelectSingleNode("./span[@class='auth']");

                    //var commentCount = titlenode.SelectNodes("./em").SingleOrDefault();
                    //if (commentCount != null)
                    //{
                    //    //댓글 갯수
                    //    commentCount.Remove();
                    //}

                    var url = new Uri(new Uri("http://www.dogdrip.net"), anchortag.GetAttributeValue("href", "undefined"));

                    //buildobject;
                    contentrevisionList.Add(new ContentRevisionDTO
                    {
                        Crawled = DateTime.Now,
                        isDepricate = false,
                        For_BoardId = 11,
                        Content = new ContentDTO
                        {
                            Article = titlenode.InnerText.Trim(),
                            Contents_URL = url.AbsoluteUri,
                            Url_Params = url.PathAndQuery,
                            Checksum = Convert.ToBase64String(HashHelper.ObjectToMD5Hash(url.PathAndQuery))
                        }
                    });
                }
                return contentrevisionList;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        protected override void ParseContent(ContentRevisionDTO contentrevision)
        {
            try
            {
                var loadedContent = webGetutf.Load(contentrevision.Content.Contents_URL);

                //var content_count = 0;
                List<string> details = new List<string>();
                var articlecontent = loadedContent.DocumentNode.SelectNodes("//div[@id = 'article_1']").FirstOrDefault();
                contentrevision.Details = articlecontent.InnerText.Trim();
                contentrevision.Details_Html = articlecontent.InnerHtml.Trim();
                contentrevision.isDepricate = false;

                var imgnodes = articlecontent.SelectNodes(".//img");

                contentrevision.SrcDatas = new List<SrcdataDTO>();
                if (imgnodes == null) return;

                foreach (var img in imgnodes)
                {
                    var srcurl = new Uri(img.GetAttributeValue("src", "default"));
                    var sourceurl = srcurl.AbsoluteUri;
                    var filnemae = System.IO.Path.GetFileName(srcurl.LocalPath);

                    var srcdata = new SrcdataDTO
                    {
                        SourceUrl = srcurl.AbsoluteUri,
                        IsDepricated = false,
                        FileName = System.IO.Path.GetFileName(srcurl.LocalPath),
                        SrcGuId = Guid.NewGuid(),
                    };
                    img.SetAttributeValue("guid", srcdata.SrcGuId.ToString());

                    contentrevision.SrcDatas.Add(srcdata);
                }
            }
            catch (ArgumentNullException)
            {
                contentrevision.isDepricate = true;
                return ;
            }
            catch (UriFormatException)
            {
                return ;
            }
            catch (Exception e)
            {
                Console.WriteLine("ReStart");
                ParseContent(contentrevision);
            }
        }
    }
}
