using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenCashier.Domain.Log;

namespace ZenCashier.Domain.Order
{
    public interface IOrder
    {
        ISkuManager Skus { get; set; }
        
        double SubTotal { get; }

        IScanLog ScanLog { get; set; }

        void ScanItem(string sku);

        void ScanItem(string sku, double qty);
    }
}
