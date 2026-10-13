using ImmoConnect.Api.Models.Domain;

namespace ImmoConnect.Api.Repository
{
    public interface IBienRepository
    {
        Task<List<Bien>> GetAllAsync(string? ville, TypeBien? type, decimal? prixMax);
        Task<Bien?> GetByIdAsync(int id);
        Task<Bien> CreateAsync(Bien bien);
        Task<Bien?> UpdateAsync(int id, Bien bien);
        Task<DeleteBienResult> DeleteAsync(int id);
    }
}
