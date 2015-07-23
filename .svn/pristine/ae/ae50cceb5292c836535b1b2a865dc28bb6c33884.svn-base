using Sitecrawler.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Crawler.Data.DbContext;
using Crawler.DTO.ResponseDTO;
using Crawler.API.Helper;
using System.Net;

namespace Sitecrawler.Core.Code
{
    public class nateCrawler : commonCrawler
    {
        private readonly int _perpage = 10;
        protected override int perpage { get { return _perpage; } }
        protected override List<ContentRevisionDTO> GetContentsPerPage(int page)
        {
            try
            {
                var contentrevisionList = new List<ContentRevisionDTO>();

                var nate = webGetutf.Load("http://m.pann.nate.com/talk/talker?order=RAN&page=" + page);

                var nate_ul = nate.DocumentNode.SelectNodes("//ul[@class= 'list']").FirstOrDefault();
                var articles = nate_ul.Descendants("li");
                foreach (var nate_li in articles)
                {
                    var anchortag = nate_li.ChildNodes.SingleOrDefault(c => c.Name.Equals("a"));
                    var titlenode = anchortag.SelectSingleNode(".//span[@class='tit']");

                    //var commentCount = titlenode.Descendants().SingleOrDefault(p => p.GetAttributeValue("class", "dd") == "count");
                    //if (commentCount != null)
                    //{
                    //    commentCount.Remove();
                    //}

                    var url = new Uri(new Uri("http://m.pann.nate.com"), anchortag.GetAttributeValue("href", "undefined"));

                    contentrevisionList.Add(new ContentRevisionDTO
                    {
                        Crawled = DateTime.Now,
                        isDepricate = false,
                        For_BoardId = 5,
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

                var articlecontent = loadedContent.DocumentNode.SelectNodes("//div[@id = 'pann-content']").FirstOrDefault();
                contentrevision.Details = articlecontent.InnerText.Trim();
                contentrevision.Details_Html = articlecontent.InnerHtml.Trim();
                contentrevision.isDepricate = false;

                var imgnodes = articlecontent.SelectNodes(".//img");

                contentrevision.SrcDatas = new List<SrcdataDTO>();
                if (imgnodes == null) return;

                foreach (var img in imgnodes)
                {
                    var srcurl = new Uri(img.GetAttributeValue("src", "default"));
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
                return;
            }
            catch (UriFormatException)
            {
                return;
            }
            catch (WebException wex)
            {
                if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    // error 404, do what you need to do
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ReStart");
                ParseContent(contentrevision);
            }
        }
    }
}
