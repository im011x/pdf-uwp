using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace UwpPdf
{
    class ImageUtils
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        // from https://gist.github.com/alexsorokoletov/56a120c5562344d60e1a6b3fa75bda2c
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Resizes and crops source file image so that resized image width/height are not larger than <param name="requestedMinSide"></param>
        /// </summary>
        /// <param name="sourceFile">Source StorageFile</param>
        /// <param name="requestedMinSide">Width/Height of the output image</param>
        /// <param name="resizedImageFile">Target StorageFile</param>
        /// <returns></returns>

        private async Task<IStorageFile> CreateThumbnaiImage(StorageFile sourceFile, int requestedMinSide, StorageFile resizedImageFile)
        {
            var imageStream = await sourceFile.OpenReadAsync();
            var decoder = await BitmapDecoder.CreateAsync(imageStream);
            var originalPixelWidth = decoder.PixelWidth;
            var originalPixelHeight = decoder.PixelHeight;
            using (imageStream)
            {
                //do resize only if needed
                if (originalPixelHeight > requestedMinSide && originalPixelWidth > requestedMinSide)
                {
                    using (var resizedStream = await resizedImageFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        //create encoder based on decoder of the source file
                        var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                        double widthRatio = (double)requestedMinSide / originalPixelWidth;
                        double heightRatio = (double)requestedMinSide / originalPixelHeight;
                        uint aspectHeight = (uint)requestedMinSide;
                        uint aspectWidth = (uint)requestedMinSide;
                        uint cropX = 0, cropY = 0;
                        var scaledSize = (uint)requestedMinSide;

                        if (originalPixelWidth > originalPixelHeight)
                        {
                            aspectWidth = (uint)(heightRatio * originalPixelWidth);
                            cropX = (aspectWidth - aspectHeight) / 2;
                        }
                        else
                        {
                            aspectHeight = (uint)(widthRatio * originalPixelHeight);
                            cropY = (aspectHeight - aspectWidth) / 2;
                        }

                        //you can adjust interpolation and other options here, so far linear is fine for thumbnails
                        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                        encoder.BitmapTransform.ScaledHeight = aspectHeight;
                        encoder.BitmapTransform.ScaledWidth = aspectWidth;
                        encoder.BitmapTransform.Bounds = new BitmapBounds()
                        {
                            Width = scaledSize,
                            Height = scaledSize,
                            X = cropX,
                            Y = cropY,
                        };
                        await encoder.FlushAsync();
                    }
                }
                else
                {
                    //otherwise just use source file as thumbnail
                    await sourceFile.CopyAndReplaceAsync(resizedImageFile);
                }
            }
            return resizedImageFile;
        }


        /// <summary>
        /// Converts source image file to jpeg of defined quality (0.85)
        /// </summary>
        /// <param name="sourceFile">Source StorageFile</param>
        /// <param name="outputFile">Target StorageFile</param>
        /// <returns></returns>
        private async Task<StorageFile> ConvertImageToJpegAsync(StorageFile sourceFile, StorageFile outputFile)
        {
            //you can use WinRTXamlToolkit StorageItemExtensions.GetSizeAsync to get file size (if you already plugged this nuget in)
            var sourceFileProperties = await sourceFile.GetBasicPropertiesAsync();
            var fileSize = sourceFileProperties.Size;
            var imageStream = await sourceFile.OpenReadAsync();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (imageStream)
            {
                var decoder = await BitmapDecoder.CreateAsync(imageStream);
                var pixelData = await decoder.GetPixelDataAsync();
                var detachedPixelData = pixelData.DetachPixelData();
                pixelData = null;
                //0.85d

                //double jpegImageQuality = Constants.ImageAttachStartingImageQuality;
                double jpegImageQuality = 0.9;

                //since we're using MvvmCross, we're outputing diagnostic info to MvxTrace, you can use System.Diagnostics.Debug.WriteLine instead
                //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Source image size: {fileSize}, trying Q={jpegImageQuality}");

                var imageWriteableStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite);
                ulong jpegImageSize = 0;
                using (imageWriteableStream)
                {
                    var propertySet = new BitmapPropertySet();
                    var qualityValue = new BitmapTypedValue(jpegImageQuality, Windows.Foundation.PropertyType.Single);
                    propertySet.Add("ImageQuality", qualityValue);
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, imageWriteableStream, propertySet);
                    //key thing here is to use decoder.OrientedPixelWidth and decoder.OrientedPixelHeight otherwise you will get garbled image on devices on some photos with orientation in metadata
                    encoder.SetPixelData(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, decoder.OrientedPixelWidth, decoder.OrientedPixelHeight, decoder.DpiX, decoder.DpiY, detachedPixelData);
                    await encoder.FlushAsync();
                    await imageWriteableStream.FlushAsync();
                    jpegImageSize = imageWriteableStream.Size;
                }
                //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Final image size now: {jpegImageSize}");
            }
            stopwatch.Stop();
            //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Time spent optimizing image: {stopwatch.Elapsed}");
            return outputFile;
        }

    }
}
