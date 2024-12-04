using Finance.Repository.IRepository;
using Finance.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IMonthRepository _monthRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionService _transactionService;

        public AnalyticsController(IMonthRepository monthRepository, ITransactionRepository transactionRepository, ITransactionService transactionService)
        {
            _monthRepository = monthRepository;
            _transactionRepository = transactionRepository;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            var months = await _monthRepository.Query();

            ViewBag.Months = months;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsByMonthAndYear(int monthId, int year)
        {
            try
            {
                var groupedTransactions = await _transactionService.QueryByMonthAndYearAsync(monthId, year);

                if (groupedTransactions == null || !groupedTransactions.Any())
                {
                    return NotFound("No transactions found for the specified month and year.");
                }

                return Ok(groupedTransactions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryTotals(int monthId, int year)
        {
            try
            {
                var categoryTotals = await _transactionService.GetCategoryTotalsByMonthAndYearAsync(monthId, year);

                if (categoryTotals == null || !categoryTotals.Any())
                {
                    return NotFound("No transactions found for the specified month and year.");
                }

                return Ok(categoryTotals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetIncomeVsExpenses(int monthId, int year)
        {
            try
            {
                var result = await _transactionService.GetIncomeVsExpensesAsync(monthId, year);
                return Ok(new
                {
                    Income = result.Income,
                    Expenses = result.Expenses
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}