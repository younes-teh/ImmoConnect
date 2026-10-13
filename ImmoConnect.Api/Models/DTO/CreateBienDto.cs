using System.ComponentModel.DataAnnotations;
using ImmoConnect.Api.Models.Domain;

namespace ImmoConnect.Api.Models.DTO
{
    public class CreateBienDto
    {
        [Required, MaxLength(150)]
        public string Titre { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, double.MaxValue)]
        public decimal Prix { get; set; }

        [Range(1, int.MaxValue)]
        public int Surface { get; set; }

        [Required, MaxLength(80)]
        public string Ville { get; set; } = string.Empty;

        public string Adresse { get; set; } = string.Empty;

        public TypeBien Type { get; set; }
    }
}