﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier
{
    public class Cashier : ICashier
    {
        public bool AddMarkdown(string sku, double amount)
        {
            throw new NotImplementedException();
        }

        public bool AddSku(string id, double price, bool isEaches)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            if (price < 0)
                return false;

            return true;
        }
    }
}
