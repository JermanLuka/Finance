using System.ComponentModel.DataAnnotations;

namespace Finance.Models
{
    public class Month
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int MonthOrdinal {  get; set; }
    }
}