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
using System.Net;
using System.Security.Cryptography;
using Crawler.API.Helper;

namespace Sitecrawler.Core.Code
{
    public class IlbeCrawler : commonCrawler
    {
        private readonly int _perpage = 15;
        protected override int perpage { get { return _perpage; } }

        protected override List<ContentRevisionDTO> GetContentsPerPage(int page)
        {
            try
            {
                var contentrevisionList = new List<ContentRevisionDTO>();
                 
                //commonCrawler로 부터 상속받은 부분
                //일베
                var ilbe = webGetutf.Load("http://www.ilbe.com/index.php?mid=ilbe&page=" + page);

                var ilbe_ul = ilbe.DocumentNode.SelectNodes("//ul[@class= 'lt']").FirstOrDefault();
                var articles = ilbe_ul.Descendants("li");

                foreach (var ilbe_li in articles)
                {
                    var spanAuth = ilbe_li.SelectNodes(".//span[@class='auth']").SingleOrDefault();
                    var managerNode = spanAuth.SelectNodes(".//img").SingleOrDefault();
                    if(managerNode != null)
                    {
                        var srcurl = new Uri(managerNode.GetAttributeValue("src", "default"));
                        var man_filename = System.IO.Path.GetFileName(srcurl.LocalPath);
                        if (man_filename.Equals("30.gif"))
                        {
                            continue;
                        }
                    }
                    
                    

                    //ilbe_justforIlbe++;
                    var anchortag = ilbe_li.ChildNodes.SingleOrDefault(c => c.Name.Equals("a"));
                    var titlenode = anchortag.SelectSingleNode("./span[@class='title']");
                    //var authnode = anchortag.SelectSingleNode("./span[@class='auth']");


                    
                    //var commentCount = titlenode.SelectNodes("./em").SingleOrDefault();
                    //if (commentCount != null)
                    //{
                    //    commentCount.Remove();
                    //}

                    var url = new Uri(new Uri("http://www.ilbe.com"), anchortag.GetAttributeValue("href", "undefined"));

                    //buildobject;
                    contentrevisionList.Add(new ContentRevisionDTO
                    {
                        Crawled = DateTime.Now,
                        isDepricate = false,
                        For_BoardId =10,
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
            catch(Exception e)
            {
                return null;
            }
        }

        //details_html 쪼개는 함수
        protected override void ParseContent(ContentRevisionDTO contentrevision)
        {
            try
            {
                var loadedContent = webGetutf.Load(contentrevision.Content.Contents_URL);

                var checkifdeprecated = loadedContent.DocumentNode.SelectNodes("//li[@class = 'hx_cate']");
                if (checkifdeprecated != null && checkifdeprecated.FirstOrDefault().InnerText.Trim() == "삭제된 글입니다.")
                {
                    contentrevision.isDepricate = true;
                    return;
                }
                
                var articlecontent = loadedContent.DocumentNode.SelectNodes("//div[@id = 'copy_layer_1']").LastOrDefault();

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
            catch(Exception e)
            {
                Console.WriteLine("ReStart");
                ParseContent(contentrevision);
            }
        }
    }
}
