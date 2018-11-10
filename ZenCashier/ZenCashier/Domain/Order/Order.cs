using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Order
{
    public class Order : IOrder
    {
        public double SubTotal { get { return Math.Round(_subTotal, 2); } }

        private double _subTotal;

        public void ScanItem(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return;

            _subTotal += .79;
        }

        public void ScanItem(string sku, double qty)
        {
            if (string.IsNullOrEmpty(sku))
                return;

            if (qty.Equals(0))
                return;

            _subTotal += 1.78;
        }
    }
}
