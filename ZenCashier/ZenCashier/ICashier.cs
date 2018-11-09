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
    }
}
