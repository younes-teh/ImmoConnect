using ImmoConnect.Api.Data;
using ImmoConnect.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ImmoConnect.Api.Repository
{
    public class BienRepository : IBienRepository
    {
        private readonly AppDbContext dbContext;

        public BienRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Bien>> GetAllAsync(string? ville, TypeBien? type, decimal? prixMax)
        {
            var query = dbContext.Biens.Include(b => b.Visites).AsQueryable();

            if (!string.IsNullOrWhiteSpace(ville))
                query = query.Where(b => b.Ville.Contains(ville));

            if (type.HasValue)
                query = query.Where(b => b.Type == type.Value);

            if (prixMax.HasValue)
                query = query.Where(b => b.Prix <= prixMax.Value);

            return await query.OrderByDescending(b => b.DateCreation).ToListAsync();
        }

        public async Task<Bien?> GetByIdAsync(int id)
        {
            return await dbContext.Biens
                .Include(b => b.Visites)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Bien> CreateAsync(Bien bien)
        {
            await dbContext.Biens.AddAsync(bien);
            await dbContext.SaveChangesAsync();
            return bien;
        }

        public async Task<Bien?> UpdateAsync(int id, Bien bien)
        {
            var bienDomain = await dbContext.Biens
                .Include(b => b.Visites)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bienDomain == null)
            {
                return null;
            }

            bienDomain.Titre = bien.Titre;
            bienDomain.Description = bien.Description;
            bienDomain.Prix = bien.Prix;
            bienDomain.Surface = bien.Surface;
            bienDomain.Ville = bien.Ville;
            bienDomain.Adresse = bien.Adresse;
            bienDomain.Type = bien.Type;
            bienDomain.Statut = bien.Statut;

            await dbContext.SaveChangesAsync();
            return bienDomain;
        }

        public async Task<DeleteBienResult> DeleteAsync(int id)
        {
            var bien = await dbContext.Biens
                .Include(b => b.Visites)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bien == null) return DeleteBienResult.NotFound;
            if (bien.Visites.Any()) return DeleteBienResult.HasVisites;

            dbContext.Biens.Remove(bien);
            await dbContext.SaveChangesAsync();
            return DeleteBienResult.Deleted;
        }
    }
}