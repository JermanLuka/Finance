using Finance.Models;

namespace Finance.Repository.IRepository
{
    public interface IMonthRepository
    {
        Task<int> Insert(Month month);
        Task<bool> Update(Month month);
        Task<bool> Delete(int id);
        Task<List<Month>> Query();
        Task<Month> Select(int id);
    }
}