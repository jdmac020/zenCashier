using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier
{
    public interface ICashier
    {
        bool AddSku(string id, double price, bool isEaches);

        bool AddMarkdown(string sku, double amount);

        bool AddSpecialPercentOff(string sku, int quantitytoTrigger, int percentOff, int limit = 0);

        bool AddSpecialSetPrice(string sku, int quantityToTrigger, double specialPrice, int limit = 0);
    }
}
