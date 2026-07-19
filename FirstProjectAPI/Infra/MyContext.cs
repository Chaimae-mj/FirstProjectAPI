using Microsoft.EntityFrameworkCore;
using FirstProjectAPI.Models;

namespace FirstProjectAPI.Infra
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        public DbSet<Formateur> Formateurs { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Sessionf> Sessions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Modulef> Modules { get; set; }
        public DbSet<Modalite> Modalites { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ReponseApprenant> ReponsesApprenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sessionf>()
                .HasMany(s => s.Users)
                .WithMany(u => u.Sessions)
                .UsingEntity<Dictionary<string, object>>(
                    "Inscription",
                    j => j.HasOne<User>().WithMany().HasForeignKey("IdUser"),
                    j => j.HasOne<Sessionf>().WithMany().HasForeignKey("IdSession")
                );
            modelBuilder.Entity<Formation>()
    .HasOne(f => f.Formateur)
    .WithMany(fm => fm.Formations)
    .HasForeignKey(f => f.FormateurId)
    .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Modulef>()
    .HasIndex(m => new { m.IdFormation, m.Ordre })
    .IsUnique();
        }
    }
}