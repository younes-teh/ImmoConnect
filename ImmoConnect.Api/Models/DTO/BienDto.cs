using ImmoConnect.Api.Models.Domain;

namespace ImmoConnect.Api.Models.DTO
{
    public class BienDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public int Surface { get; set; }
        public string Ville { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public TypeBien Type { get; set; }
        public StatutBien Statut { get; set; }
        public DateTime DateCreation { get; set; }
        public int VisitesCount { get; set; }
    }
}