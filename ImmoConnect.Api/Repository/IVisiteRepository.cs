using ImmoConnect.Api.Models.Domain;

namespace ImmoConnect.Api.Repository
{
    public interface IVisiteRepository
    {
        Task<List<Visite>> GetAllAsync(int? bienId, int? clientId, StatutVisite? statut);
        Task<Visite?> GetByIdAsync(int id);
        Task<bool> ConflitExisteAsync(int bienId, DateTime dateVisite);
        Task<Visite> CreateAsync(Visite visite);
        Task<Visite?> UpdateStatutAsync(int id, StatutVisite statut);
    }
}