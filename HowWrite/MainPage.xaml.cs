using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace HowWrite
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

        private async void DoClick(object sender, RoutedEventArgs e)
        {
            //

            StorageFile file = await DownloadsFolder.CreateFileAsync("simple.pdf", CreationCollisionOption.GenerateUniqueName);

#if JUSTFILE
           await WriteToAsync(file, "hello");
            await WriteToAsync(file, "" + Convert.ToChar(199) + Convert.ToChar(236) + Convert.ToChar(143) + Convert.ToChar(162));
            await WriteToAsync(file, "line2");
            await WriteToAsync(file, "line3");
            await WriteToAsync(file, "line4");
            await WriteToAsync(file, "line5");
#endif

#if BLANK
            await WriteToAsync(file, "%PDF-1.6");
            await WriteToAsync(file, "1 0 obj <</Type /Catalog /Pages 2 0 R>>");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "2 0 obj <</Type /Pages /Kids [3 0 R] /Count 1>>");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "3 0 obj<> /MediaBox [0 0 500 800]>>");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "xref");
            await WriteToAsync(file, "0 4");
            await WriteToAsync(file, "0000000000 65535 f");
            await WriteToAsync(file, "0000000010 00000 n");
            await WriteToAsync(file, "0000000060 00000 n");
            await WriteToAsync(file, "0000000115 00000 n");
            await WriteToAsync(file, "trailer <</Size 4/Root 1 0 R>>");
            await WriteToAsync(file, "startxref");
            await WriteToAsync(file, "199");
            await WriteToAsync(file, "%%EOF");
#endif

            await WriteToAsync(file, "%PDF-1.4");
            await WriteToAsync(file, "1 0 obj <</Type /Catalog /Pages 2 0 R>>");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "2 0 obj <</Type /Pages /Kids [3 0 R] /Count 1>>");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 500 800] /Contents 6 0 R>>");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "4 0 obj<</Font <</F1 5 0 R>>>>");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "5 0 obj<</Type /Font /Subtype /Type1 /BaseFont /Helvetica>>");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "6 0 obj");
            await WriteToAsync(file, "<</Length 44>>");
            await WriteToAsync(file, "stream");
            await WriteToAsync(file, "BT /F1 24 Tf 175 720 Td (Hello World!)Tj ET");
            await WriteToAsync(file, "endstream");
            await WriteToAsync(file, "endobj");
            await WriteToAsync(file, "xref");
            await WriteToAsync(file, "0 7");
            await WriteToAsync(file, "0000000000 65535 f");
            await WriteToAsync(file, "0000000009 00000 n");
            await WriteToAsync(file, "0000000056 00000 n");
            await WriteToAsync(file, "0000000111 00000 n");
            await WriteToAsync(file, "0000000212 00000 n");
            await WriteToAsync(file, "0000000250 00000 n");
            await WriteToAsync(file, "0000000317 00000 n");
            await WriteToAsync(file, "trailer <</Size 7/Root 1 0 R>>");
            await WriteToAsync(file, "startxref");
            await WriteToAsync(file, "406");
            await WriteToAsync(file, "%%EOF");



        }

        private async System.Threading.Tasks.Task WriteToAsync(StorageFile file, string content)
        {
            //Byte[] byteArray = Encoding.UTF8.GetBytes(content);
            string contentLine = content + Convert.ToChar(13) + Convert.ToChar(10);
            await FileIO.AppendTextAsync(file, contentLine);
        }

    }
}
