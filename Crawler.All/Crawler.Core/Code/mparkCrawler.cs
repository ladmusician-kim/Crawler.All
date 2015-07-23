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
using System.Net;
using System.Security.Cryptography;
using ServiceStack.Messaging.Rcon;
using Crawler.API.Helper;

namespace Sitecrawler.Core.Code
{
    public class mparkCrawler : commonCrawler
    {
        private readonly int _perpage = 20;
        protected override int perpage { get { return _perpage; } }

        protected override List<ContentRevisionDTO> GetContentsPerPage(int page)
        {
            try
            {
                var contentrevisionList = new List<ContentRevisionDTO>();
            
                //commonCrawler로 부터 상속받은 부분
                //엠팍
                var mlbpark = webGetkr.Load("http://mlbpark.donga.com/mbs/articleL.php?mbsC=mlbtown&cpage=" + page);

                var mlbpark_ul = mlbpark.DocumentNode.SelectNodes("//ul[@id= 'mNewsList']").FirstOrDefault();
                var articles = mlbpark_ul.Descendants("li");

                foreach (var mlbpark_li in articles)
                {
                    var anchortag = mlbpark_li.SelectSingleNode("./a");
                    var titlenode = anchortag.SelectSingleNode("./span[@class='t']");
                    //var authnode = anchortag.SelectSingleNode("./span[@class='auth']");

                  
                    //var commentCount = titlenode.SelectNodes("./em").SingleOrDefault();
                    ////댓글 갯수
                    //if (commentCount != null)
                    //{
                    //    commentCount .Remove();
                    //}
                    
                    
                    var url = new Uri(new Uri("http://mlbpark.donga.com"), anchortag.GetAttributeValue("href", "undefined"));

                    //buildobject;
                    contentrevisionList.Add(new ContentRevisionDTO
                    {
                        Crawled = DateTime.Now,
                        isDepricate = false,
                        For_BoardId = 7,
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

                //var checkifdeprecated = loadedContent.DocumentNode.SelectNodes("//li[@class = 'hx_cate']");
                //if (checkifdeprecated != null && checkifdeprecated.FirstOrDefault().InnerText.Trim() == "삭제된 글입니다.")
                //{
                //    content.isDepricate = true;
                //    return;
                //}

                var articlecontent = loadedContent.DocumentNode.SelectNodes("//div[@class = 'article']").FirstOrDefault();
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
