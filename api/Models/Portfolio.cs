using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("Portfolios")]
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string AppUserId { get; set; } 
        public List<Asset> Assets { get; set; } = new List<Asset>();  
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public AppUser AppUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}