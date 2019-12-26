using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace SokeePdfLib
{
    public class SokeeMagic
    {
        public static async Task<IRandomAccessStream> ImageToJPegStreamAsync(IRandomAccessStream imageStream)
        {
            //Create a decoder for the image
            var decoder = await BitmapDecoder.CreateAsync(imageStream);
            PixelDataProvider pixelData = await decoder.GetPixelDataAsync();
            byte[] pixelBytes = pixelData.DetachPixelData();

            //
            InMemoryRandomAccessStream jpegStream = new InMemoryRandomAccessStream();

            double jpegImageQuality = 0.9;

            var propertySet = new BitmapPropertySet();
            var qualityValue = new BitmapTypedValue(jpegImageQuality, Windows.Foundation.PropertyType.Single);
            propertySet.Add("ImageQuality", qualityValue);

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, jpegStream, propertySet);
            //key thing here is to use decoder.OrientedPixelWidth and decoder.OrientedPixelHeight otherwise you will get garbled image on devices on some photos with orientation in metadata
            encoder.SetPixelData(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, decoder.OrientedPixelWidth, decoder.OrientedPixelHeight, decoder.DpiX, decoder.DpiY, pixelBytes);

            //ulong jpegImageSize = 0;
            //jpegImageSize = jpegStream.Size;

            await encoder.FlushAsync();
            await jpegStream.FlushAsync();

            return jpegStream;
        }

        public static async Task<IRandomAccessStream> ImageToScaledStreamAsync(IRandomAccessStream imageStream, int fitWidth, int fitHeight)
        {
            var decoder = await BitmapDecoder.CreateAsync(imageStream);
            var originalPixelWidth = decoder.PixelWidth;
            var originalPixelHeight = decoder.PixelHeight;

            InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);

            double ratioWidth = (double)fitWidth / originalPixelWidth;
            double ratioHeight = (double)fitHeight / originalPixelHeight;
            double ratio = Math.Min(ratioWidth, ratioHeight);

            uint aspectWidth = (uint)((double)originalPixelWidth * ratio) - 60;
            uint aspectHeight = (uint)((double)originalPixelHeight * ratio) - 60;

            uint cropX = (uint)(fitWidth - aspectWidth) / 2;
            uint cropY = (uint)(fitHeight - aspectHeight) / 2;

            cropX = 0;
            cropY = 0;

            //you can adjust interpolation and other options here, so far linear is fine for thumbnails
            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
            encoder.BitmapTransform.ScaledWidth = aspectWidth;
            encoder.BitmapTransform.ScaledHeight = aspectHeight;
            encoder.BitmapTransform.Bounds = new BitmapBounds()
            {
                Width = (uint)aspectWidth,
                Height = (uint)aspectHeight,
                X = cropX,
                Y = cropY,
            };
            await encoder.FlushAsync();
            await resizedStream.FlushAsync();

            return resizedStream;
        }

    }
}
