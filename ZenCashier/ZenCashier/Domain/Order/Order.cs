using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenCashier.Exceptions;

namespace ZenCashier.Domain.Order
{
    public class Order : IOrder
    {
        public double SubTotal { get { return Math.Round(_subTotal, 2); } }

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

        public Dictionary<string, double> ScanLog { get; set; } = new Dictionary<string, double>();

        private ISkuManager _skus;

        private double _subTotal;

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
                var itemsScanned = GetScannedQuantity(sku);
                
                if (itemsScanned > 0 && itemsScanned % skuSpecial.TriggerQuantity == 0 && itemsScanned < skuSpecial.LimitQuantity)
                {

                    if (skuSpecial.IsPercentOff)
                    {
                        var discountAsDecimal = skuSpecial.Amount / 100;

                        var discount = price * discountAsDecimal;

                        price = price - discount;
                    }
                    else
                    {
                        var fullPricePaid = skuSpecial.TriggerQuantity * price;

                        price = skuSpecial.Amount - fullPricePaid;
                    }
                    
                }
            }

            _subTotal += price;

            LogScannedItem(sku, scanQty);

        }

        protected double GetScannedQuantity(string skuId)
        {
            var logRecord = ScanLog.Where(record => record.Key == skuId).FirstOrDefault();

            if (string.IsNullOrEmpty(logRecord.Key))
            {
                return 0;
            }
            else
            {
                return logRecord.Value;
            }
        }

        protected void LogScannedItem(string skuId, double qty)
        {
            var logRecord = ScanLog.Where(record => record.Key == skuId).FirstOrDefault();

            if (string.IsNullOrEmpty(logRecord.Key))
            {
                ScanLog.Add(skuId, qty);
            }
            else
            {
                ScanLog[skuId] += qty;
            }
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
