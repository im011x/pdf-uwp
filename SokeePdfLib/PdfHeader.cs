using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokeePdfLib
{
    internal class PdfHeader : IWritable
    {

        private int _objectIDHeader;
        private int _objectIDInfo;
        private int _objectIDOutlines;
        private int _pageTreeID;
        private bool _openBookmark;

        public int objectIDHeader
        {
            get
            {
                return _objectIDHeader;
            }

            set
            {
                _objectIDHeader = value;
            }
        }

        public int objectIDOutlines
        {
            get
            {
                return _objectIDOutlines;
            }

            set
            {
                _objectIDOutlines = value;
            }
        }

        public int objectIDInfo
        {
            get
            {
                return _objectIDInfo;
            }

            set
            {
                _objectIDInfo = value;
            }
        }

        public int pageTreeID
        {
            get
            {
                return _pageTreeID;
            }

            set
            {
                _pageTreeID = value;
            }
        }

        public PdfHeader(bool openBookmark)
        {
            _openBookmark = openBookmark;
        }

        public string getText()
        {
            StringBuilder strHeader = new StringBuilder();
            strHeader.Append(_objectIDHeader.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
            strHeader.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
            strHeader.Append("/Type /Catalog" + Convert.ToChar(13) + Convert.ToChar(10));
            strHeader.Append("/Version /1.4" + Convert.ToChar(13) + Convert.ToChar(10));
            strHeader.Append("/Pages " + _pageTreeID.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
            strHeader.Append("/Outlines " + _objectIDOutlines.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
            if (_openBookmark)
            {
                strHeader.Append("/PageMode /UseOutlines" + Convert.ToChar(13) + Convert.ToChar(10));
            }
            strHeader.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
            strHeader.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
            return strHeader.ToString();
        }
    }
}
