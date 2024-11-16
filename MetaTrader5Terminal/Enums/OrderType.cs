﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTrader5Terminal.Enums
{
    public enum OrderType
    {
        ORDER_TYPE_BUY,
        ORDER_TYPE_SELL,
        ORDER_TYPE_BUY_LIMIT,
        ORDER_TYPE_SELL_LIMIT,
        ORDER_TYPE_BUY_STOP,
        ORDER_TYPE_SELL_STOP,
        ORDER_TYPE_BUY_STOP_LIMIT,
        ORDER_TYPE_SELL_STOP_LIMIT,
        ORDER_TYPE_CLOSE_BY
    }
}
