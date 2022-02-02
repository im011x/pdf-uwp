using SokeePdfLib.Enumerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SokeePdfLib
{
    public class PdfDocument
    {
        private PdfTrailer _trailer;
        private PdfHeader _header;
        private PdfInfo _info;
        private PdfPageTree _pageTree;

        private pdfOutlines _outlines = new pdfOutlines();
        private List<PdfFont> _fonts = new List<PdfFont>();
        private List<pdfPage> _pages = new List<pdfPage>();

        private string _title;
        private string _author;
        private bool _openBookmark;

        private ulong _bufferLength;

        public PdfDocument()
        {
            _title = "SokeeViewer";
            _author = "Sokee";
            _openBookmark = false;
        }

        public PdfDocument(string title, string author)
        {
            _title = title;
            _author = author;
            _openBookmark = false;
        }

        public PdfDocument(string title, string author, bool openBookmark)
        {
            _title = title;
            _author = author;
            _openBookmark = openBookmark;
        }

        ~PdfDocument()
        {
            _header = null;
            _info = null;
            _fonts = null;
            _pages = null;
            _pageTree = null;
            _trailer = null;
            _title = null;
            _author = null;
            
            _outlines = null;

            // _pageMarker = null;
            //_persistentPage = null;
        }

        private void Initialize()
        {
            _bufferLength = 0;

            //Page's counters
            int pageIndex = 1;
            int pageNum = _pages.Count;

            int counterID = 0;
            //header			
            _header = new PdfHeader(_openBookmark)
            {
                objectIDHeader = 1,
                objectIDInfo = 2,
                objectIDOutlines = 3
            };
            //Info
            _info = new PdfInfo(_title, _author);
            _info.objectIDInfo = 2;

            //Outlines	
            _outlines.objectIDOutlines = 3;

            counterID = 4;
            //Bookmarks	
            counterID = _outlines.initializeOutlines(counterID);

            //fonts
            for (int i = 0; i < 12; i++)
            {
                _fonts.Add(new PdfFont((predefinedFont)(i + 1), i + 1));
                ((PdfFont)_fonts[i]).objectID = counterID;
                counterID++;
            }

            //pagetree
            _pageTree = new PdfPageTree();
            _pageTree.objectID = counterID;
            _header.pageTreeID = counterID;
            counterID++;
            //pages
            foreach (pdfPage page in _pages)
            {
                page.objectID = counterID;
                page.pageTreeID = _pageTree.objectID;
                page.addFonts(_fonts);
                _pageTree.addPage(counterID);
                counterID++;

#if SKIP 
                //Add page's Marker
                if (_pageMarker != null)
                {
                    page.addText(_pageMarker.getMarker(pageIndex, pageNum), _pageMarker.coordX, _pageMarker.coordY, _pageMarker.fontType, _pageMarker.fontSize, _pageMarker.fontColor);
                }

                //Add persistent elements
                if (_persistentPage != null)
                {
                    page.elements.AddRange(_persistentPage.persistentElements);
                }
#endif

                //page's elements
                foreach (pdfElement element in page.elements)
                {
                    element.objectID = counterID;
                    counterID++;
                    //Imageobject
                    if (element.GetType().Name == "imageElement")
                    {
                        ((imageElement)element).xObjectID = counterID;
                        counterID++;
                    }
                }

                //Update page's index counter
                pageIndex += 1;
            }

            //trailer
            _trailer = new PdfTrailer(counterID - 1);
        }

        public async Task CreatePDFAsync(string filename)
        {
            StorageFile storage_file = await DownloadsFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            Debug.WriteLine("Creating PDF file - " + storage_file.DisplayName);

            await writePDFAsync(storage_file);
        }
        /// <summary>
        /// Write PDF document in the given file
        /// </summary>
        /// <param name="dest"></param>
        public async Task CreatePDFAsync(StorageFile dest)
        {
            await writePDFAsync(dest);
        }

        private async Task writePDFAsync(StorageFile storageFile)
        {
            Initialize();

            MemoryStream memoryStream = new MemoryStream();

            //PDF's definition
            _bufferLength += WriteToMemory(memoryStream, @"%PDF-1.4" + Convert.ToChar(13) + Convert.ToChar(10), false);

            //PDF's header object
            _trailer.addObject(_bufferLength.ToString());
            _bufferLength += WriteToMemory(memoryStream, _header.getText(), true);

            //PDF's info object
            _trailer.addObject(_bufferLength.ToString());
            _bufferLength += WriteToMemory(memoryStream, _info.getText(), true);

            //PDF's outlines object
            _trailer.addObject(_bufferLength.ToString());
            _bufferLength += WriteToMemory(memoryStream, _outlines.getText(), true);

            /**
            //PDF's bookmarks
            foreach (pdfBookmarkNode Node in _outlines.getBookmarks())
            {
                _trailer.addObject(_bufferLength.ToString());
                _bufferLength += WriteToMemory(memoryStream, Node.getText());
            }
            **/

            //Fonts's initialization
            foreach (PdfFont font in _fonts)
            {
                _trailer.addObject(_bufferLength.ToString());
                _bufferLength += WriteToMemory(memoryStream, font.getText(), true);
            }

            //PDF's pagetree object
            _trailer.addObject(_bufferLength.ToString());
            _bufferLength += WriteToMemory(memoryStream, _pageTree.getText(), true);

            //Generation of PDF's pages
            foreach (pdfPage page in _pages)
            {
                _trailer.addObject(_bufferLength.ToString());
                _bufferLength += WriteToMemory(memoryStream, page.getText(), true);

                foreach (pdfElement element in page.elements)
                {
                    if (element is imageElement)
                    {
                        _trailer.addObject(_bufferLength.ToString());
                        _bufferLength += WriteToMemory(memoryStream, element.getText(), true);
                        _trailer.addObject(_bufferLength.ToString());
                        _bufferLength += WriteToMemory(memoryStream, ((imageElement)element).getXObjectText(), true);

                        _bufferLength += WriteToMemory(memoryStream, "stream" + Convert.ToChar(13) + Convert.ToChar(10), true);

                        _bufferLength += WriteBytesToBuffer(memoryStream, ((imageElement)element).content, true);

                        _bufferLength += WriteToMemory(memoryStream, Convert.ToChar(13).ToString(), true);
                        _bufferLength += WriteToMemory(memoryStream, Convert.ToChar(10).ToString(), true);
                        _bufferLength += WriteToMemory(memoryStream, "endstream" + Convert.ToChar(13) + Convert.ToChar(10), true);
                        _bufferLength += WriteToMemory(memoryStream, "endobj" + Convert.ToChar(13) + Convert.ToChar(10), true);
                    }
                    else
                    {
                        _trailer.addObject(_bufferLength.ToString());
                        _bufferLength += WriteToMemory(memoryStream, element.getText(), true);
                    }

                }

            }
            //PDF's trailer object
            _trailer.xrefOffset = (long)_bufferLength;
            _bufferLength += WriteToMemory(memoryStream, _trailer.getText(), true);

            ///////////////////////////////////////////////////////////////////////////////////////
            //
            await WriteToFileAsync(storageFile, memoryStream);
            //
            ///////////////////////////////////////////////////////////////////////////////////////

            memoryStream.Dispose();
        }

        private async Task WriteToFileAsync(StorageFile file, MemoryStream memoryStream)
        {
            await FileIO.WriteBytesAsync(file, memoryStream.GetBuffer());
        }

        private ulong WriteBytesToBuffer(MemoryStream memoryStream, byte[] content, bool addTrailer)
        {
            if (memoryStream != null)
            {
                memoryStream.WriteAsync(content, 0, content.Length);
                return (ulong)content.Length;
            }

            /**
            if (addTrailer == true)
            {
                _trailer.addObject(_bufferLength.ToString());
            }

            _bufferLength += (ulong)content.Length;
            **/

            return (ulong) 0;
        }

        private ulong WriteToMemory(MemoryStream memoryStream, string content, bool addTrailer)
        {
            Byte[] byteArray = Encoding.UTF8.GetBytes(content);
            return WriteBytesToBuffer(memoryStream, byteArray, addTrailer);
        }

        /// <summary>
        /// Method that creates a new page
        /// </summary>
        /// <returns>New PDF's page</returns>
        public pdfPage addPage()
        {
            _pages.Add(new pdfPage());
            return (pdfPage)_pages[_pages.Count - 1];
        }

        /// <summary>
        /// Method that creates a new page
        /// </summary>
        /// <returns>New PDF's page</returns>
        /// <param name="height">Height of the new page</param>
        /// <param name="width">Width of the new page</param>
        public pdfPage addPage(int height, int width)
        {
            _pages.Add(new pdfPage(height, width));
            return (pdfPage)_pages[_pages.Count - 1];
        }

        /***
        private async Task writeToStreamAsync(StorageFile file, string content, bool addTrailer)
        {
            //IBuffer buffer = GetBufferFromString(content);
            //await FileIO.WriteBufferAsync(file, buffer);

            Byte[] byteArray = Encoding.UTF8.GetBytes(content);
            await FileIO.WriteBytesAsync(file, byteArray);

            if (addTrailer == true)
            {
                _trailer.addObject(_bufferLength.ToString());
            }
            }


        private IBuffer GetBufferFromString(String str)
        {
            using (InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream())
            {
                using (DataWriter dataWriter = new DataWriter(memoryStream))
                {
                    dataWriter.WriteString(str);
                    return dataWriter.DetachBuffer();
                }
            }
        }
        ***/

        /***
        private async void writeToStream(DataWriter dataWriter, string content, bool addTrailer)
        {
            //dataWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            //dataWriter.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

            dataWriter.WriteString(content);
            _bufferLength += await dataWriter.StoreAsync(); // reset stream size to override the file
            //await ostream.FlushAsync();

            if (addTrailer == true)
            {
                _trailer.addObject(_bufferLength.ToString());
            }

        }
        ***/

        /***
        private async Task writeToStream(IOutputStream ostream, string content, bool addTrailer)
        {
            using (DataWriter dataWriter = new DataWriter(ostream))
            {
                //dataWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                //dataWriter.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

                dataWriter.WriteString(content);
                _bufferLength += await dataWriter.StoreAsync(); // reset stream size to override the file
                await ostream.FlushAsync();

            }

            if (addTrailer == true)
            {
                _trailer.addObject(_bufferLength.ToString());
            }
        }
        ***/

#if OBSOLETED
        private async System.Threading.Tasks.Task WriteToAsync(StorageFile file, string content, bool addTrailer)
        {
            Byte[] byteArray = Encoding.UTF8.GetBytes(content);
            //WriteToAsync(byteArray);

            //Byte[] byteArray = Encoding.UTF8.GetBytes(content);
            //string contentLine = content + Convert.ToChar(13) + Convert.ToChar(10);
            await FileIO.AppendTextAsync(file, content);

            if (addTrailer == true)
            {
                _trailer.addObject(_bufferLength.ToString());
            }

            _bufferLength += (ulong)content.Length;
        }

        /**
        private async System.Threading.Tasks.Task WriteToAsync(StorageFile file, byte[] bytes, bool addTrailer)
        {
            //Byte[] byteArray = Encoding.UTF8.GetBytes(content);
            //string contentLine = content + Convert.ToChar(13) + Convert.ToChar(10);
            await FileIO.AppendTextAsync(file, bytes);

            if (addTrailer == true)
            {
                _trailer.addObject(_bufferLength.ToString());
            }

            _bufferLength += (ulong)bytes.Length;
        }
        **/
#endif


    }
}
