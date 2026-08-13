using GestionParcMateriel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionParcMateriel.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── DbSets ────────────────────────────────────────────────────────────────
    public DbSet<Materiel> Materiels { get; set; }
    public DbSet<TypeMateriel> TypesMateriels { get; set; }
    public DbSet<EtatMateriel> EtatsMateriels { get; set; }
    public DbSet<Localisation> Localisations { get; set; }
    public DbSet<Employe> Employes { get; set; }
    public DbSet<MouvementMateriel> MouvementsMateriels { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── MATERIEL ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Materiel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CodeBarre)
                  .IsRequired()
                  .HasMaxLength(50);

            // Unicité du code-barres
            entity.HasIndex(e => e.CodeBarre)
                  .IsUnique()
                  .HasDatabaseName("IX_Materiel_CodeBarre");

            // Unicité du numéro de série si renseigné (index filtré)
            entity.HasIndex(e => e.NumeroSerie)
                  .IsUnique()
                  .HasFilter("[NumeroSerie] IS NOT NULL")
                  .HasDatabaseName("IX_Materiel_NumeroSerie");

            entity.Property(e => e.Libelle)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Marque).HasMaxLength(50);
            entity.Property(e => e.Modele).HasMaxLength(50);
            entity.Property(e => e.NumeroSerie).HasMaxLength(100);
            entity.Property(e => e.Actif).HasDefaultValue(true);

            entity.Property(e => e.DateCreation)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.DateModification)
                  .HasDefaultValueSql("GETUTCDATE()");

            // FK → TypeMateriel
            entity.HasOne(e => e.TypeMateriel)
                  .WithMany(t => t.Materiels)
                  .HasForeignKey(e => e.TypeMaterielId)
                  .OnDelete(DeleteBehavior.Restrict);

            // FK → EtatMateriel
            entity.HasOne(e => e.Etat)
                  .WithMany(et => et.Materiels)
                  .HasForeignKey(e => e.EtatId)
                  .OnDelete(DeleteBehavior.Restrict);

            // FK → Localisation
            entity.HasOne(e => e.Localisation)
                  .WithMany(l => l.Materiels)
                  .HasForeignKey(e => e.LocalisationId)
                  .OnDelete(DeleteBehavior.Restrict);

            // FK → Employe (nullable)
            entity.HasOne(e => e.Employe)
                  .WithMany(emp => emp.Materiels)
                  .HasForeignKey(e => e.EmployeId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ── TYPE_MATERIEL ─────────────────────────────────────────────────────
        modelBuilder.Entity<TypeMateriel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Libelle).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Libelle)
                  .IsUnique()
                  .HasDatabaseName("IX_TypeMateriel_Libelle");
        });

        // ── ETAT_MATERIEL ─────────────────────────────────────────────────────
        modelBuilder.Entity<EtatMateriel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Libelle).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Libelle)
                  .IsUnique()
                  .HasDatabaseName("IX_EtatMateriel_Libelle");
        });

        // ── LOCALISATION ──────────────────────────────────────────────────────
        modelBuilder.Entity<Localisation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Nom)
                  .IsUnique()
                  .HasDatabaseName("IX_Localisation_Nom");
        });

        // ── EMPLOYE ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Employe>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Matricule).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Prenom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Service).HasMaxLength(100);
            entity.Property(e => e.Poste).HasMaxLength(100);

            entity.HasIndex(e => e.Matricule)
                  .IsUnique()
                  .HasDatabaseName("IX_Employe_Matricule");
        });

        // ── MOUVEMENT_MATERIEL ────────────────────────────────────────────────
        modelBuilder.Entity<MouvementMateriel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DateMouvement)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.TypeMouvement)
                  .IsRequired()
                  .HasConversion<string>()  // stocker l'enum comme string lisible
                  .HasMaxLength(30);

            // FK → Materiel
            entity.HasOne(e => e.Materiel)
                  .WithMany(m => m.Mouvements)
                  .HasForeignKey(e => e.MaterielId)
                  .OnDelete(DeleteBehavior.Cascade);

            // FK → Employe (nullable)
            entity.HasOne(e => e.Employe)
                  .WithMany(emp => emp.Mouvements)
                  .HasForeignKey(e => e.EmployeId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);

            // FK → Localisation
            entity.HasOne(e => e.Localisation)
                  .WithMany(l => l.Mouvements)
                  .HasForeignKey(e => e.LocalisationId)
                  .OnDelete(DeleteBehavior.Restrict);

            // FK → EtatMateriel (AncienEtat) — pas de cascade pour éviter conflits
            entity.HasOne(e => e.AncienEtat)
                  .WithMany(et => et.MouvementsAncienEtat)
                  .HasForeignKey(e => e.AncienEtatId)
                  .OnDelete(DeleteBehavior.Restrict);

            // FK → EtatMateriel (NouvelEtat)
            entity.HasOne(e => e.NouvelEtat)
                  .WithMany(et => et.MouvementsNouvelEtat)
                  .HasForeignKey(e => e.NouvelEtatId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
// ── UTILISATEUR ───────────────────────────────────────────────────────
        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Utilisateur_Email");
            entity.Property(e => e.MotDePasseHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
        });
        // ── SEED DATA ─────────────────────────────────────────────────────────
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Types de matériel
        modelBuilder.Entity<TypeMateriel>().HasData(
            new TypeMateriel { Id = 1, Libelle = "PC Portable" },
            new TypeMateriel { Id = 2, Libelle = "PC Fixe" },
            new TypeMateriel { Id = 3, Libelle = "Écran" },
            new TypeMateriel { Id = 4, Libelle = "Imprimante" },
            new TypeMateriel { Id = 5, Libelle = "Scanner" },
            new TypeMateriel { Id = 6, Libelle = "Switch" },
            new TypeMateriel { Id = 7, Libelle = "Routeur" },
            new TypeMateriel { Id = 8, Libelle = "Téléphone IP" }
        );

        // États du matériel
        modelBuilder.Entity<EtatMateriel>().HasData(
            new EtatMateriel { Id = 1, Libelle = "En stock" },
            new EtatMateriel { Id = 2, Libelle = "Affecté" },
            new EtatMateriel { Id = 3, Libelle = "En maintenance" },
            new EtatMateriel { Id = 4, Libelle = "Panne" },
            new EtatMateriel { Id = 5, Libelle = "HS" },
            new EtatMateriel { Id = 6, Libelle = "Rebut" }
        );

        // Localisations
        modelBuilder.Entity<Localisation>().HasData(
            new Localisation { Id = 1, Nom = "Salle Serveur" },
            new Localisation { Id = 2, Nom = "Administration" },
            new Localisation { Id = 3, Nom = "Atelier" },
            new Localisation { Id = 4, Nom = "Magasin" },
            new Localisation { Id = 5, Nom = "Direction" }
        );

        // Utilisateurs par défaut
        modelBuilder.Entity<Utilisateur>().HasData(
            new Utilisateur
            {
                Id = 1,
                Email = "admin@sopal.tn",
                MotDePasseHash = HashPassword("Admin123!"),
                Role = "Admin",
                NomComplet = "Administrateur SOPAL"
            },
            new Utilisateur
            {
                Id = 2,
                Email = "user@sopal.tn",
                MotDePasseHash = HashPassword("User123!"),
                Role = "Utilisateur",
                NomComplet = "Technicien IT"
            }
        );
    }

    public static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
