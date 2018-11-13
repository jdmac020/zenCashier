using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Order.Models
{
    public class ScannedItemModel
    {
        public string SkuId { get; set; }
        public double ScannedQuantity { get; set; }
        public double ScannedPrice { get; set; }
    }
}
