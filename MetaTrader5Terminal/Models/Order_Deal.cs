using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaTrader5Terminal.Models
{
    public class Order_Deal
    {
        public string? TIME { get; set; }

        public long? TICKET { get; set; }

        public string? SYMBOL { get; set; }

        public string? TYPE { get; set; }

        public double? VOLUME { get; set; }

        public double? PRICE { get; set; }

        public int? MAGIC { get; set; }

        public string? COMMENT { get; set; }

        public List<Deal>? DEALS { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
