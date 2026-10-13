using ImmoConnect.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ImmoConnect.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Bien> Biens { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Visite> Visites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bien>(e =>
            {
                e.Property(b => b.Titre).IsRequired().HasMaxLength(150);
                e.Property(b => b.Ville).IsRequired().HasMaxLength(80);
                e.Property(b => b.Prix).HasPrecision(18, 2);
                e.Property(b => b.Type).HasConversion<string>().HasMaxLength(20);
                e.Property(b => b.Statut).HasConversion<string>().HasMaxLength(20);
            });

            modelBuilder.Entity<Client>(e =>
            {
                e.Property(c => c.Nom).IsRequired().HasMaxLength(100);
                e.Property(c => c.Email).IsRequired().HasMaxLength(150);
                e.HasIndex(c => c.Email).IsUnique();
            });

            modelBuilder.Entity<Visite>(e =>
            {
                e.Property(v => v.Statut).HasConversion<string>().HasMaxLength(20);

                e.HasOne(v => v.Bien)
                 .WithMany(b => b.Visites)
                 .HasForeignKey(v => v.BienId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(v => v.Client)
                 .WithMany(c => c.Visites)
                 .HasForeignKey(v => v.ClientId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}