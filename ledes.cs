using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int LINE_ITEM_NUMBER_OF_UNITS;
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
            return string.Format("{0} : {1}", EXP_FEE_INV_ADJ_TYPE, LINE_ITEM_TOTAL);
        } 
    }
}
