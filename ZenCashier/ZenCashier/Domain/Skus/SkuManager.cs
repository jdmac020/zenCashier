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
        public Dictionary<string, double> PriceList
        {
            get
            {
                if (_priceList is null)
                    _priceList = new Dictionary<string, double>();

                return _priceList;
            }

            set { _priceList = value; }
        }

        public Dictionary<string, double> MarkdownList
        {
            get
            {
                if (_markDowns is null)
                    _markDowns = new Dictionary<string, double>();

                return _markDowns;
            }

            set { _markDowns = value; }
        }

        public List<SpecialInfoModel> SpecialList
        {
            get
            {
                if (_specials is null)
                    _specials = new List<SpecialInfoModel>();

                return _specials;
            }

            set { _specials = value; }
        }

        protected Dictionary<string, double> _priceList;
        protected Dictionary<string, double> _markDowns;
        protected List<SpecialInfoModel> _specials;

        private const double ERROR_RETURN = -.01;

        public bool AddMarkdown(string sku, double amount)
        {
            if (ValidateSkuEntry(sku, amount))
            {
                MarkdownList.Add(sku, amount);

                return MarkdownList.Any(markdown => markdown.Key.Equals(sku) && markdown.Value.Equals(amount));
            }

            return false;
        }

        public bool AddSku(string id, double price)
        {
            if (ValidateSkuEntry(id, price))
            {
                PriceList.Add(id, price);

                return PriceList.Any(sku => sku.Key.Equals(id) && sku.Value.Equals(price));
            }

            return false;
        }

        public bool AddSpecial(string sku, double quantityToTrigger, double amount, bool isPercent, bool equalOrLesserValue, int limit = 0)
        {
            if (ValidateSpecialEntry(sku, quantityToTrigger, amount, limit))
            {
                SpecialList.Add(new SpecialInfoModel
                {
                    Sku = sku,
                    TriggerQuantity = quantityToTrigger,
                    Amount = amount,
                    IsPercentOff = isPercent,
                    NeedsEqualOrLesserPurchase = equalOrLesserValue,
                    LimitQuantity = limit
                });

                return SpecialList.Any(special => special.Sku.Equals(sku) && special.Amount.Equals(amount));

            }

            return false;
        }

        public double GetMarkdown(string sku)
        {

            var markdown = ERROR_RETURN;

            if (ValidateSkuRequest(sku))
            {
                if (MarkdownList.Any(mn => mn.Key.Equals(sku)))
                {
                    markdown = MarkdownList[sku];
                }
                else
                {
                    markdown = 0;
                }

            }

            return markdown;
        }

        public double GetPrice(string sku)
        {

            var price = ERROR_RETURN;

            if (ValidateSkuRequest(sku))
            {
                if (PriceList.Any(pr => pr.Key.Equals(sku)))
                {
                    price = PriceList[sku];
                }

            }

            return price;
        }

        public SpecialInfoModel GetSpecial(string sku)
        {
            var returnInfo = new SpecialInfoModel();

            if (ValidateSkuRequest(sku))
            {
                returnInfo = SpecialList.Where(special => special.Sku.Equals(sku)).FirstOrDefault();
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

        protected bool ValidateSpecialEntry(string sku, double quantityToTrigger, double specialPrice, int limit)
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
