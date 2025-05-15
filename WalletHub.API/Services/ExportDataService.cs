using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Repository;
using Microsoft.AspNetCore.Identity;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Exceptions;
using WalletHub.API.Helpers;
using WalletHub.API.Dtos.Export;

namespace WalletHub.API.Service
{
    public class ExportDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ITransactionService _transactionService;

        public ExportDataService(HttpClient httpClient, ITransactionService transactionService)
        {
            _httpClient = httpClient;
            _transactionService = transactionService;
        }

        public async Task<byte[]> ExportTransactionsAsync(ExportRequestDto request)
        {
                var transactions = await _transactionService.GetAllTransactionsAsync(request.PortfolioId, request.Query);

                if (transactions == null || !transactions.Any())
                    throw new TransactionNotFoundException("No transactions found.");

                var reportDto = transactions.ToReportDto(request.PortfolioId, request.Format);

                var endpoint = $"/api/export/{request.Format.ToString().ToLower()}/transactions";
                var response = await _httpClient.PostAsJsonAsync(endpoint, reportDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }

        }



}