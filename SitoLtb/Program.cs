using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitoLtb.Data;
using SitoLtb.Models;
using SitoLtb.Services;
using SitoLtb.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
var connectionString2 = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<LtbDbContext>(options => options.UseSqlServer(connectionString2));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
});


builder.Services.AddScoped<IDbInizializer, DbInizializer>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IArchiveService, ArchiveService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<SitoLtb.Services.CoordinazioneDigestService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<SitoLtb.Services.CoordinazioneDigestService>());
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/AccessDenied";
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10485760; // 10 MB
});
builder.Services.Configure<FormOptions>(options =>
{
    // ad esempio 100 MB
    options.MultipartBodyLengthLimit = 1024 * 1024 * 100;
});


// Aggiungi il servizio di configurazione per ConnectionStrings

var app = builder.Build();

DataSeeding();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
else
{
    app.UseDeveloperExceptionPage(); // ✅ solo in dev
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "admin_root",
    pattern: "Admin",
    defaults: new { area = "Admin", controller = "Post", action = "Index" });

app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();


void DataSeeding()
{
    using var scope = app.Services.CreateScope();
    var DbInitialize = scope.ServiceProvider.GetRequiredService<IDbInizializer>();
    DbInitialize.Initialize();

    // Crea tabelle coordinazione se non esistono (uno statement per volta)
    var db = scope.ServiceProvider.GetRequiredService<SitoLtb.Data.ApplicationDbContext>();
    EnsureTable(db);
}

void EnsureTable(SitoLtb.Data.ApplicationDbContext db)
{
    var ddl = new[]
    {
        @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='Progetti')
          CREATE TABLE Progetti (
              IdProgetto    INT IDENTITY(1,1) PRIMARY KEY,
              Titolo        NVARCHAR(200) NOT NULL,
              Descrizione   NVARCHAR(MAX) NULL,
              DataInizio    DATE NOT NULL,
              DataScadenza  DATE NULL,
              ReferenteId   NVARCHAR(450) NULL,
              Colore        NVARCHAR(20) NOT NULL DEFAULT '#4e73df',
              Stato         INT NOT NULL DEFAULT 0,
              DataCreazione DATETIME2 NOT NULL DEFAULT GETUTCDATE()
          )",
        @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='ProgettoTasks')
          CREATE TABLE ProgettoTasks (
              IdTask             INT IDENTITY(1,1) PRIMARY KEY,
              IdProgetto         INT NOT NULL REFERENCES Progetti(IdProgetto) ON DELETE CASCADE,
              Titolo             NVARCHAR(300) NOT NULL,
              Descrizione        NVARCHAR(MAX) NULL,
              Stato              INT NOT NULL DEFAULT 0,
              Priorita           INT NOT NULL DEFAULT 1,
              DataScadenza       DATETIME2 NULL,
              AssegnatoAId       NVARCHAR(450) NULL,
              DataCreazione      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
              DataCompletamento  DATETIME2 NULL
          )",
        @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='TaskCommenti')
          CREATE TABLE TaskCommenti (
              IdCommento    INT IDENTITY(1,1) PRIMARY KEY,
              IdTask        INT NOT NULL REFERENCES ProgettoTasks(IdTask) ON DELETE CASCADE,
              AutoreId      NVARCHAR(450) NULL,
              Testo         NVARCHAR(MAX) NOT NULL,
              DataCreazione DATETIME2 NOT NULL DEFAULT GETUTCDATE()
          )",
        @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='ProgettoScadenze')
          CREATE TABLE ProgettoScadenze (
              IdScadenza INT IDENTITY(1,1) PRIMARY KEY,
              IdProgetto INT NOT NULL REFERENCES Progetti(IdProgetto) ON DELETE CASCADE,
              Titolo     NVARCHAR(200) NOT NULL,
              Data       DATE NOT NULL,
              Nota       NVARCHAR(500) NULL
          )",
        @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='ProgettoNote')
          CREATE TABLE ProgettoNote (
              IdNota        INT IDENTITY(1,1) PRIMARY KEY,
              IdProgetto    INT NOT NULL REFERENCES Progetti(IdProgetto) ON DELETE CASCADE,
              AutoreId      NVARCHAR(450) NULL,
              Testo         NVARCHAR(MAX) NOT NULL,
              DataCreazione DATETIME2 NOT NULL DEFAULT GETUTCDATE()
          )",
        @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='ProgettoMembri')
          CREATE TABLE ProgettoMembri (
              IdProgetto INT NOT NULL REFERENCES Progetti(IdProgetto) ON DELETE CASCADE,
              UserId     NVARCHAR(450) NOT NULL,
              CONSTRAINT PK_ProgettoMembri PRIMARY KEY (IdProgetto, UserId)
          )",
        @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='NotificheImpostazioni')
          CREATE TABLE NotificheImpostazioni (
              IdNotifica     INT IDENTITY(1,1) PRIMARY KEY,
              UserId         NVARCHAR(450) NOT NULL,
              IntervalloOre  INT NOT NULL DEFAULT 168,
              UltimoInvio    DATETIME2 NULL,
              Attiva         BIT NOT NULL DEFAULT 1,
              DataCreazione  DATETIME2 NOT NULL DEFAULT GETUTCDATE()
          )",
    };

    foreach (var sql in ddl)
    {
        try { db.Database.ExecuteSqlRaw(sql); }
        catch (Exception ex) { Console.WriteLine($"[DDL skip] {ex.Message}"); }
    }
}
