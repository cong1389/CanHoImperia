using System;
using System.Drawing;
using System.IO;
using System.Web;
using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Scaling;

namespace App.Aplication
{
    public class ImagePlugin : IImagePlugin
    {
        public void CropAndResizeImage(HttpPostedFileBase imageFile, string outPutFilePath, string outPuthFileName, int width, int height, bool pngFormat = false)
        {
            try
            {
                Image image = Image.FromStream(imageFile.InputStream);
                //if (!width.HasValue)
                //{
                //	width = image.Width;
                //}
                //if (!height.HasValue)
                //{
                //	height = new int?(image.Height);
                //}
                KalikoImage kalikoImage = new KalikoImage(image);

                kalikoImage.Resize(width, height);

                //KalikoImage kalikoImage1 = kalikoImage.Scale(new CropScaling(width, height));
                //KalikoImage kalikoImage1 = kalikoImage.Scale(new FitScaling(width.Value, height.Value));
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(string.Concat("~/", outPutFilePath))))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(string.Concat("~/", outPutFilePath)));
                }
                string str = HttpContext.Current.Server.MapPath(string.Concat("~/", Path.Combine(outPutFilePath, outPuthFileName)));
                if (!pngFormat)
                {
                    kalikoImage.SaveJpg(str, 99);
                }
                else
                {
                    kalikoImage.SavePng(str);
                }
                kalikoImage.Dispose();
                //kalikoImage.Dispose();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void CropAndResizeImage(string inPutFilePath, string outPutFilePath, string outPuthFileName, int width, int height, bool pngFormat = false)
        {
            try
            {
                Image image = Image.FromFile(HttpContext.Current.Server.MapPath(string.Concat("~/", inPutFilePath)));
                //if (!width.HasValue)
                //{
                //	width = new int?(image.Width);
                //}
                //if (!height.HasValue)
                //{
                //	height = new int?(image.Height);
                //}
                KalikoImage kalikoImage = new KalikoImage(image);
                KalikoImage kalikoImage1 = kalikoImage.Scale(new FitScaling(width, height));
                //KalikoImage kalikoImage1 = kalikoImage.Scale(new FitScaling(width.Value, height.Value));
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(string.Concat("~/", outPutFilePath))))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(string.Concat("~/", outPutFilePath)));
                }
                string str = HttpContext.Current.Server.MapPath(string.Concat("~/", Path.Combine(outPutFilePath, outPuthFileName)));
                if (!pngFormat)
                {
                    kalikoImage1.SaveJpg(str, 99);
                }
                else
                {
                    kalikoImage1.SavePng(str);
                }
                kalikoImage1.Dispose();
                kalikoImage.Dispose();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void CropAndResizeImage(Image image, string outPutFilePath, string outPuthFileName, int width, int height, bool pngFormat = false)
        {
            try
            {
                //if (!width.HasValue)
                //{
                //	width = new int?(image.Width);
                //}
                //if (!height.HasValue)
                //{
                //	height = new int?(image.Height);
                //}
                KalikoImage kalikoImage = new KalikoImage(image);

                kalikoImage.Resize(width, height);
                //KalikoImage kalikoImage1 = kalikoImage.Scale(new FitScaling(width, height));
                //KalikoImage kalikoImage1 = kalikoImage.Scale(new FitScaling(width.Value, height.Value));
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(string.Concat("~/", outPutFilePath))))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(string.Concat("~/", outPutFilePath)));
                }
                string str = HttpContext.Current.Server.MapPath(string.Concat("~/", Path.Combine(outPutFilePath, outPuthFileName)));
                if (!pngFormat)
                {
                    kalikoImage.SaveJpg(str, 99);
                }
                else
                {
                    kalikoImage.SavePng(str);
                }
                kalikoImage.Dispose();
                kalikoImage.Dispose();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}