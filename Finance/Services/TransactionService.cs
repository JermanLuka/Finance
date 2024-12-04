using Finance.Database;
using Finance.DTO;
using Finance.Models;
using Finance.Repository;
using Finance.Repository.IRepository;
using Finance.Services.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Services
{
    public class TransactionService : ITransactionService 
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFinanceMonthRepository _financeMonthRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(AppDbContext appDbContext, IFinanceMonthRepository financeMonthRepository, ITransactionRepository transactionRepository)
        {
            _appDbContext = appDbContext;
            _financeMonthRepository = financeMonthRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<List<GroupedTransactionDto>> QueryByMonthAndYearAsync(int monthId, int year)
        {
            try
            {
                // Query the finance month
                var financeMonth = await _appDbContext.FinanceMonths
                    .Where(f => f.MonthId == monthId && f.Year == year)
                    .FirstOrDefaultAsync();

                if (financeMonth == null)
                {
                    throw new KeyNotFoundException("FinanceMonth not found for the given monthId and year.");
                }

                // Query transactions for the finance month with their categories
                var transactions = await _appDbContext.Transactions
                    .Where(f => f.FinanceMonthId == financeMonth.Id)
                    .Include(t => t.Category) // Assuming Transactions has a navigation property to Category
                    .ToListAsync();

                if (!transactions.Any())
                {
                    return new List<GroupedTransactionDto>(); // Return empty list if no transactions
                }

                // Group transactions by CategoryId and include CategoryName
                var groupedTransactions = transactions
                    .GroupBy(t => new { t.CategoryId, t.Category.Name }) // Group by CategoryId and Category Name
                    .Select(g => new GroupedTransactionDto
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.Name, // Extract the CategoryName
                        Transactions = g.ToList()
                    })
                    .ToList();

                return groupedTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in QueryByMonthAndYearAsync: {ex.Message}", ex);
            }
        }

        public async Task<List<CategorySummaryDto>> GetCategoryTotalsByMonthAndYearAsync(int monthId, int year)
        {
            try
            {
                // Query the finance month
                var financeMonth = await _appDbContext.FinanceMonths
                    .Where(f => f.MonthId == monthId && f.Year == year)
                    .FirstOrDefaultAsync();

                if (financeMonth == null)
                {
                    throw new KeyNotFoundException("FinanceMonth not found for the given monthId and year.");
                }

                // Query transactions for the finance month with their categories
                var transactions = await _appDbContext.Transactions
                    .Where(f => f.FinanceMonthId == financeMonth.Id && f.IsIncome == false)
                    .Include(t => t.Category) // Include the category for grouping
                    .ToListAsync();

                if (!transactions.Any())
                {
                    return new List<CategorySummaryDto>(); // Return empty list if no transactions
                }

                // Group transactions by CategoryId and calculate the total amount
                var categoryTotals = transactions
                    .GroupBy(t => new { t.CategoryId, t.Category.Name })
                    .Select(g => new CategorySummaryDto
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.Name,
                        TotalAmount = g.Sum(t => t.Amount)
                    })
                    .OrderByDescending(t => t.TotalAmount)
                    .ToList();

                return categoryTotals;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetCategoryTotalsByMonthAndYearAsync: {ex.Message}", ex);
            }
        }

        public async Task<(double Income, double Expenses)> GetIncomeVsExpensesAsync(int monthId, int year)
        {
            // Query the finance month
            var financeMonth = await _appDbContext.FinanceMonths
                    .Where(f => f.MonthId == monthId && f.Year == year)
                    .FirstOrDefaultAsync();

            if (financeMonth == null)
            {
                throw new KeyNotFoundException("FinanceMonth not found for the given monthId and year.");
            }

            // Query transactions for the finance month with their categories
            var transactions = await _appDbContext.Transactions
                .Where(f => f.FinanceMonthId == financeMonth.Id)
                .Include(t => t.Category) // Include the category for grouping
                .ToListAsync();

            if (!transactions.Any())
            {
                return (Income: 0.0, Expenses: 0.0);
            }

            // Calculate totals
            var income = transactions.Where(t => t.IsIncome).Sum(t => t.Amount);
            var expenses = transactions.Where(t => !t.IsIncome).Sum(t => t.Amount);

            return (Income: income, Expenses: expenses);
        }
    }
}