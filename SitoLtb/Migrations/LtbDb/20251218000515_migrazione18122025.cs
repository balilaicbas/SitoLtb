using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SitoLtb.Migrations.LtbDb
{
    /// <inheritdoc />
    public partial class migrazione18122025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allievi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cognome = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    eta = table.Column<int>(type: "int", nullable: true),
                    elo = table.Column<int>(type: "int", nullable: true),
                    socio = table.Column<bool>(type: "bit", nullable: true),
                    partecipante = table.Column<bool>(type: "bit", nullable: true),
                    agonista = table.Column<bool>(type: "bit", nullable: true),
                    lavoratore = table.Column<bool>(type: "bit", nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Allievi__3213E83F25A9DF33", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ControlliRicorrenti",
                columns: table => new
                {
                    data = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    nomeTabella = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    noccorrenze = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Eventi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    data = table.Column<DateOnly>(type: "date", nullable: true),
                    luogo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    tipologia = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    omologato = table.Column<bool>(type: "bit", nullable: true),
                    costo = table.Column<int>(type: "int", nullable: true),
                    linkBando = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    classifica = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Eventi__3213E83FB73FAEA3", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Lavoratori",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    cognome = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    eta = table.Column<int>(type: "int", nullable: true),
                    tipologiaLavoratore = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    provenienza = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    socio = table.Column<bool>(type: "bit", nullable: true),
                    partecipante = table.Column<bool>(type: "bit", nullable: true),
                    agonista = table.Column<bool>(type: "bit", nullable: true),
                    allievo = table.Column<bool>(type: "bit", nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Lavorato__3213E83F53686E39", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Partecipanti",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    cognome = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    eta = table.Column<int>(type: "int", nullable: true),
                    primoEvento = table.Column<bool>(type: "bit", nullable: true),
                    circolo = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    elo = table.Column<int>(type: "int", nullable: true),
                    socio = table.Column<bool>(type: "bit", nullable: true),
                    agonista = table.Column<bool>(type: "bit", nullable: true),
                    lavoratore = table.Column<bool>(type: "bit", nullable: true),
                    allievo = table.Column<bool>(type: "bit", nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Partecip__3213E83F855F149E", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Persone",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    cognome = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    eta = table.Column<int>(type: "int", nullable: false),
                    socio = table.Column<bool>(type: "bit", nullable: true),
                    partecipante = table.Column<bool>(type: "bit", nullable: true),
                    agonista = table.Column<bool>(type: "bit", nullable: true),
                    lavoratore = table.Column<bool>(type: "bit", nullable: true),
                    allievo = table.Column<bool>(type: "bit", nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Persone__3213E83F35604BA9", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Poli",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    piva = table.Column<string>(type: "char(11)", unicode: false, fixedLength: true, maxLength: 11, nullable: false),
                    nome = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    indirizzo = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Poli__3213E83FF4797DCC", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RealtaEsterne",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    tipologia = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RealtaEs__3213E83FAE65C21B", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Scuole",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    indirizzo = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    tipologiaScuola = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    telefono = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    mail = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Scuole__3213E83F4468ADC2", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Soci",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    cognome = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    eta = table.Column<int>(type: "int", nullable: true),
                    tipologiaIscrizione = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    elo = table.Column<int>(type: "int", nullable: true),
                    annoPrimaIscrizione = table.Column<DateOnly>(type: "date", nullable: true),
                    partecipante = table.Column<bool>(type: "bit", nullable: true),
                    agonista = table.Column<bool>(type: "bit", nullable: true),
                    lavoratore = table.Column<bool>(type: "bit", nullable: true),
                    allievo = table.Column<bool>(type: "bit", nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Soci__3213E83F6472C5CB", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "EventoLavoratore",
                columns: table => new
                {
                    FKidEvento = table.Column<int>(type: "int", nullable: false),
                    FKidLavoratore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoLavoratore", x => new { x.FKidEvento, x.FKidLavoratore });
                    table.ForeignKey(
                        name: "FK__EventoLav__FKidE__74AE54BC",
                        column: x => x.FKidEvento,
                        principalTable: "Eventi",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__EventoLav__FKidL__75A278F5",
                        column: x => x.FKidLavoratore,
                        principalTable: "Lavoratori",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "EventoPartecipante",
                columns: table => new
                {
                    FKidEvento = table.Column<int>(type: "int", nullable: false),
                    FKidPartecipante = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPartecipante", x => new { x.FKidEvento, x.FKidPartecipante });
                    table.ForeignKey(
                        name: "FK__EventoPar__FKidE__6EF57B66",
                        column: x => x.FKidEvento,
                        principalTable: "Eventi",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__EventoPar__FKidP__6FE99F9F",
                        column: x => x.FKidPartecipante,
                        principalTable: "Partecipanti",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Agonisti",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cognome = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    idFsi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    idFide = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    annoPrimaIscrizione = table.Column<DateOnly>(type: "date", nullable: true),
                    tipoTessera = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    socio = table.Column<bool>(type: "bit", nullable: true),
                    partecipante = table.Column<bool>(type: "bit", nullable: true),
                    lavoratore = table.Column<bool>(type: "bit", nullable: true),
                    allievo = table.Column<bool>(type: "bit", nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    FKidPolo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Agonisti__3213E83F2CC2EB13", x => x.id);
                    table.ForeignKey(
                        name: "Fk_AgonistaPolo",
                        column: x => x.FKidPolo,
                        principalTable: "Poli",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PoloEvento",
                columns: table => new
                {
                    FKidPolo = table.Column<int>(type: "int", nullable: false),
                    FKidEvento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoloEvento", x => new { x.FKidPolo, x.FKidEvento });
                    table.ForeignKey(
                        name: "FK__PoloEvent__FKidE__6A30C649",
                        column: x => x.FKidEvento,
                        principalTable: "Eventi",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__PoloEvent__FKidP__693CA210",
                        column: x => x.FKidPolo,
                        principalTable: "Poli",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Cooperazioni",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dataInizioCooperazione = table.Column<DateOnly>(type: "date", nullable: false),
                    dataFineCooperazione = table.Column<DateOnly>(type: "date", nullable: true),
                    tipologia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    FKPoloId = table.Column<int>(type: "int", nullable: false),
                    FKRealtaEsternaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cooperaz__3213E83F76031D81", x => x.id);
                    table.ForeignKey(
                        name: "FK__Cooperazi__FKPol__1F98B2C1",
                        column: x => x.FKPoloId,
                        principalTable: "Poli",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Cooperazi__FKRea__208CD6FA",
                        column: x => x.FKRealtaEsternaId,
                        principalTable: "RealtaEsterne",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "RealtaEsternaPersona",
                columns: table => new
                {
                    FKidRealtaEsterna = table.Column<int>(type: "int", nullable: false),
                    FKidPersona = table.Column<int>(type: "int", nullable: false),
                    anno = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKRealtaEsternaPersona", x => new { x.FKidRealtaEsterna, x.FKidPersona, x.anno });
                    table.ForeignKey(
                        name: "FK__RealtaEst__FKidP__66603565",
                        column: x => x.FKidPersona,
                        principalTable: "Persone",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__RealtaEst__FKidR__656C112C",
                        column: x => x.FKidRealtaEsterna,
                        principalTable: "RealtaEsterne",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Classi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    argomento = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    FKidScuola = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Classi__3213E83F30F9F908", x => x.id);
                    table.ForeignKey(
                        name: "FK__Classi__FKidScuo__7E37BEF6",
                        column: x => x.FKidScuola,
                        principalTable: "Scuole",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PoloScuola",
                columns: table => new
                {
                    FKidPolo = table.Column<int>(type: "int", nullable: false),
                    FKidScuola = table.Column<int>(type: "int", nullable: false),
                    anno = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKPoloScuola", x => new { x.FKidPolo, x.FKidScuola, x.anno });
                    table.ForeignKey(
                        name: "FK__PoloScuol__FKidP__7A672E12",
                        column: x => x.FKidPolo,
                        principalTable: "Poli",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__PoloScuol__FKidS__7B5B524B",
                        column: x => x.FKidScuola,
                        principalTable: "Scuole",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SocioPolo",
                columns: table => new
                {
                    FKidPolo = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    FKidSocio = table.Column<int>(type: "int", nullable: false),
                    Anno = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Anno", x => new { x.FKidPolo, x.FKidSocio, x.Anno });
                    table.ForeignKey(
                        name: "FK__SocioPolo__FKidP__5FB337D6",
                        column: x => x.FKidPolo,
                        principalTable: "Poli",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__SocioPolo__FKidS__60A75C0F",
                        column: x => x.FKidSocio,
                        principalTable: "Soci",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "AnnoAgonistico",
                columns: table => new
                {
                    anno = table.Column<int>(type: "int", nullable: false),
                    FKidAgonista = table.Column<int>(type: "int", nullable: false),
                    nTorneiSvolti = table.Column<int>(type: "int", nullable: true),
                    variazioneElo = table.Column<int>(type: "int", nullable: true),
                    Elostandard1gen = table.Column<int>(type: "int", nullable: true),
                    EloRapid1gen = table.Column<int>(type: "int", nullable: true),
                    EloBlitz1gen = table.Column<int>(type: "int", nullable: true),
                    EloStandard1mar = table.Column<int>(type: "int", nullable: true),
                    EloRapid1mar = table.Column<int>(type: "int", nullable: true),
                    EloBlitz1mar = table.Column<int>(type: "int", nullable: true),
                    EloStandard1giu = table.Column<int>(type: "int", nullable: true),
                    EloRapid1giu = table.Column<int>(type: "int", nullable: true),
                    EloBlitz1giu = table.Column<int>(type: "int", nullable: true),
                    EloStandard1set = table.Column<int>(type: "int", nullable: true),
                    EloRapid1set = table.Column<int>(type: "int", nullable: true),
                    EloBlitz1set = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__AnnoAgoni__FKidA__3493CFA7",
                        column: x => x.FKidAgonista,
                        principalTable: "Agonisti",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ClasseAllievo",
                columns: table => new
                {
                    FKidClasse = table.Column<int>(type: "int", nullable: false),
                    FKidAllievo = table.Column<int>(type: "int", nullable: false),
                    anno = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKClasseAllievo", x => new { x.FKidClasse, x.FKidAllievo, x.anno });
                    table.ForeignKey(
                        name: "FK__ClasseAll__FKidA__07C12930",
                        column: x => x.FKidAllievo,
                        principalTable: "Allievi",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__ClasseAll__FKidC__06CD04F7",
                        column: x => x.FKidClasse,
                        principalTable: "Classi",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ClasseLavoratore",
                columns: table => new
                {
                    FKidClasse = table.Column<int>(type: "int", nullable: false),
                    FKidLavoratore = table.Column<int>(type: "int", nullable: false),
                    anno = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKClasseLavoratore", x => new { x.FKidClasse, x.FKidLavoratore, x.anno });
                    table.ForeignKey(
                        name: "FK__ClasseLav__FKidC__01142BA1",
                        column: x => x.FKidClasse,
                        principalTable: "Classi",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__ClasseLav__FKidL__02084FDA",
                        column: x => x.FKidLavoratore,
                        principalTable: "Lavoratori",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agonisti_FKidPolo",
                table: "Agonisti",
                column: "FKidPolo");

            migrationBuilder.CreateIndex(
                name: "IX_AnnoAgonistico_FKidAgonista",
                table: "AnnoAgonistico",
                column: "FKidAgonista");

            migrationBuilder.CreateIndex(
                name: "IX_ClasseAllievo_FKidAllievo",
                table: "ClasseAllievo",
                column: "FKidAllievo");

            migrationBuilder.CreateIndex(
                name: "IX_ClasseLavoratore_FKidLavoratore",
                table: "ClasseLavoratore",
                column: "FKidLavoratore");

            migrationBuilder.CreateIndex(
                name: "IX_Classi_FKidScuola",
                table: "Classi",
                column: "FKidScuola");

            migrationBuilder.CreateIndex(
                name: "IX_Cooperazioni_FKPoloId",
                table: "Cooperazioni",
                column: "FKPoloId");

            migrationBuilder.CreateIndex(
                name: "IX_Cooperazioni_FKRealtaEsternaId",
                table: "Cooperazioni",
                column: "FKRealtaEsternaId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoLavoratore_FKidLavoratore",
                table: "EventoLavoratore",
                column: "FKidLavoratore");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPartecipante_FKidPartecipante",
                table: "EventoPartecipante",
                column: "FKidPartecipante");

            migrationBuilder.CreateIndex(
                name: "IX_PoloEvento_FKidEvento",
                table: "PoloEvento",
                column: "FKidEvento");

            migrationBuilder.CreateIndex(
                name: "IX_PoloScuola_FKidScuola",
                table: "PoloScuola",
                column: "FKidScuola");

            migrationBuilder.CreateIndex(
                name: "IX_RealtaEsternaPersona_FKidPersona",
                table: "RealtaEsternaPersona",
                column: "FKidPersona");

            migrationBuilder.CreateIndex(
                name: "IX_SocioPolo_FKidSocio",
                table: "SocioPolo",
                column: "FKidSocio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnoAgonistico");

            migrationBuilder.DropTable(
                name: "ClasseAllievo");

            migrationBuilder.DropTable(
                name: "ClasseLavoratore");

            migrationBuilder.DropTable(
                name: "ControlliRicorrenti");

            migrationBuilder.DropTable(
                name: "Cooperazioni");

            migrationBuilder.DropTable(
                name: "EventoLavoratore");

            migrationBuilder.DropTable(
                name: "EventoPartecipante");

            migrationBuilder.DropTable(
                name: "PoloEvento");

            migrationBuilder.DropTable(
                name: "PoloScuola");

            migrationBuilder.DropTable(
                name: "RealtaEsternaPersona");

            migrationBuilder.DropTable(
                name: "SocioPolo");

            migrationBuilder.DropTable(
                name: "Agonisti");

            migrationBuilder.DropTable(
                name: "Allievi");

            migrationBuilder.DropTable(
                name: "Classi");

            migrationBuilder.DropTable(
                name: "Lavoratori");

            migrationBuilder.DropTable(
                name: "Partecipanti");

            migrationBuilder.DropTable(
                name: "Eventi");

            migrationBuilder.DropTable(
                name: "Persone");

            migrationBuilder.DropTable(
                name: "RealtaEsterne");

            migrationBuilder.DropTable(
                name: "Soci");

            migrationBuilder.DropTable(
                name: "Poli");

            migrationBuilder.DropTable(
                name: "Scuole");
        }
    }
}
