﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZenCashier.Domain.Order.Models;
using ZenCashier.Domain.Skus.Models;
using ZenCashier.Exceptions;

namespace ZenCashier.Domain.Order
{
    public class Order : IOrder
    {
        public double SubTotal
        {
            get { return Math.Round(_subTotal, 2); }

            set
            {
                if (_subTotal == 0)
                    _subTotal = value;
            }
        }

        private double _subTotal;

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

        public void ScanItem(string sku, bool removeItem = false)
        {

            if (ValidateScan(sku))
            {

                if (removeItem)
                {
                    RemoveItem(sku);
                }
                else
                {
                    AddItem(sku);
                }

            }

        }

        public void ScanItem(string sku, double qty, bool removeItem = false)
        {

            if (ValidateScan(sku, qty))
            {

                if (removeItem)
                {
                    RemoveItem(sku, qty);
                }
                else
                {
                    AddItem(sku, qty);
                }
            }

        }

        public void AddItem(string sku, double qty = double.NaN)
        {
            var scanQty = SetQuantity(qty);

            var price = GetSalePriceForItem(sku, scanQty);

            _subTotal += price;

            LogScannedItem(sku, scanQty, price);

        }

        public void RemoveItem(string sku, double qty = double.NaN)
        {
            var scannedQty = SetQuantity(qty);

            var loggedItem = GetScannedItems(sku, qty).LastOrDefault();

            if (loggedItem != null)
            {
                var amountToDebit = loggedItem.ScannedPrice;
                
                _subTotal -= amountToDebit;

                ScanLog.Remove(loggedItem);
            }
        }

        protected double SetQuantity(double qty)
        {
            if (Double.IsNaN(qty))
                return 1;

            return qty;
        }

        protected double GetSalePriceForItem(string sku, double qty)
        {
            var price = GetUnitPrice(sku) * qty;

            var skuSpecial = Skus.GetSpecial(sku);

            if (skuSpecial != null && skuSpecial.Amount != -.01)
            {

                if (skuSpecial.NeedsEqualOrLesserPurchase)
                {
                    price = ProcessEqualOrLesserSpecial(price, qty, sku, skuSpecial);
                }
                else
                {
                    price = ProcessForEachSpecial(price, sku, skuSpecial);
                }

            }

            return price;
        }

        protected double ProcessEqualOrLesserSpecial(double currentValue, double qty, string sku, SpecialInfoModel special)
        {
            var lastScanned = GetScannedItems(sku).LastOrDefault();

            if (lastScanned is null)
                return currentValue;

            if (qty < special.TriggerQuantity && lastScanned.ScannedQuantity < special.TriggerQuantity)
                return currentValue;

            if (special.IsPercentOff)
            {
                if (currentValue <= lastScanned.ScannedPrice)
                {
                    return currentValue - (currentValue * special.PercentAmount);
                }
                else if (currentValue > lastScanned.ScannedPrice)
                {
                    return currentValue - (lastScanned.ScannedPrice * special.PercentAmount);
                }
            }

            return currentValue - special.Amount;

        }

        protected double ProcessForEachSpecial(double price, string sku, SpecialInfoModel skuSpecial)
        {
            var scannedItems = GetScannedItems(sku).Count();
            var scannedItemsFullPrice = GetScannedItems(sku).Where(item => item.ScannedPrice.Equals(price)).Count();

            if (scannedItems > 0 && (skuSpecial.LimitQuantity == 0 || scannedItems <= skuSpecial.LimitQuantity))
            {
                if (skuSpecial.IsPercentOff)
                {

                    if (scannedItemsFullPrice % skuSpecial.TriggerQuantity == 0 &&
                        (scannedItemsFullPrice / skuSpecial.TriggerQuantity) != (scannedItems - scannedItemsFullPrice))
                    {
                        var discount = price * skuSpecial.PercentAmount;

                        return price - discount;
                    }

                }
                else
                {

                    if (scannedItems % skuSpecial.TriggerQuantity == 0)
                    {
                        var fullPricePaid = skuSpecial.TriggerQuantity * price;

                        return skuSpecial.Amount - fullPricePaid;
                    }

                }
            }

            return price;
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

        protected IEnumerable<ScannedItemModel> GetScannedItems(string skuId, double qty = double.NaN)
        {
            var quantity = SetQuantity(qty);

            var scannedItems = ScanLog.Where(item => item.SkuId == skuId && item.ScannedQuantity.Equals(quantity));

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
