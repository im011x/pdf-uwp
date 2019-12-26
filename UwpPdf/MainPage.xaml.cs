using SokeePdfLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace UwpPdf
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task addImageOn(pdfPage page)
        {
            BitmapImage hiImage = (BitmapImage)imageValley.Source;

            RandomAccessStreamReference randomAccess = RandomAccessStreamReference.CreateFromUri(hiImage.UriSour‌​ce);
            using (IRandomAccessStream stream = await randomAccess.OpenReadAsync())
            {
                IRandomAccessStream resizedStream = await SokeeMagic.ImageToScaledStreamAsync(stream, page.width, page.height);
                IRandomAccessStream jpegStream = await SokeeMagic.ImageToJPegStreamAsync(resizedStream);

                page.addImage(jpegStream);
            }
        }

        private async Task manuplateImageAsync(pdfPage page, int x, int y)
        {
            BitmapImage hiImage = (BitmapImage) imageValley.Source;
            RandomAccessStreamReference randomAccess = RandomAccessStreamReference.CreateFromUri(hiImage.UriSour‌​ce);
            using (IRandomAccessStream  stream = await randomAccess.OpenReadAsync())
            {
                //Create a decoder for the image
                var decoder = await BitmapDecoder.CreateAsync(stream);
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync();
                byte[] pixelBytes =          pixelData.DetachPixelData();

                await makeJpegAsync(decoder, pixelBytes, page, hiImage, x, y);
            }
        }

        private async Task makeJpegAsync(BitmapDecoder decoder, byte[] pixelBytes, pdfPage page, BitmapImage bitmapImage, int x, int y)
        {
            //double jpegImageQuality = Constants.ImageAttachStartingImageQuality;
            double jpegImageQuality = 0.9;
            ulong jpegImageSize = 0;

            var imageWriteableStream = new InMemoryRandomAccessStream();
            //MemoryStream memoryStream = new MemoryStream();
            //var imageWriteableStream = memoryStream.AsRandomAccessStream();

            using (imageWriteableStream)
            {
                var propertySet = new BitmapPropertySet();
                var qualityValue = new BitmapTypedValue(jpegImageQuality, Windows.Foundation.PropertyType.Single);
                propertySet.Add("ImageQuality", qualityValue);

                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, imageWriteableStream, propertySet);
                //key thing here is to use decoder.OrientedPixelWidth and decoder.OrientedPixelHeight otherwise you will get garbled image on devices on some photos with orientation in metadata
                encoder.SetPixelData(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, decoder.OrientedPixelWidth, decoder.OrientedPixelHeight, decoder.DpiX, decoder.DpiY, pixelBytes);

                jpegImageSize = imageWriteableStream.Size;

                await encoder.FlushAsync();
                await imageWriteableStream.FlushAsync();

                var byteArray = new byte[imageWriteableStream.Size];
                await imageWriteableStream.ReadAsync(byteArray.AsBuffer(), (uint)imageWriteableStream.Size, InputStreamOptions.None);

                //page.addImage(bitmapImage, byteArray, x, y);
                page.addImage(imageWriteableStream);
            }

        }

        private async void DoTestAsync(object sender, RoutedEventArgs e)
        {
            PdfDocument doc = new PdfDocument();

            pdfPage page = doc.addPage();
            page.addText("Hello PDF 10, 0", 10, 0, SokeePdfLib.Enumerators.predefinedFont.csCourier, 24, SokeePdfLib.Enumerators.predefinedColor.csBlack);
            //await manuplateImageAsync(page, 10, 0);
            await addImageOn(page);

            pdfPage page2 = doc.addPage();
            page2.addText("Hello PDF 10, 100", 10, 100, SokeePdfLib.Enumerators.predefinedFont.csCourier, 24, SokeePdfLib.Enumerators.predefinedColor.csBlack);
            //await manuplateImageAsync(page2, 10, 100);
            await addImageOn(page2);

            pdfPage page3 = doc.addPage();
            page3.addText("Hello PDF 10, 200", 10, 200, SokeePdfLib.Enumerators.predefinedFont.csCourier, 24, SokeePdfLib.Enumerators.predefinedColor.csBlack);
            //await manuplateImageAsync(page3, 10, 200);
            await addImageOn(page3);

            doc.CreatePDFAsync("hello.pdf");

        }
    }
}
