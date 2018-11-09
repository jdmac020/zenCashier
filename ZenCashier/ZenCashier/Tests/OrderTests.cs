using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using ZenCashier.Domain.Order;

namespace ZenCashier.Tests
{
    public class OrderTests
    {
        [Fact]
        public void ScanItem_ValidSkuOnly_SubtotalEqualsPrice()
        {
            IOrder testClass = new Order();

            testClass.ScanItem("tater tots");

            testClass.SubTotal.ShouldBe(.79);
        }

        [Fact]
        public void ScanItem_InvalidSkuOnly_SubtotalEqualsZero()
        {
            IOrder testClass = new Order();

            testClass.ScanItem(string.Empty);

            testClass.SubTotal.ShouldBe(0);
        }
    }
}
