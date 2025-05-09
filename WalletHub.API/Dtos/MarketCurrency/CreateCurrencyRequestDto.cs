using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletHub.API.Dtos.Currency
{
    public class CreateCurrencyRequestDto
    {
        [Required]
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters")]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot be over 50 characters")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 5000000000000, ErrorMessage = "MarketCap must be between 1 and 5T")]
        public long MarketCap { get; set; }

        [Range(0, 1000000000000, ErrorMessage = "Volume24h must be between 0 and 1T")]
        public decimal Volume24h { get; set; }
    }

}