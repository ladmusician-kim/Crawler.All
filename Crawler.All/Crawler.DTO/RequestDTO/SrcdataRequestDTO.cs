using Crawler.DTO.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.DTO.RequestDTO
{
    public class SrcdataGetbycontentIdRequestDTO
    {
        public int? For_ContentId { get; set; }
    }
    public class SrcdataCreateRequestDTO
    {
        public SrcdataDTO Srcdata { get; set; }
    }
}