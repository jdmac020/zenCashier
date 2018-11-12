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
        #region Factory

        protected IOrder CreateOrder_MockSkuApi_Specials()
        {
            var mockSkuApi = Substitute.For<ISkuManager>();
            mockSkuApi.GetPrice(SKU_ONE).Returns(PRICE_ONE);
            mockSkuApi.GetSpecial(SKU_ONE).Returns(new Domain.Skus.Models.SpecialInfoModel
            {
                Amount = SPECIAL_ONE_PRICE,
                TriggerQuantity = 3,
                IsPercentOff = false,
                LimitQuantity = 8
            });

            mockSkuApi.GetPrice(SKU_TWO).Returns(PRICE_TWO);

            mockSkuApi.GetPrice(SKU_THREE).Returns(PRICE_THREE);
            mockSkuApi.GetSpecial(SKU_THREE).Returns(new Domain.Skus.Models.SpecialInfoModel
            {
                Amount = SPECIAL_BOGO_FREE,
                TriggerQuantity = 3,
                IsPercentOff = true,
                LimitQuantity = 12
            });

            mockSkuApi.GetPrice(SKU_FOUR).Returns(PRICE_FOUR);
            mockSkuApi.GetSpecial(SKU_FOUR).Returns(new Domain.Skus.Models.SpecialInfoModel
            {
                Amount = SPECIAL_BOGO_HALF,
                TriggerQuantity = 2,
                IsPercentOff = true
            });

            var order = new Order
            {
                Skus = mockSkuApi
            };

            return order;
        }

        protected IOrder CreateOrder_MockSkuApi_Markdowns()
        {
            var mockSkuApi = Substitute.For<ISkuManager>();
            mockSkuApi.GetPrice(SKU_ONE).Returns(PRICE_ONE);
            mockSkuApi.GetPrice(SKU_TWO).Returns(PRICE_TWO);
            mockSkuApi.GetPrice(SKU_THREE).Returns(PRICE_THREE);
            mockSkuApi.GetMarkdown(SKU_TWO).Returns(MARKDOWN_TWO);


            var order = new Order
            {
                Skus = mockSkuApi
            };

            return order;
        }

        protected IOrder CreateOrder_MockSkuApi_PriceOnly()
        {
            var mockSkuApi = Substitute.For<ISkuManager>();

            mockSkuApi.GetPrice(SKU_ONE).Returns(PRICE_ONE);
            mockSkuApi.GetPrice(SKU_TWO).Returns(PRICE_TWO);
            mockSkuApi.GetPrice(SKU_THREE).Returns(PRICE_THREE);

            var order = new Order
            {
                Skus = mockSkuApi
            };

            return order;
        }

        #endregion

        #region Scan simple price Sku-only

        [Fact]
        public void ScanItem_ValidEachSku_SubtotalEqualsPrice()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.ScanItem(SKU_ONE);

            testClass.SubTotal.ShouldBe(PRICE_ONE);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(1);
            
        }

        [Fact]
        public void ScanItem_InvalidEachSku_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.ScanItem(string.Empty);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidEachSkuFourScans_SubtotalEqualsPrice4x()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(PRICE_ONE * timesToExecute);
            testClass.ScanLog.Count.ShouldBe(4);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(4);
        }

        [Fact]
        public void ScanItem_TwoValidEachSkusSingleScans_SubtotalEqualsSumOfBothPrices()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

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
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.ScanItem(SKU_ONE, WEIGHT_ONE);
            
            testClass.SubTotal.ShouldBe(1.78);
            testClass.ScanLog.Count.ShouldBe(1);
        }

        [Fact]
        public void ScanItem_InvalidSkuValidWeight_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.ScanItem(string.Empty, WEIGHT_TWO);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidSkuZeroWeight_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.ScanItem(SKU_TWO, WEIGHT_ZERO);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidSkuNegativeWeight_ShouldThrowException()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            Should.Throw<InvalidWeightException>(() => testClass.ScanItem(SKU_THREE, WEIGHT_NEGATIVE));
        }

        [Fact]
        public void ScanItem_TwoValidSkuValidWeight_SubtotalEqualsWeightTimesPriceLogShowsTwoSkus()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();
            var firstExpectedPriceValue = PRICE_TWO * WEIGHT_TWO;
            var secondExpectedPriceValue = PRICE_ONE * WEIGHT_ONE;
            var expectedSubtotal = Math.Round(firstExpectedPriceValue + secondExpectedPriceValue, 2);

            testClass.ScanItem(SKU_TWO, WEIGHT_TWO);
            testClass.ScanItem(SKU_ONE, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(expectedSubtotal);
            testClass.ScanLog.Count.ShouldBe(2);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).FirstOrDefault().ScannedQuantity.ShouldBe(WEIGHT_TWO);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).FirstOrDefault().ScannedQuantity.ShouldBe(WEIGHT_ONE);
        }

        #endregion

        #region Markdown Tests

        [Fact]
        public void ScanItem_ValidEachSkuWithMarkdown_SubtotalEqualsTwo()
        {
            var testClass = CreateOrder_MockSkuApi_Markdowns();

            testClass.ScanItem(SKU_TWO);

            testClass.SubTotal.ShouldBe(PRICE_EACH_MARKDOWN);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).Count().ShouldBe(1);
        }

        [Fact]
        public void ScanItem_ValidQtySkuWithMarkdown_SubtotalEqualsOneThirtyThree()
        {
            var testClass = CreateOrder_MockSkuApi_Markdowns();

            testClass.ScanItem(SKU_TWO, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(PRICE_QTY_MARKDOWN);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).FirstOrDefault().ScannedQuantity.ShouldBe(WEIGHT_ONE);
        }

        #endregion

        #region Special Tests -- Buy One Get One

        [Fact]
        public void ScanItem_BuyFourGetOneFree_SubtotalEqualsPrice3x()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(PRICE_THREE * 3);
            testClass.ScanLog.Count.ShouldBe(4);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_THREE)).Count().ShouldBe(4);
        }

        [Fact]
        public void ScanItem_BuyFourGetOneFreeTwoScanned_SubtotalEqualsPrice3x()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 2;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(PRICE_THREE * 2);
            testClass.ScanLog.Count.ShouldBe(2);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_THREE)).Count().ShouldBe(2);
        }

        [Fact]
        public void ScanItem_BuyTwoGetOneHalfOff_SubtotalEquals2fullPriceOneHalf()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedSubtotal = Math.Round((PRICE_FOUR * 2) + (PRICE_FOUR * .5),2);

            var timesToExecute = 3;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_FOUR);
            }

            testClass.SubTotal.ShouldBe(expectedSubtotal);
            testClass.ScanLog.Count.ShouldBe(3);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_FOUR)).Count().ShouldBe(3);
        }

        [Fact]
        public void ScanItem_BuyThreeNoSpecial_SubtotalEqualsPrice3x()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 3;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_TWO);
            }

            testClass.SubTotal.ShouldBe(PRICE_TWO * timesToExecute);
            testClass.ScanLog.Count.ShouldBe(3);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).Count().ShouldBe(3);
        }

        #endregion

        #region Special Tests -- Buy X for Y

        [Fact]
        public void ScanItem_BuyFourForTwoFiftyFourScans_SubTotalEquals250()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(SPECIAL_ONE_PRICE);
            testClass.ScanLog.Count.ShouldBe(4);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(4);
        }

        [Fact]
        public void ScanItem_BuyFourForTwoFiftyTwoScans_SubTotalEqualsPrice2x()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 2;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(PRICE_ONE * timesToExecute);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void ScanItem_BuyFourForTwoFiftyFiveScans_SubTotalEqualsSpecialPlusPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = SPECIAL_ONE_PRICE + PRICE_ONE;

            var timesToExecute = 5;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void ScanItem_BuyFourForTwoFiftyTwoSkus_SubTotalEqualsSpecialPlusOtherPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = SPECIAL_ONE_PRICE + PRICE_TWO;

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.ScanItem(SKU_TWO);

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(5);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).Count().ShouldBe(1);
        }

        #endregion

        #region Limit Specials

        [Fact]
        public void ScanItem_SpecialLimit8Scan8_SubTotalEquals2xSpecialPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = (SPECIAL_ONE_PRICE * 2);

            var timesToExecute = 8;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void ScanItem_SpecialLimit8Scan12_SubTotalEquals2xSpecialPricePlus4xRegular()
        
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = (SPECIAL_ONE_PRICE * 2) + (PRICE_ONE * 4);

            var timesToExecute = 12;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void ScanItem_BogoSpecialLimit12Scan12_SubTotalEquals8xRegularPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = PRICE_THREE * 8;

            var timesToExecute = 12;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_THREE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void ScanItem_BogoSpecialLimit12Scan16_SubTotalEquals12xRegularPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = PRICE_THREE * 12;

            var timesToExecute = 14;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.ScanItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_THREE)).Count().ShouldBe(timesToExecute);
        }

        #endregion
    }
}
