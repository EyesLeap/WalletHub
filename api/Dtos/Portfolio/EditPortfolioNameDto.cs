using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.Dtos.Portfolio
{
    public class EditPortfolioNameDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Portfolio name must be between 1 and 50 characters.", MinimumLength = 1)]
        public string Name { get; set; }
    }

}