using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace DIBN.Areas.Admin.Models
{
    public class BottomLayout : PdfPageEventHelper
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
                headerTemplate = content.CreateTemplate(100, 100);
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
            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

            Phrase p1Header = new Phrase("Account Summary", baseFontNormal);

            PdfPTable pdfTab = new PdfPTable(1);
            PdfPCell pdfCell2 = new PdfPCell(p1Header);
            pdfCell2.Border = Rectangle.NO_BORDER;
            pdfCell2.PaddingBottom = 15f;
            pdfCell2.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            pdfTab.AddCell(pdfCell2);
            pdfTab.DefaultCell.Border = Rectangle.NO_BORDER;
            pdfTab.DefaultCell.PaddingBottom = 30f;
            String text = "This General Ledger Approved electronic document issued without signature by the DIBN BUSINESS SERVICES.";
            string text1 = "\r\nTo verify the DIBN Account Department.";
            {
                float len = 0;

                content.BeginText();
                content.SetFontAndSize(bf, 9);
                content.SetTextMatrix(document.PageSize.GetLeft(25), document.PageSize.GetBottom(25));
                content.ShowText(text);
                content.EndText();
                len = bf.GetWidthPoint(text, 9);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(25), document.PageSize.GetBottom(25));


                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetLeft(170), document.PageSize.GetBottom(10));
                content.ShowText(text1);
                content.EndText();
                len = bf.GetWidthPoint(text1, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(170), document.PageSize.GetBottom(10));
            }
            pdfTab.TotalWidth = document.PageSize.Width - 80f;
            pdfTab.WidthPercentage = 70f;
            pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;

            pdfTab.WriteSelectedRows(0, -1, 55, (document.PageSize.Height - 5), writer.DirectContent);

            content.Stroke();

            string path = "wwwroot/DIBN_Logo.png";
            FileStream fs1 = new FileStream(path, FileMode.Open);

            PdfContentByte under = writer.DirectContentUnder;
            Image image = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Jpeg);
            PdfGState state = new PdfGState();
            state.FillOpacity = 0.2f;
            under.SetGState(state);
            image.RotationDegrees = 35;
            image.ScaleToFit(275f, 275f);
            image.SetAbsolutePosition(150, 300);
            under.AddImage(image);
            fs1.Close();
            content.Stroke();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 8);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.EndText();
        }
    }
}
