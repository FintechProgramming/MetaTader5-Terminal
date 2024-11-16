using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaTrader5Terminal.Models
{
    public class OrderEvent
    {
        public string? MSG { get; set; }

        public TradeTransaction? TRADE_TRANSACTION { get; set; }

        public TradeRequest? TRADE_REQUEST { get; set; }

        public TradeResult? TRADE_RESULT { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
