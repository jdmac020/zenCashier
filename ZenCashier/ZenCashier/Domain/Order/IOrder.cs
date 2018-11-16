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
        
        double SubTotal { get; set; }

        List<ScannedItemModel> ScanLog { get; set; }

        void ScanItem(string sku, bool removeItem = false);

        void ScanItem(string sku, double qty, bool removeItem);

        void AddItem(string sku, double qty = Double.NaN);

        void RemoveItem(string sku, double qty = Double.NaN);
    }
}
