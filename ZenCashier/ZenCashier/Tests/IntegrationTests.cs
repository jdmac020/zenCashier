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
    public class IntegrationTests
    {
        private IOrder _testClass = CreateTestClass();

        private static IOrder CreateTestClass()
        {
            // Initialize SkuManager
                // Add price-only sku (use cases 1 & 2)
                // Add markdown sku (use case 3)
                // Add BOGO sku with limit (use case 4, 6 & 8)
                // Add M for N sku (use case 5)

            // Initialize Order with SkuManager above

            return new Order();
        }

        [Fact]
        public void UseCaseOne_ValidEachItem_IncreasesSubtotal()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseOne_InvalidSku_NoSubtotalIncrease()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseTwo_ValidWeightedItem_IncreasesSubtotal()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseTwo_InvalidSkuValidWeight_NoSubTotalIncrease()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseTwo_ValidSkuInValidWeight_NoSubTotalIncrease()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseThree_ValidMarkdown_IncreasesSubtotalByCorrectPrice()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseFour_BogoSpecial_IncreasesSubtotalByCorrectPrice()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseFive_BuyMforNSpecial_IncreasesSubtotalByCorrectPrice()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseSix_BogoLimitSpecial_IncreasesSubtotalByCorrectPrice()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseSeven_RemoveOneEachItem_DecreasesSubtotalByCorrectPrice()
        {
            var foo = 0;
            foo.ShouldBe(1);
        }

        [Fact]
        public void UseCaseSeven_RemoveWeightedItem_DecreasesSubtotalByCorrectPrice()
        {
            var foo = 0;
            foo.ShouldBe(1);
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
