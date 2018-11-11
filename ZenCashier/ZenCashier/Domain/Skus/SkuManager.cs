using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenCashier.Domain.Skus.Models;

namespace ZenCashier
{
    public class SkuManager : ISkuManager
    {

        private const double ERROR_RETURN = -.01;

        public bool AddMarkdown(string sku, double amount)
        {
            return ValidateSkuEntry(sku, amount);
        }

        public bool AddSku(string id, double price)
        {
            return ValidateSkuEntry(id, price);
        }

        public bool AddSpecial(string sku, int quantityToTrigger, double amount, bool isPercent, int limit = 0)
        {
            return ValidateSpecialEntry(sku, quantityToTrigger, amount, limit);
        }

        public double GetMarkdown(string sku)
        {

            var markdown = ERROR_RETURN;

            if (ValidateSkuRequest(sku))
            {
                markdown = .2;
            }

            return markdown;
        }

        public double GetPrice(string sku)
        {

            var price = ERROR_RETURN;

            if (ValidateSkuRequest(sku))
            {
                price = .79;
            }

            return price;
        }

        public SpecialInfoModel GetSpecial(string sku)
        {
            var returnInfo = new SpecialInfoModel();

            if (ValidateSkuRequest(sku))
            {
                returnInfo.Amount = 100;
            }

            return returnInfo;
        }

        protected bool ValidateSkuRequest(string skuId)
        {
            return ! string.IsNullOrEmpty(skuId);
        }

        protected bool ValidateSkuEntry(string skuId, double amount)
        {
            if (string.IsNullOrEmpty(skuId))
                return false;

            if (amount < 0)
                return false;

            return true;
        }

        protected bool ValidateSpecialEntry(string sku, int quantityToTrigger, double specialPrice, int limit)
        {
            if (string.IsNullOrEmpty(sku))
                return false;

            if (quantityToTrigger <= 0)
                return false;

            if (specialPrice <= 0)
                return false;

            if (limit < 0)
                return false;

            return true;
        }

        
    }
}
