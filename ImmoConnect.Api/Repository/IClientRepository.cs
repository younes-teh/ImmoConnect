using ImmoConnect.Api.Models.Domain;

namespace ImmoConnect.Api.Repository
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllAsync(string? search);
        Task<Client?> GetByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<Client> CreateAsync(Client client);
        Task<DeleteClientResult> DeleteAsync(int id);
    }
}