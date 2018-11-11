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
            mockSkuApi.GetSpecial(SKU_THREE).Returns(new Domain.Skus.Models.SpecialInfoModel
            {
                Amount = 100
            });

            var order = new Order
            {
                Skus = mockSkuApi
            };

            return order;
        }
        
        #region Scan simple price Sku-only

        [Fact]
        public void ScanItem_ValidEachSku_SubtotalEqualsPrice()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_ONE);

            testClass.SubTotal.ShouldBe(PRICE_ONE);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog[SKU_ONE].ShouldBe(1);
            
        }

        [Fact]
        public void ScanItem_InvalidEachSku_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(string.Empty);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidEachSkuFourScans_SubtotalEqualsPrice4x()
        {
            var testClass = CreateOrder_MockSkuApi();

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(PRICE_ONE * timesToExecute);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog[SKU_ONE].ShouldBe(4);
        }

        [Fact]
        public void ScanItem_TwoValidEachSkusSingleScans_SubtotalEqualsSumOfBothPrices()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_ONE);
            testClass.ScanItem(SKU_TWO);

            testClass.SubTotal.ShouldBe(PRICE_ONE + PRICE_TWO);
            testClass.ScanLog.Count.ShouldBe(2);
        }

        #endregion

        #region Scan simple price With Weight
        
        [Fact]
        public void ScanItem_ValidSkuAndWeight_SubtotalEqualsPriceTimesWeight()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_ONE, WEIGHT_ONE);
            
            testClass.SubTotal.ShouldBe(1.78);
            testClass.ScanLog.Count.ShouldBe(1);
        }

        [Fact]
        public void ScanItem_InvalidSkuValidWeight_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(string.Empty, WEIGHT_TWO);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidSkuZeroWeight_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_TWO, WEIGHT_ZERO);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
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
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog[SKU_TWO].ShouldBe(1);
        }

        [Fact]
        public void ScanItem_ValidQtySkuWithMarkdown_SubtotalEqualsOneThirtyThree()
        {
            var testClass = CreateOrder_MockSkuApi();

            testClass.ScanItem(SKU_TWO, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(PRICE_QTY_MARKDOWN);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog[SKU_TWO].ShouldBe(1);
        }

        #endregion

        #region Special Tests -- Buy One Get One

        [Fact]
        public void ScanItem_ThreeValidEachSkuWithBogo_SubtotalEqualsPrice2x()
        {
            var testClass = CreateOrder_MockSkuApi();

            var timesToExecute = 3;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(PRICE_THREE * 2);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog[SKU_THREE].ShouldBe(3);
        }

        [Fact]
        public void ScanItem_ThreeValidEachSkuNoSpecial_SubtotalEqualsPrice3x()
        {
            var testClass = CreateOrder_MockSkuApi();

            var timesToExecute = 3;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(PRICE_ONE * timesToExecute);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog[SKU_ONE].ShouldBe(3);
        }

        #endregion
    }
}
