using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Dtos.TransactionDtos;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        
        private readonly UserManager<AppUser> _userManager;
        private readonly IPortfolioService _portfolioService;
        private readonly ITransactionService _transactionService;
        private readonly ICoinMarketCapService _fmpService;

        public TransactionController(UserManager<AppUser> userManager,
        ICoinMarketCapService fmpService,
        IPortfolioService portfolioService,
        ITransactionService transactionService)
        {
            _userManager = userManager;
          
            _fmpService = fmpService;
            _portfolioService = portfolioService;       
            _transactionService = transactionService;
        }


        
        [HttpPost("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> CreateTransaction(int portfolioId, [FromBody] CreateTransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transaction = await _transactionService.CreateTransactionAsync(portfolioId, dto);

            if (transaction == null)
                return BadRequest("Transaction could not be processed");

            return CreatedAtAction(nameof(GetTransactionById), new { portfolioId = portfolioId, transactionId = transaction.Id }, transaction.ToTransactionDto());

        }
        [HttpGet("{portfolioId:int}/{transactionId:int}")]
        [Authorize]
        public async Task<IActionResult> GetTransactionById(int portfolioId, int transactionId)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);

            if (transaction == null) return NotFound("Transaction not found");

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
            if (transactionId <= 0)
            {
                return BadRequest("Invalid transaction ID.");
            }

            var transactionoModel = await _transactionService.DeleteAsync(transactionId);

            if (transactionoModel == null) return NotFound("Transaction does not exist");
            
            return NoContent();
        }
        
    }
}