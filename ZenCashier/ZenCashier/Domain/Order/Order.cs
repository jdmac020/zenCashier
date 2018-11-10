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

        private double _subTotal;

        public void ScanItem(string sku)
        {

            if (ValidateScan(sku))
            {
                _subTotal += .79;
            }
            
        }

        public void ScanItem(string sku, double qty)
        {

            if (ValidateScan(sku, qty))
            {
                _subTotal += 1.78;
            }
            
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
