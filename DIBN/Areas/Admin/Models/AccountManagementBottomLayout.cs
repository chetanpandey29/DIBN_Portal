using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using System.IO;
using System;

namespace DIBN.Areas.Admin.Models
{
    public class AccountManagementBottomLayout : PdfPageEventHelper
    {
        string minDate = "",maxDate="";
        public AccountManagementBottomLayout(string MinDate,string MaxDate)
        {
            minDate = MinDate;
            maxDate = MaxDate;
        }
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

            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

            string path = "wwwroot/DIBN_Logo.png";
                //"wwwroot/DIBN_Logo.png";

            FileStream fs1 = new FileStream(path, FileMode.Open);
            iTextSharp.text.Image JPG = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Jpeg);
            JPG.ScalePercent(4f);
            //JPG.PaddingTop = 5f;
            fs1.Close();

            PdfPTable pdfTable = new PdfPTable(2);

            Phrase p1Header = new Phrase("Account Summary \n\n Date : "+minDate+" To " + maxDate+"", baseFontNormal);

            PdfPTable pdfTab = new PdfPTable(1);


            PdfPCell  pdfCell2 = new PdfPCell(JPG);
            pdfCell2.Border = Rectangle.NO_BORDER;
            pdfCell2.PaddingBottom = 8f;
            pdfCell2.VerticalAlignment = PdfPCell.ALIGN_TOP;
            pdfCell2.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            pdfTable.AddCell(pdfCell2);
            pdfTable.DefaultCell.Border = Rectangle.NO_BORDER;
            pdfTable.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;

            pdfCell2 = new PdfPCell(p1Header);
            pdfCell2.Border = Rectangle.NO_BORDER;
            pdfCell2.VerticalAlignment = PdfPCell.ALIGN_TOP;
            pdfCell2.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            pdfTable.AddCell(pdfCell2);
            pdfTable.DefaultCell.Border = Rectangle.NO_BORDER;
            pdfTable.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;



            pdfTab.DefaultCell.Border = Rectangle.NO_BORDER;
            pdfTab.AddCell(pdfTable);
            //We will have to create separate cells to include image logo and 2 separate strings  
            //Row 1  
            String text = "This General Ledger Approved electronic document issued without signature by the DIBN BUSINESS SERVICES.";
            string text1 = "\r\nTo verify the DIBN Account Department.";
            string text2 = "For, DIBN BUSINESS SERVICES.";
            string text3 = "(Account Department)";
            //Add paging to footer  
            {
                float len = 0;

                content.BeginText();
                content.SetFontAndSize(bf, 6);
                content.SetTextMatrix(document.PageSize.GetLeft(115), document.PageSize.GetBottom(15));
                content.ShowText(text);
                content.EndText();
                len = bf.GetWidthPoint(text, 6);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(115), document.PageSize.GetBottom(15));


                content.BeginText();
                content.SetFontAndSize(bf, 6);
                content.SetTextMatrix(document.PageSize.GetLeft(250), document.PageSize.GetBottom(5));
                content.ShowText(text1);
                content.EndText();
                len = bf.GetWidthPoint(text1, 6);
                content.AddTemplate(footerTemplate, document.PageSize.GetLeft(250), document.PageSize.GetBottom(5));
            }
            pdfTab.TotalWidth = document.PageSize.Width - 80f;
            pdfTab.WidthPercentage = 70f;
            pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;

            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write
            //Third and fourth param is x and y position to start writing
            pdfTab.WriteSelectedRows(0, -1, 55, (document.PageSize.Height - 5), writer.DirectContent);
            //Move the pointer and draw line to separate footer section from rest of page  

            content.Stroke();
            path = "wwwroot/DIBN_Logo.png";
                //"wwwroot/DIBN_Logo.png";
            fs1 = new FileStream(path, FileMode.Open);

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
            footerTemplate.SetFontAndSize(bf, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.EndText();
        }
    }
}
