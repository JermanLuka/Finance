using Finance.Database;
using Finance.Models;
using Finance.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Finance.Repository
{
    public class FinanceMonthRepository : IFinanceMonthRepository
    {
        private readonly AppDbContext _context;

        public FinanceMonthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                var data = await _context.FinanceMonths.FirstOrDefaultAsync(f => f.Id == id);
                if (data == null)
                {
                    return false;
                }

                _context.FinanceMonths.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FinanceMonth Delete: " + ex.Message);
            }
        }

        public async Task<long> Insert(FinanceMonth financeMonth)
        {
            try
            {
                if (financeMonth == null)
                {
                    throw new ArgumentNullException(nameof(financeMonth), "FinanceMonth cannot be null");
                }

                await _context.FinanceMonths.AddAsync(financeMonth);
                await _context.SaveChangesAsync();

                return financeMonth.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FinanceMonth Insert: " + ex.Message);
            }
        }

        public async Task<List<FinanceMonth>> Query()
        {
            try
            {
                return await _context.FinanceMonths.Include(f => f.Month).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FinanceMonth Query: " + ex.Message);
            }
        }

        public async Task<FinanceMonth> Select(long id)
        {
            try
            {
                return await _context.FinanceMonths
                            .Include(fm => fm.Month) // Include the Month entity
                            .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FinanceMonth Select: " + ex.Message);
            }
        }

        public async Task<bool> Update(FinanceMonth financeMonth)
        {
            try
            {
                if (financeMonth == null)
                {
                    throw new ArgumentNullException(nameof(financeMonth), "FinanceMonth cannot be null");
                }

                _context.FinanceMonths.Update(financeMonth);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FinanceMonth Update: " + ex.Message);
            }
        }
    }
}