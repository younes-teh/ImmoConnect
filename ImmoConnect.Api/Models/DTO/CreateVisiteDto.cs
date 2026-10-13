using System.ComponentModel.DataAnnotations;

namespace ImmoConnect.Api.Models.DTO
{
    public class CreateVisiteDto
    {
        [Required]
        public int BienId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime DateVisite { get; set; }

        public string? Commentaire { get; set; }
    }
}