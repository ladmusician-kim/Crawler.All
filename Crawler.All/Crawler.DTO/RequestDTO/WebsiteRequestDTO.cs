﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.DTO.RequestDTO
{
    public class WebsiteGetbyIdRequestDTO
    {
        public int? WebsiteId { get; set; }
    }

    public class WebsiteGetListRequestDTO
    {

    }
    public class WebsiteCreateRequestDTO
    {
        public string Label { get; set; }
        public string Website_URL { get; set; }
        public string Mobile_URL { get; set; }
    }
}