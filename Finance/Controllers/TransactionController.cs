using Finance.Models;
using Finance.Repository;
using Finance.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Finance.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFinanceMonthRepository _financeMonthRepository;

        public TransactionController(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository, IFinanceMonthRepository financeMonthRepository)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
            _financeMonthRepository = financeMonthRepository;
        }

        public async Task<IActionResult> Index(long financeMonthId, string monthName)
        {
            ViewBag.FinanceMonthId = financeMonthId;
            ViewBag.MonthName = monthName;

            var transactions = await _transactionRepository.QueryByFinanceMonthId(financeMonthId);
            return View(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> UpsertTransaction(long? id, long financeMonthId, string monthName)
        {
            ViewBag.Categories = new SelectList(await _categoryRepository.Query(), "Id", "Name");
            ViewBag.MonthName = monthName;

            if (id == null || id == 0)
            {
                // Create new transaction
                return View(new Transaction { FinanceMonthId = financeMonthId });
            }
            else
            {
                // Edit existing transaction
                var transaction = await _transactionRepository.Select((long)id);
                if (transaction == null)
                {
                    return NotFound();
                }
                return View(transaction);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.Id == 0)
                {
                    await _transactionRepository.Insert(transaction);
                }
                else
                {
                    await _transactionRepository.Update(transaction);
                }

                // Retrieve the month name using FinanceMonthId
                var financeMonth = await _financeMonthRepository.Select(transaction.FinanceMonthId);
                var monthName = financeMonth?.Month?.Name;

                return RedirectToAction("Index", new { financeMonthId = transaction.FinanceMonthId, monthName });
            }

            // Retrieve categories again in case of an error
            ViewBag.Categories = new SelectList(await _categoryRepository.Query(), "Id", "Name");

            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTransaction(long id)
        {
            var transaction = await _transactionRepository.Select(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var financeMonth = await _financeMonthRepository.Select(transaction.FinanceMonthId);
            var monthName = financeMonth?.Month?.Name;

            await _transactionRepository.Delete(id);
            return RedirectToAction("Index", new { financeMonthId = transaction.FinanceMonthId, monthName });
        }
    }
}
