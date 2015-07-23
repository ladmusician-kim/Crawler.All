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
        /// SnapshotToContent를 For_SnapshotId, Has_ContentId, Seqno 통해서 Create 하는 함수
        /// </summary>
        /// <param name="req">
        /// For_SnapshotId
        /// </param>
        /// <param name="req">
        /// Has_ContentId
        /// </param>
        /// <param name="req">
        /// Seqno
        /// </param>
        /// <returns></returns>
        public EnvelopeDTO<GenericDummyDTO> Any(SnapshotToContentRevisionCreateRequestDTO req)
        {
            var For_SnapshotId = req.For_SnapshotId;
            var Has_ContentRevisionId = req.Has_ContentRevisionId;
            var Seqno = req.Seqno;

            if (!For_SnapshotId.HasValue && Has_ContentRevisionId.HasValue && Seqno.HasValue)
            {
                return Fail<GenericDummyDTO>("SnapshotToContentCreateRequestDTO : parameter 'For_SnapshotId' is empty.");
            }
            if (For_SnapshotId.HasValue && !Has_ContentRevisionId.HasValue && Seqno.HasValue)
            {
                return Fail<GenericDummyDTO>("SnapshotToContentCreateRequestDTO : parameter 'Has_ContentRevisionId' is empty.");
            }
            if (For_SnapshotId.HasValue && Has_ContentRevisionId.HasValue && !Seqno.HasValue)
            {
                return Fail<GenericDummyDTO>("SnapshotToContentCreateRequestDTO : parameter 'Seqno' is empty.");
            }
            if (!For_SnapshotId.HasValue && !Has_ContentRevisionId.HasValue && Seqno.HasValue)
            {
                return Fail<GenericDummyDTO>("SnapshotToContentCreateRequestDTO : parameter 'For_SnapshotId' && 'Has_ContentRevisionId' are empty.");
            }
            if (!For_SnapshotId.HasValue && Has_ContentRevisionId.HasValue && !Seqno.HasValue)
            {
                return Fail<GenericDummyDTO>("SnapshotToContentCreateRequestDTO : parameter 'For_SnapshotId' && 'Seqno' are empty.");
            }
            if (For_SnapshotId.HasValue && !Has_ContentRevisionId.HasValue && !Seqno.HasValue)
            {
                return Fail<GenericDummyDTO>("SnapshotToContentCreateRequestDTO : parameter 'Has_ContentRevisionId' && 'Seqno' are empty.");
            }
            if (!For_SnapshotId.HasValue && !Has_ContentRevisionId.HasValue && !Seqno.HasValue)
            {
                return Fail<GenericDummyDTO>("SnapshotToContentCreateRequestDTO : parameter 'For_SnapshotId' && 'Has_ContentRevisionId' && 'Seqno' are empty.");
            }
            else
            {
                using (var entities = new CrawlerStorage())
                {
                    try
                    {
                        var snapshotTocontentrevision = new SnapshotToContentRevision();

                        snapshotTocontentrevision.For_SnapshotId = For_SnapshotId.Value;
                        snapshotTocontentrevision.Has_ContentRevisionId = Has_ContentRevisionId.Value;
                        snapshotTocontentrevision.Seqno = Seqno.Value;

                        entities.SnapshotToContentRevisions.Add(snapshotTocontentrevision);

                        entities.SaveChanges();

                        return Succeeded(new GenericDummyDTO());
                    }
                    catch (Exception e)
                    {
                        return Fail<GenericDummyDTO>("e");
                    }
                }
            }
        }

        /// <summary>
        /// SnapshotToContentDTO를 For_SnapshotId, Has_ContentId를 통해서 가져오는 함수
        /// </summary>
        /// <param name="req">
        /// For_SnapshotId 
        /// </param>
        /// <param name="req">
        /// Has_ContentId
        /// </param>
        /// <returns></returns>
        public EnvelopeDTO<SnapshotToContentRevisionDTO> Any(SnapshotToContentRevisionGetbyIdRequestDTO req)
        {
            var For_SnapshotId = req.For_SnapshotId;
            var Has_ContentRevisionId = req.Has_ContentRevisionId;

            if (!For_SnapshotId.HasValue && Has_ContentRevisionId.HasValue)
            {
                return Fail<SnapshotToContentRevisionDTO>("SnapshotToContentRevisionGetbyIdRequestDTO : parameter 'For_SnapshotId' is empty.");
            }
            if (For_SnapshotId.HasValue && !Has_ContentRevisionId.HasValue)
            {
                return Fail<SnapshotToContentRevisionDTO>("SnapshotToContentRevisionGetbyIdRequestDTO : parameter 'Has_ContentRevisionId' is empty.");
            }
            if (!For_SnapshotId.HasValue && !Has_ContentRevisionId.HasValue)
            {
                return Fail<SnapshotToContentRevisionDTO>("SnapshotToContentRevisionGetbyIdRequestDTO : parameter 'For_SnapshotId' && 'Has_ContentRevisionId' are empty.");
            }
            else
            {
                using (var entities = new CrawlerStorage())
                {
                    try
                    {
                        var snapshotTocontentrevision = (from s in entities.SnapshotToContentRevisions.AsNoTracking()
                                                 where s.For_SnapshotId == For_SnapshotId && s.Has_ContentRevisionId == Has_ContentRevisionId
                                                 select new SnapshotToContentRevisionDTO
                                                 {
                                                     For_SnapshotId = s.For_SnapshotId,
                                                     Has_ContentRevisionId = s.Has_ContentRevisionId,
                                                     Seqno = s.Seqno
                                                 }).SingleOrDefault();

                        if (snapshotTocontentrevision == null)
                        {
                            return Fail<SnapshotToContentRevisionDTO>("SnapshotToContentRevisionGetbyIdRequestDTO : SnapshotToContentREvision matching given 'snapshotTocontentrevision' does not exist.");
                        }

                        return Succeeded(snapshotTocontentrevision);
                    }

                    catch (Exception e)
                    {
                        return Fail<SnapshotToContentRevisionDTO>("SnapshotToContentRevisionGetbyIdRequestDTO : Exception - " + e.Message);
                    }
                }
            }
        }
        public EnvelopeDTO<GenericDummyDTO> Any(SnapshotToContentRevisionUpdateRequestDTO req)
        {
            var Has_ContentRevisioid = req.Has_ContentRevisionId;
            var for_snapshotid = req.For_SnapshotId;
            var seqno = req.Seqno;

            using (var entities = new CrawlerStorage())
            {
                try
                {
                    var snapshotTocontentrevision = entities.SnapshotToContentRevisions.SingleOrDefault(p => p.Has_ContentRevisionId == Has_ContentRevisioid);
                    snapshotTocontentrevision.For_SnapshotId = for_snapshotid.Value;
                    snapshotTocontentrevision.Seqno = seqno.Value;
                    
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
}