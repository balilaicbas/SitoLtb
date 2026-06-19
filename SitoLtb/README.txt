================================================================================
  LIBERO TORNEIFICIO DEL BORGO — SITO WEB
  Documentazione tecnica completa
  Ultimo aggiornamento: giugno 2026
================================================================================

--------------------------------------------------------------------------------
INDICE
--------------------------------------------------------------------------------
 1. Panoramica del progetto
 2. Requisiti e stack tecnologico
 3. Struttura cartelle
 4. Configurazione e avvio
 5. Database e migrazioni
 6. Autenticazione e ruoli
 7. Controllers e routing
 8. Modelli dati
 9. Servizi
10. ViewModels
11. Frontend (CSS, JS, asset)
12. Area Admin
13. Funzionalità principali
14. Email e SMTP
15. Cookie consent (Cookiebot)
16. Note di deployment

================================================================================
1. PANORAMICA DEL PROGETTO
================================================================================

Sito web dell'associazione sportiva dilettantistica "Libero Torneificio del
Borgo" (ASD LTB), circolo di scacchi attivo da oltre 15 anni.

Il sito offre:
  - Calendario tornei interattivo con navigazione per mese
  - Preiscrizione online ai tornei (divisi per sede: Verdolina / Comala)
  - Blog/notizie per categorie (In evidenza, Tornei, Eventi, CIS)
  - Sezioni informative: Scuola di scacchi, CAT, Chi siamo, Dove siamo
  - Iscrizione newsletter
  - Form di contatto con invio email
  - Area amministrativa per gestione contenuti, utenti, tornei e newsletter
  - Modulo Coordinazione: project management interno per il direttivo
  - Dashboard dati (DataController)

URL produzione (Azure): https://liberotorneificiodelborgo.azurewebsites.net

================================================================================
2. REQUISITI E STACK TECNOLOGICO
================================================================================

Runtime:
  - .NET 8.0 (ASP.NET Core MVC)
  - SQL Server (due database distinti, vedi sez. 5)

Principali pacchetti NuGet:
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.11
  - Microsoft.EntityFrameworkCore.SqlServer 9.0.0
  - Microsoft.EntityFrameworkCore.Tools 9.0.0
  - X.PagedList.Mvc.Core 8.4.7          (paginazione)
  - AspNetCoreHero.ToastNotification 1.1.0 (notifiche toast)
  - Azure.AI.OpenAI 2.1.0               (integrazione AI, uso futuro)
  - Microsoft.Extensions.Options 9.0.2

Frontend (CDN, nessun bundler):
  - Bootstrap 5.3.3
  - Font Awesome 5.15.4
  - Leaflet.js 1.9.4     (mappa "Dove siamo")
  - Vanilla Tilt 1.8.1   (effetti 3D card)
  - Cookiebot            (cookie consent, CBID: 62a5f49a-db3a-4f29-bc85-f68fb4b165ba)
  - jQuery (locale, lib/)

================================================================================
3. STRUTTURA CARTELLE
================================================================================

SitoLtb/
├── Program.cs                        Entry point, DI container, pipeline HTTP
├── SitoLtb.csproj                    Configurazione progetto, NuGet
├── appsettings.json                  Configurazione (vedi sez. 4)
├── appsettings.Development.json      Override per sviluppo locale
├── README.txt                        Questo file
│
├── Areas/
│   └── Admin/
│       ├── Controllers/              Controller solo per utenti autenticati
│       │   ├── PostController.cs
│       │   ├── TournamentController.cs
│       │   ├── CoordinazioneController.cs
│       │   ├── UserController.cs
│       │   ├── NewsletterController.cs
│       │   ├── ArchiveController.cs
│       │   ├── DataController.cs
│       │   └── SeedController.cs
│       ├── Views/
│       │   ├── Post/
│       │   ├── Tournament/
│       │   ├── Coordinazione/
│       │   ├── User/
│       │   ├── Newsletter/
│       │   ├── Archive/
│       │   ├── Data/
│       │   └── Shared/_AdminLayout.cshtml
│       ├── _ViewImports.cshtml
│       └── _ViewStart.cshtml
│
├── Controllers/
│   ├── HomeController.cs             Tutte le pagine pubbliche
│   ├── BlogController.cs             Dettaglio singolo articolo
│   ├── ScuolaController.cs           Redirect Lichess, bundle principianti
│   └── IscrizioneController.cs       Tesseramento FSI
│
├── Data/
│   ├── ApplicationDbContext.cs       DB principale (Identity + Blog + Tornei)
│   └── LtbDbContext.cs               DB secondario (dati associativi gestionali)
│
├── Migrations/
│   ├── (ApplicationDbContext — 10+ migrazioni)
│   ├── Ltb/                          Migrazioni LtbDbContext
│   └── LtbDb/
│
├── Models/
│   ├── ApplicationUser.cs            Estende IdentityUser
│   ├── Post.cs
│   ├── Tournament.cs
│   ├── NewsletterSubscriber.cs
│   ├── ErrorViewModel.cs
│   ├── Progetto.cs                   Coordinazione
│   ├── ProgettoTask.cs
│   ├── TaskCommento.cs
│   ├── ProgettoScadenza.cs
│   ├── ProgettoNota.cs
│   ├── ProgettoMembro.cs
│   └── NotificaImpostazione.cs
│
├── Services/
│   ├── IPostService.cs / PostService.cs
│   ├── ITournamentService.cs / TournamentService.cs
│   ├── IArchiveService.cs / ArchiveService.cs
│   ├── IEmailService.cs / EmailService.cs
│   └── CoordinazioneDigestService.cs (BackgroundService)
│
├── Utilities/
│   ├── DbInizializer.cs              Seed ruoli, admin, tabelle runtime
│   ├── IDbInizializer.cs
│   ├── WebsiteRoles.cs               Costanti: "Admin", "Editor", "Data"
│   └── GlobalExceptionHandler.cs     Middleware eccezioni globali
│
├── ViewModels/                       23 classi ViewModel (vedi sez. 10)
│
├── Views/
│   ├── Home/                         23 viste pubbliche
│   ├── Blog/Post.cshtml
│   ├── Iscrizione/TesseramentoFsi.cshtml
│   └── Shared/
│       ├── _Layout.cshtml            Layout principale
│       ├── _AdminLayout.cshtml       (referenziato da Area Admin)
│       ├── _CtaStrip.cshtml          Sezione CTA (newsletter + link)
│       ├── _NotizieSection.cshtml    Widget notizie riutilizzabile
│       └── Error.cshtml
│
└── wwwroot/
    ├── css/
    │   ├── layout.css                Tutti gli stili pubblici
    │   └── adminLayout.css           Stili area admin
    ├── images/                       Logo, hero, WhatsApp icon
    ├── thumbnails/default_image.png  Placeholder thumbnail articoli
    ├── uploads/                      Immagini caricate dall'admin (~46 file)
    └── lib/                          jQuery e validation (locale)

================================================================================
4. CONFIGURAZIONE E AVVIO
================================================================================

--- appsettings.json ---

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=LtbDbBlog;...",   // DB principale
    "Default":           "Server=...;Database=LtbDb;..."        // DB gestionale
  },
  "AdminUser": {
    "UserName": "",
    "Email":    "",
    "Password": "",
    "FirstName": "",
    "LastName":  ""
  },
  "Smtp": {
    "Host":      "smtp.gmail.com",
    "Port":      587,
    "EnableSsl": true,
    "Username":  "",
    "Password":  "",
    "FromEmail": "info@liberotorneificioscacchi.it",
    "FromName":  "Libero Torneificio del Borgo",
    "ContactTo": "info@liberotorneificioscacchi.it"
  }
}

ATTENZIONE: Non committare mai credenziali reali. Usare:
  - User Secrets in sviluppo:  dotnet user-secrets set "Smtp:Password" "xxx"
  - Environment Variables / Azure App Settings in produzione.

--- Avvio locale ---

  1. dotnet restore
  2. Configurare le ConnectionStrings in appsettings.Development.json
  3. dotnet ef database update --context ApplicationDbContext
  4. dotnet ef database update --context LtbDbContext
  5. dotnet run

Al primo avvio DbInizializer crea automaticamente:
  - Ruoli: Admin, Editor, Data
  - Utente admin dai valori in AdminUser
  - Tabelle Coordinazione (DDL diretto, non EF migration)

================================================================================
5. DATABASE E MIGRAZIONI
================================================================================

Il progetto usa DUE database separati:

--- LtbDbBlog (ApplicationDbContext) ---
Contiene dati del sito web:
  - AspNetUsers / AspNetRoles / ... (Identity)
  - Posts                (articoli blog)
  - Tournaments          (tornei)
  - NewsletterSubscribers
  - Progetti             (Coordinazione)
  - ProgettoTasks
  - TaskCommenti
  - ProgettoScadenze
  - ProgettoNote
  - ProgettoMembri
  - NotificheImpostazioni

Migrazioni (cartella Migrations/):
  - 20250115010206_InitialIdentity
  - 20250125110938_addCategoriaToPost
  - 20250406170634_TournamentsModified
  - 20250406230848_AddAuthorAndDescriptionToPost
  - 20250617000000_AddNewsletterSubscriber
  - + altre per Coordinazione

--- LtbDb (LtbDbContext) ---
Contiene dati gestionali dell'associazione:

  Schema anag (Anagrafica):
    - LtbCircoli
    - LtbGiocatori
    - LtbFornitori
    - LtbLivelli
    - LtbScuole / LtbTipiScuola
    - LtbTipiEvento

  Schema hist (Storico):
    - LtbEloScores        (storico punteggi Elo per giocatore)

  Schema orga (Organizzativo):
    - LtbCorsi
    - LtbEventi
    - LtbPartecipantiCorso
    - LtbPartecipantiTorneo
    - LtbSoci

Comandi migrazioni:
  dotnet ef migrations add NomeMigrazione --context ApplicationDbContext
  dotnet ef migrations add NomeMigrazione --context LtbDbContext --output-dir Migrations/LtbDb
  dotnet ef database update --context ApplicationDbContext
  dotnet ef database update --context LtbDbContext

================================================================================
6. AUTENTICAZIONE E RUOLI
================================================================================

Sistema basato su ASP.NET Core Identity.

Ruoli disponibili (definiti in Utilities/WebsiteRoles.cs):
  - Admin   → accesso completo all'area admin
  - Editor  → può creare/modificare i propri post e accedere alla Coordinazione
  - Data    → accesso alla dashboard dati (DataController)

Configurazione cookie:
  - LoginPath:        /login
  - AccessDeniedPath: /AccessDenied
  - Lockout: 5 tentativi falliti → blocco 15 minuti

Seed admin: al primo avvio viene creato l'utente Admin con le credenziali
definite nella sezione "AdminUser" di appsettings.json.

Reset password: disponibile da area admin (UserController) per gli admin.

================================================================================
7. CONTROLLERS E ROUTING
================================================================================

--- Route principali ---

Route default:   {controller=Home}/{action=Index}/{id?}
Route area:      Admin/{controller=Home}/{action=Index}/{id?}
Route area root: Admin → Admin/Post/Index

--- HomeController (pubblico) ---

GET  /                          → Index (homepage)
GET  /Home/Calendario           → Calendario tornei interattivo
GET  /Home/Preiscrizione        → Preiscrizione tornei (paginata x sede)
GET  /Home/Scuola3Livello       → Scuola di scacchi 3° livello
GET  /Home/IstruttoriLtb        → Gli istruttori
GET  /Home/CAT                  → Chess as Therapy
GET  /Home/Notizie              → Blog (tutte le categorie, paginato)
GET  /Home/NotizieInEvidenza    → Categoria "In evidenza"
GET  /Home/NotizieTornei        → Categoria "Tornei"
GET  /Home/NotizieEventi        → Categoria "Eventi"
GET  /Home/NotizieCis           → Categoria "CIS"
GET  /Home/ChiSiamo             → Il direttivo
GET  /Home/Iscrizione           → Iscrizione e vantaggi 2026
GET  /Home/Statuto              → Lo statuto
GET  /Home/DoveSiamo            → Mappa sede (Leaflet)
GET  /Home/ParlanoDiNoi         → Rassegna stampa
GET  /Home/CinquePerMille       → Dona il 5 per mille
GET  /Home/Contatti             → Form contatti
POST /Home/Contatti             → Invia email contatto
POST /Home/NewsletterIscriviti  → Iscrizione newsletter
GET  /Home/PrivacyPolicy        → Privacy Policy e Cookies
GET  /Home/Accessibilita        → Dichiarazione accessibilità

--- BlogController ---

GET  /{slug}                    → Dettaglio articolo (es. /torneo-blitz-maggio)

--- Admin/PostController [Authorize: Admin, Editor] ---

GET  /Admin/Post                → Lista post (paginata, 5 per pagina)
GET  /Admin/Post/Create         → Form creazione
POST /Admin/Post/Create         → Salva nuovo post
GET  /Admin/Post/Edit/{id}      → Form modifica
POST /Admin/Post/Edit           → Salva modifiche
POST /Admin/Post/Delete/{id}    → Elimina post
POST /Admin/Post/UploadImage    → Upload immagine → restituisce path relativo

Nota: Admin vede tutti i post; Editor vede solo i propri.

--- Admin/TournamentController [Authorize: Admin] ---

GET  /Admin/Tournament          → Lista tornei (paginata)
GET  /Admin/Tournament/Create   → Form creazione
POST /Admin/Tournament/Create   → Salva nuovo torneo
GET  /Admin/Tournament/Edit/{id}→ Form modifica
POST /Admin/Tournament/Edit     → Salva modifiche
POST /Admin/Tournament/Delete/{id} → Elimina torneo

--- Admin/CoordinazioneController [Authorize: Admin, Editor] ---

GET  /Admin/Coordinazione               → Dashboard progetti
GET  /Admin/Coordinazione/Crea          → Nuovo progetto
POST /Admin/Coordinazione/Crea
GET  /Admin/Coordinazione/Modifica/{id}
POST /Admin/Coordinazione/Modifica/{id}
GET  /Admin/Coordinazione/Dettaglio/{id}→ Dettaglio con task, note, scadenze
POST /Admin/Coordinazione/EliminaProgetto/{id}
POST /Admin/Coordinazione/NuovaTask
POST /Admin/Coordinazione/ToggleTask
POST /Admin/Coordinazione/EliminaTask
POST /Admin/Coordinazione/NuovoCommento
POST /Admin/Coordinazione/NuovaNota
POST /Admin/Coordinazione/EliminaNota
POST /Admin/Coordinazione/NuovaScadenza   → Invia .ics via email ai membri
POST /Admin/Coordinazione/EliminaScadenza
POST /Admin/Coordinazione/AggiungiMembro  → Invia .ics al nuovo membro
POST /Admin/Coordinazione/RimuoviMembro
GET  /Admin/Coordinazione/ExportIcs/{id} → Scarica calendario .ics del progetto
GET  /Admin/Coordinazione/Impostazioni   → Notifiche digest
POST /Admin/Coordinazione/AggiungiNotifica
POST /Admin/Coordinazione/ToggleNotifica
POST /Admin/Coordinazione/EliminaNotifica
POST /Admin/Coordinazione/InviaOra       → Trigger manuale digest email

--- Admin/UserController [Authorize: Admin] ---

GET  /Admin/User                → Lista utenti con ruoli
GET  /Admin/User/EditRoles/{id} → Modifica ruoli utente
POST /Admin/User/EditRoles
GET  /login                     → Form login
POST /login
GET  /logout                    → Logout
GET  /register                  → Registrazione nuovo utente

--- Admin/NewsletterController [Authorize: Admin] ---

GET  /Admin/Newsletter          → Lista iscritti
POST /Admin/Newsletter/Delete/{id}
GET  /Admin/Newsletter/Invia    → Componi newsletter
POST /Admin/Newsletter/Invia    → Invia a tutti gli iscritti attivi

--- Admin/ArchiveController [Authorize: Admin] ---

GET  /Admin/Archive             → Archivio eventi e articoli (paginato)

================================================================================
8. MODELLI DATI
================================================================================

--- Post.cs ---
  Id:                int         [PK, auto-increment]
  Title:             string      [Required, MaxLength 200]
  Description:       string      [Required, MaxLength 4000 — HTML body]
  ApplicationUserId: string?     [FK → AspNetUsers]
  Image:             string?     [Path relativo es. /uploads/uuid.jpg]
  Url:               string      [Required, MaxLength 300 — slug univoco]
  Categoria:         string      [Required, MaxLength 100]
                                 Valori: "In evidenza","Tornei","Eventi","Cis"
  DateTimeCreated:   DateTime    [Default: DateTime.Now]

--- Tournament.cs ---
  Id:                int         [PK]
  Nome:              string      [Required, MaxLength 200]
  Data:              DateTime
  Tipologia:         string      [Required] es. "Standard","Rapid","Blitz","Eterodosso"
  Sede:              string      [Required] "Verdolina" | "Comala"
  LinkBando:         string?     [MaxLength 500]
  LinkPreiscrizione: string?     [MaxLength 500]
  Url:               string?     [MaxLength 300 — slug]
  Elo:               bool        [true = valido per classifica Elo]

--- NewsletterSubscriber.cs ---
  Id:                int         [PK]
  Email:             string      [Required, MaxLength 200]
  Nome:              string?     [MaxLength 100]
  DataIscrizione:    DateTime    [Default: UTC now]
  Attivo:            bool        [Default: true]

--- ApplicationUser.cs (estende IdentityUser) ---
  FirstName:         string?
  LastName:          string?
  Posts:             List<Post>?

--- Progetto.cs (Coordinazione) ---
  IdProgetto:        int         [PK]
  Titolo:            string
  Descrizione:       string?
  DataInizio:        DateOnly
  DataScadenza:      DateOnly?
  ReferenteId:       string?     [FK → AspNetUsers]
  Colore:            string      [Default: "#4e73df" — colore UI card]
  Stato:             StatoProgetto (Attivo=0, Completato=1, Sospeso=2)
  DataCreazione:     DateTime

  Navigation: Tasks, Note, Scadenze, Membri

--- ProgettoTask.cs ---
  IdTask:            int         [PK]
  IdProgetto:        int         [FK]
  Titolo:            string
  Descrizione:       string?
  Stato:             StatoTask   (InCorso=0, Risolto=1)
  Priorita:          PrioritaTask (Bassa=0, Media=1, Alta=2)
  DataScadenza:      DateTime?
  AssegnatoAId:      string?     [FK → AspNetUsers]
  DataCreazione:     DateTime
  DataCompletamento: DateTime?

  Navigation: Commenti

--- NotificaImpostazione.cs ---
  IdNotifica:        int         [PK]
  UserId:            string      [FK → AspNetUsers]
  IntervalloOre:     int         [Default: 168 = settimanale]
                                 Opzioni: 24, 48, 72, 168, 336, 720
  UltimoInvio:       DateTime?
  Attiva:            bool        [Default: true]
  DataCreazione:     DateTime

================================================================================
9. SERVIZI
================================================================================

--- IPostService / PostService ---
  GetAll()
    → tutti i Post, ordinati per data DESC, mappati a PostVM

  GetBySlugAsync(slug)
    → Post per slug + dati autore → BlogPostVM

  GetByCategoriaPagedAsync(categoria, pageNumber, pageSize)
    → Post filtrati per Categoria, paginati con X.PagedList

  GetRelatedAsync(postId, categoria, title, count=5)
    → Post correlati per stessa categoria, escluso il corrente

--- ITournamentService / TournamentService ---
  GetAll()
    → Tornei con Data >= oggi, mappati a TournamentVM

  GetAllForCalendar()
    → Tutti i tornei (anche passati) per il calendario interattivo

  GetUpcomingBySedePagedAsync(sede, pageNumber, pageSize)
    → Tornei futuri filtrati per sede, paginati

  GetNextMonth()
    → Tornei nei prossimi 30 giorni (widget homepage)

--- IEmailService / EmailService ---
  SendContactAsync(fromName, fromEmail, message)
    → Invia email al ContactTo configurato in Smtp

  SendAsync(toEmail, subject, htmlBody)
    → Invio email generico HTML

  SendNewsletterAsync(recipients, subject, htmlBody)
    → Invio multiplo (un'email per destinatario)

  SendWithIcsAsync(toEmail, subject, htmlBody, icsContent, icsFileName)
    → Invia email con allegato .ics (calendario)

  Configurazione letta da appsettings: Smtp:Host, Port, EnableSsl,
  Username, Password, FromEmail, FromName, ContactTo

--- IArchiveService / ArchiveService ---
  GetArchiveAsync(pageEventi, pageArticoli)
    → ArchiveVM con eventi e articoli paginati

--- CoordinazioneDigestService (BackgroundService) ---
  Gira in background, si sveglia ogni ora.
  Per ogni NotificaImpostazione attiva il cui intervallo è scaduto:
    1. Carica i progetti visibili all'utente
    2. Costruisce un'email HTML di riepilogo
    3. Invia via IEmailService
    4. Aggiorna UltimoInvio

  Intervalli configurabili per utente: 24h, 48h, 72h, 168h (1 sett.),
  336h (2 sett.), 720h (1 mese).

================================================================================
10. VIEWMODELS
================================================================================

Blog / Notizie:
  BlogVM              → 4 liste paginate (InEvidenza, Tornei, Eventi, Cis)
  BlogPostVM          → dettaglio articolo + RelatedPosts
  PostVM              → card riassuntiva articolo
  CreatePostVM        → form creazione/modifica post (con IFormFile Thumbnail)
  CategoriaPostVM     → pagina categoria con lista paginata

Tornei:
  TournamentVM        → dati torneo per le viste pubbliche
  CreateTournamentVM  → form admin creazione/modifica torneo
  PreiscrizioneVM     → 2 liste paginate (Verdolina, Comala)
  CalendarioVM        → lista tornei per calendario interattivo

Homepage:
  IndexVM             → PostsFuturi + TournamentsFuturi + TorneiProssimoMese

Contatti / Newsletter:
  ContattiVM          → Nome, Email, Messaggio (form contatti)
  NewsletterSubscribeVM → Email, Nome (form iscrizione)
  SendNewsletterVM    → Oggetto, Corpo (form invio newsletter)

Autenticazione:
  LoginVM             → Username, Password
  RegisterVM          → dati registrazione + flag ruoli
  ResetPasswordVM     → Email, NewPassword
  UserVM              → dati utente + lista ruoli
  EditRolesVM         → Id, UserName, IsAdmin, IsEditor, IsData

Coordinazione:
  CoordinazioneIndexVM   → lista ProgettoCardVM + contatori (Totale/Attivi/...)
  ProgettoCardVM         → dati card progetto + avanzamento % calcolato
  ProgettoDetailVM       → dettaglio completo con tasks, note, scadenze, membri
  ProgettoTaskVM         → task con commenti annidati
  TaskCommentoVM         → singolo commento
  MembroVM               → UserId, Nome, Email
  CreaProgettoVM         → form crea/modifica progetto + lista utenti disponibili
  NotificaVM             → impostazione notifica con label intervallo
  ImpostazioniNotificheVM→ lista notifiche + utenti disponibili

================================================================================
11. FRONTEND (CSS, JS, ASSET)
================================================================================

--- wwwroot/css/layout.css ---
Contiene tutti gli stili per il sito pubblico, organizzati in sezioni:

  - Global toast (feedback newsletter)
  - CTA Strip (.cta-strip)
  - Sub-dropdown 3° livello navbar
  - Related posts slider
  - Base / reset / variabili CSS (:root --header-h, --gap-under-header)
  - Header fisso (.main-header, .logo-title, .main-navbar)
  - Section-1 (hero image con widget tornei)
  - Section-2 (slider card notizie)
  - Section-3 (riquadro info)
  - Section-4 (galleria foto)
  - Footer (.main-footer, .footer-inner, .footer-col, .footer-bottom)
  - WhatsApp fisso (.wa-fixed)
  - Image comparison slider (before/after)
  - Tabella tornei (.styled-table)
  - Pagina Scuola (.scuola-page, .scuola-two-col, ecc.)
  - Blog grid (.blog-container, .blog-card, .blog-card--featured)
  - Blog post singolo (.blog-post-title, .blog-post__content)
  - Calendario interattivo (.cal-*, .cal-card, .cal-pill, .cal-modal)
  - Notizie categoria (.notiziacat-*)

  Breakpoint principali: 560px, 640px, 700px, 768px, 900px, 960px, 992px

--- wwwroot/css/adminLayout.css ---
Stili per l'area admin (layout, sidebar, dashboard coordinazione).

--- JavaScript (inline nei layout) ---
Tutti gli script sono inline in _Layout.cshtml e nelle singole viste.
Nessun bundler/transpiler. Script principali:

  - Hamburger menu mobile + chiusura automatica dropdown
  - Sub-dropdown 3° livello (hover desktop / click mobile)
  - Auto-chiusura toast newsletter (5 secondi)
  - Sync altezza header (variabile CSS --header-h)
  - Calendario interattivo (navigazione mese, selezione giorno, modal torneo)
  - Image comparison slider (drag before/after)
  - Related posts slider (scroll snap + bottoni prev/next)

--- Immagini ---
  wwwroot/images/
    logo_LTB_Site_Icon.png    Logo circolo (favicon + header)
    HeroImage.jpg             Immagine hero homepage
    logoWhattsapp.jpg         Icona WhatsApp (bottone fisso)
    fotoLogin.jpg             Sfondo pagina login
    split-test1.jpg           Before (image comparison)
    split-test2.jpg           After (image comparison)

  wwwroot/thumbnails/
    default_image.png         Placeholder quando il post non ha immagine

  wwwroot/uploads/
    File caricati dall'admin per i post (UUID.jpg/png).
    Non versionati in git (solo la cartella è inclusa nel .csproj).

================================================================================
12. AREA ADMIN
================================================================================

Accesso: /Admin → reindirizza a /Admin/Post/Index
Layout: Areas/Admin/Views/Shared/_AdminLayout.cshtml

Sezioni disponibili:
  Post         → CRUD articoli blog con upload immagine
  Tornei       → CRUD calendario tornei
  Coordinazione→ Project management interno (vedi sotto)
  Utenti       → Gestione utenti e ruoli (solo Admin)
  Newsletter   → Lista iscritti + invio campagne email
  Archivio     → Archivio storico eventi e articoli
  Dati         → Dashboard dati e statistiche

--- Modulo Coordinazione ---
Sistema di project management per il direttivo. Non usa migrazioni EF
classiche: le tabelle vengono create con DDL diretto in Program.cs al primo
avvio (se non esistono).

Funzionalità:
  - Creazione e gestione progetti con colore, stato, referente, scadenza
  - Task con priorità (Bassa/Media/Alta), assegnazione utente, stato
  - Commenti sui task
  - Note libere di progetto
  - Scadenze con invio automatico file .ics via email ai membri
  - Gestione membri del progetto
  - Export calendario .ics dell'intero progetto
  - Digest email configurabile per utente (intervallo da 24h a 1 mese)
  - Trigger manuale digest da UI (pulsante "Invia ora")

================================================================================
13. FUNZIONALITÀ PRINCIPALI
================================================================================

--- Calendario tornei interattivo ---
Pagina: /Home/Calendario
ViewModel: CalendarioVM → List<TournamentVM>
La griglia mensile è generata in JavaScript. Ogni cella-giorno mostra
le "pillole" dei tornei colorati per tipologia:
  Standard → blu (#1a56db)
  Rapid    → verde (#057a55)
  Blitz    → arancio (#d03801)
  Eterodosso → viola (#7e3af2)
Click su una pillola apre un modal Bootstrap con i dettagli del torneo.

--- Preiscrizione tornei ---
Pagina: /Home/Preiscrizione
Due tab/sezioni paginate: Verdolina e Comala.
Mostra solo tornei futuri (Data >= oggi).
5 tornei per pagina per sede.

--- Blog / Notizie ---
Categorie: "In evidenza", "Tornei", "Eventi", "Cis"
Homepage mostra le ultime 5 per categoria.
Pagine dedicate per categoria con paginazione.
Slug URL per ogni articolo (es. /torneo-blitz-maggio).
Articoli correlati in fondo al post (stessa categoria o keyword).

--- Newsletter ---
Iscrizione via form (CTA strip in fondo ad ogni pagina).
Disiscrizione non implementata lato utente (gestita da admin).
Invio campagne da Admin/Newsletter/Invia.
Un'email separata per ogni iscritto attivo.

--- Mappa "Dove siamo" ---
Usa Leaflet.js con tile OpenStreetMap. Nessuna API key richiesta.

--- Cookie Consent (Cookiebot) ---
CBID: 62a5f49a-db3a-4f29-bc85-f68fb4b165ba
Script caricato come primo elemento in <head> con data-blockingmode="auto".
Blocca automaticamente script di terze parti prima del consenso.
Link "Impostazioni sui cookie" nel footer chiama Cookiebot.renew().

================================================================================
14. EMAIL E SMTP
================================================================================

Provider: Gmail SMTP (smtp.gmail.com:587, STARTTLS)
Mittente: info@liberotorneificioscacchi.it

Tipi di email inviate:
  1. Contatto dal form pubblico → all'indirizzo ContactTo
  2. Newsletter campagne → a tutti gli iscritti attivi
  3. Scadenza aggiunta a progetto Coordinazione → ai membri del progetto
     (con allegato .ics)
  4. Nuovo membro aggiunto a progetto → al membro aggiunto
     (con allegato .ics di tutte le scadenze)
  5. Digest coordinazione → agli utenti con notifica attiva

ATTENZIONE: Gmail richiede una "App Password" se l'account ha 2FA attivo.
La password normale non funziona. Generare App Password da:
  Account Google → Sicurezza → Verifica in 2 passaggi → Password per le app

================================================================================
15. COOKIE CONSENT (COOKIEBOT)
================================================================================

Implementazione:
  - Script in _Layout.cshtml, prima di qualsiasi altro script/CSS
  - Modalità: Automatic Blocking (data-blockingmode="auto")
  - Cookiebot intercetta e blocca script di terze parti (Bootstrap, Leaflet,
    Font Awesome, ecc.) finché l'utente non acconsente

Banner configurato dal pannello Cookiebot (cookiebot.com):
  - Testo, colori e layout gestiti online, non nel codice
  - Assicurarsi che il dominio di produzione sia registrato nell'account

Collegamento footer:
  <a href="javascript:void(0);" onclick="Cookiebot.renew()">
    Impostazioni sui cookie
  </a>

================================================================================
16. NOTE DI DEPLOYMENT (AZURE)
================================================================================

Piattaforma: Azure App Service

Variabili d'ambiente da configurare in Azure App Settings
(sovrascrivono appsettings.json):

  ConnectionStrings__DefaultConnection   → stringa connessione DB principale
  ConnectionStrings__Default             → stringa connessione DB gestionale
  AdminUser__UserName
  AdminUser__Email
  AdminUser__Password
  AdminUser__FirstName
  AdminUser__LastName
  Smtp__Username
  Smtp__Password

Database: Azure SQL Database (o SQL Server su VM)

Upload immagini: wwwroot/uploads/ è locale al container/istanza.
Per ambienti multi-istanza o deploy con sovrascrittura, valutare
Azure Blob Storage per i file caricati.

Logging: configurato su "Information" per default, "Warning" per ASP.NET Core.
In produzione i log sono visibili da Azure → App Service → Log Stream.

Limiti upload configurati:
  - Kestrel MaxRequestBodySize: 10 MB
  - Form MultipartBodyLengthLimit: 100 MB

================================================================================
  Fine documentazione — ASD Libero Torneificio del Borgo
================================================================================
