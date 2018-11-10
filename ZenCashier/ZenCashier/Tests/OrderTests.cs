using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using ZenCashier.Domain.Order;
using static ZenCashier.Tests.TestValues;
using ZenCashier.Exceptions;
using NSubstitute;

namespace ZenCashier.Tests
{
    public class OrderTests
    {
        protected IOrder CreateOrder_MockSkuApi()
        {
            var mockSkuApi = Substitute.For<ISkuManager>();
            mockSkuApi.GetPrice(SKU_ONE).Returns(PRICE_ONE);
            mockSkuApi.GetPrice(SKU_TWO).Returns(PRICE_TWO);
            mockSkuApi.GetMarkdown(SKU_TWO).Returns(MARKDOWN_TWO);

            var order = new Order
            {
                Skus = mockSkuApi
            };

            return order;
        }

        #region Sku-only

        [Fact]
        public void ScanItem_ValidSkuOnly_SubtotalEqualsPrice()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_ONE);

            testClass.SubTotal.ShouldBe(PRICE_ONE);
        }

        [Fact]
        public void ScanItem_InvalidSkuOnly_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(string.Empty);

            testClass.SubTotal.ShouldBe(0);
        }

        #endregion

        #region With Weight
        
        [Fact]
        public void ScanItem_ValidSkuAndWeight_SubtotalEqualsPriceTimesWeight()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_ONE, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(1.78);
        }

        [Fact]
        public void ScanItem_InvalidSkuValidWeight_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(string.Empty, WEIGHT_TWO);

            testClass.SubTotal.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidSkuZeroWeight_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_TWO, WEIGHT_ZERO);

            testClass.SubTotal.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidSkuNegativeWeight_ShouldThrowException()
        {
            var testClass = CreateOrder_MockSkuApi();

            Should.Throw<InvalidWeightException>(() => testClass.ScanItem(SKU_THREE, WEIGHT_NEGATIVE));
        }

        #endregion

        #region Markdown Tests

        [Fact]
        public void ScanItem_ValidEachSkuWithMarkdown_SubtotalEqualsTwo()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_TWO);

            testClass.SubTotal.ShouldBe(PRICE_EACH_MARKDOWN);
        }

        [Fact]
        public void ScanItem_ValidQtySkuWithMarkdown_SubtotalEqualsOneThirtyThree()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_ONE, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(PRICE_QTY_MARKDOWN);
        }

        #endregion
    }
}
