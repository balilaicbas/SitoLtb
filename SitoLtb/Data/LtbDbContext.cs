using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SitoLtb.Data;

public partial class LtbDbContext : DbContext
{
    public LtbDbContext()
    {
    }

    public LtbDbContext(DbContextOptions<LtbDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agonisti> Agonistis { get; set; }

    public virtual DbSet<Allievi> Allievis { get; set; }

    public virtual DbSet<AnnoAgonistico> AnnoAgonisticos { get; set; }

    public virtual DbSet<ClasseAllievo> ClasseAllievos { get; set; }

    public virtual DbSet<ClasseLavoratore> ClasseLavoratores { get; set; }

    public virtual DbSet<Classi> Classis { get; set; }

    public virtual DbSet<ControlliRicorrenti> ControlliRicorrentis { get; set; }

    public virtual DbSet<Cooperazioni> Cooperazionis { get; set; }

    public virtual DbSet<Eventi> Eventis { get; set; }

    public virtual DbSet<Lavoratori> Lavoratoris { get; set; }

    public virtual DbSet<Partecipanti> Partecipantis { get; set; }

    public virtual DbSet<Persone> Persones { get; set; }

    public virtual DbSet<Poli> Polis { get; set; }

    public virtual DbSet<PoloScuola> PoloScuolas { get; set; }

    public virtual DbSet<RealtaEsternaPersona> RealtaEsternaPersonas { get; set; }

    public virtual DbSet<RealtaEsterne> RealtaEsternes { get; set; }

    public virtual DbSet<Scuole> Scuoles { get; set; }

    public virtual DbSet<Soci> Socis { get; set; }

    public virtual DbSet<SocioPolo> SocioPolos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=aspettaespera;Database=LtbDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agonisti>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Agonisti__3213E83F2CC2EB13");

            entity.ToTable("Agonisti");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Allievo).HasColumnName("allievo");
            entity.Property(e => e.AnnoPrimaIscrizione).HasColumnName("annoPrimaIscrizione");
            entity.Property(e => e.Cognome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cognome");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FkidPolo).HasColumnName("FKidPolo");
            entity.Property(e => e.IdFide)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idFide");
            entity.Property(e => e.IdFsi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idFsi");
            entity.Property(e => e.Lavoratore).HasColumnName("lavoratore");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Partecipante).HasColumnName("partecipante");
            entity.Property(e => e.Socio).HasColumnName("socio");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.TipoTessera)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipoTessera");

            entity.HasOne(d => d.FkidPoloNavigation).WithMany(p => p.Agonistis)
                .HasForeignKey(d => d.FkidPolo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_AgonistaPolo");
        });

        modelBuilder.Entity<Allievi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Allievi__3213E83F25A9DF33");

            entity.ToTable("Allievi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agonista).HasColumnName("agonista");
            entity.Property(e => e.Cognome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cognome");
            entity.Property(e => e.Elo).HasColumnName("elo");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Eta).HasColumnName("eta");
            entity.Property(e => e.Lavoratore).HasColumnName("lavoratore");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Partecipante).HasColumnName("partecipante");
            entity.Property(e => e.Socio).HasColumnName("socio");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<AnnoAgonistico>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AnnoAgonistico");

            entity.Property(e => e.Anno).HasColumnName("anno");
            entity.Property(e => e.FkidAgonista).HasColumnName("FKidAgonista");
            entity.Property(e => e.NTorneiSvolti).HasColumnName("nTorneiSvolti");
            entity.Property(e => e.VariazioneElo).HasColumnName("variazioneElo");

            entity.HasOne(d => d.FkidAgonistaNavigation).WithMany()
                .HasForeignKey(d => d.FkidAgonista)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AnnoAgoni__FKidA__3493CFA7");
        });

        modelBuilder.Entity<ClasseAllievo>(entity =>
        {
            entity.HasKey(e => new { e.FkidClasse, e.FkidAllievo, e.Anno }).HasName("PKClasseAllievo");

            entity.ToTable("ClasseAllievo");

            entity.Property(e => e.FkidClasse).HasColumnName("FKidClasse");
            entity.Property(e => e.FkidAllievo).HasColumnName("FKidAllievo");
            entity.Property(e => e.Anno).HasColumnName("anno");

            entity.HasOne(d => d.FkidAllievoNavigation).WithMany(p => p.ClasseAllievos)
                .HasForeignKey(d => d.FkidAllievo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClasseAll__FKidA__07C12930");

            entity.HasOne(d => d.FkidClasseNavigation).WithMany(p => p.ClasseAllievos)
                .HasForeignKey(d => d.FkidClasse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClasseAll__FKidC__06CD04F7");
        });

        modelBuilder.Entity<ClasseLavoratore>(entity =>
        {
            entity.HasKey(e => new { e.FkidClasse, e.FkidLavoratore, e.Anno }).HasName("PKClasseLavoratore");

            entity.ToTable("ClasseLavoratore");

            entity.Property(e => e.FkidClasse).HasColumnName("FKidClasse");
            entity.Property(e => e.FkidLavoratore).HasColumnName("FKidLavoratore");
            entity.Property(e => e.Anno).HasColumnName("anno");

            entity.HasOne(d => d.FkidClasseNavigation).WithMany(p => p.ClasseLavoratores)
                .HasForeignKey(d => d.FkidClasse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClasseLav__FKidC__01142BA1");

            entity.HasOne(d => d.FkidLavoratoreNavigation).WithMany(p => p.ClasseLavoratores)
                .HasForeignKey(d => d.FkidLavoratore)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClasseLav__FKidL__02084FDA");
        });

        modelBuilder.Entity<Classi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classi__3213E83F30F9F908");

            entity.ToTable("Classi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Argomento)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("argomento");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.FkidScuola).HasColumnName("FKidScuola");
            entity.Property(e => e.Nome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nome");

            entity.HasOne(d => d.FkidScuolaNavigation).WithMany(p => p.Classis)
                .HasForeignKey(d => d.FkidScuola)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classi__FKidScuo__7E37BEF6");
        });

        modelBuilder.Entity<ControlliRicorrenti>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ControlliRicorrenti");

            entity.Property(e => e.Data)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("data");
            entity.Property(e => e.Noccorrenze).HasColumnName("noccorrenze");
            entity.Property(e => e.NomeTabella)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nomeTabella");
        });

        modelBuilder.Entity<Cooperazioni>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cooperaz__3213E83F76031D81");

            entity.ToTable("Cooperazioni");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataFineCooperazione).HasColumnName("dataFineCooperazione");
            entity.Property(e => e.DataInizioCooperazione).HasColumnName("dataInizioCooperazione");
            entity.Property(e => e.FkpoloId).HasColumnName("FKPoloId");
            entity.Property(e => e.FkrealtaEsternaId).HasColumnName("FKRealtaEsternaId");
            entity.Property(e => e.Tipologia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipologia");

            entity.HasOne(d => d.Fkpolo).WithMany(p => p.Cooperazionis)
                .HasForeignKey(d => d.FkpoloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cooperazi__FKPol__1F98B2C1");

            entity.HasOne(d => d.FkrealtaEsterna).WithMany(p => p.Cooperazionis)
                .HasForeignKey(d => d.FkrealtaEsternaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cooperazi__FKRea__208CD6FA");
        });

        modelBuilder.Entity<Eventi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Eventi__3213E83FB73FAEA3");

            entity.ToTable("Eventi", tb =>
                {
                    tb.HasTrigger("insert_table_eventopolo");
                    tb.HasTrigger("trg_DeletePoloEvento");
                });

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Classifica)
                .HasColumnType("xml")
                .HasColumnName("classifica");
            entity.Property(e => e.Costo).HasColumnName("costo");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.LinkBando)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("linkBando");
            entity.Property(e => e.Luogo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("luogo");
            entity.Property(e => e.Nome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Omologato).HasColumnName("omologato");
            entity.Property(e => e.Tipologia)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("tipologia");

            entity.HasMany(d => d.FkidLavoratores).WithMany(p => p.FkidEventos)
                .UsingEntity<Dictionary<string, object>>(
                    "EventoLavoratore",
                    r => r.HasOne<Lavoratori>().WithMany()
                        .HasForeignKey("FkidLavoratore")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EventoLav__FKidL__75A278F5"),
                    l => l.HasOne<Eventi>().WithMany()
                        .HasForeignKey("FkidEvento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EventoLav__FKidE__74AE54BC"),
                    j =>
                    {
                        j.HasKey("FkidEvento", "FkidLavoratore");
                        j.ToTable("EventoLavoratore");
                        j.IndexerProperty<int>("FkidEvento").HasColumnName("FKidEvento");
                        j.IndexerProperty<int>("FkidLavoratore").HasColumnName("FKidLavoratore");
                    });

            entity.HasMany(d => d.FkidPartecipantes).WithMany(p => p.FkidEventos)
                .UsingEntity<Dictionary<string, object>>(
                    "EventoPartecipante",
                    r => r.HasOne<Partecipanti>().WithMany()
                        .HasForeignKey("FkidPartecipante")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EventoPar__FKidP__6FE99F9F"),
                    l => l.HasOne<Eventi>().WithMany()
                        .HasForeignKey("FkidEvento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EventoPar__FKidE__6EF57B66"),
                    j =>
                    {
                        j.HasKey("FkidEvento", "FkidPartecipante");
                        j.ToTable("EventoPartecipante");
                        j.IndexerProperty<int>("FkidEvento").HasColumnName("FKidEvento");
                        j.IndexerProperty<int>("FkidPartecipante").HasColumnName("FKidPartecipante");
                    });
        });

        modelBuilder.Entity<Lavoratori>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lavorato__3213E83F53686E39");

            entity.ToTable("Lavoratori");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agonista).HasColumnName("agonista");
            entity.Property(e => e.Allievo).HasColumnName("allievo");
            entity.Property(e => e.Cognome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("cognome");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Eta).HasColumnName("eta");
            entity.Property(e => e.Nome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Partecipante).HasColumnName("partecipante");
            entity.Property(e => e.Provenienza)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("provenienza");
            entity.Property(e => e.Socio).HasColumnName("socio");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.TipologiaLavoratore)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipologiaLavoratore");
        });

        modelBuilder.Entity<Partecipanti>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Partecip__3213E83F855F149E");

            entity.ToTable("Partecipanti", tb => tb.HasTrigger("InsertIntoEventoPartecipante"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agonista).HasColumnName("agonista");
            entity.Property(e => e.Allievo).HasColumnName("allievo");
            entity.Property(e => e.Circolo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("circolo");
            entity.Property(e => e.Cognome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("cognome");
            entity.Property(e => e.Elo).HasColumnName("elo");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Eta).HasColumnName("eta");
            entity.Property(e => e.Lavoratore).HasColumnName("lavoratore");
            entity.Property(e => e.Nome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.PrimoEvento).HasColumnName("primoEvento");
            entity.Property(e => e.Socio).HasColumnName("socio");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Persone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Persone__3213E83F35604BA9");

            entity.ToTable("Persone");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agonista).HasColumnName("agonista");
            entity.Property(e => e.Allievo).HasColumnName("allievo");
            entity.Property(e => e.Cognome)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cognome");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Eta).HasColumnName("eta");
            entity.Property(e => e.Lavoratore).HasColumnName("lavoratore");
            entity.Property(e => e.Nome)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nome");
            entity.Property(e => e.Partecipante).HasColumnName("partecipante");
            entity.Property(e => e.Socio).HasColumnName("socio");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Poli>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Poli__3213E83FF4797DCC");

            entity.ToTable("Poli");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Indirizzo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("indirizzo");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Piva)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("piva");

            entity.HasMany(d => d.FkidEventos).WithMany(p => p.FkidPolos)
                .UsingEntity<Dictionary<string, object>>(
                    "PoloEvento",
                    r => r.HasOne<Eventi>().WithMany()
                        .HasForeignKey("FkidEvento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PoloEvent__FKidE__6A30C649"),
                    l => l.HasOne<Poli>().WithMany()
                        .HasForeignKey("FkidPolo")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PoloEvent__FKidP__693CA210"),
                    j =>
                    {
                        j.HasKey("FkidPolo", "FkidEvento");
                        j.ToTable("PoloEvento");
                        j.IndexerProperty<int>("FkidPolo").HasColumnName("FKidPolo");
                        j.IndexerProperty<int>("FkidEvento").HasColumnName("FKidEvento");
                    });
        });

        modelBuilder.Entity<PoloScuola>(entity =>
        {
            entity.HasKey(e => new { e.FkidPolo, e.FkidScuola, e.Anno }).HasName("PKPoloScuola");

            entity.ToTable("PoloScuola");

            entity.Property(e => e.FkidPolo).HasColumnName("FKidPolo");
            entity.Property(e => e.FkidScuola).HasColumnName("FKidScuola");
            entity.Property(e => e.Anno).HasColumnName("anno");

            entity.HasOne(d => d.FkidPoloNavigation).WithMany(p => p.PoloScuolas)
                .HasForeignKey(d => d.FkidPolo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PoloScuol__FKidP__7A672E12");

            entity.HasOne(d => d.FkidScuolaNavigation).WithMany(p => p.PoloScuolas)
                .HasForeignKey(d => d.FkidScuola)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PoloScuol__FKidS__7B5B524B");
        });

        modelBuilder.Entity<RealtaEsternaPersona>(entity =>
        {
            entity.HasKey(e => new { e.FkidRealtaEsterna, e.FkidPersona, e.Anno }).HasName("PKRealtaEsternaPersona");

            entity.ToTable("RealtaEsternaPersona");

            entity.Property(e => e.FkidRealtaEsterna).HasColumnName("FKidRealtaEsterna");
            entity.Property(e => e.FkidPersona).HasColumnName("FKidPersona");
            entity.Property(e => e.Anno).HasColumnName("anno");

            entity.HasOne(d => d.FkidPersonaNavigation).WithMany(p => p.RealtaEsternaPersonas)
                .HasForeignKey(d => d.FkidPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RealtaEst__FKidP__66603565");

            entity.HasOne(d => d.FkidRealtaEsternaNavigation).WithMany(p => p.RealtaEsternaPersonas)
                .HasForeignKey(d => d.FkidRealtaEsterna)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RealtaEst__FKidR__656C112C");
        });

        modelBuilder.Entity<RealtaEsterne>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RealtaEs__3213E83FAE65C21B");

            entity.ToTable("RealtaEsterne");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nome");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.Tipologia)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tipologia");
        });

        modelBuilder.Entity<Scuole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Scuole__3213E83F4468ADC2");

            entity.ToTable("Scuole", tb =>
                {
                    tb.HasTrigger("InsertIntoPoloScuola");
                    tb.HasTrigger("trg_DeletePoloScuola");
                });

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Indirizzo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("indirizzo");
            entity.Property(e => e.Mail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mail");
            entity.Property(e => e.Nome)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nome");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.TipologiaScuola)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("tipologiaScuola");
        });

        modelBuilder.Entity<Soci>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Soci__3213E83F6472C5CB");

            entity.ToTable("Soci", tb =>
                {
                    tb.HasTrigger("after_insert_on_soci");
                    tb.HasTrigger("trg_DeleteSocioPolo");
                });

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agonista).HasColumnName("agonista");
            entity.Property(e => e.Allievo).HasColumnName("allievo");
            entity.Property(e => e.AnnoPrimaIscrizione).HasColumnName("annoPrimaIscrizione");
            entity.Property(e => e.Cognome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("cognome");
            entity.Property(e => e.Elo).HasColumnName("elo");
            entity.Property(e => e.Email)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Eta).HasColumnName("eta");
            entity.Property(e => e.Lavoratore).HasColumnName("lavoratore");
            entity.Property(e => e.Nome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Partecipante).HasColumnName("partecipante");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.TipologiaIscrizione)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipologiaIscrizione");
        });

        modelBuilder.Entity<SocioPolo>(entity =>
        {
            entity.HasKey(e => new { e.FkidPolo, e.FkidSocio, e.Anno }).HasName("Anno");

            entity.ToTable("SocioPolo");

            entity.Property(e => e.FkidPolo)
                .HasDefaultValue(1)
                .HasColumnName("FKidPolo");
            entity.Property(e => e.FkidSocio).HasColumnName("FKidSocio");

            entity.HasOne(d => d.FkidPoloNavigation).WithMany(p => p.SocioPolos)
                .HasForeignKey(d => d.FkidPolo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SocioPolo__FKidP__5FB337D6");

            entity.HasOne(d => d.FkidSocioNavigation).WithMany(p => p.SocioPolos)
                .HasForeignKey(d => d.FkidSocio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SocioPolo__FKidS__60A75C0F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
