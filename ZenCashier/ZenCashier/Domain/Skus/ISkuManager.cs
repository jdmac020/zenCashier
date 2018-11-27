using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenCashier.Domain.Skus.Models;

namespace ZenCashier
{
    public interface ISkuManager
    {
        bool AddSku(string id, double price);

        bool AddMarkdown(string sku, double amount);

        bool AddSpecial(string sku, int quantityToTrigger, double amount, bool isPercent, bool equalOrLesserValue, int limit = 0);

        double GetPrice(string sku);

        double GetMarkdown(string sku);

        SpecialInfoModel GetSpecial(string sku);
    }
}
