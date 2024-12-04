using Finance.Models;

namespace Finance.DTO
{
    public class GroupedTransactionDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
