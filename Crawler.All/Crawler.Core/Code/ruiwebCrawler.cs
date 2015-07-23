﻿using Crawler.Data.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Crawler.DTO.ResponseDTO;
using Crawler.API.Helper;

namespace Sitecrawler.Core.Code
{
    public class ruiwebCrawler : commonCrawler
    {
        private readonly int _perpage = 30;
        protected override int perpage { get { return _perpage; } }

        protected override List<ContentRevisionDTO> GetContentsPerPage(int page)
        {
            var contentrevisionList = new List<ContentRevisionDTO>();
            try
            {
                //commonCrawler로 부터 상속받은 부분
                //루리웹
                var ruiweb = webGetutf.Load("http://bbs2.ruliweb.daum.net/gaia/do/ruliweb/default/community/322/list?bbsId=G005&pageIndex=" + page + "&itemId=141");
                var ruiweb_articlescount = 0;

                foreach (var ruiweb_tr in ruiweb.DocumentNode.SelectNodes("//tr"))
                {
                    if(ruiweb_tr.Name == "emph ")
                    {
                        ruiweb_tr.Remove();
                        continue;
                    }

                    var ruiweb_subject = ruiweb_tr.Descendants().Where(p => p.GetAttributeValue("class", "dd") == "subject").FirstOrDefault();
                    var anchortag = ruiweb_subject.ChildNodes.SingleOrDefault(c => c.Name.Equals("a"));
                    var article = (anchortag.InnerText.Trim());
                        
                    var url = new Uri(new Uri("http://bbs2.ruliweb.daum.net/gaia/do/ruliweb/default/community/322/"), anchortag.GetAttributeValue("href", "undefined"));

                    contentrevisionList.Add(new ContentRevisionDTO
                    {
                        Crawled = DateTime.Now,
                        For_BoardId = 1,
                        isDepricate = false,
                        Content = new ContentDTO
                        {
                            Article = article,
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

                var articlecontent = loadedContent.DocumentNode.SelectNodes("//td[@class = 'tx-content-container read_cont_td']").LastOrDefault();
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