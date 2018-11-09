using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Order
{
    public class Order : IOrder
    {
        public double SubTotal => throw new NotImplementedException();

        public void ScanItem(string sku)
        {
            throw new NotImplementedException();
        }

        public void ScanItem(string sku, double qty)
        {
            throw new NotImplementedException();
        }
    }
}
