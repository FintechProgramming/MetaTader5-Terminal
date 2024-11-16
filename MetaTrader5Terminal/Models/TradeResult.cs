using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaTrader5Terminal.Models
{
    public class TradeResult
    {
        public double? ASK { get; set; }

        public double? BID { get; set; }

        public string? COMMENT { get; set; }

        public long? DEAL { get; set; }

        public long? ORDER { get; set; }

        public double? PRICE { get; set; }

        public long? REQUEST_ID { get; set; }

        public long? RETCODE { get; set; }

        public int? RETCODE_EXTERNAL { get; set; }

        public double? VOLUME { get; set; }

        public string? TYPE { get; set; }
        public int? MagicNumber { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
