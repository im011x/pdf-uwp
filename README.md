# Introduction
pdf writer for uwp 


The name is SokeePdfLib.


With this you can create PDF document in UWP (C#) environment.  

# Background
pdf writer for uwp 

A few years ago, I've tried to find PDF Writing Open Source. But I've failed to find it. 
So I've made it by myself and it is uploading to github now. 
PDFShart or SharpPDF was refered to make my own pdf library (SokeePdfLib).

## The Status 
It's not perfect. 
But it's simple to expand to meet your requirement. 

## PDF format sample 
refer HowWrite/MainPage.xaml.cs 

PDF is just a format. The following shows how to write a file with PDF format.

### 1. Create a file first
<pre><code>
StorageFile file = await DownloadsFolder.CreateFileAsync("sample.pdf", CreationCollisionOption.GenerateUniqueName);
</code></pre>

With this code, "sample.pdf" will be created at Download folder of yours. 

### 2. write a text with pdf format
<pre><code>
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
</code></pre>

You can find the pdf in the "sample.pdf" file. 

# How to use SokeePdfLib
refer UwpPdf/MainPage.xaml.cs 

SokeePdfLib is a library to write PDF file. 

## refer the following code 
<pre><code>
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
</code></pre>

You can find the pdf in the "hello.pdf" file in your Download folder. 

