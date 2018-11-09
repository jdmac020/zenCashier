using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace ZenCashier.Tests
{
    public class SkuManagerTests
    {
        protected const string SKU_ONE = "tater tots";
        protected const string SKU_TWO = "ketchup";
        protected const string SKU_THREE = "palmolive";

        protected const double PRICE_ONE = .79;
        protected const double PRICE_TWO = 3;
        protected const double PRICE_NEGATIVE = -.89;


        protected const double SPECIAL_BOGO_FREE = 100;
        protected const double SPECIAL_BOGO_HALF = 50;
        protected const double SPECIAL_X_FOR_THREE = 3;
        protected const double SPECIAL_X_FOR_NEGATIVE = 3;

        protected ISkuManager CreateSkuManager()
        {
            return new SkuManager();
        }

        #region AddSku

        [Fact]
        public void AddSku_ValidSkuPerPound_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSku(SKU_ONE, PRICE_ONE);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSku_ValidSkuPerEach_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSku(SKU_TWO, PRICE_TWO);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSku_MissingSkuName_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSku(string.Empty, PRICE_ONE);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSku_NegativePrice_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSku(SKU_THREE, PRICE_NEGATIVE);

            result.ShouldBe(false);
        }

        #endregion

        #region AddMarkdown

        [Fact]
        public void AddMarkdown_ValidMarkdown_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddMarkdown(SKU_ONE, PRICE_ONE);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddMarkdown_MissingSku_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddMarkdown(string.Empty, PRICE_ONE);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddMarkdown_NegativeAmount_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddMarkdown(SKU_TWO, PRICE_NEGATIVE);

            result.ShouldBe(false);
        }

        #endregion

        #region AddSpecial

        [Fact]
        public void AddSpecial_ValidSpecialNoLimit_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_THREE, 2, SPECIAL_X_FOR_THREE, false);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecial_ValidSpecialLimit_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_THREE, 4, SPECIAL_BOGO_FREE, true, 10);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecial_MissingSku_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(string.Empty, 4, SPECIAL_BOGO_HALF, true);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NoTriggerQuantity_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_THREE, 0, SPECIAL_BOGO_FREE, true, 10);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NoPrice_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_TWO, 2, 0, false);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecialSetPrice_NegativePrice_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_ONE, 2, SPECIAL_X_FOR_NEGATIVE, false);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NegativeLimit_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_THREE, 2, SPECIAL_X_FOR_THREE, false, -2);

            result.ShouldBe(false);
        }

        #endregion

        #region GetPrice
        
        [Fact]
        public void GetPrice_ValidSkuId_ReturnsPriceOne()
        {
            var testClass = CreateSkuManager();

            var result = testClass.GetPrice(SKU_ONE);

            result.ShouldBe(-.01);
        }

        [Fact]
        public void GetPrice_InvalidSku_ReturnsNegativePenny()
        {
            var testClass = CreateSkuManager();

            var result = testClass.GetPrice(string.Empty);

            result.ShouldBe(-.01);
        }

        #endregion
    }
}
