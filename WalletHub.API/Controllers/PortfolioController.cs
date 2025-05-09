using System.Security.Claims;
using WalletHub.API.Dtos.AssetDtos;
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

namespace WalletHub.API.Controllers
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
            if (string.IsNullOrEmpty(username)) 
                throw new UnauthorizedException();

            var appUser = await _userManager.FindByNameAsync(username);     
            if (appUser == null) 
                 throw new UserNotFoundException(User.GetUsername());

            var portfolioDtos = await _portfolioService.GetAllUserPortfolios(appUser.Id);
            if (portfolioDtos == null) 
                throw new NotFoundException($"Portfolios for {appUser.Id} were not found");
                
            return Ok(portfolioDtos);      
        }

        [HttpGet("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int portfolioId)
        {
            var appUser = await _userManager.FindByNameAsync(User.GetUsername());
            if (appUser == null)     
                throw new UserNotFoundException(User.GetUsername());
            

            var portfolio = await _portfolioService.GetPortfolioById(portfolioId);
            if (portfolio == null)           
                throw new PortfolioNotFoundException(portfolioId);            

            return Ok(portfolio.ToPortfolioDto());
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePortfolio([FromBody] CreatePortfolioDto createPortfolioDto)
        {
            var appUser = await _userManager.FindByNameAsync(User.GetUsername());
            if (appUser == null)
                throw new UserNotFoundException(User.GetUsername());

            var portfolioDto = await _portfolioService.CreatePortfolio(appUser, createPortfolioDto);

            return CreatedAtAction(nameof(GetById), new { portfolioId = portfolioDto.Id }, portfolioDto);
        }


        [HttpGet("{portfolioId:int}/total-value")]
        [Authorize]
        public async Task<IActionResult> GetPortfolioTotalValue(int portfolioId)
        {
            var portfolioValue = await _portfolioService.GetPortfolioTotalValue(portfolioId);
            
            if (portfolioValue == null)
                throw new PortfolioNotFoundException(portfolioId);

            return Ok(portfolioValue);
        }


        [HttpGet("{portfolioId:int}/daily-change")]
        [Authorize]
        public async Task<IActionResult> GetPortfolioDailyChange(int portfolioId)
        {
            var portfolioDailyChange = await _portfolioService.GetPortfolioDailyChange(portfolioId);
            if (portfolioDailyChange == null)
                throw new PortfolioNotFoundException(portfolioId);

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
            if (appUser == null)
                 throw new UserNotFoundException(User.GetUsername());

            var portfolioModel = await _portfolioService.DeleteAsync(appUser, portfolioId);

            if (portfolioModel == null) 
                throw new PortfolioNotFoundException(portfolioId);
            
            return NoContent();
        }

        [HttpPatch("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdatePortfolioName([FromRoute] int portfolioId, [FromBody] UpdatePortfolioNameDto editedPortfolio)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) 
                throw new UserNotFoundException(User.GetUsername());

            var portfolioModel = await _portfolioService.UpdateNameAsync(appUser, portfolioId, editedPortfolio.Name);

            if (portfolioModel == null)      
                throw new PortfolioNotFoundException(portfolioId);
            

            return Ok(portfolioModel);
}


       
    }
}