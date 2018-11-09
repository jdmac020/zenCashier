using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier
{
    public class SkuManager : ISkuManager
    {
        public bool AddMarkdown(string sku, double amount)
        {
            return IsValidSkuAndAmount(sku, amount);
        }

        public bool AddSku(string id, double price, bool isEaches)
        {
            return IsValidSkuAndAmount(id, price);
        }

        public bool AddSpecial(string sku, int quantityToTrigger, double amount, bool isPercent, int limit = 0)
        {
            return IsValidSpecial(sku, quantityToTrigger, amount, limit);
        }

        protected bool IsValidSkuAndAmount(string skuId, double amount)
        {
            if (string.IsNullOrEmpty(skuId))
                return false;

            if (amount < 0)
                return false;

            return true;
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
