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
        protected ICashier CreateCashier()
        {
            return new Cashier();
        }

        #region AddSku

        [Fact]
        public void Cashier_ValidSkuPerPound_ReturnsTrue()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSku("Tater Tots", .79, false);

            result.ShouldBe(true);
        }

        [Fact]
        public void Cashier_ValidSkuPerEach_ReturnsTrue()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSku("Ketchup", 1.89, true);

            result.ShouldBe(true);
        }

        [Fact]
        public void Cashier_MissingSkuName_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSku(string.Empty, 1.75, true);

            result.ShouldBe(false);
        }

        [Fact]
        public void Cashier_NegativePrice_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSku("Tartar Sauce", -1.95, false);

            result.ShouldBe(false);
        }

        #endregion

        #region AddMarkdown

        [Fact]
        public void AddMarkdown_ValidMarkdown_ReturnsTrue()
        {
            var testClass = CreateCashier();

            var result = testClass.AddMarkdown("tater tots", .25);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddMarkdown_MissingSku_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddMarkdown(string.Empty, .65);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddMarkdown_NegativeAmount_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddMarkdown("calamari", -.85);

            result.ShouldBe(false);
        }

        #endregion

        #region AddSpecialPercentOff

        [Fact]
        public void AddSpecialPercentOff_ValidSpecialNoLimit_ReturnsTrue()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialPercentOff("french fries", 10, 50);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecialPercentOff_ValidSpecialLimit_ReturnsTrue()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialPercentOff("taco shells", 4, 100, 10);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecialPercentOff_MissingSku_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialPercentOff(string.Empty, 4, 100);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecialPercentOff_NoTriggerQuantity_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialPercentOff("carrots", 0, 100, 10);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecialPercentOff_NoAmountOff_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialPercentOff("cabbage", 2, 0);

            result.ShouldBe(false);
        }

        #endregion

        #region AddSpecialSetPrice

        [Fact]
        public void AddSpecialSetPrice_ValidSpecialNoLimit_ReturnsTrue()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialSetPrice("french bread", 2, 1.75);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecialSetPrice_ValidSpecialLimit_ReturnsTrue()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialSetPrice("taco shells", 4, .25, 10);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecialSetPrice_MissingSku_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialSetPrice(string.Empty, 4, 100);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecialSetPrice_NoTriggerQuantity_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialSetPrice("palmolive", 0, 100, 10);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecialSetPrice_NoPrice_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialPercentOff("lifebouy", 2, 0);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecialSetPrice_NegativePrice_ReturnsFalse()
        {
            var testClass = CreateCashier();

            var result = testClass.AddSpecialPercentOff("ivory", 2, -5);

            result.ShouldBe(false);
        }

        #endregion
    }
}
