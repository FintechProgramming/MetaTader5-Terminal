using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaTrader5Terminal.Models
{
    public class TradeTransaction
    {
        public long? DEAL { get; set; }

        public string? DEAL_TYPE { get; set; }

        public long? ORDER { get; set; }

        public string? ORDER_STATE { get; set; }

        public string? ORDER_TYPE { get; set; }

        public long? POSITION { get; set; }

        public long? POSITION_BY { get; set; }

        public double? PRICE { get; set; }

        public double? PRICE_SL { get; set; }

        public double? PRICE_TP { get; set; }

        public double? PRICE_TRIGGER { get; set; }

        public string? SYMBOL { get; set; }

        public string? TIME_EXPIRATION { get; set; }

        public string? TIME_TYPE { get; set; }

        public string? TYPE { get; set; }

        public string? REASON { get; set; }

        public double? VOLUME { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
