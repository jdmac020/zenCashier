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

namespace ZenCashier.Tests
{
    public class IntegrationTests
    {

        private static IOrder CreateTestClass()
        {
            var skus = new SkuManager();

            // Add price-only sku (use cases 1 & 2)
            skus.AddSku(SKU_ONE, PRICE_ONE);

            // Add markdown sku (use case 3)
            skus.AddSku(SKU_TWO, PRICE_TWO);
            skus.AddMarkdown(SKU_TWO, MARKDOWN_TWO);

            // Add BOGO sku with limit (use case 4, 6)
            skus.AddSku(SKU_THREE, PRICE_THREE);
            skus.AddSpecial(SKU_THREE, 3, SPECIAL_BOGO_FREE, true, false, 8);
            
            // Add M for N sku (use case 5)
            skus.AddSku(SKU_FOUR, PRICE_FOUR);
            skus.AddSpecial(SKU_FOUR, 2, PRICE_TWO, false, false);

            // Add M for N off equal or lesser value (use case 8)
            skus.AddSku(SKU_FIVE, PRICE_FIVE);
            skus.AddSpecial(SKU_FIVE, .1, SPECIAL_BOGO_HALF, true, true);

            return new Order { Skus = skus };
        }

        [Fact]
        public void UseCaseOne_ValidEachItemAndInvalidEachItem_SubtotalEqualsPriceOne()
        {
            var testClass = CreateTestClass();

            testClass.ScanItem(SKU_ONE);
            testClass.ScanItem(string.Empty);
            testClass.SubTotal.ShouldBe(PRICE_ONE);
        }

        [Fact]
        public void UseCaseTwo_ValidWeightedItemAndOneInvalid_SubtotalEqualsPriceOneTimesWeightOne()
        {
            var testClass = CreateTestClass();
            var expectedTotal = Math.Round(PRICE_ONE * WEIGHT_ONE, 2);

            testClass.ScanItem(SKU_ONE, WEIGHT_ONE);
            testClass.ScanItem(string.Empty, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(expectedTotal);
        }

        [Fact]
        public void UseCaseTwo_InvalidSkuValidWeight_ShouldThrowInvalidWeightException()
        {
            var testClass = CreateTestClass();

            Should.Throw<InvalidWeightException>(() => testClass.ScanItem(SKU_ONE, WEIGHT_NEGATIVE));
        }

        [Fact]
        public void UseCaseThree_ValidMarkdown_SubtotalEqualsPriceTwoMinusMarkdownTwo()
        {
            var testClass = CreateTestClass();

            var expectedPrice = Math.Round(PRICE_TWO - MARKDOWN_TWO, 2);

            testClass.ScanItem(SKU_TWO);

            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void UseCaseThree_ValidMarkdownTwice_SubtotalEqualsPriceTwo2xMinusMarkdownTwo2x()
        {
            var testClass = CreateTestClass();

            var scans = 2;
            var expectedPrice = Math.Round((PRICE_TWO - MARKDOWN_TWO) * scans, 2);

            for (int i = 0; i < scans; i++)
            {
                testClass.ScanItem(SKU_TWO);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void UseCaseFour_BogoSpecial_FourScansSubtotalOfThree()
        {
            var testClass = CreateTestClass();

            var scans = 4;
            var expectedPrice = Math.Round(PRICE_THREE * (scans - 1), 2);

            for (int i = 0; i < scans; i++)
            {
                testClass.ScanItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
            testClass.ScanLog.Count.ShouldBe(4);
        }

        [Fact]
        public void UseCaseFive_BuyMforNSpecial_SubtotalEqualsPriceTwo()
        {
            var testClass = CreateTestClass();

            var scans = 3;
            var expectedPrice = PRICE_TWO;

            for (int i = 0; i < scans; i++)
            {
                testClass.ScanItem(SKU_FOUR);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void UseCaseSix_BogoLimitSpecial_SubtotalEqualsPriceThree10x()
        {
            var testClass = CreateTestClass();

            var scans = 12;
            var expectedPrice = Math.Round(PRICE_THREE * 10, 2);

            for (int i = 0; i < scans; i++)
            {
                testClass.ScanItem(SKU_THREE);
            }

            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void UseCaseSeven_FourScansRemoveOne_SubtotalEqualsPriceOne3x()
        {
            var testClass = CreateTestClass();

            var scans = 4;
            var expectedPrice = Math.Round(PRICE_ONE * (scans - 1), 2);

            for (int i = 0; i < scans; i++)
            {
                testClass.ScanItem(SKU_ONE);
            }

            testClass.ScanItem(SKU_ONE, true);

            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void UseCaseSeven_RemoveOneEachItem_InvalidatesSpecial()
        {
            var testClass = CreateTestClass();

            var scans = 3;
            var expectedPrice = PRICE_FOUR * 2;

            for (int i = 0; i < scans; i++)
            {
                testClass.ScanItem(SKU_FOUR);
            }

            testClass.ScanItem(SKU_FOUR, true);

            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void UseCaseSeven_RemoveWeightedItem_SubtotalEqualsPriceOneTimesWeightOne()
        {
            var testClass = CreateTestClass();

            var expectedPrice = Math.Round(PRICE_ONE * WEIGHT_ONE, 2);

            testClass.ScanItem(SKU_TWO, WEIGHT_THREE);
            testClass.ScanItem(SKU_ONE, WEIGHT_ONE);
            testClass.ScanItem(SKU_TWO, WEIGHT_THREE, true);

            testClass.SubTotal.ShouldBe(expectedPrice);
        }

        [Fact]
        public void UseCaseEight_BogoSpecialWeightedQualifyingScan_IncreasesSubtotalBySpecialPrice()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseEight_BogoSpecialWeightedNonQualifyingScan_IncreasesSubtotalByFullPrice()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }
    }
}
