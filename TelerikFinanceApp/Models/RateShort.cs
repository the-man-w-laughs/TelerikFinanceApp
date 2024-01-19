using System.ComponentModel.DataAnnotations;

namespace NbrbAPI.Models
{
    // Represents a short rate entity with details obtained from the National Bank of the Republic of Belarus API.
    public class RateShort
    {
        public int Cur_ID { get; set; }
        [Key]
        public System.DateTime Date { get; set; }
        public decimal? Cur_OfficialRate { get; set; }
    }
}
