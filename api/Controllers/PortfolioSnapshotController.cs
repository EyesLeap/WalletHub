using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Dtos.TransactionDtos;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
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
            {
                return NotFound("Portfolio not found");
            }

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