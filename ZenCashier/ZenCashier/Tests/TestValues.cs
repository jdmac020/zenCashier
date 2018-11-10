using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Tests
{
    public static class TestValues
    {
        public const string SKU_ONE = "tater tots";
        public const string SKU_TWO = "ketchup";
        public const string SKU_THREE = "palmolive";

        public const double WEIGHT_ONE = 2.25;
        public const double WEIGHT_TWO = 1;
        public const double WEIGHT_ZERO = 0;
        public const double WEIGHT_NEGATIVE = -1.5;
        
        public const double PRICE_ONE = .79;
        public const double PRICE_TWO = 3;
        public const double PRICE_NEGATIVE = -.89;
        public const double PRICE_EACH_MARKDOWN = .59;
        public const double PRICE_QTY_MARKDOWN = 1.33;

        public const double MARKDOWN_ONE = .2;

        public const double SPECIAL_BOGO_FREE = 100;
        public const double SPECIAL_BOGO_HALF = 50;
        public const double SPECIAL_X_FOR_THREE = 3;
        public const double SPECIAL_X_FOR_NEGATIVE = -3;
    }
}
