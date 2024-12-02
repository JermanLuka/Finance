using Finance.Database;
using Finance.Models;
using Finance.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Finance.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        } 

        public async Task<bool> Delete(long id)
        {
            try
            {
                var data = await _context.Transactions.FirstOrDefaultAsync(f => f.Id == id);
                if (data == null)
                {
                    return false;
                }

                _context.Transactions.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Transaction Delete: " + ex.Message);
            }
        }

        public async Task<int> Insert(Transaction transaction)
        {
            try
            {
                if (transaction == null)
                {
                    throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null");
                }

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                return transaction.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Transaction Insert: " + ex.Message);
            }
        }

        public async Task<List<Transaction>> Query()
        {
            try
            {
                return await _context.Transactions.Include(f => f.Category).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Transaction Query: " + ex.Message);
            }
        }

        public async Task<List<Transaction>> QueryByFinanceMonthId(long id)
        {
            try
            {
                return await _context.Transactions.Include(f => f.Category).Where(f => f.FinanceMonthId == id).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Transaction Query: " + ex.Message);
            }
        }

        public async Task<Transaction> Select(long id)
        {
            try
            {
                return await _context.Transactions.FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Transaction Select: " + ex.Message);
            }
        }

        public async Task<bool> Update(Transaction transaction)
        {
            try
            {
                if (transaction == null)
                {
                    throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null");
                }

                _context.Transactions.Update(transaction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Transaction Update: " + ex.Message);
            }
        }
    }
}