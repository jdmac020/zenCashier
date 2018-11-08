using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier
{
    public class Cashier : ICashier
    {
        public bool AddSku(string sku, double price, bool isEaches)
        {
            if (string.IsNullOrEmpty(sku))
                return false;

            if (price < 0)
                return false;

            return true;
        }
    }
}
