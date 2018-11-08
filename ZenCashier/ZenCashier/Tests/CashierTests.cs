using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace ZenCashier.Tests
{
    public class CashierTests
    {
        protected ICashier CreateTestClass()
        {
            return new Cashier();
        }

        [Fact]
        public void Cashier_ValidSkuPerPound_ReturnsTrue()
        {
            var testClass = CreateTestClass();

            var result = testClass.AddSku("Tater Tots", .79, false);

            result.ShouldBe(true);
        }

        [Fact]
        public void Cashier_ValidSkuPerEach_ReturnsTrue()
        {
            var testClass = CreateTestClass();

            var result = testClass.AddSku("Ketchup", 1.89, true);

            result.ShouldBe(true);
        }

        [Fact]
        public void Cashier_MissingSkuName_ReturnsFalse()
        {
            var testClass = CreateTestClass();

            var result = testClass.AddSku(string.Empty, 1.75, true);

            result.ShouldBe(false);
        }

        [Fact]
        public void Cashier_NegativePrice_ReturnsFalse()
        {
            var testClass = CreateTestClass();

            var result = testClass.AddSku("Tartar Sauce", -1.95, false);

            result.ShouldBe(false);
        }
    }
}
