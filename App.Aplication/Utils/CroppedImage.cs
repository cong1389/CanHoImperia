using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace App.Aplication
{
    public static class CroppedImage
	{
		public static string SaveCroppedImage(HttpPostedFileBase imageFile, string filePath, string fileName, int? width = null, int? height = null)
		{
            Image image = Image.FromStream(imageFile.InputStream);
            Guid guid = ImageFormat.Jpeg.Guid;
			string lower = Path.GetExtension(imageFile.FileName)?.ToLower();
			if (lower != null && (lower.Equals(".jpeg") || lower.Equals(".jpg")))
			{
				guid = ImageFormat.Jpeg.Guid;
			}
			if (lower != null && lower.Equals(".png"))
			{
				guid = ImageFormat.Png.Guid;
			}
			if (lower != null && lower.Equals(".gif"))
			{
				guid = ImageFormat.Gif.Guid;
			}
			if (lower != null && lower.Equals(".bmp"))
			{
				guid = ImageFormat.Bmp.Guid;
			}
			if (lower != null && lower.Equals(".tiff"))
			{
				guid = ImageFormat.Tiff.Guid;
			}
			ImageCodecInfo imageCodecInfo = ImageCodecInfo.GetImageEncoders().First(codecInfo => codecInfo.FormatID == guid);
			Image image1 = image;
			Bitmap bitmap = null;
            string result;
            try
            {
                int num = 0;
                int num1 = 0;
                int value = 1;
                int value1 = 1;
                value1 = height.Value;
                value = width.Value;
                bitmap = new Bitmap(value, value1);
                double num2 = value1 / (double)value;
                double num3 = value / (double)value1;
                if (image.Width <= image.Height)
                {
                    value1 = (int)Math.Round(image.Width * num2);
                    if (value1 >= image.Height)
                    {
                        value = (int)Math.Round(image.Width * (image.Height / (double)value1));
                        value1 = image.Height;
                        num = (image.Width - value) / 2;
                    }
                    else
                    {
                        value = image.Width;
                        num1 = (image.Height - value1) / 2;
                    }
                }
                else
                {
                    value = (int)Math.Round(image.Height * num3);
                    if (value >= image.Width)
                    {
                        value1 = (int)Math.Round(image.Height * (image.Width / (double)value));
                        value = image.Width;
                        num1 = (image.Height - value1) / 2;
                    }
                    else
                    {
                        value1 = image.Height;
                        num = (image.Width - value) / 2;
                    }
                }
                using (Graphics graphic = Graphics.FromImage(bitmap))
                {
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(num, num1, value, value1), GraphicsUnit.Pixel);
                }
                image1 = bitmap;
                using (EncoderParameters encoderParameter = new EncoderParameters(1))
                {
                    encoderParameter.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);
                    string empty = string.Empty;
                    empty = (!string.IsNullOrEmpty(fileName) ? $"{filePath}{string.Concat(fileName, lower)}" : $"{filePath}{string.Concat(Utils.GetTime(), lower)}"
                        );
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(string.Concat("~/", filePath))))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(string.Concat("~/", filePath)));
                    }
                    image1.Save(HttpContext.Current.Server.MapPath(string.Concat("~/", empty)), imageCodecInfo, encoderParameter);
                    if (bitmap != null)
                    {
                        bitmap.Dispose();
                    }
                    result = empty;
                }
            }
            catch (Exception ex)
            {
                bitmap?.Dispose();
                throw new Exception(string.Concat("Error First: ", ex.Message));
            }

            return result;
		}
	}
}