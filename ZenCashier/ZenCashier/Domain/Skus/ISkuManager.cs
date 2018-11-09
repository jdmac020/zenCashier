using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier
{
    public interface ISkuManager
    {
        bool AddSku(string id, double price, bool isEaches);

        bool AddMarkdown(string sku, double amount);

        bool AddSpecial(string sku, int quantityToTrigger, double amount, bool isPercentOff, int limit = 0);
    }
}
