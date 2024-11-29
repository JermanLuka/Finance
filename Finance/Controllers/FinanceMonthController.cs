using Finance.Models;
using Finance.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Finance.Controllers
{
    public class FinanceMonthController : Controller
    {
        private readonly ILogger<FinanceMonthController> _logger;
        private readonly IFinanceMonthRepository _financeMonthRepository;
        private readonly IMonthRepository _monthRepository;

        public FinanceMonthController(ILogger<FinanceMonthController> logger, IFinanceMonthRepository financeMonthRepository, IMonthRepository monthRepository)
        {
            _logger = logger;
            _financeMonthRepository = financeMonthRepository;
            _monthRepository = monthRepository;
        }

        public async Task<IActionResult> Index()
        {
            var financeMonths = await _financeMonthRepository.Query();
            return View(financeMonths);
        }

        [HttpGet]
        public async Task<IActionResult> UpsertFinanceMonth(long? id)
        {
            ViewBag.Months = new SelectList(await _monthRepository.Query(), "Id", "Name");

            if (id == null)
            {
                // For create
                return View(new FinanceMonth());
            }
            else
            {
                // For update
                var financeMonth = await _financeMonthRepository.Select((long)id);
                if (financeMonth == null)
                {
                    return NotFound();
                }
                return View(financeMonth);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertFinanceMonth(FinanceMonth financeMonth)
        {
            if (ModelState.IsValid)
            {
                if (financeMonth.Id == 0)
                {
                    // Insert
                    await _financeMonthRepository.Insert(financeMonth);
                }
                else
                {
                    // Update
                    await _financeMonthRepository.Update(financeMonth);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(financeMonth);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _financeMonthRepository.Delete(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Transactions(long id)
        {
            var financeMonth = await _financeMonthRepository.Select(id);

            if (financeMonth == null)
            {
                return NotFound();
            }

            // Pass the FinanceMonth object with the Month included to the view
            return RedirectToAction("Index", "Transaction", new { financeMonthId = id, monthName = financeMonth.Month.Name });
        }
    }
}
