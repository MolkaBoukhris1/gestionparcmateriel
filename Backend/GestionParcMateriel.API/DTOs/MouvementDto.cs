using System.ComponentModel.DataAnnotations;
using GestionParcMateriel.API.Models;

namespace GestionParcMateriel.API.DTOs;

// ── Lecture historique ────────────────────────────────────────────────────────

public class MouvementMaterielDto
{
    public int Id { get; set; }
    public int MaterielId { get; set; }
    public string MaterielLibelle { get; set; } = string.Empty;
    public DateTime DateMouvement { get; set; }
    public string TypeMouvement { get; set; } = string.Empty;
    public int? EmployeId { get; set; }
    public string? EmployeNomComplet { get; set; }
    public int LocalisationId { get; set; }
    public string LocalisationNom { get; set; } = string.Empty;
    public int AncienEtatId { get; set; }
    public string AncienEtatLibelle { get; set; } = string.Empty;
    public int NouvelEtatId { get; set; }
    public string NouvelEtatLibelle { get; set; } = string.Empty;
    public string? Commentaires { get; set; }
}

// ── Affecter ──────────────────────────────────────────────────────────────────

public class AffecterMaterielDto
{
    [Required(ErrorMessage = "L'employé est obligatoire pour une affectation.")]
    public int EmployeId { get; set; }

    [Required(ErrorMessage = "La localisation est obligatoire.")]
    public int LocalisationId { get; set; }

    public string? Commentaires { get; set; }
}

// ── Retour ────────────────────────────────────────────────────────────────────

public class RetourMaterielDto
{
    [Required(ErrorMessage = "La localisation de retour est obligatoire.")]
    public int LocalisationId { get; set; }

    public string? Commentaires { get; set; }
}

// ── Transfert ─────────────────────────────────────────────────────────────────

public class TransfertMaterielDto
{
    [Required(ErrorMessage = "La nouvelle localisation est obligatoire.")]
    public int NouvelleLocalisationId { get; set; }

    public string? Commentaires { get; set; }
}
