using Finance.Models;
using Finance.Repository.IRepository;

namespace Finance.Services
{
    public class MonthService
    {
        private readonly IMonthRepository _monthRepository;

        public MonthService(IMonthRepository monthRepository)
        {
            _monthRepository = monthRepository;
        }

        //public async Task<Month> QueryMonths()
        //{
        //    try
        //    {
        //        var data = await _monthRepository.Query();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error in MonthService " + ex.Message);
        //    }
        //}
    }
}
