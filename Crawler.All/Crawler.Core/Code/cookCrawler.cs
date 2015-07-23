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
using System.Web;
using Crawler.API.Helper;


namespace Sitecrawler.Core.Code
{
    public class cookCrawler : commonCrawler
    {
        private readonly int _perpage = 28;
        protected override int perpage { get { return _perpage; } }
        protected override List<ContentRevisionDTO> GetContentsPerPage(int page)
        {
            try
            {
                var contentrevisionList = new List<ContentRevisionDTO>();

                var cook = webGetutf.Load("http://www.82cook.com/entiz/enti.php?bn=15&cn=&searchType=&search1=&keys=%2A&page=" + page);
                var cook_licount = 0;

                foreach (var cook_div in cook.DocumentNode.SelectNodes("//td[@class= 'title']"))
                {
                    var content = new ContentDTO();

                    cook_licount++;
                    if (cook_licount > 3)
                    {
                        var anchortag = cook_div.ChildNodes.SingleOrDefault(c => c.Name.Equals("a"));
                        var url = new Uri(new Uri("http://www.82cook.com/entiz/"), HttpUtility.HtmlDecode(anchortag.GetAttributeValue("href", "undefined")));
                        var Article = cook_div.InnerText.Trim();
                        var url_parmas = url.Query;

                        contentrevisionList.Add(new ContentRevisionDTO
                        {
                            Crawled = DateTime.Now,
                            isDepricate = false,
                            For_BoardId = 3,
                            Content = new ContentDTO
                            {
                                Article = Article,
                                Contents_URL = url.AbsoluteUri,
                                Url_Params = url.PathAndQuery,
                                Checksum = Convert.ToBase64String(HashHelper.ObjectToMD5Hash(url.PathAndQuery))
                            }
                        });
                    }
                }
                return contentrevisionList;
            }
            catch(Exception e)
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
                var checkifdeprecated = loadedContent.DocumentNode.InnerText.Trim();
                if (checkifdeprecated != null && checkifdeprecated == "게시물이 없습니다")
                {
                    contentrevision.isDepricate = true;
                    return;
                }

                var articlecontent = loadedContent.DocumentNode.SelectNodes("//div[@id = 'articleBody']").FirstOrDefault();
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
            catch (Exception e)
            {
                Console.WriteLine("ReStart");
                ParseContent(contentrevision);
            }
        }
    }
}
