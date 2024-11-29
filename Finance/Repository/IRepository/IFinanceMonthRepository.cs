using Finance.Models;

namespace Finance.Repository.IRepository
{
    public interface IFinanceMonthRepository
    {
        Task<long> Insert(FinanceMonth financeMonth);
        Task<bool> Update(FinanceMonth financeMonth);
        Task<bool> Delete(long id);
        Task<List<FinanceMonth>> Query();
        Task<FinanceMonth> Select(long id);
    }
}