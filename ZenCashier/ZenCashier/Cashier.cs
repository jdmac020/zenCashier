using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier
{
    public class Cashier : ICashier
    {
        public bool AddMarkdown(string sku, double amount)
        {
            if (string.IsNullOrEmpty(sku))
                return false;

            if (amount < 0)
                return false;

            return true;
        }

        public bool AddSku(string id, double price, bool isEaches)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            if (price < 0)
                return false;

            return true;
        }

        public bool AddSpecialPercentOff(string sku, int quantitytoTrigger, int percentOff, int limit = 0)
        {
            return true;
        }

        public bool AddSpecialSetPrice(string sku, int quantityToTrigger, double specialPrice, int limit = 0)
        {
            throw new NotImplementedException();
        }
    }
}
