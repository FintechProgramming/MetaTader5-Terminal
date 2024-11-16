using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTrader5Terminal.Models
{
    public class PriceState
    {
        public double Current { get; set; }
        public double Closed { get; set; }
        public double Previous { get; set; }

    }
}
