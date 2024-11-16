﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaTrader5Terminal.Models
{
    public class Order
    {
        public string? TIME_SETUP { get; set; }

        public string? SYMBOL { get; set; }

        public long? TICKET { get; set; }

        public string? TYPE { get; set; }

        public double? VOLUME_INITIAL { get; set; }

        public double? VOLUME_CURRENT { get; set; }

        public double? PRICE { get; set; }

        public double? SL { get; set; }

        public double? TP { get; set; }

        public string? STATE { get; set; }

        public int? MAGIC { get; set; }

        public string? COMMENT { get; set; }

        public string? TIME_DONE { get; set; }

        public long? POSITION { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}