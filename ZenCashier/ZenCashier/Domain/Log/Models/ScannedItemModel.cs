using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Log
{
    public class ScannedItemModel
    {
        public string SkuId { get; set; }
        public double SalePrice { get; set; }
        public double QuantityScanned { get; set; }
    }
}
