using System.ComponentModel.DataAnnotations;
using System.Transactions;
using WalletHub.API.Helpers;
using Microsoft.EntityFrameworkCore;
using WalletHub.API.Dtos.TransactionDtos;

namespace WalletHub.API.Dtos.Export
{
    public class ExportRequestDto
    {
        public int PortfolioId { get; set; }
        public ExportFormat Format { get; set; }
        public TransactionQueryDto Query { get; set; } = new TransactionQueryDto();
    }




}