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

        public List<ScannedItemModel> ScannedItems
        {
            get
            {
                if (_scannedItems is null)
                    _scannedItems = new List<ScannedItemModel>();

                return _scannedItems;
            }
            set { _scannedItems = value; }
        }

        protected List<ScannedItemModel> _scannedItems;

        public void LogScan(string skuId, double salePrice, double qty)
        {
            ScannedItems.Add(new ScannedItemModel
            {
                SkuId = skuId,
                SalePrice = salePrice,
                QuantityScanned = qty
            });
        }
        
        public IEnumerable<ScannedItemModel> GetScansForSku(string sku)
        {
            var returnRecords = Enumerable.Empty<ScannedItemModel>();

            var scans = ScannedItems.Where(item => item.SkuId.Equals(sku));

            if (scans.Any())
                returnRecords = scans;

            return returnRecords;
        }

        protected double GetSubTotal()
        {
            var subTotal = ScannedItems.Sum(item => item.SalePrice);

            return Math.Round(subTotal, 2);
        }

    }
}
