using System.Security.Claims;
using api.Dtos.AssetDtos;
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
    [Route("api/portfolios")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPortfolioService _portfolioService;
        private readonly ITransactionService _transactionService;
        private readonly ICoinMarketCapService _cmpService;
        private readonly IAssetService _assetService;


        public PortfolioController(UserManager<AppUser> userManager,
        IPortfolioService portfolioService,
        ITransactionService transactionService,
        ICoinMarketCapService cmpService,
        IAssetService assetService)
        {
            _userManager = userManager;                   
            _portfolioService = portfolioService;
            _transactionService = transactionService;
            _cmpService = cmpService;
            _assetService = assetService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUserPortfolios()
        {
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var appUser = await _userManager.FindByNameAsync(username);     
            if (appUser == null) return Unauthorized();

            var portfolioDtos = await _portfolioService.GetAllUserPortfolios(appUser.Id);
            if (portfolioDtos == null) return NotFound();
            
            return Ok(portfolioDtos);      
        }

        [HttpGet("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int portfolioId)
        {
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized();

            var portfolio = await _portfolioService.GetPortfolioById(portfolioId);
            if (portfolio == null) return NotFound();

            return Ok(portfolio.ToPortfolioDto());      
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePortfolio([FromBody] CreatePortfolioDto createPortfolioDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized();

            var portfolioDto = await _portfolioService.CreatePortfolio(appUser, createPortfolioDto);

            return CreatedAtAction(nameof(GetById), new { portfolioId = portfolioDto.Id }, portfolioDto);
        }

        [HttpGet("{portfolioId:int}/total-value")]
        [Authorize]
        public async Task<IActionResult> GetPortfolioTotalValue(int portfolioId)
        {
            var portfolioValue = await _portfolioService.GetPortfolioTotalValue(portfolioId);
            if (portfolioValue == null)
            {
                return NotFound("Portfolio not found");
            }

            return Ok(portfolioValue);
        }

        [HttpGet("{portfolioId:int}/daily-change")]
        [Authorize]
        public async Task<IActionResult> GetPortfolioDailyChange(int portfolioId)
        {
            var portfolioDailyChange = await _portfolioService.GetPortfolioDailyChange(portfolioId);
            if (portfolioDailyChange == null)
            {
                return NotFound("No daily change or portfolio does not exist");
            }

            return Ok(portfolioDailyChange);
        }
        
        [HttpDelete("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio([FromRoute] int portfolioId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized();

            var portfolioModel = await _portfolioService.DeleteAsync(appUser, portfolioId);

            if (portfolioModel == null) return NotFound("Portfolio does not exist");
            
            return NoContent();
        }

        [HttpPatch("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdatePortfolioName([FromRoute] int portfolioId,[FromBody] EditPortfolioNameDto editedPortfolio)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized();

            var portfolioModel = await _portfolioService.UpdateNameAsync(appUser, portfolioId, editedPortfolio.Name);

            if (portfolioModel == null) return NotFound("Portfolio does not exist");
            
            return Ok(portfolioModel);
        }   

       
    }
}