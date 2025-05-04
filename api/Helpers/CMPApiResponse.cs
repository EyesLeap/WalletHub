using api.Dtos.Currency;

namespace api.Helpers
{
    public class CMPListingsResponse
    {
        public CMPStatus Status { get; set; }
        public List<CMPListingData> Data { get; set; }
    }
    public class CMPQuoteResponse
    {
        public Dictionary<string, CMPListingData> Data { get; set; }
    }
    public class CMPStatus
    {
        public string Timestamp { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int Elapsed { get; set; }
        public int CreditCount { get; set; }
    }

    public class CMPListingData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public CMPQuote Quote { get; set; }
    }

    public class CMPQuote
    {
        public CMPUSD USD { get; set; }
    }

    public class CMPUSD
    {
        public decimal price { get; set; }
        public decimal market_cap { get; set; }
        public decimal volume_24h { get; set; }
        public decimal percent_change_24h { get; set; }
    }

}