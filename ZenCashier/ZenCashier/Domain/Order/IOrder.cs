using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenCashier.Domain.Order.Models;

namespace ZenCashier.Domain.Order
{
    public interface IOrder
    {
        ISkuManager Skus { get; set; }
        
        double SubTotal { get; }

        List<ScannedItemModel> ScanLog { get; set; }

        void ScanItem(string sku);

        void ScanItem(string sku, double qty);
    }
}
