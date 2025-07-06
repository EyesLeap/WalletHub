using WalletHub.API.Caching;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;


namespace WalletHub.Tests.UnitTests.Services
{

    public class TokenServiceTests : IDisposable
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JWT:SigningKey", "ThisIsAReallyLongSigningKeyThatIsAtLeast512BitsLongForHMACSHA512"},
                {"JWT:Issuer", "Issuer"},
                {"JWT:Audience", "Audience"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(configuration);
        }
        
        [Fact]
        public void CreateToken_ReturnsValidToken()
        {
            var user = new AppUser
            {
                Email = "test@example.com",
                UserName = "TestUser"
            };

            var token = _tokenService.CreateToken(user);

            Assert.NotNull(token);

            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
            var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value;

            Assert.Equal("test@example.com", emailClaim);
            Assert.Equal("TestUser", usernameClaim);
        }

        public void Dispose()
        {
            
        }
    }
}