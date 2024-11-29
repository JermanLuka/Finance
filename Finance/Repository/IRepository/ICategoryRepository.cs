using Finance.Models;

namespace Finance.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<int> Insert(Category category);
        Task<bool> Update(Category category);
        Task<bool> Delete(int id);
        Task<List<Category>> Query();
        Task<Category> Select(int id);
    }
}