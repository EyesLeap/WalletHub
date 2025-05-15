using WalletHub.API.Helpers;

namespace WalletHub.API.Dtos.TransactionDtos
{
    public class TransactionReportDto
    {
        public int PortfolioId { get; set; } 
        public ExportFormat Format { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();  
    }

}