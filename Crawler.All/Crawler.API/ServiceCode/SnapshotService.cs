using AutoMapper;
using Crawler.API.Helper;
using Crawler.Data.DbContext;
using Crawler.DTO.RequestDTO;
using Crawler.DTO.ResponseDTO;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Crawler.API.ServiceCode
{
    public partial class CrawlerService : ServiceBase
    {
        //public EnvelopeDTO<SnapshotDTO> Any(SnapshotGetbyIdRequestDTO req)
        //{
        //    var snapshotId = req.SnapshotId;

        //    if (!snapshotId.HasValue)
        //    {
        //        return Fail<SnapshotDTO>("SnapshotGetbyIdRequestDTO : parameter 'snapshotId' is empty.");
        //    }

        //    else
        //    {
        //        using (var entities = new CrawlerStorage())
        //        {
        //            try
        //            {
        //                var snapshot = (from s in entities.Snapshots.AsNoTracking()
        //                                where s.Id == snapshotId
        //                                select new SnapshotDTO
        //                                {
        //                                    Id = s.Id,
        //                                    For_BoardId = s.For_BoardId,
        //                                    For_Timeperiod = s.For_Timeperiod,
        //                                    Taken = s.Taken,
        //                                    TimePeriod = new TimePeriodDTO
        //                                    {
        //                                        Id = s.TimePeriod.Id,
        //                                        Label = s.TimePeriod.Label,
        //                                        Crawled = s.TimePeriod.Crawled,
        //                                        Scheduled = s.TimePeriod.Scheduled
        //                                    },
        //                                    Board = new BoardDTO
        //                                    {
        //                                        Id = s.Board.Id,
        //                                        Label = s.Board.Label,
        //                                        For_WebsiteId = s.Board.For_WebsiteId
        //                                    },
        //                                    ContentRevisions = (from c in s.SnapshotToContentRevisions
        //                                                        let con = c.ContentRevision
        //                                                        select new ContentRevisionDTO
        //                                                        {
        //                                                            Crawled = con.Crawled,
        //                                                            Details = con.Details,
        //                                                            Details_Html = con.Details_Html,
        //                                                            isDepricate = con.IsDepricated,
        //                                                        }).ToList(),
        //                                }).SingleOrDefault();

        //                var dto = Mapper.DynamicMap<SnapshotDTO>(snapshot);

        //                return Succeeded(dto);
        //            }

        //            catch (Exception e)
        //            {
        //                return Fail<SnapshotDTO>("SnapshotGetbyIdRequestDTO : Exception --->" + e);
        //            }
        //        }
        //    }
        //}
        public EnvelopeDTO<SnapshotDTO> Any(SnapshotSaveRequestDTO req)
        {
            var timeslot = req.TimeSlot;
            return null;
        }

        /// <summary>
        /// Snapshot을 SnapshotDTO를 통해서 Create 하는 함수
        /// </summary>
        /// <param name="req">
        /// SnapshotDTO
        /// </param>
        /// <returns></returns>
        public EnvelopeDTO<SnapshotDTO> Any(SnapshotCreateRequestDTO req)
        {
            // 단정하기.
            Debug.Assert(req.Snapshot.Id == 0);
            Debug.Assert(req.Snapshot.ContentRevisions != null);

            var Snapshot = req.Snapshot;

            using (var entities = new CrawlerStorage())
            {
                try
                {
                    // 가장 최근의 TimePeriod를 가져온다.
                    var timeperiod = entities.TimePeriods.OrderByDescending(t => t.Scheduled).FirstOrDefault();

                    var snapshot = new Snapshot
                    {
                        For_Timeperiod = timeperiod.Id,
                        Taken = Snapshot.Taken,
                        For_BoardId = Snapshot.For_BoardId
                    };

                    entities.Snapshots.Add(snapshot);

                    // 해당하는 boardId의 가장 최근의 Snapshot을 가져온다.
                    var LastSnapshot = entities.Snapshots.Where(p => p.For_BoardId == Snapshot.For_BoardId).OrderByDescending(t => t.Id).FirstOrDefault();
                    if (LastSnapshot != null)
                    {
                        foreach (var eachCurrentRevision in Snapshot.ContentRevisions)
                        {
                            foreach (var eachLastRevision in LastSnapshot.SnapshotToContentRevisions)
                            {
                                if (eachCurrentRevision.CheckSum.Equals(eachLastRevision.ContentRevision.CheckSum))
                                {
                                    eachCurrentRevision.id = eachLastRevision.ContentRevision.Id;
                                }
                            }
                        }
                    }
                    

                    int contentseq = 0;
                    List<long> sizelist = new List<long>();

                    if (Snapshot.ContentRevisions == null)
                    {
                        return Fail<SnapshotDTO>("발생하면 안됨.");
                    }

                    foreach (var contentRevision in Snapshot.ContentRevisions)
                    {
                        // 이미 뭔가 있는 상황
                        if (contentRevision.id != 0)
                        {
                            var snapshotTocontentrevision = new SnapshotToContentRevision
                            {
                                Has_ContentRevisionId = contentRevision.id,
                                Seqno = ++contentseq,
                                Snapshot = snapshot
                            };

                            entities.SnapshotToContentRevisions.Add(snapshotTocontentrevision);
                            continue;
                        }

                        // snapshotcontentrevision.id -> snapshotcontentrevision가 DB에 있었던 정보인지 아닌지 판단함

                        var newContentRevision = new ContentRevision
                        {
                            Details = contentRevision.Details,
                            Details_Html = contentRevision.Details_Html,
                            Crawled = contentRevision.Crawled,
                            RecommandCount = contentRevision.recommandCount,
                            ViewCount = contentRevision.viewCount,
                            CheckSum = contentRevision.CheckSum
                        };

                        var contentChecksum = Convert.ToBase64String(HashHelper.ObjectToMD5Hash(contentRevision.Content.Url_Params));

                        var existingContent = entities.Contents.SingleOrDefault(p => p.CheckSum.Equals(contentChecksum));

                        var content = existingContent == null ? new Content
                        {
                            Article = contentRevision.Content.Article,
                            Contents_URL = contentRevision.Content.Contents_URL,
                            ContentGuId = Guid.NewGuid(),
                            Url_Params = contentRevision.Content.Url_Params,
                            CheckSum = Convert.ToBase64String(HashHelper.ObjectToMD5Hash(contentRevision.Content.Url_Params))
                        } : existingContent;

                        // 글 중에서 그림이 없는 글이 있을 수 있으므로 그림 여부 판단 -> 없으면 if문 해당 안되므로 저장을 안함.
                        if (contentRevision.SrcDatas != null)
                        {
                            foreach (var data in contentRevision.SrcDatas)
                            {
                                
                                // 그림 데이터 중에서 originalpayload가 없는 경우, commonCrawler에서 다운로드 이미지 하는 부분에서 isDepricated 변수가 true로 넘어오므로 판단할 수 있다.

                                if (data == null || data.OriginalPayload == null) continue;

                                var srcdataChecksum = Convert.ToBase64String(HashHelper.ObjectToMD5Hash(data.OriginalPayload));

                                var existingSrcdataId = entities.Srcdatas.Where(s => s.CheckSum.Equals(srcdataChecksum)).Select(s => s.Id).FirstOrDefault();

                                // ExistSrcdata는 기존 DB에서 가지고 있던 Srcdata 이미지와 중복되는지 여부를 판단 해준다.
                                if (existingSrcdataId == 0)
                                {
                                    var srcdata = new Srcdata{
                                        SrcGuId = Guid.NewGuid(),
                                        Original_SourceUrl = data.SourceUrl,
                                        Content = content,
                                        FileName = data.FileName,
                                        OriginalPayload = data.OriginalPayload,
                                        OriginalPayload_Size = data.OriginalPayload_Size,
                                        CheckSum = srcdataChecksum
                                    };

                                    entities.Srcdatas.Add(srcdata);

                                }
                                  
                            }
                        }
                        newContentRevision.Content = content;
                        entities.ContentRevisions.Add(newContentRevision);
                        entities.SnapshotToContentRevisions.Add(new SnapshotToContentRevision
                        {
                            ContentRevision = newContentRevision,
                            Snapshot = snapshot,
                            Seqno = ++contentseq,
                        });
                    }
                    try
                    {
                        entities.SaveChanges();
                        return Succeeded(new SnapshotDTO());
                    }
                    catch (Exception e)
                    {
                        return null;
                    }



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



        public EnvelopeDTO<List<SnapshotDTO>> Any(SnapshotListGetbyDataAndBoardIdRequestDTO req)
        {
            var startdate = req.StartDate;
            var enddate = req.EndDate;
            List<int> BoardList = req.boardIdList;
            List<SnapshotDTO> SnapshotList = new List<SnapshotDTO>();

            if (!startdate.HasValue && enddate.HasValue && BoardList.Count != 0)
            {
                return Fail<List<SnapshotDTO>>("ContentRevisionListGetbyDataAndBoardIdRequestDTO : parameter 'startdate' is empty.");
            }
            if (startdate.HasValue && !enddate.HasValue && BoardList.Count != 0)
            {
                return Fail<List<SnapshotDTO>>("ContentRevisionListGetbyDataAndBoardIdRequestDTO : parameter 'enddate' is empty.");
            }
            if (startdate.HasValue && enddate.HasValue && BoardList.Count == 0)
            {
                return Fail<List<SnapshotDTO>>("ContentRevisionListGetbyDataAndBoardIdRequestDTO : parameter 'BoardList' is empty.");
            }
            if (!startdate.HasValue && !enddate.HasValue && BoardList.Count != 0)
            {
                return Fail<List<SnapshotDTO>>("ContentRevisionListGetbyDataAndBoardIdRequestDTO : parameter 'startdate' and 'enddate' is empty.");
            }
            if (!startdate.HasValue && enddate.HasValue && BoardList.Count != 0)
            {
                return Fail<List<SnapshotDTO>>("ContentRevisionListGetbyDataAndBoardIdRequestDTO : parameter 'startdate' and 'BoardList' is empty.");
            }
            if (startdate.HasValue && !enddate.HasValue && BoardList.Count != 0)
            {
                return Fail<List<SnapshotDTO>>("ContentRevisionListGetbyDataAndBoardIdRequestDTO : parameter 'enddate' and 'BoardList' is empty.");
            }

            else
            {
                using (var entities = new CrawlerStorage())
                {
                    try
                    {
                        foreach (var boardid in BoardList)
                        {
                            var snapshots = (from s in entities.Snapshots.AsNoTracking()
                                             where s.For_BoardId == boardid
                                            select new SnapshotDTO
                                            {
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
                                                                        Crawled = con.Crawled,
                                                                        Details = con.Details,
                                                                        Details_Html = con.Details_Html,
                                                                        isDepricate = con.IsDepricated,
                                                                    }).ToList(),
                                            }).ToList();

                            foreach(var snapshot in snapshots)
                            {
                                SnapshotList.Add(snapshot);
                            }
                        }

                        if (SnapshotList.Count == 0)
                        {
                            return Fail<List<SnapshotDTO>>("ContentRevisionListGetbyDataAndBoardIdRequestDTO : SnapshotList is null");
                        }

                        return Succeeded(new List<SnapshotDTO>(SnapshotList));
                    }
                    catch (Exception e)
                    {
                        return Fail<List<SnapshotDTO>>("ContentRevisionListGetbyDataAndBoardIdRequestDTO : Exception - " + e.Message);
                    }
                }
            }
        }

        ///// <summary>
        ///// SnapshotDTO를 for_boardid를 통해서 가져오는 함수
        ///// </summary>
        ///// <param name="req">
        ///// for_boardid
        ///// </param>
        ///// <returns>
        ///// SnapshotDTO
        ///// </returns>
        //public EnvelopeDTO<SnapshotDTO> Any(SnapshotGetbyBoardIdRequestDTO req)
        //{
        //    var for_boardid = req.For_BoardId;

        //    if (!for_boardid.HasValue)
        //    {
        //        return Fail<SnapshotDTO>("SnapshotGetbyIdRequestDTO : parameter 'for_boardid' is empty.");
        //    }

        //    else
        //    {
        //        using (var entities = new CrawlerStorage())
        //        {
        //            try
        //            {
        //                var snapshots = (from s in entities.Snapshots.AsNoTracking()
        //                                where s.For_BoardId == for_boardid
        //                                select new SnapshotDTO
        //                                {
        //                                    Id = s.Id,
        //                                    For_BoardId = s.For_BoardId,
        //                                    For_Timeperiod = s.For_Timeperiod,
        //                                    Taken = s.Taken,
        //                                    TimePeriod = new TimePeriodDTO
        //                                    {
        //                                        Id = s.TimePeriod.Id,
        //                                        Label = s.TimePeriod.Label,
        //                                        Crawled = s.TimePeriod.Crawled,
        //                                        Scheduled = s.TimePeriod.Scheduled
        //                                    },
        //                                    Board = new BoardDTO
        //                                    {
        //                                        Id = s.Board.Id,
        //                                        Label = s.Board.Label,
        //                                        For_WebsiteId = s.Board.For_WebsiteId
        //                                    },
        //                                    ContentRevisions = (from c in s.SnapshotToContentRevisions
        //                                                        let con = c.ContentRevision
        //                                                        select new ContentRevisionDTO
        //                                                        {
        //                                                            Crawled = con.Crawled,
        //                                                            Details = con.Details,
        //                                                            Details_Html = con.Details_Html,
        //                                                            isDepricate = con.IsDepricated,
        //                                                        }).ToList(),
        //                                }).ToList().OrderByDescending(p => p.Id).FirstOrDefault();
        //                if (snapshots == null)
        //                {
        //                    return null;
        //                }

        //                var dto = Mapper.DynamicMap<SnapshotDTO>(snapshots);

        //                return Succeeded(dto);
        //            }

        //            catch (Exception e)
        //            {
        //                return Fail<SnapshotDTO>("SnapshotGetbyIdRequestDTO : Exception --->" + e);
        //            }
        //        }
        //    }
        //}

        //public EnvelopeDTO<ItemsDTO<SnapshotDTO>> Any(SnapshotListGetbyBoardIdRequestDTO req)
        //{
        //    var for_boardid = req.For_BoardId;
        //    List<SnapshotDTO> SnapshotList = new List<SnapshotDTO>();

        //    if (!for_boardid.HasValue)
        //    {
        //        return Fail <ItemsDTO<SnapshotDTO>>("SnapshotListGetbyBoardIdRequestDTO : parameter 'for_boardid' is empty.");
        //    }

        //    using (var entities = new CrawlerStorage())
        //    {
        //        try
        //        {
        //            var snapshots = (from s in entities.Snapshots.AsNoTracking()
        //                             where s.For_BoardId == for_boardid
        //                             select new SnapshotDTO
        //                             {
        //                                 Id = s.Id,
        //                                 For_BoardId = for_boardid.Value,
        //                                 ContentRevisions = (from c in s.SnapshotToContentRevisions
        //                                                     let con = c.ContentRevision
        //                                                     select new ContentRevisionDTO
        //                                                     {
        //                                                         Crawled = con.Crawled,
        //                                                         Details = con.Details,
        //                                                         Details_Html = con.Details_Html,
        //                                                         isDepricate = con.IsDepricated,
        //                                                         For_ContentId = con.For_ContentId,
        //                                                     }).ToList(),
        //                             }).Take(10).ToList();

        //            foreach (var snapshot in snapshots)
        //            {
        //                SnapshotList.Add(new SnapshotDTO
        //                {
        //                    Id = snapshot.Id,
        //                    For_BoardId = for_boardid.Value,
        //                    ContentRevisions = snapshot.ContentRevisions
        //                });
        //            }

        //            if (snapshots == null)
        //            {
        //                return null;
        //            }

        //            return Succeeded(new ItemsDTO<SnapshotDTO> { Items = SnapshotList, RowCount = 0, PerPage = 10 });
        //        }

        //        catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
        //        {
        //            Exception raise = dbEx;
        //            foreach (var validationErrors in dbEx.EntityValidationErrors)
        //            {
        //                foreach (var validationError in validationErrors.ValidationErrors)
        //                {
        //                    string message = string.Format("{0}:{1}",
        //                        validationErrors.Entry.Entity.ToString(),
        //                        validationError.ErrorMessage);
        //                    raise = new InvalidOperationException(message, raise);
        //                }
        //            }
        //            throw raise;
        //        }
        //    }
        //}
    }
}