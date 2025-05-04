using System.ComponentModel.DataAnnotations.Schema;
using api.Models;

[Table("Assets")]
public class Asset
{
    public int Id { get; set; }
    public int PortfolioId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    private decimal _amount;
    [Column(TypeName = "decimal(34,18)")]
    public decimal Amount
    {
        get => _amount;
        set
        {
            if (value < 0)
                _amount = 0;
            else
                _amount = value;
        }
    }

    private decimal _averagePurchasePrice;
    [Column(TypeName = "decimal(34,18)")]
    public decimal AveragePurchasePrice
    {
        get => _averagePurchasePrice;
        set
        {
            if (value < 0)
                _averagePurchasePrice = 0;  
            else
                _averagePurchasePrice = value;
        }
    }

    public Portfolio Portfolio { get; set; }
}
