using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Order
{
    public interface IOrder
    {
        ISkuManager Skus { get; set; }

        double SubTotal { get; }

        void ScanItem(string sku);

        void ScanItem(string sku, double qty);
    }
}
