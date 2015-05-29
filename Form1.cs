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
        public String location = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        public String dateformat = "yyyyMMdd";
        //-----------------------------------------------------------------------------//
        //Constructor
        //-----------------------------------------------------------------------------//
        public Form1()
        {
            InitializeComponent();
        }
        //-----------------------------------------------------------------------------//
        //Public methods
        //-----------------------------------------------------------------------------//
        public void addLine(ledes l)
        {
            this.lLines.Items.Add(l);
        }
        public double calcTotal()
        {
            double total = 0;
            foreach (ledes l in this.lLines.Items)
            {
                total += l.LINE_ITEM_TOTAL;
            }
            return total;
        }
        public String toDollars(double c)
        {
            if (c == 0)
            {
                return "";
            }
            String dollar = c.ToString("#######0.00");
            return dollar;
        }
        //-----------------------------------------------------------------------------//
        //Event handlers
        //-----------------------------------------------------------------------------//
        private void btn_New_Click(object sender, EventArgs e)
        {
            foreach(Control c in this.flowLayoutPanel1.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = String.Empty;
                }
                if (c is DateTimePicker)
                {
                    c.Text = String.Empty;
                }
                this.LineFE.SelectedIndex = -1;
                this.LineExCode.SelectedIndex = -1;
                this.LineActCode.SelectedIndex = -1;
                this.LineActCode.SelectedIndex = -1;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            ledes l = new ledes();
            //Invoice common things are set across for each item as I go.
            //There is certainly a better way to do this but I'm not working that hard on this.
            //Line item number is the next in the list
            l.LINE_ITEM_NUMBER = this.lLines.Items.Count + 1;
            l.EXP_FEE_INV_ADJ_TYPE = this.LineFE.Text;
            l.LINE_ITEM_DATE = this.LineDate.Value;
            l.LINE_ITEM_TASK_CODE = this.LineTaskCode.Text;
            l.LINE_ITEM_EXPENSE_CODE = this.LineExCode.Text;
            l.LINE_ITEM_ACTIVITY_CODE = this.LineActCode.Text;
            l.LINE_ITEM_DESCRIPTION = this.LineDesc.Text;
            l.TIMEKEEPER_ID = this.LineKeeperID.Text;
            l.TIMEKEEPER_NAME = this.LineKeeperName.Text;
            l.TIMEKEEPER_CLASSIFICATION = this.LineKeeperClas.Text;
            //Deal with converting to numbers
            int num;
            double doub;
            if (int.TryParse(this.LineUnit.Text, out num))
            {
                l.LINE_ITEM_NUMBER_OF_UNITS = int.Parse(this.LineUnit.Text);
            }
            else
            {
                l.LINE_ITEM_NUMBER_OF_UNITS = 0;
            }
            if (double.TryParse(this.LineAdj.Text,out doub))
            {
                l.LINE_ITEM_ADJUSTMENT_AMOUNT = double.Parse(this.LineAdj.Text);
            }
            else
            {
                l.LINE_ITEM_ADJUSTMENT_AMOUNT = 0;
            }
            if (double.TryParse(this.LinePrice.Text,out doub))
            {
                l.LINE_ITEM_UNIT_COST = double.Parse(this.LinePrice.Text);
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
            foreach (ledes i in this.lLines.Items)
            {
                i.INVOICE_DATE = this.InvoiceDate.Value;
                i.INVOICE_NUMBER = this.InvoiceNum.Text;
                i.CLIENT_ID = this.InvoiceClientID.Text;
                i.LAW_FIRM_MATTER_ID = this.InvoiceMatterID.Text;
                i.CLIENT_MATTER_ID = this.InvoiceMatterID.Text;
                i.LAW_FIRM_ID = this.InvoiceTIN.Text;
                i.BILLING_START_DATE = this.BillStart.Value;
                i.BILLING_END_DATE = this.BillEnd.Value;
                i.INVOICE_DESCRIPTION = this.InvoiceDesc.Text;
                i.INVOICE_TOTAL = calcTotal();
            }
            this.txt_Inv_Total.Text = "Total: $" + toDollars(calcTotal());
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            if (this.lLines.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an item to load.");
            }
            else
            {
                //grab the ledes item
                ledes l = (ledes)this.lLines.Items[this.lLines.SelectedIndex];
                //fill the invoice details
                this.InvoiceDate.Value = l.INVOICE_DATE;
                this.InvoiceNum.Text = l.INVOICE_NUMBER;
                this.InvoiceClientID.Text = l.CLIENT_ID;
                this.InvoiceMatterID.Text = l.LAW_FIRM_MATTER_ID;
                this.BillStart.Value = l.BILLING_START_DATE;
                this.BillEnd.Value = l.BILLING_END_DATE;
                this.InvoiceDesc.Text = l.INVOICE_DESCRIPTION;
                this.InvoiceTIN.Text = l.LAW_FIRM_ID;
                //Line item fields
                this.LineFE.Text = l.EXP_FEE_INV_ADJ_TYPE;
                this.LineUnit.Text = l.LINE_ITEM_NUMBER_OF_UNITS.ToString();
                this.LineAdj.Text = l.LINE_ITEM_ADJUSTMENT_AMOUNT.ToString();
                this.LineDate.Value = l.LINE_ITEM_DATE;
                this.LineTaskCode.Text = l.LINE_ITEM_TASK_CODE;
                this.LineExCode.Text = l.LINE_ITEM_EXPENSE_CODE;
                this.LineActCode.Text = l.LINE_ITEM_ACTIVITY_CODE;
                this.LineKeeperID.Text = l.TIMEKEEPER_ID;
                this.LineDesc.Text = l.LINE_ITEM_DESCRIPTION;
                this.LinePrice.Text = l.LINE_ITEM_UNIT_COST.ToString();
                this.LineKeeperName.Text = l.TIMEKEEPER_NAME;
                this.LineKeeperClas.Text = l.TIMEKEEPER_CLASSIFICATION;
            }
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            if (this.lLines.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an item to delete.");
            }
            else
            {
                this.lLines.Items.Remove(this.lLines.SelectedItem);
                this.txt_Inv_Total.Text = "Total: $" + toDollars(calcTotal());
            }
        }

        private void btn_Export_Click(object sender, EventArgs e)
        {
            List<String> lines = new List<String>();
            //add header information for ledes files
            lines.Add("LEDES1998B[]");
            lines.Add("INVOICE_DATE|INVOICE_NUMBER|CLIENT_ID|LAW_FIRM_MATTER_ID|INVOICE_TOTAL|BILLING_START_DATE|BILLING_END_DATE|INVOICE_DESCRIPTION|LINE_ITEM_NUMBER|EXP/FEE/INV_ADJ_TYPE|LINE_ITEM_NUMBER_OF_UNITS|LINE_ITEM_ADJUSTMENT_AMOUNT|LINE_ITEM_TOTAL|LINE_ITEM_DATE|LINE_ITEM_TASK_CODE|LINE_ITEM_EXPENSE_CODE|LINE_ITEM_ACTIVITY_CODE|TIMEKEEPER_ID|LINE_ITEM_DESCRIPTION|LAW_FIRM_ID|LINE_ITEM_UNIT_COST|TIMEKEEPER_NAME|TIMEKEEPER_CLASSIFICATION|CLIENT_MATTER_ID[]");
            //add each item as a row
            foreach (ledes l in this.lLines.Items)
            {
                String newLine = "";
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
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@location + "\\ledes.txt"))
            {
                foreach (String line in lines)
                {
                    file.WriteLine(line);
                }
            }
            MessageBox.Show("Exported file to " + location + "\\ledes.txt");
        }

        private void btn_NewInvoice_Click(object sender, EventArgs e)
        {
            foreach (Control c in this.flowLayoutPanel1.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = String.Empty;
                }
                if (c is DateTimePicker)
                {
                    c.Text = String.Empty;
                }
            }
            foreach (Control c in this.flowLayoutPanel2.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = String.Empty;
                }
                if (c is DateTimePicker)
                {
                    c.Text = String.Empty;
                }
            }
            this.LineFE.SelectedIndex = -1;
            this.LineExCode.SelectedIndex = -1;
            this.LineActCode.SelectedIndex = -1;
            this.LineActCode.SelectedIndex = -1;
            this.lLines.Items.Clear();
        }

        private void btn_Quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LineFE_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (LineFE.Text)
            {
                case "E":
                    LineActCode.Text = String.Empty;
                    LineActCode.Enabled = false;
                    LineExCode.Enabled = true;
                    LineTaskCode.Text = String.Empty;
                    LineTaskCode.Enabled = false;
                    break;
                case "F":
                    LineActCode.Enabled = true;
                    LineExCode.Text = String.Empty;
                    LineExCode.Enabled = false;
                    LineTaskCode.Enabled = true;
                    break;
                default:
                    LineActCode.Text = String.Empty;
                    LineActCode.Enabled = true;
                    LineExCode.Text = String.Empty;
                    LineExCode.Enabled = true;
                    LineTaskCode.Text = String.Empty;
                    LineTaskCode.Enabled = true;
                    break;
            }
        }
    }
}
