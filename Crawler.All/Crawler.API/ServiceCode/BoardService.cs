using Crawler.Data.DbContext;
using Crawler.DTO.RequestDTO;
using Crawler.DTO.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crawler.API.ServiceCode
{
    public partial class CrawlerService : ServiceBase
    {
        /// <summary>
        /// Board를 BoardId 를 통해서 불러오는 함수
        /// </summary>
        /// <param name="req">
        /// BoardId
        /// </param>
        /// <returns></returns>
        public EnvelopeDTO<BoardDTO> Any(BoardGetbyIdRequestDTO req)
        {
            var boardid = req.BoardId;

            if (!boardid.HasValue)
            {
                return Fail<BoardDTO>("BoardGetRequestDTO : parameter 'boardid' is empty.");
            }
            else
            {
                using (var entities = new CrawlerStorage())
                {
                    try
                    {
                        var board = (from b in entities.Boards.AsNoTracking()
                                     where b.Id == boardid
                                     select new BoardDTO
                                     {
                                         Id = b.Id,
                                         For_WebsiteId = b.For_WebsiteId,
                                         Label = b.Label
                                     }).SingleOrDefault();

                        board.Website = (from w in entities.Websites.AsNoTracking()
                                         where w.Id == board.For_WebsiteId
                                         select new WebsiteDTO
                                         {
                                             Id = w.Id,
                                             Label = w.Label,
                                             Website_URL = w.Website_URL,
                                             Mobile_URL = w.Mobile_URL
                                         }).SingleOrDefault();

                        board.Snapshots = (from s in entities.Snapshots.AsNoTracking()
                                           where s.For_BoardId == board.Id
                                           select new SnapshotDTO
                                           {
                                               Id = s.Id,
                                               For_BoardId = s.For_BoardId,
                                               For_Timeperiod = s.For_Timeperiod,
                                               Taken = s.Taken,
                                               TimePeriod = new TimePeriodDTO
                                               {
                                                   Id = s.TimePeriod.Id,
                                                   Label = s.TimePeriod.Label,
                                                   Crawled = s.TimePeriod.Crawled,
                                                   Scheduled = s.TimePeriod.Scheduled
                                               },
                                               Board = new BoardDTO
                                               {
                                                   Id = s.Board.Id,
                                                   Label = s.Board.Label,
                                                   For_WebsiteId = s.Board.For_WebsiteId
                                               },
                                               ContentRevisions = (from c in s.SnapshotToContentRevisions
                                                                   let con = c.ContentRevision
                                                                   select new ContentRevisionDTO
                                                                   {
                                                                       recommandCount = con.RecommandCount,
                                                                       viewCount = con.ViewCount,
                                                                       For_ContentId = con.For_ContentId,
                                                                       Details = con.Details,
                                                                       Details_Html = con.Details_Html,
                                                                       Content = new ContentDTO
                                                                       {
                                                                           Article = con.Content.Article,
                                                                           Contents_URL = con.Content.Contents_URL,
                                                                       }

                                                                   }).ToList()
                                           }).OrderByDescending(s=>s.Id).ToList();
                        if (board == null)
                        {
                            return Fail<BoardDTO>("BoardGetRequestDTO : Board matching given 'BoardId' does not exist.");
                        }

                        return Succeeded(board);
                    }
                    catch (Exception e)
                    {
                        return Fail<BoardDTO>("BoardGetRequestDTO : Exception - " + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Board를 Label, For_WebsiteId를 통해서 Create하는 함수
        /// </summary>
        /// <param name="req">
        /// Label
        /// </param>
        /// <param name="req">
        /// For_WebsiteId
        /// </param>
        /// <returns></returns>
        public EnvelopeDTO<GenericDummyDTO> Any(BoardCreateRequestDTO req)
        {
            var Label = req.Label;
            var For_WebsiteId = req.For_WebsiteId;

            if(!For_WebsiteId.HasValue && !string.IsNullOrWhiteSpace(Label))
            {
                return Fail<GenericDummyDTO>("BoardCreateRequestDTO : parameter 'For_WebsiteId' is empty.");
            }
            if(For_WebsiteId.HasValue && string.IsNullOrWhiteSpace(Label))
            {
                return Fail<GenericDummyDTO>("BoardCreateRequestDTO : parameter 'For_WebsiteId' is empty.");
            }
            if(!For_WebsiteId.HasValue && string.IsNullOrWhiteSpace(Label))
            {
                return Fail<GenericDummyDTO>("BoardCreateRequestDTO : parameter 'For_WebsiteId' && 'Label' are empty.");
            }
            else
            {
                using (var entities = new CrawlerStorage())
                {
                    try
                    {
                        var board = new Board();
                        board.Label = Label;
                        board.For_WebsiteId = For_WebsiteId.Value;

                        entities.Boards.Add(board);

                        entities.SaveChanges();

                        return Succeeded(new GenericDummyDTO());
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                }
            }
        }

        /// <summary>
        /// WebsiteList를 가져오는 함수
        /// </summary>
        /// <returns>
        /// WebsiteList
        /// </returns>
        public EnvelopeDTO<List<BoardDTO>> Any(BoardGetListRequestDTO req)
        {
            using (var entities = new CrawlerStorage())
            {
                var boards = (from b in entities.Boards.AsNoTracking()
                                orderby b.Id
                                select new BoardDTO
                                {
                                    Id = b.Id,
                                    For_WebsiteId = b.For_WebsiteId,
                                    Label = b.Label,
                                }).ToList();
                foreach(var board in boards)
                {
                    board.Website = (from w in entities.Websites.AsNoTracking()
                                     where w.Id == board.For_WebsiteId
                                     select new WebsiteDTO
                                     {
                                         Id = w.Id,
                                         Label = w.Label,
                                         Website_URL = w.Website_URL,
                                         Website_logo = w.website_logo,
                                     }).SingleOrDefault();

                    board.Snapshots = (from s in entities.Snapshots.AsNoTracking()
                                       where s.For_BoardId == board.Id
                                       select new SnapshotDTO
                                       {
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

                                                                   Content = new ContentDTO
                                                                   {
                                                                       Article = con.Content.Article,
                                                                       Contents_URL = con.Content.Contents_URL,
                                                                   }
                                                               }).ToList()
                                       }).ToList();

                    List<ContentRevisionDTO> ContentRevisionList = new List<ContentRevisionDTO>();
                    List<ContentRevisionDTO> finalList = new List<ContentRevisionDTO>();
                    
                    foreach (var snapshot in board.Snapshots)
                    {
                        foreach (var contentrevision in snapshot.ContentRevisions)
                        {
                            ContentRevisionList.Add(contentrevision);
                        }
                    }

                    finalList = ContentRevisionList.GroupBy(p => p.For_ContentId).Select(g => g.First()).ToList();

                    board.Snapshots = null;
                    board.ContentRevisions = finalList;
                }
                return Succeeded(boards);
            }
        }


        /// <summary>
        /// 각 Board에 해당하는 ContentRevision을 Snapshot에 상관없이 중복없이 들고오는 코드
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        
        }
    }
