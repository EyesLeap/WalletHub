using System.ComponentModel.DataAnnotations;
using System.Transactions;
using api.Helpers;

namespace api.Dtos.TransactionDtos
{
    public class TransactionQueryDto
    {
        public string? AssetSymbol { get; set; }  
        public TransactionType? TransactionType { get; set; }  
        public int Page { get; set; } = 1;       
        public int PageSize { get; set; } = 10;    
        public string SortBy { get; set; } = "date"; 
        public bool SortDescending { get; set; } = true; 
    }



}