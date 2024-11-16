using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MetaTrader5Terminal.Functions
{
    public class MovingAverages
    {
        public static int FloatingPoint = 6;
        // Simple Moving Average (SMA)
        public static double SMA(List<double> prices, int period)
        {
            if (prices.Count < period)
                throw new ArgumentException("Not enough data points.");

            double sum = 0;
            for (int i = 0; i < period; i++)
            {
                sum += prices[i];
            }

            return double.Round(sum / period, FloatingPoint);
        }


        // Linear Weighted Moving Average (LWMA)
        public static double LWMA(List<double> prices, int period)
        {
            if (prices.Count < period)
                throw new ArgumentException("Not enough data points.");

            double weightedSum = 0;
            double weightTotal = 0;

            int weight = 1;
            for (int i = prices.Count - period; i < prices.Count; i++)
            {
                weightedSum += prices[i] * weight;
                weightTotal += weight;
                weight++;
            }

            return double.Round(weightedSum / weightTotal, FloatingPoint);
        }

        // Smoothed Moving Average (SMMA)
        public static double SMMA(List<double> prices, int period)
        {
            if (prices.Count < period)
                throw new ArgumentException("Not enough data points.");

            double smmaPrevious = SMA(prices.GetRange(0, period), period); // First SMMA is the SMA

            for (int i = period; i < prices.Count; i++)
            {
                smmaPrevious = (smmaPrevious * (period - 1) + prices[i]) / period;
            }

            return double.Round(smmaPrevious, FloatingPoint);
        }




        //Exponential Moving Average(EMA)
        public static double EMA(List<double> prices, int period, int floatingPoint = 6)
        {
            if (prices.Count < period) throw new ArgumentException("Not enough data points.");
            double emaPrevious = prices[0];
            //double emaPrevious = SMA(prices, 26);
            double multiplier = 2.0 / (period + 1.0);

            for (int i = 1; i < prices.Count; i++)
            {
                emaPrevious = (prices[i] - emaPrevious) * multiplier + emaPrevious;
                //emaPrevious = (prices[i] * multiplier) + (emaPrevious * (1 - multiplier));
                //prices[i] = prices[i-1] + (multiplier * (value - prices[i-1]));
            }
            return Math.Round(emaPrevious, floatingPoint);
        }










        public int OnCalculate(int ratesTotal, int prevCalculated, DateTime[] time,
                       double[] open, double[] high, double[] low, double[] close,
                       long[] tickVolume, long[] volume, int[] spread,
                       AppliedPrice inpPrice, CEma ema, ref double[] val, ref int[] valc)
        {
            int i = prevCalculated - 1;
            if (i < 0) i = 0;

            // Loop through the price data and calculate EMA
            for (; i < ratesTotal; i++)
            {
                val[i] = ema.Calculate(GetPrice(inpPrice, open, high, low, close, i), i);
                valc[i] = 0;  // You can modify this depending on how you want to handle colors or indices.
            }

            return i;
        }




        public enum AppliedPrice
        {
            PRICE_CLOSE,
            PRICE_OPEN,
            PRICE_HIGH,
            PRICE_LOW,
            PRICE_MEDIAN,
            PRICE_TYPICAL,
            PRICE_WEIGHTED
        }

        public static double GetPrice(AppliedPrice tprice, double[] open, double[] high, double[] low, double[] close, int i)
        {
            switch (tprice)
            {
                case AppliedPrice.PRICE_CLOSE:
                    return close[i];
                case AppliedPrice.PRICE_OPEN:
                    return open[i];
                case AppliedPrice.PRICE_HIGH:
                    return high[i];
                case AppliedPrice.PRICE_LOW:
                    return low[i];
                case AppliedPrice.PRICE_MEDIAN:
                    return (high[i] + low[i]) / 2.0;
                case AppliedPrice.PRICE_TYPICAL:
                    return (high[i] + low[i] + close[i]) / 3.0;
                case AppliedPrice.PRICE_WEIGHTED:
                    return (high[i] + low[i] + close[i] + close[i]) / 4.0;
                default:
                    return 0;
            }
        }




    }
    public class CEma
    {
        private double _period;
        private double _alpha;
        private List<double> _emaValues;

        public CEma()
        {
            _period = 1;
            _alpha = 1;
            _emaValues = new List<double>();
        }

        // Initialize EMA with the period
        public void Init(int period)
        {
            _period = period > 1 ? period : 1;
            _alpha = 2.0 / (1.0 + _period);
        }

        // Calculate EMA for a specific bar
        public double Calculate(double value, int index)
        {
            if (_emaValues.Count <= index)
            {
                // Expand the list to accommodate more values
                while (_emaValues.Count <= index)
                    _emaValues.Add(0);
            }

            // Calculate EMA
            if (index > 0)
            {
                _emaValues[index] = _emaValues[index - 1] + _alpha * (value - _emaValues[index - 1]);
            }
            else
            {
                _emaValues[index] = value;
            }

            return _emaValues[index];
        }
    }

}
