using Finance.DTO;
using Finance.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Services.IServices
{
    public interface ITransactionService
    {
        Task<List<GroupedTransactionDto>> QueryByMonthAndYearAsync(int monthId, int yearId);
        Task<List<CategorySummaryDto>> GetCategoryTotalsByMonthAndYearAsync(int monthId, int year);

        Task<(double Income, double Expenses)> GetIncomeVsExpensesAsync(int monthId, int year);
    }
}
