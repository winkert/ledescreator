using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ledescreator
{
    [Serializable]
    public class ledes
    {
        public ledes()
        {

        }
        public List<ledesLineItem> InvoiceLineItems = new List<ledesLineItem>();
        public DateTime INVOICE_DATE;
        public string INVOICE_NUMBER;
        public string CLIENT_ID;
        public string LAW_FIRM_MATTER_ID;
        public double INVOICE_TOTAL;
        public DateTime BILLING_START_DATE;
        public DateTime BILLING_END_DATE;
        public string INVOICE_DESCRIPTION;
        public string LAW_FIRM_ID;
        public string CLIENT_MATTER_ID;
        public void UpdateLineItem(ledesLineItem l, int index)
        {
            if (index < InvoiceLineItems.Count)
                InvoiceLineItems[index] = l;
            else
                throw new IndexOutOfRangeException();
            CalculateTotal();
        }
        public void AddLineItem(ledesLineItem l)
        {
            InvoiceLineItems.Add(l);
            CalculateTotal();
        }
        public void AddLineItem(string FE, double units, double adj, DateTime date, string taskcode, string expcode, string actcode, string tkID, string description, double cost, string tkName, string tkClass)
        {
            try
            {
                ledesLineItem l = new ledesLineItem();
                l.LINE_ITEM_NUMBER = InvoiceLineItems.Count + 1;
                l.EXP_FEE_INV_ADJ_TYPE = FE;
                l.LINE_ITEM_NUMBER_OF_UNITS = units;
                l.LINE_ITEM_ADJUSTMENT_AMOUNT = adj;
                l.LINE_ITEM_DATE = date;
                l.LINE_ITEM_TASK_CODE = taskcode;
                l.LINE_ITEM_ACTIVITY_CODE = actcode;
                l.TIMEKEEPER_ID = tkID;
                l.LINE_ITEM_DESCRIPTION = description;
                l.LINE_ITEM_UNIT_COST = cost;
                if (adj == 0)
                    l.LINE_ITEM_TOTAL = units * cost;
                else
                    l.LINE_ITEM_TOTAL = units * adj;
                l.TIMEKEEPER_CLASSIFICATION = tkClass;
                l.TIMEKEEPER_NAME = tkName;
                l.LINE_ITEM_EXPENSE_CODE = expcode;
            }
            catch (Exception e)
            {
                throw;
            }
            CalculateTotal();
        }
        public void CalculateTotal()
        {
            INVOICE_TOTAL = 0;
            foreach (ledesLineItem i in InvoiceLineItems)
            {
                INVOICE_TOTAL += i.LINE_ITEM_TOTAL;
            }
        }
        public string WriteInvoice(int index)
        {
            string newLine = "";
            newLine += INVOICE_DATE.ToString(Utilities.dateformat) + "|";
            newLine += INVOICE_NUMBER + "|";
            newLine += CLIENT_ID + "|";
            newLine += LAW_FIRM_MATTER_ID + "|";
            newLine += Utilities.ParseAsCurrency(INVOICE_TOTAL) + "|";

            newLine += BILLING_START_DATE.ToString(Utilities.dateformat) + "|";
            newLine += BILLING_END_DATE.ToString(Utilities.dateformat) + "|";
            newLine += INVOICE_DESCRIPTION + "|";
            newLine += InvoiceLineItems[index].LINE_ITEM_NUMBER + "|";
            newLine += InvoiceLineItems[index].EXP_FEE_INV_ADJ_TYPE + "|";
            newLine += InvoiceLineItems[index].LINE_ITEM_NUMBER_OF_UNITS + "|";
            if (InvoiceLineItems[index].LINE_ITEM_ADJUSTMENT_AMOUNT > 0)
            {
                newLine += Utilities.ParseAsCurrency(InvoiceLineItems[index].LINE_ITEM_ADJUSTMENT_AMOUNT) + "|";
            }
            else
            {
                newLine += "|";
            }
            newLine += Utilities.ParseAsCurrency(InvoiceLineItems[index].LINE_ITEM_TOTAL) + "|";
            newLine += InvoiceLineItems[index].LINE_ITEM_DATE.ToString(Utilities.dateformat) + "|";
            newLine += InvoiceLineItems[index].LINE_ITEM_TASK_CODE + "|";
            newLine += InvoiceLineItems[index].LINE_ITEM_EXPENSE_CODE + "|";
            newLine += InvoiceLineItems[index].LINE_ITEM_ACTIVITY_CODE + "|";
            newLine += InvoiceLineItems[index].TIMEKEEPER_ID + "|";
            newLine += InvoiceLineItems[index].LINE_ITEM_DESCRIPTION + "|";
            newLine += LAW_FIRM_ID + "|";
            newLine += Utilities.ParseAsCurrency(InvoiceLineItems[index].LINE_ITEM_UNIT_COST) + "|";
            newLine += InvoiceLineItems[index].TIMEKEEPER_NAME + "|";
            newLine += InvoiceLineItems[index].TIMEKEEPER_CLASSIFICATION + "|";
            newLine += CLIENT_MATTER_ID + "[]";
            return newLine;
        }
        public void RemoveLineItem(ledesLineItem l)
        {
            if (InvoiceLineItems.Contains(l))
                InvoiceLineItems.Remove(l);
            else
                throw new ArgumentException("Invalid line item selected.");
        }
        public void RemoveLineItems(int l)
        {
            if (l < InvoiceLineItems.Count)
                InvoiceLineItems.RemoveAt(l);
            else
                throw new IndexOutOfRangeException();
        }
        //override ToString()
        public override string ToString()
        {
            return string.Format("{0} : {1}", INVOICE_NUMBER, Utilities.ParseAsCurrency(INVOICE_TOTAL, true));
        } 
    }
    [Serializable]
    public class ledesLineItem
    {
        public ledesLineItem()
        {

        }
        public int LINE_ITEM_NUMBER;
        public string EXP_FEE_INV_ADJ_TYPE;
        public double LINE_ITEM_NUMBER_OF_UNITS;
        public double LINE_ITEM_ADJUSTMENT_AMOUNT;
        public double LINE_ITEM_TOTAL;
        public DateTime LINE_ITEM_DATE;
        public string LINE_ITEM_TASK_CODE;
        public string LINE_ITEM_EXPENSE_CODE;
        public string LINE_ITEM_ACTIVITY_CODE;
        public string TIMEKEEPER_ID;
        public string LINE_ITEM_DESCRIPTION;
        public double LINE_ITEM_UNIT_COST;
        public string TIMEKEEPER_NAME;
        public string TIMEKEEPER_CLASSIFICATION;
        //override ToString()
        public override string ToString()
        {
            return string.Format("{0} : {1}", EXP_FEE_INV_ADJ_TYPE, Utilities.ParseAsCurrency(LINE_ITEM_TOTAL, true));
        }
    }

    public static class Ledes
    {
        public static string HeadLine = "LEDES1998B[]";
        public static string topLine = "INVOICE_DATE|INVOICE_NUMBER|CLIENT_ID|LAW_FIRM_MATTER_ID|INVOICE_TOTAL|BILLING_START_DATE|BILLING_END_DATE|INVOICE_DESCRIPTION|LINE_ITEM_NUMBER|EXP/FEE/INV_ADJ_TYPE|LINE_ITEM_NUMBER_OF_UNITS|LINE_ITEM_ADJUSTMENT_AMOUNT|LINE_ITEM_TOTAL|LINE_ITEM_DATE|LINE_ITEM_TASK_CODE|LINE_ITEM_EXPENSE_CODE|LINE_ITEM_ACTIVITY_CODE|TIMEKEEPER_ID|LINE_ITEM_DESCRIPTION|LAW_FIRM_ID|LINE_ITEM_UNIT_COST|TIMEKEEPER_NAME|TIMEKEEPER_CLASSIFICATION|CLIENT_MATTER_ID[]";
        public class InvalidLedesFile : Exception
        {
            public InvalidLedesFile()
            { }
            public InvalidLedesFile(string message) : base(message)
            { }
            public InvalidLedesFile(string message, Exception inner) : base(message, inner)
            { }
        }
        public static ledes parseToLedes(List<string> lines)
        {
            ledes l = new ledes();
            string line = lines.First();
            try
            {
                l.INVOICE_DATE = grabPart(line,1) == string.Empty? new DateTime() : Utilities.convertToDate(grabPart(line, 1));
                l.INVOICE_NUMBER = grabPart(line, 2);
                l.CLIENT_ID = grabPart(line, 3);
                l.LAW_FIRM_MATTER_ID = grabPart(line, 4);
                l.INVOICE_TOTAL = grabPart(line, 5) == string.Empty ? 0.00 : double.Parse(grabPart(line, 5));
                l.BILLING_START_DATE = grabPart(line, 6) == string.Empty ? new DateTime() : Utilities.convertToDate(grabPart(line, 6));
                l.BILLING_END_DATE = grabPart(line, 7) == string.Empty ? new DateTime() : Utilities.convertToDate(grabPart(line, 7));
                l.INVOICE_DESCRIPTION = grabPart(line, 8);
                l.LAW_FIRM_ID = grabPart(line, 20);
                l.CLIENT_MATTER_ID = grabPart(line, 24);

                foreach (string item in lines)
                {
                    ledesLineItem li = new ledesLineItem();
                    li.LINE_ITEM_NUMBER = grabPart(item, 1) == string.Empty ? 0 : int.Parse(grabPart(item, 9));
                    li.EXP_FEE_INV_ADJ_TYPE = grabPart(item, 10);
                    li.LINE_ITEM_NUMBER_OF_UNITS = grabPart(item, 11) == string.Empty ? 0.00 : double.Parse(grabPart(item, 11));
                    li.LINE_ITEM_ADJUSTMENT_AMOUNT = grabPart(item, 12) == string.Empty ? 0.00 : double.Parse(grabPart(item, 12));
                    li.LINE_ITEM_TOTAL = grabPart(item, 13) == string.Empty ? 0.00 : double.Parse(grabPart(item, 13));
                    li.LINE_ITEM_DATE = grabPart(item, 14) == string.Empty ? new DateTime() : Utilities.convertToDate(grabPart(item, 14));
                    li.LINE_ITEM_TASK_CODE = grabPart(item, 15);
                    li.LINE_ITEM_EXPENSE_CODE = grabPart(item, 16);
                    li.LINE_ITEM_ACTIVITY_CODE = grabPart(item, 17);
                    li.TIMEKEEPER_ID = grabPart(item, 18);
                    li.LINE_ITEM_DESCRIPTION = grabPart(item, 19);
                    li.LINE_ITEM_UNIT_COST = grabPart(item, 21) == string.Empty ? 0.00 : double.Parse(grabPart(item, 21));
                    li.TIMEKEEPER_NAME = grabPart(item, 22);
                    li.TIMEKEEPER_CLASSIFICATION = grabPart(item, 23);
                    l.AddLineItem(li); 
                }
            }
            catch (Exception e)
            {
                InvalidLedesFile er = new InvalidLedesFile("Invalid Ledes File", e);
                throw er;
            }
            return l;
        }
        public static double CalculateTotal(ledes l)
        {
            l.CalculateTotal();
            return l.INVOICE_TOTAL;
        }

        private static string grabPart(string line, int part)
        {
            int start = 0;
            for(int c = 1; c < part; c ++)
            {
                start = line.IndexOf('|', start + 1);
            }
            int len = line.IndexOf('|', start + 1) - start > 0 ? line.IndexOf('|', start + 1) - start : line.IndexOf('[') - start;
            string p = string.Empty;
            if (len > 0)
                p = line.Substring(start, len).Replace("|",string.Empty);
            Console.WriteLine(p);
            return p;
        }
    }
}
