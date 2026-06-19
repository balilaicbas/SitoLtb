using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Models;

namespace SitoLtb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicationUser>?     ApplicationUsers      { get; set; }
        public DbSet<Post>?                Posts                 { get; set; }
        public DbSet<Tournament>?          Tournaments           { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; } = null!;

        // ── Coordinazione ────────────────────────────────────────────
        public DbSet<Progetto>        Progetti       { get; set; } = null!;
        public DbSet<ProgettoTask>    ProgettoTasks  { get; set; } = null!;
        public DbSet<TaskCommento>    TaskCommenti   { get; set; } = null!;
        public DbSet<ProgettoScadenza> ProgettoScadenze { get; set; } = null!;
        public DbSet<ProgettoNota>    ProgettoNote   { get; set; } = null!;
        public DbSet<ProgettoMembro>          ProgettoMembri        { get; set; } = null!;
        public DbSet<NotificaImpostazione>    NotificheImpostazioni  { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Chiavi primarie esplicite (nomi non seguono la convenzione EF "Id" / "ProgettoId")
            builder.Entity<Progetto>()            .HasKey(p => p.IdProgetto);
            builder.Entity<NotificaImpostazione>().HasKey(n => n.IdNotifica);
            builder.Entity<ProgettoTask>()   .HasKey(t => t.IdTask);
            builder.Entity<TaskCommento>()   .HasKey(c => c.IdCommento);
            builder.Entity<ProgettoScadenza>().HasKey(s => s.IdScadenza);
            builder.Entity<ProgettoNota>()   .HasKey(n => n.IdNota);
            builder.Entity<ProgettoMembro>() .HasKey(m => new { m.IdProgetto, m.UserId });

            builder.Entity<ProgettoTask>()
                .HasOne(t => t.Progetto)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.IdProgetto)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskCommento>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Commenti)
                .HasForeignKey(c => c.IdTask)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProgettoScadenza>()
                .HasOne(s => s.Progetto)
                .WithMany(p => p.Scadenze)
                .HasForeignKey(s => s.IdProgetto)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProgettoNota>()
                .HasOne(n => n.Progetto)
                .WithMany(p => p.Note)
                .HasForeignKey(n => n.IdProgetto)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProgettoMembro>()
                .HasOne(m => m.Progetto)
                .WithMany(p => p.Membri)
                .HasForeignKey(m => m.IdProgetto)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
