using ImmoConnect.Api.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImmoConnect.Api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Bien> Biens { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Visite> Visites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);   // obligatoire, en premier

            // BIEN
            modelBuilder.Entity<Bien>(entity =>
            {
                entity.Property(b => b.Titre).IsRequired().HasMaxLength(150);
                entity.Property(b => b.Ville).IsRequired().HasMaxLength(80);
                entity.Property(b => b.Prix).HasPrecision(18, 2);
                entity.Property(b => b.Type).HasConversion<string>().HasMaxLength(20);
                entity.Property(b => b.Statut).HasConversion<string>().HasMaxLength(20);
            });

            // CLIENT
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(c => c.Nom).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(c => c.Email).IsUnique();
            });

            // VISITE
            modelBuilder.Entity<Visite>(entity =>
            {
                entity.Property(v => v.Statut).HasConversion<string>();

                entity.HasOne(v => v.Bien)
                      .WithMany(b => b.Visites)
                      .HasForeignKey(v => v.BienId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Client)
                      .WithMany(c => c.Visites)
                      .HasForeignKey(v => v.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}