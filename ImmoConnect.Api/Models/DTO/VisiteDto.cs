using ImmoConnect.Api.Models.Domain;

namespace ImmoConnect.Api.Models.DTO
{
    public class VisiteDto
    {
        public int Id { get; set; }
        public int BienId { get; set; }
        public string BienTitre { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ClientNom { get; set; } = string.Empty;
        public DateTime DateVisite { get; set; }
        public StatutVisite Statut { get; set; }
        public string? Commentaire { get; set; }
    }
}