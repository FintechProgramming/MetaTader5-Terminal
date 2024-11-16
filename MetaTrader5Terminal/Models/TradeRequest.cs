using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaTrader5Terminal.Models
{
    public class TradeRequest
    {
        public string? ACTION { get; set; }

        public string? COMMENT { get; set; }

        public int? DEVIATION { get; set; }

        public string? EXPIRATION { get; set; }

        public int? MAGIC { get; set; }

        public long? ORDER { get; set; }

        public long? POSITION { get; set; }

        public long? POSITION_BY { get; set; }

        public double? PRICE { get; set; }

        public double? SL { get; set; }

        public double? STOPLIMIT { get; set; }

        public string? SYMBOL { get; set; }

        public double? TP { get; set; }

        public string? TYPE { get; set; }

        public string? TYPE_FILLING { get; set; }

        public string? TYPE_TIME { get; set; }

        public double? VOLUME { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
