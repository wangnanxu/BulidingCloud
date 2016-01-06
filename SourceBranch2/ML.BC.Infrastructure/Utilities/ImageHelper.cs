using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.Utilities
{
    public static class ImageHelper
    {
        public static MemoryStream PressImage(Stream imgStream, int width = 120, int? height = null)
        {
            if (imgStream == null || imgStream.Length == 0) return new MemoryStream();
            using (var bitmap = new Bitmap(imgStream))
            {
                var size = GetthumbnailImageSize(bitmap.Width, bitmap.Height, width);
                using (Image myThu = bitmap.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero))
                {
                    var ms = new MemoryStream();
                    myThu.Save(ms, ImageFormat.Jpeg);
                    ms.Position = 0;
                    return ms;
                }
            }
        }
        private static Size GetthumbnailImageSize(int width, int height, int desHeight)
        {
            if (width < 1 || height < 1)
                return Size.Empty;
            var size = new Size(width, height);
            var ratio = desHeight * 1.0 / height;
            size.Height = Convert.ToInt32(height * ratio);
            size.Width = Convert.ToInt32(width * ratio);
            return size;
        }

        public static MemoryStream MakeThumbnailImage(Stream stream)
        {
            if (null == stream || stream.Length == 0) return new MemoryStream();
            using (var bitmap = new Bitmap(stream))
            {
                var size = GetthumbnailImageSize(bitmap.Width, bitmap.Height, 200);
                using (Image myThu = bitmap.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero))
                {
                    var ms = new MemoryStream();
                    myThu.Save(ms, ImageFormat.Jpeg);
                    ms.Position = 0;
                    return ms;
                }
            }
        }
        private static Stream CutImage(Stream imgStream, int width, int height)
        {
            //根据传递过来的范围，将该范围的图片画到画布上，将画布保存。
            using (Bitmap map = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(map))//为画布创建画笔.
                {
                    using (Image img = Image.FromStream(imgStream))//创建img
                    {
                        var zoom = width > height ? img.Width * 1.0 / width : img.Height * 1.0 / height;
                        int x = Convert.ToInt32(img.Width * (1 - zoom) / 2);
                        x = x < 0 ? 0 : x;
                        int y = Convert.ToInt32(img.Height * (1 - zoom) / 2);
                        y = y < 0 ? 0 : y;

                        //将图片画到画布上
                        //第一：对哪张图片进行操作
                        //二：画多么大
                        //三：画哪部分
                        g.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                        var resultStream = new MemoryStream();
                        map.Save(resultStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return resultStream;
                    }
                }
            }
        }
    }
}
