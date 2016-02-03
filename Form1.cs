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
        public string dateformat = "yyyyMMdd";
        #region Constructor
        public Form1()
        {
            InitializeComponent();
        }
        #endregion
        #region Methods
        public void addLine(ledes l)
        {
            lLines.Items.Add(l);
        }
        public double calcTotal()
        {
            double total = 0;
            foreach (ledes l in lLines.Items)
            {
                total += l.LINE_ITEM_TOTAL;
            }
            return total;
        }
        public void updateTotals()
        {
            //correct the totals for all items
            foreach (ledes i in lLines.Items)
            {
                i.INVOICE_DATE = InvoiceDate.Value;
                i.INVOICE_NUMBER = InvoiceNum.Text;
                i.CLIENT_ID = InvoiceClientID.Text;
                i.LAW_FIRM_MATTER_ID = InvoiceMatterID.Text;
                i.CLIENT_MATTER_ID = ClientMatterID.Text;
                i.LAW_FIRM_ID = InvoiceTIN.Text;
                i.BILLING_START_DATE = BillStart.Value;
                i.BILLING_END_DATE = BillEnd.Value;
                i.INVOICE_DESCRIPTION = InvoiceDesc.Text;
                i.INVOICE_TOTAL = calcTotal();
            }
            DisplayInvoice(lLines.Items.OfType<ledes>().ToList().First());
            txt_Inv_Total.Text = "Total: " + Utilities.ParseAsCurrency(calcTotal(), true);
        }
        private ledes SaveFieldsToInvoice(bool SaveLine = false)
        {
            ledes l = new ledes();
            try
            {
                l.INVOICE_DATE = InvoiceDate.Value;
                l.INVOICE_NUMBER = InvoiceNum.Text;
                l.CLIENT_ID = InvoiceClientID.Text;
                l.LAW_FIRM_MATTER_ID = InvoiceMatterID.Text;
                l.INVOICE_TOTAL = calcTotal();
                l.BILLING_START_DATE = BillStart.Value;
                l.BILLING_END_DATE = BillEnd.Value;
                l.INVOICE_DESCRIPTION = InvoiceDesc.Text;
                l.LAW_FIRM_ID = InvoiceTIN.Text;
                l.CLIENT_MATTER_ID = ClientMatterID.Text;
            }
            catch (Exception e)
            {
                throw new InvalidLedesFile("Error saving LEDES", e);
            }
            try
            {
                if (SaveLine)
                {
                    l.LINE_ITEM_NUMBER = lLines.Items.Count + 1;
                    l.EXP_FEE_INV_ADJ_TYPE = LineFE.Text;
                    if (!double.TryParse(LineUnit.Text, out l.LINE_ITEM_NUMBER_OF_UNITS))
                        l.LINE_ITEM_NUMBER_OF_UNITS = 0;
                    if (!double.TryParse(LineAdj.Text, out l.LINE_ITEM_ADJUSTMENT_AMOUNT))
                        l.LINE_ITEM_ADJUSTMENT_AMOUNT = 0;
                    if (!double.TryParse(LinePrice.Text, out l.LINE_ITEM_UNIT_COST))
                        l.LINE_ITEM_UNIT_COST = 0;
                    if (l.LINE_ITEM_ADJUSTMENT_AMOUNT > 0)
                        l.LINE_ITEM_TOTAL = l.LINE_ITEM_NUMBER_OF_UNITS * l.LINE_ITEM_ADJUSTMENT_AMOUNT;
                    else
                        l.LINE_ITEM_TOTAL = l.LINE_ITEM_NUMBER_OF_UNITS * l.LINE_ITEM_UNIT_COST;
                    l.LINE_ITEM_DATE = LineDate.Value;
                    l.LINE_ITEM_TASK_CODE = LineTaskCode.Text;
                    l.LINE_ITEM_EXPENSE_CODE = LineExCode.Text;
                    l.LINE_ITEM_ACTIVITY_CODE = LineActCode.Text;
                    l.TIMEKEEPER_ID = LineKeeperID.Text;
                    l.LINE_ITEM_DESCRIPTION = LineDesc.Text;
                    l.TIMEKEEPER_NAME = LineKeeperName.Text;
                    l.TIMEKEEPER_CLASSIFICATION = LineKeeperClas.Text;
                    l.INVOICE_TOTAL = calcTotal();
                }
            }
            catch (Exception e)
            {
                throw new InvalidLedesFile("Error saving line item", e);
            }
            return l;
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
        private void FillFields(ledes l)
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
            addLine(SaveFieldsToInvoice(true));
            //correct the totals for all items
            updateTotals();
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
                ledes l = (ledes)lLines.Items[lLines.SelectedIndex];
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
            foreach (ledes l in lLines.Items)
            {

                string newLine = "";
                newLine += l.INVOICE_DATE.ToString(dateformat) + "|";
                newLine += l.INVOICE_NUMBER + "|";
                newLine += l.CLIENT_ID + "|";
                newLine += l.LAW_FIRM_MATTER_ID + "|";
                newLine += Utilities.ParseAsCurrency(l.INVOICE_TOTAL) + "|";
                newLine += l.BILLING_START_DATE.ToString(dateformat) + "|";
                newLine += l.BILLING_END_DATE.ToString(dateformat) + "|";
                newLine += l.INVOICE_DESCRIPTION + "|";
                newLine += l.LINE_ITEM_NUMBER + "|";
                newLine += l.EXP_FEE_INV_ADJ_TYPE + "|";
                newLine += l.LINE_ITEM_NUMBER_OF_UNITS + "|";
                if (l.LINE_ITEM_ADJUSTMENT_AMOUNT > 0)
                {
                    newLine += Utilities.ParseAsCurrency(l.LINE_ITEM_ADJUSTMENT_AMOUNT) + "|";
                }
                else
                {
                    newLine += "|";
                }
                newLine += Utilities.ParseAsCurrency(l.LINE_ITEM_TOTAL) + "|";
                newLine += l.LINE_ITEM_DATE.ToString(dateformat) + "|";
                newLine += l.LINE_ITEM_TASK_CODE + "|";
                newLine += l.LINE_ITEM_EXPENSE_CODE + "|";
                newLine += l.LINE_ITEM_ACTIVITY_CODE + "|";
                newLine += l.TIMEKEEPER_ID + "|";
                newLine += l.LINE_ITEM_DESCRIPTION + "|";
                newLine += l.LAW_FIRM_ID + "|";
                newLine += Utilities.ParseAsCurrency(l.LINE_ITEM_UNIT_COST) + "|";
                newLine += l.TIMEKEEPER_NAME + "|";
                newLine += l.TIMEKEEPER_CLASSIFICATION + "|";
                newLine += l.CLIENT_MATTER_ID + "[]";
                lines.Add(newLine);
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
            {
                fileLocation = lfd.FileName;
            }
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
                for(int n = 2; n < lines.Count; n ++)
                {
                    string s = lines[n];
                    ledes l = parseToLedes(s);
                    addLine(l);
                }
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
        }

        private void exportToPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Invoice PDF|*.pdf";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                InvoicePDF pdf = new InvoicePDF(lLines.Items.OfType<ledes>().ToList());
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
