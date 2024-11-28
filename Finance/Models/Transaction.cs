namespace Finance.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public bool IsIncome { get; set; }
        public long FinanceMonthId { get; set; }
        public int CategoryId { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}
