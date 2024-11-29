using Finance.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IActionResult> Index(long financeMonthId, string monthName)
        {
            ViewBag.MonthName = monthName;

            // Retrieve transactions for the given financeMonthId
            var transactions = await _transactionRepository.QueryByFinanceMonthId(financeMonthId);

            return View(transactions);
        }
    }
}
