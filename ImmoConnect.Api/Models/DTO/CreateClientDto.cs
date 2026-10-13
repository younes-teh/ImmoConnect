using System.ComponentModel.DataAnnotations;

namespace ImmoConnect.Api.Models.DTO
{
    public class CreateClientDto
    {
        [Required, MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public string? Telephone { get; set; }
    }
}