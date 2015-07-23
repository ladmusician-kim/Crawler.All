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
    public class humorCrwaler : commonCrawler
    {
        private readonly int _perpage = 20;
        protected override int perpage { get { return _perpage; } }

        protected override List<ContentRevisionDTO> GetContentsPerPage(int page)
        {
            var contentrevisionList = new List<ContentRevisionDTO>();

            var todayhumor = webGetutf.Load("http://m.todayhumor.co.kr/list.php?table=bestofbest&page=" + page);
            //사이트 이름
            var todayhumor_contentscount = 0;
            if(todayhumor.DocumentNode.SelectNodes("//a[@href]") != null)
            {
                foreach (var todayhumor_a in todayhumor.DocumentNode.SelectNodes("//a[@href]"))
                {
                    var content = new ContentDTO();

                    DateTime time = DateTime.Now;

                    todayhumor_contentscount++;
                    if (todayhumor_contentscount > 6 && todayhumor_contentscount < 27)
                    {
                        var todayhumor_article = todayhumor_a.Descendants().SingleOrDefault(p => p.GetAttributeValue("class", "dd") == "listSubject");

                        
                        //var commentCount = todayhumor_article.SelectNodes("./span[@class = 'list_comment_count']").SingleOrDefault();
                        //if (commentCount != null)
                        //{
                        //    commentCount.Remove();
                        //}

                        var article = todayhumor_article.InnerText.Trim();
                        HtmlAttribute todayhumor_url = todayhumor_a.Attributes["href"];

                        //url
                        var url = ("http://m.todayhumor.co.kr/" + todayhumor_url.Value);
                        var url_params = todayhumor_url.Value;

                        contentrevisionList.Add(new ContentRevisionDTO
                        {
                            Crawled = DateTime.Now,
                            isDepricate = false,
                            For_BoardId = 9,
                            Content = new ContentDTO
                            {
                                Article = article,
                                Contents_URL = url,
                                Url_Params = url_params,
                                Checksum = Convert.ToBase64String(HashHelper.ObjectToMD5Hash(url_params))
                            }
                        });
                    }
                }
                return contentrevisionList;
            }
            else
            {
                Console.Write("9번 홈페이지에 문제가 있습니다. 확인 바랍니다.");
                return null;
            }
        }
        protected override void ParseContent(ContentRevisionDTO contentrevision)
        {
            try
            {
                var loadedContent = webGetutf.Load(contentrevision.Content.Contents_URL);

                //var checkifdeprecated = loadedContent.DocumentNode.SelectNodes("//div(@class='whole_box')").ToList().Where(p => p.InnerText.Trim() == "해당 게시물이 존재하지 않습니다.");
                //if (checkifdeprecated != null)
                //{
                //    contentrevision.isDepricate = true;
                //    return;
                //}

                //var content_count = 0;SSS
                List<string> details = new List<string>();
                var articlecontent = loadedContent.DocumentNode.SelectNodes("//div[@class = 'view_content']").SingleOrDefault();
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
