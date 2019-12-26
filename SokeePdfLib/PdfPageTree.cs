using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokeePdfLib
{
    class PdfPageTree : IWritable
    {
        private List<int> _pages;
        private int _pageCount;
        private int _objectID;

        public int objectID
        {

            get
            {
                return _objectID;
            }

            set
            {
                _objectID = value;
            }

        }

        public PdfPageTree()
        {
            _pageCount = 0;
            _pages = new List<int>();
        }

        public void addPage(int pageID)
        {
            _pages.Add(pageID);
            _pageCount++;
        }


        public string getText()
        {
            if (_pages.Count > 0)
            {
                StringBuilder content = new StringBuilder();
                content.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
                content.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
                content.Append("/Type /Pages" + Convert.ToChar(13) + Convert.ToChar(10));
                content.Append("/Count " + _pages.Count.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
                content.Append("/Kids [");
                foreach (int item in _pages)
                {
                    content.Append(item.ToString() + " 0 R ");
                }
                content.Append("]" + Convert.ToChar(13) + Convert.ToChar(10));
                content.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
                content.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
                return content.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
