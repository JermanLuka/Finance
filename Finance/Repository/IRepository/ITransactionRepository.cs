using Finance.Models;

namespace Finance.Repository.IRepository
{
    public interface ITransactionRepository
    {
        Task<int> Insert(Transaction transaction);
        Task<bool> Update(Transaction transaction);
        Task<bool> Delete(long id);
        Task<List<Transaction>> Query();
        Task<List<Transaction>> QueryByFinanceMonthId(long id);
        Task<Transaction> Select(long id);
    }
}