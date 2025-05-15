using WalletHub.API.Dtos.TransactionDtos;

namespace WalletHub.ExportService.Interfaces
{
    public interface IPdfGenerationService
    {
        Task<byte[]> GenerateTransactionReportPdfAsync(TransactionReportDto reportDto);
    }
}