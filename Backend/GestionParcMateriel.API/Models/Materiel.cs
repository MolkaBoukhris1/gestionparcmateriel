using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.Models;

public class Materiel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string CodeBarre { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Libelle { get; set; } = string.Empty;

    public int TypeMaterielId { get; set; }

    [MaxLength(50)]
    public string? Marque { get; set; }

    [MaxLength(50)]
    public string? Modele { get; set; }

    /// <summary>Numéro de série — unique si renseigné (nullable).</summary>
    [MaxLength(100)]
    public string? NumeroSerie { get; set; }

    public DateTime DateAcquisition { get; set; }

    public int EtatId { get; set; }

    public int LocalisationId { get; set; }

    /// <summary>Employé affecté — null si le matériel est en stock.</summary>
    public int? EmployeId { get; set; }

    public bool Actif { get; set; } = true;

    public string? Commentaires { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public DateTime DateModification { get; set; } = DateTime.UtcNow;

    // ── Navigation ────────────────────────────────────────────────────────────
    public TypeMateriel TypeMateriel { get; set; } = null!;
    public EtatMateriel Etat { get; set; } = null!;
    public Localisation Localisation { get; set; } = null!;
    public Employe? Employe { get; set; }
    public ICollection<MouvementMateriel> Mouvements { get; set; } = new List<MouvementMateriel>();
}
