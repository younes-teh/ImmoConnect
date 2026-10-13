namespace ImmoConnect.Api.Models.Domain
{
    public class Client
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telephone { get; set; }

        public List<Visite> Visites { get; set; } = new();
    }
}