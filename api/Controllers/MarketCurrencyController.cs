using api.Data;
using api.Dtos.Currency;
using api.Helpers;
using api.Interfaces;
using api.Models;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
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
            {
                return NotFound("No popular currencies found.");
            }
            return Ok(currencies);
        }

        
    }
}