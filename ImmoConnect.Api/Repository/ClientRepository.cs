using ImmoConnect.Api.Data;
using ImmoConnect.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ImmoConnect.Api.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext dbContext;

        public ClientRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await dbContext.Clients.AnyAsync(c => c.Email == email);
        }

        public async Task<Client> CreateAsync(Client client)
        {
            await dbContext.Clients.AddAsync(client);
            await dbContext.SaveChangesAsync();
            return client;
        }

        public async Task<DeleteClientResult> DeleteAsync(int id)
        {
            var client = await dbContext.Clients
                .Include(c => c.Visites)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return DeleteClientResult.NotFound;
            if (client.Visites.Any()) return DeleteClientResult.HasVisites;

            dbContext.Clients.Remove(client);
            await dbContext.SaveChangesAsync();
            return DeleteClientResult.Deleted;
        }

        public async Task<List<Client>> GetAllAsync(string? search)
        {
            var query = dbContext.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nom.Contains(search) || c.Email.Contains(search));
            }

            return await query.OrderBy(c => c.Nom).ToListAsync();
        }
    }
}