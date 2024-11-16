using MetaTrader5Terminal;
using MetaTrader5Terminal.Models;

namespace MetaTrader5_Terminal
{
    public class FinancialStrategy
    {
        public MetaTraderTerminal terminal;
        public DateTime CurrentTimeTesting;
        public void LetsTrade(object? sender, Quote e)
        {
            CurrentTimeTesting = ((new DateTime(1970, 1, 1)).AddMilliseconds((double)e.Time_Msc));
            Console.WriteLine("Time: " + CurrentTimeTesting.ToString("yyyy-MM-dd - HH:mm:ss.fff"));
            Console.WriteLine("Ask: " + e.ASK);
            Console.WriteLine("Bid: " + e.BID);
            //Write Your Strategy here ...
        }
        public void Start()
        {
            terminal = new MetaTraderTerminal();    
            terminal.Connect();
            terminal.OnPrice += LetsTrade;
            var res = terminal.GetSymbolInfo("GBPUSD_i");
            Thread UpdateOrdersThread = new Thread(() => UpdateOrderListLoop())
            {
                //IsBackground = true
            };
            UpdateOrdersThread.Start();
        }
        void UpdateOrderListLoop()
        {
            do
            {
                System.Threading.Thread.Sleep(1000);
                UpdateOrderList();
            } while (true);
        }
        void UpdateOrderList()
        {
            List<Position> lst = terminal.GetOpenPosition();
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    Console.WriteLine(item);
                }
            }
        }

    }
}
