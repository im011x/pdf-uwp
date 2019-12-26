using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokeePdfLib
{
    internal class PdfTrailer : IWritable
    {
        private int _lastObjectID;
        private List<string> _objectOffsets;
        private long _xrefOffset;

        public long xrefOffset
        {
            get
            {
                return _xrefOffset;
            }

            set
            {
                _xrefOffset = value;
            }
        }

        public PdfTrailer(int lastObjectID)
        {
            _lastObjectID = lastObjectID;
            _objectOffsets = new List<string>();
        }

        ~PdfTrailer()
        {
            _objectOffsets = null;
        }

        public void addObject(string offset)
        {
            _objectOffsets.Add(new string('0', 10 - offset.Length) + offset);
        }


        public string getText()
        {
            StringBuilder content = new StringBuilder();
            content.Append("xref" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("0 " + (_lastObjectID + 1).ToString() + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("0000000000 65535 f" + Convert.ToChar(13) + Convert.ToChar(10));
            foreach (object offset in _objectOffsets)
            {
                content.Append(offset.ToString() + " 00000 n" + Convert.ToChar(13) + Convert.ToChar(10));
            }
            content.Append("trailer" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("/Size " + (_lastObjectID + 1).ToString() + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("/Root 1 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("/Info 2 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("startxref" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append(_xrefOffset.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("%%EOF");
            return content.ToString();
        }
    }
}
