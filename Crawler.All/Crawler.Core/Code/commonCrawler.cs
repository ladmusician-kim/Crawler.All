using Crawler.DTO.RequestDTO;
using Crawler.DTO.ResponseDTO;
using HtmlAgilityPack;
using Sitecrawler.Core.Interface;
using System.Collections.Generic;
using System.Text;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Crawler.API.Helper;
using System.Web;

using ServiceStack.Plugins.ProtoBuf;
using ServiceStack.ServiceClient.Web;

namespace Sitecrawler.Core.Code
{
  
    public abstract class commonCrawler : ICrawler
    {
        
        ProtoBufServiceClient client;
        protected SnapshotDTO Snapshot;

        public commonCrawler()
        {
            try
            {
                client = new ProtoBufServiceClient("http://localhost:61910");
            }
            catch (Exception e)
            {

            }
        }
        
        protected abstract int perpage { get;}
        
        //ContentRevigion을 Count 갯수 만큼 가져오는 함수.
        public void GetList(int count, int for_boardid)
        {
            var pagecount = (int)((count - 1) / perpage);

            var dic = new Dictionary<int, List<ContentRevisionDTO>>();

            try
            {
                Parallel.For(1, pagecount + 2, i =>
                {
                    var list = GetContentsPerPage(i);
                    if(list == null)
                    {
                        Console.WriteLine(for_boardid + "GetContentsPerPage 할때 문제가 있습니다.");
                    }
                    else
                    {
                        Console.WriteLine(for_boardid + "GetContentsPerPage이 정상적으로 작동됩니다.");
                    }
                    
                    dic.Add(i, list);
                });

                var orderedlist = dic.OrderBy(d => d.Key);
                var finallist = new List<ContentRevisionDTO>();

                foreach (var eachList in orderedlist)
                {
                    finallist.AddRange(eachList.Value);
                }

                Snapshot = new SnapshotDTO
                {
                    ContentRevisions = finallist.Take(count).ToList(),
                };
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //page에 맞춘 ContentReivigion의 Content의 Article, Url_Params 등을 가져옴.
        protected abstract List<ContentRevisionDTO> GetContentsPerPage(int page);

        //ParseContent-> ContentRevigion의 Details, Detail_Html등의 정보를 가져옴
        //CacheImgae -> Srcdata가 있을경우 이미지 파일을 파싱 해옴.
        //CompareContent -> DB에 저장되어 있는, ContentRevigion이랑 비교할때, 중복 체크와 DB 저장을 함.
        public void ParseArticles()
        {
            var for_boardid = 0;
            if (Snapshot.ContentRevisions != null)
            {
                Parallel.ForEach(Snapshot.ContentRevisions, contentrevision =>
                {
                    ParseContent(contentrevision);
                    CacheImage(contentrevision);
                    for_boardid = contentrevision.For_BoardId;
                });
            }
            CompareContent(for_boardid, Snapshot);
        }

        //ContentRevigion의 Details, Detail_Html등의 정보를 가져옴
        protected abstract void ParseContent(ContentRevisionDTO contentrevision);

        //Srcdata가 있을경우 이미지 파일을 파싱 해옴.
        protected void CacheImage(ContentRevisionDTO ContentRevision)
        {
            try
            {
                if (ContentRevision.SrcDatas != null)
                {
                    Parallel.ForEach(ContentRevision.SrcDatas, srcdata =>
                    {
                        var client = new WebClient();
                        //System.Console.WriteLine(content.Contents_URL);
                        var url = new Uri(HttpUtility.HtmlDecode(srcdata.SourceUrl));

                        client.Headers.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                        client.Headers.Add("Referer", ContentRevision.Content.Contents_URL);
                        client.Headers.Add("Accept-Encoding", @"gzip, deflate, sdch");
                        client.Headers.Add("Accept-Language", @"ko,en-US;q=0.8,en;q=0.6");
                        client.Headers.Add("User-Agent", webGetkr.UserAgent);
                        client.UseDefaultCredentials = true;
                        try
                        {
                            var data = client.DownloadData(url);

                            srcdata.OriginalPayload = data;
                            srcdata.OriginalPayload_Size = data.LongLength;
                        }
                        catch (ArgumentNullException)
                        {
                            try
                            {
                                var data = client.DownloadData(url);

                                srcdata.OriginalPayload = data;
                                srcdata.OriginalPayload_Size = data.LongLength;
                            }
                            catch (ArgumentNullException)
                            {
                                try
                                {
                                    var data = client.DownloadData(url);
                                    srcdata.OriginalPayload = data;
                                    srcdata.OriginalPayload_Size = data.LongLength;
                                }
                                catch (ArgumentNullException)
                                {
                                    srcdata.IsDepricated = true;
                                    return;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            var errorlog = new ErrorLogDTO
                            {
                                Error_Address = "SrcData",
                                Error_URL = url.AbsoluteUri,
                                Error_Details = e.Message.ToString(),
                                Hresult = e.HResult
                            };
                            //SendErrorLog(errorlog);

                            srcdata.IsDepricated = true;
                            return;
                        }
                    });
                }
            }
            catch (WebException wex)
            {
                if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    // error 404, do what you need to do
                }
            }
        }

        //DB에 저장되어 있는 해당 Boardid와 동일한 Snapshot의 가장 최신 정보를 불러와 ContentReivigion의 중복을 체크하는 함수.
        public void CompareContent(int for_boardid, SnapshotDTO Snapshot)
        {
            var LastSnapshot = GetsnapshotbyBoard(for_boardid);

            var CurrentSnapshot = Snapshot;

            //예전 데이터가 없는 경우 -> Crawler가 처음 돌때.
            if (LastSnapshot == null)
            {
                foreach (var eachCurrentRevision in CurrentSnapshot.ContentRevisions)
                {
                    var Revisionchecksum = eachCurrentRevision.Details_Html + eachCurrentRevision.Content.Url_Params;
                    var checksum = Convert.ToBase64String(HashHelper.ObjectToMD5Hash(Revisionchecksum));
                    eachCurrentRevision.CheckSum = checksum;
                }
                Console.WriteLine("이번에" + for_boardid + "에서 새로 생성된 글은" + Snapshot.ContentRevisions.Count + "개 입니다.");

                //DB에 Snapshot을 저장하는 부분.
                SendSnapshot(CurrentSnapshot.ContentRevisions, for_boardid);
                return;
            }

            //예전 데이터와 비교하는 부분.
            foreach (var eachCurrentRevision in CurrentSnapshot.ContentRevisions)
            {
                var Revisionchecksum = eachCurrentRevision.Details_Html + eachCurrentRevision.Content.Url_Params;
                var checksum = Convert.ToBase64String(HashHelper.ObjectToMD5Hash(Revisionchecksum));
                eachCurrentRevision.CheckSum = checksum;

                //예전 ContentReivigion의 CheckSum과 비교를 하여, 같은 경우, 예전 ContentReivigon의 아이디를 부여함.
                foreach (var eachLastRevision in LastSnapshot.ContentRevisions)
                {
                    if (eachCurrentRevision.CheckSum.Equals(eachLastRevision.CheckSum))
                    {
                        eachCurrentRevision.id = eachLastRevision.id;
                    }
                }
            }

            //중복된 데이터와 달리 처음으로 들어간 데이터는 Id를 설정 해 주지 않았으므로, Id가 0이 됨.
            Console.WriteLine("이번에" + for_boardid + "에서 새로 생성된 글은" + CurrentSnapshot.ContentRevisions.Where(p => p.id.Equals(0)).Count() + "개 입니다.");
            
            //DB에 Snapshot을 저장하는 부분.
            SendSnapshot(CurrentSnapshot.ContentRevisions, for_boardid);
        }

        //해당하는 BoardId를 가진 Snapshot의 가장 최근 데이터를 불러옴.
        public SnapshotDTO GetsnapshotbyBoard(int boardid)
        {
            try
            {
                var dto = client.Send<EnvelopeDTO<SnapshotDTO>>(new SnapshotGetbyBoardIdRequestDTO
                {
                    For_BoardId = boardid
                });

                return dto.SafeBody;
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("메모리 오류 입니다.");

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SnapshotDTO SendSnapshot(List<ContentRevisionDTO> ContentRevisionList, int for_boardid)
        {
            //snapshot 저장 부분;
            SnapshotDTO snapshot;
            snapshot = new SnapshotDTO
            {
                For_BoardId = for_boardid,
                Taken = DateTime.Now,
                ContentRevisions = ContentRevisionList
            };

            try
            {
                var dto = client.Send<EnvelopeDTO<SnapshotDTO>>(new SnapshotCreateRequestDTO
                {
                    Snapshot = snapshot
                });

                return dto.SafeBody;
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("메모리 오류 입니다.");

                return null;
            }catch(NotImplementedException ne)
            {
                Console.WriteLine(ne);
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ErrorLogDTO SendErrorLog (ErrorLogDTO ErrorLog)
        {
            ErrorLogDTO errorlog;
            errorlog = new ErrorLogDTO
            {
                Error_Address = ErrorLog.Error_Address,
                Error_URL = ErrorLog.Error_URL,
                Error_Details = ErrorLog.Error_Details,
                Hresult = ErrorLog.Hresult
            };

            var dto = new ErrorLogCreateRequestDTO
            {
                ErrorLog = errorlog
            };

            client.Send<EnvelopeDTO<ErrorLogDTO>>(dto);

            return dto.ErrorLog;
        }
        
        public SnapshotDTO getSnapshot(int snapshotId)
        {
            try
            {
                var dto = client.Send<EnvelopeDTO<SnapshotDTO>>(new SnapshotGetbyIdRequestDTO
                {
                    SnapshotId = snapshotId
                });

                return dto.SafeBody;
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("메모리 오류 입니다.");

                return null;
            }
            catch(Exception e)
            {
                return null;
            }

        }






        

        //공통으로 쓰이는 변수
        public HtmlWeb webGetkr = new HtmlWeb
        {
            UserAgent = @"Mozilla/5.0 (iPhone; U; CPU iPhone OS 4_3_2 like Mac OS X; en-us) AppleWebKit/533.17.9 (KHTML, like Gecko) Version/5.0.2 Mobile/8H7 Safari/6533.18.5",
            OverrideEncoding = Encoding.GetEncoding("euc-kr")
        };
        
        public HtmlWeb webGetutf = new HtmlWeb
        {
            UserAgent = @"Mozilla/5.0 (iPhone; U; CPU iPhone OS 4_3_2 like Mac OS X; en-us) AppleWebKit/533.17.9 (KHTML, like Gecko) Version/5.0.2 Mobile/8H7 Safari/6533.18.5",
            OverrideEncoding = Encoding.GetEncoding("UTF-8")
        };
        
        //Icrawler 에서 상속받은 부분
       
    }
}
