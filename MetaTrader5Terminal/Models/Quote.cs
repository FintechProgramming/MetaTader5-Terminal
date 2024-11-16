using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaTrader5Terminal.Models
{
    public class Quote
    {
        public string? SYMBOL { get; set; }

        public double? ASK { get; set; }

        public double? BID { get; set; }

        public double? LAST { get; set; }


        public int? FLAGS { get; set; }

        public string? TIME { get; set; }

        public int? VOLUME { get; set; }

        public double? RealVOLUME { get; set; }
        public long? Time_Msc { get; set; }



        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
