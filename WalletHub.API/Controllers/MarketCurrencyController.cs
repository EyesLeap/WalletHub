using WalletHub.API.Data;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using WalletHub.API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletHub.API.Exceptions;

namespace WalletHub.API.Controllers
{
    [Route("api/currency")]
    [ApiController]
    public class MarketCurrencyController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ICoinMarketCapService _cmpService;
        public MarketCurrencyController(ApplicationDBContext context,
         ICoinMarketCapService cmpService)
        {
            _context = context;
            _cmpService = cmpService;
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularCurrencies([FromQuery] int limit = 10)
        {
            var currencies = await _cmpService.GetPopularCurrenciesAsync(limit);
            if (currencies == null || !currencies.Any())     
                throw new NotFoundException("No popular currencies found.");
            
            return Ok(currencies);
        }

        
    }
}