using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.ExportService.Interfaces;

namespace WalletHub.ExportService.Controllers
{

    [ApiController]
    [Route("api/export/pdf")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfGenerationService _pdfGenerationService;

        public PdfController(IPdfGenerationService pdfGenerationService)
        {
            _pdfGenerationService = pdfGenerationService;
        }

        [HttpPost("transactions")]
        public async Task<IActionResult> GenerateTransactionPdf([FromBody] TransactionReportDto reportDto)
        {           
            try
            {
                var pdfBytes = await _pdfGenerationService.GenerateTransactionReportPdfAsync(reportDto);

                if (pdfBytes == null || pdfBytes.Length == 0)
                    return BadRequest("Failed to generate PDF.");

                return File(pdfBytes, "application/pdf", "transaction-report.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating PDF: {ex.Message}");
            }
        }
    }
}

