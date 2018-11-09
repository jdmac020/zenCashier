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
            return IsValidSpecial(sku, quantitytoTrigger, percentOff, limit);
        }

        public bool AddSpecialSetPrice(string sku, int quantityToTrigger, double specialPrice, int limit = 0)
        {
            return IsValidSpecial(sku, quantityToTrigger, specialPrice, limit);
        }

        protected bool IsValidSpecial(string sku, int quantityToTrigger, double specialPrice, int limit)
        {
            if (string.IsNullOrEmpty(sku))
                return false;

            if (quantityToTrigger <= 0)
                return false;

            if (specialPrice <= 0)
                return false;

            if (limit < 0)
                return false;

            return true;
        }
    }
}
