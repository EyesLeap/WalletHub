using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Exceptions;
using WalletHub.API.Extensions;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WalletHub.API.Dtos.Export;

namespace WalletHub.API.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        
        private readonly UserManager<AppUser> _userManager;
        private readonly ExportDataService _exportDataService;
        private readonly IPortfolioService _portfolioService;
        private readonly ITransactionService _transactionService;
        private readonly ICoinMarketCapService _fmpService;

        public TransactionController(UserManager<AppUser> userManager,
        ICoinMarketCapService fmpService,
        IPortfolioService portfolioService,
        ITransactionService transactionService,
        ExportDataService exportDataService)
        {
            _userManager = userManager;
            _fmpService = fmpService;
            _portfolioService = portfolioService;       
            _transactionService = transactionService;
            _exportDataService = exportDataService;
        }


        
        [HttpPost("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> CreateTransaction(int portfolioId, [FromBody] CreateTransactionDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transaction = await _transactionService.CreateTransactionAsync(portfolioId, dto, cancellationToken);

            if (transaction is null)
                throw new TransactionProcessingException("Transaction could not be processed");

            return CreatedAtAction(nameof(GetTransactionById), new { portfolioId = portfolioId, transactionId = transaction.Id }, transaction.ToTransactionDto());

        }

        [HttpPost("{portfolioId:int}/export")]
        [Authorize]
        public async Task<IActionResult> ExportTransactions(
            [FromBody] ExportRequestDto requestDto)
        {
            var userName = User.GetUsername();
            if (string.IsNullOrEmpty(userName))
                throw new UnauthorizedException();

            var fileBytes = await _exportDataService.ExportTransactionsAsync(requestDto);

            var contentType = requestDto.Format.ToString().ToLower() switch
            {
                "pdf" => "application/pdf",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            return File(fileBytes, contentType, $"transactions.{requestDto.Format.ToString().ToLower()}");
        }


        [HttpGet("{portfolioId:int}/{transactionId:int}")]
        [Authorize]
        public async Task<IActionResult> GetTransactionById(int portfolioId, int transactionId)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);

            if (transaction is null)
                throw new TransactionNotFoundException("Transaction not found");

            return Ok(transaction.ToTransactionDto());
        }


        [HttpGet("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> GetAllPortfolioTransactions(int portfolioId, 
            [FromQuery] TransactionQueryDto queryDto)
        {
            var transactions = await _transactionService.GetAllTransactionsAsync(portfolioId, queryDto);
            
            return Ok(transactions);
        }



        [HttpDelete("{transactionId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteTransaction([FromRoute] int transactionId)
        {
            var transaction = await _transactionService.DeleteAsync(transactionId);

            if (transaction is null)
                throw new TransactionNotFoundException("Transaction does not exist");

            return NoContent();
        }

        
    }
}