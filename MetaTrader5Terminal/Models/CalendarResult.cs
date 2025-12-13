
namespace MetaTrader5Terminal.Models
{
    public class CalendarResult
    {
        public long Id { get; set; }
        public int Event_id { get; set; }
        public DateTime Time { get; set; }
        public DateTime Period { get; set; }
        public int Revision { get; set; }
        public int Actual_value { get; set; }
        public int Prev_value { get; set; }
        public int Revised_Prev_Value { get; set; }
        public int Forecast_value { get; set; }
        public int Impact_type { get; set; }
        public string? Event { get; set; }
    }
}
