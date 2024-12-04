using Finance.Models;
using Finance.Repository;
using Finance.Repository.IRepository;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text.RegularExpressions;

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
            var financeMonth = await _financeMonthRepository.Select(financeMonthId);

            ViewBag.MonthOrdinal = financeMonth.Month.MonthOrdinal;
            ViewBag.Categories = new SelectList(await _categoryRepository.Query(), "Id", "Name");
            ViewBag.MonthName = monthName;
            ViewBag.Year = financeMonth.Year;

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

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExtractText(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please upload a valid PDF file.";
                return View("Upload");
            }

            try
            {
                string extractedText;
                using (var stream = file.OpenReadStream())
                {
                    using (var pdfReader = new PdfReader(stream))
                    using (var pdfDocument = new PdfDocument(pdfReader))
                    {
                        extractedText = "";
                        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                        {
                            extractedText += PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i));
                            extractedText += "\n";
                        }
                    }
                }

                // Parse transactions from the extracted text
                var transactions = ParseTransactions(extractedText);

                // Option 1: Send transactions to the view
                ViewBag.Transactions = transactions;
                return View("Upload");

                // Option 2: Save transactions to database or other storage
                // _dbContext.Transactions.AddRange(transactions);
                // _dbContext.SaveChanges();
                // return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View("Upload");
            }
        }

        public static List<Transaction> ParseTransactions(string rawText)
        {
            var transactions = new List<Transaction>();
            var lines = rawText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            bool foundTable = false;
            var dateRegex = new Regex(@"^\d{2}\.\d{2}\.\d{4}"); // Matches dates like 01.10.2024

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip any lines before the actual data starts (look for header like "Datum")
                if (!foundTable && trimmedLine.Contains("Datum") && trimmedLine.Contains("Breme") && trimmedLine.Contains("Dobro"))
                {
                    foundTable = true;
                    continue; // Skip the header line itself
                }

                if (foundTable)
                {
                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                    // Split by spaces to extract columns
                    var columns = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Ensure the line has enough columns to extract data
                    if (columns.Length >= 6)
                    {
                        try
                        {
                            // Parse the date from the first column
                            var date = DateTime.ParseExact(columns[0], "dd.MM.yyyy", CultureInfo.InvariantCulture);

                            // Parse the Breme (expense) and Dobro (income) columns
                            double expense = 0, income = 0;
                            if (double.TryParse(columns[3].Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out expense))
                            {
                                expense = Math.Abs(expense); // Make sure expense is positive
                            }
                            if (double.TryParse(columns[4].Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out income))
                            {
                                income = Math.Abs(income); // Make sure income is positive
                            }

                            // If there's no income, treat it as an expense, otherwise income
                            double amount = expense > 0 ? expense : income;
                            bool isIncome = income > 0;

                            // Parse the description (everything after the last number column)
                            string description = string.Join(" ", columns.Skip(5));

                            // Create and add the transaction
                            transactions.Add(new Transaction
                            {
                                TransactionDate = date,
                                Amount = amount,
                                IsIncome = isIncome,
                                FinanceMonthId = 0, // Default value; to be updated as needed
                                CategoryId = 0,     // Default value; to be updated as needed
                                Category = null,
                                FinanceMonth = null
                            });
                        }
                        catch (Exception ex)
                        {
                            // Handle any parsing errors (e.g., invalid date or number format)
                            Console.WriteLine($"Error parsing line: {trimmedLine} - {ex.Message}");
                        }
                    }
                }
            }

            return transactions;
        }
    }
}
