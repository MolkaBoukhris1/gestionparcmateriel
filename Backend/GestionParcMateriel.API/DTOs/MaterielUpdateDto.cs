using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.DTOs;

/// <summary>DTO de modification d'un matériel existant.</summary>
public class MaterielUpdateDto
{
    [Required(ErrorMessage = "Le libellé est obligatoire.")]
    [MaxLength(100)]
    public string Libelle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le type de matériel est obligatoire.")]
    public int TypeMaterielId { get; set; }

    [MaxLength(50)]
    public string? Marque { get; set; }

    [MaxLength(50)]
    public string? Modele { get; set; }

    /// <summary>Numéro de série — doit être unique si renseigné.</summary>
    [MaxLength(100)]
    public string? NumeroSerie { get; set; }

    [Required(ErrorMessage = "La date d'acquisition est obligatoire.")]
    public DateTime DateAcquisition { get; set; }

    [Required(ErrorMessage = "L'état est obligatoire.")]
    public int EtatId { get; set; }

    [Required(ErrorMessage = "La localisation est obligatoire.")]
    public int LocalisationId { get; set; }

    public int? EmployeId { get; set; }

    public string? Commentaires { get; set; }
}
