using Crawler.DTO.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.DTO.RequestDTO
{
    public class ContentCreateRequestDTO
    {
        public ContentDTO Content { get; set; }
    }
    public class ContentGetbyIdRequestDTO
    {
        public int? ContentId { get; set; }
    }
    public class ContentUpdateDeatilsRequestDTO
    {
        public int? ContentId { get; set; }
        public string Details { get; set; }
        public string Details_Html { get; set; }
    }
    public class ContentUpdateImagesRequestDTO
    {
        public int? ContentId { get; set; }
        public List<SrcdataDTO> SrcData { get; set; }
    }
    public class ContentGetbySearchKeywordRequestDTO
    {
        public string Keyword { get; set; }
    }
}
