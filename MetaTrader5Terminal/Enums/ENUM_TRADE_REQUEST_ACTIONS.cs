using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTrader5Terminal.Enums
{
    public enum ENUM_TRADE_REQUEST_ACTIONS
    {
        TRADE_ACTION_DEAL = 0,
        TRADE_ACTION_PENDING = 1,
        TRADE_ACTION_SLTP = 2,
        TRADE_ACTION_MODIFY = 3,
        TRADE_ACTION_REMOVE = 4,
        TRADE_ACTION_CLOSE_BY = 5,
    }
}
