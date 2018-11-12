using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using ZenCashier.Domain.Log;
using static ZenCashier.Tests.TestValues;

namespace ZenCashier.Tests
{
    public class ScanLogTests
    {
        [Fact]
        public void LogScan_ValidScan_SubTotalMatches()
        {
            var testClass = new ScanLog();

            testClass.LogScan(SKU_ONE, PRICE_ONE, 1);

            testClass.SubTotal.ShouldBe(PRICE_ONE);
        }

        [Fact]
        public void LogScan_FourScans_SubTotalMatches()
        {
            var testClass = new ScanLog();

            var timesToScan = 4;
            var expectedResult = Math.Round(PRICE_ONE * timesToScan, 2);

            for (int i = 0; i < timesToScan; i++)
            {
                testClass.LogScan(SKU_ONE, PRICE_ONE, 1);
            }

            testClass.SubTotal.ShouldBe(expectedResult);
        }

        [Fact]
        public void LogScan_FourDifferentScans_SubTotalMatches()
        {
            var testClass = new ScanLog();

            var expectedResult = Math.Round(PRICE_ONE + PRICE_TWO + PRICE_THREE + PRICE_FOUR, 2);

            testClass.LogScan(SKU_ONE, PRICE_ONE, 1);
            testClass.LogScan(SKU_TWO, PRICE_TWO, 1);
            testClass.LogScan(SKU_THREE, PRICE_THREE, 1);
            testClass.LogScan(SKU_FOUR, PRICE_FOUR, 1);

            testClass.SubTotal.ShouldBe(expectedResult);
        }
    }
}
