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
        protected Dictionary<string, double> _skuList = new Dictionary<string, double>();
        protected Dictionary<string, double> _markDowns = new Dictionary<string, double>();
        protected List<SpecialInfoModel> _specials = new List<SpecialInfoModel>();

        private const double ERROR_RETURN = -.01;

        public bool AddMarkdown(string sku, double amount)
        {
            if (ValidateSkuEntry(sku, amount))
            {
                _markDowns.Add(sku, amount);

                return _markDowns.Any(markdown => markdown.Key.Equals(sku) && markdown.Value.Equals(amount));
            }

            return false;
        }

        public bool AddSku(string id, double price)
        {
            if (ValidateSkuEntry(id, price))
            {
                _skuList.Add(id, price);

                return _skuList.Any(sku => sku.Key.Equals(id) && sku.Value.Equals(price));
            }

            return false;
        }

        public bool AddSpecial(string sku, int quantityToTrigger, double amount, bool isPercent, bool equalOrLesserValue, int limit = 0)
        {
            if (ValidateSpecialEntry(sku, quantityToTrigger, amount, limit))
            {
                _specials.Add(new SpecialInfoModel
                {
                    Sku = sku,
                    TriggerQuantity = quantityToTrigger,
                    Amount = amount,
                    IsPercentOff = isPercent,
                    NeedsEqualOrLesserPurchase = equalOrLesserValue,
                    LimitQuantity = limit
                });

                return _specials.Any(special => special.Sku.Equals(sku) && special.Amount.Equals(amount));

            }

            return false;
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
