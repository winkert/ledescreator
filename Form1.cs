using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ledescreator
{
    public partial class Form1 : Form
    {
        public string location = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        public string dateformat = "yyyyMMdd";
        #region Constructor
        public Form1()
        {
            InitializeComponent();
        }
        #endregion
        #region Public Methods
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
        public string toDollars(double c)
        {
            if (c == 0)
            {
                return "";
            }
            string dollar = c.ToString("#######0.00");
            return dollar;
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
                i.CLIENT_MATTER_ID = InvoiceMatterID.Text;
                i.LAW_FIRM_ID = InvoiceTIN.Text;
                i.BILLING_START_DATE = BillStart.Value;
                i.BILLING_END_DATE = BillEnd.Value;
                i.INVOICE_DESCRIPTION = InvoiceDesc.Text;
                i.INVOICE_TOTAL = calcTotal();
            }
            txt_Inv_Total.Text = "Total: $" + toDollars(calcTotal());
        }
        #endregion
        #region Event Handlers
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
            ledes l = new ledes();
            //Invoice common things are set across for each item as I go.
            //There is certainly a better way to do this but I'm not working that hard on this.
            //Line item number is the next in the list
            l.LINE_ITEM_NUMBER = lLines.Items.Count + 1;
            l.EXP_FEE_INV_ADJ_TYPE = LineFE.Text;
            l.LINE_ITEM_DATE = LineDate.Value;
            l.LINE_ITEM_TASK_CODE = LineTaskCode.Text;
            l.LINE_ITEM_EXPENSE_CODE = LineExCode.Text;
            l.LINE_ITEM_ACTIVITY_CODE = LineActCode.Text;
            l.LINE_ITEM_DESCRIPTION = LineDesc.Text;
            l.TIMEKEEPER_ID = LineKeeperID.Text;
            l.TIMEKEEPER_NAME = LineKeeperName.Text;
            l.TIMEKEEPER_CLASSIFICATION = LineKeeperClas.Text;
            //Deal with converting to numbers
            int num;
            double doub;
            if (int.TryParse(LineUnit.Text, out num))
            {
                l.LINE_ITEM_NUMBER_OF_UNITS = int.Parse(LineUnit.Text);
            }
            else
            {
                l.LINE_ITEM_NUMBER_OF_UNITS = 0;
            }
            if (double.TryParse(LineAdj.Text,out doub))
            {
                l.LINE_ITEM_ADJUSTMENT_AMOUNT = double.Parse(LineAdj.Text);
            }
            else
            {
                l.LINE_ITEM_ADJUSTMENT_AMOUNT = 0;
            }
            if (double.TryParse(LinePrice.Text,out doub))
            {
                l.LINE_ITEM_UNIT_COST = double.Parse(LinePrice.Text);
            }
            else
            {
                l.LINE_ITEM_UNIT_COST = 0;
            }
            //Calculate the totals
            if (l.LINE_ITEM_ADJUSTMENT_AMOUNT > 0)
            {
                l.LINE_ITEM_TOTAL = l.LINE_ITEM_NUMBER_OF_UNITS * l.LINE_ITEM_ADJUSTMENT_AMOUNT;
            }
            else
            {
                l.LINE_ITEM_TOTAL = l.LINE_ITEM_NUMBER_OF_UNITS * l.LINE_ITEM_UNIT_COST;
            }
            //add the new line item
            addLine(l);
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
                InvoiceDate.Value = l.INVOICE_DATE;
                InvoiceNum.Text = l.INVOICE_NUMBER;
                InvoiceClientID.Text = l.CLIENT_ID;
                InvoiceMatterID.Text = l.LAW_FIRM_MATTER_ID;
                BillStart.Value = l.BILLING_START_DATE;
                BillEnd.Value = l.BILLING_END_DATE;
                InvoiceDesc.Text = l.INVOICE_DESCRIPTION;
                InvoiceTIN.Text = l.LAW_FIRM_ID;
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

        private void btn_Export_Click(object sender, EventArgs e)
        {
            updateTotals();
            List<string> lines = new List<string>();
            //add header information for ledes files
            lines.Add("LEDES1998B[]");
            lines.Add("INVOICE_DATE|INVOICE_NUMBER|CLIENT_ID|LAW_FIRM_MATTER_ID|INVOICE_TOTAL|BILLING_START_DATE|BILLING_END_DATE|INVOICE_DESCRIPTION|LINE_ITEM_NUMBER|EXP/FEE/INV_ADJ_TYPE|LINE_ITEM_NUMBER_OF_UNITS|LINE_ITEM_ADJUSTMENT_AMOUNT|LINE_ITEM_TOTAL|LINE_ITEM_DATE|LINE_ITEM_TASK_CODE|LINE_ITEM_EXPENSE_CODE|LINE_ITEM_ACTIVITY_CODE|TIMEKEEPER_ID|LINE_ITEM_DESCRIPTION|LAW_FIRM_ID|LINE_ITEM_UNIT_COST|TIMEKEEPER_NAME|TIMEKEEPER_CLASSIFICATION|CLIENT_MATTER_ID[]");
            //add each item as a row
            foreach (ledes l in lLines.Items)
            {

                string newLine = "";
                newLine += l.INVOICE_DATE.ToString(dateformat) + "|";
                newLine += l.INVOICE_NUMBER + "|";
                newLine += l.CLIENT_ID + "|";
                newLine += l.LAW_FIRM_MATTER_ID + "|";
                newLine += toDollars(l.INVOICE_TOTAL) + "|";
                newLine += l.BILLING_START_DATE.ToString(dateformat) + "|";
                newLine += l.BILLING_END_DATE.ToString(dateformat) + "|";
                newLine += l.INVOICE_DESCRIPTION + "|";
                newLine += l.LINE_ITEM_NUMBER + "|";
                newLine += l.EXP_FEE_INV_ADJ_TYPE + "|";
                newLine += l.LINE_ITEM_NUMBER_OF_UNITS + "|";
                if (l.LINE_ITEM_ADJUSTMENT_AMOUNT > 0)
                {
                    newLine += toDollars(l.LINE_ITEM_ADJUSTMENT_AMOUNT) + "|";
                }
                else
                {
                    newLine += "|";
                }
                newLine += toDollars(l.LINE_ITEM_TOTAL) + "|";
                newLine += l.LINE_ITEM_DATE.ToString(dateformat) + "|";
                newLine += l.LINE_ITEM_TASK_CODE + "|";
                newLine += l.LINE_ITEM_EXPENSE_CODE + "|";
                newLine += l.LINE_ITEM_ACTIVITY_CODE + "|";
                newLine += l.TIMEKEEPER_ID + "|";
                newLine += l.LINE_ITEM_DESCRIPTION + "|";
                newLine += l.LAW_FIRM_ID + "|";
                newLine += toDollars(l.LINE_ITEM_UNIT_COST) + "|";
                newLine += l.TIMEKEEPER_NAME + "|";
                newLine += l.TIMEKEEPER_CLASSIFICATION + "|";
                newLine += l.CLIENT_MATTER_ID + "[]";
                lines.Add(newLine);
            }
            //save the file to the desktop
            //Need to change the Constructor used here. Should confirm that the Encoding is correct.
            //Default Encoding is UTF8 which seems to cause issues with the SQL procedure.
            //Should switch to Encosing.ASCII instead as that seems to work.
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@location + "\\ledes.txt", false, Encoding.ASCII))
            {
                foreach (string line in lines)
                {
                    file.Write(line + "\r\n"); //Changed from file.WriteLine and added \r\n
                }
            }
            MessageBox.Show("Exported file to " + location + "\\ledes.txt");
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

        private void btn_Quit_Click(object sender, EventArgs e)
        {
            Close();
        }

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
