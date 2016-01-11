using System;
using System.Collections.Generic;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using MigraDoc.DocumentObjectModel.Tables;

namespace ledescreator
{
    public class InvoicePDF
    {
        #region Constructors
        public InvoicePDF()
        { }
        public InvoicePDF(List<ledes> l)
        {
            InvoiceLines = l;
            InvoiceTotal = Utilities.ParseAsCurrency(l[0].INVOICE_TOTAL, true);
            InvoiceDate = l.First().INVOICE_DATE.ToShortDateString();
            InvoiceDateRange = l.First().BILLING_START_DATE.ToShortDateString() + " - " + l.First().BILLING_END_DATE.ToShortDateString();
            InvoiceNumber = l.First().INVOICE_NUMBER;
            InvoiceDescription = l.First().INVOICE_DESCRIPTION;
            InvoiceTitle = InvoiceNumber + " - " + l.First().LAW_FIRM_MATTER_ID + " : " + InvoiceTotal + " (" + InvoiceDate + ")";
            #region Set Default Formats
            defaultBorder.Color = Colors.Black;
            defaultBorder.Style = BorderStyle.Single;
            headerShading.Color = Colors.LightGray;
            defaultParagraph.Alignment = ParagraphAlignment.Left;
            defaultParagraph.Font = new Font("Times New Roman", "10");
            headerFormat.Alignment = ParagraphAlignment.Center;
            headerFormat.Font = new Font("Times New Roman", "11");
            headerFormat.Font.Bold = true;
            #endregion
        }
        #endregion
        #region Fields
        private List<ledes> InvoiceLines = new List<ledes>();
        private string InvoiceTitle = string.Empty;
        private string InvoiceNumber = string.Empty;
        private string InvoiceDescription = string.Empty;
        private string InvoiceDate = string.Empty;
        private string InvoiceDateRange = string.Empty;
        private string InvoiceTotal = string.Empty;
        #region Default Formats
        private Borders defaultBorder = new Borders();
        private Shading headerShading = new Shading();
        private ParagraphFormat defaultParagraph = new ParagraphFormat();
        private ParagraphFormat headerFormat = new ParagraphFormat();
        #endregion
        #endregion
        #region Methods
        public void SavePDF(string location)
        {
            Document file = new Document();
            file.Info.Title = InvoiceTitle;
            Section invoice = file.AddSection();
            drawInvoiceHeader(ref invoice);
            drawTable(InvoiceLines, ref invoice);
            RenderAndSavePDF(file, location);
        }
        private void drawInvoiceHeader(ref Section s)
        {
            Paragraph title = s.AddParagraph("Invoice Number: " + InvoiceNumber);
            Paragraph descTitle = s.AddParagraph("Invoice Details:");
            Paragraph desc = s.AddParagraph(InvoiceDescription);
            Paragraph date = s.AddParagraph("Date: " + InvoiceDate);
            Paragraph daterange = s.AddParagraph(InvoiceDateRange);
            Paragraph total = s.AddParagraph("Total Due: " + InvoiceTotal);
        }
        private void drawTable(List<ledes> l, ref Section s)
        {
            Table t = s.AddTable();
            t.Borders = defaultBorder;
            t.Format = defaultParagraph;
            Column lineDate = t.AddColumn("2cm");
            Column lineDesc = t.AddColumn("5cm");
            Column lineUnits = t.AddColumn("1.5cm");
            Column linePrice = t.AddColumn("2.1cm");
            Column lineDisallow = t.AddColumn("2.1cm");
            Column lineAmt = t.AddColumn("2.1cm");
            drawHeader(ref t);
            foreach (ledes line in l)
            {
                drawLineItem(line, ref t);
            }
        }
        private void drawHeader(ref Table t)
        {
            Row r = t.AddRow();
            r.Shading = headerShading;
            r.Format = headerFormat;
            r.Cells[0].AddParagraph("Date");
            r.Cells[1].AddParagraph("Description");
            r.Cells[2].AddParagraph("Units");
            r.Cells[3].AddParagraph("Unit Cost");
            r.Cells[4].AddParagraph("Disallowed");
            r.Cells[5].AddParagraph("Total");
        }
        private void drawLineItem(ledes l, ref Table t)
        {
            Row r = t.AddRow();
            r.Cells[0].AddParagraph(l.LINE_ITEM_DATE.ToShortDateString());
            r.Cells[1].AddParagraph(l.LINE_ITEM_DESCRIPTION);
            r.Cells[2].AddParagraph(l.LINE_ITEM_NUMBER_OF_UNITS.ToString());
            r.Cells[3].AddParagraph(Utilities.ParseAsCurrency(l.LINE_ITEM_UNIT_COST, true));
            r.Cells[4].AddParagraph(Utilities.ParseAsCurrency(l.LINE_ITEM_ADJUSTMENT_AMOUNT, true));
            r.Cells[5].AddParagraph(Utilities.ParseAsCurrency(l.LINE_ITEM_TOTAL, true));
        }
        private void RenderAndSavePDF(Document file, string filename)
        {
            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = file;
            pdfRenderer.RenderDocument();
            pdfRenderer.Save(filename);
        }
        #endregion
    }

    public static class Utilities
    {
        public static string ParseAsCurrency(double m, bool dollarsign = false)
        {
            if (m == 0)
            {
                return "";
            }
            string dollar;
            if (dollarsign)
                dollar = "$" + m.ToString("##,###,##0.00");
            else
                dollar = m.ToString("#######0.00");
            return dollar;
        }
        public static DateTime convertToDate(string d)
        {
            int yyyy = int.Parse(d.Substring(0, 4));
            int mm = int.Parse(d.Substring(4, 2));
            int dd = int.Parse(d.Substring(6, 2));
            return new DateTime(yyyy, mm, dd);
        }
    }
}
