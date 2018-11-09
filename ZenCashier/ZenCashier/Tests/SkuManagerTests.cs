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
        protected ISkuManager CreateSkuManager()
        {
            return new SkuManager();
        }

        #region AddSku

        [Fact]
        public void AddSku_ValidSkuPerPound_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSku("Tater Tots", .79);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSku_ValidSkuPerEach_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSku("Ketchup", 1.89);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSku_MissingSkuName_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSku(string.Empty, 1.75);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSku_NegativePrice_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSku("Tartar Sauce", -1.95);

            result.ShouldBe(false);
        }

        #endregion

        #region AddMarkdown

        [Fact]
        public void AddMarkdown_ValidMarkdown_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddMarkdown("tater tots", .25);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddMarkdown_MissingSku_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddMarkdown(string.Empty, .65);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddMarkdown_NegativeAmount_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddMarkdown("calamari", -.85);

            result.ShouldBe(false);
        }

        #endregion

        #region AddSpecial

        [Fact]
        public void AddSpecial_ValidSpecialNoLimit_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial("french bread", 2, 1.75, false);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecial_ValidSpecialLimit_ReturnsTrue()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial("taco shells", 4, 50, true, 10);

            result.ShouldBe(true);
        }

        [Fact]
        public void AddSpecial_MissingSku_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial(string.Empty, 4, 100, true);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NoTriggerQuantity_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial("palmolive", 0, 100, true, 10);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NoPrice_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial("lifebouy", 2, 0, false);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecialSetPrice_NegativePrice_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial("ivory", 2, -5, false);

            result.ShouldBe(false);
        }

        [Fact]
        public void AddSpecial_NegativeLimit_ReturnsFalse()
        {
            var testClass = CreateSkuManager();

            var result = testClass.AddSpecial("lemon pledge", 2, 5, false, -2);

            result.ShouldBe(false);
        }

        #endregion

        #region GetPrice
        

        #endregion
    }
}
