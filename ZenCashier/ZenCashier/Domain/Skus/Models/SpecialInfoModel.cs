using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Skus.Models
{
    public class SpecialInfoModel
    {
        public string Sku { get; set; }
        public double Amount { get; set; }
        public double PercentAmount { get { return GetPercentAmount(); } }
        public double TriggerQuantity { get; set; }
        public bool IsPercentOff { get; set; }
        public bool NeedsEqualOrLesserPurchase { get; set; }
        public int LimitQuantity { get; set; }

        protected double GetPercentAmount()
        {
            return Amount / 100;
        }
    }
}
