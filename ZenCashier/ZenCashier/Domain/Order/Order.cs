using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenCashier.Domain.Order.Models;
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

        public List<ScannedItemModel> ScanLog { get; set; } = new List<ScannedItemModel>();

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

                var scannedItems = GetScannedItems(sku).Count();
                var scannedItemsFullPrice = GetScannedItems(sku).Where(item => item.ScannedPrice.Equals(price)).Count();

                if (scannedItems > 0 && (skuSpecial.LimitQuantity == 0 || scannedItems <= skuSpecial.LimitQuantity))
                {
                    if (skuSpecial.IsPercentOff)
                    {

                        if (scannedItemsFullPrice % skuSpecial.TriggerQuantity == 0 && scannedItemsFullPrice / skuSpecial.TriggerQuantity != scannedItems - scannedItemsFullPrice)
                        {
                            var discountAsDecimal = skuSpecial.Amount / 100;

                            var discount = price * discountAsDecimal;

                            price = price - discount;
                        }

                    }
                    else
                    {

                        if (scannedItems % skuSpecial.TriggerQuantity == 0)
                        {
                            var fullPricePaid = skuSpecial.TriggerQuantity * price;

                            price = skuSpecial.Amount - fullPricePaid;
                        }

                    }
                }

                

                // if scanned full price == trigger quantity, do discount

                // PROBLEM: this does not advance count of items forward, meaning all subsequent scans get discounted

                // SOLUTION(?): check that # of scanned items relates to the discount given (eg IF totalItems / triggerQty == 1 AND totalItems - fullPriceItems == 1 THEN normal price



            }

            _subTotal += price;

            LogScannedItem(sku, scanQty, price);

        }

        protected IEnumerable<ScannedItemModel> GetScannedItems(string skuId)
        {
            var scannedItems = ScanLog.Where(item => item.SkuId == skuId);

            if (scannedItems.Any())
            {
                return scannedItems;
            }
            else
            {
                return Enumerable.Empty<ScannedItemModel>();
            }
        }

        protected void LogScannedItem(string skuId, double qty, double price)
        {
            ScanLog.Add(new ScannedItemModel
            {
                SkuId = skuId,
                ScannedQuantity = qty,
                ScannedPrice = price
            });
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
