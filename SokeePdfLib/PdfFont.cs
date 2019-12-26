using SokeePdfLib.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokeePdfLib
{
    class PdfFont : IWritable
    {
        private predefinedFont _fontStyle;
        private int _objectID;
        private int _fontNumber;

        public predefinedFont fontStyle
        {
            get
            {
                return _fontStyle;
            }
        }

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

        public PdfFont(predefinedFont newFontStyle, int newFontNumber)
        {
            _fontStyle = newFontStyle;
            _fontNumber = newFontNumber;
        }

        public string getText()
        {
            StringBuilder content = new StringBuilder();
            content.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("/Type /Font" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("/Subtype /Type1" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("/Name /F" + _fontNumber.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("/BaseFont /" + PdfFont.getFontName(_fontStyle) + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("/Encoding /WinAnsiEncoding" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
            content.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
            return content.ToString();
        }

        public static string getFontName(predefinedFont fontType)
        {
            switch (fontType)
            {
                case predefinedFont.csHelvetica:
                    return "Helvetica";
                case predefinedFont.csHelveticaBold:
                    return "Helvetica-Bold";
                case predefinedFont.csHelveticaOblique:
                    return "Helvetica-Oblique";
                case predefinedFont.csHelvetivaBoldOblique:
                    return "Helvetica-BoldOblique";
                case predefinedFont.csCourier:
                    return "Courier";
                case predefinedFont.csCourierBold:
                    return "Courier-Bold";
                case predefinedFont.csCourierOblique:
                    return "Courier-Oblique";
                case predefinedFont.csCourierBoldOblique:
                    return "Courier-BoldOblique";
                case predefinedFont.csTimes:
                    return "Times-Roman";
                case predefinedFont.csTimesBold:
                    return "Times-Bold";
                case predefinedFont.csTimesOblique:
                    return "Times-Italic";
                case predefinedFont.csTimesBoldOblique:
                    return "Times-BoldItalic";
                default:
                    return "";
            }
        }


    }
}
