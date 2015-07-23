using Crawler.DTO.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.DTO.RequestDTO
{
    public class ContentRevisionGetbyCheckSumRequestDTO
    {
        public string CheckSum { get; set;}
    }
    public class ContentRevisionGetbySearchKeywordRequestDTO
    {
        public string Keyword { get; set; }
    }

    public class ContentRevisionListGetbyBoardIdListRequestDTO
    {
        public int? boardId { get; set; }
    }
    public class ContentRevisionListGetbyKeywordRequestDTO
    {
        public string Keyword { get; set; }
    }
}
