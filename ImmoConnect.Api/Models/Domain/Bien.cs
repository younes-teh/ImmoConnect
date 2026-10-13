using System.ComponentModel;

namespace ImmoConnect.Api.Models.Domain
{
    public class Bien
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public int Surface { get; set; }
        public string Ville { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public TypeBien Type { get; set; }
        public StatutBien Statut { get; set; } = StatutBien.Disponible;
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        public List<Visite> Visites { get; set; } = new();
    }
}