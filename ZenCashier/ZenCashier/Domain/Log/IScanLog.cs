using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenCashier.Domain.Log
{
    public interface IScanLog
    {
        double SubTotal { get; }

        void LogScan(string skuId, double salePrice, double qty);

        IEnumerable<ScannedItemModel> GetScansForSku(string sku);
    }
}
