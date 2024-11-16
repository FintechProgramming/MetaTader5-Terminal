using System.Text;
using Newtonsoft.Json;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using MetaTrader5Terminal.Models;
using MetaTrader5Terminal.Enums;

namespace MetaTrader5Terminal
{
    public class MetaTraderTerminal
    {
        public MetaTraderTerminal(string Host = "127.0.0.1", int CommandPort = 97, int DataPort = 98)
        {
            host = Host;
            cmd_port = CommandPort;
            data_port = DataPort;
            bufferLen = 65536;
            tcpClient_cmd = new TcpClient();
            tcpClient_data = new TcpClient();
            sendCmdLock = new object();
        }
        string host { get; set; }

        int cmd_port { get; set; }

        int data_port { get; set; }

        private static int bufferLen { get; set; }

        TcpClient tcpClient_cmd;

        TcpClient tcpClient_data;

        private object sendCmdLock;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler<Quote> OnPrice;

        JObject SendCommand(JObject cmd)
        {
            lock (sendCmdLock)
            {
                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(cmd.ToString(Formatting.None) + "\r\n");
                    NetworkStream stream = tcpClient_cmd.GetStream();
                    stream.ReadTimeout = 3000;
                    stream.Write(bytes, 0, bytes.Length);
                    bytes = new byte[bufferLen];
                    string text = string.Empty;
                    do
                    {
                        int count = stream.Read(bytes, 0, bufferLen);
                        text += Encoding.ASCII.GetString(bytes, 0, count);
                    }
                    while (stream.DataAvailable || !text.EndsWith("\r\n"));
                    JObject jObject = JsonConvert.DeserializeObject<JObject>(text);
                    if (jObject != null)
                    {
                        return jObject;
                    }

                    throw new Exception("Error with deserialization in SendCommand");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        private void ListenMTData()
        {
            Thread thread = new Thread((ThreadStart)delegate
            {
                ListenMTDataStream();
            });
            thread.IsBackground = true;
            thread.Start();
        }
        private void ListenMTDataStream()
        {
            byte[] array = new byte[bufferLen];
            NetworkStream stream = tcpClient_data.GetStream();
            while (true)
            {
                string text = string.Empty;
                do
                {
                    try
                    {
                        int count = stream.Read(array, 0, array.Length);
                        text += Encoding.ASCII.GetString(array, 0, count);
                    }
                    catch (Exception)
                    {
                    }
                }
                while (stream.DataAvailable);
                try
                {
                    text.Split(new string[2] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(delegate (string line)
                    {
                        JObject jObject = JObject.Parse(line);
                        if (jObject["MSG"].ToString() == "TRACK_PRICES")
                        {
                            Quote e = JsonConvert.DeserializeObject<Quote>(jObject["Data"].ToString());
                            if (OnPrice != null)
                            {
                                OnPrice(this, e);
                            }
                        }
                    });
                }
                catch (Exception)
                {
                }

                bool flag = true;
            }
        }
        public bool Connect()
        {
            try
            {
                tcpClient_cmd = new TcpClient(host, cmd_port);
                tcpClient_data = new TcpClient(host, data_port);
                ListenMTData();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (OnConnect != null)
            {
                OnConnect(this, new EventArgs());
            }

            return true;
        }
        public TerminalInfo GetTerminalInfo()
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "TERMINAL_INFO";
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<TerminalInfo>(jObject2.ToString());
                }

                throw new Exception("Error with the GetTerminalInfo command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public AccountStatus GetAccountStatus()
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "ACCOUNT_STATUS";
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<AccountStatus>(jObject2.ToString());
                }

                throw new Exception("Error with the GetAccountStatus command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Position> GetPendingOrders()
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "GetPendingOrders";
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<Position>>(jObject2["PENDING"].ToString());
                }

                throw new Exception("Error with the GetPendingOrders command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Position> GetOpenPosition()
        {
            try
            {
                JObject jObject = new JObject();
                //jObject["MSG"] = "ORDER_LIST";
                //
                jObject["MSG"] = "GetOpenPosition";

                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    List<Position> _positions = JsonConvert.DeserializeObject<List<Position>>(jObject2["OPENED"].ToString());
                    if (_positions is null)
                    {
                        _positions = new List<Position>();
                    }

                    return _positions;
                }

                throw new Exception("Error with the GetOpenedOrders command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<double> MA_Indicator(string Symbol, TimeFrame tf, int MA_Period, int MA_Shift, MA_Method MA_Method, Applied_Price Applied_Price, int Shift, int Num = 1)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "MA_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["MA_PERIOD"] = MA_Period;
                jObject["MA_SHIFT"] = MA_Shift;
                jObject["MA_METHOD"] = MA_Method.ToString();
                jObject["APPLIED_PRICE"] = Applied_Price.ToString();
                jObject["SHIFT"] = Shift;
                jObject["NUM"] = Num;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<double>>(jObject2["DATA_VALUES"].ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<double> ATR(string Symbol, TimeFrame tf, int Period, int Shift, int Num = 1)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "ATR_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["PERIOD"] = Period;
                jObject["SHIFT"] = Shift;
                jObject["NUM"] = Num;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<double>>(jObject2["DATA_VALUES"].ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double iClose(string Symbol, TimeFrame tf, int Shift)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iClose_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["SHIFT"] = Shift;
                int num = 1;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<double>(jObject2["DATA_VALUE"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double iATR(string Symbol, TimeFrame tf,  int Period, int BarShift = 0)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iATR_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["Period"] = Period;
                jObject["BarShift"] = BarShift;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<double>(jObject2["DATA_VALUE"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double iMA(string Symbol, TimeFrame tf, int Period, int Shift,MA_Method mA,Applied_Price applied_Price,  int BarShift = 0)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iMA_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["PERIOD"] = Period;
                jObject["SHIFT"] = Shift;
                jObject["MA_Method"] = mA.ToString();
                jObject["Applied_Price"] = applied_Price.ToString();
                jObject["BarShift"] = BarShift;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<double>(jObject2["DATA_VALUE"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ResultLine iStochastic(string Symbol, TimeFrame period, int Kperiod =14, int Dperiod=4, int Slowing=3, MA_Method ma_method = MA_Method.MODE_EMA, STO_PRICE price_field = STO_PRICE.STO_CLOSECLOSE, int BarShift = 1)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iStochastic_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = period.ToString();
                jObject["Kperiod"] = Kperiod;
                jObject["Dperiod"] = Dperiod;
                jObject["Slowing"] = Slowing;
                jObject["MA_Method"] = ma_method.ToString();
                jObject["STO_PRICE"] = price_field.ToString();
                jObject["BarShift"] = BarShift;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<ResultLine>(jObject2["DATA_VALUE"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ResultLine iMACD(string Symbol, TimeFrame period, int Fast_Ema_Period, int Slow_Ema_Period, int Signal_Period, Applied_Price Applied_Price, int BarShift = 0)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iMACD_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = period.ToString();
                jObject["Fast_Ema_Period"] = Fast_Ema_Period;
                jObject["Slow_Ema_Period"] = Slow_Ema_Period;
                jObject["Signal_Period"] = Signal_Period;
                jObject["Applied_Price"] = Applied_Price.ToString();
                jObject["BarShift"] = BarShift;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<ResultLine>(jObject2["DATA_VALUE"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<double> iCustom_INDICATOR(string Symbol, TimeFrame tf, string Indicator_Name,int valval, int Mode, int Shift)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iCustom_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["Indicator_Name"] = Indicator_Name;
                jObject["valval"] = valval;
                jObject["MODE"] = Mode;
                jObject["SHIFT"] = Shift;

                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<double>>(jObject2["DATA_VALUES"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<double> Custom_Indicator(string Symbol, TimeFrame tf, string Indicator_Name, int Mode, int Shift, int Num = 1, List<string> Params = null)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "CUSTOM_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["INDICATOR_NAME"] = Indicator_Name;
                jObject["MODE"] = Mode;
                jObject["SHIFT"] = Shift;
                jObject["NUM"] = Num;
                int num = 1;
                foreach (string Param in Params)
                {
                    double result2;
                    if (int.TryParse(Param, out var result))
                    {
                        jObject["PARAM" + num] = result;
                    }
                    else if (double.TryParse(Param, out result2))
                    {
                        jObject["PARAM" + num] = result2;
                    }
                    else
                    {
                        jObject["PARAM" + num] = Param.ToString();
                    }

                    num++;
                }

                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<double>>(jObject2["DATA_VALUES"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int iBarShift(string Symbol, TimeFrame tf, DateTime time, bool Exact = false )
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iBarShift_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["TIME"] = time.ToString("yyyy.MM.dd HH:mm:ss.fff");
                jObject["EXACT"] = Exact;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<int>(jObject2["DATA_VALUE"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DonchianChannelResult iGetDonchianChannelClose(string Symbol, TimeFrame tf, int Period, int BarShift = 0)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iGetDonchianChannelClose_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["Period"] = Period;
                jObject["BarShift"] = BarShift;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<DonchianChannelResult>(jObject2["DATA_VALUE"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DonchianChannelResult iGetDonchianChannel(string Symbol, TimeFrame tf, int Period, int BarShift)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "iGetDonchianChannel_INDICATOR";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["Period"] = Period;
                jObject["Shift"] = BarShift;

                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<DonchianChannelResult>(jObject2["DATA_VALUE"].ToString());
                }

                throw new Exception("Error with the Custom_Indicator command. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Position> GetTradeHistoryPositions(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "TRADE_HISTORY";
                jObject["MODE"] = TradeHistoryMode.POSITIONS.ToString();
                jObject["FROM_DATE"] = FromDate.ToString("yyyy.MM.dd HH:mm:ss");
                jObject["TO_DATE"] = ToDate.ToString("yyyy.MM.dd HH:mm:ss");
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<Position>>(jObject2[TradeHistoryMode.POSITIONS.ToString()].ToString());
                }

                throw new Exception("Error with the command TRADE_HISTORY. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Deal> GetTradeHistoryDeals(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "TRADE_HISTORY";
                jObject["MODE"] = TradeHistoryMode.DEALS.ToString();
                jObject["FROM_DATE"] = FromDate.ToString("yyyy.MM.dd HH:mm:ss");
                jObject["TO_DATE"] = ToDate.ToString("yyyy.MM.dd HH:mm:ss");
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<Deal>>(jObject2[TradeHistoryMode.DEALS.ToString()].ToString());
                }

                throw new Exception("Error with the command TRADE_HISTORY. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Order> GetTradeHistoryOrders(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "TRADE_HISTORY";
                jObject["MODE"] = TradeHistoryMode.ORDERS.ToString();
                jObject["FROM_DATE"] = FromDate.ToString("yyyy.MM.dd HH:mm:ss");
                jObject["TO_DATE"] = ToDate.ToString("yyyy.MM.dd HH:mm:ss");
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<Order>>(jObject2[TradeHistoryMode.ORDERS.ToString()].ToString());
                }

                throw new Exception("Error with the command TRADE_HISTORY. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Order_Deal> GetTradeHistoryOrdersDeals(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "TRADE_HISTORY";
                jObject["MODE"] = TradeHistoryMode.ORDERS_DEALS.ToString();
                jObject["FROM_DATE"] = FromDate.ToString("yyyy.MM.dd HH:mm:ss");
                jObject["TO_DATE"] = ToDate.ToString("yyyy.MM.dd HH:mm:ss");
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<Order_Deal>>(jObject2[TradeHistoryMode.ORDERS_DEALS.ToString()].ToString());
                }

                throw new Exception("Error with the command TRADE_HISTORY. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Rates> PriceHistory(string Symbol, TimeFrame tf, DateTime FromDate, int Count)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "PRICE_HISTORY";
                jObject["SYMBOL"] = Symbol;
                jObject["TIMEFRAME"] = tf.ToString();
                jObject["FROM_DATE"] = FromDate.ToString("yyyy.MM.dd HH:mm:ss");
                jObject["COUNT"] = Count;
                //var jObject2 = SendCommandXXX(jObject);
                var jObject2 = SendCommand(jObject);
                if (jObject2["MSG"].ToString() == "PRICE_HISTORY")
                {
                    List<Rates> ratesList = JsonConvert.DeserializeObject<List<Rates>>(jObject2["Data"].ToString());
                    return ratesList;
                    // return JsonConvert.DeserializeObject<List<Rates>>(jObject2.ToString());
                }
                return null;

                //throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public TradeResult SendOrder(string Symbol, double Volume,OrderType Type, double Price = 0.0, double SL = 0.0,double TP = 0.0, double Slippage = 0.0,string Comment = "", int MagicNr = 0,string Expiration = "1970/01/01 00:00",ENUM_TRADE_REQUEST_ACTIONS TRADE_REQUEST_ACTIONS = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_DEAL)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "ORDER_SEND";
                jObject["SYMBOL"] = Symbol;
                jObject["ACTION"] = (int)TRADE_REQUEST_ACTIONS;
                jObject["VOLUME"] = Volume;
                jObject["TYPE"] = Type.ToString();
                if (SL > 0.0)
                {
                    jObject["SL"] = SL;
                }

                if (TP > 0.0)
                {
                    jObject["TP"] = TP;
                }

                if (Price > 0.0)
                {
                    jObject["PRICE"] = Price;
                }

                if (Slippage > 0.0)
                {
                    jObject["SLIPPAGE"] = Slippage;
                }

                if (Comment != "")
                {
                    jObject["COMMENT"] = Comment;
                }

                if (MagicNr > 0)
                {
                    jObject["MAGICNR"] = MagicNr;
                }

                if (Expiration != "1970/01/01 00:00")
                {
                    jObject["EXPIRATION"] = Expiration;
                }

                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<TradeResult>(jObject2["Data"].ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
        public TradeResult OrderModify(long Ticket, double SL = 0.0, double TP = 0.0)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "ORDER_MODIFY";
                jObject["TICKET"] = Ticket;
                if (SL > 0.0)
                {
                    jObject["SL"] = SL;
                }

                if (TP != 0.0)
                {
                    jObject["TP"] = TP;
                }

                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<TradeResult>(jObject2.ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool OrderClose(long Ticket)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "ORDER_CLOSE";
                jObject["TICKET"] = Ticket;             
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<bool>(jObject2["Data"].ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Asset> GetSymbolList()
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "SYMBOL_LIST";
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<Asset>>(jObject2["SYMBOLS"].ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Asset GetSymbolInfo(string Symbol)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "SYMBOL_INFO";
                jObject["SYMBOL"] = Symbol;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<Asset>(jObject2.ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Quote GetQuote(string Symbol)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "QUOTE";
                jObject["SYMBOL"] = Symbol;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2.ToString() != "")
                {
                    return JsonConvert.DeserializeObject<Quote>(jObject2.ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Quote> GetTicksByRange(string symbol, DateTime fromDate, DateTime toDate)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "GetTicksByRange";
                jObject["SYMBOL"] = symbol;
                jObject["FROM_DATE"] = fromDate.ToString("yyyy.MM.dd HH:mm:ss.fff");
                jObject["TO_DATE"] = toDate.ToString("yyyy.MM.dd HH:mm:ss.fff");
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<Quote>>(jObject2["Data"].ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Quote> GetTicksByStep(string symbol, DateTime fromDate, int count)
        {
            try
            {
                JObject jObject = new JObject();
                jObject["MSG"] = "GetTicksByStep";
                jObject["SYMBOL"] = symbol;
                jObject["FROM_DATE"] = fromDate.ToString("yyyy.MM.dd HH:mm:ss.fff");
                jObject["COUNT"] = count;
                JObject jObject2 = SendCommand(jObject);
                if (jObject2["ERROR_ID"].ToString() == "0")
                {
                    return JsonConvert.DeserializeObject<List<Quote>>(jObject2["Data"].ToString());
                }

                throw new Exception("Error with the command sent. ERROR_ID: " + jObject2["ERROR_ID"]?.ToString() + " ERROR_DESCRIPTION: " + jObject2["ERROR_DESCRIPTION"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
