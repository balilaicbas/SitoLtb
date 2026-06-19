using Microsoft.EntityFrameworkCore;

namespace SitoLtb.Data;

public class LtbDbContext(DbContextOptions<LtbDbContext> options) : DbContext(options)
{

    // anag schema
    public DbSet<LtbCircolo> Circoli { get; set; } = null!;
    public DbSet<LtbFornitore> Fornitori { get; set; } = null!;
    public DbSet<LtbGiocatore> Giocatori { get; set; } = null!;
    public DbSet<LtbLivello> Livelli { get; set; } = null!;
    public DbSet<LtbScuola> Scuole { get; set; } = null!;
    public DbSet<LtbTipoEvento> TipiEvento { get; set; } = null!;
    public DbSet<LtbTipoScuola> TipiScuola { get; set; } = null!;

    // hist schema
    public DbSet<LtbEloScore> EloScores { get; set; } = null!;

    // orga schema
    public DbSet<LtbCorso> Corsi { get; set; } = null!;
    public DbSet<LtbEvento> Eventi { get; set; } = null!;
    public DbSet<LtbPartecipanteCorso> PartecipantiCorsi { get; set; } = null!;
    public DbSet<LtbPartecipanteTorneo> PartecipantiTornei { get; set; } = null!;
    public DbSet<LtbSocio> Soci { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── anag.Circoli ──────────────────────────────────────────────────
        modelBuilder.Entity<LtbCircolo>(e =>
        {
            e.ToTable("Circoli", schema: "anag");
            e.HasKey(x => x.IdCircolo).HasName("PK_Circoli");
            e.Property(x => x.RagioneSociale).HasMaxLength(100).IsRequired();
            e.Property(x => x.Indirizzo).HasMaxLength(100);
            e.Property(x => x.Localita).HasMaxLength(100).HasColumnName("Località");
            e.Property(x => x.Note).HasMaxLength(10).IsFixedLength();
        });

        // ── anag.Fornitori ────────────────────────────────────────────────
        modelBuilder.Entity<LtbFornitore>(e =>
        {
            e.ToTable("Fornitori", schema: "anag");
            e.HasKey(x => x.IdFornitore).HasName("PK_Fornitori");
            e.Property(x => x.RagioneSociale).HasMaxLength(100).IsRequired();
            e.Property(x => x.Indirizzo).HasMaxLength(100);
            e.Property(x => x.NumeroCivico).HasMaxLength(10).IsFixedLength();
            e.Property(x => x.Email).HasMaxLength(50);
        });

        // ── anag.Giocatori ────────────────────────────────────────────────
        modelBuilder.Entity<LtbGiocatore>(e =>
        {
            e.ToTable("Giocatori", schema: "anag");
            e.HasKey(x => x.IdGiocatore).HasName("PK_Giocatori");
            e.Property(x => x.Nome).HasMaxLength(50);
            e.Property(x => x.Cognome).HasMaxLength(50);
            e.Property(x => x.Indirizzo).HasMaxLength(100);
            e.Property(x => x.Email).HasMaxLength(50);
            e.Property(x => x.NumeroCivico).HasMaxLength(10).IsFixedLength();
            e.Property(x => x.Localita).HasMaxLength(100);
        });

        // ── anag.Livello ──────────────────────────────────────────────────
        modelBuilder.Entity<LtbLivello>(e =>
        {
            e.ToTable("Livello", schema: "anag");
            e.HasKey(x => x.IdLivello).HasName("PK_Livello");
            e.Property(x => x.IdLivello).ValueGeneratedNever(); // no IDENTITY
            e.Property(x => x.Descrizione).HasMaxLength(150).IsRequired();
        });

        // ── anag.TipoScuola ───────────────────────────────────────────────
        modelBuilder.Entity<LtbTipoScuola>(e =>
        {
            e.ToTable("TipoScuola", schema: "anag");
            e.HasKey(x => x.IdTipoScuola).HasName("PK_TipoScuola");
            e.Property(x => x.Descrizione).HasMaxLength(100);
        });

        // ── anag.Scuole ───────────────────────────────────────────────────
        modelBuilder.Entity<LtbScuola>(e =>
        {
            e.ToTable("Scuole", schema: "anag");
            e.HasKey(x => x.IdScuola).HasName("PK_Scuole");
            e.Property(x => x.Nome).HasMaxLength(50).IsRequired();
            e.Property(x => x.Descrizione).HasMaxLength(50);
            e.HasOne(x => x.TipoScuola)
             .WithMany(x => x.Scuole)
             .HasForeignKey(x => x.IdTipoScuola)
             .HasConstraintName("FK_Scuole_TipoScuola");
        });

        // ── anag.TipoEvento ───────────────────────────────────────────────
        modelBuilder.Entity<LtbTipoEvento>(e =>
        {
            e.ToTable("TipoEvento", schema: "anag");
            e.HasKey(x => x.IdTipoEvento).HasName("PK_TipoEvento");
            e.Property(x => x.Descrizione).HasMaxLength(100);
        });

        // ── hist.EloScores ────────────────────────────────────────────────
        modelBuilder.Entity<LtbEloScore>(e =>
        {
            e.ToTable("EloScores", schema: "hist");
            e.HasKey(x => x.IdScore).HasName("PK_EloScores");
            e.HasOne(x => x.Giocatore)
             .WithMany(x => x.EloScores)
             .HasForeignKey(x => x.IdGiocatore)
             .HasConstraintName("FK_EloScores_Giocatori");
        });

        // ── orga.Corsi ────────────────────────────────────────────────────
        modelBuilder.Entity<LtbCorso>(e =>
        {
            e.ToTable("Corsi", schema: "orga");
            e.HasKey(x => x.IdCorso).HasName("PK_Corsi");
            e.Property(x => x.DataFine).HasColumnName("Datafine");
            e.HasOne(x => x.Livello)
             .WithMany(x => x.Corsi)
             .HasForeignKey(x => x.IdLivello)
             .HasConstraintName("FK_Corsi_Livello");
            e.HasOne(x => x.Scuola)
             .WithMany(x => x.Corsi)
             .HasForeignKey(x => x.IdScuola)
             .HasConstraintName("FK_Corsi_Scuole");
        });

        // ── orga.Eventi ───────────────────────────────────────────────────
        modelBuilder.Entity<LtbEvento>(e =>
        {
            e.ToTable("Eventi", schema: "orga");
            e.HasKey(x => x.IdEvento).HasName("PK_Eventi");
            e.Property(x => x.IdEvento).ValueGeneratedNever(); // no IDENTITY
            e.HasOne(x => x.TipoEvento)
             .WithMany(x => x.Eventi)
             .HasForeignKey(x => x.IdTipoEvento)
             .HasConstraintName("FK_Eventi_TipoEvento");
        });

        // ── orga.PartecipantiCorsi ────────────────────────────────────────
        modelBuilder.Entity<LtbPartecipanteCorso>(e =>
        {
            e.ToTable("PartecipantiCorsi", schema: "orga");
            e.HasKey(x => new { x.IdCorso, x.IdGiocatore }).HasName("PK_PartecipantiCorsi");
            e.HasOne(x => x.Corso)
             .WithMany(x => x.Partecipanti)
             .HasForeignKey(x => x.IdCorso)
             .HasConstraintName("FK_PartecipantiCorsi_Corsi");
            e.HasOne(x => x.Giocatore)
             .WithMany(x => x.PartecipantiCorsi)
             .HasForeignKey(x => x.IdGiocatore)
             .HasConstraintName("FK_PartecipantiCorsi_Giocatori");
        });

        // ── orga.PartecipantiTornei ───────────────────────────────────────
        modelBuilder.Entity<LtbPartecipanteTorneo>(e =>
        {
            e.ToTable("PartecipantiTornei", schema: "orga");
            e.HasKey(x => x.IdPartecipanteTornei).HasName("PK_PartecipantiTornei");
            e.HasOne(x => x.Giocatore)
             .WithMany(x => x.Partecipazioni)
             .HasForeignKey(x => x.IdGiocatore)
             .HasConstraintName("FK_PartecipantiTornei_Giocatori");
            e.HasOne(x => x.Evento)
             .WithMany(x => x.Partecipanti)
             .HasForeignKey(x => x.IdEvento)
             .HasConstraintName("FK_PartecipantiTornei_Eventi");
        });

        // ── orga.Socio ────────────────────────────────────────────────────
        modelBuilder.Entity<LtbSocio>(e =>
        {
            e.ToTable("Socio", schema: "orga");
            e.HasKey(x => x.IdSocio).HasName("PK_Socio");
            e.HasOne(x => x.Giocatore)
             .WithMany(x => x.Soci)
             .HasForeignKey(x => x.IdGiocatore)
             .HasConstraintName("FK_Socio_Giocatori");
            e.HasOne(x => x.Circolo)
             .WithMany(x => x.Soci)
             .HasForeignKey(x => x.IdCircolo)
             .HasConstraintName("FK_Socio_Circoli");
        });
    }
}
