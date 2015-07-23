using Crawler.Data.DbContext;
using Crawler.DTO.RequestDTO;
using Crawler.DTO.ResponseDTO;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Crawler.API.ServiceCode
{
    public partial class CrawlerService : ServiceBase
    {
        /// <summary>
        /// Cotent를 contentId를 통해서 불러오는 함수
        /// </summary>SSSSSS
        /// <param name="req">
        /// ContentId
        /// </param>
        /// <returns></returns>
        public EnvelopeDTO<ContentRevisionDTO> Any(ContentRevisionGetbyCheckSumRequestDTO req)
        {
            var checksum = req.CheckSum;

            if (string.IsNullOrWhiteSpace(checksum))
            {
                return Fail<ContentRevisionDTO>("ContentRevisionGetbyCheckSumRequestDTO : parameter 'checksum' is empty.");
            }
            else
            {
                using (var entities = new CrawlerStorage())
                {
                    try
                    {
                        var contentrevision = (from c in entities.ContentRevisions.AsNoTracking()
                                               where c.CheckSum == checksum
                                               select new ContentRevisionDTO
                                               {
                                                   id = c.Id
                                                   //SrcDatas = (from src in c.Srcdatas
                                                   //            select new SrcdataDTO
                                                   //            {
                                                   //                Id = src.Id,
                                                   //                SrcGuId = src.SrcGuId,
                                                   //                For_ContentId = src.For_ContentId,
                                                   //                SourceUrl = src.SourceUrl
                                                   //            }).ToList()
                                               }).SingleOrDefault();

                        if (contentrevision == null)
                        {
                            return Fail<ContentRevisionDTO>("ContentRevisionGetbyCheckSumRequestDTO : Contents matching given 'contentrevision' does not exist.");
                        }

                        return Succeeded(contentrevision);
                    }

                    catch (Exception e)
                    {
                        return Fail<ContentRevisionDTO>("ContentRevisionGetbyCheckSumRequestDTO : Exception - " + e.Message);
                    }
                }
            }
        }

        public EnvelopeDTO<List<ContentRevisionDTO>> Any(ContentRevisionListGetbyBoardIdListRequestDTO req)
        {
            var boardid = req.boardId;

            List<ContentRevisionDTO> ContentRevisionList = new List<ContentRevisionDTO>();
            if (!boardid.HasValue)
            {
                return Fail<List<ContentRevisionDTO>>("BoardGetContentRevisionListRequestDTO : parameter 'boardid' is empty.");
            }
            using (var entities = new CrawlerStorage())
            {
                var board = (from b in entities.Boards.AsNoTracking()
                             where b.Id == boardid
                             select new BoardDTO
                             {
                                 Id = b.Id,
                                 For_WebsiteId = b.For_WebsiteId,
                                 Label = b.Label,
                             }).SingleOrDefault();

                board.Snapshots = (from s in entities.Snapshots.AsNoTracking()
                                   where s.For_BoardId == board.Id
                                   select new SnapshotDTO
                                   {
                                       Id = s.Id,
                                       For_BoardId = s.For_BoardId,
                                       For_Timeperiod = s.For_Timeperiod,
                                       Taken = s.Taken,

                                       ContentRevisions = (from c in s.SnapshotToContentRevisions
                                                           let con = c.ContentRevision
                                                           select new ContentRevisionDTO
                                                           {
                                                               recommandCount = con.RecommandCount,
                                                               viewCount = con.ViewCount,
                                                               For_ContentId = con.For_ContentId,
                                                               Details = con.Details,
                                                               Details_Html = con.Details_Html,
                                                               Crawled = con.Crawled,

                                                               Board = new BoardDTO
                                                               {
                                                                   Id = boardid.Value
                                                               },

                                                               Content = new ContentDTO
                                                               {
                                                                   Article = con.Content.Article,
                                                                   Contents_URL = con.Content.Contents_URL,
                                                               }
                                                           }).ToList()
                                   }).ToList();
                
                foreach (var snapshot in board.Snapshots)
                {
                    foreach (var contentrevision in snapshot.ContentRevisions)
                    {
                        ContentRevisionList.Add(contentrevision);
                    }
                }

                var contentList1 = ContentRevisionList;

                List<ContentRevisionDTO> finalList = contentList1.GroupBy(p => p.For_ContentId).Select(g=>g.First()).ToList();

                return Succeeded(finalList);
            }
        }
        public EnvelopeDTO<List<ContentRevisionDTO>> Any(ContentRevisionListGetbyKeywordRequestDTO req)
        {
            var Keyword = req.Keyword;
            List<ContentRevisionDTO> searchedContentRevisionList = new List<ContentRevisionDTO>();

            if (string.IsNullOrWhiteSpace(Keyword))
            {
                return Fail<List<ContentRevisionDTO>>("ContentRevisionListGetbyKeywordRequestDTO : parameter 'Keyword' is empty.");
            }
            else
            {
                using (var entites = new CrawlerStorage())
                {
                    try
                    {
                        var contentRevisions = (from c in entites.ContentRevisions
                                                select c).ToList();

                        foreach (var eachcontentRevision in contentRevisions)
                        {
                            if (eachcontentRevision.Content.Article.Contains(Keyword))
                            {
                                searchedContentRevisionList.Add(new ContentRevisionDTO
                                {
                                    Crawled = eachcontentRevision.Crawled,
                                    Details = eachcontentRevision.Details,
                                    Details_Html = eachcontentRevision.Details_Html,
                                    Board = new BoardDTO{
                                         Website = new WebsiteDTO
                                         {
                                             Website_logo = eachcontentRevision.SnapshotToContentRevisions.First().Snapshot.Board.Website.website_logo,
                                             Website_URL = eachcontentRevision.SnapshotToContentRevisions.First().Snapshot.Board.Website.Website_URL,
                                             Label = eachcontentRevision.SnapshotToContentRevisions.First().Snapshot.Board.Website.Label
                                         }
                                    },
                                    Content = new ContentDTO
                                    {
                                        Article = eachcontentRevision.Content.Article,
                                        Contents_URL = eachcontentRevision.Content.Contents_URL
                                    }
                                });

                            }
                        }

                        if (searchedContentRevisionList.Count == 0)
                        {
                            return Fail<List<ContentRevisionDTO>>("ContentRevisionListGetbyKeywordRequestDTO : contentRevisions is null");
                        }


                        return Succeeded(new List<ContentRevisionDTO>(searchedContentRevisionList));
                    }
                    catch (Exception e)
                    {
                        return Fail<List<ContentRevisionDTO>>("ContentRevisionListGetbyKeywordRequestDTO : Exception - " + e.Message);
                    }
                }
            }
        }
    }
}