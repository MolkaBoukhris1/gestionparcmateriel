using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.Models;

/// <summary>Types de mouvement possibles pour un matériel.</summary>
public enum TypeMouvement
{
    Affectation,
    Retour,
    Transfert,
    ChangementEtat,
    Maintenance,
    MiseEnPanne,
    Reforme
}

public class MouvementMateriel
{
    public int Id { get; set; }

    public int MaterielId { get; set; }

    public DateTime DateMouvement { get; set; } = DateTime.UtcNow;

    [Required]
    public TypeMouvement TypeMouvement { get; set; }

    /// <summary>Employé concerné par ce mouvement (nullable — pas d'employé pour un transfert de salle).</summary>
    public int? EmployeId { get; set; }

    public int LocalisationId { get; set; }

    public int AncienEtatId { get; set; }

    public int NouvelEtatId { get; set; }

    public string? Commentaires { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public Materiel Materiel { get; set; } = null!;
    public Employe? Employe { get; set; }
    public Localisation Localisation { get; set; } = null!;
    public EtatMateriel AncienEtat { get; set; } = null!;
    public EtatMateriel NouvelEtat { get; set; } = null!;
}
