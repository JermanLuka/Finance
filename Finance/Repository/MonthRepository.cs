using Finance.Database;
using Finance.Models;
using Finance.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Finance.Repository
{
    public class MonthRepository : IMonthRepository
    {
        private readonly AppDbContext _context;

        public MonthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var data = await _context.Months.FirstOrDefaultAsync(f => f.Id == id);
                if (data == null)
                {
                    return false;
                }

                _context.Months.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Month Delete: " + ex.Message);
            }
        }

        public async Task<int> Insert(Month month)
        {
            try
            {
                if (month == null)
                {
                    throw new ArgumentNullException(nameof(month), "Month cannot be null");
                }

                await _context.Months.AddAsync(month);
                await _context.SaveChangesAsync();

                return month.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Month Insert: " + ex.Message);
            }
        }

        public async Task<List<Month>> Query()
        {
            try
            {
                return await _context.Months.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Month Query: " + ex.Message);
            }
        }

        public async Task<Month> Select(int id)
        {
            try
            {
                return await _context.Months.FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Month Select: " + ex.Message);
            }
        }

        public async Task<bool> Update(Month month)
        {
            try
            {
                if (month == null)
                {
                    throw new ArgumentNullException(nameof(month), "Month cannot be null");
                }

                _context.Months.Update(month);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Month Update: " + ex.Message);
            }
        }
    }
}