using System;

namespace ledescreator
{
    public class ledes
    {
        public DateTime INVOICE_DATE;
        public string INVOICE_NUMBER;
        public string CLIENT_ID;
        public string LAW_FIRM_MATTER_ID;
        public double INVOICE_TOTAL;
        public DateTime BILLING_START_DATE;
        public DateTime BILLING_END_DATE;
        public string INVOICE_DESCRIPTION;
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
        public string LAW_FIRM_ID;
        public double LINE_ITEM_UNIT_COST;
        public string TIMEKEEPER_NAME;
        public string TIMEKEEPER_CLASSIFICATION;
        public string CLIENT_MATTER_ID;
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
        public static ledes parseToLedes(string line)
        {
            ledes l = new ledes();
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
                l.LINE_ITEM_NUMBER = grabPart(line, 1) == string.Empty ? 0 : int.Parse(grabPart(line, 9));
                l.EXP_FEE_INV_ADJ_TYPE = grabPart(line, 10);
                l.LINE_ITEM_NUMBER_OF_UNITS = grabPart(line, 11) == string.Empty ? 0.00 : double.Parse(grabPart(line, 11));
                l.LINE_ITEM_ADJUSTMENT_AMOUNT = grabPart(line, 12) == string.Empty ? 0.00 : double.Parse(grabPart(line, 12));
                l.LINE_ITEM_TOTAL = grabPart(line, 13) == string.Empty ? 0.00 : double.Parse(grabPart(line, 13));
                l.LINE_ITEM_DATE = grabPart(line, 14) == string.Empty ? new DateTime() : Utilities.convertToDate(grabPart(line, 14));
                l.LINE_ITEM_TASK_CODE = grabPart(line, 15);
                l.LINE_ITEM_EXPENSE_CODE = grabPart(line, 16);
                l.LINE_ITEM_ACTIVITY_CODE = grabPart(line, 17);
                l.TIMEKEEPER_ID = grabPart(line, 18);
                l.LINE_ITEM_DESCRIPTION = grabPart(line, 19);
                l.LAW_FIRM_ID = grabPart(line, 20);
                l.LINE_ITEM_UNIT_COST = grabPart(line, 21) == string.Empty ? 0.00 : double.Parse(grabPart(line, 21));
                l.TIMEKEEPER_NAME = grabPart(line, 22);
                l.TIMEKEEPER_CLASSIFICATION = grabPart(line, 23);
                l.CLIENT_MATTER_ID = grabPart(line, 24);
            }
            catch (Exception e)
            {
                InvalidLedesFile er = new InvalidLedesFile("Invalid Ledes File", e);
                throw er;
            }
            return l;
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
