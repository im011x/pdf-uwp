using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokeePdfLib
{
    internal class PdfInfo : IWritable
    {
        private int _objectIDInfo;
        private string _title;
        private string _author;


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

        public PdfInfo(string title, string author)
        {
            _title = title;
            _author = author;
        }

        public string getText()
        {
            StringBuilder strInfo = new StringBuilder();
            strInfo.Append(_objectIDInfo.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
            strInfo.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
            strInfo.Append("/Title (" + _title + ")" + Convert.ToChar(13) + Convert.ToChar(10));
            strInfo.Append("/Author (" + _author + ")" + Convert.ToChar(13) + Convert.ToChar(10));
            strInfo.Append("/Creator (Sokee PDF)" + Convert.ToChar(13) + Convert.ToChar(10));
            strInfo.Append("/CreationDate (" + DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Day.ToString() + ")" + Convert.ToChar(13) + Convert.ToChar(10));
            strInfo.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
            strInfo.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
            return strInfo.ToString();
        }
    }
}
