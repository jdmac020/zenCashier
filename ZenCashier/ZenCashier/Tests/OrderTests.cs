﻿using System;
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
    public class OrderTests
    {

        #region Sku-only

        [Fact]
        public void ScanItem_ValidSkuOnly_SubtotalEqualsPrice()
        {
            var testClass = new Order();

            testClass.ScanItem(SKU_ONE);

            testClass.SubTotal.ShouldBe(PRICE_ONE);
        }

        [Fact]
        public void ScanItem_InvalidSkuOnly_SubtotalEqualsZero()
        {
            var testClass = new Order();

            testClass.ScanItem(string.Empty);

            testClass.SubTotal.ShouldBe(0);
        }

        #endregion

        #region With Weight
        
        [Fact]
        public void ScanItem_ValidSkuAndWeight_SubtotalEqualsPriceTimesWeight()
        {
            var testClass = new Order();

            testClass.ScanItem(SKU_ONE, WEIGHT_ONE);

            testClass.SubTotal.ShouldBe(1.78);
        }

        [Fact]
        public void ScanItem_InvalidSkuValidWeight_SubtotalEqualsZero()
        {
            var testClass = new Order();

            testClass.ScanItem(string.Empty, WEIGHT_TWO);

            testClass.SubTotal.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidSkuZeroWeight_SubtotalEqualsZero()
        {
            var testClass = new Order();

            testClass.ScanItem(SKU_TWO, WEIGHT_ZERO);

            testClass.SubTotal.ShouldBe(0);
        }

        [Fact]
        public void ScanItem_ValidSkuNegativeWeight_ShouldThrowException()
        {
            var testClass = new Order();

            Should.Throw<InvalidWeightException>(() => testClass.ScanItem(SKU_THREE, WEIGHT_NEGATIVE));
        }

        #endregion

        #region Markdown Tests

        [Fact]
        public void ScanItem_ValidEachSku_SubtotalEqualsOneFiftyEight()
        {

        }

        #endregion
    }
}
