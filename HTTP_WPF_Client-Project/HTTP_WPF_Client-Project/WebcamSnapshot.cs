using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;

namespace HTTP_WPF_Client_Project
{
    class WebcamSnapshot
    {
        public static byte[] takeSnapshot()
        {
            VideoCapture videoCapture = new VideoCapture();
            if (videoCapture.IsOpened)
            {
                Bitmap bmp = videoCapture.QueryFrame().ToImage<Bgr, byte>().ToBitmap();
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                videoCapture.Stop();
                return ms.ToArray();
            }
            return null;
        }
    }
}
