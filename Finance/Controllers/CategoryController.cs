using Finance.Models;
using Finance.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.Query();

            return View(categories);
        }

        // Action for Upsert Category (Insert or Update)
        public async Task<IActionResult> UpsertCategory(int? id)
        {
            if (id == null)
            {
                // If no ID is passed, it's an insert operation.
                return View(new Category()); // Provide an empty category for insert.
            }

            // If an ID is passed, it's an update operation, so we retrieve the existing category.
            var category = await _categoryRepository.Select(id.Value);

            if (category == null)
            {
                // If category is not found, return a not found response.
                return NotFound();
            }

            return View(category); // Return the category for editing
        }

        // POST action for inserting or updating category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    // If the category ID is 0, it's an insert operation.
                    await _categoryRepository.Insert(category);
                    return RedirectToAction(nameof(Index)); // Redirect to Index after insert
                }
                else
                {
                    // If the category has an ID, it's an update operation.
                    await _categoryRepository.Update(category);
                    return RedirectToAction(nameof(Index)); // Redirect to Index after update
                }
            }

            // If model state is invalid, return to the same view.
            return View(category);
        }

        // Action for deleting a category
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryRepository.Delete(id);
            if (success)
            {
                return RedirectToAction(nameof(Index)); // Redirect to Index after delete
            }

            // Handle delete failure (if category doesn't exist, etc.)
            return NotFound();
        }
    }
}