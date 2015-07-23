using ImageProcessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Crawler.API.Helper
{
    public class ImageHelper
    {
        private byte[] _payload { get; set; }
        private ImageFactory _fac { get; set; } 

        public ImageHelper(byte[] payload)
        {
            _fac = new ImageFactory(false);
            this.Load(payload);
        }


        public void Load(byte[] payload)
        {
            _fac.Load(new MemoryStream(payload));
        }

        public void BuildThumbnail()
        {
            //resize 하고 잘라낸다. 
        }

        public void MD5HashChecksum()
        {

        }

        public byte[] SaveImage(int? quality)
        {
            var newstream = new MemoryStream();

            _fac.Format(_fac.CurrentImageFormat);
            _fac.Quality(quality.GetValueOrDefault(65));
            _fac.Save(newstream);

            return newstream.ToArray();
        }

        public void SaveWebP()
        {

        }
    }
}