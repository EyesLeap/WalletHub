using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("PortfolioSnapshots")]
    public class PortfolioSnapshot
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "decimal(34,18)")]
        public decimal TotalValueUSD { get; set; }
        public Portfolio Portfolio { get; set; }
    }

}