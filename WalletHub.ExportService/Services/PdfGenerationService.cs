

using PdfSharp.Drawing;
using PdfSharp.Pdf;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.ExportService.Interfaces;

namespace WalletHub.ExportService.Services
{
    public class PdfGenerationService : IPdfGenerationService
    {
        public async Task<byte[]> GenerateTransactionReportPdfAsync(TransactionReportDto reportDto)
        {
            using (var ms = new MemoryStream())
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                var font = new XFont("Arial", 12);
                var headerFont = new XFont("Arial", 14, XFontStyleEx.Bold);
                var titleFont = new XFont("Arial", 16, XFontStyleEx.Bold);

                
                gfx.DrawString($"Transaction Report for Portfolio {reportDto.PortfolioId}", titleFont, XBrushes.Black,
                    new XRect(0, 20, page.Width, 30), XStringFormats.Center);

                
                var yPosition = 60;
                gfx.DrawString("Symbol", headerFont, XBrushes.Black, new XRect(40, yPosition, 100, 20), XStringFormats.TopLeft);
                gfx.DrawString("Amount", headerFont, XBrushes.Black, new XRect(140, yPosition, 100, 20), XStringFormats.TopLeft);
                gfx.DrawString("Price Per Coin (USD)", headerFont, XBrushes.Black, new XRect(240, yPosition, 120, 20), XStringFormats.TopLeft);
                gfx.DrawString("Total Cost (USD)", headerFont, XBrushes.Black, new XRect(360, yPosition, 120, 20), XStringFormats.TopLeft);
                gfx.DrawString("Date", headerFont, XBrushes.Black, new XRect(480, yPosition, 100, 20), XStringFormats.TopLeft);

                yPosition += 30;

                
                foreach (var transaction in reportDto.Transactions)
                {
                    gfx.DrawString(transaction.Symbol, font, XBrushes.Black, new XRect(40, yPosition, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString(transaction.Amount.ToString(), font, XBrushes.Black, new XRect(140, yPosition, 100, 20), XStringFormats.TopLeft);
                    gfx.DrawString(transaction.PricePerCoin.ToString("F2"), font, XBrushes.Black, new XRect(240, yPosition, 120, 20), XStringFormats.TopLeft);
                    gfx.DrawString(transaction.TotalCost.ToString("F2"), font, XBrushes.Black, new XRect(360, yPosition, 120, 20), XStringFormats.TopLeft);
                    gfx.DrawString(transaction.CreatedAt.ToString("yyyy-MM-dd"), font, XBrushes.Black, new XRect(480, yPosition, 100, 20), XStringFormats.TopLeft);

                    yPosition += 25;

                    
                    if (yPosition > page.Height - 40)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPosition = 40;

                        
                        gfx.DrawString("Symbol", headerFont, XBrushes.Black, new XRect(40, yPosition, 100, 20), XStringFormats.TopLeft);
                        gfx.DrawString("Amount", headerFont, XBrushes.Black, new XRect(140, yPosition, 100, 20), XStringFormats.TopLeft);
                        gfx.DrawString("Price Per Coin (USD)", headerFont, XBrushes.Black, new XRect(240, yPosition, 120, 20), XStringFormats.TopLeft);
                        gfx.DrawString("Total Cost (USD)", headerFont, XBrushes.Black, new XRect(360, yPosition, 120, 20), XStringFormats.TopLeft);
                        gfx.DrawString("Date", headerFont, XBrushes.Black, new XRect(480, yPosition, 100, 20), XStringFormats.TopLeft);

                        yPosition += 30;
                    }
                }

                document.Save(ms);
                return ms.ToArray();
            }
        }
    }

}
