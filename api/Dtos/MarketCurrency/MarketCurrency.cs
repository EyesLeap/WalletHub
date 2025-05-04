namespace api.Dtos.Currency
{
    public class MarketCurrency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; } 
        public string Slug { get; set; } 
        public decimal Price { get; set; } 
        public decimal Volume24h { get; set; }
        public decimal PercentChange1h { get; set; } 
        public decimal PercentChange24h { get; set; }
        public decimal PercentChange7d { get; set; } 
        public decimal MarketCap { get; set; } 
        public DateTime LastUpdated { get; set; } 
        public string LogoUrl { get; set; } 
        public string Description { get; set; } 
        public string Website { get; set; } 
        public string TechnicalDoc { get; set; } 
        public string SourceCode { get; set; } 
        public string Platform { get; set; } 
        public string Category { get; set; } 
        public bool IsActive { get; set; } 
        public DateTime DateAdded { get; set; } 
        public string[] Tags { get; set; } 
    }
}