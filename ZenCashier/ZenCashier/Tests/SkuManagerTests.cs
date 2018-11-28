using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using static ZenCashier.Tests.TestValues;
using ZenCashier.Domain.Skus.Models;

namespace ZenCashier.Tests
{
    public class SkuManagerTests
    {
        
        protected ISkuManager CreateSkuManager()
        {
            return new SkuManager();
        }

        protected ISkuManager CreateSkuManager_PriceSeeded()
        {
            return new SkuManager
            {
                PriceList = new Dictionary<string, double> { { SKU_ONE, PRICE_ONE} }
            };
        }

        protected ISkuManager CreateSkuManager_MarkdownSeeded()
        {
            return new SkuManager
            {
                MarkdownList = new Dictionary<string, double> { { SKU_ONE, MARKDOWN_ONE } }
            };
        }

        protected ISkuManager CreateSkuManager_SpecialSeeded()
        {
            return new SkuManager
            {
                SpecialList = new List<SpecialInfoModel> { new SpecialInfoModel { Sku = SKU_THREE, Amount = 100} }
            };
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

            var result = testClass.AddSpecial(SKU_THREE, 2, SPECIAL_X_FOR_THREE, false, false);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecial_ValidSpecialLimit_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_THREE, 4, SPECIAL_BOGO_FREE, true, true, 10);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecial_MissingSku_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(string.Empty, 4, SPECIAL_BOGO_HALF, false, true);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NoTriggerQuantity_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_THREE, 0, SPECIAL_BOGO_FREE, true, true, 10);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NoPrice_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_TWO, 2, 0, true, false);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecialSetPrice_NegativePrice_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_ONE, 2, SPECIAL_X_FOR_NEGATIVE, false, false);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NegativeLimit_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(SKU_THREE, 2, SPECIAL_X_FOR_THREE, false, false, -2);

            result.ShouldBe(false);
        }

        #endregion

        #region GetPrice
        
        [Fact]
        public void GetPrice_ValidSkuId_ReturnsPriceOne()
        {
            var testClass = CreateSkuManager_PriceSeeded();

            var result = testClass.GetPrice(SKU_ONE);

            result.ShouldBe(PRICE_ONE);
        }

        [Fact]
        public void GetPrice_InvalidSku_ReturnsNegativePenny()
        {
            var testClass = CreateSkuManager_PriceSeeded();

            var result = testClass.GetPrice(string.Empty);

            result.ShouldBe(-.01);
        }

        #endregion

        #region GetMarkdown

        [Fact]
        public void GetMarkdown_ValidSkuId_ReturnsPriceOne()
        {
            var testClass = CreateSkuManager_MarkdownSeeded();

            var result = testClass.GetMarkdown(SKU_ONE);

            result.ShouldBe(MARKDOWN_ONE);
        }

        [Fact]
        public void GetMarkdown_InvalidSku_ReturnsNegativePenny()
        {
            var testClass = CreateSkuManager_MarkdownSeeded();

            var result = testClass.GetMarkdown(string.Empty);

            result.ShouldBe(-.01);
        }

        #endregion

        #region GetSpecial

        [Fact]
        public void GetSpecial_ValidSkuId_ReturnsPriceOne()
        {
            var testClass = CreateSkuManager_SpecialSeeded();

            var result = testClass.GetSpecial(SKU_THREE);

            result.Amount.ShouldBe(100);
        }

        [Fact]
        public void GetSpecial_InvalidSku_ReturnsEmptySpecial()
        {
            var testClass = CreateSkuManager_SpecialSeeded();

            var result = testClass.GetSpecial(string.Empty);

            result.Amount.ShouldBe(0);
        }

        #endregion
    }
}
