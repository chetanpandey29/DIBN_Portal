using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace DIBN.Models
{
    public class Border: PdfPageEventHelper
    {

        // This is the contentbyte object of the writer  
        PdfContentByte content = null;

        // we will put the final number of pages in a template  
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer  
        BaseFont bf = null;

        // This keeps track of the creation time  
        DateTime PrintTime = DateTime.Now;

        #region Fields  
        private string _header;
        #endregion

        #region Properties  
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                content = writer.DirectContent;
                footerTemplate = content.CreateTemplate(100, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);
            content = writer.DirectContent;
            Rectangle rectangle = new Rectangle(document.PageSize);
            rectangle.Left += document.LeftMargin;
            rectangle.Right -= document.RightMargin;
            rectangle.Top -= document.TopMargin;
            rectangle.Bottom += (document.BottomMargin);
            content.SetColorStroke(BaseColor.BLACK);
            content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
            //content.SetTextMatrix(document.LeftMargin, document.BottomMargin);

            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
        

            //We will have to create separate cells to include image logo and 2 separate strings  
            //Row 1  
            String text = "This is Computer Generated Invoice";

            String text1 = "Page No.";
            int _pageCount = 0;
            _pageCount = document.PageNumber;
            //Add paging to footer  
            {
                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetRight(390), document.PageSize.GetBottom(10));
                content.ShowText(text);
                content.EndText();
                float len = bf.GetWidthPoint(text, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetRight(390) + len, document.PageSize.GetBottom(10));
                if (_pageCount > 1)
                {
                    content.BeginText();
                    content.SetFontAndSize(bf, 10);
                    content.SetTextMatrix(document.PageSize.GetRight(100), document.PageSize.GetBottom(10));
                    content.ShowText(text1 + " " + writer.CurrentPageNumber.ToString());
                    content.EndText();
                    len = bf.GetWidthPoint(text1 + " " + writer.CurrentPageNumber.ToString(), 10);
                    content.AddTemplate(footerTemplate, document.PageSize.GetRight(100) + len, document.PageSize.GetBottom(10));
                }
            }

            //Move the pointer and draw line to separate footer section from rest of page  
            content.MoveTo(40, document.PageSize.GetBottom(50));
          
            content.Stroke();
            string path = "wwwroot/DIBN_Logo.png";
            FileStream fs1 = new FileStream(path, FileMode.Open);

            PdfContentByte under = writer.DirectContentUnder;
            Image image = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), System.Drawing.Imaging.ImageFormat.Jpeg);
            PdfGState state = new PdfGState();
            state.FillOpacity = 0.1f;
            under.SetGState(state);
            image.RotationDegrees = 40;
            image.ScaleToFit(275f, 275f);
            image.SetAbsolutePosition(150, 300);
            under.AddImage(image);
            fs1.Close();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);


            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.EndText();
        }
    }
    public class DIBNBorder : PdfPageEventHelper
    {

        // This is the contentbyte object of the writer  
        PdfContentByte content = null;

        // we will put the final number of pages in a template  
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer  
        BaseFont bf = null;

        // This keeps track of the creation time  
        DateTime PrintTime = DateTime.Now;

        #region Fields  
        private string _header;
        #endregion

        #region Properties  
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                content = writer.DirectContent;
                footerTemplate = content.CreateTemplate(100, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            float width = document.PageSize.Width - 250;
            float height = document.PageSize.Height - 250;
            base.OnEndPage(writer, document);
            content = writer.DirectContent;
            Rectangle rectangle = new Rectangle(document.PageSize);
            rectangle.Left += document.LeftMargin;
            rectangle.Right -= document.RightMargin;
            rectangle.Top -= document.TopMargin;
            rectangle.Bottom += (document.BottomMargin);
            content.SetColorStroke(BaseColor.BLACK);
            content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);

            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);


            //We will have to create separate cells to include image logo and 2 separate strings  
            //Row 1  
            String text = "This is Computer Generated Invoice";
            String text1 = "Page No.";
            int _pageCount = 0;
            _pageCount = document.PageNumber;

            //Add paging to footer  
            {
                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetRight(390), document.PageSize.GetBottom(10));
                content.ShowText(text);
                content.EndText();
                float len = bf.GetWidthPoint(text, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetRight(390) + len, document.PageSize.GetBottom(10));

                if (_pageCount > 1)
                {
                    content.BeginText();
                    content.SetFontAndSize(bf, 10);
                    content.SetTextMatrix(document.PageSize.GetRight(100), document.PageSize.GetBottom(10));
                    content.ShowText(text1 + " " + writer.CurrentPageNumber.ToString());
                    content.EndText();
                    len = bf.GetWidthPoint(text1 + " " + writer.CurrentPageNumber.ToString(), 10);
                    content.AddTemplate(footerTemplate, document.PageSize.GetRight(100) + len, document.PageSize.GetBottom(10));
                }
            }

            //Move the pointer and draw line to separate footer section from rest of page  
            content.MoveTo(40, document.PageSize.GetBottom(50));

            content.Stroke();
            string path = "wwwroot/PDF/DIBN_Logo.png";
            FileStream fs1 = new FileStream(path, FileMode.Open);

            PdfContentByte under = writer.DirectContentUnder;
            Image image = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Jpeg);
            PdfGState state = new PdfGState();
            state.FillOpacity = 0.1f;
            under.SetGState(state);
            image.RotationDegrees = 40;
            image.ScaleToFit(275f, 275f);
            image.SetAbsolutePosition(150, 300);
            under.AddImage(image);
            fs1.Close();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.EndText();
        }
    }
}
