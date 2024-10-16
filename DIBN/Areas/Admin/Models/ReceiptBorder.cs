using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace DIBN.Areas.Admin.Models
{
    public class ReceiptBorder : PdfPageEventHelper
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
            //We will have to create separate cells to include image logo and 2 separate strings  
            //Row 1  
            String text = "DIBN BUSINESS SERVICES - DUBAI (UAE)";
            string text1 = "\r\n1901 - Al Fahidi Heights Tower , Near Sharaf DG Metro Station exit-4 , Bur Dubai.";
            string text2 = "For, DIBN BUSINESS SERVICES";
            string text3 = "(Account Department)";
            string text4 = "Telephone: +971 43421947   CELL : +971 522877801 ";
            string text5 = "WEB : www.dibnbusiness.com   E-Mail :info@dibnbusiness.com";
            string text6 = " Authorized Signatory";
            string text7 = "Notes: \r\n";
            string text8 = "1. DIBN Professional fees is not refundable.  \r\n";
            string text9 = "2. DIBN Professional fees + vat 100% advance and will be deducted from the first payment.  \r\n";
            string text10 = "3. All Government charges to be paid as actual by client.  \r\n";
            string text12 = "4. DIBN is not responsible for any increase/changes in the Government charges.   \r\n";
            string text13 = "4. Government charges are subject to change without prior notice.   \r\n";

            //Add paging to footer  
            {
                float len = 0;

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetRight(150), document.PageSize.GetBottom(390));
                content.ShowText(text6);
                content.EndText();
                len = bf.GetWidthPoint(text6, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetRight(150), document.PageSize.GetBottom(390));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetBottom(320));
                content.ShowText(text2);
                content.EndText();
                len = bf.GetWidthPoint(text2, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetRight(200), document.PageSize.GetBottom(320));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetRight(175), document.PageSize.GetBottom(310));
                content.ShowText(text3);
                content.EndText();
                len = bf.GetWidthPoint(text3, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetRight(175), document.PageSize.GetBottom(310));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(25), document.PageSize.GetBottom(250));
                content.ShowText(text7);
                content.EndText();
                len = bf.GetWidthPoint(text7, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(25), document.PageSize.GetBottom(250));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(25), document.PageSize.GetBottom(230));
                content.ShowText(text8);
                content.EndText();
                len = bf.GetWidthPoint(text8, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(25), document.PageSize.GetBottom(230));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(25), document.PageSize.GetBottom(210));
                content.ShowText(text9);
                content.EndText();
                len = bf.GetWidthPoint(text9, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(25), document.PageSize.GetBottom(210));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(25), document.PageSize.GetBottom(190));
                content.ShowText(text10);
                content.EndText();
                len = bf.GetWidthPoint(text10, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(25), document.PageSize.GetBottom(190));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(25), document.PageSize.GetBottom(170));
                content.ShowText(text12);
                content.EndText();
                len = bf.GetWidthPoint(text12, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(25), document.PageSize.GetBottom(170));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(25), document.PageSize.GetBottom(150));
                content.ShowText(text13);
                content.EndText();
                len = bf.GetWidthPoint(text13, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(25), document.PageSize.GetBottom(150));
                // -------------------
                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(180), document.PageSize.GetBottom(75));
                content.ShowText(text);
                content.EndText();
                len = bf.GetWidthPoint(text, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(180), document.PageSize.GetBottom(75));


                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(120), document.PageSize.GetBottom(60));
                content.ShowText(text1);
                content.EndText();
                len = bf.GetWidthPoint(text1, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(120), document.PageSize.GetBottom(60));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(190), document.PageSize.GetBottom(45));
                content.ShowText(text4);
                content.EndText();
                len = bf.GetWidthPoint(text4, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(190), document.PageSize.GetBottom(45));

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(150), document.PageSize.GetBottom(30));
                content.ShowText(text5);
                content.EndText();
                len = bf.GetWidthPoint(text5, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(150), document.PageSize.GetBottom(30));
            }
            
            content.MoveTo(40, document.PageSize.GetBottom(50));

            content.Stroke();
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
            String text11 = "This is Computer Generated Receipt";


            //Add paging to footer  
            {
                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetRight(390), document.PageSize.GetTop(15));
                content.ShowText(text11);
                content.EndText();
                float len = bf.GetWidthPoint(text11, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetRight(390) + len, document.PageSize.GetTop(15));
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
}
