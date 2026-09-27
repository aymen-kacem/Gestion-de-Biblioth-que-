using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoworkingGes.Models
{
    public class Context : IdentityDbContext<
        Utlisateur,
        IdentityRole<int>,
        int,
        IdentityUserClaim<int>,
        IdentityUserRole<int>,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
    {
        public Context(DbContextOptions options) : base(options) { }

        public DbSet<Abonnement> abonnement { get; set; }
        public DbSet<Espace> espace { get; set; }
        public DbSet<Maintenance> maintenance { get; set; }
        public DbSet<Reservation> reservation { get; set; }
        public DbSet<Ressource> ressource { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CRITICAL: Call base method first to configure Identity entities
            base.OnModelCreating(modelBuilder);

            // Configure Identity composite keys explicitly
            modelBuilder.Entity<IdentityUserLogin<int>>(b =>
            {
                b.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
            });

            modelBuilder.Entity<IdentityUserRole<int>>(b =>
            {
                b.HasKey(ur => new { ur.UserId, ur.RoleId });
            });

            modelBuilder.Entity<IdentityUserToken<int>>(b =>
            {
                b.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });
            });

            // ========== ESPACE RELATIONS ==========

            // Espace -> Utilisateur (Admin qui crée l'espace) - AJOUTÉ
            modelBuilder.Entity<Espace>()
                .HasOne(e => e.Utilisateur)
                .WithMany(u => u.Espaces)
                .HasForeignKey(e => e.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            // Espace -> Reservations (NO ACTION - prevent cascade conflicts)
            modelBuilder.Entity<Espace>()
                .HasMany(e => e.Reservations)
                .WithOne(r => r.Espace)
                .HasForeignKey(r => r.EspaceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Espace -> Ressources (CASCADE - delete resources when espace is deleted)
            modelBuilder.Entity<Espace>()
                .HasMany(e => e.Ressources)
                .WithOne(r => r.Espace)
                .HasForeignKey(r => r.EspaceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Espace -> Maintenances (NO ACTION - prevent cascade conflicts)
            modelBuilder.Entity<Espace>()
                .HasMany(e => e.Maintenances)
                .WithOne(m => m.Espace)
                .HasForeignKey(m => m.EspaceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Espace -> Abonnements (NO ACTION - prevent cascade conflicts)
            modelBuilder.Entity<Espace>()
                .HasMany(e => e.Abonnements)
                .WithOne(a => a.Espace)
                .HasForeignKey(a => a.EspaceId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========== RESERVATION RELATIONS ==========

            // Utilisateur -> Reservations
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Utilisateur)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== ABONNEMENT RELATIONS ==========

            // Utilisateur -> Abonnements
            modelBuilder.Entity<Abonnement>()
                .HasOne(a => a.Utilisateur)
                .WithMany(u => u.Abonnements)
                .HasForeignKey(a => a.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== MAINTENANCE RELATIONS ==========

            // Utilisateur (Technicien) -> Maintenances
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Technicien)
                .WithMany(u => u.Maintenances)
                .HasForeignKey(m => m.TechnicienId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== RESSOURCE RELATIONS ==========

            // Utilisateur -> Ressources (Admin qui crée la ressource)
            modelBuilder.Entity<Ressource>()
                .HasOne(r => r.Utilisateur)
                .WithMany(u => u.Ressources)
                .HasForeignKey(r => r.UtilisateurId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========== PRIMARY KEYS ==========

            modelBuilder.Entity<Espace>().HasKey(e => e.Id);
            modelBuilder.Entity<Reservation>().HasKey(r => r.Id);
            modelBuilder.Entity<Ressource>().HasKey(r => r.Id);
            modelBuilder.Entity<Maintenance>().HasKey(m => m.Id);
            modelBuilder.Entity<Abonnement>().HasKey(a => a.Id);
        }
    }
}