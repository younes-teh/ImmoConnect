namespace ImmoConnect.Api.Models.Domain
{
    public class Visite
    {
        public int Id { get; set; }

        public int BienId { get; set; }
        public Bien? Bien { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTime DateVisite { get; set; }
        public StatutVisite Statut { get; set; } = StatutVisite.Planifiee;
        public string? Commentaire { get; set; }
    }
}