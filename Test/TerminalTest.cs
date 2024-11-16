using MetaTrader5Terminal;
using MetaTrader5Terminal.Enums;

namespace Test
{
    [TestClass]
    public class TerminalTest
    {
        MetaTraderTerminal metaTerminal;
        string symbol = "GBPUSD_i";
        TimeFrame timeFrame = TimeFrame.PERIOD_M15;

        public TerminalTest()
        {
            metaTerminal = new();
            metaTerminal.Connect();
        }
        [TestMethod]
        public void GetSymbolList()
        {
            var symbols = metaTerminal.GetSymbolList();
            var selectedSymbol = symbols.Where(x => x.NAME == symbol).Select(p => p.NAME).FirstOrDefault();
            Assert.AreEqual(symbol, selectedSymbol);
        }

        [TestMethod]
        public void GetQuote()
        {
            var qoute = metaTerminal.GetQuote(symbol);

            string LocalTime = DateTime.Now.AddMinutes(-30).ToString("yyyy/MM/dd HH:mm");
            string ServerTime = Convert.ToDateTime(qoute.TIME).ToString("yyyy/MM/dd HH:mm");

            Assert.AreEqual(symbol, qoute.SYMBOL);
            Assert.AreEqual(LocalTime, ServerTime);
        }

        [TestMethod]
        public void PriceHistory()
        {
            DateTime start = new DateTime(2024, 7, 22, 10, 00, 00, 111);

            var histories = metaTerminal.PriceHistory(symbol, TimeFrame.PERIOD_D1, start, 3);

            Assert.AreEqual(3, histories.Count);
        }

        [TestMethod]
        public void GetTicksByRange()
        {
            DateTime start = new DateTime(2024, 7, 24, 10, 00, 00, 111);
            DateTime end = start.AddMinutes(30);

            var tickdata = metaTerminal.GetTicksByRange(symbol, start, end);

            Assert.AreEqual(1494, tickdata.Count);
        }

        double position_size = 0.3;
        int magicNumber = 100;
        int BuyMagicIndex = 11;
        int SellMagicIndex = 11;

        [TestMethod]
        public void GetOpenPosition()
        {
            var openorder = metaTerminal.GetOpenPosition().FirstOrDefault();
            Assert.IsNull(openorder);
        }

        [TestMethod]
        public void GetPendingOrders()
        {
            var pendingorder = metaTerminal.GetPendingOrders().FirstOrDefault();
            Assert.IsNull(pendingorder);
        }

        [TestMethod]
        public void CheckBuySendOrder()
        {
            var qoute = metaTerminal.GetQuote(symbol);
            int magic = magicNumber + BuyMagicIndex;
            var sendOrderResult = metaTerminal.SendOrder(symbol, position_size, OrderType.ORDER_TYPE_BUY, (double)qoute.ASK, 0, 0, 0, "This is test", magic);
            var openorder = metaTerminal.GetOpenPosition().FirstOrDefault();

            var iscloseOrderResult = metaTerminal.OrderClose((long)openorder.TICKET);

            Assert.AreEqual(magic, openorder.MAGIC);
            Assert.IsTrue(iscloseOrderResult);
        }

        [TestMethod]
        public void CheckSellSendOrder()
        {
            var qoute = metaTerminal.GetQuote(symbol);
            int magic = magicNumber + BuyMagicIndex;
            var sendOrderResult = metaTerminal.SendOrder(symbol, position_size, OrderType.ORDER_TYPE_SELL, (double)qoute.BID, 0, 3, 0, "This is test", magic);
            var openorder = metaTerminal.GetOpenPosition().FirstOrDefault();

            var iscloseOrderResult = metaTerminal.OrderClose((long)openorder.TICKET!);

            Assert.AreEqual(magic, openorder.MAGIC);
            Assert.IsTrue(iscloseOrderResult);
        }

        [TestMethod]
        public void iMA_INDICATOR()
        {
            DateTime start = new DateTime(2024, 10, 09, 14, 30, 00, 000);
            var shiftBar = metaTerminal.iBarShift(symbol, TimeFrame.PERIOD_M15, start, true);
            var ma = double.Round(metaTerminal.iMA(symbol, TimeFrame.PERIOD_M15, 26, 0, MA_Method.MODE_EMA, Applied_Price.PRICE_CLOSE, shiftBar), 6);
            Assert.AreEqual(1.308491, ma);
        }


        [TestMethod]
        public void iGetDonchianChannelClose_INDICATOR()
        {
            DateTime start = ((new DateTime(1970, 1, 1)).AddMilliseconds((double)1724748430703));
            int barShift = metaTerminal.iBarShift(symbol, timeFrame, start, true);
            var donchianChannelResult = metaTerminal.iGetDonchianChannelClose(symbol, TimeFrame.PERIOD_M15, 20, barShift + 2);

            Assert.AreEqual(3, 3);
        }
    }
}