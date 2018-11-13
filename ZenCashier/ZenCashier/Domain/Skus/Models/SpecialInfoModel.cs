using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Skus.Models
{
    public class SpecialInfoModel
    {
        public double Amount { get; set; }
        public int TriggerQuantity { get; set; }
        public bool IsPercentOff { get; set; }
        public int LimitQuantity { get; set; }
    }
}
