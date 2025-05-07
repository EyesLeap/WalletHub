using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Dtos.TransactionDtos;
using api.Interfaces;
using api.Models;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace api.Validators.Portfolio
{
    public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
    {
        public CreateTransactionDtoValidator()
        {
            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Symbol is required")
                .MaximumLength(10).WithMessage("Symbol cannot be over 10 characters");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero");

            RuleFor(x => x.PricePerCoin)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.TransactionType)
                .IsInEnum().WithMessage("Invalid transaction type");

            RuleFor(x => x.CreatedAt)
                .NotEmpty().WithMessage("CreatedAt is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt cannot be in the future");
        }
    }
}