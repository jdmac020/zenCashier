using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Log
{
    public class ScanLog : IScanLog
    {
        public double SubTotal { get { return GetSubTotal(); } }

        protected List<ScannedItemModel> _scannedItems = new List<ScannedItemModel>();

        public void LogScan(string skuId, double salePrice, double qty)
        {
            _scannedItems.Add(new ScannedItemModel
            {
                SkuId = skuId,
                SalePrice = salePrice,
                QuantityScanned = qty
            });
        }
        
        public IEnumerable<ScannedItemModel> GetScansForSku(string sku)
        {
            throw new NotImplementedException();
        }

        protected double GetSubTotal()
        {
            var subTotal = _scannedItems.Sum(item => item.SalePrice);

            return Math.Round(subTotal, 2);
        }

    }
}
