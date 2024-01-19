using System.ComponentModel.DataAnnotations;

namespace NbrbAPI.Models
{
    public class RateShort
    {
        public int Cur_ID { get; set; }
        [Key]
        public System.DateTime Date { get; set; }
        public decimal? Cur_OfficialRate { get; set; }
    }
}
