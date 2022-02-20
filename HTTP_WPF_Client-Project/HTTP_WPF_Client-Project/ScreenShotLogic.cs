using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace HTTP_WPF_Client_Project
{
    class ScreenshotLogic
    {
        public static byte[] createScreenshot()
        {
            var size = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics graph = Graphics.FromImage(bmp);
            graph.CopyFromScreen(0, 0, 0, 0, new Size(size.Width, size.Height));
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            byte[] imagebt = new byte[3110400];
            return ms.ToArray();
        }
    }
}
