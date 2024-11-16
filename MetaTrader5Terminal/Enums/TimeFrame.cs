using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTrader5Terminal.Enums
{
    public enum TimeFrame
    {
        /// <summary>
        /// Current timeframe
        /// </summary>
        PERIOD_CURRENT = 0,
        /// <summary>
        ///	1 minute 
        /// </summary>
        PERIOD_M1 = 1,
        /// <summary>
        /// 2 minute 
        /// </summary>
        PERIOD_M2 = 2,
        /// <summary>
        /// 3 minute 
        /// </summary>
        PERIOD_M3 = 3,
        /// <summary>
        /// 4 minute 
        /// </summary>
        PERIOD_M4 = 4,
        /// <summary>
        /// 5 minute 
        /// </summary>
        PERIOD_M5 = 5,
        /// <summary>
        /// 6 minute 
        /// </summary>
        PERIOD_M6 = 6,
        /// <summary>
        /// 10 minute 
        /// </summary>
        PERIOD_M10 = 10,
        /// <summary>
        /// 12 minute 
        /// </summary>
        PERIOD_M12 = 12,
        /// <summary>
        /// 15 minute 
        /// </summary>
        PERIOD_M15 = 15,
        /// <summary>
        /// 20 minute 
        /// </summary>
        PERIOD_M20 = 20,
        /// <summary>
        /// 30 minute 
        /// </summary>
        PERIOD_M30 = 30,
        /// <summary>
        /// 1 hour
        /// </summary>
        PERIOD_H1 = 60,
        /// <summary>
        /// 2 hours
        /// </summary>
        PERIOD_H2 = 120,
        /// <summary>
        /// 3 hours
        /// </summary>
        PERIOD_H3 = 180,
        /// <summary>
        /// 4 hours
        /// </summary>
        PERIOD_H4 = 240,
        /// <summary>
        /// 6 hours
        /// </summary>
        PERIOD_H6 = 360,
        /// <summary>
        /// 8 hours
        /// </summary>
        PERIOD_H8 = 480,
        /// <summary>
        /// 12 hours
        /// </summary>
        PERIOD_H12 = 720,
        /// <summary>
        /// 1 day
        /// </summary>
        PERIOD_D1 = 1440,
        /// <summary>
        /// 1 week
        /// </summary>
        PERIOD_W1 = 10080,
        /// <summary>
        /// 1 month
        /// </summary>
        PERIOD_MN1 = 40320
    }

}
