using System;
using System.ComponentModel.DataAnnotations;

namespace NbrbAPI.Models
{
    // Represents a rate entity with details obtained from the National Bank of the Republic of Belarus API.
    public class Rate
    {
        [Key]
        public int Cur_ID { get; set; }
        public DateTime Date { get; set; }
        public string Cur_Abbreviation { get; set; }
        public int Cur_Scale { get; set; }
        public string Cur_Name { get; set; }
        public decimal? Cur_OfficialRate { get; set; }
    }
}