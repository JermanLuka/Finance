namespace Finance.Models
{
    public class FinanceMonth
    {
        public long Id { get; set; }
        public int MonthId { get; set; }
        public Month? Month { get; set; }
        public int Year {  get; set; }
    }
}