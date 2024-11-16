namespace MetaTrader5Terminal.Models
{
    public class SensetiveMacdResult
    {
        public SensetiveMacdResult()
        {
            Main = new PriceState();
            Signal = new PriceState();
        }
        public PriceState Main { get; set; }
        public PriceState Signal { get; set; }
    }
}
