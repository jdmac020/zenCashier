using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenCashier.Domain.Log;
using ZenCashier.Exceptions;

namespace ZenCashier.Domain.Order
{
    public class Order : IOrder
    {
        public double SubTotal { get { return ScanLog.SubTotal; } }

        public ISkuManager Skus
        {
            get
            {
                if (_skus == null)
                    _skus = new SkuManager();

                return _skus;
            }

            set { _skus = value; }
        }

        private ISkuManager _skus;

        public IScanLog ScanLog
        {
            get
            {
                if (_scanLog is null)
                    _scanLog = new ScanLog();

                return _scanLog;
            }
            set {_scanLog = value; }
        }

        IScanLog _scanLog;

        public void ScanItem(string sku)
        {

            if (ValidateScan(sku))
            {
                UpdateSubTotal(sku);
            }

        }

        public void ScanItem(string sku, double qty)
        {

            if (ValidateScan(sku, qty))
            {
                UpdateSubTotal(sku, qty);
            }

        }

        protected void UpdateSubTotal(string sku, double qty = double.NaN)
        {
            var price = GetUnitPrice(sku);
            double scanQty = 1;
            
            if (double.IsNaN(qty).Equals(false))
            {
                scanQty = qty;
                price = price * scanQty;
            }

            var skuSpecial = Skus.GetSpecial(sku);

            if (skuSpecial != null && skuSpecial.Amount != -.01)
            {
                var itemsScanned = ScanLog.GetScansForSku(sku).Count();
                
                if (itemsScanned > 0 && itemsScanned % skuSpecial.TriggerQuantity == 0)
                {
                    var discountAsDecimal = skuSpecial.Amount / 100;

                    var discount = price * discountAsDecimal;

                    price = price - discount;
                }
            }

            ScanLog.LogScan(sku, price, qty);

        }

        protected double GetUnitPrice(string sku)
        {
            var price = Skus.GetPrice(sku);

            var markdown = Skus.GetMarkdown(sku);

            return price - markdown;
        }

        protected bool ValidateScan(string skuId, double qty = Double.NaN)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(skuId))
                isValid = false;

            if (double.IsNaN(qty).Equals(false))
            {
                if (qty < 0)
                    throw new InvalidWeightException();

                if (qty.Equals(0))
                    isValid = false;
            }

            return isValid;
        }
    }
}
