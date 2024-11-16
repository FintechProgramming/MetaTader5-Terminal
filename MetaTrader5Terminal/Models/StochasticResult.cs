namespace MetaTrader5Terminal.Models
{
    public class StochasticResult
    {
        public StochasticResult()
        {
            Main = new PriceState();
            Signal = new PriceState();
        }
        public PriceState Main { get; set; }
        public PriceState Signal { get; set; }
    }
}
