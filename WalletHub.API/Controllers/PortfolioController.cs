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
using WalletHub.API.Exceptions.NotFound;

namespace WalletHub.API.Controllers
{
    [Route("api/portfolios")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPortfolioService _portfolioService;
        public PortfolioController(UserManager<AppUser> userManager,
        IPortfolioService portfolioService)
        {
            _userManager = userManager;                   
            _portfolioService = portfolioService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUserPortfolios(CancellationToken cancellationToken)
        {
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username)) 
                throw new UnauthorizedException();

            var appUser = await _userManager.FindByNameAsync(username);     
            if (appUser is null) 
                 throw new UserNotFoundException(User.GetUsername());

            var portfolioDtos = await _portfolioService.GetAllUserPortfolios(appUser.Id, cancellationToken);
            if (portfolioDtos is null) 
                throw new NotFoundException($"Portfolios for {appUser.Id} were not found");
                
            return Ok(portfolioDtos);      
        }

        [HttpGet("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int portfolioId,
            CancellationToken cancellationToken)
        {
            var appUser = await _userManager.FindByNameAsync(User.GetUsername());
            if (appUser is null)     
                throw new UserNotFoundException(User.GetUsername());
            

            var portfolio = await _portfolioService.GetPortfolioById(portfolioId, cancellationToken);
            if (portfolio is null)           
                throw new PortfolioNotFoundException(portfolioId);            

            return Ok(portfolio);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePortfolio([FromBody] CreatePortfolioDto createPortfolioDto,
            CancellationToken cancellationToken)
        {
            var appUser = await _userManager.FindByNameAsync(User.GetUsername());
            if (appUser is null)
                throw new UserNotFoundException(User.GetUsername());

            var portfolioDto = await _portfolioService.CreatePortfolio(appUser, createPortfolioDto, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { portfolioId = portfolioDto.Id }, portfolioDto);
        }


        [HttpGet("{portfolioId:int}/total-value")]
        [Authorize]
        public async Task<IActionResult> GetPortfolioTotalValue(int portfolioId,
            CancellationToken cancellationToken)
        {
            var portfolioValue = await _portfolioService.GetPortfolioTotalValue(portfolioId, cancellationToken);
            
            if (portfolioValue is null)
                throw new PortfolioNotFoundException(portfolioId);

            return Ok(portfolioValue);
        }


        [HttpGet("{portfolioId:int}/daily-change")]
        [Authorize]
        public async Task<IActionResult> GetPortfolioDailyChange(int portfolioId, CancellationToken cancellationToken)
        {
            var portfolioDailyChange = await _portfolioService.GetPortfolioDailyChange(portfolioId, cancellationToken);
            if (portfolioDailyChange is null)
                throw new PortfolioNotFoundException(portfolioId);

            return Ok(portfolioDailyChange);
        }
        
        [HttpDelete("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio([FromRoute] int portfolioId,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser is null)
                 throw new UserNotFoundException(User.GetUsername());

            var portfolioModel = await _portfolioService.DeleteAsync(appUser, portfolioId, cancellationToken);

            if (portfolioModel is null) 
                throw new PortfolioNotFoundException(portfolioId);
            
            return NoContent();
        }

        [HttpPatch("{portfolioId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdatePortfolioName([FromRoute] int portfolioId,
            [FromBody] UpdatePortfolioNameDto editedPortfolio,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser is null) 
                throw new UserNotFoundException(User.GetUsername());

            var portfolioModel = await _portfolioService.UpdateNameAsync(appUser, portfolioId, editedPortfolio.Name, cancellationToken);

            if (portfolioModel is null)      
                throw new PortfolioNotFoundException(portfolioId);
            
            return Ok(portfolioModel);
        }


       
    }
}