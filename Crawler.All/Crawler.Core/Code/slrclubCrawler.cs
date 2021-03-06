﻿using Sitecrawler.Core.Interface;
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
using System.Web;

namespace Sitecrawler.Core.Code
{
    public  class slrclubCrawler : commonCrawler
    {
        private readonly int _perpage = 15;
        protected override int perpage { get { return _perpage; } }

        protected override List<ContentRevisionDTO> GetContentsPerPage(int page)
        {
            try
            {
                var contentrevisionList = new List<ContentRevisionDTO>();
                var slr = webGetkr.Load("http://m.slrclub.com/l/free/" + page + "/?category=&sn=off&sid=off&ss=off&sc=off&st=off&keyword=&sn1=&sid1=&divpage=5554");

                var slr_ul = slr.DocumentNode.SelectNodes("//ul[@class= 'list']").LastOrDefault();
                var articles = slr_ul.Descendants("li");

                foreach (var slr_li in articles)
                {
                    //공지사항은 제외
                    if (slr_li.Attributes.Where(p=>p.Name.Equals("class")).Count() > 0) continue;

                    var titlenode = slr_li.SelectSingleNode(".//div[@class='subject']");

                    //var commentCount = titlenode.SelectNodes("./span[@class = 'cmt']").SingleOrDefault();
                    //if (commentCount != null)
                    //{
                    //    commentCount.Remove();
                    //}

                    var anchortag = titlenode.ChildNodes.SingleOrDefault(c => c.Name.Equals("a"));
                    var url = new Uri(new Uri("http://m.slrclub.com"), HttpUtility.HtmlDecode(anchortag.GetAttributeValue("href", "undefined")));

                    contentrevisionList.Add(new ContentRevisionDTO
                    {
                        Crawled = DateTime.Now,
                        For_BoardId = 4,
                        isDepricate = false,
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
                var loadedContent = webGetkr.Load(contentrevision.Content.Contents_URL);

                var checkifdeprecated = loadedContent.DocumentNode.SelectNodes("//div[@id= 'slrct']");
                if (checkifdeprecated != null)
                {
                    contentrevision.isDepricate = true;
                    return;
                }
                var infocontent = loadedContent.DocumentNode.SelectNodes("//div[@class = 'info-wrap']").SingleOrDefault();
                var articlecontent = loadedContent.DocumentNode.SelectNodes("//div[@class = 'contents']").SingleOrDefault();
                contentrevision.Details = articlecontent.InnerText.Trim();
                contentrevision.Details_Html = articlecontent.InnerHtml.Trim();
                contentrevision.isDepricate = false;

                var imgnodes = articlecontent.SelectNodes("./img");

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
