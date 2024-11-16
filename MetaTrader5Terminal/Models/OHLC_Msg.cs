using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaTrader5Terminal.Models
{
    public class OHLC_Msg
    {
        public string? SYMBOL { get; set; }

        public string? PERIOD { get; set; }

        public List<Rates>? OHLC { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
