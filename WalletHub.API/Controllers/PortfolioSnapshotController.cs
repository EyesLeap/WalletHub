using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Exceptions;
using WalletHub.API.Extensions;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WalletHub.API.Controllers
{
    [Route("api/portfolio-snapshots")]
    [ApiController]
    [Authorize]
    public class PortfolioSnapshotController : ControllerBase
    {
        private readonly IPortfolioSnapshotService _portfolioSnapshotService;
        private readonly IPortfolioService _portfolioService;

        public PortfolioSnapshotController(IPortfolioSnapshotService portfolioSnapshotService, IPortfolioService portfolioService)
        {
            _portfolioSnapshotService = portfolioSnapshotService;
            _portfolioService = portfolioService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateSnapshot([FromBody] int portfolioId)
        {
            var portfolio = await _portfolioService.GetPortfolioById(portfolioId);
            if (portfolio == null)          
                throw new PortfolioNotFoundException(portfolioId);
            
            var portfolioTotalValue = await _portfolioService.GetPortfolioTotalValue(portfolioId);
            var snapshot = await _portfolioSnapshotService.CreateSnapshotAsync(portfolioId, portfolioTotalValue);

            return Ok(snapshot);
        }

        
        [HttpGet("{portfolioId:int}")]
        public async Task<IActionResult> GetPortfolioSnapshots(int portfolioId, [FromQuery] PortfolioSnapshotRange snapshotRange)
        {
            var snapshots = await _portfolioSnapshotService.GetPortfolioSnapshotsAsync(portfolioId, snapshotRange);

            return Ok(snapshots ?? new List<PortfolioSnapshot>());
        }
    }

}