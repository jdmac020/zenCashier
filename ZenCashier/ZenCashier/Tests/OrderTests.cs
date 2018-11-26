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
using ZenCashier.Domain.Order.Models;

namespace ZenCashier.Tests
{
    public class OrderTests
    {
        #region Factory
        
        protected List<ScannedItemModel> Create_ScannedItems_ThreeSingleSkuThrees()
        {
            return new List<ScannedItemModel>
            {
                new ScannedItemModel { SkuId = SKU_THREE, ScannedPrice = PRICE_THREE, ScannedQuantity = 1 },
                new ScannedItemModel { SkuId = SKU_THREE, ScannedPrice = PRICE_THREE, ScannedQuantity = 1 },
                new ScannedItemModel { SkuId = SKU_THREE, ScannedPrice = PRICE_THREE, ScannedQuantity = 1 }
            };
        }

        protected List<ScannedItemModel> Create_ScannedItems_ThreeWeightedSkuThrees()
        {
            return new List<ScannedItemModel>
            {
                new ScannedItemModel { SkuId = SKU_THREE, ScannedPrice = PRICE_THREE, ScannedQuantity = 2.25 },
                new ScannedItemModel { SkuId = SKU_THREE, ScannedPrice = PRICE_THREE, ScannedQuantity = 2.09 },
                new ScannedItemModel { SkuId = SKU_THREE, ScannedPrice = PRICE_THREE, ScannedQuantity = 1.98 }
            };
        }

        protected List<ScannedItemModel> Create_ScannedItems_ThreeSingleSkuOne_ForGetXforY()
        {
            return new List<ScannedItemModel>
            {
                new ScannedItemModel { SkuId = SKU_ONE, ScannedPrice = PRICE_ONE, ScannedQuantity = 1 },
                new ScannedItemModel { SkuId = SKU_ONE, ScannedPrice = PRICE_ONE, ScannedQuantity = 1 },
                new ScannedItemModel { SkuId = SKU_ONE, ScannedPrice = .92, ScannedQuantity = 1 }
            };
        }

        protected List<ScannedItemModel> Create_ScannedItems_ThreeSingleSkuTwo_ForBogo()
        {
            return new List<ScannedItemModel>
            {
                new ScannedItemModel { SkuId = SKU_TWO, ScannedPrice = PRICE_TWO, ScannedQuantity = 1 },
                new ScannedItemModel { SkuId = SKU_TWO, ScannedPrice = PRICE_TWO, ScannedQuantity = 1 },
                new ScannedItemModel { SkuId = SKU_TWO, ScannedPrice = 0, ScannedQuantity = 1 }
            };
        }

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
            mockSkuApi.GetSpecial(SKU_TWO).Returns(new Domain.Skus.Models.SpecialInfoModel
            {
                Amount = SPECIAL_BOGO_FREE,
                TriggerQuantity = 2,
                IsPercentOff = true,
                LimitQuantity = 6
            });

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

        protected IOrder CreateOrder_MockSkuApi_EqualOrLesserSpecials()
        {
            var mockSkuApi = Substitute.For<ISkuManager>();
            mockSkuApi.GetPrice(SKU_ONE).Returns(PRICE_ONE);
            mockSkuApi.GetSpecial(SKU_ONE).Returns(new Domain.Skus.Models.SpecialInfoModel
            {
                Amount = SPECIAL_20_PERCENT_OFF,
                TriggerQuantity = 3,
                IsPercentOff = false,
                NeedsEqualOrGreaterPurchase = true,
                LimitQuantity = 8
            });

            mockSkuApi.GetPrice(SKU_TWO).Returns(PRICE_TWO);
            mockSkuApi.GetSpecial(SKU_TWO).Returns(new Domain.Skus.Models.SpecialInfoModel
            {
                Amount = SPECIAL_BOGO_FREE,
                TriggerQuantity = 2,
                IsPercentOff = true,
                NeedsEqualOrGreaterPurchase = true,
                LimitQuantity = 6
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

        #region ScanItem

        [Fact]
        public void ScanItem_InvalidEachSku_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.ScanItem(string.Empty, false);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_InvalidSkuValidWeight_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.ScanItem(string.Empty, WEIGHT_TWO, false);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
        }
        
        [Fact]
        public void ScanItem_ValidSkuNegativeWeight_ShouldThrowException()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            Should.Throw<InvalidWeightException>(() => testClass.ScanItem(SKU_THREE, WEIGHT_NEGATIVE, false));
        }

        #endregion


        #region Add simple price Sku-only

        [Fact]
        public void AddItem_ValidEachSku_SubtotalEqualsPrice()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.AddItem(SKU_ONE);

            testClass.SubTotal.ShouldBe(PRICE_ONE);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(1);

        }
        
        [Fact]
        public void AddItem_ValidEachSkuFourScans_SubtotalEqualsPrice4x()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(PRICE_ONE * timesToExecute);
            testClass.ScanLog.Count.ShouldBe(4);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(4);
        }

        [Fact]
        public void AddItem_TwoValidEachSkusSingleScans_SubtotalEqualsSumOfBothPrices()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.AddItem(SKU_ONE);
            testClass.AddItem(SKU_TWO);

            testClass.SubTotal.ShouldBe(PRICE_ONE + PRICE_TWO);
            testClass.ScanLog.Count.ShouldBe(2);
        }

        [Fact]
        public void ScanItem_ValidSkuZeroWeight_SubtotalEqualsZero()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.ScanItem(SKU_TWO, WEIGHT_ZERO, false);

            testClass.SubTotal.ShouldBe(0);
            testClass.ScanLog.Count.ShouldBe(0);
        }
        
        #endregion

        #region Add simple price With Weight

        [Fact]
        public void AddItem_ValidSkuAndWeight_SubtotalEqualsPriceTimesWeight()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            testClass.AddItem(SKU_ONE, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(1.78);
            testClass.ScanLog.Count.ShouldBe(1);
        }
        
        [Fact]
        public void AddItem_TwoValidSkuValidWeight_SubtotalEqualsWeightTimesPriceLogShowsTwoSkus()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();
            var firstExpectedPriceValue = PRICE_TWO * WEIGHT_TWO;
            var secondExpectedPriceValue = PRICE_ONE * WEIGHT_ONE;
            var expectedSubtotal = Math.Round(firstExpectedPriceValue + secondExpectedPriceValue, 2);

            testClass.AddItem(SKU_TWO, WEIGHT_TWO);
            testClass.AddItem(SKU_ONE, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(expectedSubtotal);
            testClass.ScanLog.Count.ShouldBe(2);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).FirstOrDefault().ScannedQuantity.ShouldBe(WEIGHT_TWO);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).FirstOrDefault().ScannedQuantity.ShouldBe(WEIGHT_ONE);
        }

        #endregion

        #region Markdown Tests

        [Fact]
        public void AddItem_ValidEachSkuWithMarkdown_SubtotalEqualsTwo()
        {
            var testClass = CreateOrder_MockSkuApi_Markdowns();

            testClass.AddItem(SKU_TWO);

            testClass.SubTotal.ShouldBe(PRICE_EACH_MARKDOWN);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).Count().ShouldBe(1);
        }

        [Fact]
        public void AddItem_ValidQtySkuWithMarkdown_SubtotalEqualsOneThirtyThree()
        {
            var testClass = CreateOrder_MockSkuApi_Markdowns();

            testClass.AddItem(SKU_TWO, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(PRICE_QTY_MARKDOWN);
            testClass.ScanLog.Count.ShouldBe(1);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).FirstOrDefault().ScannedQuantity.ShouldBe(WEIGHT_ONE);
        }

        #endregion

        #region Special Tests -- Buy One Get One

        [Fact]
        public void AddItem_BuyFourGetOneFree_SubtotalEqualsPrice3x()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(PRICE_THREE * 3);
            testClass.ScanLog.Count.ShouldBe(4);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_THREE)).Count().ShouldBe(4);
        }

        [Fact]
        public void AddItem_BuyFourGetOneFreeTwoScanned_SubtotalEqualsPrice3x()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 2;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(PRICE_THREE * 2);
            testClass.ScanLog.Count.ShouldBe(2);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_THREE)).Count().ShouldBe(2);
        }

        [Fact]
        public void AddItem_BuyTwoGetOneHalfOff_SubtotalEquals2fullPriceOneHalf()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedSubtotal = Math.Round((PRICE_FOUR * 2) + (PRICE_FOUR * .5), 2);

            var timesToExecute = 3;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_FOUR);
            }

            testClass.SubTotal.ShouldBe(expectedSubtotal);
            testClass.ScanLog.Count.ShouldBe(3);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_FOUR)).Count().ShouldBe(3);
        }

        [Fact]
        public void AddItem_BuyThreeNoSpecial_SubtotalEqualsPrice3x()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();

            var timesToExecute = 3;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_TWO);
            }

            testClass.SubTotal.ShouldBe(PRICE_TWO * timesToExecute);
            testClass.ScanLog.Count.ShouldBe(3);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).Count().ShouldBe(3);
        }

        #endregion

        #region Special Tests -- Buy X for Y

        [Fact]
        public void AddItem_BuyFourForTwoFiftyFourScans_SubTotalEquals250()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(SPECIAL_ONE_PRICE);
            testClass.ScanLog.Count.ShouldBe(4);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(4);
        }

        [Fact]
        public void AddItem_BuyFourForTwoFiftyTwoScans_SubTotalEqualsPrice2x()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();

            var timesToExecute = 2;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(PRICE_ONE * timesToExecute);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void AddItem_BuyFourForTwoFiftyFiveScans_SubTotalEqualsSpecialPlusPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = SPECIAL_ONE_PRICE + PRICE_ONE;

            var timesToExecute = 5;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void AddItem_BuyFourForTwoFiftyTwoSkus_SubTotalEqualsSpecialPlusOtherPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = SPECIAL_ONE_PRICE + PRICE_TWO;

            var timesToExecute = 4;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_ONE);
            }

            testClass.AddItem(SKU_TWO);

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(5);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).Count().ShouldBe(1);
        }

        #endregion

        #region Limit Specials

        [Fact]
        public void AddItem_SpecialLimit8Scan8_SubTotalEquals2xSpecialPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = (SPECIAL_ONE_PRICE * 2);

            var timesToExecute = 8;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void AddItem_SpecialLimit8Scan12_SubTotalEquals2xSpecialPricePlus4xRegular()

        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = (SPECIAL_ONE_PRICE * 2) + (PRICE_ONE * 4);

            var timesToExecute = 12;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_ONE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_ONE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void AddItem_BogoSpecialLimit12Scan12_SubTotalEquals9xRegularPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = PRICE_THREE * 9;

            var timesToExecute = 12;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_THREE)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void AddItem_BogoSpecialLimit6Scan9_SubTotalEquals9xRegularPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = PRICE_TWO * 7;

            var timesToExecute = 9;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_TWO);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_TWO)).Count().ShouldBe(timesToExecute);
        }

        [Fact]
        public void AddItem_BogoSpecialLimit12Scan14_SubTotalEquals12xRegularPrice()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            var expectedPrice = PRICE_THREE * 11;

            var timesToExecute = 14;

            for (int i = 0; i < timesToExecute; i++)
            {
                testClass.AddItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(timesToExecute);
            testClass.ScanLog.Where(scan => scan.SkuId.Equals(SKU_THREE)).Count().ShouldBe(timesToExecute);
        }

        #endregion

        #region Remove Item

        [Fact]
        public void RemoveItem_ValidEachSku_SubtotalEqualsMinusSkuPrice()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();
            testClass.ScanLog = Create_ScannedItems_ThreeSingleSkuThrees();
            testClass.SubTotal = PRICE_THREE * 3;

            var expectedPrice = PRICE_THREE * 2;

            testClass.RemoveItem(SKU_THREE);

            testClass.ScanLog.Count.ShouldBe(2);
            testClass.SubTotal.ShouldBe(expectedPrice);

        }

        [Fact]
        public void RemoveItem_ValidQuantitySku_SubtotalEqualsMinusSalePrice()
        {
            var testClass = CreateOrder_MockSkuApi_PriceOnly();
            testClass.ScanLog = Create_ScannedItems_ThreeWeightedSkuThrees();
            testClass.SubTotal = (PRICE_THREE * WEIGHT_ONE) + (PRICE_THREE * WEIGHT_TWO) + (PRICE_THREE * WEIGHT_THREE);

            var expectedPrice = (PRICE_THREE * WEIGHT_ONE) + (PRICE_THREE * WEIGHT_TWO);

            testClass.RemoveItem(SKU_THREE, WEIGHT_THREE);

            testClass.ScanLog.Count.ShouldBe(2);
            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void RemoveItem_InvalidatesXforYSpecial_SubtotalEqualsSumOfPrices()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            testClass.ScanLog = Create_ScannedItems_ThreeSingleSkuOne_ForGetXforY();

            var expectedPrice = (PRICE_ONE * 2);
            testClass.SubTotal = SPECIAL_ONE_PRICE;

            testClass.RemoveItem(SKU_ONE);

            testClass.ScanLog.Count.ShouldBe(2);
            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void RemoveItem_InvalidatesBuy2Get1Free_SubtotalDoesNotChange()
        {
            var testClass = CreateOrder_MockSkuApi_Specials();
            testClass.ScanLog = Create_ScannedItems_ThreeSingleSkuTwo_ForBogo();

            var expectedPrice = (PRICE_TWO * 2);
            testClass.SubTotal = expectedPrice;

            testClass.RemoveItem(SKU_TWO);

            testClass.ScanLog.Count.ShouldBe(2);
            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        #endregion

        #region Special Tests -- Weighted Buy X for Y

        [Fact]
        public void AddItem_SkuOneWeightedSpecial_SubTotalEqualsFourFifteen()
        {
            var testClass = CreateOrder_MockSkuApi_EqualOrLesserSpecials();

            var firstScanTotal = PRICE_ONE * 3.25;
            var secondScanTotal = PRICE_ONE * 2.5;
            var discountAmount = secondScanTotal * (SPECIAL_20_PERCENT_OFF / 100);
            var expectedTotal = Math.Round((firstScanTotal + secondScanTotal) - discountAmount, 2);

            testClass.AddItem(SKU_ONE, 3.25);
            testClass.AddItem(SKU_ONE, 2.5);

            testClass.SubTotal.ShouldBe(expectedTotal);
        }

        [Fact]
        public void AddItem_SkuTwoWeightedSpecial_SubTotalEqualsEightTwentyFive()
        {
            var testClass = CreateOrder_MockSkuApi_EqualOrLesserSpecials();

            var firstScanTotal = PRICE_TWO * 2.75;
            var secondScanTotal = PRICE_TWO * 2.25;
            var discountAmount = secondScanTotal;
            var expectedTotal = Math.Round((firstScanTotal + secondScanTotal) - discountAmount, 2);

            testClass.AddItem(SKU_TWO, 2.75);
            testClass.AddItem(SKU_TWO, 2.25);

            testClass.SubTotal.ShouldBe(expectedTotal);
        }

        [Fact]
        public void AddItem_SkuOneWeightedSpecialBelowTrigger_SubTotalEqualsPriceTimesQuantity()
        {
            var testClass = CreateOrder_MockSkuApi_EqualOrLesserSpecials();

            var firstScanTotal = PRICE_ONE * 2.75;
            var secondScanTotal = PRICE_ONE * 1.75;
            var expectedPrice = Math.Round(firstScanTotal + secondScanTotal, 2);

            testClass.AddItem(SKU_ONE, 2.75);
            testClass.AddItem(SKU_ONE, 1.75);

            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void AddItem_SkuTwoWeightedSpecialTriggerAmountSecond_FirstScanZeroed()
        {
            var testClass = CreateOrder_MockSkuApi_EqualOrLesserSpecials();

            var firstScanTotal = PRICE_TWO * 1.75;
            var secondScanTotal = PRICE_TWO * 2.25;
            var discountAmount = firstScanTotal;
            var expectedTotal = Math.Round((firstScanTotal + secondScanTotal) - discountAmount, 2);

            testClass.AddItem(SKU_TWO, 1.75);
            testClass.AddItem(SKU_TWO, 2.25);

            testClass.SubTotal.ShouldBe(expectedTotal);
        }

        #endregion
    }
}
