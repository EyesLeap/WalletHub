using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace WalletHub.API.Validators.Portfolio
{
    public class CreatePortfolioDtoValidator : AbstractValidator<CreatePortfolioDto>
    {
        public CreatePortfolioDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Portfolio name is required.")
                .Length(1, 50).WithMessage("Portfolio name must be between 1 and 50 characters.");
        }
    }
}