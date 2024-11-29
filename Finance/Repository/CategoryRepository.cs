using Finance.Database;
using Finance.Models;
using Finance.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Finance.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var data = await _context.Categories.FirstOrDefaultAsync(f => f.Id == id);
                if(data == null)
                {
                    return false;
                }

                _context.Categories.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) {
                throw new Exception("Error in Category Delete: " + ex.Message);
            }
        }

        public async Task<int> Insert(Category category)
        {
            try
            {
                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category), "Category cannot be null");
                }

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return category.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Category Insert: " + ex.Message);
            }
        }

        public async Task<List<Category>> Query()
        {
            try
            {
                return await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Category Query: " + ex.Message);
            }
        }

        public async Task<Category> Select(int id)
        {
            try
            {
                return await _context.Categories.FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Category Select: " + ex.Message);
            }
        }

        public async Task<bool> Update(Category category)
        {
            try
            {
                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category), "Category cannot be null");
                }

                _context.Categories.Update(category);
                await _context.SaveChangesAsync(); // Save changes to the database
                return true; // Return true to indicate success
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Category Update: " + ex.Message);
            }
        }
    }
}