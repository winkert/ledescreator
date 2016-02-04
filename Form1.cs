using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static ledescreator.Ledes;

namespace ledescreator
{
    public partial class Form1 : Form
    {
        
        #region Constructor
        public Form1()
        {
            InitializeComponent();
        }
        #endregion
        public ledes Invoice = new ledes();
        #region Methods
        public void updateTotals()
        {
            //correct the totals for the invoice
            Invoice.INVOICE_DATE = InvoiceDate.Value;
            Invoice.INVOICE_NUMBER = InvoiceNum.Text;
            Invoice.CLIENT_ID = InvoiceClientID.Text;
            Invoice.LAW_FIRM_MATTER_ID = InvoiceMatterID.Text;
            Invoice.CLIENT_MATTER_ID = ClientMatterID.Text;
            Invoice.LAW_FIRM_ID = InvoiceTIN.Text;
            Invoice.BILLING_START_DATE = BillStart.Value;
            Invoice.BILLING_END_DATE = BillEnd.Value;
            Invoice.INVOICE_DESCRIPTION = InvoiceDesc.Text;
            Invoice.CalculateTotal();
            DisplayInvoice(Invoice);
            txt_Inv_Total.Text = "Total: " + Utilities.ParseAsCurrency(Invoice.INVOICE_TOTAL, true);
        }
        private void SaveFieldsToInvoice(bool SaveLine = false)
        {
            try
            {
                Invoice.INVOICE_DATE = InvoiceDate.Value;
                Invoice.INVOICE_NUMBER = InvoiceNum.Text;
                Invoice.CLIENT_ID = InvoiceClientID.Text;
                Invoice.LAW_FIRM_MATTER_ID = InvoiceMatterID.Text;
                Invoice.BILLING_START_DATE = BillStart.Value;
                Invoice.BILLING_END_DATE = BillEnd.Value;
                Invoice.INVOICE_DESCRIPTION = InvoiceDesc.Text;
                Invoice.LAW_FIRM_ID = InvoiceTIN.Text;
                Invoice.CLIENT_MATTER_ID = ClientMatterID.Text;
                Invoice.CalculateTotal();
            }
            catch (Exception e)
            {
                throw new InvalidLedesFile("Error saving LEDES", e);
            }
            try
            {
                if (SaveLine)
                {
                    double units, adj, cost;
                    if (!double.TryParse(LineUnit.Text, out units))
                        units = 0;
                    if (!double.TryParse(LineAdj.Text, out adj))
                        adj = 0;
                    if (!double.TryParse(LinePrice.Text, out cost))
                        cost = 0;
                    Invoice.AddLineItem(LineFE.Text, units, adj, LineDate.Value, LineTaskCode.Text, LineExCode.Text, LineActCode.Text, LineKeeperName.Text, LineDesc.Text, cost, LineKeeperName.Text, LineKeeperClas.Text);
                }
            }
            catch (Exception e)
            {
                throw new InvalidLedesFile("Error saving line item", e);
            }
        }
        private void DisplayInvoice(ledes l)
        {
            InvoiceDate.Value = l.INVOICE_DATE;
            InvoiceNum.Text = l.INVOICE_NUMBER;
            InvoiceClientID.Text = l.CLIENT_ID;
            InvoiceMatterID.Text = l.LAW_FIRM_MATTER_ID;
            ClientMatterID.Text = l.CLIENT_MATTER_ID;
            BillStart.Value = l.BILLING_START_DATE;
            BillEnd.Value = l.BILLING_END_DATE;
            InvoiceDesc.Text = l.INVOICE_DESCRIPTION;
            InvoiceTIN.Text = l.LAW_FIRM_ID;
        }
        private void FillFields(ledesLineItem l)
        {
            //Line item fields
            LineFE.Text = l.EXP_FEE_INV_ADJ_TYPE;
            LineUnit.Text = l.LINE_ITEM_NUMBER_OF_UNITS.ToString();
            LineAdj.Text = l.LINE_ITEM_ADJUSTMENT_AMOUNT.ToString();
            LineDate.Value = l.LINE_ITEM_DATE;
            LineTaskCode.Text = l.LINE_ITEM_TASK_CODE;
            LineExCode.Text = l.LINE_ITEM_EXPENSE_CODE;
            LineActCode.Text = l.LINE_ITEM_ACTIVITY_CODE;
            LineKeeperID.Text = l.TIMEKEEPER_ID;
            LineDesc.Text = l.LINE_ITEM_DESCRIPTION;
            LinePrice.Text = l.LINE_ITEM_UNIT_COST.ToString();
            LineKeeperName.Text = l.TIMEKEEPER_NAME;
            LineKeeperClas.Text = l.TIMEKEEPER_CLASSIFICATION;
        }
        #endregion
        #region Event Handlers
        #region Buttons
        private void btn_New_Click(object sender, EventArgs e)
        {
            foreach(Control c in flowLayoutPanel1.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = string.Empty;
                }
                if (c is DateTimePicker)
                {
                    c.Text = string.Empty;
                }
                LineFE.SelectedIndex = -1;
                LineExCode.SelectedIndex = -1;
                LineActCode.SelectedIndex = -1;
                LineActCode.SelectedIndex = -1;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //add the new line item
            SaveFieldsToInvoice(true);
            //correct the totals for all items
            updateTotals();
            if (lLines.SelectedIndex > -1)
                lLines.Items.Add(Invoice.InvoiceLineItems[lLines.SelectedIndex]);
            else
                lLines.Items.Add(Invoice.InvoiceLineItems.Last());
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            if (lLines.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an item to load.");
            }
            else
            {
                //grab the ledes item
                ledesLineItem l = (ledesLineItem)lLines.Items[lLines.SelectedIndex];
                //fill the invoice details
                FillFields(l);
            }
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            if (lLines.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an item to delete.");
            }
            else
            {
                lLines.Items.Remove(lLines.SelectedItem);
                updateTotals();
            }
        }
        #endregion
        #region Menu Items
        private void btn_Export_Click(object sender, EventArgs e)
        {
            string saveLocation;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "LEDES 98B|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                saveLocation = sfd.FileName;
            }
            else
                return;
            #region Create File Data
            updateTotals();
            List<string> lines = new List<string>();
            //add header information for ledes files
            lines.Add(HeadLine);
            lines.Add(topLine);
            //add each item as a row
            for(int i = 0; i < Invoice.InvoiceLineItems.Count; i++)
            {
                lines.Add(Invoice.WriteInvoice(i));
            }
            #endregion
            #region SaveFile
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@saveLocation, false, Encoding.ASCII))
            {
                foreach (string line in lines)
                {
                    file.Write(line + "\r\n"); //Changed from file.WriteLine and added \r\n
                }
            }
            #endregion
        }

        private void btn_Import_Click(object sender, EventArgs e)
        {
            string fileLocation;
            OpenFileDialog lfd = new OpenFileDialog();
            lfd.Filter = "Ledes 98B|*.txt";
            if (lfd.ShowDialog() == DialogResult.OK)
                fileLocation = lfd.FileName;
            else
                return;
            #region Check File
            try
            {
                List<string> lines = new List<string>();
                using (System.IO.StreamReader file = new System.IO.StreamReader(fileLocation, Encoding.ASCII))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
                if (lines[0] != HeadLine || lines[1] != topLine)
                {
                    throw new InvalidLedesFile("Bad Header on LEDES file");
                }
                string[] exceptions = { HeadLine, topLine };
                lines = lines.Except(exceptions).ToList();
                //This should be threaded....
                Invoice = parseToLedes(lines);
                updateTotals();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.InnerException.Message, error.Message);
            }
            #endregion
        }

        private void btn_NewInvoice_Click(object sender, EventArgs e)
        {
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = string.Empty;
                }
                if (c is DateTimePicker)
                {
                    c.Text = string.Empty;
                }
            }
            foreach (Control c in flowLayoutPanel2.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = string.Empty;
                }
                if (c is DateTimePicker)
                {
                    c.Text = string.Empty;
                }
            }
            LineFE.SelectedIndex = -1;
            LineExCode.SelectedIndex = -1;
            LineActCode.SelectedIndex = -1;
            LineActCode.SelectedIndex = -1;
            lLines.Items.Clear();
            Invoice = new ledes();
        }

        private void exportToPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Invoice PDF|*.pdf";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                InvoicePDF pdf = new InvoicePDF(Invoice);
                pdf.SavePDF(sfd.FileName);
            }
        }

        private void btn_Quit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
        private void LineFE_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (LineFE.Text)
            {
                case "E":
                    LineActCode.Text = string.Empty;
                    LineActCode.Enabled = false;
                    LineExCode.Enabled = true;
                    LineTaskCode.Text = string.Empty;
                    LineTaskCode.Enabled = false;
                    break;
                case "F":
                    LineActCode.Enabled = true;
                    LineExCode.Text = string.Empty;
                    LineExCode.Enabled = false;
                    LineTaskCode.Enabled = true;
                    break;
                default:
                    LineActCode.Text = string.Empty;
                    LineActCode.Enabled = true;
                    LineExCode.Text = string.Empty;
                    LineExCode.Enabled = true;
                    LineTaskCode.Text = string.Empty;
                    LineTaskCode.Enabled = true;
                    break;
            }
        }
        #endregion

        
    }
}
