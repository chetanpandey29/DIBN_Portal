using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using System.IO;
using System;

namespace DIBN.Areas.Admin.Models
{
    public class ProfitLossReportMonthlyPdfLayout : PdfPageEventHelper
    {
        string month = "", year = "", totalCredit = "", totalDebit = "";
        double totalProfitLoss = 0, profitLossPercentage = 0, totalCredits = 0, totalDebits = 0;
        public ProfitLossReportMonthlyPdfLayout(string Month, string Year, string TotalCredit, string TotalDebit,
            double totalProfitLoss, double profitLossPercentage, double totalCredits, double totalDebits)
        {
            month = Month;
            year = Year;
            totalCredit = TotalCredit;
            totalDebit = TotalDebit;
            this.totalCredits = totalCredits;
            this.totalDebits = totalDebits;
            this.totalProfitLoss = totalProfitLoss;
            this.profitLossPercentage = profitLossPercentage;
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
            content = writer.DirectContent;

            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            Font debitFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED);
            Font creditFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 128, 0));

            string path = "wwwroot/DIBN_Logo.png";

            FileStream fs1 = new FileStream(path, FileMode.Open);
            iTextSharp.text.Image JPG = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Jpeg);
            JPG.ScalePercent(4.5f);
            //JPG.PaddingTop = 5f;
            fs1.Close();

            Phrase data = new Phrase();

            Phrase p1Header = new Phrase("Profit & Loss Monthly Report of " + month + " , " + year + "", baseFontNormal);
            Phrase p2Header = new Phrase("Total Credited Amount : " + totalCredit + " AED", creditFont);
            data.Add(p2Header);
            Phrase p3Header = new Phrase(",Total Debited Amount : " + totalDebit + " AED", debitFont);
            data.Add(p3Header);
            Phrase p4Header = null;
            if (totalProfitLoss > 0)
            {
                p4Header = new Phrase("Profit : " + totalProfitLoss.ToString("0.00") + " AED ( " + profitLossPercentage.ToString("0.00") + "% )", creditFont);
            }
            else
            {
                p4Header = new Phrase("Loss : " + totalProfitLoss.ToString("0.00") + " AED ( " + profitLossPercentage.ToString("0.00") + "% )", debitFont);
            }

            PdfPTable pdfTab = new PdfPTable(1);
            pdfTab.DefaultCell.PaddingTop = 5f;


            PdfPCell pdfCell2 = new PdfPCell(JPG);
            pdfCell2.Border = Rectangle.NO_BORDER;
            pdfCell2.PaddingBottom = 4f;
            pdfCell2.PaddingTop = 0f;
            pdfCell2.VerticalAlignment = PdfPCell.ALIGN_TOP;
            pdfCell2.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            pdfTab.AddCell(pdfCell2);
            pdfTab.DefaultCell.Border = Rectangle.NO_BORDER;
            pdfTab.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

            pdfCell2 = new PdfPCell(p1Header);
            pdfCell2.Border = Rectangle.NO_BORDER;
            pdfCell2.VerticalAlignment = PdfPCell.ALIGN_TOP;
            pdfCell2.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            pdfCell2.PaddingBottom = 4f;
            pdfTab.AddCell(pdfCell2);
            pdfTab.DefaultCell.Border = Rectangle.NO_BORDER;

            pdfCell2 = new PdfPCell(p4Header);
            pdfCell2.Border = Rectangle.NO_BORDER;
            pdfCell2.VerticalAlignment = PdfPCell.ALIGN_TOP;
            pdfCell2.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            pdfCell2.PaddingBottom = 4f;
            pdfTab.AddCell(pdfCell2);
            pdfTab.DefaultCell.Border = Rectangle.NO_BORDER;

            pdfCell2 = new PdfPCell(data);
            pdfCell2.Border = Rectangle.NO_BORDER;
            pdfCell2.VerticalAlignment = PdfPCell.ALIGN_TOP;
            pdfCell2.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            pdfCell2.PaddingBottom = 4f;
            pdfTab.AddCell(pdfCell2);
            pdfTab.DefaultCell.Border = Rectangle.NO_BORDER;

            //We will have to create separate cells to include image logo and 2 separate strings  
            //Row 1  
            String text = "Page No. ";
            //Add paging to footer  
            {
                float len = 0;
                var currentPage = document.PageNumber;

                content.BeginText();
                content.SetFontAndSize(bf, 10);
                content.SetTextMatrix(document.PageSize.GetRight(90), document.PageSize.GetBottom(5));
                content.ShowText(text + currentPage);
                content.EndText();
                len = bf.GetWidthPoint(text + currentPage, 10);
                content.AddTemplate(footerTemplate, document.PageSize.GetRight(90), document.PageSize.GetBottom(5));
            }

            pdfTab.TotalWidth = document.PageSize.Width - 20f;
            pdfTab.WidthPercentage = 100f;
            pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;

            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write
            //Third and fourth param is x and y position to start writing
            pdfTab.WriteSelectedRows(0, -1, 10, (document.PageSize.Height - 5), writer.DirectContent);
            //Move the pointer and draw line to separate footer section from rest of page  

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
