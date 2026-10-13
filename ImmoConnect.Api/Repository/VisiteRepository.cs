using ImmoConnect.Api.Data;
using ImmoConnect.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ImmoConnect.Api.Repository
{
    public class VisiteRepository : IVisiteRepository
    {
        private readonly AppDbContext dbContext;
        public VisiteRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> ConflitExisteAsync(int bienId, DateTime dateVisite)
        {
            var debut = dateVisite.AddMinutes(-60);
            var fin = dateVisite.AddMinutes(60);

            return await dbContext.Visites.AnyAsync(v =>
                v.BienId == bienId &&
                v.Statut == StatutVisite.Planifiee &&
                v.DateVisite > debut &&
                v.DateVisite < fin);
        }

        public async Task<Visite> CreateAsync(Visite visite)
        {
            await dbContext.Visites.AddAsync(visite);
            await dbContext.SaveChangesAsync();

            return (await GetByIdAsync(visite.Id))!;
        }

        public async Task<List<Visite>> GetAllAsync(int? bienId, int? clientId, StatutVisite? statut)
        {
            var query = dbContext.Visites.Include(c => c.BienId).Include(c => c.ClientId).AsQueryable();
            if (bienId.HasValue)
            {
                query = query.Where(v => v.BienId == bienId.Value);
            }
            if (clientId.HasValue)
                query = query.Where(v => v.ClientId == clientId.Value);

            if (statut.HasValue)
                query = query.Where(v => v.Statut == statut.Value);

            return await query.OrderBy(v => v.DateVisite).ToListAsync();
        }

        public async Task<Visite?> GetByIdAsync(int id)
        {
            var visiteDomain = dbContext.Visites
                .Include(c => c.BienId)
                .Include(c => c.ClientId).AsQueryable();

            return await visiteDomain.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Visite?> UpdateStatutAsync(int id, StatutVisite statut)
        {
            var viste = await GetByIdAsync(id);
            if(viste == null)
            {
                return null;
            }

            viste.Statut = statut;

            await dbContext.SaveChangesAsync();

            return viste;
        }
    }
}
